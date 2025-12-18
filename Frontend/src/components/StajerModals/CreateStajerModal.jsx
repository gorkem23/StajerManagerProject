import { useStajerForm } from '../../hooks/useStajerForm.js'
import stajerService from '../../services/stajerService.js'
import { useToast } from '../ToastProvider.jsx'
import { useBootstrapModal } from '../../hooks/useBootstrapModal.js'

function CreateStajerModal({ onClose, onSuccess, show = false }) {
  const {
    formData,
    setFormData,
    dropdownData,
    errors,
    setErrors,
    loading,
    setLoading,
    validateForm,
    prepareStajerData
  } = useStajerForm()

  const { showToast } = useToast()
  const { modalRef, handleClose: baseHandleClose } = useBootstrapModal(show, onClose)

  const handleClose = () => {
    baseHandleClose()
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setErrors({})

    if (!validateForm()) {
      return
    }

    setLoading(true)
    try {
      const stajerData = prepareStajerData()
      const response = await stajerService.create(stajerData)
      
      if (response.success) {
        showToast('Stajer başarıyla eklendi.', { type: 'success' })
        onSuccess?.()
        baseHandleClose()
      } else {
        showToast(response.message || 'Stajer eklenemedi.', { type: 'error' })
      }
    } catch (err) {
      // Backend'den gelen hata mesajını al
      const errorData = err.response?.data
      const errorMessage = errorData?.message || 
                          (typeof errorData === 'string' ? errorData : null) ||
                          err.message || 
                          'Stajer eklenirken bir hata oluştu.'
      
      showToast(errorMessage, { type: 'error' })
      console.error('Error creating stajer:', err)
    } finally {
      setLoading(false)
    }
  }

  if (!show) return null

  return (
    <div 
      className="modal fade" 
      id="createStajerModal" 
      tabIndex="-1" 
      aria-labelledby="createStajerModalLabel" 
      aria-hidden="true"
      ref={modalRef}
    >
      <div className="modal-dialog modal-dialog-centered modal-lg">
        <div className="modal-content">
          <div className="modal-header">
            <h1 className="modal-title fs-5" id="createStajerModalLabel">
              <i className="fas fa-plus me-2"></i>Yeni Stajer Ekle
            </h1>
            <button 
              type="button" 
              className="btn-close" 
              data-bs-dismiss="modal" 
              aria-label="Close"
              onClick={handleClose}
            ></button>
          </div>

          <form onSubmit={handleSubmit}>
            <div className="modal-body">
              <div className="row">
                <div className="col-md-6 mb-3">
                  <label htmlFor="fullName" className="form-label">Ad Soyad *</label>
                  <input
                    type="text"
                    id="fullName"
                    className="form-control"
                    value={formData.fullName}
                    onChange={(e) => setFormData({ ...formData, fullName: e.target.value })}
                    required
                    maxLength={30}
                    placeholder="Örn: Ahmet Yılmaz"
                  />
                  {errors.fullName && <div className="text-danger small">{errors.fullName}</div>}
                </div>

                <div className="col-md-6 mb-3">
                  <label htmlFor="email" className="form-label">E-mail *</label>
                  <input
                    type="email"
                    id="email"
                    className="form-control"
                    value={formData.email}
                    onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                    required
                    maxLength={50}
                    placeholder="ornek@email.com"
                  />
                  {errors.email && <div className="text-danger small">{errors.email}</div>}
                </div>
              </div>

              <div className="row">
                <div className="col-md-6 mb-3">
                  <label htmlFor="phoneNumber" className="form-label">Telefon *</label>
                  <input
                    type="tel"
                    id="phoneNumber"
                    className="form-control"
                    value={formData.phoneNumber}
                    onChange={(e) => setFormData({ ...formData, phoneNumber: e.target.value })}
                    required
                    maxLength={10}
                    placeholder="5551234567"
                  />
                  {errors.phoneNumber && <div className="text-danger small">{errors.phoneNumber}</div>}
                </div>
                <div className="col-md-6 mb-3">
                  <label htmlFor="departmanID" className="form-label">Departman *</label>
                  <select
                    id="departmanID"
                    className="form-select"
                    value={formData.departmanID}
                    onChange={(e) => setFormData({ ...formData, departmanID: e.target.value })}
                    required
                  >
                    <option value="">Seçiniz...</option>
                    {dropdownData.departmanlar.map((dep) => (
                      <option key={dep.departmanID} value={dep.departmanID}>
                        {dep.departmanAdi}
                      </option>
                    ))}
                  </select>
                  {errors.departmanID && <div className="text-danger small">{errors.departmanID}</div>}
                </div>
              </div>

              <div className="row">
                <div className="col-md-6 mb-3">
                  <label htmlFor="universiteID" className="form-label">Üniversite</label>
                  <select
                    id="universiteID"
                    className="form-select"
                    value={formData.universiteID}
                    onChange={(e) => setFormData({ ...formData, universiteID: e.target.value, bolumID: '' })}
                  >
                    <option value="">Seçiniz...</option>
                    {dropdownData.universiteler.map((uni) => (
                      <option key={uni.universiteID} value={uni.universiteID}>
                        {uni.universiteAdi}
                      </option>
                    ))}
                  </select>
                </div>
                <div className="col-md-6 mb-3">
                  <label htmlFor="bolumID" className="form-label">Bölüm</label>
                  <select
                    id="bolumID"
                    className="form-select"
                    value={formData.bolumID}
                    onChange={(e) => setFormData({ ...formData, bolumID: e.target.value })}
                    disabled={!formData.universiteID}
                  >
                    <option value="">Seçiniz...</option>
                    {dropdownData.bolumler.map((bol) => (
                      <option key={bol.bolumID} value={bol.bolumID}>
                        {bol.bolumAdi}
                      </option>
                    ))}
                  </select>
                </div>
              </div>

              <div className="row">
                <div className="col-md-6 mb-3">
                  <label htmlFor="startDate" className="form-label">Başlangıç Tarihi *</label>
                  <input
                    type="date"
                    id="startDate"
                    className="form-control"
                    value={formData.startDate}
                    onChange={(e) => setFormData({ ...formData, startDate: e.target.value })}
                    required
                  />
                  {errors.startDate && <div className="text-danger small">{errors.startDate}</div>}
                </div>
                <div className="col-md-6 mb-3">
                  <label htmlFor="endDate" className="form-label">Bitiş Tarihi *</label>
                  <input
                    type="date"
                    id="endDate"
                    className="form-control"
                    value={formData.endDate}
                    onChange={(e) => setFormData({ ...formData, endDate: e.target.value })}
                    required
                    min={formData.startDate || ''}
                  />
                  {errors.endDate && <div className="text-danger small">{errors.endDate}</div>}
                </div>
              </div>

              <div className="mb-3">
                <label htmlFor="notes" className="form-label">Notlar</label>
                <textarea
                  id="notes"
                  className="form-control"
                  value={formData.notes}
                  onChange={(e) => setFormData({ ...formData, notes: e.target.value })}
                  maxLength={400}
                  rows={4}
                  placeholder="Stajer hakkında notlar..."
                />
              </div>
            </div>
            <div className="modal-footer">
              <button type="button" className="btn btn-secondary" data-bs-dismiss="modal" onClick={handleClose}>
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

export default CreateStajerModal
