-- Test verisi ekleme scripti
USE StajerManager;

-- Departmanlar zaten var, kontrol edelim
SELECT COUNT(*) as DepartmanSayisi FROM Departmans;

-- Üniversiteler ekle - 56 üniversite
IF NOT EXISTS (SELECT 1 FROM Universiteler WHERE UniversiteAdi = 'İstanbul Teknik Üniversitesi')
BEGIN
    INSERT INTO Universiteler (UniversiteAdi, Sehir, Aktif, OlusturmaTarihi) VALUES 
    ('İstanbul Teknik Üniversitesi', 'İstanbul', 1, GETDATE()),
    ('Boğaziçi Üniversitesi', 'İstanbul', 1, GETDATE()),
    ('Orta Doğu Teknik Üniversitesi', 'Ankara', 1, GETDATE()),
    ('Hacettepe Üniversitesi', 'Ankara', 1, GETDATE()),
    ('İstanbul Üniversitesi', 'İstanbul', 1, GETDATE()),
    ('Ankara Üniversitesi', 'Ankara', 1, GETDATE()),
    ('Ege Üniversitesi', 'İzmir', 1, GETDATE()),
    ('Dokuz Eylül Üniversitesi', 'İzmir', 1, GETDATE()),
    ('Marmara Üniversitesi', 'İstanbul', 1, GETDATE()),
    ('Yıldız Teknik Üniversitesi', 'İstanbul', 1, GETDATE()),
    ('Galatasaray Üniversitesi', 'İstanbul', 1, GETDATE()),
    ('Sabancı Üniversitesi', 'İstanbul', 1, GETDATE()),
    ('Koç Üniversitesi', 'İstanbul', 1, GETDATE()),
    ('Bahçeşehir Üniversitesi', 'İstanbul', 1, GETDATE()),
    ('İstanbul Bilgi Üniversitesi', 'İstanbul', 1, GETDATE()),
    ('Kadir Has Üniversitesi', 'İstanbul', 1, GETDATE()),
    ('Acıbadem Üniversitesi', 'İstanbul', 1, GETDATE()),
    ('Bezmialem Vakıf Üniversitesi', 'İstanbul', 1, GETDATE()),
    ('Gazi Üniversitesi', 'Ankara', 1, GETDATE()),
    ('Bilkent Üniversitesi', 'Ankara', 1, GETDATE()),
    ('Atılım Üniversitesi', 'Ankara', 1, GETDATE()),
    ('Çankaya Üniversitesi', 'Ankara', 1, GETDATE()),
    ('Başkent Üniversitesi', 'Ankara', 1, GETDATE()),
    ('TED Üniversitesi', 'Ankara', 1, GETDATE()),
    ('TOBB Ekonomi ve Teknoloji Üniversitesi', 'Ankara', 1, GETDATE()),
    ('Yıldırım Beyazıt Üniversitesi', 'Ankara', 1, GETDATE()),
    ('Ufuk Üniversitesi', 'Ankara', 1, GETDATE()),
    ('Yaşar Üniversitesi', 'İzmir', 1, GETDATE()),
    ('İzmir Ekonomi Üniversitesi', 'İzmir', 1, GETDATE()),
    ('İzmir Yüksek Teknoloji Enstitüsü', 'İzmir', 1, GETDATE()),
    ('Gediz Üniversitesi', 'İzmir', 1, GETDATE()),
    ('Bursa Teknik Üniversitesi', 'Bursa', 1, GETDATE()),
    ('Uludağ Üniversitesi', 'Bursa', 1, GETDATE()),
    ('Bursa Orhangazi Üniversitesi', 'Bursa', 1, GETDATE()),
    ('Çukurova Üniversitesi', 'Adana', 1, GETDATE()),
    ('Mersin Üniversitesi', 'Mersin', 1, GETDATE()),
    ('Karadeniz Teknik Üniversitesi', 'Trabzon', 1, GETDATE()),
    ('Atatürk Üniversitesi', 'Erzurum', 1, GETDATE()),
    ('Fırat Üniversitesi', 'Elazığ', 1, GETDATE()),
    ('İnönü Üniversitesi', 'Malatya', 1, GETDATE()),
    ('Erciyes Üniversitesi', 'Kayseri', 1, GETDATE()),
    ('Selçuk Üniversitesi', 'Konya', 1, GETDATE()),
    ('Akdeniz Üniversitesi', 'Antalya', 1, GETDATE()),
    ('Pamukkale Üniversitesi', 'Denizli', 1, GETDATE()),
    ('Süleyman Demirel Üniversitesi', 'Isparta', 1, GETDATE()),
    ('Kocaeli Üniversitesi', 'Kocaeli', 1, GETDATE()),
    ('Sakarya Üniversitesi', 'Sakarya', 1, GETDATE()),
    ('Trakya Üniversitesi', 'Edirne', 1, GETDATE()),
    ('Ondokuz Mayıs Üniversitesi', 'Samsun', 1, GETDATE()),
    ('Gaziantep Üniversitesi', 'Gaziantep', 1, GETDATE()),
    ('Harran Üniversitesi', 'Şanlıurfa', 1, GETDATE()),
    ('Dicle Üniversitesi', 'Diyarbakır', 1, GETDATE()),
    ('Van Yüzüncü Yıl Üniversitesi', 'Van', 1, GETDATE()),
    ('Kastamonu Üniversitesi', 'Kastamonu', 1, GETDATE()),
    ('Zonguldak Bülent Ecevit Üniversitesi', 'Zonguldak', 1, GETDATE()),
    ('Bartın Üniversitesi', 'Bartın', 1, GETDATE());
END

-- Bölümler ekle - Mevcut veritabanındaki gerçek veriler
IF NOT EXISTS (SELECT 1 FROM Bolumler WHERE BolumAdi = 'Bilgisayar Mühendisliği' AND UniversiteID = 1)
BEGIN
    INSERT INTO Bolumler (BolumAdi, UniversiteID, BolumKodu, Fakulte, EgitimTuru, OlusturmaTarihi, Aktif) VALUES 
    ('Bilgisayar Mühendisliği', 1, 'BM', 'Bilgisayar ve Bilişim Fakültesi', 'Lisans', GETDATE(), 1),
    ('Yazılım Mühendisliği', 1, 'YM', 'Bilgisayar ve Bilişim Fakültesi', 'Lisans', GETDATE(), 1),
    ('Elektrik Mühendisliği', 1, 'ELM', 'Elektrik-Elektronik Fakültesi', 'Lisans', GETDATE(), 1),
    ('Endüstri Mühendisliği', 2, 'EM', 'Mühendislik Fakültesi', 'Lisans', GETDATE(), 1),
    ('Matematik', 2, 'MAT', 'Fen-Edebiyat Fakültesi', 'Lisans', GETDATE(), 1),
    ('Fizik', 2, 'FIZ', 'Fen-Edebiyat Fakültesi', 'Lisans', GETDATE(), 1),
    ('Bilgisayar Mühendisliği', 2, 'BM', 'Mühendislik Fakültesi', 'Lisans', GETDATE(), 1),
    ('Elektrik Mühendisliği', 3, 'ELM', 'Mühendislik Fakültesi', 'Lisans', GETDATE(), 1),
    ('Makine Mühendisliği', 3, 'MM', 'Mühendislik Fakültesi', 'Lisans', GETDATE(), 1),
    ('İnşaat Mühendisliği', 3, 'IM', 'Mühendislik Fakültesi', 'Lisans', GETDATE(), 1);
END

-- Test stajerleri ekle
IF NOT EXISTS (SELECT 1 FROM Stajers WHERE Email = 'ahmet.yilmaz@test.com')
BEGIN
    INSERT INTO Stajers (FullName, Email, PhoneNumber, UniversiteID, BolumID, DepartmanID, StartDate, EndDate, Notes) VALUES 
    ('Ahmet Yılmaz', 'ahmet.yilmaz@test.com', '0532123456', 1, 51, 1, '2025-01-15', '2025-02-15', 'Başarılı bir stajer'),
    ('Ayşe Demir', 'ayse.demir@test.com', '0532987654', 2, 54, 2, '2025-01-20', '2025-02-20', 'Çok çalışkan'),
    ('Mehmet Kaya', 'mehmet.kaya@test.com', '0533555443', 3, 58, 4, '2025-02-01', '2025-03-01', 'Teknik bilgisi güçlü'),
    ('Fatma Özkan', 'fatma.ozkan@test.com', '0533777889', 3, 59, 5, '2025-02-10', '2025-03-10', 'Takım çalışmasına uyumlu'),
    ('Ali Çelik', 'ali.celik@test.com', '0533111223', 1, 52, 6, '2025-02-15', '2025-03-15', 'Yaratıcı fikirler üretiyor');
END

-- Kontrol sorguları
SELECT 'Stajer Sayısı' as Tablo, COUNT(*) as KayitSayisi FROM Stajers
UNION ALL
SELECT 'Departman Sayısı', COUNT(*) FROM Departmans
UNION ALL
SELECT 'Üniversite Sayısı', COUNT(*) FROM Universiteler
UNION ALL
SELECT 'Bölüm Sayısı', COUNT(*) FROM Bolumler;
