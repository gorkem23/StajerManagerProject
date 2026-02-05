-- Migration: AddDosyaFieldsToProjeModel
-- Adds DosyaAdi and DosyaYolu columns to Projeler table

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Projeler]') AND name = 'DosyaAdi')
BEGIN
    ALTER TABLE [dbo].[Projeler]
    ADD [DosyaAdi] nvarchar(255) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Projeler]') AND name = 'DosyaYolu')
BEGIN
    ALTER TABLE [dbo].[Projeler]
    ADD [DosyaYolu] nvarchar(500) NULL;
END
GO

