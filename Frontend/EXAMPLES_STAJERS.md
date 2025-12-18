# Stajers Sayfasını React'e Taşıma - Detaylı Örnek

Bu dosya, MVC'deki Stajers sayfasını React'e nasıl taşıyacağınızı adım adım gösterir.

## 📋 Mevcut MVC Yapısı

### Backend Controller:
- `GET /Stajers/GetStajers` → JSON döndürüyor ✅
- `POST /Stajers/Create` → JSON döndürüyor ✅
- `POST /Stajers/Edit/{id}` → JSON döndürüyor ✅
- `POST /Stajers/Delete/{id}` → JSON döndürüyor ✅
- `GET /Stajers/GetBolumlerByUniversite/{id}` → Bölümleri getiriyor ✅

### MVC View Özellikleri:
- Tablo ile listeleme
- Arama (searchText)
- Sıralama (sortBy, sortOrder)
- Modal ile Create/Edit
- Cascade dropdown (Üniversite → Bölüm)
- Form validation

---

## 🔄 React Component Yapısı

### 1. Ana Component: StajersList.jsx

```jsx
import { useState, useEffect } from 'react';
import { backendApi } from '../lib/api';
import StajerModal from './StajerModal';
import './Stajers.css';

function StajersList() {
  // State'ler
  const [stajers, setStajers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchText, setSearchText] = useState('');
  const [sortBy, setSortBy] = useState('StajerID');
  const [sortOrder, setSortOrder] = useState('desc');
  const [showModal, setShowModal] = useState(false);
  const [editingStajer, setEditingStajer] = useState(null);

  // Verileri yükle
  useEffect(() => {
    fetchStajers();
  }, [sortBy, sortOrder, searchText]);

  const fetchStajers = async () => {
    try {
      setLoading(true);
      const response = await backendApi.get('/Stajers/GetStajers', {
        params: { sortBy, sortOrder, searchText }
      });
      setStajers(response.data.stajers || []);
    } catch (error) {
      console.error('Hata:', error);
    } finally {
      setLoading(false);
    }
  };

  // Arama
  const handleSearch = (e) => {
    e.preventDefault();
    fetchStajers();
  };

  // Sıralama
  const handleSort = (column) => {
    if (sortBy === column) {
      setSortOrder(sortOrder === 'asc' ? 'desc' : 'asc');
    } else {
      setSortBy(column);
      setSortOrder('asc');
    }
  };

  // Modal
  const openCreateModal = () => {
    setEditingStajer(null);
    setShowModal(true);
  };

  const openEditModal = (stajer) => {
    setEditingStajer(stajer);
    setShowModal(true);
  };

  const closeModal = () => {
    setShowModal(false);
    setEditingStajer(null);
    fetchStajers(); // Listeyi yenile
  };

  // Silme
  const handleDelete = async (id) => {
    if (!confirm('Silinsin mi?')) return;
    
    try {
      await backendApi.post(`/Stajers/Delete/${id}`);
      fetchStajers();
    } catch (error) {
      alert('Hata: ' + error.response?.data?.message);
    }
  };

  if (loading && stajers.length === 0) {
    return <div>Yükleniyor...</div>;
  }

  return (
    <div className="stajers-container">
      {/* Arama */}
      <form onSubmit={handleSearch}>
        <input
          value={searchText}
          onChange={e => setSearchText(e.target.value)}
          placeholder="Ara..."
        />
        <button type="submit">Ara</button>
        <button type="button" onClick={() => setSearchText('')}>Temizle</button>
      </form>

      {/* Yeni Ekle Butonu */}
      <button onClick={openCreateModal}>+ Yeni Stajer</button>

      {/* Tablo */}
      <table>
        <thead>
          <tr>
            <th onClick={() => handleSort('StajerID')}>ID</th>
            <th onClick={() => handleSort('FullName')}>Ad Soyad</th>
            <th>Email</th>
            <th>Telefon</th>
            <th onClick={() => handleSort('Universite')}>Üniversite</th>
            <th onClick={() => handleSort('Bolum')}>Bölüm</th>
            <th onClick={() => handleSort('Departman')}>Departman</th>
            <th onClick={() => handleSort('StartDate')}>Başlangıç</th>
            <th onClick={() => handleSort('EndDate')}>Bitiş</th>
            <th>İşlemler</th>
          </tr>
        </thead>
        <tbody>
          {stajers.map(stajer => (
            <tr key={stajer.stajerID}>
              <td>{stajer.stajerID}</td>
              <td>{stajer.fullName}</td>
              <td>{stajer.email}</td>
              <td>{stajer.phoneNumber}</td>
              <td>{stajer.universite?.universiteAdi || '-'}</td>
              <td>{stajer.bolum?.bolumAdi || '-'}</td>
              <td>{stajer.departman?.departmanAdi || '-'}</td>
              <td>{formatDate(stajer.startDate)}</td>
              <td>{formatDate(stajer.endDate)}</td>
              <td>
                <button onClick={() => openEditModal(stajer)}>Düzenle</button>
                <button onClick={() => handleDelete(stajer.stajerID)}>Sil</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {/* Modal */}
      {showModal && (
        <StajerModal
          stajer={editingStajer}
          onClose={closeModal}
        />
      )}
  </div>
  );
}

function formatDate(dateString) {
  if (!dateString) return '-';
  return new Date(dateString).toLocaleDateString('tr-TR');
}

export default StajersList;
```

---

### 2. Modal Component: StajerModal.jsx

```jsx
import { useState, useEffect } from 'react';
import { backendApi } from '../lib/api';

function StajerModal({ stajer, onClose }) {
  const [formData, setFormData] = useState({
    fullName: '',
    email: '',
    phoneNumber: '',
    universiteID: '',
    bolumID: '',
    departmanID: '',
    startDate: '',
    endDate: '',
    notes: ''
  });

  const [universiteler, setUniversiteler] = useState([]);
  const [bolumler, setBolumler] = useState([]);
  const [departmanlar, setDepartmanlar] = useState([]);
  const [loadingBolumler, setLoadingBolumler] = useState(false);

  // Form'u doldur (edit modunda)
  useEffect(() => {
    if (stajer) {
      setFormData({
        fullName: stajer.fullName || '',
        email: stajer.email || '',
        phoneNumber: stajer.phoneNumber || '',
        universiteID: stajer.universiteID || '',
        bolumID: stajer.bolumID || '',
        departmanID: stajer.departmanID || '',
        startDate: stajer.startDate || '',
        endDate: stajer.endDate || '',
        notes: stajer.notes || ''
      });
      // Eğer üniversite seçiliyse, bölümleri yükle
      if (stajer.universiteID) {
        loadBolumler(stajer.universiteID);
      }
    }
  }, [stajer]);

  // Üniversiteleri yükle
  useEffect(() => {
    loadUniversiteler();
    loadDepartmanlar();
  }, []);

  // Üniversite değiştiğinde bölümleri yükle
  useEffect(() => {
    if (formData.universiteID) {
      loadBolumler(formData.universiteID);
    } else {
      setBolumler([]);
      setFormData(prev => ({ ...prev, bolumID: '' }));
    }
  }, [formData.universiteID]);

  const loadUniversiteler = async () => {
    try {
      const res = await backendApi.get('/UniversiteModels');
      setUniversiteler(res.data || []);
    } catch (error) {
      console.error('Üniversiteler yüklenemedi:', error);
    }
  };

  const loadBolumler = async (universiteId) => {
    if (!universiteId) return;
    
    try {
      setLoadingBolumler(true);
      const res = await backendApi.get(`/Stajers/GetBolumlerByUniversite/${universiteId}`);
      if (res.data.success) {
        setBolumler(res.data.bolumler || []);
      }
    } catch (error) {
      console.error('Bölümler yüklenemedi:', error);
    } finally {
      setLoadingBolumler(false);
    }
  };

  const loadDepartmanlar = async () => {
    try {
      const res = await backendApi.get('/api/DepartmanApi');
      setDepartmanlar(res.data || []);
    } catch (error) {
      console.error('Departmanlar yüklenemedi:', error);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      const payload = {
        FullName: formData.fullName,
        Email: formData.email,
        PhoneNumber: formData.phoneNumber,
        UniversiteID: formData.universiteID || null,
        BolumID: formData.bolumID || null,
        DepartmanID: formData.departmanID,
        StartDate: formData.startDate,
        EndDate: formData.endDate,
        Notes: formData.notes || null
      };

      if (stajer) {
        // Edit
        payload.StajerID = stajer.stajerID;
        await backendApi.post(`/Stajers/Edit/${stajer.stajerID}`, payload);
        alert('Güncellendi!');
      } else {
        // Create
        await backendApi.post('/Stajers/Create', payload);
        alert('Eklendi!');
      }

      onClose();
    } catch (error) {
      alert('Hata: ' + (error.response?.data?.message || error.message));
    }
  };

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={e => e.stopPropagation()}>
        <div className="modal-header">
          <h2>{stajer ? 'Düzenle' : 'Yeni Stajer'}</h2>
          <button onClick={onClose}>×</button>
        </div>

        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label>Ad Soyad *</label>
            <input
              value={formData.fullName}
              onChange={e => setFormData({...formData, fullName: e.target.value})}
              required
            />
          </div>

          <div className="form-group">
            <label>Email *</label>
            <input
              type="email"
              value={formData.email}
              onChange={e => setFormData({...formData, email: e.target.value})}
              required
            />
          </div>

          <div className="form-group">
            <label>Telefon *</label>
            <input
              value={formData.phoneNumber}
              onChange={e => setFormData({...formData, phoneNumber: e.target.value})}
              required
            />
          </div>

          <div className="form-group">
            <label>Üniversite</label>
            <select
              value={formData.universiteID}
              onChange={e => setFormData({...formData, universiteID: e.target.value, bolumID: ''})}
            >
              <option value="">Seçiniz</option>
              {universiteler.map(u => (
                <option key={u.universiteID} value={u.universiteID}>
                  {u.universiteAdi}
                </option>
              ))}
            </select>
          </div>

          <div className="form-group">
            <label>Bölüm</label>
            <select
              value={formData.bolumID}
              onChange={e => setFormData({...formData, bolumID: e.target.value})}
              disabled={!formData.universiteID || loadingBolumler}
            >
              <option value="">
                {loadingBolumler ? 'Yükleniyor...' : 'Önce Üniversite Seçiniz'}
              </option>
              {bolumler.map(b => (
                <option key={b.bolumID} value={b.bolumID}>
                  {b.bolumAdi}
                </option>
              ))}
            </select>
          </div>

          <div className="form-group">
            <label>Departman *</label>
            <select
              value={formData.departmanID}
              onChange={e => setFormData({...formData, departmanID: e.target.value})}
              required
            >
              <option value="">Seçiniz</option>
              {departmanlar.map(d => (
                <option key={d.departmanID} value={d.departmanID}>
                  {d.departmanAdi}
                </option>
              ))}
            </select>
          </div>

          <div className="form-group">
            <label>Başlangıç Tarihi *</label>
            <input
              type="date"
              value={formData.startDate}
              onChange={e => setFormData({...formData, startDate: e.target.value})}
              required
            />
          </div>

          <div className="form-group">
            <label>Bitiş Tarihi *</label>
            <input
              type="date"
              value={formData.endDate}
              onChange={e => setFormData({...formData, endDate: e.target.value})}
              required
            />
          </div>

          <div className="form-group">
            <label>Notlar</label>
            <textarea
              value={formData.notes}
              onChange={e => setFormData({...formData, notes: e.target.value})}
              rows={3}
            />
          </div>

          <div className="modal-actions">
            <button type="button" onClick={onClose}>İptal</button>
            <button type="submit">{stajer ? 'Güncelle' : 'Ekle'}</button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default StajerModal;
```

---

## 🎯 Önemli Noktalar

### 1. Cascade Dropdown (Üniversite → Bölüm)
- MVC'de JavaScript ile yapılıyordu
- React'te `useEffect` ile otomatik yükleniyor

### 2. Form State
- Her input için `onChange` handler
- `useState` ile form state yönetimi

### 3. API Çağrıları
- `backendApi` instance kullanılıyor
- Try-catch ile error handling

### 4. Loading States
- `loading` state ile spinner
- `loadingBolumler` ile dropdown disable

### 5. Validation
- HTML5 `required` attribute
- Backend validation mesajları gösteriliyor

---

## ✅ Sonraki Adımlar

1. CSS stillerini ekle
2. Toast notification sistemi ekle (alert yerine)
3. Pagination ekle (çok kayıt varsa)
4. Loading skeleton ekle
5. Error boundary ekle

---

**Bu örnek, tüm MVC sayfaları için template olarak kullanılabilir!** 🚀

