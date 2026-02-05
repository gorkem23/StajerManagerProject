# Manuel Klasör Ayırma Rehberi - Adım Adım

Bu rehber, Windows File Explorer ve PowerShell kullanarak backend ve frontend klasörlerini manuel olarak ayırmanız için hazırlanmıştır.

## 📋 Genel Bakış

**Mevcut Durum:**
- `C:\Users\lercan\source\repos\StajerManager` → Backend (ASP.NET Core)
- `C:\Users\lercan\source\repos\StajerManagerVite` → Frontend (React + Vite)

**Hedef Yapı:**
- `C:\Users\lercan\source\repos\StajerManagerProject\backend` → Backend
- `C:\Users\lercan\source\repos\StajerManagerProject\frontend` → Frontend

---

## 🚀 Yöntem 1: File Explorer ile (Görsel - Önerilen)

### Adım 1: Yeni Ana Klasör Oluştur

1. Windows Dosya Gezgini'ni açın (`Win + E`)
2. Şu adrese gidin: `C:\Users\lercan\source\repos`
3. Sağ tıklayın → **Yeni** → **Klasör**
4. Klasör adını `StajerManagerProject` yapın
5. İçine girin (çift tıklayın)

### Adım 2: Backend ve Frontend Klasörlerini Oluştur

1. İçinde sağ tıklayın → **Yeni** → **Klasör** → Ad: `backend`
2. Tekrar sağ tıklayın → **Yeni** → **Klasör** → Ad: `frontend`

Şu an yapınız:
```
C:\Users\lercan\source\repos\StajerManagerProject\
  ├── backend\    (boş)
  └── frontend\   (boş)
```

### Adım 3: Backend Dosyalarını Taşı

1. File Explorer'da `C:\Users\lercan\source\repos\StajerManager` klasörüne gidin
2. **Tüm dosyaları seçin:**
   - `Ctrl + A` (Tümünü seç)
   - VEYA `Ctrl` tuşuna basılı tutarak dosyaları tek tek seçin
3. **Kes:**
   - `Ctrl + X` (Kes)
   - VEYA Sağ tık → **Kes**
4. **Yapıştır:**
   - `C:\Users\lercan\source\repos\StajerManagerProject\backend` klasörüne gidin
   - `Ctrl + V` (Yapıştır)
   - VEYA Sağ tık → **Yapıştır**

⚠️ **Önemli:** `.git` klasörü varsa, onu da taşımayı unutmayın (gizli klasör olabilir)

#### Git Klasörünü Taşıma (Özel Adım)

Eğer `.git` klasörü normal taşıma sırasında aktarılmadıysa:

**Yöntem A: File Explorer ile (Görsel)**

1. File Explorer'da **Görünüm** sekmesine tıklayın
2. Sağ üstte **Gizli öğeler** kutusunu işaretleyin (✓)
3. Artık `.git` klasörü görünecektir (yarı saydam görünür)
4. `C:\Users\lercan\source\repos\StajerManager` klasörüne gidin
5. `.git` klasörünü bulun
6. `.git` klasörüne **sağ tıklayın** → **Kes** (`Ctrl + X`)
7. `C:\Users\lercan\source\repos\StajerManagerProject\backend` klasörüne gidin
8. **Yapıştır** (`Ctrl + V`)

**Yöntem B: PowerShell ile (Hızlı)**

PowerShell'i açın ve şu komutu çalıştırın:

```powershell
# Eğer .git klasörü hala eski yerdeyse:
cd C:\Users\lercan\source\repos\StajerManager
if (Test-Path .git) {
    Move-Item -Path ".git" -Destination "C:\Users\lercan\source\repos\StajerManagerProject\backend\" -Force
    Write-Host ".git klasörü başarıyla taşındı!" -ForegroundColor Green
} else {
    Write-Host ".git klasörü bulunamadı (belki zaten taşınmış)" -ForegroundColor Yellow
}

# Kontrol et:
cd C:\Users\lercan\source\repos\StajerManagerProject\backend
if (Test-Path .git) {
    Write-Host "✓ .git klasörü yeni konumda mevcut" -ForegroundColor Green
} else {
    Write-Host "✗ .git klasörü bulunamadı" -ForegroundColor Red
}
```

**Yöntem C: Git Repository'yi Yeniden Başlatma (En Kolay)**

Eğer `.git` klasörünü bulamıyorsanız veya sorun yaşıyorsanız:

```powershell
cd C:\Users\lercan\source\repos\StajerManagerProject\backend

# Yeni git repository başlat
git init

# Eğer remote varsa ekleyin (opsiyonel):
# git remote add origin <repository-url>

# Mevcut dosyaları ekle
git add .

# İlk commit
git commit -m "Initial commit - Project reorganization"
```

### Adım 4: Frontend Dosyalarını Taşı

1. File Explorer'da `C:\Users\lercan\source\repos\StajerManagerVite` klasörüne gidin
2. **Tüm dosyaları seçin:**
   - `Ctrl + A`
3. **Kes:**
   - `Ctrl + X`
4. **Yapıştır:**
   - `C:\Users\lercan\source\repos\StajerManagerProject\frontend` klasörüne gidin
   - `Ctrl + V`

### Adım 5: Gizli Dosyaları Kontrol Et

**File Explorer'da gizli dosyaları görmek için:**
1. Üst menüden **Görünüm** sekmesine tıklayın
2. **Gizli öğeler** kutusunu işaretleyin
3. `.git`, `.gitignore` gibi dosyalar görünecek
4. Bunları da taşımayı unutmayın!

### Adım 6: Eski Klasörleri Sil

1. `C:\Users\lercan\source\repos\StajerManager` klasörüne gidin
2. Klasörün **boş olduğundan emin olun**
3. Klasöre sağ tıklayın → **Sil** (veya `Delete` tuşu)
4. Aynısını `StajerManagerVite` klasörü için de yapın

⚠️ **Dikkat:** Önce klasörlerin boş olduğundan emin olun!

---

## 💻 Yöntem 2: PowerShell ile (Hızlı Komutlar)

PowerShell'i **Yönetici olarak çalıştırın** (`Win + X` → Windows PowerShell (Yönetici))

### Komutları Sırayla Çalıştırın:

```powershell
# 1. Repos klasörüne git
cd C:\Users\lercan\source\repos

# 2. Ana klasör yapısını oluştur
New-Item -ItemType Directory -Path "StajerManagerProject" -Force
New-Item -ItemType Directory -Path "StajerManagerProject\backend" -Force
New-Item -ItemType Directory -Path "StajerManagerProject\frontend" -Force

# 3. Backend dosyalarını taşı (tüm içerik)
Get-ChildItem -Path "StajerManager" -Force | Move-Item -Destination "StajerManagerProject\backend\" -Force

# 4. Frontend dosyalarını taşı (tüm içerik)
Get-ChildItem -Path "StajerManagerVite" -Force | Move-Item -Destination "StajerManagerProject\frontend\" -Force

# 5. Eski boş klasörleri kontrol et ve sil
# Önce kontrol edin:
Get-ChildItem -Path "StajerManager" -Force
Get-ChildItem -Path "StajerManagerVite" -Force

# Eğer boşlarsa:
Remove-Item -Path "StajerManager" -Recurse -Force
Remove-Item -Path "StajerManagerVite" -Recurse -Force
```

---

## ✅ Kontrol Listesi

İşlem bittikten sonra şunları kontrol edin:

### Backend Kontrolleri:
- [ ] `C:\Users\lercan\source\repos\StajerManagerProject\backend\StajerManager` klasörü var
- [ ] İçinde `Program.cs`, `Controllers`, `Models`, `Views` klasörleri var
- [ ] `.csproj` dosyası var
- [ ] `.git` klasörü var (eğer Git kullanıyorsanız)

### Frontend Kontrolleri:
- [ ] `C:\Users\lercan\source\repos\StajerManagerProject\frontend` klasörü var
- [ ] İçinde `package.json`, `vite.config.js` dosyaları var
- [ ] `src` klasörü var
- [ ] `node_modules` klasörü var (yoksa `npm install` çalıştırın)

---

## 🔧 Workspace'i Güncelleme

### Cursor/VS Code için:

1. **Cursor'u kapatın**
2. Yeni workspace'i açın:
   - `File` → `Open Folder`
   - `C:\Users\lercan\source\repos\StajerManagerProject` seçin
3. VEYA her iki klasörü ayrı workspace olarak açın:
   - Backend: `C:\Users\lercan\source\repos\StajerManagerProject\backend`
   - Frontend: `C:\Users\lercan\source\repos\StajerManagerProject\frontend`

---

## 🧪 Test Etme

### 1. Backend Test:

```powershell
cd C:\Users\lercan\source\repos\StajerManagerProject\backend\StajerManager
dotnet restore
dotnet build
dotnet run
```

Backend `http://localhost:5203` adresinde çalışmalı.

### 2. Frontend Test:

```powershell
cd C:\Users\lercan\source\repos\StajerManagerProject\frontend

# Eğer node_modules yoksa:
npm install

# Frontend'i başlat:
npm run dev
```

Frontend `http://localhost:5173` adresinde çalışmalı.

### 3. Birlikte Test:

- Backend'i bir terminalde çalıştırın
- Frontend'i başka bir terminalde çalıştırın
- Frontend'den backend API'lerine istek atabildiğinizi kontrol edin

---

## ⚠️ Olası Sorunlar ve Çözümleri

### Sorun 1: "Dosya kullanımda" hatası
**Çözüm:** 
- Tüm IDE'leri (Cursor, VS Code) kapatın
- Çalışan backend/frontend process'lerini durdurun
- Tekrar deneyin

### Sorun 2: ".git klasörü taşınmadı"
**Çözüm:**
- File Explorer'da **Görünüm** → **Gizli öğeler** aktif edin
- `.git` klasörünü manuel olarak taşıyın

### Sorun 3: "node_modules taşınmıyor" (çok büyük)
**Çözüm:**
- `node_modules`'i silin (taşımanıza gerek yok)
- Frontend klasörüne gidin: `cd frontend`
- Yeniden yükleyin: `npm install`

### Sorun 4: "Backend çalışmıyor"
**Çözüm:**
- `launchSettings.json` içindeki port ayarlarını kontrol edin
- `appsettings.json` içindeki connection string'i kontrol edin
- Database migration'ları çalıştırın: `dotnet ef database update`

---

## 📝 Özet Komutlar (Hızlı Referans)

```powershell
# Yapıyı oluştur
New-Item -ItemType Directory -Path "C:\Users\lercan\source\repos\StajerManagerProject\backend"
New-Item -ItemType Directory -Path "C:\Users\lercan\source\repos\StajerManagerProject\frontend"

# Dosyaları taşı
Move-Item -Path "C:\Users\lercan\source\repos\StajerManager\*" -Destination "C:\Users\lercan\source\repos\StajerManagerProject\backend\" -Force
Move-Item -Path "C:\Users\lercan\source\repos\StajerManagerVite\*" -Destination "C:\Users\lercan\source\repos\StajerManagerProject\frontend\" -Force
```

---

## ✅ İşlem Tamamlandı!

Artık projeniz şu yapıda olmalı:

```
C:\Users\lercan\source\repos\
└── StajerManagerProject\
    ├── backend\
    │   └── StajerManager\
    │       ├── Controllers\
    │       ├── Models\
    │       ├── Views\
    │       ├── Program.cs
    │       └── ...
    └── frontend\
        ├── src\
        ├── package.json
        ├── vite.config.js
        └── ...
```

Başarılar! 🎉

