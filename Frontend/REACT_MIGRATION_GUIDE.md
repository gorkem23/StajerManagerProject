# MVC'den React'e Taşıma Rehberi

Bu rehber, ASP.NET Core MVC View'larını React'e nasıl taşıyacağınızı adım adım öğretir.

## 📋 Genel Strateji

### 1. Mevcut Durum Analizi
- ✅ Backend: ASP.NET Core MVC + API endpoints
- ✅ Frontend: React + Vite başlangıç yapısı
- ✅ Departmanlar sayfası zaten React'e taşındı (örnek)

### 2. Taşıma Adımları

#### Adım 1: Backend API Endpoint'lerini Belirle
#### Adım 2: React Component Yapısını Oluştur
#### Adım 3: State Management (useState, useEffect)
#### Adım 4: API Çağrıları (axios)
#### Adım 5: Form Yönetimi ve Validasyon
#### Adım 6: Routing (React Router)
#### Adım 7: Authentication/Authorization

---

## 🔄 Dönüşüm Örnekleri

### Örnek 1: Basit Liste Sayfası

#### MVC View (Departmanlar/Index.cshtml):
```cshtml
@model IEnumerable<DepartmanModel>
@foreach (var item in Model) {
    <div>@item.DepartmanAdi</div>
}
```

#### React Component:
```jsx
function Departmanlar() {
  const [departmanlar, setDepartmanlar] = useState([]);
  
  useEffect(() => {
    api.get('/DepartmanApi').then(res => {
      setDepartmanlar(res.data);
    });
  }, []);
  
  return (
    <div>
      {departmanlar.map(d => (
        <div key={d.departmanID}>{d.departmanAdi}</div>
      ))}
    </div>
  );
}
```

### Örnek 2: Form ile Create/Edit

#### MVC View:
```cshtml
<form asp-action="Create" method="post">
    <input asp-for="DepartmanAdi" />
    <button type="submit">Kaydet</button>
</form>
```

#### React Component:
```jsx
function DepartmanForm({ editingItem, onSave }) {
  const [formData, setFormData] = useState({
    departmanAdi: editingItem?.departmanAdi || ''
  });
  
  const handleSubmit = async (e) => {
    e.preventDefault();
    await api.post('/Departman/Create', formData);
    onSave();
  };
  
  return (
    <form onSubmit={handleSubmit}>
      <input 
        value={formData.departmanAdi}
        onChange={e => setFormData({...formData, departmanAdi: e.target.value})}
      />
      <button type="submit">Kaydet</button>
    </form>
  );
}
```

### Örnek 3: Tablo ile Listeleme

#### MVC View (Razor):
```cshtml
<table>
    <thead>
        <tr><th>Ad</th><th>Email</th></tr>
    </thead>
    <tbody>
        @foreach(var item in Model) {
            <tr>
                <td>@item.FullName</td>
                <td>@item.Email</td>
            </tr>
        }
    </tbody>
</table>
```

#### React Component:
```jsx
function StajerList() {
  const [stajers, setStajers] = useState([]);
  
  return (
    <table>
      <thead>
        <tr><th>Ad</th><th>Email</th></tr>
      </thead>
      <tbody>
        {stajers.map(stajer => (
          <tr key={stajer.stajerID}>
            <td>{stajer.fullName}</td>
            <td>{stajer.email}</td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}
```

---

## 🎯 Önemli Dönüşüm Noktaları

### 1. Razor Syntax → JSX

| MVC Razor | React JSX |
|-----------|-----------|
| `@Model.Property` | `{model.property}` |
| `@foreach(var x in Model)` | `{items.map(x => ...)}` |
| `@if (condition)` | `{condition && ...}` |
| `@Html.DisplayFor()` | `{item.field}` |
| `@Url.Action()` | `'/path'` veya `useNavigate()` |
| `@User.IsInRole()` | API'den user bilgisi çek veya context |

### 2. Server-Side → Client-Side

| MVC | React |
|-----|-------|
| Page Load'da veri | `useEffect()` ile API çağrısı |
| Form Submit → Redirect | Form Submit → API → State update |
| ViewBag/ViewData | `useState()` |
| Partial Views | Component'ler |

### 3. JavaScript Kodları

MVC'de `<script>` tagları içindeki JavaScript'i:
- Custom hooks'a taşı (`useDepartmanlar.js`)
- Component içinde `useEffect` ve handler'lara dönüştür

---

## 📦 Proje Yapısı Önerisi

```
Frontend/src/
├── components/          # Reusable components
│   ├── Departmanlar/
│   ├── Stajers/
│   └── common/         # Modal, Button, Table gibi
├── pages/              # Sayfa component'leri
│   ├── Login.jsx
│   ├── Dashboard.jsx
│   └── StajersPage.jsx
├── hooks/              # Custom hooks
│   ├── useAuth.js
│   └── useStajers.js
├── services/           # API çağrıları
│   ├── departmanService.js
│   └── stajerService.js
├── context/            # React Context (Auth, Theme vb.)
└── utils/              # Helper functions
```

---

## 🔐 Authentication Taşıma

### MVC'de:
```csharp
[Authorize]
public class StajersController : Controller { }
```

### React'te:

**1. Auth Context Oluştur:**
```jsx
// context/AuthContext.jsx
const AuthContext = createContext();

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  
  const login = async (email, password) => {
    const res = await api.post('/Account/Login', { email, password });
    setUser(res.data.user);
  };
  
  return (
    <AuthContext.Provider value={{ user, login }}>
      {children}
    </AuthContext.Provider>
  );
}
```

**2. Protected Route:**
```jsx
function ProtectedRoute({ children }) {
  const { user } = useAuth();
  if (!user) return <Navigate to="/login" />;
  return children;
}
```

---

## 📝 Adım Adım Taşıma Süreci

### Her Sayfa İçin:

1. **Analiz Et:**
   - MVC View'ı incele
   - Hangi veriler gösteriliyor?
   - Hangi endpoint'ler kullanılıyor?
   - Form var mı?

2. **API Endpoint'leri Kontrol Et:**
   - Controller'da hangi endpoint'ler var?
   - JSON dönüyor mu?
   - [Authorize] var mı?

3. **Component Oluştur:**
   - State'leri belirle
   - useEffect ile veri çek
   - UI'ı JSX'e çevir

4. **Form Varsa:**
   - useState ile form state
   - onSubmit handler
   - Validation

5. **Test Et:**
   - Backend çalışıyor mu?
   - API çağrıları çalışıyor mu?
   - UI doğru görünüyor mu?

---

## 🚀 Pratik Örnek: Stajers Sayfası Taşıma

### Adım 1: Backend API'yi İncele
```csharp
// StajersController.cs
[HttpGet]
public async Task<IActionResult> GetStajers() 
{
    // JSON döndürüyor ✅
}
```

### Adım 2: React Component
```jsx
// components/Stajers/StajersList.jsx
function StajersList() {
  const [stajers, setStajers] = useState([]);
  const [loading, setLoading] = useState(true);
  
  useEffect(() => {
    async function fetchStajers() {
      try {
        const res = await api.get('/Stajers/GetStajers');
        setStajers(res.data.stajers);
      } catch (error) {
        console.error(error);
      } finally {
        setLoading(false);
      }
    }
    fetchStajers();
  }, []);
  
  if (loading) return <div>Yükleniyor...</div>;
  
  return (
    <div>
      <h1>Stajerler</h1>
      <table>
        {/* ... */}
      </table>
    </div>
  );
}
```

---

## ⚠️ Dikkat Edilmesi Gerekenler

1. **CORS:** Backend'de CORS ayarları doğru mu?
2. **Authentication:** Cookie-based auth mı? JWT mi?
3. **Anti-Forgery Token:** MVC'de `@Html.AntiForgeryToken()` varsa, backend'de kontrol et
4. **Redirect:** MVC'de `RedirectToAction()` → React'te `navigate()` kullan
5. **Validation:** Backend validation'ı koru, frontend'de de ekle

---

## 🎓 Öğrenme Kaynakları

- React Hooks: `useState`, `useEffect`, `useContext`
- Axios: API çağrıları
- React Router: Navigation
- Form Handling: Controlled components

---

## ✅ Checklist

Her sayfa için:
- [ ] Backend API endpoint'leri hazır mı?
- [ ] Component oluşturuldu mu?
- [ ] State management doğru mu?
- [ ] API çağrıları çalışıyor mu?
- [ ] Form varsa, validation var mı?
- [ ] Loading ve error states var mı?
- [ ] Styling uygun mu?
- [ ] Test edildi mi?

---

**İyi şanslar! 🚀**

