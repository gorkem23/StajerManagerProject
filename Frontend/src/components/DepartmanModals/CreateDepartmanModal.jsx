import { useState } from 'react'
import api from '../../lib/api.js'
import { useToast } from '../ToastProvider.jsx'
import { useBootstrapModal } from '../../hooks/useBootstrapModal.js'

function CreateDepartmanModal({ show = false, onClose, onSuccess }) {
  const [formData, setFormData] = useState({
    departmanAdi: '',
    aciklama: ''
  })
  const [loading, setLoading] = useState(false)
  const { showToast } = useToast()
  const { modalRef, handleClose: baseHandleClose } = useBootstrapModal(show, onClose)

  const handleClose = () => {
    setFormData({ departmanAdi: '', aciklama: '' })
    baseHandleClose()
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    if (!formData.departmanAdi.trim()) {
      showToast('Departman adı zorunludur.', { type: 'warning' })
      return
    }

    setLoading(true)
    try {
      const response = await api.post('/DepartmanApi', {
        departmanAdi: formData.departmanAdi.trim(),
        aciklama: formData.aciklama.trim()
      })

      if (response.data.success) {
        showToast(response.data.message || 'Departman eklendi.', { type: 'success' })
        onSuccess?.()
        handleClose()
      } else {
        showToast(response.data.message || 'Departman eklenemedi.', { type: 'error' })
      }
    } catch (err) {
      showToast(err.response?.data?.message || err.message || 'Departman eklenemedi.', {
        type: 'error'
      })
      console.error('Create departman error:', err)
    } finally {
      setLoading(false)
    }
  }

  if (!show) {
    return null
  }

  return (
    <div
      className="modal fade"
      id="createDepartmanModal"
      tabIndex="-1"
      aria-labelledby="createDepartmanModalLabel"
      aria-hidden="true"
      ref={modalRef}
    >
      <div className="modal-dialog">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title" id="createDepartmanModalLabel">
              <i className="bi bi-plus-lg me-2"></i>Yeni Departman Ekle
            </h5>
            <button type="button" className="btn-close" aria-label="Close" onClick={handleClose} />
          </div>

          <form onSubmit={handleSubmit}>
            <div className="modal-body">
              <div className="mb-3">
                <label htmlFor="departmanAdiCreate" className="form-label">
                  Departman Adı *
                </label>
                <input
                  id="departmanAdiCreate"
                  type="text"
                  className="form-control"
                  value={formData.departmanAdi}
                  onChange={(e) => setFormData((prev) => ({ ...prev, departmanAdi: e.target.value }))}
                  maxLength={50}
                  required
                  placeholder="Örn: Yazılım Geliştirme"
                />
              </div>

              <div className="mb-3">
                <label htmlFor="aciklamaCreate" className="form-label">
                  Açıklama
                </label>
                <textarea
                  id="aciklamaCreate"
                  className="form-control"
                  rows={4}
                  maxLength={200}
                  value={formData.aciklama}
                  onChange={(e) => setFormData((prev) => ({ ...prev, aciklama: e.target.value }))}
                  placeholder="Departman hakkında kısa bilgi..."
                />
              </div>
            </div>

            <div className="modal-footer">
              <button type="button" className="btn btn-secondary" onClick={handleClose} disabled={loading}>
                İptal
              </button>
              <button type="submit" className="btn btn-primary" disabled={loading}>
                {loading ? 'Kaydediliyor...' : 'Ekle'}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  )
}

export default CreateDepartmanModal

