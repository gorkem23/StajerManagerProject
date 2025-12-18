import { useState, useEffect, useCallback } from 'react'
import stajerService from '../services/stajerService.js'
import { formatDateForInput } from '../utils/dateUtils.js'

export function useStajerForm(initialStajer = null) {
  const [loading, setLoading] = useState(false)
  const [formData, setFormData] = useState({
    fullName: '',
    email: '',
    phoneNumber: '',
    universiteID: '',
    bolumID: '',
    departmanID: '',
    startDate: '',
    endDate: '',
    notes: ''
  })
  const [dropdownData, setDropdownData] = useState({
    departmanlar: [],
    universiteler: [],
    bolumler: []
  })
  const [errors, setErrors] = useState({})

  const loadDropdownData = useCallback(async () => {
    try {
      const data = await stajerService.getDropdownData()
      console.log('Dropdown verileri yüklendi:', data)
      setDropdownData({
        departmanlar: data.departmanlar || [],
        universiteler: data.universiteler || [],
        bolumler: []
      })
    } catch (err) {
      console.error('Dropdown verileri yüklenirken hata:', err)
      setDropdownData({
        departmanlar: [],
        universiteler: [],
        bolumler: []
      })
    }
  }, [])

  const loadBolumler = useCallback(async (universiteId) => {
    try {
      const bolumler = await stajerService.getBolumlerByUniversite(universiteId)
      setDropdownData(prev => ({ ...prev, bolumler: bolumler || [] }))
    } catch (err) {
      console.error('Bölümler yüklenirken hata:', err)
      setDropdownData(prev => ({ ...prev, bolumler: [] }))
    }
  }, [])

  // İlk yüklemede dropdown verilerini getir
  useEffect(() => {
    loadDropdownData()
  }, [loadDropdownData])

  // Üniversite değiştiğinde bölümleri yükle
  useEffect(() => {
    if (formData.universiteID) {
      loadBolumler(formData.universiteID)
    } else {
      setFormData(prev => ({ ...prev, bolumID: '' }))
      setDropdownData(prev => ({ ...prev, bolumler: [] }))
    }
  }, [formData.universiteID, loadBolumler])

  // Edit modunda stajer verilerini formData'ya yükle
  useEffect(() => {
    if (initialStajer) {
      setFormData({
        fullName: initialStajer.fullName || '',
        email: initialStajer.email || '',
        phoneNumber: initialStajer.phoneNumber || '',
        universiteID: initialStajer.universiteID || '',
        bolumID: initialStajer.bolumID || '',
        departmanID: initialStajer.departmanID || '',
        startDate: formatDateForInput(initialStajer.startDate),
        endDate: formatDateForInput(initialStajer.endDate),
        notes: initialStajer.notes || ''
      })
    }
  }, [initialStajer])

  const validateForm = () => {
    const newErrors = {}
    if (!formData.fullName.trim()) newErrors.fullName = 'Ad Soyad gereklidir'
    if (!formData.email.trim()) newErrors.email = 'E-mail gereklidir'
    if (!formData.phoneNumber.trim()) newErrors.phoneNumber = 'Telefon gereklidir'
    if (!formData.departmanID) newErrors.departmanID = 'Departman seçilmelidir'
    if (!formData.startDate) newErrors.startDate = 'Başlangıç tarihi gereklidir'
    if (!formData.endDate) newErrors.endDate = 'Bitiş tarihi gereklidir'

    if (Object.keys(newErrors).length > 0) {
      setErrors(newErrors)
      return false
    }

    // Tarih kontrolü
    if (new Date(formData.startDate) > new Date(formData.endDate)) {
      setErrors({ endDate: 'Bitiş tarihi başlangıç tarihinden önce olamaz' })
      return false
    }

    return true
  }

  const prepareStajerData = () => {
    return {
      fullName: formData.fullName.trim(),
      email: formData.email.trim(),
      phoneNumber: formData.phoneNumber.trim(),
      universiteID: formData.universiteID ? parseInt(formData.universiteID) : null,
      bolumID: formData.bolumID ? parseInt(formData.bolumID) : null,
      departmanID: parseInt(formData.departmanID),
      startDate: formData.startDate,
      endDate: formData.endDate,
      notes: formData.notes.trim() || null
    }
  }

  return {
    formData,
    setFormData,
    dropdownData,
    errors,
    setErrors,
    loading,
    setLoading,
    validateForm,
    prepareStajerData
  }
}

