import { useEffect, useState } from 'react'
import api from '../../lib/api.js'
import { useToast } from '../ToastProvider.jsx'
import { useBootstrapModal } from '../../hooks/useBootstrapModal.js'

function EditDepartmanModal({ show = false, departman, onClose, onSuccess }) {
  const [formData, setFormData] = useState({
    departmanAdi: '',
    aciklama: ''
  })
  const [loading, setLoading] = useState(false)
  const { showToast } = useToast()
  const { modalRef, handleClose } = useBootstrapModal(show, onClose)

  useEffect(() => {
    if (departman) {
      setFormData({
        departmanAdi: departman.departmanAdi || '',
        aciklama: departman.aciklama || ''
      })
    }
  }, [departman])

  const handleSubmit = async (e) => {
    e.preventDefault()
    if (!departman) return
    if (!formData.departmanAdi.trim()) {
      showToast('Departman adı zorunludur.', { type: 'warning' })
      return
    }

    setLoading(true)
    try {
      const response = await api.put(`/DepartmanApi/${departman.departmanID}`, {
        departmanAdi: formData.departmanAdi.trim(),
        aciklama: formData.aciklama.trim()
      })

      if (response.data.success) {
        showToast(response.data.message || 'Departman güncellendi.', { type: 'success' })
        onSuccess?.()
        handleClose()
      } else {
        showToast(response.data.message || 'Departman güncellenemedi.', { type: 'error' })
      }
    } catch (err) {
      showToast(err.response?.data?.message || err.message || 'Departman güncellenemedi.', {
        type: 'error'
      })
      console.error('Edit departman error:', err)
    } finally {
      setLoading(false)
    }
  }

  if (!show || !departman) {
    return null
  }

  return (
    <div
      className="modal fade"
      id="editDepartmanModal"
      tabIndex="-1"
      aria-labelledby="editDepartmanModalLabel"
      aria-hidden="true"
      ref={modalRef}
    >
      <div className="modal-dialog">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title" id="editDepartmanModalLabel">
              <i className="bi bi-pencil-fill me-2"></i>Departman Düzenle
            </h5>
            <button type="button" className="btn-close" aria-label="Close" onClick={handleClose} />
          </div>

          <form onSubmit={handleSubmit}>
            <div className="modal-body">
              <div className="mb-3">
                <label htmlFor="departmanAdiEdit" className="form-label">
                  Departman Adı *
                </label>
                <input
                  id="departmanAdiEdit"
                  type="text"
                  className="form-control"
                  value={formData.departmanAdi}
                  onChange={(e) => setFormData((prev) => ({ ...prev, departmanAdi: e.target.value }))}
                  maxLength={50}
                  required
                />
              </div>

              <div className="mb-3">
                <label htmlFor="aciklamaEdit" className="form-label">
                  Açıklama
                </label>
                <textarea
                  id="aciklamaEdit"
                  className="form-control"
                  rows={4}
                  maxLength={200}
                  value={formData.aciklama}
                  onChange={(e) => setFormData((prev) => ({ ...prev, aciklama: e.target.value }))}
                />
              </div>
            </div>

            <div className="modal-footer">
              <button type="button" className="btn btn-secondary" onClick={handleClose} disabled={loading}>
                İptal
              </button>
              <button type="submit" className="btn btn-primary" disabled={loading}>
                {loading ? 'Kaydediliyor...' : 'Güncelle'}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  )
}

export default EditDepartmanModal

