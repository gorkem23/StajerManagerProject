import Fuse, { type IFuseOptions, FuseResult } from 'fuse.js'
//import { Stajer } from '../types/stajer.types'


const fuseOptions: IFuseOptions<Stajer> = {
    keys: [
        {
            name: 'fullName',
            weight: 0.4,
        },
        {
            name: 'email',
            weight: 0.3,
        },
        {
            name: 'phoneNumber',
            weight: 0.1,
        },
        {
            name: 'universite.universiteAdi',
            weight: 0.1,
        },
        {
            name: 'bolum.bolumAdi',
            weight: 0.1,
        },
        {
            name: 'departman.departmanAdi',
            weight: 0.1,
        },
    ],
    threshold: 0.3,
    ignoreLocation: true,
    minMatchCharLength: 2,
    includeScore: true,
    findAllMatches: false,
}

   
export const fuseSearch = (searchText: string, stajers: Stajer[]) => {
    return new Fuse(stajers, fuseOptions)
}

/**
 * Fuse instance oluştur
 * Memoization için kullanılır
 */
export const createFuseInstance = (stajers: Stajer[]): Fuse<Stajer> => {
  return new Fuse(stajers, fuseOptions)
}

export const searchStajers = (
    stajers: Stajer[],
    searchText: string,
    options?: Partial<IFuseOptions<Stajer>>
  ): Stajer[] => {
    // Boş arama metni ise tüm listeyi döndür
    if (!searchText || !searchText.trim()) {
      return stajers
    }
  
    // Fuse instance oluştur (opsiyonel ayarlarla birleştir)
    const fuse = new Fuse(stajers, {
      ...fuseOptions,
      ...options
    })
  
    // Arama yap
    const results = fuse.search(searchText.trim())
  
    // Fuse sonuçları { item, score, refIndex } formatında
    // Sadece item'ları (stajer objelerini) döndür
    return results.map(result => result.item)
  }


  export const sortByRelevance = (
    results: FuseResult<Stajer>[]
  ): Stajer[] => {
    return results
      .sort((a, b) => (a.score ?? 0) - (b.score ?? 0))
      .map(result => result.item)
  }

