---
marp: true
theme: default
paginate: true
header: 'Stajer Manager - Stajerlerin Ekrana Yazdırılması'
footer: '© 2024 Stajer Manager'
---

<style>
/* Kod blokları için genel stil - Beyaz arkaplan için optimize ve BELİRGİN */
pre {
  background: #1e1e1e !important;
  color: #d4d4d4 !important;
  padding: 1.5em !important;
  border-radius: 0.6em !important;
  overflow-x: auto !important;
  border: 3px solid #007acc !important;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.25) !important;
  font-size: 1em !important;
  line-height: 1.8 !important;
  font-weight: 500 !important;
  margin: 1.2em 0 !important;
}

pre code {
  background: transparent !important;
  padding: 0 !important;
  color: inherit !important;
  font-family: 'Consolas', 'Monaco', 'Courier New', monospace !important;
  font-size: 1.05em !important;
  font-weight: 500 !important;
  letter-spacing: 0.3px !important;
}

/* Inline kod - Beyaz arkaplan için BELİRGİN */
code {
  background: #fff3cd !important;
  padding: 0.3em 0.6em !important;
  border-radius: 0.4em !important;
  font-size: 0.95em !important;
  color: #b91c1c !important;
  font-family: 'Consolas', 'Monaco', 'Courier New', monospace !important;
  border: 2px solid #ffc107 !important;
  font-weight: 600 !important;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1) !important;
}

/* JavaScript/JSX Syntax Highlighting - Yüksek kontrast ve BELİRGİN */
.hljs-keyword,
.hljs-selector-tag,
.hljs-built_in,
.hljs-name,
.hljs-tag { 
  color: #4fc3f7 !important; 
  font-weight: 700 !important;
  text-shadow: 0 0 2px rgba(79, 195, 247, 0.3) !important;
}

.hljs-string,
.hljs-title,
.hljs-section,
.hljs-attribute { 
  color: #ff9800 !important; 
  font-weight: 600 !important;
  text-shadow: 0 0 2px rgba(255, 152, 0, 0.2) !important;
}

.hljs-comment,
.hljs-quote,
.hljs-doctag { 
  color: #81c784 !important; 
  font-style: italic !important;
  font-weight: 500 !important;
  opacity: 0.9 !important;
}

.hljs-function,
.hljs-title.function_ { 
  color: #ffeb3b !important; 
  font-weight: 700 !important;
  text-shadow: 0 0 3px rgba(255, 235, 59, 0.4) !important;
}

.hljs-variable,
.hljs-template-variable,
.hljs-type,
.hljs-selector-class,
.hljs-selector-attr,
.hljs-selector-pseudo { 
  color: #64b5f6 !important; 
  font-weight: 600 !important;
}

.hljs-number,
.hljs-literal { 
  color: #a5d6a7 !important; 
  font-weight: 600 !important;
}

.hljs-class,
.hljs-title.class_ { 
  color: #26c6da !important; 
  font-weight: 700 !important;
  text-shadow: 0 0 2px rgba(38, 198, 218, 0.3) !important;
}

.hljs-attr,
.hljs-property { 
  color: #64b5f6 !important; 
  font-weight: 600 !important;
}

.hljs-operator { 
  color: #ffffff !important; 
  font-weight: 600 !important;
}

.hljs-punctuation { 
  color: #e0e0e0 !important; 
  font-weight: 500 !important;
}

.hljs-meta,
.hljs-meta-keyword { 
  color: #4fc3f7 !important; 
  font-weight: 700 !important;
}

.hljs-built_in-name { 
  color: #26c6da !important; 
  font-weight: 700 !important;
}

/* C# özel renkler */
.hljs-keyword.hljs-built_in { 
  color: #4fc3f7 !important; 
  font-weight: 700 !important;
}

/* Text/Plain kod blokları */
pre code.hljs {
  display: block;
  overflow-x: auto;
  padding: 1em;
  font-weight: 500 !important;
}

/* Genel sayfa okunurluğu */
section {
  color: #333 !important;
}

h1, h2, h3 {
  color: #1a1a1a !important;
  font-weight: 700 !important;
}

/* Kod bloklarını daha belirgin yapmak için ek stiller */
pre:hover {
  border-color: #005a9e !important;
  box-shadow: 0 6px 16px rgba(0, 0, 0, 0.3) !important;
  transform: translateY(-1px) !important;
  transition: all 0.2s ease !important;
}

/* Syntax elementlerini daha belirgin yap */
.hljs-keyword,
.hljs-function,
.hljs-class {
  font-size: 1.05em !important;
}
</style>

# Stajerlerin Ekrana Yazdırılması
## Teknik Dokümantasyon

**Stajer Manager Projesi**

---

## İçindekiler

1. Genel Bakış
2. Routing ve Component Yapısı
3. Backend API Katmanı
4. Frontend Service Katmanı
5. State Yönetimi ve Filtreleme
6. Render İşlemi
7. Veri Akış Diyagramı
8. Önemli Teknik Detaylar

---

## 1. Genel Bakış

### Mimari Yapı

- **Frontend**: React.js (SPA)
- **Backend**: ASP.NET Core Web API
- **Veritabanı**: Entity Framework Core
- **HTTP İletişimi**: Axios

### Temel Akış

```text
Kullanıcı → React Component → Service → API → Controller → Database
```

---

## 2. Routing ve Component Yapısı

### Route Tanımı (App.jsx)

```jsx
// React Router import edilir
import { Routes, Route } from 'react-router-dom'
// Stajerler sayfası component'i import edilir
import StajersPage from './pages/StajersPage'

// Ana uygulama component'i
function App() {
  return (
    <Routes>
      {/* /stajers path'i için route tanımı */}
      <Route
        path="/stajers"
        element={
          {/* PrivateRoute ile korumalı sayfa */}
          <PrivateRoute>
            <StajersPage />
          </PrivateRoute>
        }
      />
    </Routes>
  )
}
```

### PrivateRoute Component

```jsx
// PrivateRoute: Sadece giriş yapmış kullanıcıların erişebileceği route'lar için
function PrivateRoute({ children }) {
  // Auth context'ten kullanıcı bilgisi ve yükleme durumu alınır
  const { user, loading } = useAuth()

  // Eğer auth kontrolü hala devam ediyorsa loading göster
  if (loading) {
    return <div>Yükleniyor...</div>
  }
  
  // Kullanıcı varsa children'ı render et, yoksa login sayfasına yönlendir
  return user ? children : <Navigate to="/login" replace />
}
```

### Component Hiyerarşisi

- `App.jsx` → Routing yönetimi
- `StajersPage.jsx` → Ana sayfa component'i
- `stajerService.js` → API çağrıları
- `StajersApiController.cs` → Backend endpoint

---

## 3. Backend API Katmanı

### Controller Endpoint (StajersApiController.cs)

```csharp
// API Controller attribute'u - RESTful API endpoint'leri için
[ApiController]
// Route tanımı: api/StajersApi şeklinde erişilir
[Route("api/[controller]")]
public class StajersApiController : ControllerBase
{
    // Entity Framework DbContext - Veritabanı erişimi için
    private readonly Context _context;
    // Logging için logger instance'ı
    private readonly ILogger<StajersApiController> _logger;

    // GET endpoint - Tüm stajerleri getirir
    [HttpGet]
    [AllowAnonymous] // Herkes erişebilir (authentication gerekmez)
    public async Task<IActionResult> GetStajers(
        [FromQuery] string? sortBy,      // Sıralama kolonu
        [FromQuery] string? sortOrder,  // Sıralama yönü (asc/desc)
        [FromQuery] string? searchText) // Arama metni
    {
        // Varsayılan sıralama: StajerID'ye göre
        sortBy ??= "StajerID";
        // Sıralama yönü kontrolü: "asc" değilse "desc"
        sortOrder = (sortOrder?.ToLower() == "asc") ? "asc" : "desc";

        // Entity Framework query başlatılır
        // Include ile ilişkili tablolar (Departman, Universite, Bolum) yüklenir
        var query = _context.Stajers
            .Include(s => s.Departman)    // Departman bilgisi eager loading
            .Include(s => s.Universite)   // Universite bilgisi eager loading
            .Include(s => s.Bolum)        // Bolum bilgisi eager loading
            .AsQueryable();              // IQueryable'a dönüştür (lazy evaluation)

        // Arama filtresi: Eğer arama metni varsa
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var lower = searchText.Trim().ToLower();
            // Ad, email, telefon, üniversite, bölüm, departman alanlarında arama yap
            query = query.Where(s =>
                s.FullName.ToLower().Contains(lower) ||
                s.Email.ToLower().Contains(lower) ||
                s.PhoneNumber.Contains(lower) ||
                (s.Universite != null && s.Universite.UniversiteAdi.ToLower().Contains(lower)) ||
                (s.Bolum != null && s.Bolum.BolumAdi.ToLower().Contains(lower)) ||
                (s.Departman != null && s.Departman.DepartmanAdi.ToLower().Contains(lower))
            );
        }

        // Sıralama: Pattern matching ile dinamik sıralama
        query = (sortBy, sortOrder) switch
        {
            ("FullName", "asc") => query.OrderBy(s => s.FullName),
            ("FullName", "desc") => query.OrderByDescending(s => s.FullName),
            ("StajerID", "asc") => query.OrderBy(s => s.StajerID),
            _ => query.OrderByDescending(s => s.StajerID) // Varsayılan: ID'ye göre azalan
        };

        // Query'yi veritabanında çalıştır ve liste olarak al
        var stajers = await query.ToListAsync();
        // Entity'leri DTO'ya map et
        var response = stajers.Select(MapStajerListItem).ToList();

        // JSON response döndür
        return Ok(new
        {
            success = true,
            stajers = response,
            totalCount = response.Count
        });
    }
}
```

### İşlem Adımları

1. **Include İlişkileri**: Departman, Universite, Bolum
2. **Filtreleme**: SearchText varsa WHERE koşulu
3. **Sıralama**: SortBy ve SortOrder'a göre
4. **Mapping**: Entity → DTO dönüşümü
5. **Response**: JSON formatında dönüş

---

## 4. Frontend Service Katmanı

### API Service (stajerService.js)

```javascript
// Axios instance'ı import edilir (baseURL, headers vb. yapılandırılmış)
import { api } from '../lib/api.js'

// Stajer API işlemleri için service objesi
export const stajerService = {
  // Tüm stajerleri getir (arama ve sıralama ile)
  // Parametreler: sortBy (sıralama kolonu), sortOrder (yön), searchText (arama metni)
  getAll: async (sortBy = 'StajerID', sortOrder = 'desc', searchText = '') => {
    // GET isteği ile backend'den veri çekilir
    const response = await api.get('/StajersApi', {
      params: {
        sortBy,        // Query parametresi: Hangi kolona göre sıralanacak
        sortOrder,      // Query parametresi: Artan (asc) veya azalan (desc)
        searchText: searchText || ''  // Query parametresi: Arama metni (boş string default)
      }
    })
    // Response'dan data kısmı döndürülür
    return response.data
  },

  // Tek bir stajer getir (ID'ye göre)
  getById: async (id) => {
    // GET isteği: /StajersApi/{id} endpoint'ine istek atılır
    const response = await api.get(`/StajersApi/${id}`)
    // Response yapısı kontrol edilir (data.data veya data)
    return response.data?.data || response.data
  },

  // Yeni stajer oluştur
  create: async (stajer) => {
    // POST isteği ile yeni stajer oluşturulur
    const response = await api.post('/StajersApi', stajer)
    // Oluşturulan stajer bilgisi döndürülür
    return response.data
  }
}

// Default export: Service objesini dışa aktar
export default stajerService
```

### Axios Konfigürasyonu (lib/api.js)

```javascript
// Axios HTTP client kütüphanesi import edilir
import axios from 'axios'

// Axios instance oluşturulur (yapılandırılmış HTTP client)
const api = axios.create({
  baseURL: 'http://localhost:5203/api',  // Backend API base URL'i
  withCredentials: true,                   // Cookie'lerin gönderilmesi için (authentication)
  headers: {
    'Content-Type': 'application/json'    // Request body formatı: JSON
  }
})

// Default export: api instance'ı dışa aktar
export default api
// Named export: api instance'ı dışa aktar
export { api }
```

### Özellikler

- **Base URL**: `http://localhost:5203/api`
- **Credentials**: `withCredentials: true` (Cookie desteği)
- **Content-Type**: `application/json`
- **Interceptors**: Request/Response interceptors eklenebilir

---

## 5. State Yönetimi

### State Tanımlamaları (StajersPage.jsx)

```jsx
// React hooks import edilir
import { useState, useEffect, useCallback } from 'react'
// Stajer API service'i import edilir
import stajerService from '../services/stajerService.js'
// Aktif stajer kontrolü için utility fonksiyonu import edilir
import { isStajActive } from '../utils/stajerUtils.js'

// Ana stajerler sayfası component'i
function StajersPage() {
  // Veri state'leri - Component'in tuttuğu veriler
  const [stajers, setStajers] = useState([])        // Filtrelenmiş stajerler (ekranda gösterilecek)
  const [allStajers, setAllStajers] = useState([])  // Tüm stajerler (backend'den gelen ham veri)
  
  // UI state'leri - Kullanıcı arayüzü durumları
  const [loading, setLoading] = useState(true)      // Veri yükleniyor mu? (başlangıçta true)
  const [error, setError] = useState(null)          // Hata mesajı (varsa)
  const [searchText, setSearchText] = useState('') // Arama kutusundaki metin
  const [sortBy, setSortBy] = useState('StajerID') // Hangi kolona göre sıralanacak
  const [sortOrder, setSortOrder] = useState('desc') // Sıralama yönü (asc/desc)
  const [totalCount, setTotalCount] = useState(0)   // Toplam stajer sayısı
  const [IsActive, setIsActive] = useState(false)  // Sadece aktif stajerleri göster?
  
  // Modal state'leri - Modal pencerelerin açık/kapalı durumu
  const [showCreateModal, setShowCreateModal] = useState(false)  // Yeni stajer ekleme modalı
  const [showEditModal, setShowEditModal] = useState(false)      // Düzenleme modalı
  const [selectedStajer, setSelectedStajer] = useState(null)   // Seçili stajer (modal için)
  
  // Auth context - Kullanıcı bilgisi ve yetkilendirme
  const { user, loading: authLoading } = useAuth()  // Auth context'ten kullanıcı bilgisi
  const isAdmin = !authLoading && (user?.role === 'admin')  // Admin kontrolü
  
  // ... component logic ...
}
```

### İki Aşamalı Filtreleme

1. **Backend**: Sıralama ve opsiyonel arama
2. **Frontend**: Aktif stajer filtresi + arama

---

## 6. Filtreleme Mekanizmasi

### Aktif Stajer Kontrolü (utils/stajerUtils.js)

```javascript
// Stajerin aktif olup olmadığını kontrol eden fonksiyon
// Aktif: Bugünün tarihi başlangıç ve bitiş tarihleri arasında
export const isStajActive = (stajer) => {
  // Eğer başlangıç veya bitiş tarihi yoksa, stajer aktif değildir
  if (!stajer?.startDate || !stajer?.endDate) {
    return false
  }

  // Bugünün tarihini al ve saat bilgisini sıfırla (00:00:00)
  const today = new Date()
  today.setHours(0, 0, 0, 0)

  // Başlangıç tarihini al ve saat bilgisini sıfırla
  const startDate = new Date(stajer.startDate)
  startDate.setHours(0, 0, 0, 0)

  // Bitiş tarihini al ve saat bilgisini sıfırla
  const endDate = new Date(stajer.endDate)
  endDate.setHours(0, 0, 0, 0)

  // Bugün başlangıç ve bitiş tarihleri arasındaysa aktif
  return today >= startDate && today <= endDate
}
```

### Filtreleme Fonksiyonu (StajersPage.jsx)

```jsx
// Stajerleri filtrele - useCallback ile memoize edilmiş fonksiyon
// Performans optimizasyonu: Sadece IsActive veya searchText değiştiğinde yeniden oluşturulur
const filterStajers = useCallback((stajersList) => {
  // Orijinal listeyi kopyala (mutasyon önlemek için)
  let filtered = [...stajersList]
  
  // Aktif stajer filtresi: Eğer IsActive true ise
  if (IsActive) {
    // Sadece aktif stajerleri filtrele (isStajActive fonksiyonu ile)
    filtered = filtered.filter(isStajActive)
  }
  
  // Arama filtresi: Eğer arama metni varsa
  if (searchText.trim()) {
    // Arama metnini küçük harfe çevir (case-insensitive arama için)
    const lower = searchText.trim().toLowerCase()
    // Birden fazla alanda arama yap (OR koşulu)
    filtered = filtered.filter(s =>
      s.fullName?.toLowerCase().includes(lower) ||           // Ad soyad
      s.email?.toLowerCase().includes(lower) ||             // Email
      s.phoneNumber?.includes(lower) ||                     // Telefon
      s.universite?.universiteAdi?.toLowerCase().includes(lower) ||  // Üniversite
      s.bolum?.bolumAdi?.toLowerCase().includes(lower) ||   // Bölüm
      s.departman?.departmanAdi?.toLowerCase().includes(lower)       // Departman
    )
  }
  
  // Filtrelenmiş listeyi döndür
  return filtered
}, [IsActive, searchText])  // Dependency array: Bu değerler değiştiğinde fonksiyon yeniden oluşturulur
```

### Arama Filtresi Kapsamı

- ✅ Ad Soyad (`fullName`)
- ✅ Email (`email`)
- ✅ Telefon (`phoneNumber`)
- ✅ Üniversite (`universite.universiteAdi`)
- ✅ Bölüm (`bolum.bolumAdi`)
- ✅ Departman (`departman.departmanAdi`)

---

## 7. Render İşlemi

### Veri Yükleme Fonksiyonu

```jsx
const loadStajers = async () => {
  try {
    setLoading(true)
    setError(null)
    const data = await stajerService.getAll(
      sortBy,
      sortOrder,
      '' // Arama metnini backend'e göndermiyor
    )
    
    if (data && data.stajers) {
      setAllStajers(data.stajers) // Tüm stajerleri kaydet
      const filtered = filterStajers(data.stajers) // Filtrele
      setStajers(filtered)
      setTotalCount(filtered.length)
    } else if (Array.isArray(data)) {
      setAllStajers(data)
      const filtered = filterStajers(data)
      setStajers(filtered)
      setTotalCount(filtered.length)
    } else {
      setAllStajers([])
      setStajers([])
      setTotalCount(0)
    }
  } catch (err) {
    console.error('Stajer yükleme hatası:', err)
    setError('Stajerler yüklenirken hata oluştu: ' + (err.response?.data?.message || err.message))
    setAllStajers([])
    setStajers([])
  } finally {
    setLoading(false)
  }
}
```

### Tablo Yapısı (JSX)

```jsx
<tbody>
  {stajers.length === 0 ? (
    <tr>
      <td colSpan="10" className="text-center py-4 text-muted">
        {searchText ? 'Arama sonucu bulunamadı.' : 'Henüz stajer eklenmemiş.'}
      </td>
    </tr>
  ) : (
    stajers.map((s) => (
      <tr key={s.stajerID} className={!isStajActive(s) ? 'table-danger' : ''}>
        <td><span className="badge bg-secondary">{s.stajerID}</span></td>
        <td>{s.fullName}</td>
        <td>{s.email}</td>
        <td>{s.phoneNumber}</td>
        <td>{s.universite?.universiteAdi || s.universite || '-'}</td>
        <td>{s.bolum?.bolumAdi || s.bolum || '-'}</td>
        <td>{s.departman?.departmanAdi || s.departman || '-'}</td>
        <td>{formatDate(s.startDate)}</td>
        <td>{formatDate(s.endDate)}</td>
        <td className="text-center">
          <div className="d-flex justify-content-center gap-2">
            <button
              className="btn btn-info btn-sm"
              onClick={() => openDetailsModal(s)}
              title="Detaylar"
            >
              <i className="fas fa-eye"></i>
            </button>
            {isAdmin && (
              <>
                <button
                  className="btn btn-warning btn-sm"
                  onClick={() => openEditModal(s)}
                  title="Düzenle"
                >
                  <i className="fas fa-edit"></i>
                </button>
                <button
                  className="btn btn-danger btn-sm"
                  onClick={() => openDeleteModal(s)}
                  title="Sil"
                >
                  <i className="fas fa-trash"></i>
                </button>
              </>
            )}
          </div>
        </td>
      </tr>
    ))
  )}
</tbody>
```

### Tarih Formatlama

```jsx
const formatDate = (dateString) => {
  if (!dateString) return '-'
  try {
    const date = new Date(dateString)
    if (isNaN(date.getTime())) return dateString
    return date.toLocaleDateString('tr-TR')
  } catch {
    return dateString
  }
}
```

### Özellikler

- **Key Prop**: React için unique identifier (`s.stajerID`)
- **Conditional Styling**: Aktif olmayanlar kırmızı (`table-danger`)
- **Formatting**: Tarihler Türkçe formatında (`tr-TR`)
- **Action Buttons**: Detay, Düzenle, Sil (Admin only)
- **Null Safety**: Optional chaining (`?.`) ve fallback değerler

---

## 8. Veri Akış Diyagramı

```text
┌─────────────┐
│  Kullanıcı  │
└──────┬──────┘
       │
       ▼
┌─────────────────┐
│  StajersPage    │
│  Component      │
└──────┬──────────┘
       │ loadStajers()
       ▼
┌─────────────────┐
│ stajerService   │
│ .getAll()       │
└──────┬──────────┘
       │ HTTP GET
       ▼
┌──────────────────────┐
│ StajersApiController │
│ GetStajers()         │
└──────┬───────────────┘
       │ EF Core Query
       ▼
┌─────────────────┐
│   Database      │
│   (Stajers)     │
└─────────────────┘
```

---

## 9. Önemli Teknik Detaylar

### Performance Optimizasyonları

```jsx
// useCallback ile memoization
const filterStajers = useCallback((stajersList) => {
  // ... filtreleme logic ...
}, [IsActive, searchText])

// Conditional rendering
{!loading && (
  <div className="table-responsive">
    {/* Tablo içeriği */}
  </div>
)}

// Dependency array kontrolü
useEffect(() => {
  loadStajers()
}, [sortBy, sortOrder]) // Sadece gerekli değişkenler
```

### Error Handling

```jsx
// Try-catch bloğu
try {
  setLoading(true)
  const data = await stajerService.getAll(sortBy, sortOrder, '')
  // ... veri işleme ...
} catch (err) {
  console.error('Stajer yükleme hatası:', err)
  setError('Stajerler yüklenirken hata oluştu: ' + 
    (err.response?.data?.message || err.message))
} finally {
  setLoading(false)
}
```

### Security

```jsx
// PrivateRoute ile authentication
<PrivateRoute>
  <StajersPage />
</PrivateRoute>

// Role-based access control
const isAdmin = !authLoading && (user?.role === 'admin')

{isAdmin && (
  <button onClick={openCreateModal}>
    Yeni Stajer Ekle
  </button>
)}
```

### Best Practices

- ✅ **Memoization**: `useCallback` ile gereksiz render'ları önleme
- ✅ **Error Boundaries**: Hata yakalama ve kullanıcıya gösterme
- ✅ **Loading States**: Kullanıcı deneyimi için yükleme göstergeleri
- ✅ **Input Validation**: Backend ve frontend'de doğrulama
- ✅ **Type Safety**: Optional chaining ve null checks

---

## 10. Mapping Fonksiyonu

### Backend Mapping (StajersApiController.cs)

```csharp
private static object MapStajerListItem(StajerModel s) => new
{
    stajerID = s.StajerID,
    fullName = s.FullName,
    email = s.Email,
    phoneNumber = s.PhoneNumber,
    universiteID = s.UniversiteID,
    bolumID = s.BolumID,
    departmanID = s.DepartmanID,
    universite = s.Universite != null 
        ? new { universiteAdi = s.Universite.UniversiteAdi } 
        : null,
    bolum = s.Bolum != null 
        ? new { bolumAdi = s.Bolum.BolumAdi } 
        : null,
    departman = s.Departman != null 
        ? new { departmanAdi = s.Departman.DepartmanAdi } 
        : null,
    startDate = s.StartDate,
    endDate = s.EndDate,
    notes = s.Notes
};

private static object MapStajerDetail(StajerModel s) => new
{
    stajerID = s.StajerID,
    fullName = s.FullName,
    email = s.Email,
    phoneNumber = s.PhoneNumber,
    universiteID = s.UniversiteID,
    bolumID = s.BolumID,
    departmanID = s.DepartmanID,
    startDate = s.StartDate,
    endDate = s.EndDate,
    notes = s.Notes,
    universite = s.Universite != null 
        ? new { s.Universite.UniversiteAdi } 
        : null,
    bolum = s.Bolum != null 
        ? new { s.Bolum.BolumAdi } 
        : null,
    departman = s.Departman != null 
        ? new { s.Departman.DepartmanAdi } 
        : null
};
```

### Frontend Null Safety

```jsx
// Optional chaining
{s.universite?.universiteAdi}

// Null coalescing
{user?.role ?? 'user'}

// Fallback değerler
{s.universite?.universiteAdi || s.universite || '-'}
```

### Null Safety Teknikleri

- ✅ **Optional Chaining** (`?.`): Null/undefined kontrolü
- ✅ **Null Coalescing** (`??`): Varsayılan değer atama
- ✅ **Fallback Değerler** (`|| '-'`): Alternatif gösterim
- ✅ **Ternary Operator**: Koşullu render

---

## 11. Kullanıcı Deneyimi

### Loading States

```jsx
{loading && (
  <div className="text-center my-4">
    <div className="spinner-border text-primary"></div>
    <p className="mt-2">Veriler yükleniyor...</p>
  </div>
)}
```

### Error States

```jsx
{error && (
  <div className="alert alert-danger d-flex justify-content-between">
    <span>{error}</span>
    <button 
      className="btn btn-outline-light btn-sm" 
      onClick={loadStajers}
    >
      Veriler Yüklenemedi. Tekrar Dene
    </button>
  </div>
)}
```

### Empty States

```jsx
{stajers.length === 0 && !loading && (
  <tr>
    <td colSpan="10" className="text-center py-4 text-muted">
      {searchText 
        ? 'Arama sonucu bulunamadı.' 
        : 'Henüz stajer eklenmemiş.'}
    </td>
  </tr>
)}
```

### useEffect Hook'ları

```jsx
// İlk yükleme
useEffect(() => {
  loadStajers()
}, [sortBy, sortOrder])

// Otomatik filtreleme
useEffect(() => {
  if (allStajers.length > 0) {
    const filtered = filterStajers(allStajers)
    setStajers(filtered)
    setTotalCount(filtered.length)
  }
}, [IsActive, searchText, allStajers, filterStajers])
```

### UX Özellikleri

- ✅ **Loading Spinner**: Veri yüklenirken gösterge
- ✅ **Error Messages**: Anlaşılır hata mesajları
- ✅ **Retry Button**: Hata durumunda yeniden deneme
- ✅ **Empty States**: Boş durum mesajları
- ✅ **Search Feedback**: Arama sonuç sayısı gösterimi

---

## 12. Sonuç

### Özet

✅ **Mimari**: Modern React + ASP.NET Core  
✅ **Performans**: Optimize edilmiş filtreleme  
✅ **UX**: Loading, error, empty states  
✅ **Security**: Authentication & Authorization  
✅ **Maintainability**: Clean code principles  

### Teknoloji Stack

- React.js (Hooks, Context API)
- ASP.NET Core (Web API)
- Entity Framework Core
- Axios
- Bootstrap 5

---

## Sorular?

**Teşekkürler!**

