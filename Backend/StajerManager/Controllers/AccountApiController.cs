using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StajerManager.Models;
using StajerManager.Services;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace StajerManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountApiController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountApiController> _logger;
        private readonly IEmailService _emailService;

        public AccountApiController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<AccountApiController> logger,
            IEmailService emailService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _emailService = emailService;
        }

        // Login API Endpoint
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { 
                    success = false, 
                    message = "Geçersiz model", 
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            try
            {
                // Email ile kullanıcıyı bul
                var user = await _userManager.FindByEmailAsync(model.Email);
                
                if (user == null)
                {
                    _logger.LogWarning("Login attempt with non-existent email: {Email}", model.Email);
                    return Unauthorized(new { success = false, message = "Email veya şifre hatalı" });
                }

                // Kullanıcıyı bulduktan sonra şifre kontrolü yap
                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName, // PasswordSignInAsync username bekler
                    model.Password, 
                    model.RememberMe, 
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User {Email} logged in successfully", model.Email);
                    var roles = await _userManager.GetRolesAsync(user);
                    var role = roles.FirstOrDefault() ?? user.Role ?? "User";
                    var isAdmin = roles.Contains("Admin") || role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
                    
                    return Ok(new { success = true, message = "Giriş başarılı", user = new{
                        userName = user.UserName,
                        email = user.Email,
                        role = role,
                        isAdmin = isAdmin
                    }});
                    
                }
                
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User {Email} account locked out", model.Email);
                    return BadRequest(new { success = false, message = "Hesap kilitlendi" });
                }
                
                if (result.IsNotAllowed)
                {
                    return BadRequest(new { success = false, message = "Email doğrulanmamış" });
                }

                _logger.LogWarning("Failed login attempt for {Email}", model.Email);
                return Unauthorized(new { success = false, message = "Email veya şifre hatalı" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", model.Email);
                return StatusCode(500, new { success = false, message = "Bir hata oluştu" });
            }
        }

        // Get Current User - Authentication durumunu kontrol et
        [HttpGet("GetCurrentUser")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var email = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity.Name;
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    return Unauthorized(new { isAuthenticated = false });
                }

                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? user.Role ?? "User";
                var isAdmin = roles.Contains("Admin") || role.Equals("Admin", StringComparison.OrdinalIgnoreCase);

                return Ok(new
                {
                    isAuthenticated = true,
                    userName = user.UserName,
                    email = user.Email,
                    role,
                    isAdmin
                });
            }

            return Unauthorized(new { isAuthenticated = false });
        }

        // Register API Endpoint
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { 
                    success = false, 
                    message = "Geçersiz model", 
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            try
            {
                // Şifrelerin eşleşip eşleşmediğini kontrol et
                if (model.Password != model.ConfirmPassword)
                {
                    return BadRequest(new { success = false, message = "Şifreler eşleşmiyor" });
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Role = "User",
                    EmailConfirmed = false
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
                        return Ok(new { 
                            success = true, 
                            message = "Kayıt başarılı! E-posta adresinize doğrulama linki gönderildi." 
                        });
                    }
                    catch (Exception emailEx)
                    {
                        _logger.LogError(emailEx, "Email gönderilirken hata oluştu: {Email}", user.Email);
                        return Ok(new { 
                            success = true, 
                            message = "Kayıt başarılı ancak e-posta gönderilemedi. Lütfen yönetici ile iletişime geçin." 
                        });
                    }
                }

                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(new { 
                    success = false, 
                    message = "Kayıt başarısız", 
                    errors = errors 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for {Email}", model.Email);
                return StatusCode(500, new { success = false, message = "Kayıt sırasında bir hata oluştu" });
            }
        }

        // Logout API Endpoint
        [HttpPost("Logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                _logger.LogInformation("User logged out");
                return Ok(new { success = true, message = "Çıkış başarılı" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return StatusCode(500, new { success = false, message = "Çıkış yapılırken bir hata oluştu" });
            }
        }
    }

    // Login Model
    public class LoginModel
    {
        [Required(ErrorMessage = "Email gereklidir")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre gereklidir")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; } = false;
    }

    // Register Model
    public class RegisterModel
    {
        [Required(ErrorMessage = "Ad gereklidir")]
        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad gereklidir")]
        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta adresi gereklidir")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre gereklidir")]
        [StringLength(100, ErrorMessage = "Şifre en az {2} karakter olmalıdır.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre tekrar gereklidir")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

