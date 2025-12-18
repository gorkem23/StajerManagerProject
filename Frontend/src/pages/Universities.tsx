import { useState, useEffect } from 'react'
import api from '../lib/api'
import { useAuth } from '../contexts/AuthContext.jsx'
import DetailsUniversityModal from '../components/UniversitiesModals/DetaislUniversityModal.jsx'
import CreateUniversiteModal from '../components/UniversitiesModals/CreateUniversiteModal.jsx'
import DeleteUniversiteModal from '../components/UniversitiesModals/DeleteUniversiteModal.jsx'
import { Universite, Stajer, User, InternDetail } from '../types/universite.types'

function Universities() {
    const [universities, setUniversities] = useState<Universite[]>([])
    const [error, setError] = useState<string | null>(null)
    const [showDetailsModal, setShowDetailsModal] = useState(false)
    const [showCreateModal, setShowCreateModal] = useState(false)
    const [selectedUniversity, setSelectedUniversity] = useState<Universite | null>(null)
    const [detailInterns, setDetailInterns] = useState<InternDetail[]>([])
    const [detailLoading, setDetailLoading] = useState(false)
    const [universiteToDelete, setUniversiteToDelete] = useState<Universite | null>(null)
  
    const { user, loading: authLoading } = useAuth() as { user: User; loading: boolean }

    const role = (user?.role ?? '').trim().toLowerCase()
    const email = (user?.email ?? '').trim().toLowerCase()
    const isAdmin = !authLoading && (role === 'admin' || email === 'admin@stajermanager.com')

    const fetchUniversities = async (): Promise<void> => {
        try {
            const response = await api.get<Universite[]>('/UniversiteApi')
            // API'den gelen veriyi güvenli şekilde işle
            const data = Array.isArray(response.data) ? response.data : []
            setUniversities(data)
            setError(null)
        } catch (err: any) {
            setError('Üniversiteler yüklenirken hata oluştu: ' + (err.response?.data?.message || err.message))
            console.error('Error fetching universities:', err)
            setUniversities([]) // Hata durumunda boş array set et
        }
    }

    const fetchUniversityInterns = async (universiteId: number): Promise<void> => {
        setDetailLoading(true)
        try {
            const response = await api.get<{ stajers: Stajer[] }>('/StajersApi', {
                params: {
                    sortBy: 'FullName',
                    sortOrder: 'asc'
                }
            })

            const stajerler = Array.isArray(response.data?.stajers) ? response.data.stajers : []
            const filtered = stajerler.filter((stajer) => stajer.universiteID === universiteId)

            setDetailInterns(
                filtered.map((stajer) => ({
                    id: stajer.stajerID,
                    adSoyad: stajer.fullName,
                    email: stajer.email,
                    durum: stajer.departman?.departmanAdi ?? ''
                }))
            )
        } catch (err: any) {
            console.error('Üniversite stajerleri yüklenirken hata oluştu:', err)
            setDetailInterns([])
        } finally {
            setDetailLoading(false)
        }
    }

    const openDetailsModal = async (universite: Universite): Promise<void> => {
        setSelectedUniversity(universite)
        setShowDetailsModal(true)
        await fetchUniversityInterns(universite.universiteID)
    }

    const closeDetailsModal = (): void => {
        setShowDetailsModal(false)
        setSelectedUniversity(null)
        setDetailInterns([])
        setDetailLoading(false)
    }

    const openCreateUniversiteModal = (): void => {
        setShowCreateModal(true)
    }

    const closeCreateModal = (): void => {
        setShowCreateModal(false)
    }

    const handleDeleteClick = (universite: Universite): void => {
        setUniversiteToDelete(universite)
    }

    const handleDeleteSuccess = async (): Promise<void> => {
        await fetchUniversities()
        setUniversiteToDelete(null)
    }

    useEffect(() => {
        fetchUniversities()
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [])

    return (
        <div className="container-fluid py-5 px-4 bg-primary-subtle">
            <div className="mb-4 d-flex justify-content-between align-items-center">
                <h1 className="m-0">Üniversiteler</h1>
                {isAdmin && (
                    <div className="mb-4 d-flex justify-content-end align-items-center">
                        <button className="btn btn-primary" onClick={openCreateUniversiteModal}>
                            <i className="bi bi-plus-lg me-1"></i>Yeni Üniversite Ekle
                        </button>
                    </div>
                )}
            </div>

            {error && (
                <div className="alert alert-danger" role="alert">
                    {error}
                </div>
            )}

            <div className="row g-3">
                {universities.length === 0 ? (
                <div className="col-12">
                    <p>Henüz üniversite eklenmemiş.</p>
                </div>
                ) : (
                universities.map((universite) => (
                    <div className="col-md-4" key={universite.universiteID}>
                    <div
                        className="card h-100 shadow-sm"
                        role="button"
                        onClick={() => openDetailsModal(universite)}
                    >
                        <div className="card-header d-flex justify-content-between align-items-center">
                        <h5 className="mb-0">{universite.universiteAdi}</h5>

                        {isAdmin && (
                            <div className="d-flex gap-2" onClick={(e) => e.stopPropagation()}>
                            <button 
                                className="btn btn-sm btn-outline-warning"
                                // onClick={() => openEditModal(universite)}
                            >
                                <i className="bi bi-pencil-fill me-1"></i> Düzenle
                            </button>
                            <button 
                                className="btn btn-sm btn-outline-danger"
                                onClick={() => handleDeleteClick(universite)}
                            >
                                <i className="bi bi-trash me-1"></i> Sil
                            </button>
                            </div>
                        )}
                        </div>

                        <div className="card-body">
                        {universite.aciklama ? (
                            <p className="card-text text-muted">{universite.aciklama}</p>
                        ) : (
                            <p className="card-text text-muted fst-italic">Açıklama yok</p>
                        )}
                        </div>
                    </div>
                    </div>
                ))
                )}
            </div>
            
            <DetailsUniversityModal
                show={showDetailsModal}
                university={selectedUniversity as any}
                interns={detailInterns as any}
                loading={detailLoading}
                onClose={closeDetailsModal}
            />
            
            {isAdmin && (
                <CreateUniversiteModal
                    show={showCreateModal}
                    onClose={closeCreateModal}
                    onSuccess={async () => {
                        await fetchUniversities()
                        closeCreateModal()
                    }}
                />
            )}

            {/* Delete Modal */}
            <DeleteUniversiteModal
                show={!!universiteToDelete}
                universite={universiteToDelete as any}
                onClose={() => setUniversiteToDelete(null)}
                onSuccess={handleDeleteSuccess}
            />
        </div>
    )
}

export default Universities