using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StajerManager.Models;
using StajerManager.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext
builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password ayarları
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    // User ayarları
    options.User.RequireUniqueEmail = true;
    
    // Email confirmation ayarları - EKLE
    options.SignIn.RequireConfirmedEmail = true;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<Context>()
.AddDefaultTokenProviders();

// Cookie ayarları - Giriş yapmamış kullanıcıları login sayfasına yönlendir
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.SlidingExpiration = true;
});


builder.Services.AddScoped<IEmailService, EmailService>();


var app = builder.Build();
// Seed data
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<Context>();  
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>(); 
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        await SeedData(context, userManager, roleManager);
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Seed data sırasında hata oluştu");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();      
app.UseAuthorization();
    
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Stajers}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

static async Task SeedData(Context context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
{
    try
    {
        // Database bağlantısını test et
        await context.Database.EnsureCreatedAsync();
        
        // Departmanları kontrol et
        if (!context.Departmans.Any())
        {
            var departmanlar = new List<DepartmanModel>
            {
                new DepartmanModel { DepartmanAdi = "Bilgi İşlem", Aciklama = "IT ve yazılım geliştirme departmanı" },
                new DepartmanModel { DepartmanAdi = "İnsan Kaynakları", Aciklama = "İK ve personel yönetimi departmanı" },
                new DepartmanModel { DepartmanAdi = "Muhasebe", Aciklama = "Mali işler ve muhasebe departmanı" },
                new DepartmanModel { DepartmanAdi = "Pazarlama", Aciklama = "Pazarlama ve satış departmanı" },
                new DepartmanModel { DepartmanAdi = "Üretim", Aciklama = "Üretim ve kalite kontrol departmanı" }
            };

            context.Departmans.AddRange(departmanlar);
            await context.SaveChangesAsync();
        }
        
        // Rolleri oluştur
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }
        if (!await roleManager.RoleExistsAsync("User"))
        {
            await roleManager.CreateAsync(new IdentityRole("User"));
        }

        // Admin kullanıcısını kontrol et ve oluştur
        var adminUser = await userManager.FindByEmailAsync("admin@stajermanager.com");
        if (adminUser == null)
        {
            adminUser = new ApplicationUser  
            {
                UserName = "admin@stajermanager.com",
                Email = "admin@stajermanager.com",
                FirstName = "Admin",
                LastName = "User",
                Role = "Admin",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!"); 

            if (result.Succeeded)
            {
                // Admin rolü ekle
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
            else
            {
                throw new InvalidOperationException($"Admin kullanıcısı oluşturulamadı: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
        else
        {
            // Admin kullanıcısı varsa rolünü kontrol et
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
    catch (Exception ex)
    {
        throw new InvalidOperationException("Seed data işlemi başarısız oldu", ex);
    }
}
