-- Migration: AddStajerEvaluationModel
-- Tarih: 2025-12-31

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20251231083103_AddStajerEvaluationModel')
BEGIN
    -- Create StajerEvaluations table
    CREATE TABLE [StajerEvaluations] (
        [EvaluationID] int NOT NULL IDENTITY,
        [StajerID] int NOT NULL,
        [EvaluationDate] date NOT NULL,
        [Score] decimal(5,2) NULL,
        [Notes] nvarchar(1000) NULL, -----sil
        [EvaluatedBy] nvarchar(100) NULL, 
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,   -----sil
        CONSTRAINT [PK_StajerEvaluations] PRIMARY KEY ([EvaluationID]),
        CONSTRAINT [FK_StajerEvaluations_Stajers_StajerID] FOREIGN KEY ([StajerID]) REFERENCES [Stajers] ([StajerID]) ON DELETE CASCADE
    );

    -- Create unique index for StajerID and EvaluationDate combination
    CREATE UNIQUE INDEX [IX_StajerEvaluations_StajerID_EvaluationDate] 
    ON [StajerEvaluations] ([StajerID], [EvaluationDate]);

    -- Create index for StajerID
    CREATE INDEX [IX_StajerEvaluations_StajerID] 
    ON [StajerEvaluations] ([StajerID]);

    -- Add migration to history
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20251231083103_AddStajerEvaluationModel', '9.0.0');
END
GO

