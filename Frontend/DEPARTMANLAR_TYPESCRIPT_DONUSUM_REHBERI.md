# Departmanlar.jsx → Departmanlar.tsx Dönüşüm Rehberi

Bu dokümantasyon, `Departmanlar.jsx` dosyasının TypeScript'e (`Departmanlar.tsx`) dönüştürülmesi sırasında yapılan tüm değişiklikleri detaylı olarak açıklar.

## 📋 İçindekiler

1. [Import Değişiklikleri](#1-import-değişiklikleri)
2. [State Tip Tanımlamaları](#2-state-tip-tanımlamaları)
3. [Fonksiyon Tip Tanımlamaları](#3-fonksiyon-tip-tanımlamaları)
4. [API Çağrıları ve Generic Tipler](#4-api-çağrıları-ve-generic-tipler)
5. [JSX/TSX Özellikleri](#5-jsxtsx-özellikleri)
6. [Kod İyileştirmeleri](#6-kod-iyileştirmeleri)
7. [Özet](#7-özet)

---

## 1. Import Değişiklikleri

### 1.1. Dosya Uzantılarının Kaldırılması

**ESKİ (JavaScript):**
```javascript
import { useAuth } from '../contexts/AuthContext.jsx'
import { useToast } from '../components/ToastProvider.jsx'
import ConfirmModal from '../components/StajerModals/ConfirmModal.jsx'
import CreateDepartmanModal from '../components/DepartmanModals/CreateDepartmanModal.jsx'
import EditDepartmanModal from '../components/DepartmanModals/EditDepartmanModal.jsx'
import { useBootstrapModal } from '../hooks/useBootstrapModal.js'
```

**YENİ (TypeScript):**
```typescript
import { useAuth } from '../contexts/AuthContext'
import { useToast } from '../components/ToastProvider'
import ConfirmModal from '../components/StajerModals/ConfirmModal'
import CreateDepartmanModal from '../components/DepartmanModals/CreateDepartmanModal'
import EditDepartmanModal from '../components/DepartmanModals/EditDepartmanModal'
import { useBootstrapModal } from '../hooks/useBootstrapModal'
```

**Açıklama:** TypeScript/React'te import'larda dosya uzantısı belirtmek gerekmez. TypeScript otomatik olarak `.ts`, `.tsx`, `.js`, `.jsx` uzantılarını arar.

### 1.2. Tip Import'larının Eklenmesi

**ESKİ:**
```javascript
// Tip import'u yok
```

**YENİ:**
```typescript
import { Departman, Stajer } from '../types/stajer.types'
```

**Açıklama:** TypeScript'te kullanılacak tipleri import etmek gerekir. Bu tipler state'lerde ve fonksiyon parametrelerinde kullanılacak.

---

## 2. State Tip Tanımlamaları

### 2.1. Basit State'ler

**ESKİ:**
```javascript
const [loading, setLoading] = useState(true)
const [error, setError] = useState(null)
const [showCreateModal, setShowCreateModal] = useState(false)
```

**YENİ:**
```typescript
const [loading, setLoading] = useState<boolean>(true)
const [error, setError] = useState<string | null>(null)
const [showCreateModal, setShowCreateModal] = useState<boolean>(false)
```

**Açıklama:** 
- `useState<boolean>`: Boolean tipinde state
- `useState<string | null>`: String veya null olabilen state (union type)
- Generic type syntax: `useState<Tip>(başlangıçDeğeri)`

### 2.2. Array State'ler

**ESKİ:**
```javascript
const [departmanlar, setDepartmanlar] = useState([])
const [departmanStajerler, setDepartmanStajerler] = useState([])
const [allStajers, setAllStajers] = useState([])
```

**YENİ:**
```typescript
const [departmanlar, setDepartmanlar] = useState<Departman[]>([])
const [departmanStajerler, setDepartmanStajerler] = useState<Stajer[]>([])
const [allStajers, setAllStajers] = useState<Stajer[]>([])
```

**Açıklama:** 
- `useState<Departman[]>`: Departman tipinde elemanlar içeren array
- `useState<Stajer[]>`: Stajer tipinde elemanlar içeren array
- Array tipi: `Tip[]` veya `Array<Tip>`

### 2.3. Nullable State'ler

**ESKİ:**
```javascript
const [editingDepartman, setEditingDepartman] = useState(null)
const [departmanToDelete, setDepartmanToDelete] = useState(null)
```

**YENİ:**
```typescript
const [editingDepartman, setEditingDepartman] = useState<Departman | null>(null)
const [departmanToDelete, setDepartmanToDelete] = useState<Departman | null>(null)
```

**Açıklama:** 
- `useState<Departman | null>`: Departman tipinde veya null olabilen state
- Union type (`|`) kullanarak birden fazla tip belirtilebilir
- Bu sayede TypeScript null kontrolü yapılmasını zorunlu kılar

---

## 3. Fonksiyon Tip Tanımlamaları

### 3.1. Void Fonksiyonlar

**ESKİ:**
```javascript
const handleCloseDetailsModal = () => {
  setShowDetailsModal(false)
  setDepartmanStajerler([])
}

const openCreateModal = () => setShowCreateModal(true)
```

**YENİ:**
```typescript
const handleCloseDetailsModal = (): void => {
  setShowDetailsModal(false)
  setDepartmanStajerler([])
}

const openCreateModal = (): void => setShowCreateModal(true)
```

**Açıklama:** 
- `: void`: Fonksiyonun bir değer döndürmediğini belirtir
- Return type annotation fonksiyon parametrelerinden sonra yazılır
- Syntax: `(parametreler): dönüşTipi => { ... }`

### 3.2. Async Fonksiyonlar

**ESKİ:**
```javascript
const fetchDepartmanlar = async () => {
  // ...
}

const handleModalSuccess = async () => {
  await fetchDepartmanlar()
  closeModals()
}
```

**YENİ:**
```typescript
const fetchDepartmanlar = async (): Promise<void> => {
  // ...
}

const handleModalSuccess = async (): Promise<void> => {
  await fetchDepartmanlar()
  closeModals()
}
```

**Açıklama:** 
- `Promise<void>`: Async fonksiyonlar Promise döndürür
- `void` Promise'i hiçbir değer döndürmez
- Eğer değer döndürüyorsa: `Promise<string>`, `Promise<number>` gibi kullanılır

### 3.3. Parametreli Fonksiyonlar

**ESKİ:**
```javascript
const openEditModal = (departman) => {
  setEditingDepartman(departman)
  setShowEditModal(true)
}

const fetchDepartmanStajerler = async (departmanId, departmanAdi) => {
  // ...
}

const getStajerCount = (departmanId) => {
  return allStajers.filter((stajer) => stajer.departmanID === departmanId).length
}
```

**YENİ:**
```typescript
const openEditModal = (departman: Departman): void => {
  setEditingDepartman(departman)
  setShowEditModal(true)
}

const fetchDepartmanStajerler = async (departmanId: number, departmanAdi: string): Promise<void> => {
  // ...
}

const getStajerCount = (departmanId: number): number => {
  return allStajers.filter((stajer) => stajer.departmanID === departmanId).length
}
```

**Açıklama:** 
- Parametre tipleri: `parametreAdı: Tip`
- Her parametre için tip belirtilmelidir
- Return type parametrelerden sonra yazılır

### 3.4. Hata Yakalama

**ESKİ:**
```javascript
catch (err) {
  setError('Departmanlar yüklenirken hata oluştu: ' + (err.response?.data?.message || err.message))
  console.error('Error fetching departmanlar:', err)
}
```

**YENİ:**
```typescript
catch (err: any) {
  setError('Departmanlar yüklenirken hata oluştu: ' + (err.response?.data?.message || err.message))
  console.error('Error fetching departmanlar:', err)
}
```

**Açıklama:** 
- `err: any`: Hata tipini `any` olarak belirtiriz (esnek tip)
- Alternatif: `err: unknown` kullanıp type guard ile kontrol edilebilir
- `any` kullanımı tip güvenliğini azaltır ama pratik kullanım için uygundur

---

## 4. API Çağrıları ve Generic Tipler

### 4.1. GET İstekleri

**ESKİ:**
```javascript
const response = await api.get('/DepartmanApi')
setDepartmanlar(response.data)
```

**YENİ:**
```typescript
const response = await api.get<Departman[]>('/DepartmanApi')
setDepartmanlar(response.data)
```

**Açıklama:** 
- `api.get<Tip>`: Generic type ile API'den dönecek veri tipini belirtiriz
- TypeScript artık `response.data`'nın `Departman[]` tipinde olduğunu bilir
- IntelliSense ve tip kontrolü sağlanır

### 4.2. Karmaşık Response Tipleri

**ESKİ:**
```javascript
const response = await api.get('/StajersApi', {
  params: {
    sortBy: 'FullName',
    sortOrder: 'asc',
  },
})
const stajerler = Array.isArray(response.data?.stajers) ? response.data.stajers : []
```

**YENİ:**
```typescript
const response = await api.get<{ stajers: Stajer[] }>('/StajersApi', {
  params: {
    sortBy: 'FullName',
    sortOrder: 'asc',
  },
})
const stajerler = Array.isArray(response.data?.stajers) ? response.data.stajers : []
```

**Açıklama:** 
- `{ stajers: Stajer[] }`: Object tipinde, içinde `stajers` property'si olan tip
- TypeScript artık `response.data.stajers`'ın `Stajer[]` olduğunu bilir

### 4.3. DELETE İstekleri

**ESKİ:**
```javascript
const response = await api.delete(`/DepartmanApi/${id}`)
if (response.data.success) {
  // ...
}
```

**YENİ:**
```typescript
const response = await api.delete<{ success: boolean; message?: string }>(`/DepartmanApi/${id}`)
if (response.data.success) {
  // ...
}
```

**Açıklama:** 
- `{ success: boolean; message?: string }`: Optional property (`?`) ile `message` alanı opsiyonel
- TypeScript artık `response.data.success` ve `response.data.message` tiplerini bilir

---

## 5. JSX/TSX Özellikleri

### 5.1. String Attribute'lar → Number Attribute'lar

**ESKİ:**
```jsx
<td colSpan="10" className="text-center py-4 text-muted">
  {'Henüz stajer eklenmemiş.'}
</td>

<div 
  className="modal fade" 
  id="detailsDepartmanModal" 
  tabIndex="-1"
  // ...
>
```

**YENİ:**
```tsx
<td colSpan={10} className="text-center py-4 text-muted">
  {'Henüz stajer eklenmemiş.'}
</td>

<div 
  className="modal fade" 
  id="detailsDepartmanModal" 
  tabIndex={-1}
  // ...
>
```

**Açıklama:** 
- TypeScript'te sayısal değerler için string yerine number kullanılmalı
- `colSpan="10"` → `colSpan={10}`
- `tabIndex="-1"` → `tabIndex={-1}`
- Bu sayede tip kontrolü yapılır

### 5.2. Key Prop Düzeltmesi

**ESKİ:**
```jsx
departmanlar.map((s) => (
  <tr key={s.stajerID} className={...}>
    {/* ... */}
  </tr>
))
```

**YENİ:**
```tsx
departmanlar.map((s) => (
  <tr key={s.departmanID} className={...}>
    {/* ... */}
  </tr>
))
```

**Açıklama:** 
- `key` prop'u unique olmalı ve doğru property'yi kullanmalı
- `s.stajerID` yerine `s.departmanID` kullanılmalı (çünkü departman objesi)

---

## 6. Kod İyileştirmeleri

### 6.1. Karşılaştırma Operatörleri

**ESKİ:**
```javascript
className={getStajerCount(s.departmanID) == 0 ? 'table-warning' : ''}
```

**YENİ:**
```typescript
className={getStajerCount(s.departmanID) === 0 ? 'table-warning' : ''}
```

**Açıklama:** 
- `==` (loose equality) yerine `===` (strict equality) kullanılmalı
- TypeScript strict mode'da `==` kullanımını uyarır
- `===` tip kontrolü de yapar, daha güvenlidir

### 6.2. Type Guard ile Tip Kontrolü

**ESKİ:**
```jsx
{departmanStajerler.map((stajer) => (
  <div key={stajer.stajerID} className="list-group-item">
    {/* ... */}
    <p className="mb-0 text-muted small">
      {stajer.universite?.universiteAdi || stajer.universite || ''}
      {stajer.bolum && (
        <span> • {stajer.bolum?.bolumAdi || stajer.bolum}</span>
      )}
    </p>
  </div>
))}
```

**YENİ:**
```tsx
{departmanStajerler.map((stajer) => {
  const universiteAdi = typeof stajer.universite === 'string' 
    ? stajer.universite 
    : stajer.universite?.universiteAdi
  const bolumAdi = typeof stajer.bolum === 'string' 
    ? stajer.bolum 
    : stajer.bolum?.bolumAdi
  
  return (
    <div key={stajer.stajerID} className="list-group-item">
      {/* ... */}
      <p className="mb-0 text-muted small">
        {universiteAdi || ''}
        {bolumAdi && (
          <span> • {bolumAdi}</span>
        )}
      </p>
    </div>
  )
})}
```

**Açıklama:** 
- Type guard ile tip kontrolü yapılır
- `typeof stajer.universite === 'string'` kontrolü ile tip belirlenir
- Daha güvenli ve okunabilir kod

### 6.3. Null Kontrolü

**ESKİ:**
```jsx
onConfirm={() => confirmDelete(departmanToDelete?.departmanID)}
```

**YENİ:**
```tsx
onConfirm={() => departmanToDelete && confirmDelete(departmanToDelete.departmanID)}
```

**Açıklama:** 
- Optional chaining (`?.`) yerine explicit null check kullanılır
- Daha açık ve tip güvenli

### 6.4. User Type Assertion

**ESKİ:**
```javascript
const role = (user?.role ?? '').trim().toLowerCase()
const email = (user?.email ?? '').trim().toLowerCase()
```

**YENİ:**
```typescript
const role = ((user as any)?.role ?? '').trim().toLowerCase()
const email = ((user as any)?.email ?? '').trim().toLowerCase()
```

**Açıklama:** 
- `as any`: Type assertion ile tip kontrolünü atlarız
- `user` objesinin tipi tam olarak bilinmediği için `as any` kullanılır
- Alternatif: User tipini tanımlayıp kullanılabilir

### 6.5. useEffect Eslint Yorumu

**ESKİ:**
```javascript
useEffect(() => {
  fetchDepartmanlar()
}, [])
```

**YENİ:**
```typescript
useEffect(() => {
  fetchDepartmanlar()
  // eslint-disable-next-line react-hooks/exhaustive-deps
}, [])
```

**Açıklama:** 
- ESLint `exhaustive-deps` kuralı dependency array'i kontrol eder
- `fetchDepartmanlar` fonksiyonu dependency'de olmalı ama bu sonsuz döngü yaratabilir
- Bu yüzden eslint disable yorumu eklenir

---

## 7. Özet

### Yapılan Değişiklikler

1. ✅ **Import'lar**: Dosya uzantıları kaldırıldı, tip import'ları eklendi
2. ✅ **State'ler**: Tüm state'lere tip tanımlamaları eklendi
3. ✅ **Fonksiyonlar**: Tüm fonksiyonlara parametre ve dönüş tipleri eklendi
4. ✅ **API Çağrıları**: Generic tipler ile tip güvenliği sağlandı
5. ✅ **JSX/TSX**: Sayısal attribute'lar düzeltildi, key prop'ları düzeltildi
6. ✅ **Kod Kalitesi**: Strict equality, type guard, null kontrolü eklendi

### TypeScript'in Avantajları

- **Tip Güvenliği**: Derleme zamanında hataları yakalar
- **IntelliSense**: Daha iyi kod tamamlama ve öneriler
- **Refactoring**: Daha güvenli kod değişiklikleri
- **Dokümantasyon**: Kod kendini dokümante eder
- **Hata Önleme**: Runtime hatalarını azaltır

### Önemli Notlar

- TypeScript JavaScript'in üzerine tip sistemi ekler, JavaScript değildir
- Tüm JavaScript kodu geçerli TypeScript kodudur
- Tip tanımlamaları opsiyoneldir ama önerilir
- `any` tipi kullanımından kaçınılmalıdır (mümkün olduğunca)

### Sonraki Adımlar

1. Diğer `.jsx` dosyalarını da TypeScript'e dönüştürün
2. Daha spesifik tip tanımlamaları yapın (User tipi gibi)
3. Interface'ler oluşturun (API response tipleri için)
4. Utility type'ları kullanın (`Partial`, `Pick`, `Omit` gibi)

---

**Hazırlayan:** AI Assistant  
**Tarih:** 2024  
**Versiyon:** 1.0

