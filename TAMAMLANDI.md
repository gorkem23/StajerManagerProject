# ✅ Proje Yeniden Yapılandırması Tamamlandı!

## 📁 Yeni Klasör Yapısı

```
StajerManagerProject/
├── backend/
│   ├── StajerManager/          (Ana ASP.NET Core projesi)
│   │   ├── Controllers/
│   │   ├── Models/
│   │   ├── Views/
│   │   ├── Program.cs           (CORS yapılandırması eklendi ✓)
│   │   └── ...
│   ├── .git                     (Git repository ✓)
│   └── StajerManager.sln        (Solution dosyası ✓)
│
└── frontend/
    ├── src/                     (React bileşenleri ✓)
    │   ├── App.jsx
    │   ├── main.jsx
    │   └── lib/api.js
    ├── package.json             ✓
    ├── vite.config.js           (Backend port: 5203 ✓)
    └── ...
```

## ✅ Yapılan İşlemler

1. ✅ **Backend taşındı**: `StajerManager/StajerManager` → `backend/StajerManager`
2. ✅ **Frontend taşındı**: `StajerManagerVite` → `frontend`
3. ✅ **Git klasörü**: Backend'de mevcut
4. ✅ **CORS yapılandırması**: Program.cs'e eklendi (localhost:5173)
5. ✅ **Vite config**: Backend portu 5203 olarak ayarlandı
6. ✅ **Solution dosyası**: Taşındı

## 🚀 Nasıl Çalıştırılır?

### Backend'i Çalıştırma:

```powershell
cd C:\Users\lercan\source\repos\StajerManagerProject\backend\StajerManager
dotnet run
```

Backend `http://localhost:5203` adresinde çalışacak.

### Frontend'i Çalıştırma:

```powershell
cd C:\Users\lercan\source\repos\StajerManagerProject\frontend
npm install    # İlk kez çalıştırıyorsanız
npm run dev
```

Frontend `http://localhost:5173` adresinde çalışacak ve backend'e otomatik bağlanacak.

## ⚙️ Yapılandırma Detayları

### Backend (Program.cs)
- CORS ayarı: `http://localhost:5173` (frontend portu)
- Backend portu: `5203` (launchSettings.json)

### Frontend (vite.config.js)
- Frontend portu: `5173`
- API proxy: `/api` → `http://localhost:5203`

## 📝 Notlar

- Eski `StajerManager` ve `StajerManagerVite` klasörleri boşsa silinebilir
- `.git` klasörü backend'de mevcut
- Tüm dosyalar başarıyla taşındı
- Proje çalışır durumda! 🎉

## 🔍 Sorun Giderme

### Backend çalışmıyorsa:
- `dotnet restore` çalıştırın
- `dotnet build` ile derleyin
- Database connection string'i kontrol edin

### Frontend çalışmıyorsa:
- `npm install` çalıştırın
- `node_modules` klasörünü silip tekrar `npm install` yapın
- Port 5173'ün açık olduğundan emin olun

### CORS hatası alıyorsanız:
- Backend'in çalıştığından emin olun
- Frontend portunun 5173 olduğunu kontrol edin
- Program.cs'deki CORS ayarlarını kontrol edin

