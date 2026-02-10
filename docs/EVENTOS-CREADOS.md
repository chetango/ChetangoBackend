# 📅 Eventos Creados - Dashboard Alumno

## ✅ Estado: Eventos Activos

Se han creado **2 eventos** que aparecerán automáticamente en el carrusel del dashboard del alumno.

---

## 🎯 Evento 1: Seminario Especial de Tango

**📌 Detalles:**
- **Título:** Seminario Especial de Tango
- **Maestros:** Jorge Padilla y Ana Gómez
- **Descripción:** Únete a un seminario único con los reconocidos maestros Jorge Padilla y Ana Gómez. Explora técnicas avanzadas de tango, musicalidad y conexión en pareja. ¡Cupos limitados!
- **Fecha:** 22 de febrero 2026 (Sábado)
- **Hora:** 15:00 (3:00 PM)
- **Precio:** $35,000
- **Destacado:** ✅ Sí (aparecerá con badge rojo)
- **Imagen:** `/uploads/eventos/seminario-tango-padilla-gomez.jpeg`
- **Estado:** Activo

---

## 🎯 Evento 2: Taller de Técnica Masculina

**📌 Detalles:**
- **Título:** Taller de Técnica Masculina
- **Maestro:** Jorge Padilla
- **Descripción:** Taller especializado para el rol masculino en el tango. El maestro Jorge Padilla te enseñará técnicas de liderazgo, marcación y disociación para llevar tu baile al siguiente nivel.
- **Fecha:** 15 de febrero 2026 (Domingo)
- **Hora:** 17:00 (5:00 PM)
- **Precio:** $25,000
- **Destacado:** ❌ No
- **Imagen:** `/uploads/eventos/taller-tecnica-masculina.jpeg`
- **Estado:** Activo

---

## 📂 Archivos Relacionados

### Scripts SQL
- `scripts/seed-eventos-carrusel.sql` - Script para crear los eventos

### Imágenes
- `Chetango.Api/wwwroot/uploads/eventos/seminario-tango-padilla-gomez.jpeg` (Evento 1)
- `Chetango.Api/wwwroot/uploads/eventos/taller-tecnica-masculina.jpeg` (Evento 2)
- Originales: `docs/Evento1.jpeg` y `docs/Evento2.jpeg`

---

## 🔍 Verificación

### Para ver los eventos en el dashboard:

1. **Iniciar backend:**
   ```bash
   dotnet run --project Chetango.Api/Chetango.Api.csproj --launch-profile http-qa
   ```

2. **Iniciar frontend:**
   ```bash
   npm run dev
   ```

3. **Login como alumno y navegar a Dashboard**

4. **Verificar sección "Eventos Próximos":**
   - Debería mostrar un carrusel con los 2 eventos
   - El Evento 1 (Seminario) aparecerá primero por fecha
   - El Evento 2 (Taller) aparecerá después
   - Autoplay cada 5 segundos
   - Botones de navegación ← →
   - Dots indicadores en la parte inferior

---

## 🎨 Cómo se Verá

### Evento 1 (Destacado)
```
┌────────────────────────────────────┐
│  [Imagen del seminario]            │
│  🔴 Destacado (badge rojo)         │
│────────────────────────────────────│
│  Seminario Especial de Tango      │
│  Únete a un seminario único...    │
│  📅 Sáb 22 Feb - 15:00            │
│  💰 $35,000                        │
│  [Reservar Cupo →]                 │
└────────────────────────────────────┘
```

### Evento 2 (Normal)
```
┌────────────────────────────────────┐
│  [Imagen del taller]               │
│────────────────────────────────────│
│  Taller de Técnica Masculina      │
│  Taller especializado para el...  │
│  📅 Dom 15 Feb - 17:00            │
│  💰 $25,000                        │
│  [Reservar Cupo →]                 │
└────────────────────────────────────┘
```

---

## 🛠️ Gestión de Eventos

### Ver eventos en la BD:
```sql
SELECT 
    Titulo, 
    Fecha, 
    Hora, 
    Precio, 
    Destacado, 
    Activo 
FROM Eventos 
WHERE Activo = 1 
ORDER BY Fecha;
```

### Desactivar un evento:
```sql
UPDATE Eventos 
SET Activo = 0 
WHERE Titulo LIKE '%nombre del evento%';
```

### Agregar más eventos:
Usar el endpoint POST `/api/eventos` o ejecutar INSERT similar al script.

---

## ✅ Checklist de Verificación

- [x] Imágenes copiadas a `wwwroot/uploads/eventos/`
- [x] Script SQL ejecutado exitosamente
- [x] 2 eventos creados en la tabla `Eventos`
- [x] Eventos configurados como `Activo = 1`
- [x] Fechas futuras (15 y 22 de febrero)
- [x] URLs de imágenes correctas
- [ ] Backend ejecutándose
- [ ] Frontend ejecutándose
- [ ] Dashboard verificado visualmente

---

**Próximos pasos:** Iniciar el backend y frontend para ver los eventos en acción.
