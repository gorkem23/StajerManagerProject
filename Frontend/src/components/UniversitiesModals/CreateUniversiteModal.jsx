import { useState } from 'react'
import api from '../../lib/api.js'
import { useToast } from '../ToastProvider.jsx'
import { useBootstrapModal } from '../../hooks/useBootstrapModal.js'

function CreateUniversiteModal({ show = false, onClose, onSuccess }) {
  const [formData, setFormData] = useState({
    UniversiteAdi: '', Adres: '', Telefon: '', Website: '', Sehir: '', PostaKodu: ''
  })
  const [loading, setLoading] = useState(false)
  const { showToast } = useToast()
  const { modalRef, handleClose: baseHandleClose } = useBootstrapModal(show, onClose)

  // Helper: Input değerini güncelle
  const updateField = (field) => (e) => setFormData({ ...formData, [field]: e.target.value })

  const handleClose = () => {
    setFormData({ UniversiteAdi: '', Adres: '', Telefon: '', Website: '', Sehir: '', PostaKodu: '' })
    baseHandleClose()
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    if (!formData.UniversiteAdi.trim()) {
      showToast('Üniversite adı zorunludur.', { type: 'warning' })
      return
    }

    setLoading(true)
    try {
      const response = await api.post('/UniversiteApi', {
        UniversiteAdi: formData.UniversiteAdi.trim(),
        Adres: formData.Adres.trim() || null,
        Telefon: formData.Telefon.trim() || null,
        Website: formData.Website.trim() || null,
        Sehir: formData.Sehir.trim() || null,
        PostaKodu: formData.PostaKodu.trim() || null
      })

      if (response.data.success) {
        showToast(response.data.message || 'Üniversite eklendi.', { type: 'success' })
        onSuccess?.()
        handleClose()
      } else {
        showToast(response.data.message || 'Üniversite eklenemedi.', { type: 'error' })
      }
    } catch (err) {
      showToast(err.response?.data?.message || err.message || 'Üniversite eklenemedi.', { type: 'error' })
      console.error('Create universite error:', err)
    } finally {
      setLoading(false)
    }
  }

  if (!show) return null

  return (
    <div className="modal fade" id="createUniversiteModal" tabIndex="-1" ref={modalRef}>
      <div className="modal-dialog modal-lg">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title">
              <i className="bi bi-plus-lg me-2"></i>Yeni Üniversite Ekle
            </h5>
            <button type="button" className="btn-close" onClick={handleClose} />
          </div>
          <form onSubmit={handleSubmit}>
            <div className="modal-body">
              <div className="mb-3">
                <label className="form-label">Üniversite Adı *</label>
                <input
                  type="text"
                  className="form-control"
                  value={formData.UniversiteAdi}
                  onChange={updateField('UniversiteAdi')}
                  required
                  maxLength={100}
                  placeholder="Örn: İstanbul Teknik Üniversitesi"
                />
              </div>
              <div className="row">
                <div className="col-md-6 mb-3">
                  <label className="form-label">Şehir</label>
                  <input
                    type="text"
                    className="form-control"
                    value={formData.Sehir}
                    onChange={updateField('Sehir')}
                    maxLength={50}
                    placeholder="Örn: İstanbul"
                  />
                </div>
                <div className="col-md-6 mb-3">
                  <label className="form-label">Posta Kodu</label>
                  <input
                    type="text"
                    className="form-control"
                    value={formData.PostaKodu}
                    onChange={updateField('PostaKodu')}
                    maxLength={10}
                    placeholder="34000"
                  />
                </div>
              </div>
              <div className="mb-3">
                <label className="form-label">Adres</label>
                <textarea
                  className="form-control"
                  rows={3}
                  value={formData.Adres}
                  onChange={updateField('Adres')}
                  maxLength={200}
                  placeholder="Üniversite adresi..."
                />
              </div>
              <div className="row">
                <div className="col-md-6 mb-3">
                  <label className="form-label">Telefon</label>
                  <input
                    type="tel"
                    className="form-control"
                    value={formData.Telefon}
                    onChange={updateField('Telefon')}
                    maxLength={20}
                    placeholder="0212 123 45 67"
                  />
                </div>
                <div className="col-md-6 mb-3">
                  <label className="form-label">Website</label>
                  <input
                    type="url"
                    className="form-control"
                    value={formData.Website}
                    onChange={updateField('Website')}
                    maxLength={100}
                    placeholder="https://www.itu.edu.tr"
                  />
                </div>
              </div>
            </div>
            <div className="modal-footer">
              <button type="button" className="btn btn-secondary" onClick={handleClose} disabled={loading}>
                İptal
              </button>
              <button type="submit" className="btn btn-primary" disabled={loading}>
                {loading ? 'Ekleniyor...' : 'Ekle'}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  )
}

export default CreateUniversiteModal

