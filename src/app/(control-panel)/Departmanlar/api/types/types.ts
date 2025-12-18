import { Departman } from "../services/departmanService";

type Universite = {
	universiteID: number;
	universiteAdi: string;
	aciklama?: string;
	adres?: string;
	sehir?: string;
	postaKodu?: string;
	telefon?: string;
	website?: string;
};

type Intern = {
	id: number | string;
	adSoyad?: string;
	email?: string;
	durum?: string;
	universiteAdi?: string;
	stajer?: any; // ✅ EKLENEN: Orijinal stajer objesini saklamak için
};
