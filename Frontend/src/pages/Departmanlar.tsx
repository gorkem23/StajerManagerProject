import { useState, useEffect } from 'react'
import api from '../lib/api'
import { useAuth } from '../contexts/AuthContext'
import { useToast } from '../components/ToastProvider'
import CreateDepartmanModal from '../components/DepartmanModals/CreateDepartmanModal'
import EditDepartmanModal from '../components/DepartmanModals/EditDepartmanModal'
import DetailsDepartmanModal from '../components/DepartmanModals/DetailsDepartmanModal'
import DeleteDepartmanModal from '../components/DepartmanModals/DeleteDepartmanModal'
import { Departman, Stajer } from '../types/stajer.types'

function Departmanlar() {
  // State'ler - TypeScript ile tip güvenliği
  const [departmanlar, setDepartmanlar] = useState<Departman[]>([])
  const [loading, setLoading] = useState<boolean>(true)
  const [error, setError] = useState<string | null>(null)
  const [showCreateModal, setShowCreateModal] = useState<boolean>(false)
  const [showEditModal, setShowEditModal] = useState<boolean>(false)
  const [editingDepartman, setEditingDepartman] = useState<Departman | null>(null)
  const [departmanToDelete, setDepartmanToDelete] = useState<Departman | null>(null)
  const [showDetailsModal, setShowDetailsModal] = useState<boolean>(false)
  const [allStajers, setAllStajers] = useState<Stajer[]>([])
  
  const { user, loading: authLoading } = useAuth()
  const role = ((user as any)?.role ?? '').trim().toLowerCase()
  const email = ((user as any)?.email ?? '').trim().toLowerCase()
  const isAdmin = !authLoading && (role === 'admin' || email === 'admin@stajermanager.com')
  const { showToast } = useToast()

  // Departmanları yükle
  useEffect(() => {
    fetchDepartmanlar()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  const fetchDepartmanlar = async (): Promise<void> => {
    try {
      setLoading(true)
      const response = await api.get<Departman[]>('/DepartmanApi')
      setDepartmanlar(response.data)
      setError(null)
    } catch (err: any) {
      setError('Departmanlar yüklenirken hata oluştu: ' + (err.response?.data?.message || err.message))
      console.error('Error fetching departmanlar:', err)
    } finally {
      setLoading(false)
    }
  }

  const openCreateModal = (): void => setShowCreateModal(true)
  
  const openEditModal = (departman: Departman): void => {
    setEditingDepartman(departman)
    setShowEditModal(true)
  }
  
  const closeModals = (): void => {
    setShowCreateModal(false)
    setShowEditModal(false)
    setEditingDepartman(null)
  }
  
  const handleModalSuccess = async (): Promise<void> => {
    await fetchDepartmanlar()
    closeModals()
  }

  // Detay modalı aç
  const openDetailsModal = (departman: Departman): void => {
    setEditingDepartman(departman)
    setShowDetailsModal(true)
  }
  
  const handleCloseDetailsModal = (): void => {
    setShowDetailsModal(false)
    setEditingDepartman(null)
  }

  // Silme
  const handleDeleteClick = (departman: Departman): void => {
    setDepartmanToDelete(departman)
  }

  const handleDeleteSuccess = async (): Promise<void> => {
    await fetchDepartmanlar()
    setDepartmanToDelete(null)
  }

  useEffect(() => {
    if (departmanlar.length > 0) {
      fetchAllStajers()
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [departmanlar])

  const fetchAllStajers = async (): Promise<void> => {
    try {
      const response = await api.get<{ stajers: Stajer[] }>('/StajersApi', {
        params: {
          sortBy: 'FullName',
          sortOrder: 'asc',
        },
      })
      // API'den gelen stajerleri array olarak al
      const stajerler = Array.isArray(response.data?.stajers) ? response.data.stajers : []
      // Tüm stajerleri state'e kaydet
      setAllStajers(stajerler)
    } catch (err: any) {
      console.error('Stajerler yüklenirken hata:', err)
      // Hata durumunda boş array set et
      setAllStajers([])
    }
  }

  // ✅ Her departman için stajer sayısını hesapla - departmanID'ye göre filtreleme yaparak
  const getStajerCount = (departmanId: number): number => {
    // Tüm stajerler içinden bu departmana ait olanları filtrele ve sayısını döndür
    return allStajers.filter((stajer) => stajer.departmanID === departmanId).length
  }

  if (loading) {
    return (
      <div className="container mt-4">
        <div className="text-center py-4">
          <div className="spinner-border text-primary" role="status">
            <span className="visually-hidden">Yükleniyor...</span>
          </div>
        </div>
      </div>
    )
  }

  return (
    <div className="container-fluid py-5 px-4">
      <div className="mb-4 d-flex justify-content-between align-items-center">
        <h1 className="m-0">Departmanlar</h1>
        {isAdmin && (
          <button className="btn btn-primary" onClick={openCreateModal}>
            <i className="bi bi-plus-lg me-1"></i>
            Yeni Departman
          </button>
        )}
      </div>

      {error && (
        <div className="alert alert-danger" role="alert">
          {error}
        </div>
      )}

      {!loading && (
        <div className='table-responsive w-100 px-0'>
          <table className='table table-hover align-middle w-100'>
            <thead className='table-dark'>
              <tr>
                <th>Departman Adı</th>
                <th className='text-center align-middle'>Açıklama</th>
                <th className='text-center align-middle'>Stajer Sayısı</th>
                {isAdmin && <th className='text-center align-middle'>İşlemler</th>}
                {!isAdmin && <th className='align-middle bi bi-info-circle'></th>}
              </tr>
            </thead>
            <tbody>
              {departmanlar.length === 0 ? (
                <tr>
                  <td colSpan={10} className="text-center py-4 text-muted">
                    {'Henüz stajer eklenmemiş.'}
                  </td>
                </tr>
              ) : (
                departmanlar.map((s) => (
                  <tr key={s.departmanID} className={getStajerCount(s.departmanID) === 0 ? 'table-warning' : ''}>
                    <td>{s.departmanAdi || '-'}</td>
                    <td className='text-center align-middle'>{s.aciklama || '-'}</td>
                    <td className='text-center align-middle'>{getStajerCount(s.departmanID) || '-'}</td>

                    <td className="text-center">
                      <div className="d-flex justify-content-center gap-2">

                        {/* Details */}
                        <button
                          className="btn btn-info btn-sm align-middle bi bi-info-circle"
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
                              <i className="bi bi-vector-pen"></i>
                            </button>

                            <button
                              className="btn btn-danger btn-sm"
                              onClick={() => handleDeleteClick(s)}
                              title="Sil"
                            >
                              <i className="bi bi-trash3 text-dark"></i>
                            </button>
                          </>
                        )}

                      </div>
                    </td>

                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      )}

      {/* Detay Modal */}
      <DetailsDepartmanModal
        show={showDetailsModal}
        departman={editingDepartman}
        onClose={handleCloseDetailsModal}
        onEdit={openEditModal}
        isAdmin={isAdmin}
      />

      {isAdmin && (
        <CreateDepartmanModal
          show={showCreateModal}
          onClose={() => setShowCreateModal(false)}
          onSuccess={handleModalSuccess}
        />
      )}

      {isAdmin && editingDepartman && (
        <EditDepartmanModal
          show={showEditModal}
          departman={editingDepartman}
          onClose={() => {
            setShowEditModal(false)
            setEditingDepartman(null)
          }}
          onSuccess={handleModalSuccess}
        />
      )}

      {/* Delete Modal */}
      <DeleteDepartmanModal
        show={!!departmanToDelete}
        departman={departmanToDelete}
        onClose={() => setDepartmanToDelete(null)}
        onSuccess={handleDeleteSuccess}
      />
    </div>
  )
}

export default Departmanlar

