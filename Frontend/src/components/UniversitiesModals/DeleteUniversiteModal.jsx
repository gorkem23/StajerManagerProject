import { useState } from 'react'
import api from '../../lib/api.js'
import { useToast } from '../ToastProvider.jsx'
import { useBootstrapModal } from '../../hooks/useBootstrapModal.js'

function DeleteUniversiteModal({ show = false, universite, onClose, onSuccess }) {
  const [loading, setLoading] = useState(false)
  const { showToast } = useToast()
  const { modalRef, handleClose: baseHandleClose } = useBootstrapModal(show && !!universite, onClose)

  const handleDelete = async () => {
    if (!universite) return

    setLoading(true)
    try {
      const response = await api.delete(`/UniversiteApi/${universite.universiteID}`)

      if (response.data.success) {
        showToast(response.data.message || 'Üniversite silindi.', { type: 'success' })
        onSuccess?.()
        baseHandleClose()
      } else {
        showToast(response.data.message || 'Üniversite silinemedi.', { type: 'error' })
      }
    } catch (err) {
      const errorMsg = err.response?.data?.message || err.message
      showToast(errorMsg || 'Üniversite silinirken bir hata oluştu.', { type: 'error' })
      console.error('Error deleting universite:', err)
    } finally {
      setLoading(false)
    }
  }

  const handleClose = () => {
    baseHandleClose()
  }

  if (!show || !universite) {
    return null
  }

  return (
    <div 
      className="modal fade" 
      id="deleteUniversiteModal" 
      tabIndex="-1" 
      aria-labelledby="deleteUniversiteModalLabel" 
      aria-hidden="true"
      ref={modalRef}
    >
      <div className="modal-dialog modal-dialog-centered">
        <div className="modal-content">
          <div className="modal-header">
            <h1 className="modal-title fs-5" id="deleteUniversiteModalLabel">
              <i className="fas fa-trash me-2"></i>Üniversite Sil
            </h1>
            <button 
              type="button" 
              className="btn-close" 
              data-bs-dismiss="modal" 
              aria-label="Close"
              onClick={handleClose}
            ></button>
          </div>

          <div className="modal-body">
            <p>Bu üniversiteyi silmek istediğinize emin misiniz?</p>
            <div className="delete-info">
              <p><strong>Üniversite Adı:</strong> {universite.universiteAdi}</p>
              <p><strong>Üniversite ID:</strong> {universite.universiteID}</p>
              {universite.aciklama && (
                <p><strong>Açıklama:</strong> {universite.aciklama}</p>
              )}
            </div>
          </div>

          <div className="modal-footer">
            <button 
              type="button" 
              className="btn btn-secondary" 
              data-bs-dismiss="modal"
              onClick={handleClose}
              disabled={loading}
            >
              İptal
            </button>
            <button 
              type="button" 
              className="btn btn-danger" 
              onClick={handleDelete}
              disabled={loading}
            >
              {loading ? 'Siliniyor...' : 'Sil'}
            </button>
          </div>
        </div>
      </div>
    </div>
  )
}

export default DeleteUniversiteModal

