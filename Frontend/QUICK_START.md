# MVC'den React'e Taşıma - Hızlı Başlangıç

## ⚡ 5 Dakikada Başlayın

### 1. Yeni Bir Sayfa Eklemek İçin:

```bash
# 1. Component oluştur
cd Frontend/src/components
# Yeni klasör oluştur veya dosya ekle
```

### 2. Backend API Endpoint'lerini Kontrol Et:

```csharp
// Controller'da JSON döndüren endpoint var mı?
[HttpGet]
public async Task<IActionResult> GetItems() {
    return Json(items); // ✅ Var
}
```

### 3. Basit Component Şablonu:

```jsx
// components/YeniSayfa.jsx
import { useState, useEffect } from 'react';
import { backendApi } from '../lib/api';

function YeniSayfa() {
  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function fetchData() {
      const res = await backendApi.get('/Controller/Action');
      setItems(res.data);
      setLoading(false);
    }
    fetchData();
  }, []);

  if (loading) return <div>Yükleniyor...</div>;

  return (
    <div>
      <h1>Sayfa Başlığı</h1>
      {items.map(item => (
        <div key={item.id}>{item.name}</div>
      ))}
    </div>
  );
}

export default YeniSayfa;
```

### 4. App.jsx'e Ekle:

```jsx
import YeniSayfa from './components/YeniSayfa';

function App() {
  return <YeniSayfa />;
}
```

### 5. Test Et:

```bash
# Backend çalışıyor mu?
cd Backend/StajerManager
dotnet run

# Frontend çalışıyor mu?
cd Frontend
npm run dev
```

---

## 📚 Öğrenme Yolu

1. **Basit Liste** → Departmanlar örneği
2. **Form ile Create** → Departmanlar modal
3. **Tablo + Arama** → Stajers örneği (EXAMPLES_STAJERS.md)
4. **Cascade Dropdown** → StajerModal örneği
5. **Authentication** → AuthContext oluştur
6. **Routing** → React Router ekle

---

## 🔍 Hızlı Referans

### MVC → React Çevirici:

| MVC | React |
|-----|-------|
| `@Model.Items` | `{items.map(...)}` |
| `@foreach` | `.map()` |
| `@if` | `{condition && ...}` |
| `ViewBag` | `useState()` |
| `Html.ActionLink` | `<Link>` veya `navigate()` |
| Form Submit | `onSubmit` handler |

### API Çağrıları:

```jsx
// GET
const res = await backendApi.get('/Controller/Action');
setData(res.data);

// POST
await backendApi.post('/Controller/Create', formData);

// PUT
await backendApi.put('/Controller/Edit/5', formData);

// DELETE
await backendApi.post('/Controller/Delete/5');
```

### State Yönetimi:

```jsx
// Basit state
const [value, setValue] = useState('');

// Object state
const [form, setForm] = useState({ name: '', email: '' });

// Update
setForm({ ...form, name: 'yeni değer' });
```

---

## 🆘 Sorun Giderme

### CORS Hatası:
- Backend'de CORS ayarlarını kontrol et
- `Program.cs` → `AllowFrontend` policy

### 401 Unauthorized:
- Authentication gerekiyor mu?
- Cookie'ler gönderiliyor mu? (`withCredentials: true`)

### 404 Not Found:
- Vite proxy doğru mu? (`vite.config.js`)
- Endpoint URL'i doğru mu?

### Veri Görünmüyor:
- Console'da hata var mı?
- Network tab'de response var mı?
- State güncelleniyor mu?

---

**Detaylı rehber için:** `REACT_MIGRATION_GUIDE.md` ve `EXAMPLES_STAJERS.md` dosyalarına bakın! 🚀

