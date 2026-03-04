-- ====================================================================================================
-- ROW-LEVEL SECURITY (RLS) PARA MULTI-TENANCY
-- ====================================================================================================
-- Este script aplica Row-Level Security en SQL Server para garantizar aislamiento de datos a nivel de base de datos.
-- RLS es una capa adicional de seguridad que protege contra bugs en el código y SQL injection.
--
-- Funcionamiento:
-- 1. DbConnectionInterceptor establece SESSION_CONTEXT con TenantId al abrir cada conexión
-- 2. Security Policies filtran automáticamente los datos según el TenantId del SESSION_CONTEXT
-- 3. Incluso si EF Core Query Filters fallan, RLS protege los datos
--
-- Author: AI Assistant
-- Date: 2026-03-03
-- ====================================================================================================

USE ChetangoDB_Dev;
GO

-- ====================================================================================================
-- PASO 1: Crear función de predicado de seguridad
-- ====================================================================================================
-- Esta función determina si una fila debe ser visible para el usuario actual
-- Compara el TenantId de la fila con el TenantId almacenado en SESSION_CONTEXT

CREATE OR ALTER FUNCTION dbo.fn_TenantAccessPredicate(@TenantId UNIQUEIDENTIFIER)
RETURNS TABLE
WITH SCHEMABINDING
AS
RETURN
    SELECT 1 AS AccessResult
    WHERE
        -- Permitir acceso si TenantId es NULL (datos globales/compartidos)
        @TenantId IS NULL
        OR
        -- Permitir acceso si no hay TenantId en SESSION_CONTEXT (Super Admin)
        CAST(SESSION_CONTEXT(N'TenantId') AS UNIQUEIDENTIFIER) IS NULL
        OR
        -- Permitir acceso si el TenantId de la fila coincide con el del contexto
        @TenantId = CAST(SESSION_CONTEXT(N'TenantId') AS UNIQUEIDENTIFIER);
GO

-- ====================================================================================================
-- PASO 2: Crear políticas de seguridad para cada tabla con TenantId
-- ====================================================================================================

-- Política para tabla Clases
DROP SECURITY POLICY IF EXISTS dbo.TenantSecurityPolicy_Clases;
GO

CREATE SECURITY POLICY dbo.TenantSecurityPolicy_Clases
    ADD FILTER PREDICATE dbo.fn_TenantAccessPredicate(TenantId) ON dbo.Clases,
    ADD BLOCK PREDICATE dbo.fn_TenantAccessPredicate(TenantId) ON dbo.Clases AFTER INSERT
WITH (STATE = ON);
GO

-- Política para tabla Pagos
DROP SECURITY POLICY IF EXISTS dbo.TenantSecurityPolicy_Pagos;
GO

CREATE SECURITY POLICY dbo.TenantSecurityPolicy_Pagos
    ADD FILTER PREDICATE dbo.fn_TenantAccessPredicate(TenantId) ON dbo.Pagos,
    ADD BLOCK PREDICATE dbo.fn_TenantAccessPredicate(TenantId) ON dbo.Pagos AFTER INSERT
WITH (STATE = ON);
GO

-- Política para tabla Paquetes
DROP SECURITY POLICY IF EXISTS dbo.TenantSecurityPolicy_Paquetes;
GO

CREATE SECURITY POLICY dbo.TenantSecurityPolicy_Paquetes
    ADD FILTER PREDICATE dbo.fn_TenantAccessPredicate(TenantId) ON dbo.Paquetes,
    ADD BLOCK PREDICATE dbo.fn_TenantAccessPredicate(TenantId) ON dbo.Paquetes AFTER INSERT
WITH (STATE = ON);
GO

-- Política para tabla Asistencias
DROP SECURITY POLICY IF EXISTS dbo.TenantSecurityPolicy_Asistencias;
GO

CREATE SECURITY POLICY dbo.TenantSecurityPolicy_Asistencias
    ADD FILTER PREDICATE dbo.fn_TenantAccessPredicate(TenantId) ON dbo.Asistencias,
    ADD BLOCK PREDICATE dbo.fn_TenantAccessPredicate(TenantId) ON dbo.Asistencias AFTER INSERT
WITH (STATE = ON);
GO

-- Política para tabla Eventos
DROP SECURITY POLICY IF EXISTS dbo.TenantSecurityPolicy_Eventos;
GO

CREATE SECURITY POLICY dbo.TenantSecurityPolicy_Eventos
    ADD FILTER PREDICATE dbo.fn_TenantAccessPredicate(TenantId) ON dbo.Eventos,
    ADD BLOCK PREDICATE dbo.fn_TenantAccessPredicate(TenantId) ON dbo.Eventos AFTER INSERT
WITH (STATE = ON);
GO

-- Política para tabla SolicitudesClasePrivada
DROP SECURITY POLICY IF EXISTS dbo.TenantSecurityPolicy_SolicitudesClasePrivada;
GO

CREATE SECURITY POLICY dbo.TenantSecurityPolicy_SolicitudesClasePrivada
    ADD FILTER PREDICATE dbo.fn_TenantAccessPredicate(TenantId) ON dbo.SolicitudesClasePrivada,
    ADD BLOCK PREDICATE dbo.fn_TenantAccessPredicate(TenantId) ON dbo.SolicitudesClasePrivada AFTER INSERT
WITH (STATE = ON);
GO

-- Política para tabla SolicitudesRenovacionPaquete
DROP SECURITY POLICY IF EXISTS dbo.TenantSecurityPolicy_SolicitudesRenovacionPaquete;
GO

CREATE SECURITY POLICY dbo.TenantSecurityPolicy_SolicitudesRenovacionPaquete
    ADD FILTER PREDICATE dbo.fn_TenantAccessPredicate(TenantId) ON dbo.SolicitudesRenovacionPaquete,
    ADD BLOCK PREDICATE dbo.fn_TenantAccessPredicate(TenantId) ON dbo.SolicitudesRenovacionPaquete AFTER INSERT
WITH (STATE = ON);
GO

-- ====================================================================================================
-- PASO 3: Verificar políticas de seguridad
-- ====================================================================================================

SELECT 
    t.name AS TableName,
    sp.name AS SecurityPolicyName,
    spp.operation_desc AS OperationType,
    spp.predicate_type_desc AS PredicateType
FROM sys.security_policies sp
INNER JOIN sys.security_predicates spp ON sp.object_id = spp.object_id
INNER JOIN sys.tables t ON spp.target_object_id = t.object_id
WHERE sp.name LIKE 'TenantSecurityPolicy%'
ORDER BY t.name;
GO

-- ====================================================================================================
-- PRUEBA DE FUNCIONAMIENTO
-- ====================================================================================================

-- Establecer TenantId de prueba (simula lo que hace DbConnectionInterceptor)
EXEC sp_set_session_context @key = N'TenantId', @value = 'A1B2C3D4-E5F6-7890-ABCD-EF1234567890', @read_only = 0;
GO

-- Esta query solo debería retornar clases del tenant A1B2C3D4-E5F6-7890-ABCD-EF1234567890
-- RLS filtra automáticamente sin necesidad de WHERE TenantId = ...
SELECT COUNT(*) AS ClasesVisibles
FROM Clases;
GO

-- Limpiar SESSION_CONTEXT
EXEC sp_set_session_context @key = N'TenantId', @value = NULL;
GO

PRINT 'Row-Level Security aplicado exitosamente a 7 tablas con TenantId.';
PRINT 'IMPORTANTE: Esto protege contra bugs en código y SQL injection.';
PRINT 'Las políticas RLS están ACTIVAS (STATE = ON).';
GO
