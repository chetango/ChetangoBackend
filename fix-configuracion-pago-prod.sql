-- ===========================================================================
-- ACTUALIZAR DATOS BANCARIOS - ConfiguracionPagos
-- Fecha: 2026-03-06
-- Motivo: Cambiar cuenta Aphellion (placeholder) a cuenta real Jorge Padilla
-- ===========================================================================

-- Verificar estado actual
SELECT 'Estado actual:' AS Info;
SELECT Id, Banco, TipoCuenta, NumeroCuenta, Titular, NIT, Activo, MostrarEnPortal
FROM ConfiguracionPagos;

GO

-- Actualizar a datos reales
UPDATE ConfiguracionPagos
SET
    Banco         = 'Bancolombia',
    TipoCuenta    = 'Ahorros',
    NumeroCuenta  = '00530986713',
    Titular       = 'Jorge Padilla',
    NIT           = NULL
WHERE Activo = 1
  AND MostrarEnPortal = 1;

SELECT 'Filas actualizadas: ' + CAST(@@ROWCOUNT AS nvarchar(5)) AS Resultado;

GO

-- Verificar resultado
SELECT 'Estado actualizado:' AS Info;
SELECT Id, Banco, TipoCuenta, NumeroCuenta, Titular, NIT, Activo, MostrarEnPortal
FROM ConfiguracionPagos;

GO
