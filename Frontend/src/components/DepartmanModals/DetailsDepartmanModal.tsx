import { useState, useEffect } from 'react'
import api from '../../lib/api.js'
import { useToast } from '../ToastProvider.jsx'
import { useBootstrapModal } from '../../hooks/useBootstrapModal.js'
import { Departman, Stajer } from '../../types/stajer.types'

interface DetailsDepartmanModalProps {
  show: boolean
  departman: Departman | null
  onClose: () => void
  onEdit?: (departman: Departman) => void
  isAdmin?: boolean
}

function DetailsDepartmanModal({ show, departman, onClose, onEdit, isAdmin = false }: DetailsDepartmanModalProps) {
  const [departmanStajerler, setDepartmanStajerler] = useState<Stajer[]>([])
  const [loadingStajerler, setLoadingStajerler] = useState<boolean>(false)
  const { showToast } = useToast()
  const { modalRef, handleClose: baseHandleClose } = useBootstrapModal(show && !!departman, onClose)

  useEffect(() => {
    if (show && departman) {
      fetchDepartmanStajerler(departman.departmanID, departman.departmanAdi)
    } else {
      setDepartmanStajerler([])
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [show, departman])

  const fetchDepartmanStajerler = async (departmanId: number, departmanAdi: string): Promise<void> => {
    setLoadingStajerler(true)
    try {
      const response = await api.get<{ stajers: Stajer[] }>('/StajersApi', {
        params: {
          sortBy: 'FullName',
          sortOrder: 'asc',
        },
      })

      const stajerler = Array.isArray(response.data?.stajers) ? response.data.stajers : []
      const filtered = stajerler.filter((stajer) => stajer.departmanID === departmanId)

      setDepartmanStajerler(filtered)

      if (filtered.length === 0) {
        console.info(`Departman (${departmanAdi}) için kayıtlı stajer bulunamadı.`)
      }
    } catch (err: any) {
      console.error('Departman stajerleri yüklenirken hata oluştu:', err)
      showToast('Departman stajerleri yüklenirken hata oluştu.', { type: 'error' })
      setDepartmanStajerler([])
    } finally {
      setLoadingStajerler(false)
    }
  }

  if (!show || !departman) {
    return null
  }

  return (
    <div 
      className="modal fade" 
      id="detailsDepartmanModal" 
      tabIndex={-1} 
      aria-labelledby="detailsDepartmanModalLabel" 
      aria-hidden="true"
      ref={modalRef}
    >
      <div className="modal-dialog modal-dialog-centered">
        <div className="modal-content">
          <div className="modal-header">
            <h1 className="modal-title fs-5" id="detailsDepartmanModalLabel">
              <i className="fas fa-building me-2"></i>Departman Detayları
            </h1>
            <button 
              type="button" 
              className="btn-close" 
              data-bs-dismiss="modal" 
              aria-label="Close"
              onClick={baseHandleClose}
            ></button>
          </div>

          <div className="modal-body">
            {/* Departman Bilgileri */}
            <dl className="row mb-4">
              <dt className="col-sm-3">Departman ID:</dt>
              <dd className="col-sm-9">
                <span className="badge bg-primary">{departman.departmanID}</span>
              </dd>

              <dt className="col-sm-3">Departman Adı:</dt>
              <dd className="col-sm-9"><strong>{departman.departmanAdi}</strong></dd>

              <dt className="col-sm-3">Açıklama:</dt>
              <dd className="col-sm-9">
                {departman.aciklama || <span className="text-muted fst-italic">Açıklama yok</span>}
              </dd>
            </dl>

            {/* Stajerler Listesi */}
            <div className="mt-4">
              <h5 className="mb-3">
                <i className="fas fa-users me-2"></i>
                Stajerler 
                <span className="badge bg-secondary ms-2">{departmanStajerler.length}</span>
              </h5>
              
              {loadingStajerler ? (
                <div className="text-center py-4">
                  <div className="spinner-border text-primary" role="status">
                    <span className="visually-hidden">Yükleniyor...</span>
                  </div>
                </div>
              ) : departmanStajerler.length === 0 ? (
                <div className="alert alert-info mb-0">
                  <i className="fas fa-info-circle me-2"></i>
                  Bu departmanda henüz stajer bulunmuyor.
                </div>
              ) : (
                <div className="list-group">
                  {departmanStajerler.map((stajer) => {
                    const universiteAdi = typeof stajer.universite === 'string' 
                      ? stajer.universite 
                      : stajer.universite?.universiteAdi
                    const bolumAdi = typeof stajer.bolum === 'string' 
                      ? stajer.bolum 
                      : stajer.bolum?.bolumAdi
                    
                    return (
                      <div key={stajer.stajerID} className="list-group-item">
                        <div className="d-flex w-100 justify-content-between align-items-start">
                          <div className="flex-grow-1">
                            <h6 className="mb-1">{stajer.fullName}</h6>
                            <p className="mb-1 text-muted small">
                              <i className="fas fa-envelope me-1"></i>
                              {stajer.email}
                            </p>
                            {(stajer.universite || stajer.bolum) && (
                              <p className="mb-0 text-muted small">
                                {universiteAdi || ''}
                                {bolumAdi && (
                                  <span> • {bolumAdi}</span>
                                )}
                              </p>
                            )}
                          </div>
                          {stajer.phoneNumber && (
                            <small className="text-muted">
                              <i className="fas fa-phone me-1"></i>
                              {stajer.phoneNumber}
                            </small>
                          )}
                        </div>
                      </div>
                    )
                  })}
                </div>
              )}
            </div>
          </div>

          <div className="modal-footer">
            <button 
              type="button" 
              className="btn btn-secondary" 
              data-bs-dismiss="modal" 
              onClick={baseHandleClose}
            >
              Kapat
            </button>
            {isAdmin && onEdit && (
              <button 
                type="button" 
                className="btn btn-primary" 
                onClick={() => {
                  baseHandleClose()
                  onEdit(departman)
                }}
              >
                Düzenle
              </button>
            )}
          </div>
        </div>
      </div>
    </div>
  )
}

export default DetailsDepartmanModal

