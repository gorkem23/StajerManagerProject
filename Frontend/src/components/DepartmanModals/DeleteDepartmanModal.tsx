import { useState } from 'react'
import api from '../../lib/api.js'
import { useToast } from '../ToastProvider.jsx'
import { useBootstrapModal } from '../../hooks/useBootstrapModal.js'
import { Departman } from '../../types/stajer.types'

interface DeleteDepartmanModalProps {
  show: boolean
  departman: Departman | null
  onClose: () => void
  onSuccess?: () => void
}

function DeleteDepartmanModal({ show, departman, onClose, onSuccess }: DeleteDepartmanModalProps) {
  const [loading, setLoading] = useState<boolean>(false)
  const { showToast } = useToast()
  const { modalRef, handleClose: baseHandleClose } = useBootstrapModal(show && !!departman, onClose)

  const handleDelete = async (): Promise<void> => {
    if (!departman) return

    setLoading(true)
    try {
      const response = await api.delete<{ success: boolean; message?: string }>(`/DepartmanApi/${departman.departmanID}`)

      if (response.data.success) {
        showToast(response.data.message || 'Departman silindi.', { type: 'success' })
        onSuccess?.()
        baseHandleClose()
      } else {
        showToast(response.data.message || 'Departman silinemedi.', { type: 'error' })
      }
    } catch (err: any) {
      const errorMsg = err.response?.data?.message || err.message
      showToast(errorMsg || 'Departman silinirken bir hata oluştu.', { type: 'error' })
      console.error('Error deleting departman:', err)
    } finally {
      setLoading(false)
    }
  }

  const handleClose = (): void => {
    baseHandleClose()
  }

  if (!show || !departman) {
    return null
  }

  return (
    <div 
      className="modal fade" 
      id="deleteDepartmanModal" 
      tabIndex={-1} 
      aria-labelledby="deleteDepartmanModalLabel" 
      aria-hidden="true"
      ref={modalRef}
    >
      <div className="modal-dialog modal-dialog-centered">
        <div className="modal-content">
          <div className="modal-header">
            <h1 className="modal-title fs-5" id="deleteDepartmanModalLabel">
              <i className="fas fa-trash me-2"></i>Departman Sil
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
            <p>Bu departmanı silmek istediğinize emin misiniz?</p>
            <div className="delete-info">
              <p><strong>Departman Adı:</strong> {departman.departmanAdi}</p>
              <p><strong>Departman ID:</strong> {departman.departmanID}</p>
              {departman.aciklama && (
                <p><strong>Açıklama:</strong> {departman.aciklama}</p>
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

export default DeleteDepartmanModal

