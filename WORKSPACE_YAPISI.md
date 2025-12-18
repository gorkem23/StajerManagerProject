# 📁 Workspace'de Görmeniz Gereken Yapı

## ✅ Doğru Workspace Yapısı:

```
StajerManagerProject/
│
├── 📁 Backend/
│   ├── 📁 StajerManager/          ← ✅ BURAYI GÖRMELİSİNİZ!
│   │   ├── 📄 Program.cs         ← Ana uygulama dosyası
│   │   ├── 📄 StajerManager.csproj
│   │   ├── 📁 Controllers/       ← AccountController, StajersController, vs.
│   │   ├── 📁 Models/            ← Veritabanı modelleri
│   │   ├── 📁 Views/             ← Razor view dosyaları
│   │   ├── 📁 Services/          ← EmailService, vs.
│   │   ├── 📁 Migrations/        ← Entity Framework migrations
│   │   ├── 📁 wwwroot/           ← CSS, JS, statik dosyalar
│   │   └── 📄 appsettings.json
│   │
│   └── 📄 StajerManager.sln      ← Solution dosyası
│
└── 📁 Frontend/
    ├── 📁 src/                   ← React bileşenleri
    ├── 📄 package.json
    └── 📄 vite.config.js
```

## 🎯 Önemli Noktalar:

### Backend Klasörü İçinde:
- **StajerManager/** klasörü **MUTLAKA** görünmeli
- Bu klasörün içinde:
  - ✅ Program.cs
  - ✅ ✅ StajerManager.csproj
  - ✅ Controllers/ klasörü (AccountController.cs, StajersController.cs, vs.)
  - ✅ Models/ klasörü
  - ✅ Views/ klasörü

### Eğer StajerManager Klasörünü Görmüyorsanız:

1. **Cursor/VS Code'u kapatın**
2. **File → Open Folder** ile açın
3. **Şu klasörü seçin:** `C:\Users\lercan\source\repos\StajerManagerProject\Backend`
4. Veya tam path: `C:\Users\lercan\source\repos\StajerManagerProject`

## 🔍 Kontrol Listesi:

Backend klasörünü açtığınızda şunları görmelisiniz:

- [ ] `StajerManager/` klasörü var mı?
- [ ] `StajerManager/Program.cs` dosyası var mı?
- [ ] `StajerManager/Controllers/` klasörü var mı?
- [ ] `StajerManager/Models/` klasörü var mı?
- [ ] `StajerManager/Views/` klasörü var mı?
- [ ] `StajerManager.sln` dosyası Backend klasöründe var mı?

## ⚠️ Sorun Giderme:

**Eğer Backend klasörünü açtığınızda sadece şunları görüyorsanız:**
- Controllers/ (tek başına)
- StajerManager.sln
- MANUEL_ADIMLAR.md

**VE StajerManager/ klasörünü görmüyorsanız:**
- Explorer'da `C:\Users\lercan\source\repos\StajerManagerProject\Backend` klasörünü açın
- StajerManager klasörünün orada olup olmadığını kontrol edin
- Eğer yoksa, klasör taşıma işlemi eksik kalmış demektir

## ✅ Doğru Görünüm:

Backend klasörü açıkken, sol panelde şunu görmelisiniz:
```
📁 Backend
  ├── 📁 .github
  ├── 📁 Controllers
  ├── 📁 StajerManager        ← ✅ BURASI ÖNEMLİ!
  │   ├── 📁 Controllers
  │   ├── 📁 Models
  │   ├── 📁 Views
  │   ├── 📄 Program.cs
  │   └── ...
  └── 📄 StajerManager.sln
```

