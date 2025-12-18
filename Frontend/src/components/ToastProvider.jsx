import { createContext, useCallback, useContext, useEffect, useMemo, useState } from 'react'
import './Toast.css'

const ToastContext = createContext({
  showToast: () => {}
})

export function ToastProvider({ children }) {
  const [toast, setToast] = useState(null)

  const hideToast = useCallback(() => {
    setToast(null)
  }, [])

  const showToast = useCallback((message, options = {}) => {
    if (!message) return

    const { type = 'info', duration = 3000 } = options

    setToast({
      id: Date.now(),
      message,
      type,
      duration
    })
  }, [])

  useEffect(() => {
    if (!toast) return

    const timer = setTimeout(() => {
      hideToast()
    }, toast.duration)

    return () => clearTimeout(timer)
  }, [toast, hideToast])

  const value = useMemo(() => ({ showToast }), [showToast])

  return (
    <ToastContext.Provider value={value}>
      {children}
      {toast && (
        <div className={`toast-container toast-${toast.type}`}>
          <div className="toast-message">{toast.message}</div>
        </div>
      )}
    </ToastContext.Provider>
  )
}

export function useToast() {
  return useContext(ToastContext)
}

