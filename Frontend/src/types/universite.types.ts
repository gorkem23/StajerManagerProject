export type Universite = {
  universiteID: number
  universiteAdi: string
  aciklama?: string
}

export type Stajer = {
  stajerID: number
  fullName: string
  email: string
  universiteID: number
  departman?: { departmanAdi?: string }
}

export type User ={
    role?: string
    email?: string
} | null

export type InternDEtail ={
    id: number
    adSoyad: string
    email: string
    durum: string
}