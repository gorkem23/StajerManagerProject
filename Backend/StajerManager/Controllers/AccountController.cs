using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StajerManager.Models;
using StajerManager.Services; // Email servisi için ekle
using System.ComponentModel.DataAnnotations;

namespace StajerManager.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailService _emailService; // Email servisi ekle

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger,
            IEmailService emailService) // Email servisi ekle
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailService = emailService; // Email servisi ekle
        }

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _signInManager.PasswordSignInAsync(
                        model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Kullanıcı giriş yaptı: {Email}", model.Email);
                        return RedirectToLocal(returnUrl);
                    }
                    else if (result.IsLockedOut)
                    {
                        _logger.LogWarning("Kullanıcı hesabı kilitlendi: {Email}", model.Email);
                        ModelState.AddModelError(string.Empty, "Hesabınız kilitlenmiştir. Lütfen daha sonra tekrar deneyin.");
                    }
                    else if (result.RequiresTwoFactor)
                    {
                        return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Geçersiz giriş denemesi.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Giriş sırasında hata oluştu: {Email}", model.Email);
                    ModelState.AddModelError(string.Empty, "Giriş sırasında bir hata oluştu. Lütfen tekrar deneyin.");
                }
            }

            return View(model);
        }

        // GET: Account/Register
        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                try
                {
                    var user = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Role = "User",
                        EmailConfirmed = false // Email doğrulaması gerekli
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Yeni kullanıcı oluşturuldu: {Email}", model.Email);

                        // Email confirmation token oluştur
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", 
                            new { userId = user.Id, token = token }, Request.Scheme);

                        // Email gönder
                        try
                        {
                            await _emailService.SendEmailConfirmationAsync(user.Email, callbackUrl);
                            
                            // Kullanıcıyı bilgilendir
                            TempData["Message"] = "🎉 <strong>Kayıt başarılı!</strong><br><br>" +
                                "📧 E-posta adresinize doğrulama linki gönderildi.<br>" +
                                "🔗 Lütfen e-postanızı kontrol edin ve linke tıklayarak hesabınızı aktifleştirin.<br><br>" +
                                "⚠️ <em>E-posta gelmezse spam klasörünü kontrol edin.</em>";
                            return RedirectToAction("Login");
                        }
                        catch (Exception emailEx)
                        {
                            _logger.LogError(emailEx, "Email gönderilirken hata oluştu: {Email}", user.Email);
                            
                            // Email gönderilemese bile kullanıcı oluşturuldu, manuel doğrulama için bilgi ver
                            TempData["Message"] = "✅ <strong>Kayıt başarılı!</strong><br><br>" +
                                "⚠️ Ancak e-posta gönderilemedi.<br>" +
                                "📞 Lütfen yönetici ile iletişime geçin veya tekrar deneyin.<br><br>" +
                                "🔧 <em>Teknik destek: admin@stajermanager.com</em>";
                            return RedirectToAction("Login");
                        }
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Kayıt sırasında hata oluştu: {Email}", model.Email);
                    ModelState.AddModelError(string.Empty, "Kayıt sırasında bir hata oluştu. Lütfen tekrar deneyin.");
                }
            }

            return View(model);
        }

        // GET: Account/ConfirmEmail
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                TempData["Message"] = "🎉 <strong>E-posta doğrulandı!</strong><br><br>" +
                    "✅ E-posta adresiniz başarıyla doğrulandı.<br>" +
                    "🚀 Artık hesabınıza giriş yapabilirsiniz.<br><br>" +
                    "👋 <em>Hoş geldiniz!</em>";
                return RedirectToAction("Login");
            }

            TempData["Error"] = "❌ <strong>Doğrulama başarısız!</strong><br><br>" +
                "⚠️ E-posta doğrulama işlemi başarısız oldu.<br>" +
                "🔗 Linkin süresi dolmuş olabilir veya geçersiz olabilir.<br><br>" +
                "📧 <em>Lütfen yeni bir doğrulama linki isteyin.</em>";
            return RedirectToAction("Login");
        }

        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                _logger.LogInformation("Kullanıcı çıkış yaptı.");
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Çıkış sırasında hata oluştu.");
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        // GET: Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var callbackUrl = Url.Action("ResetPassword", "Account", 
                        new { userId = user.Id, token = token }, Request.Scheme);

                    await _emailService.SendPasswordResetAsync(user.Email, callbackUrl);
                }

                // Güvenlik için her zaman başarılı mesaj göster
                TempData["Message"] = "Eğer e-posta adresiniz sistemde kayıtlıysa, şifre sıfırlama bağlantısı gönderilmiştir.";
                return RedirectToAction("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        // GET: Account/ForgotPasswordConfirmation
        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: Account/ResetPassword
        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return BadRequest();
            }

            var model = new ResetPasswordViewModel { UserId = userId, Token = token };
            return View(model);
        }

        // POST: Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                TempData["Message"] = "Şifreniz başarıyla sıfırlandı!";
                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // GET: Account/LoginWith2fa
        [HttpGet]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string? returnUrl = null)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException("İki faktörlü kimlik doğrulama kullanıcısı yüklenemedi.");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginWith2faViewModel { RememberMe = rememberMe });
        }

        // GET: Account/DebugUsers - Sadece development için
        [HttpGet]
        public async Task<IActionResult> DebugUsers()
        {
            if (!HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
            {
                return NotFound();
            }

            var users = _userManager.Users.ToList();
            var userInfo = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userInfo.Add(new
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role,
                    Roles = roles,
                    EmailConfirmed = user.EmailConfirmed
                });
            }

            return Json(userInfo);
        }

        // GET: Account/CreateAdmin - Sadece development için
        [HttpGet]
        public async Task<IActionResult> CreateAdmin()
        {
            if (!HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
            {
                return NotFound();
            }

            try
            {
                // Admin kullanıcısını kontrol et
                var existingAdmin = await _userManager.FindByEmailAsync("admin@stajermanager.com");
                if (existingAdmin != null)
                {
                    return Json(new { success = false, message = "Admin kullanıcısı zaten mevcut" });
                }

                // Admin kullanıcısını oluştur
                var adminUser = new ApplicationUser
                {
                    UserName = "admin@stajermanager.com",
                    Email = "admin@stajermanager.com",
                    FirstName = "Admin",
                    LastName = "User",
                    Role = "Admin",
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(adminUser, "Admin123!");

                if (result.Succeeded)
                {
                    // Admin rolü ekle
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                    return Json(new { success = true, message = "Admin kullanıcısı başarıyla oluşturuldu" });
                }
                else
                {
                    return Json(new { success = false, message = $"Admin kullanıcısı oluşturulamadı: {string.Join(", ", result.Errors.Select(e => e.Description))}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        // GET: Account/TestEmail - Sadece development için
        [HttpGet]
        public async Task<IActionResult> TestEmail()
        {
            if (!HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
            {
                return NotFound();
            }

            try
            {
                await _emailService.SendEmailAsync(
                    "test@example.com", 
                    "Test Email", 
                    "Bu bir test email'idir."
                );
                return Json(new { success = true, message = "Email gönderildi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Account/CheckUser - Sadece development için
        [HttpGet]
        public async Task<IActionResult> CheckUser()
        {
            if (!HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
            {
                return NotFound();
            }

            try
            {
                var userInfo = new
                {
                    IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
                    UserName = User.Identity?.Name,
                    IsAdmin = User.IsInRole("Admin"),
                    Claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
                };

                return Json(userInfo);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}
