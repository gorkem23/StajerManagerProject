import { formatDisplayDate } from '../../utils/dateUtils.js'
import { useBootstrapModal } from '../../hooks/useBootstrapModal.js'
import { isStajActive } from '../../utils/stajerUtils.js'

function DetailsStajerModal({ stajer, onClose, show = false }) {
  const { modalRef, handleClose } = useBootstrapModal(show && !!stajer, onClose)

  if (!stajer) return null

  return (
    <div 
      className={isStajActive(stajer) ? 'modal fade' : 'modal fade modal-danger-fade'}
      id="detailsStajerModal" 
      tabIndex="-1" 
      aria-labelledby="detailsStajerModalLabel" 
      aria-hidden="true"
      ref={modalRef}
    >
      <div className="modal-dialog modal-dialog-centered modal-lg">
        <div className="modal-content">
          <div className="modal-header">
            <h1 className="modal-title fs-5" id="detailsStajerModalLabel">
              <i className="fas fa-eye me-2"></i>Stajer Detayları
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
            <dl className="row">
              <dt className="col-sm-3">Stajer ID:</dt>
              <dd className="col-sm-9"><span className="badge bg-primary">{stajer.stajerID}</span></dd>

              <dt className="col-sm-3">Ad Soyad:</dt>
              <dd className="col-sm-9"><strong>{stajer.fullName}</strong></dd>

              <dt className="col-sm-3">E-mail:</dt>
              <dd className="col-sm-9">{stajer.email}</dd>

              <dt className="col-sm-3">Telefon:</dt>
              <dd className="col-sm-9">{stajer.phoneNumber || '-'}</dd>

              <dt className="col-sm-3">Üniversite:</dt>
              <dd className="col-sm-9">{stajer.universite?.universiteAdi || stajer.universite || '-'}</dd>

              <dt className="col-sm-3">Bölüm:</dt>
              <dd className="col-sm-9">{stajer.bolum?.bolumAdi || stajer.bolum || '-'}</dd>

              <dt className="col-sm-3">Departman:</dt>
              <dd className="col-sm-9">{stajer.departman?.departmanAdi || stajer.departman || '-'}</dd>

              <dt className="col-sm-3">Başlangıç Tarihi:</dt>
              <dd className="col-sm-9">
                {formatDisplayDate(stajer.startDate, 'tr-TR', {
                  year: 'numeric',
                  month: 'long',
                  day: 'numeric'
                })}
              </dd>

              <dt className="col-sm-3">Bitiş Tarihi:</dt>
              <dd className="col-sm-9">
                {formatDisplayDate(stajer.endDate, 'tr-TR', {
                  year: 'numeric',
                  month: 'long',
                  day: 'numeric'
                })}
              </dd>

              {stajer.notes && (
                <>
                  <dt className="col-sm-3">Notlar:</dt>
                  <dd className="col-sm-9" style={{ whiteSpace: 'pre-wrap' }}>{stajer.notes}</dd>
                </>
              )}
            </dl>
          </div>

          <div className="modal-footer">
            <button type="button" className="btn btn-secondary" data-bs-dismiss="modal" onClick={handleClose}>
              Kapat
            </button>
          </div>
        </div>
      </div>
    </div>
  )
}

export default DetailsStajerModal
