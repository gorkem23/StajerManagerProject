const normalizeDateInput = (value) => {
  if (!value) {
    return null
  }
  // Eğer value zaten Date ise tekrar new Date yapmak güvenli
  const date = new Date(value)
  return Number.isNaN(date.getTime()) ? null : date
}

export const formatDisplayDate = (
  value,
  locale = 'tr-TR',
  options = { year: 'numeric', month: '2-digit', day: '2-digit' },
  fallback = '-'
) => {
  const date = normalizeDateInput(value)
  if (!date) {
    return fallback
  }

  try {
    return date.toLocaleDateString(locale, options)
  } catch (err) {
    console.error('Tarih formatlanamadı:', err)
    return fallback
  }
}

export const formatDateForInput = (value) => {
  const date = normalizeDateInput(value)
  if (!date) {
    return ''
  }
  return date.toISOString().split('T')[0]
}

