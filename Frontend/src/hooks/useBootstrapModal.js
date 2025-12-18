import { useEffect, useRef } from 'react'

/**
 * Bootstrap modal lifecycle yönetimi için custom hook
 * @param {boolean} show - Modal'ın gösterilip gösterilmeyeceği
 * @param {function} onClose - Modal kapandığında çağrılacak callback
 * @returns {object} { modalRef, handleClose }
 */
export function useBootstrapModal(show = false, onClose) {
  const modalRef = useRef(null)
  const modalInstanceRef = useRef(null)

  const cleanupBody = () => {
    document.body.classList.remove('modal-open')
    document.body.style.overflow = ''
    document.body.style.paddingRight = ''
    const backdrop = document.querySelector('.modal-backdrop')
    if (backdrop) backdrop.remove()
  }

  // Modal lifecycle yönetimi
  useEffect(() => {
    if (show) {
      // Modal ref'in hazır olmasını bekle
      let attempts = 0
      const maxAttempts = 10
      const checkAndShow = () => {
        if (modalRef.current) {
          if (window.bootstrap) {
            // Önceki instance varsa dispose et
            if (modalInstanceRef.current) {
              modalInstanceRef.current.dispose()
            }
            modalInstanceRef.current = new window.bootstrap.Modal(modalRef.current, {
              backdrop: 'static',
              keyboard: false
            })
            modalInstanceRef.current.show()
          }
        } else if (attempts < maxAttempts) {
          // Ref henüz hazır değilse, bir sonraki frame'de tekrar dene
          attempts++
          requestAnimationFrame(checkAndShow)
        }
      }
      checkAndShow()
    }

    return () => {
      if (modalInstanceRef.current) {
        modalInstanceRef.current.dispose()
        modalInstanceRef.current = null
      }
      cleanupBody()
    }
  }, [show])

  // Modal hidden event listener
  useEffect(() => {
    const modalElement = modalRef.current
    if (!modalElement) return

    const handleHidden = () => {
      cleanupBody()
      onClose?.()
    }

    modalElement.addEventListener('hidden.bs.modal', handleHidden)
    return () => {
      modalElement.removeEventListener('hidden.bs.modal', handleHidden)
    }
  }, [onClose])

  const handleClose = () => {
    if (modalInstanceRef.current) {
      modalInstanceRef.current.hide()
    }
    setTimeout(cleanupBody, 100)
    onClose?.()
  }

  return { modalRef, handleClose }
}

