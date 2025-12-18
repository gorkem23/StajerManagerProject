import { useBootstrapModal } from '../../hooks/useBootstrapModal.js'

function ConfirmModal({ 
  message, 
  title = "Onay", 
  confirmText = "Onayla", 
  cancelText = "İptal",
  onConfirm, 
  onClose,
  confirmButtonType = "primary",
  show = false
}) {
  const { modalRef, handleClose: baseHandleClose } = useBootstrapModal(show, onClose)

  const handleConfirm = () => {
    onConfirm?.()
    baseHandleClose()
  }

  const handleClose = () => {
    baseHandleClose()
  }

  if (!show) return null

  return (
    <div 
      className="modal fade" 
      id="confirmModal" 
      tabIndex="-1" 
      aria-labelledby="confirmModalLabel" 
      aria-hidden="true"
      ref={modalRef}
    >
      <div className="modal-dialog modal-dialog-centered">
        <div className="modal-content">
          <div className="modal-header">
            <h1 className="modal-title fs-5" id="confirmModalLabel">
              {title}
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
            {message || "Bu işlemi yapmak istediğinize emin misiniz?"}
          </div>
          <div className="modal-footer">
            <button 
              type="button" 
              className="btn btn-secondary" 
              data-bs-dismiss="modal"
              onClick={handleClose}
            >
              {cancelText}
            </button>
            <button 
              type="button" 
              className={`btn btn-${confirmButtonType}`}
              onClick={handleConfirm}
            >
              {confirmText}
            </button>
          </div>
        </div>
      </div>
    </div>
  )
}

export default ConfirmModal
