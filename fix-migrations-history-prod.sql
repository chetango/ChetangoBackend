-- ===========================================================================
-- REGISTRAR MIGRACIONES EN __EFMigrationsHistory - PRODUCCIÓN
-- Fecha: 2026-03-06
-- Motivo: Las migraciones multi-tenancy fueron aplicadas con scripts SQL manuales
--         pero no quedaron registradas en __EFMigrationsHistory.
--         EF intenta re-aplicarlas al arrancar → falla → app no inicia → 502/CORS.
--
-- INSTRUCCIONES:
--   1. Ejecutar este script PRIMERO en la BD de producción
--   2. Luego reiniciar el App Service (o esperar el redeploy)
-- ===========================================================================

-- Ver estado actual
SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory ORDER BY MigrationId;
GO

-- Registrar SOLO las migraciones que faltan (IF NOT EXISTS)
INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
SELECT MigrationId, ProductVersion FROM (VALUES
    ('20260228055648_AgregarMultiTenancy',                        '9.0.9'),
    ('20260228071433_CambiarMultiTenancyADominioCompleto',         '9.0.9'),
    ('20260304044601_AgregarTenantUsersYCorregirMultiTenancy',     '9.0.9'),
    ('20260304072710_AgregarTenantIdATodasLasEntidades',           '9.0.9'),
    ('20260306041112_AgregarSedeConfig',                           '9.0.9'),
    ('20260306053737_AgregarTenantIdATiposClaseYTipoPaquete',       '9.0.9')
) AS nuevas(MigrationId, ProductVersion)
WHERE NOT EXISTS (
    SELECT 1 FROM __EFMigrationsHistory h
    WHERE h.MigrationId = nuevas.MigrationId
);

SELECT 'Filas insertadas: ' + CAST(@@ROWCOUNT AS nvarchar(5)) AS Resultado;
GO

-- Verificar estado final
SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory ORDER BY MigrationId;
GO
