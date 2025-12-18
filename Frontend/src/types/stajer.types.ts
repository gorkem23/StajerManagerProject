export interface Universite {
    universiteID?: number;
    universiteAdi?: string;
    aktif?: boolean;
}

export interface Bolum {
    bolumID?: number;
    bolumAdi?: string;
    universiteID?: number;
    aktif?: boolean;
}

export interface Departman {
    departmanID: number;
    departmanAdi: string;
    aciklama?: string;
}

export interface Stajer {
    stajerID: number
    fullName: string
    email: string
    phoneNumber?: string
    startDate?: string
    endDate?: string
    universite?: Universite | string
    bolum?: Bolum | string
    departman?: Departman | string
    notes?: string
    // Backend'den gelebilecek diğer alanlar
    [key: string]: any
  }