# Backend ve Frontend Klasörlerini Ayırma Rehberi

## Yapılacaklar

### 1. Yeni Klasör Yapısı Oluşturma

Workspace'inizde yeni bir ana klasör oluşturun ve içine backend ve frontend klasörlerini taşıyın.

**Mevcut yapı:**
```
C:\Users\lercan\source\repos\
  ├── StajerManager\          (Backend - ASP.NET Core)
  └── StajerManagerVite\  (Frontend - React + Vite)
```

**Yeni yapı (Önerilen):**
```
C:\Users\lercan\source\repos\
  └── StajerManagerProject\
      ├── backend\             (StajerManager içeriği)
      └── frontend\            (StajerManagerVite içeriği)
```

### 2. Adımlar

#### PowerShell ile (Önerilen):

```powershell
# Ana klasör oluştur
cd C:\Users\lercan\source\repos
New-Item -ItemType Directory -Path "StajerManagerProject"
New-Item -ItemType Directory -Path "StajerManagerProject\backend"
New-Item -ItemType Directory -Path "StajerManagerProject\frontend"

# Backend'i taşı
Move-Item -Path "StajerManager\*" -Destination "StajerManagerProject\backend\" -Force
Move-Item -Path "StajerManager\.git" -Destination "StajerManagerProject\backend\" -ErrorAction SilentlyContinue

# Frontend'i taşı
Move-Item -Path "StajerManagerVite\*" -Destination "StajerManagerProject\frontend\" -Force
Move-Item -Path "StajerManagerVite\.git" -Destination "StajerManagerProject\frontend\" -ErrorAction SilentlyContinue

# Eski klasörleri sil (dikkatli olun!)
Remove-Item -Path "StajerManager" -Recurse -Force
Remove-Item -Path "StajerManagerVite" -Recurse -Force
```

#### Manuel Adımlar:

1. **Yeni klasör oluştur:**
   - `C:\Users\lercan\source\repos\StajerManagerProject` klasörünü oluşturun
   - İçine `backend` ve `frontend` klasörlerini oluşturun

2. **Backend'i taşı:**
   - `StajerManager` klasöründeki tüm dosyaları `StajerManagerProject\backend\` içine taşıyın

3. **Frontend'i taşı:**
   - `StajerManagerVite` klasöründeki tüm dosyaları `StajerManagerProject\frontend\` içine taşıyın

### 3. Backend CORS Yapılandırması

Backend'in frontend'den gelen istekleri kabul edebilmesi için CORS eklenmelidir.

**Program.cs'e eklenecek kod:**

```csharp
// Add services bölümüne ekle (builder.Services.AddControllersWithViews() sonrasına)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder =>
        {
            builder.WithOrigins("http://localhost:5173")  // Vite default port
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
        });
});

// Configure pipeline bölümüne ekle (app.UseRouting() öncesine)
app.UseCors("AllowFrontend");
```

### 4. Frontend Vite Config Kontrolü

`vite.config.js` zaten proxy yapılandırmasına sahip. Backend portunu kontrol edin:
- Backend default port: 5003 (veya appsettings.json'daki port)
- Frontend proxy: `http://localhost:5003` olarak ayarlı

### 5. Git Yapılandırması (Opsiyonel)

Eğer her klasör için ayrı git repo istiyorsanız:
- `backend\.git` klasörü zaten mevcut
- `frontend` için yeni bir git repo başlatabilirsiniz

Eğer tek bir repo'da tutmak istiyorsanız:
- Ana klasörde `.gitignore` oluşturun ve her iki projeyi yönetin

### 6. Workspace'i Güncelleme

Cursor/VS Code'da workspace ayarlarını güncelleyin:
- Yeni workspace path: `C:\Users\lercan\source\repos\StajerManagerProject`
- Veya her iki klasörü ayrı workspace olarak açın

## Kontrol Listesi

- [ ] Yeni klasör yapısı oluşturuldu
- [ ] Backend dosyaları taşındı
- [ ] Frontend dosyaları taşındı
- [ ] CORS eklendi (Program.cs)
- [ ] Vite config port kontrolü yapıldı
- [ ] Backend ve frontend test edildi
- [ ] Git yapılandırması güncellendi

## Sonraki Adımlar

1. Backend'i çalıştırın: `cd backend/StajerManager && dotnet run`
2. Frontend'i çalıştırın: `cd frontend && npm run dev`
3. Her iki uygulamanın birbiriyle iletişim kurduğunu test edin

