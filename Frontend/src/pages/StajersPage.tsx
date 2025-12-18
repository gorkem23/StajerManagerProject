// Frontend/src/pages/StajersPage.tsx

import { useState, useEffect, useCallback } from 'react'
import stajerService from '../services/stajerService.js'
import CreateStajerModal from '../components/StajerModals/CreateStajerModal'
import EditStajerModal from '../components/StajerModals/EditStajerModal'
import DeleteStajerModal from '../components/StajerModals/DeleteStajerModal'
import DetailsStajerModal from '../components/StajerModals/DetailsStajerModal'
import { useAuth } from '../contexts/AuthContext'
import { isStajActive } from '../utils/stajerUtils.js'
import { searchStajers } from '../utils/fuseSearch'
import { Stajer } from '../types/stajer.types'

// Sıralama kolonları için type
type SortColumn = 'StajerID' | 'FullName' | 'Universite' | 'Bolum' | 'Departman' | 'StartDate' | 'EndDate'
type SortOrder = 'asc' | 'desc'

function StajersPage() {
  console.log('StajersPage component rendered')
  
  // TypeScript
  const [stajers, setStajers] = useState<Stajer[]>([])
  const [allStajers, setAllStajers] = useState<Stajer[]>([])
  const [loading, setLoading] = useState<boolean>(true)
  const [error, setError] = useState<string | null>(null)
  const [searchText, setSearchText] = useState<string>('')
  const [sortBy, setSortBy] = useState<SortColumn>('StajerID')
  const [sortOrder, setSortOrder] = useState<SortOrder>('desc')
  const [totalCount, setTotalCount] = useState<number>(0)
  

  const [showCreateModal, setShowCreateModal] = useState<boolean>(false)
  const [showEditModal, setShowEditModal] = useState<boolean>(false)
  const [showDeleteModal, setShowDeleteModal] = useState<boolean>(false)
  const [showDetailsModal, setShowDetailsModal] = useState<boolean>(false)
  const [selectedStajer, setSelectedStajer] = useState<Stajer | null>(null)
  
  const { user, loading: authLoading } = useAuth()
  const [IsActive, setIsActive] = useState<boolean>(false)
  
  // Admin kontrolü - type-safe
  const role = ((user as any)?.role ?? '').trim().toLowerCase()
  const email = ((user as any)?.email ?? '').trim().toLowerCase()
  const isAdmin = !authLoading && (role === 'admin' || email === 'admin@stajermanager.com')

  /**
   * Stajerleri filtrele - Fuse.js ile arama
   * 
   * useCallback ile memoize edilmiş fonksiyon
   * Sadece bağımlılıklar değiştiğinde yeniden oluşturulur
   */
  const filterStajers = useCallback((stajersList: Stajer[]): Stajer[] => {
    let filtered = [...stajersList]
    
    // 1. Aktif stajer filtresi (eski mantık korunuyor)
    if (IsActive) {
      filtered = filtered.filter(isStajActive)
    }
    
    // 2. Fuse.js ile arama
    if (searchText.trim()) {
      // Fuse.js kullanarak fuzzy search yap
      filtered = searchStajers(filtered, searchText.trim())
    }
    
    return filtered
  }, [IsActive, searchText])

  // User için otomatik aktif filtre
  useEffect(() => {
    if (!isAdmin) {
      setIsActive(true) // User için her zaman aktif filtre açık
    }
  }, [isAdmin])

  // İlk yükleme - sıralama değiştiğinde
  useEffect(() => {
    loadStajers()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [sortBy, sortOrder])

  // IsActive veya searchText değiştiğinde filtrele
  useEffect(() => {
    if (allStajers.length > 0) {
      const filtered = filterStajers(allStajers)
      setStajers(filtered)
      setTotalCount(filtered.length)
    }
  }, [IsActive, searchText, allStajers, filterStajers])

  /**
   * Stajerleri yükle
   * Backend'den veri çeker ve state'e set eder
   */
  const loadStajers = async (): Promise<void> => {
    try {
      console.log('loadStajers called')
      setLoading(true)
      setError(null)
      const data = await stajerService.getAll(sortBy, sortOrder, '')
      console.log('Stajer data received:', data)
      
      if (data && data.stajers) {
        setAllStajers(data.stajers as Stajer[])
        const filtered = filterStajers(data.stajers as Stajer[])
        setStajers(filtered)
        setTotalCount(filtered.length)
      } else if (Array.isArray(data)) {
        setAllStajers(data as Stajer[])
        const filtered = filterStajers(data as Stajer[])
        setStajers(filtered)
        setTotalCount(filtered.length)
      } else {
        setAllStajers([])
        setStajers([])
        setTotalCount(0)
      }
    } catch (err: any) {
      console.error('Stajer yükleme hatası:', err)
      setError('Stajerler yüklenirken hata oluştu: ' + (err.response?.data?.message || err.message))
      setAllStajers([])
      setStajers([])
    } finally {
      setLoading(false)
    }
  }

  // Arama fonksiyonu - artık useEffect otomatik yapıyor
  const handleSearch = (): void => {
    // useEffect otomatik filtreleyecek
  }

  // Arama temizle
  const handleClearSearch = (): void => {
    setSearchText('')
    //setIsActive(false)
  }

  // Sıralama fonksiyonu
  const handleSort = (column: SortColumn): void => {
    if (sortBy === column) {
      // Aynı kolona tıklandıysa yönü değiştir
      setSortOrder(sortOrder === 'asc' ? 'desc' : 'asc')
    } else {
      // Farklı kolona tıklandıysa yeni kolon ve asc yön
      setSortBy(column)
      setSortOrder('asc')
    }
  }

  // Modal açma fonksiyonları
  const openCreateModal = (): void => {
    if (!isAdmin) return
    setSelectedStajer(null)
    setShowCreateModal(true)
  }

  const openEditModal = (stajer: Stajer): void => {
    if (!isAdmin) return
    setSelectedStajer(stajer)
    setShowEditModal(true)
  }

  const openDeleteModal = (stajer: Stajer): void => {
    if (!isAdmin) return
    setSelectedStajer(stajer)
    setShowDeleteModal(true)
  }

  const openDetailsModal = (stajer: Stajer): void => {
    setSelectedStajer(stajer)
    setShowDetailsModal(true)
  }

  // Modal kapatma
  const closeModals = (): void => {
    setShowCreateModal(false)
    setShowEditModal(false)
    setShowDeleteModal(false)
    setShowDetailsModal(false)
    setSelectedStajer(null)
  }

  // Başarılı işlem sonrası
  const handleSuccess = (): void => {
    closeModals()
    loadStajers()
  }

  // Tarih formatla
  const formatDate = (dateString?: string): string => {
    if (!dateString) return '-'
    try {
      const date = new Date(dateString)
      if (isNaN(date.getTime())) return dateString
      return date.toLocaleDateString('tr-TR')
    } catch {
      return dateString
    }
  }

  return (
    <div className="stajers-page">
      <div className="container-fluid py-5 px-0">
        <div className="stajers-header d-flex justify-content-between align-items-center mb-4">
          <h1 className="h3 m-0">
            <i className="fas fa-user-graduate text-success me-2"></i>
            STAJERLER
          </h1>
          {isAdmin && (
            <button className="btn btn-success d-flex align-items-center gap-1" onClick={openCreateModal}>
              <i className="fas fa-plus"></i>
              Yeni Stajer Ekle
            </button>
          )}
        </div>

        {/* Arama Paneli */}
        <div className="card mb-4">
          <div className="card-body">
            <div className="d-flex flex-wrap gap-3 align-items-center w-100">
              <div className="input-group flex-grow-1">
                <span className="input-group-text">
                  <i className="fas fa-search"></i>
                </span>
                <input
                  type="text"
                  className="form-control"
                  placeholder="Ad, soyad, email, telefon, üniversite, bölüm veya departman ara..."
                  value={searchText}
                  onChange={(e) => setSearchText(e.target.value)}
                  onKeyPress={(e) => e.key === 'Enter' && handleSearch()}
                />
              </div>

              <div className="d-flex flex-wrap gap-2">
                <button className="btn btn-primary" onClick={handleSearch}>
                  <i className="fas fa-search me-2"></i>
                  Ara
                </button>
                <button className="btn btn-secondary" onClick={handleClearSearch}>
                  <i className="fas fa-times me-1"></i>
                  Temizle
                </button>
              </div>

              {isAdmin && (
                <div className="form-check form-switch ms-auto">
                  <input
                    className="form-check-input"
                    type="checkbox"
                    role="switch"
                    id="switchCheckDefault"
                    checked={IsActive}
                    onChange={(e) => setIsActive(e.target.checked)}
                  />
                  <label className="form-check-label ms-2" htmlFor="switchCheckDefault">
                    {IsActive ? 'Tüm stajerleri göster' : 'Yalnızca stajı devam edenleri göster'}
                  </label>
                </div>
              )}
            </div>
          </div>
        </div>

      {searchText && (
        <div className="alert alert-info d-flex justify-content-between align-items-center">
          <div>
            <i className="fas fa-info-circle me-2"></i>
            "<strong>{searchText}</strong>" için arama sonuçları:
            <span className="badge bg-primary ms-1">{totalCount}</span> stajer bulundu.
          </div>

          <button className="btn btn-sm btn-outline-danger" onClick={handleClearSearch}>
            <i className="fas fa-times me-1"></i>Aramayı Temizle
          </button>
        </div>
      )}

      {/* Loading */}
      {loading && (
        <div className="text-center my-4">
          <div className="spinner-border text-primary"></div>
          <p className="mt-2">Veriler yükleniyor...</p>
        </div>
      )}

      {/* Error */}
      {error && (
        <div className="alert alert-danger d-flex justify-content-between">
          <span>{error}</span>
          <button className="btn btn-outline-light btn-sm" onClick={loadStajers}>
            Veriler Yüklenemedi. Tekrar Dene
          </button>
        </div>
      )}

      {/* Tablo */}
      {!loading && (
        <div className="table-responsive w-100 px-0">
          <table className="table table-hover align-middle w-100">

            <thead className="table-light">
              <tr>
                <th>
                  <button className="btn btn-link p-0" onClick={() => handleSort('StajerID')}>
                    <i className="fa-solid fa-ranking-star me-1"></i>
                    Oluşturma
                  </button>
                </th>

                <th>
                  <button className="btn btn-link p-0" onClick={() => handleSort('FullName')}>
                    <i className="fas fa-user me-1"></i>
                    Ad Soyad
                  </button>
                </th>

                <th><i className="fas fa-envelope me-1"></i>E-mail</th>
                <th><i className="fas fa-phone me-1"></i>Telefon</th>

                <th>
                  <button className="btn btn-link p-0" onClick={() => handleSort('Universite')}>
                    <i className="fas fa-university me-1"></i>Üniversite
                  </button>
                </th>

                <th>
                  <button className="btn btn-link p-0" onClick={() => handleSort('Bolum')}>
                    <i className="fas fa-book me-1"></i>Bölüm
                  </button>
                </th>

                <th>
                  <button className="btn btn-link p-0" onClick={() => handleSort('Departman')}>
                    <i className="fas fa-graduation-cap me-1"></i>Departman
                  </button>
                </th>

                <th>
                  <button className="btn btn-link p-0" onClick={() => handleSort('StartDate')}>
                    <i className="fas fa-calendar-alt me-1"></i>Başlangıç
                  </button>
                </th>

                <th>
                  <button className="btn btn-link p-0" onClick={() => handleSort('EndDate')}>
                    <i className="fas fa-calendar-check me-1"></i>Bitiş
                  </button>
                </th>

                <th className="text-center">
                  <i className="fas fa-cogs me-1"></i>İşlemler
                </th>
              </tr>
            </thead>

            <tbody>
              {stajers.length === 0 ? (
                <tr>
                  <td colSpan={10} className="text-center py-4 text-muted">
                    {searchText ? 'Arama sonucu bulunamadı.' : 'Henüz stajer eklenmemiş.'}
                  </td>
                </tr>
              ) : (
                stajers.map((s) => {
                  const universiteAdi = typeof s.universite === 'string' ? s.universite : s.universite?.universiteAdi
                  const bolumAdi = typeof s.bolum === 'string' ? s.bolum : s.bolum?.bolumAdi
                  const departmanAdi = typeof s.departman === 'string' ? s.departman : s.departman?.departmanAdi
                  
                  return (
                  <tr key={s.stajerID} className={!isStajActive(s) ? 'table-danger' : ''}>
                    <td><span className="badge bg-secondary">{s.stajerID}</span></td>
                    <td>{s.fullName}</td>
                    <td>{s.email}</td>
                    <td>{s.phoneNumber}</td>
                    <td>{universiteAdi || '-'}</td>
                    <td>{bolumAdi || '-'}</td>
                    <td>{departmanAdi || '-'}</td>
                    <td>{formatDate(s.startDate)}</td>
                    <td>{formatDate(s.endDate)}</td>

                    <td className="text-center">
                      <div className="d-flex justify-content-center gap-2">

                        {/* Details */}
                        <button
                          className="btn btn-info btn-sm"
                          onClick={() => openDetailsModal(s)}
                          title="Detaylar"
                        >
                          <i className="fas fa-eye"></i>
                        </button>

                        {/* Edit & Delete only for Admin */}
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
                  )
                })
              )}
            </tbody>
          </table>
        </div>
      )}

      </div>

      {/* Modals */}
      {isAdmin && (
        <CreateStajerModal
          show={showCreateModal}
          onClose={closeModals}
          onSuccess={handleSuccess}
        />
      )}

      {isAdmin && selectedStajer && (
        <EditStajerModal
          stajer={selectedStajer}
          show={showEditModal}
          onClose={closeModals}
          onSuccess={handleSuccess}
        />
      )}

      {isAdmin && selectedStajer && (
        <DeleteStajerModal
          stajer={selectedStajer}
          show={showDeleteModal}
          onClose={closeModals}
          onSuccess={handleSuccess}
        />
      )}

      {selectedStajer && (
        <DetailsStajerModal
          stajer={selectedStajer}
          show={showDetailsModal}
          onClose={closeModals}
        />
      )}
    </div>
  )
}

export default StajersPage