export const isStajActive = (stajer) => {
  if (!stajer?.startDate || !stajer?.endDate) {
    return false
  }

  const today = new Date()
  today.setHours(0, 0, 0, 0)

  const startDate = new Date(stajer.startDate)
  startDate.setHours(0, 0, 0, 0)

  const endDate = new Date(stajer.endDate)
  endDate.setHours(0, 0, 0, 0)

  return today >= startDate && today <= endDate
}

export const filterActiveStajers = (stajers = []) =>
  stajers.filter(isStajActive)

export const matchesSearchText = (stajer, searchText) => {
  if (!searchText) return true
  const lower = searchText.toLowerCase()

  const candidateFields = [
    stajer.fullName,
    stajer.email,
    stajer.phoneNumber,
    stajer.universite?.universiteAdi || stajer.universite,
    stajer.bolum?.bolumAdi || stajer.bolum,
    stajer.departman?.departmanAdi || stajer.departman
  ]

  return candidateFields.some((field) =>
    typeof field === 'string' && field.toLowerCase().includes(lower)
  )
}

export const filterStajersBySearch = (stajers = [], searchText = '') => {
  const trimmed = searchText.trim()
  if (!trimmed) return [...stajers]
  return stajers.filter((stajer) => matchesSearchText(stajer, trimmed))
}

