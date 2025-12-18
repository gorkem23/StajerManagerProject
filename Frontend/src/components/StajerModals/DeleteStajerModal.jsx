import { useState } from 'react'
import stajerService from '../../services/stajerService.js'
import { useToast } from '../ToastProvider.jsx'
import { useBootstrapModal } from '../../hooks/useBootstrapModal.js'

function DeleteStajerModal({ stajer, onClose, onSuccess, show = false }) {
  const [loading, setLoading] = useState(false)
  const { showToast } = useToast()
  const { modalRef, handleClose: baseHandleClose } = useBootstrapModal(show, onClose)

  const handleDelete = async () => {
    setLoading(true)
    try {
      const response = await stajerService.delete(stajer.stajerID)
      if (response.success) {
        showToast('Stajer başarıyla silindi.', { type: 'success' })
        onSuccess?.()
        baseHandleClose()
      } else {
        showToast(response.message || 'Silme işlemi başarısız.', { type: 'error' })
      }
    } catch (err) {
      showToast(err.response?.data?.message || err.message || 'Silme işlemi sırasında bir hata oluştu.', {
        type: 'error'
      })
    } finally {
      setLoading(false)
    }
  }

  const handleClose = () => {
    baseHandleClose()
  }

  if (!show) return null

  return (
    <div 
      className="modal fade" 
      id="deleteStajerModal" 
      tabIndex="-1" 
      aria-labelledby="deleteStajerModalLabel" 
      aria-hidden="true"
      ref={modalRef}
    >
      <div className="modal-dialog modal-dialog-centered">
        <div className="modal-content">
          <div className="modal-header">
            <h1 className="modal-title fs-5" id="deleteStajerModalLabel">
              <i className="fas fa-trash me-2"></i>Stajer Sil
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
            <p>Bu stajeri silmek istediğinize emin misiniz?</p>
            <div className="delete-info">
              <p><strong>Ad Soyad:</strong> {stajer.fullName}</p>
              <p><strong>E-mail:</strong> {stajer.email}</p>
              <p><strong>Departman:</strong> {stajer.departman?.departmanAdi || '-'}</p>
            </div>
          </div>

          <div className="modal-footer">
            <button 
              type="button" 
              className="btn btn-secondary" 
              data-bs-dismiss="modal"
              onClick={handleClose}
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

export default DeleteStajerModal
