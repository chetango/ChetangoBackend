# ☁️ INTEGRACIÓN AWS LAMBDA - CHETANGO

**Proyecto:** Chetango Dance Studio Management  
**Fecha Creación:** 14 Febrero 2026  
**Estado:** 📋 Planificación  
**Propósito:** Casos de uso y arquitectura para implementar AWS Lambda en el proyecto

---

## 📋 ÍNDICE

1. [Visión General](#visión-general)
2. [Casos de Uso Actuales](#casos-de-uso-actuales)
3. [Nuevas Funcionalidades](#nuevas-funcionalidades)
4. [Arquitectura Propuesta](#arquitectura-propuesta)
5. [Priorización](#priorización)
6. [Estimación de Costos](#estimación-de-costos)
7. [Plan de Implementación](#plan-de-implementación)

---

## 🎯 VISIÓN GENERAL

### ¿Por qué AWS Lambda?

**Beneficios Clave:**
- **Serverless:** Sin servidores que mantener o escalar
- **Cost-Effective:** Pagas solo por ejecuciones (primeros 1M requests gratis/mes)
- **Auto-Scaling:** Escala automáticamente según demanda
- **Performance:** Tareas pesadas en background → API más rápida
- **Integración:** Fácil desde .NET actual via HTTP, SQS, EventBridge

### Filosofía de Implementación
```
┌──────────────────────────────────────────────┐
│  ACTUAL: Todo en .NET API (síncrono)        │
│  • Procesar comprobantes                     │
│  • Calcular nóminas                          │
│  • Generar reportes                          │
│  • Enviar notificaciones                     │
│  Problema: Timeouts, recursos limitados      │
└──────────────────────────────────────────────┘
                    ↓
┌──────────────────────────────────────────────┐
│  HÍBRIDO: .NET API + AWS Lambda              │
│  • API: Operaciones CRUD rápidas             │
│  • Lambda: Procesamiento pesado/programado   │
│  Ventaja: Mejor UX, menor carga, escalable   │
└──────────────────────────────────────────────┘
```

---

## 🔧 CASOS DE USO ACTUALES

### 1. 📸 Procesamiento de Comprobantes de Pago
**⭐ PRIORIDAD ALTA**

#### Problema Actual
```csharp
// AdminPaymentsPage.tsx → RegisterPaymentModal
// Usuario sube imagen → API .NET procesa síncrono → Espera ~5-10 segundos
public async Task<IActionResult> SubirComprobante(IFormFile file)
{
    // ❌ Todo síncrono, usuario espera
    await SaveFile(file);
    await OptimizeImage(file); // Lento
    await ValidateWithBank(file); // API externa, puede fallar
    await UpdateDatabase();
    return Ok();
}
```

#### Solución con Lambda
```
Flujo Mejorado:
┌─────────────┐      ┌─────────────┐      ┌─────────────┐
│  Frontend   │─────▶│  API .NET   │─────▶│  S3 Bucket  │
│  (Upload)   │◀─────│  (Inmediato)│      │   /uploads  │
└─────────────┘      └─────────────┘      └─────────────┘
                                                  │ Trigger
                                                  ▼
                                          ┌─────────────┐
                                          │  Lambda:    │
                                          │  Process    │
                                          │  Receipt    │
                                          └─────────────┘
                                                  │
                                    ┌─────────────┼─────────────┐
                                    ▼             ▼             ▼
                              Optimize       OCR Extract    Validate
                              Image          Data           Bank API
                                    │             │             │
                                    └─────────────┴─────────────┘
                                              ▼
                                      Update SQL Server
                                      + Notify Admin/Alumno
```

#### Lambda Function: `ProcessPaymentReceipt`
```javascript
// Lambda: process-payment-receipt (Node.js)
exports.handler = async (event) => {
  const { bucket, key } = event.Records[0].s3;
  
  // 1. Descargar imagen de S3
  const imageBuffer = await s3.getObject({ Bucket: bucket, Key: key }).promise();
  
  // 2. Optimizar imagen (Sharp.js)
  const optimized = await sharp(imageBuffer)
    .resize(1024, 1024, { fit: 'inside' })
    .webp({ quality: 80 })
    .toBuffer();
  
  // 3. OCR con AWS Textract (extraer texto)
  const textractResult = await textract.detectDocumentText({
    Document: { Bytes: optimized }
  }).promise();
  
  const extractedData = parseTextractResult(textractResult);
  // { monto: 500, fecha: '2026-02-14', referencia: 'REF123' }
  
  // 4. Validar con API bancaria (opcional)
  const isValid = await validateWithBankAPI(extractedData.referencia);
  
  // 5. Actualizar base de datos
  await updatePaymentInDatabase({
    imageKey: key,
    amount: extractedData.monto,
    date: extractedData.fecha,
    reference: extractedData.referencia,
    status: isValid ? 'Verificado' : 'Pendiente',
    processedAt: new Date()
  });
  
  // 6. Notificar usuarios
  await sns.publish({
    TopicArn: 'arn:aws:sns:region:account:payment-processed',
    Message: JSON.stringify({ paymentId, status, userId })
  });
  
  return { statusCode: 200, body: 'Processed successfully' };
};
```

#### Configuración
- **Trigger:** S3 Upload (`/uploads/comprobantes/*`)
- **Runtime:** Node.js 20.x
- **Memory:** 512 MB
- **Timeout:** 60 segundos
- **Concurrency:** 10 ejecuciones simultáneas
- **Costo Estimado:** $0.02/mes (100 comprobantes/mes)

---

### 2. 💰 Cálculo de Nómina Mensual
**⭐ PRIORIDAD ALTA**

#### Problema Actual
```typescript
// AdminPayrollPage.tsx → LiquidarMesModal
// Admin hace clic "Liquidar Mes" → Espera 20-30 segundos
const liquidarMes = async () => {
  // ❌ Query pesado: todas las clases + asistencias de 15 profesores x 30 días
  const clases = await fetchClasesDelMes(); // Lento
  const asistencias = await fetchAsistencias(); // Lento
  const calculos = procesarNomina(clases, asistencias); // CPU intensivo
  // Usuario esperando... ⏳
}
```

#### Solución con Lambda
```
Opción A: Programado (Recomendado)
┌─────────────────────────────────────────┐
│  EventBridge Schedule                   │
│  "Último día del mes a las 23:00"       │
└─────────────────────────────────────────┘
            │ Trigger
            ▼
┌─────────────────────────────────────────┐
│  Lambda: CalculateMonthlyPayroll        │
│  1. Query todas las clases del mes      │
│  2. Calcular horas/pago por profesor    │
│  3. Generar PDFs de recibos             │
│  4. Guardar en BD status "Pendiente"    │
│  5. Email a admin "Nómina lista"        │
└─────────────────────────────────────────┘
            │
            ▼
┌─────────────────────────────────────────┐
│  Admin entra al día siguiente           │
│  Solo debe: Revisar + Aprobar           │
│  (Sin esperas, todo pre-calculado)      │
└─────────────────────────────────────────┘

Opción B: On-Demand
Admin → API .NET → SQS → Lambda (async)
```

#### Lambda Function: `CalculateMonthlyPayroll`
```python
# Lambda: calculate-monthly-payroll (Python 3.12)
import boto3
import pymssql
from datetime import datetime, timedelta
from fpdf import FPDF

def lambda_handler(event, context):
    # 1. Conectar a SQL Server
    conn = pymssql.connect(
        server=os.environ['DB_HOST'],
        user=os.environ['DB_USER'],
        password=os.environ['DB_PASSWORD'],
        database='ChetangoDB'
    )
    
    # 2. Query clases del mes por profesor
    mes_anterior = datetime.now().replace(day=1) - timedelta(days=1)
    
    query = """
        SELECT 
            p.ProfesorId,
            p.Nombre,
            COUNT(c.ClaseId) as TotalClases,
            SUM(c.DuracionMinutos) as TotalMinutos,
            p.TarifaPorHora
        FROM Profesores p
        INNER JOIN Clases c ON p.ProfesorId = c.ProfesorId
        WHERE YEAR(c.Fecha) = %s AND MONTH(c.Fecha) = %s
            AND c.EstadoAsistencia = 'Confirmada'
        GROUP BY p.ProfesorId, p.Nombre, p.TarifaPorHora
    """
    
    cursor = conn.cursor()
    cursor.execute(query, (mes_anterior.year, mes_anterior.month))
    profesores = cursor.fetchall()
    
    # 3. Calcular nómina
    nominas = []
    for profesor in profesores:
        profesor_id, nombre, total_clases, total_minutos, tarifa = profesor
        
        total_horas = total_minutos / 60
        pago_base = total_horas * tarifa
        bonificacion = calcular_bonificacion(total_clases)
        pago_total = pago_base + bonificacion
        
        nominas.append({
            'profesor_id': profesor_id,
            'nombre': nombre,
            'total_clases': total_clases,
            'total_horas': round(total_horas, 2),
            'pago_base': pago_base,
            'bonificacion': bonificacion,
            'pago_total': pago_total,
            'mes': mes_anterior.strftime('%Y-%m')
        })
    
    # 4. Guardar en BD
    for nomina in nominas:
        cursor.execute("""
            INSERT INTO Nominas 
            (ProfesorId, Mes, TotalClases, TotalHoras, PagoBase, Bonificacion, PagoTotal, Estado)
            VALUES (%s, %s, %s, %s, %s, %s, %s, 'Pendiente')
        """, (
            nomina['profesor_id'], nomina['mes'], nomina['total_clases'],
            nomina['total_horas'], nomina['pago_base'], nomina['bonificacion'],
            nomina['pago_total']
        ))
    
    conn.commit()
    
    # 5. Generar PDFs (opcional)
    s3 = boto3.client('s3')
    for nomina in nominas:
        pdf = generar_pdf_recibo(nomina)
        s3.put_object(
            Bucket='chetango-nominas',
            Key=f"recibos/{nomina['mes']}/{nomina['profesor_id']}.pdf",
            Body=pdf
        )
    
    # 6. Notificar admin
    ses = boto3.client('ses')
    ses.send_email(
        Source='noreply@chetango.com',
        Destination={'ToAddresses': ['admin@chetango.com']},
        Message={
            'Subject': {'Data': f'Nómina {mes_anterior.strftime("%B %Y")} lista para aprobación'},
            'Body': {
                'Text': {'Data': f'{len(nominas)} profesores procesados. Ingresa al sistema para aprobar.'}
            }
        }
    )
    
    return {
        'statusCode': 200,
        'body': f'Procesados {len(nominas)} profesores'
    }

def calcular_bonificacion(total_clases):
    if total_clases >= 40:
        return 500  # Bonificación por 40+ clases
    elif total_clases >= 30:
        return 250
    return 0

def generar_pdf_recibo(nomina):
    pdf = FPDF()
    pdf.add_page()
    pdf.set_font('Arial', 'B', 16)
    pdf.cell(0, 10, f'Recibo de Nómina - {nomina["mes"]}', 0, 1, 'C')
    # ... generar contenido PDF
    return pdf.output(dest='S').encode('latin1')
```

#### Configuración
- **Trigger:** EventBridge Schedule (`cron(0 23 L * ? *)` - último día del mes 23:00)
- **Runtime:** Python 3.12
- **Memory:** 1024 MB
- **Timeout:** 5 minutos
- **VPC:** Sí (para acceso a SQL Server)
- **Costo Estimado:** $0.10/mes (1 ejecución/mes)

---

### 3. 📊 Generación de Reportes Pesados
**⭐ PRIORIDAD MEDIA**

#### Problema Actual
```typescript
// AdminReportsPage.tsx
// Usuario solicita reporte "Asistencias del año" → Timeout
const generarReporte = async () => {
  // ❌ Query de 10,000+ registros
  const data = await fetch('/api/reportes/asistencias?year=2025'); // 30+ segundos
  // Browser timeout después de 30 segundos
}
```

#### Solución con Lambda
```
Flujo Asíncrono:
┌─────────────┐      ┌─────────────┐      ┌─────────────┐
│  Frontend   │─────▶│  API .NET   │─────▶│  SQS Queue  │
│  Solicita   │◀─────│  Responde:  │      │  /reports   │
│  Reporte    │      │  "En proceso"│      └─────────────┘
└─────────────┘      └─────────────┘              │
      │                                            │ Trigger
      │ WebSocket/Polling                          ▼
      │                                    ┌─────────────┐
      │                                    │  Lambda:    │
      │                                    │  Generate   │
      │                                    │  Report     │
      │                                    └─────────────┘
      │                                            │
      │                                            ▼
      │                                    Query SQL Server
      │                                    Process Data
      │                                    Generate Excel/PDF
      │                                            │
      │                                            ▼
      │                                    Upload to S3
      │                                    Update Status
      │                                            │
      └────────────────────────────────────────────┘
                   "Reporte listo: [Download Link]"
```

#### Lambda Function: `GenerateReport`
```javascript
// Lambda: generate-report (Node.js)
const ExcelJS = require('exceljs');
const { S3, SQS } = require('@aws-sdk/client-s3');
const sql = require('mssql');

exports.handler = async (event) => {
  const message = JSON.parse(event.Records[0].body);
  const { reportId, reportType, dateRange, filters, userId } = message;
  
  try {
    // 1. Conectar a SQL Server
    await sql.connect({
      server: process.env.DB_HOST,
      database: 'ChetangoDB',
      user: process.env.DB_USER,
      password: process.env.DB_PASSWORD
    });
    
    // 2. Query según tipo de reporte
    let data;
    switch (reportType) {
      case 'asistencias':
        data = await queryAsistencias(dateRange, filters);
        break;
      case 'pagos':
        data = await queryPagos(dateRange, filters);
        break;
      case 'nominas':
        data = await queryNominas(dateRange, filters);
        break;
      default:
        throw new Error('Tipo de reporte no válido');
    }
    
    // 3. Generar Excel
    const workbook = new ExcelJS.Workbook();
    const worksheet = workbook.addWorksheet('Reporte');
    
    // Headers
    worksheet.columns = [
      { header: 'Fecha', key: 'fecha', width: 15 },
      { header: 'Alumno', key: 'alumno', width: 30 },
      { header: 'Clase', key: 'clase', width: 25 },
      { header: 'Profesor', key: 'profesor', width: 30 },
      { header: 'Asistió', key: 'asistio', width: 10 }
    ];
    
    // Rows
    worksheet.addRows(data);
    
    // Styling
    worksheet.getRow(1).font = { bold: true };
    worksheet.getRow(1).fill = {
      type: 'pattern',
      pattern: 'solid',
      fgColor: { argb: 'FFFF6B9D' } // Primary color
    };
    
    // 4. Subir a S3
    const buffer = await workbook.xlsx.writeBuffer();
    const fileName = `reportes/${reportType}_${Date.now()}.xlsx`;
    
    const s3 = new S3();
    await s3.putObject({
      Bucket: 'chetango-reportes',
      Key: fileName,
      Body: buffer,
      ContentType: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
    });
    
    // 5. Generar URL firmada (válida 7 días)
    const downloadUrl = await s3.getSignedUrl('getObject', {
      Bucket: 'chetango-reportes',
      Key: fileName,
      Expires: 7 * 24 * 60 * 60
    });
    
    // 6. Actualizar estado en BD
    await sql.query`
      UPDATE Reportes
      SET Estado = 'Completado', 
          DownloadUrl = ${downloadUrl},
          CompletedAt = GETDATE()
      WHERE ReportId = ${reportId}
    `;
    
    // 7. Notificar usuario (WebSocket, Email, o SNS)
    await notifyUser(userId, {
      reportId,
      status: 'completed',
      downloadUrl,
      fileName: fileName.split('/').pop()
    });
    
    return { statusCode: 200, body: 'Report generated successfully' };
    
  } catch (error) {
    // Manejar error
    await sql.query`
      UPDATE Reportes
      SET Estado = 'Error', ErrorMessage = ${error.message}
      WHERE ReportId = ${reportId}
    `;
    
    await notifyUser(userId, {
      reportId,
      status: 'error',
      error: error.message
    });
    
    throw error;
  }
};

async function queryAsistencias(dateRange, filters) {
  const result = await sql.query`
    SELECT 
      c.Fecha as fecha,
      CONCAT(a.Nombre, ' ', a.Apellido) as alumno,
      c.NombreClase as clase,
      CONCAT(p.Nombre, ' ', p.Apellido) as profesor,
      CASE WHEN ac.Asistio = 1 THEN 'Sí' ELSE 'No' END as asistio
    FROM AsistenciasClases ac
    INNER JOIN Clases c ON ac.ClaseId = c.ClaseId
    INNER JOIN Alumnos a ON ac.AlumnoId = a.AlumnoId
    INNER JOIN Profesores p ON c.ProfesorId = p.ProfesorId
    WHERE c.Fecha BETWEEN ${dateRange.start} AND ${dateRange.end}
    ORDER BY c.Fecha DESC
  `;
  return result.recordset;
}
```

#### Configuración
- **Trigger:** SQS Queue (`chetango-report-requests`)
- **Runtime:** Node.js 20.x
- **Memory:** 2048 MB (reportes grandes)
- **Timeout:** 15 minutos (máximo Lambda)
- **VPC:** Sí (para SQL Server)
- **Costo Estimado:** $0.50/mes (50 reportes/mes)

---

### 4. 🔔 Notificaciones Automáticas Programadas
**⭐ PRIORIDAD MEDIA**

#### Funcionalidades

##### A. Recordatorio de Clases del Día
```python
# Lambda: remind-upcoming-classes
# Trigger: EventBridge Schedule (Diario 8:00 AM)

def lambda_handler(event, context):
    # 1. Query clases de hoy
    hoy = datetime.now().date()
    
    clases_hoy = query_db("""
        SELECT 
            c.ClaseId, c.NombreClase, c.HoraInicio,
            a.AlumnoId, a.Nombre, a.Telefono, a.Email
        FROM Clases c
        INNER JOIN InscripcionesClases ic ON c.ClaseId = ic.ClaseId
        INNER JOIN Alumnos a ON ic.AlumnoId = a.AlumnoId
        WHERE CAST(c.Fecha AS DATE) = %s
            AND ic.Estado = 'Activo'
    """, (hoy,))
    
    # 2. Agrupar por alumno
    recordatorios = {}
    for clase in clases_hoy:
        alumno_id = clase['AlumnoId']
        if alumno_id not in recordatorios:
            recordatorios[alumno_id] = {
                'nombre': clase['Nombre'],
                'telefono': clase['Telefono'],
                'email': clase['Email'],
                'clases': []
            }
        recordatorios[alumno_id]['clases'].append({
            'nombre': clase['NombreClase'],
            'hora': clase['HoraInicio'].strftime('%I:%M %p')
        })
    
    # 3. Enviar notificaciones
    ses = boto3.client('ses')
    sns = boto3.client('sns')
    
    for alumno_id, data in recordatorios.items():
        # Email
        mensaje_email = f"""
        Hola {data['nombre']}! 👋
        
        Recordatorio de tus clases de hoy:
        
        {chr(10).join([f"• {c['nombre']} a las {c['hora']}" for c in data['clases']])}
        
        ¡Nos vemos! 💃
        Chetango Dance Studio
        """
        
        ses.send_email(
            Source='noreply@chetango.com',
            Destination={'ToAddresses': [data['email']]},
            Message={
                'Subject': {'Data': 'Recordatorio: Clases de Hoy 📅'},
                'Body': {'Text': {'Data': mensaje_email}}
            }
        )
        
        # WhatsApp (via SNS o Twilio)
        if data['telefono']:
            enviar_whatsapp(data['telefono'], mensaje_email)
    
    return {
        'statusCode': 200,
        'body': f'Enviados {len(recordatorios)} recordatorios'
    }
```

##### B. Recordatorio de Pagos Pendientes
```python
# Lambda: remind-pending-payments
# Trigger: EventBridge Schedule (Lunes 9:00 AM)

def lambda_handler(event, context):
    # Query alumnos con pagos vencidos o próximos a vencer
    alumnos_deuda = query_db("""
        SELECT 
            a.AlumnoId, a.Nombre, a.Email, a.Telefono,
            p.MontoTotal, p.FechaVencimiento,
            DATEDIFF(day, GETDATE(), p.FechaVencimiento) as DiasRestantes
        FROM Alumnos a
        INNER JOIN Pagos p ON a.AlumnoId = p.AlumnoId
        WHERE p.Estado IN ('Pendiente', 'Parcial')
            AND p.FechaVencimiento BETWEEN GETDATE() AND DATEADD(day, 7, GETDATE())
        ORDER BY p.FechaVencimiento
    """)
    
    for alumno in alumnos_deuda:
        dias = alumno['DiasRestantes']
        
        if dias < 0:
            mensaje = f"Tu pago de ${alumno['MontoTotal']} está vencido hace {abs(dias)} días"
            urgencia = "🔴 URGENTE"
        elif dias <= 3:
            mensaje = f"Tu pago de ${alumno['MontoTotal']} vence en {dias} días"
            urgencia = "⚠️ Próximo a vencer"
        else:
            mensaje = f"Recuerda que tu pago de ${alumno['MontoTotal']} vence el {alumno['FechaVencimiento']}"
            urgencia = "📅 Recordatorio"
        
        # Enviar notificación
        enviar_email(alumno['Email'], f"{urgencia}: Pago Pendiente", mensaje)
        enviar_whatsapp(alumno['Telefono'], f"{urgencia}\n\n{mensaje}")
    
    return {'statusCode': 200}
```

##### C. Recordatorio Confirmación de Asistencia
```python
# Lambda: remind-attendance-confirmation
# Trigger: EventBridge Schedule (Diario 9:00 PM)

def lambda_handler(event, context):
    # Query clases de hoy sin asistencia confirmada
    clases_pendientes = query_db("""
        SELECT 
            c.ClaseId, c.NombreClase, c.HoraInicio,
            p.ProfesorId, p.Nombre, p.Email, p.Telefono
        FROM Clases c
        INNER JOIN Profesores p ON c.ProfesorId = p.ProfesorId
        WHERE CAST(c.Fecha AS DATE) = CAST(GETDATE() AS DATE)
            AND c.EstadoAsistencia = 'Pendiente'
            AND c.HoraInicio < DATEADD(hour, -1, GETDATE())  -- Hace más de 1 hora
    """)
    
    for clase in clases_pendientes:
        mensaje = f"""
        Hola {clase['Nombre']},
        
        No has confirmado la asistencia de tu clase:
        • {clase['NombreClase']}
        • Hora: {clase['HoraInicio'].strftime('%I:%M %p')}
        
        Por favor, ingresa al sistema para confirmar. ✅
        
        Chetango Dance Studio
        """
        
        enviar_email(clase['Email'], "Confirma asistencia de tu clase", mensaje)
        enviar_whatsapp(clase['Telefono'], mensaje)
    
    return {'statusCode': 200}
```

#### Configuración General
- **Runtime:** Python 3.12
- **Memory:** 256 MB
- **Timeout:** 2 minutos
- **Concurrency:** 5
- **Costo Estimado:** $0.05/mes (3 ejecuciones/día)

---

### 5. 🧹 Limpieza y Mantenimiento Automático
**⭐ PRIORIDAD BAJA**

#### Lambda: `DatabaseMaintenance`
```python
# Trigger: Diario 2:00 AM

def lambda_handler(event, context):
    tareas_completadas = []
    
    # 1. Eliminar archivos temporales antiguos
    eliminados = eliminar_archivos_temp()
    tareas_completadas.append(f"Archivos temp: {eliminados}")
    
    # 2. Archivar registros antiguos (>1 año)
    archivados = archivar_registros_antiguos()
    tareas_completadas.append(f"Registros archivados: {archivados}")
    
    # 3. Limpiar logs antiguos
    logs_borrados = limpiar_logs()
    tareas_completadas.append(f"Logs eliminados: {logs_borrados}")
    
    # 4. Optimizar índices de BD
    optimizar_indices_bd()
    tareas_completadas.append("Índices optimizados")
    
    # 5. Backup incremental
    backup_path = realizar_backup_incremental()
    tareas_completadas.append(f"Backup: {backup_path}")
    
    # 6. Enviar reporte a admin
    enviar_reporte_mantenimiento(tareas_completadas)
    
    return {'statusCode': 200, 'tareas': tareas_completadas}

def eliminar_archivos_temp():
    s3 = boto3.client('s3')
    fecha_limite = datetime.now() - timedelta(days=30)
    
    objetos = s3.list_objects_v2(
        Bucket='chetango-uploads',
        Prefix='temp/'
    )
    
    eliminados = 0
    for obj in objetos.get('Contents', []):
        if obj['LastModified'].replace(tzinfo=None) < fecha_limite:
            s3.delete_object(Bucket='chetango-uploads', Key=obj['Key'])
            eliminados += 1
    
    return eliminados

def archivar_registros_antiguos():
    # Mover clases >1 año a tabla histórica
    result = query_db("""
        INSERT INTO ClasesHistoricas
        SELECT * FROM Clases
        WHERE Fecha < DATEADD(year, -1, GETDATE())
        
        DELETE FROM Clases
        WHERE Fecha < DATEADD(year, -1, GETDATE())
    """)
    return result.rowcount

def limpiar_logs():
    query_db("DELETE FROM Logs WHERE Fecha < DATEADD(month, -3, GETDATE())")
    return query_db("SELECT @@ROWCOUNT").fetchone()[0]

def optimizar_indices_bd():
    tablas = ['Clases', 'Alumnos', 'Pagos', 'Asistencias']
    for tabla in tablas:
        query_db(f"ALTER INDEX ALL ON {tabla} REBUILD")
```

---

## 🚀 NUEVAS FUNCIONALIDADES

### 6. 🤖 Análisis Inteligente con Machine Learning
**⭐ FUTURO**

#### Lambda: `PredictStudentChurn`
```python
# Lambda con scikit-learn pre-entrenado
import joblib
import numpy as np

model = joblib.load('churn_model.pkl')  # Modelo pre-entrenado

def lambda_handler(event, context):
    # 1. Query datos de alumnos
    alumnos = query_db("""
        SELECT 
            a.AlumnoId, a.Nombre,
            COUNT(DISTINCT ac.ClaseId) as TotalClases,
            SUM(CASE WHEN ac.Asistio = 1 THEN 1 ELSE 0 END) as Asistencias,
            SUM(CASE WHEN ac.Asistio = 0 THEN 1 ELSE 0 END) as Faltas,
            COUNT(DISTINCT p.PagoId) as TotalPagos,
            DATEDIFF(day, MAX(ac.Fecha), GETDATE()) as DiasSinAsistir
        FROM Alumnos a
        LEFT JOIN AsistenciasClases ac ON a.AlumnoId = ac.AlumnoId
        LEFT JOIN Pagos p ON a.AlumnoId = p.AlumnoId
        WHERE a.Estado = 'Activo'
        GROUP BY a.AlumnoId, a.Nombre
    """)
    
    # 2. Preparar features
    alumnos_riesgo = []
    for alumno in alumnos:
        features = np.array([[
            alumno['TotalClases'],
            alumno['Asistencias'] / max(alumno['TotalClases'], 1),  # Tasa asistencia
            alumno['Faltas'],
            alumno['DiasSinAsistir'],
            alumno['TotalPagos']
        ]])
        
        # 3. Predicción
        probabilidad_abandono = model.predict_proba(features)[0][1]
        
        if probabilidad_abandono > 0.7:  # Alto riesgo
            alumnos_riesgo.append({
                'alumno_id': alumno['AlumnoId'],
                'nombre': alumno['Nombre'],
                'probabilidad': round(probabilidad_abandono * 100, 2),
                'dias_sin_asistir': alumno['DiasSinAsistir'],
                'faltas': alumno['Faltas']
            })
    
    # 4. Guardar alertas en BD
    for alumno in alumnos_riesgo:
        query_db("""
            INSERT INTO AlertasAlumnos (AlumnoId, TipoAlerta, Probabilidad, Fecha)
            VALUES (%s, 'Riesgo de Abandono', %s, GETDATE())
        """, (alumno['alumno_id'], alumno['probabilidad']))
    
    # 5. Notificar admin
    if alumnos_riesgo:
        enviar_email_admin(
            'Alerta: Alumnos en Riesgo',
            f"{len(alumnos_riesgo)} alumnos identificados con riesgo de abandono:\n" +
            "\n".join([f"• {a['nombre']} ({a['probabilidad']}% riesgo)" for a in alumnos_riesgo])
        )
    
    return {'statusCode': 200, 'alumnos_riesgo': len(alumnos_riesgo)}
```

---

### 7. 🖼️ Optimización Automática de Imágenes
**⭐ PRIORIDAD ALTA**

#### Lambda: `OptimizeImages`
```javascript
// Trigger: S3 Upload en /uploads/*
const sharp = require('sharp');
const { S3 } = require('@aws-sdk/client-s3');

exports.handler = async (event) => {
  const { bucket, key } = event.Records[0].s3;
  const s3 = new S3();
  
  // 1. Descargar imagen original
  const originalImage = await s3.getObject({ Bucket: bucket, Key: key });
  const imageBuffer = await streamToBuffer(originalImage.Body);
  
  // 2. Generar múltiples versiones
  const versions = [
    { suffix: '-thumb', width: 150, height: 150 },
    { suffix: '-small', width: 300, height: 300 },
    { suffix: '-medium', width: 800, height: 800 },
    { suffix: '-webp', format: 'webp', quality: 80 }
  ];
  
  for (const version of versions) {
    let processor = sharp(imageBuffer);
    
    if (version.width) {
      processor = processor.resize(version.width, version.height, {
        fit: 'inside',
        withoutEnlargement: true
      });
    }
    
    if (version.format === 'webp') {
      processor = processor.webp({ quality: version.quality });
    }
    
    const optimized = await processor.toBuffer();
    
    // 3. Subir versión optimizada
    const newKey = key.replace(/\.[^.]+$/, `${version.suffix}$&`);
    await s3.putObject({
      Bucket: bucket,
      Key: newKey,
      Body: optimized,
      ContentType: version.format === 'webp' 
        ? 'image/webp' 
        : originalImage.ContentType
    });
  }
  
  // 4. Actualizar BD con URLs de versiones
  await updateImageUrls(key, versions);
  
  return { statusCode: 200, message: 'Image optimized successfully' };
};
```

**Beneficio:** App móvil carga 70% más rápido

---

### 8. 💳 Webhooks de Pasarelas de Pago
**⭐ PRIORIDAD MEDIA**

#### Lambda: `ProcessStripeWebhook`
```javascript
// API Gateway → Lambda
const stripe = require('stripe')(process.env.STRIPE_SECRET_KEY);

exports.handler = async (event) => {
  const sig = event.headers['stripe-signature'];
  const payload = event.body;
  
  let webhookEvent;
  try {
    webhookEvent = stripe.webhooks.constructEvent(
      payload, 
      sig, 
      process.env.STRIPE_WEBHOOK_SECRET
    );
  } catch (err) {
    return { statusCode: 400, body: 'Invalid signature' };
  }
  
  // Manejar eventos
  switch (webhookEvent.type) {
    case 'payment_intent.succeeded':
      await handlePaymentSuccess(webhookEvent.data.object);
      break;
      
    case 'payment_intent.payment_failed':
      await handlePaymentFailed(webhookEvent.data.object);
      break;
      
    case 'customer.subscription.deleted':
      await handleSubscriptionCanceled(webhookEvent.data.object);
      break;
  }
  
  return { statusCode: 200, body: 'Webhook processed' };
};

async function handlePaymentSuccess(paymentIntent) {
  const { metadata } = paymentIntent;
  
  // Actualizar pago en BD
  await query_db(`
    UPDATE Pagos
    SET Estado = 'Verificado',
        StripePaymentId = '${paymentIntent.id}',
        FechaVerificacion = GETDATE()
    WHERE PagoId = ${metadata.pagoId}
  `);
  
  // Notificar alumno
  await sendPaymentConfirmation(metadata.alumnoId, paymentIntent.amount);
}
```

---

### 9. 💾 Backup Incremental Inteligente
**⭐ PRIORIDAD BAJA**

#### Lambda: `IncrementalBackup`
```bash
#!/bin/bash
# Lambda Container Image (con Azure CLI + AWS CLI)

# 1. Backup incremental SQL Server
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
BACKUP_FILE="chetango_backup_${TIMESTAMP}.bak"

# Conectar a SQL Server y hacer backup
sqlcmd -S $DB_SERVER -U $DB_USER -P $DB_PASSWORD -Q \
  "BACKUP DATABASE ChetangoDB TO DISK='/tmp/${BACKUP_FILE}' WITH DIFFERENTIAL"

# 2. Comprimir
gzip /tmp/${BACKUP_FILE}

# 3. Subir a S3
aws s3 cp "/tmp/${BACKUP_FILE}.gz" \
  "s3://chetango-backups/sql/${BACKUP_FILE}.gz" \
  --storage-class GLACIER  # Storage económico

# 4. Backup archivos modificados últimas 24h
aws s3 sync /mnt/uploads s3://chetango-backups/uploads/ \
  --exclude "*" \
  --include "*.jpg" --include "*.png" --include "*.pdf" \
  --size-only

# 5. Mantener solo últimos 30 backups
aws s3 ls s3://chetango-backups/sql/ | \
  sort -r | \
  tail -n +31 | \
  awk '{print $4}' | \
  xargs -I {} aws s3 rm "s3://chetango-backups/sql/{}"

# 6. Enviar reporte
echo "Backup completado: ${BACKUP_FILE}.gz" | \
  aws sns publish --topic-arn $SNS_TOPIC --message file:///dev/stdin
```

---

### 10. 🤖 Chatbot Asistente con IA
**⭐ FUTURO**

#### Lambda: `ChatbotAssistant`
```python
# API Gateway + Lambda + OpenAI API
import openai
import json

openai.api_key = os.environ['OPENAI_API_KEY']

def lambda_handler(event, context):
    # Webhook de WhatsApp/Telegram
    body = json.loads(event['body'])
    user_message = body['message']
    user_id = body['from']
    
    # 1. Obtener contexto del usuario
    contexto = obtener_contexto_usuario(user_id)
    
    # 2. Prompt con contexto
    prompt = f"""
    Eres un asistente virtual de Chetango Dance Studio.
    
    Información del usuario:
    - Nombre: {contexto['nombre']}
    - Clases inscritas: {', '.join(contexto['clases'])}
    - Próxima clase: {contexto['proxima_clase']}
    - Pagos pendientes: ${contexto['deuda']}
    
    Usuario pregunta: {user_message}
    
    Responde de manera amigable y concisa.
    """
    
    # 3. Consultar ChatGPT
    response = openai.ChatCompletion.create(
        model="gpt-4",
        messages=[
            {"role": "system", "content": "Eres un asistente de academia de baile"},
            {"role": "user", "content": prompt}
        ],
        max_tokens=200
    )
    
    bot_message = response.choices[0].message.content
    
    # 4. Enviar respuesta
    enviar_whatsapp(user_id, bot_message)
    
    return {'statusCode': 200}

def obtener_contexto_usuario(user_id):
    # Query BD para contexto
    usuario = query_db("""
        SELECT 
            a.Nombre,
            STRING_AGG(c.NombreClase, ', ') as Clases,
            MIN(c.Fecha) as ProximaClase,
            SUM(p.MontoTotal - p.MontoPagado) as Deuda
        FROM Alumnos a
        LEFT JOIN InscripcionesClases ic ON a.AlumnoId = ic.AlumnoId
        LEFT JOIN Clases c ON ic.ClaseId = c.ClaseId AND c.Fecha > GETDATE()
        LEFT JOIN Pagos p ON a.AlumnoId = p.AlumnoId AND p.Estado = 'Pendiente'
        WHERE a.Telefono = %s
        GROUP BY a.Nombre
    """, (user_id,))
    
    return {
        'nombre': usuario['Nombre'],
        'clases': usuario['Clases'].split(', ') if usuario['Clases'] else [],
        'proxima_clase': usuario['ProximaClase'],
        'deuda': usuario['Deuda'] or 0
    }
```

---

## 🏗️ ARQUITECTURA PROPUESTA

### Diagrama de Integración

```
┌─────────────────────────────────────────────────────────────────┐
│                        ARQUITECTURA HÍBRIDA                     │
└─────────────────────────────────────────────────────────────────┘

┌──────────────────────┐
│   FRONTEND (React)   │
│   Azure Static Web   │
└──────────┬───────────┘
           │ HTTPS
           ▼
┌──────────────────────┐
│   API .NET 8         │◀──────────────────┐
│   Azure App Service  │                   │
└──────────┬───────────┘                   │
           │                               │
           ├─────────────┬─────────────────┤
           ▼             ▼                 ▼
    ┌───────────┐  ┌──────────┐    ┌──────────┐
    │ SQL Server│  │   SQS    │    │    S3    │
    │  Azure    │  │  Queue   │    │  Bucket  │
    └───────────┘  └────┬─────┘    └────┬─────┘
                        │ Trigger       │ Trigger
                        ▼               ▼
                   ┌─────────────────────────┐
                   │    AWS LAMBDA           │
                   │                         │
                   │  • ProcessReceipt       │
                   │  • GenerateReport       │
                   │  • CalculatePayroll     │
                   │  • SendNotifications    │
                   │  • OptimizeImages       │
                   │  • DatabaseMaintenance  │
                   │  • PredictChurn (ML)    │
                   │  • ChatbotAssistant     │
                   └─────────────────────────┘
                            │
                            │ SNS/SES
                            ▼
                   ┌─────────────────┐
                   │  Notifications  │
                   │  Email/WhatsApp │
                   └─────────────────┘
```

### Flujos de Datos

#### Flujo Síncrono (API directa)
```
Frontend → API .NET → SQL Server → Response
```

#### Flujo Asíncrono (Lambda)
```
Frontend → API .NET → SQS/S3 → Lambda → SQL Server
                  ↓
            Response inmediato
```

### Conectividad

#### Desde Lambda a SQL Server
```typescript
// Opción 1: VPC Peering (Azure ↔ AWS)
Lambda (VPC) → VPN/Peering → Azure VNet → SQL Server

// Opción 2: SQL Server con IP pública (menos seguro)
Lambda → Internet → SQL Server (Azure, IP whitelisted)

// Opción 3: API Gateway intermedio
Lambda → API .NET → SQL Server
```

**Recomendación:** Opción 3 (API como intermediario) para:
- Mantener lógica de negocio centralizada
- Evitar duplicar código de acceso a datos
- Mejor seguridad (SQL Server sin exposición)

---

## 📊 PRIORIZACIÓN

### Fase 1: Quick Wins (2-3 semanas)
**Esfuerzo:** Bajo | **Impacto:** Alto

1. ✅ **Notificaciones automáticas** (Scheduled Lambda)
   - Recordatorio clases del día
   - Recordatorio pagos pendientes
   - Recordatorio confirmación asistencia
   - **Esfuerzo:** 3 días | **Costo:** $0.05/mes

2. ✅ **Optimización de imágenes** (S3 Trigger)
   - Thumbnails automáticos
   - Conversión a WebP
   - Compresión
   - **Esfuerzo:** 2 días | **Costo:** $0.10/mes

3. ✅ **Backup automático** (Scheduled Lambda)
   - Backup incremental diario
   - Limpieza de archivos antiguos
   - Reporte a admin
   - **Esfuerzo:** 2 días | **Costo:** $0.15/mes + storage

**Total Fase 1:** 7 días | $0.30/mes

---

### Fase 2: Mejoras de Performance (1 mes)
**Esfuerzo:** Medio | **Impacto:** Alto

4. ✅ **Procesamiento de comprobantes** (S3/SQS Trigger)
   - OCR con Textract
   - Optimización de imagen
   - Validación asíncrona
   - **Esfuerzo:** 5 días | **Costo:** $0.50/mes

5. ✅ **Generación de reportes** (SQS Trigger)
   - Reportes pesados en background
   - Generación de Excel/PDF
   - Notificación por email con link
   - **Esfuerzo:** 5 días | **Costo:** $0.50/mes

6. ✅ **Cálculo de nómina** (Scheduled Lambda)
   - Cálculo automático mensual
   - Generación de recibos PDF
   - Notificación a admin
   - **Esfuerzo:** 4 días | **Costo:** $0.10/mes

**Total Fase 2:** 14 días | $1.10/mes

---

### Fase 3: Features Avanzados (2-3 meses)
**Esfuerzo:** Alto | **Impacto:** Medio

7. ✅ **Webhooks pasarelas de pago**
   - Integración Stripe/PayPal
   - Verificación automática
   - **Esfuerzo:** 5 días | **Costo:** $0.20/mes

8. ✅ **Análisis con ML** (Predict Churn)
   - Entrenar modelo
   - Predicción semanal
   - Alertas a admin
   - **Esfuerzo:** 10 días | **Costo:** $1.00/mes

9. ✅ **Chatbot asistente** (OpenAI)
   - WhatsApp/Telegram integration
   - Consultas automáticas
   - **Esfuerzo:** 7 días | **Costo:** $5.00/mes (OpenAI API)

**Total Fase 3:** 22 días | $6.20/mes

---

## 💰 ESTIMACIÓN DE COSTOS

### AWS Lambda Pricing (Región us-east-1)

**Free Tier Permanente:**
- 1M requests/mes gratis
- 400,000 GB-seconds/mes gratis

**Después del Free Tier:**
- $0.20 por 1M requests
- $0.0000166667 por GB-second

### Ejemplo Práctico: Fase 1 Completa

```
Notificaciones (3 lambdas, 3x diario):
  • Requests: 90/mes
  • Duración: 256MB x 2s
  • Costo: $0.00 (dentro del free tier)

Optimización Imágenes:
  • Requests: 100/mes (100 imágenes subidas)
  • Duración: 512MB x 10s
  • Costo: $0.02

Backup:
  • Requests: 30/mes (diario)
  • Duración: 512MB x 60s
  • Costo: $0.05
  • Storage S3: ~20GB = $0.46/mes

TOTAL FASE 1: ~$0.53/mes
```

### Proyección Anual

| Fase | Lambdas | Requests/mes | Costo Mensual | Costo Anual |
|------|---------|--------------|---------------|-------------|
| Fase 1 | 5 | 220 | $0.53 | $6.36 |
| Fase 2 | 8 | 320 | $1.63 | $19.56 |
| Fase 3 | 11 | 500 | $7.83 | $93.96 |
| **TOTAL** | **11** | **500** | **$7.83** | **$93.96** |

**Nota:** Incluye Lambda + S3 + SQS + SNS. No incluye OpenAI API (variable).

---

## 📅 PLAN DE IMPLEMENTACIÓN

### Preparación (1 semana)

#### 1. Setup de Infraestructura AWS
```bash
# Crear cuenta AWS
# Configurar IAM roles y policies
# Crear S3 buckets
# Configurar VPC (si es necesario)
# Setup EventBridge schedules
# Configurar SQS queues
```

#### 2. Configurar Conexión desde .NET
```bash
# Instalar AWS SDK
dotnet add package AWSSDK.S3
dotnet add package AWSSDK.SQS
dotnet add package AWSSDK.SimpleNotificationService
```

#### 3. Setup de Desarrollo Local
```bash
# Instalar AWS CLI
# Configurar credenciales
# Instalar SAM CLI (para testing local)
# Setup Docker (para containers)
```

---

### Implementación por Fases

#### ✅ Fase 1: Notificaciones (Semana 1-2)

**Día 1-2: Lambda "RemindUpcomingClasses"**
- [ ] Crear función Lambda (Python)
- [ ] Configurar EventBridge (cron diario 8 AM)
- [ ] Implementar query a SQL Server
- [ ] Integrar SES para emails
- [ ] Testing

**Día 3-4: Lambda "RemindPendingPayments"**
- [ ] Crear función Lambda
- [ ] Configurar EventBridge (cron semanal)
- [ ] Implementar lógica de alertas
- [ ] Testing

**Día 5: Lambda "RemindAttendanceConfirmation"**
- [ ] Crear función Lambda
- [ ] Configurar EventBridge (cron diario 9 PM)
- [ ] Testing end-to-end

**Día 6-7: Optimización de Imágenes**
- [ ] Lambda "OptimizeImages"
- [ ] Configurar S3 trigger
- [ ] Implementar Sharp.js
- [ ] Testing con imágenes reales

---

#### ✅ Fase 2: Performance (Semana 3-6)

**Semana 3-4: Procesamiento de Comprobantes**
- [ ] Modificar API .NET para upload a S3
- [ ] Lambda "ProcessPaymentReceipt"
- [ ] Integrar AWS Textract
- [ ] Actualizar frontend (feedback asíncrono)
- [ ] Testing completo

**Semana 5: Generación de Reportes**
- [ ] Crear SQS queue
- [ ] Lambda "GenerateReport"
- [ ] Modificar AdminReportsPage (async)
- [ ] Implementar notificaciones
- [ ] Testing con reportes grandes

**Semana 6: Cálculo de Nómina**
- [ ] Lambda "CalculateMonthlyPayroll"
- [ ] EventBridge schedule (último día mes)
- [ ] Generar PDFs de recibos
- [ ] Modificar AdminPayrollPage
- [ ] Testing con datos reales

---

#### ✅ Fase 3: Features Avanzados (Semana 7-12)

**Semana 7-8: Webhooks Pasarelas**
- [ ] API Gateway + Lambda
- [ ] Integración Stripe
- [ ] Manejo de eventos
- [ ] Testing con Stripe test mode

**Semana 9-11: ML Predict Churn**
- [ ] Recolectar datos históricos
- [ ] Entrenar modelo (scikit-learn)
- [ ] Lambda con modelo
- [ ] Dashboard de alertas
- [ ] Testing y ajuste

**Semana 12: Chatbot**
- [ ] Setup OpenAI API
- [ ] Lambda "ChatbotAssistant"
- [ ] Integración WhatsApp/Telegram
- [ ] Testing conversacional

---

## 📚 RECURSOS Y REFERENCIAS

### Documentación Oficial
- [AWS Lambda Docs](https://docs.aws.amazon.com/lambda/)
- [AWS SDK for .NET](https://aws.amazon.com/sdk-for-net/)
- [EventBridge Schedules](https://docs.aws.amazon.com/eventbridge/latest/userguide/eb-create-rule-schedule.html)
- [S3 Event Notifications](https://docs.aws.amazon.com/AmazonS3/latest/userguide/NotificationHowTo.html)

### Tutoriales Recomendados
- [Lambda + SQL Server Tutorial](https://aws.amazon.com/blogs/database/)
- [Image Processing with Lambda](https://aws.amazon.com/blogs/compute/)
- [ML on Lambda](https://aws.amazon.com/blogs/machine-learning/)

### Herramientas
- [AWS SAM CLI](https://docs.aws.amazon.com/serverless-application-model/) - Testing local
- [LocalStack](https://localstack.cloud/) - Emular AWS localmente
- [Serverless Framework](https://www.serverless.com/) - Deployment simplificado

---

## ✅ CHECKLIST DE IMPLEMENTACIÓN

### Pre-requisitos
- [ ] Cuenta AWS creada y configurada
- [ ] AWS CLI instalado y configurado
- [ ] IAM roles creados (Lambda execution role)
- [ ] S3 buckets creados
- [ ] SQS queues creadas
- [ ] EventBridge configurado
- [ ] SDK AWS en .NET instalado

### Por cada Lambda
- [ ] Código implementado y testeado localmente
- [ ] Dependencias empaquetadas
- [ ] Variables de entorno configuradas
- [ ] Timeout apropiado configurado
- [ ] Memory allocation optimizada
- [ ] Trigger configurado
- [ ] CloudWatch Logs configurado
- [ ] IAM permissions correctos
- [ ] Testing en staging
- [ ] Deploy a producción
- [ ] Monitoring activo

### Post-implementación
- [ ] Documentar en docs/
- [ ] Actualizar diagramas de arquitectura
- [ ] Capacitar al equipo
- [ ] Establecer alertas CloudWatch
- [ ] Revisar costos mensuales

---

## 🎯 PRÓXIMOS PASOS

1. **Revisar y aprobar este documento**
2. **Decidir qué fase implementar primero**
3. **Crear cuenta AWS y setup inicial**
4. **Comenzar con Fase 1 (Quick Wins)**
5. **Monitorear resultados y ajustar**

---

**Fecha Última Actualización:** 14 Febrero 2026  
**Versión:** 1.0  
**Autor:** Equipo Desarrollo Chetango

---

**Documentos Relacionados:**
- [PLAN-RESPONSIVE-MOBILE.md](../development/PLAN-RESPONSIVE-MOBILE.md)
- [DEPLOYMENT-STRATEGY.md](../DEPLOYMENT-STRATEGY.md)
