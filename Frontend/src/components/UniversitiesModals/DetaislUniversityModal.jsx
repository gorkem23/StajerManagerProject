import PropTypes from 'prop-types'
import { useBootstrapModal } from '../../hooks/useBootstrapModal.js'

function DetailsUniversityModal({
  show = false,
  university = null,
  interns = [],
  loading = false,
  onClose,
}) {
  const { modalRef, handleClose } = useBootstrapModal(show, onClose)

  if (!university) {
    return null
  }

  return (
    <div
      className="modal fade"
      id="detailsUniversityModal"
      tabIndex="-1"
      aria-labelledby="detailsUniversityModalLabel"
      aria-hidden="true"
      ref={modalRef}
    >
      <div className="modal-dialog modal-dialog-centered">
        <div className="modal-content">
          <div className="modal-header">
            <h1 className="modal-title fs-5" id="detailsUniversityModalLabel">
              <i className="fas fa-building me-2"></i>Üniversite Detayları
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
            <dl className="row mb-4">
              <dt className="col-sm-4">Üniversite ID:</dt>
              <dd className="col-sm-8">
                <span className="badge bg-primary">{university.universiteID}</span>
              </dd>

              <dt className="col-sm-4">Üniversite Adı:</dt>
              <dd className="col-sm-8">
                <strong>{university.universiteAdi}</strong>
              </dd>
            </dl>

            <div>
              <h2 className="h6 mb-3">İlgili Stajyerler</h2>
              {loading ? (
                <div className="text-center py-3">
                  <div className="spinner-border text-primary" role="status">
                    <span className="visually-hidden">Yükleniyor...</span>
                  </div>
                </div>
              ) : interns.length === 0 ? (
                <p className="text-muted fst-italic">Bu üniversiteye ait kayıtlı stajyer yok.</p>
              ) : (
                <ul className="list-group">
                  {interns.map((intern) => (
                    <li key={intern.id} className="list-group-item d-flex justify-content-between">
                      <div>
                        <strong>{intern.adSoyad}</strong>
                        <div className="small text-muted">{intern.email}</div>
                      </div>
                      <span className="badge bg-secondary">{intern.durum || 'Bilinmiyor'}</span>
                    </li>
                  ))}
                </ul>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}

DetailsUniversityModal.propTypes = {
  show: PropTypes.bool,
  university: PropTypes.shape({
    universiteID: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
    universiteAdi: PropTypes.string,
    aciklama: PropTypes.string,
  }),
  interns: PropTypes.arrayOf(
    PropTypes.shape({
      id: PropTypes.oneOfType([PropTypes.string, PropTypes.number]).isRequired,
      adSoyad: PropTypes.string,
      email: PropTypes.string,
      durum: PropTypes.string,
    })
  ),
  loading: PropTypes.bool,
  onClose: PropTypes.func,
}

export default DetailsUniversityModal
