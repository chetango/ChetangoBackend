# 📱 GUÍA DE IMPLEMENTACIÓN: MODALES RESPONSIVE

**Proyecto:** Chetango Dance Studio Management  
**Fecha:** 13 Febrero 2026  
**Documento:** Especificación de Modales Responsive  
**Versión:** 1.0

---

## 🎯 Objetivo

Los modales son componentes críticos en la aplicación. En desktop funcionan bien, pero en móvil requieren un tratamiento especial para garantizar una buena experiencia de usuario.

---

## 📋 PRINCIPIOS DE DISEÑO PARA MODALES MÓVILES

### 1. **Layout Responsivo**
```css
/* ❌ MAL: Modal con ancho fijo */
.modal {
  width: 600px;
  max-height: 80vh;
}

/* ✅ BIEN: Modal adaptativo */
.modal {
  width: 100%;                    /* Móvil: Full-width */
  height: 100vh;                  /* Móvil: Full-height */
  max-height: none;
}

@media (min-width: 768px) {
  .modal {
    width: 600px;                 /* Desktop: Centrado */
    height: auto;
    max-height: 90vh;
  }
}
```

### 2. **Gestos Touch**
- **Cierre por swipe-down** desde el header
- **Botón X visible** y grande (min 44px)
- **Overlay tap to close** (opcional, cuidado con forms)
- **Scroll interno** si el contenido excede viewport

### 3. **Formularios Touch-Friendly**
- Campos apilados en vertical
- Inputs con height mínimo 48px
- Botones con height mínimo 44px
- Spacing generoso (16px entre campos)
- Labels claros y visibles
- Validación inline

### 4. **Animaciones**
```typescript
// Móvil: Slide from bottom
const mobileAnimation = {
  initial: { y: '100%' },
  animate: { y: 0 },
  exit: { y: '100%' },
  transition: { type: 'spring', damping: 30 }
}

// Desktop: Fade + scale
const desktopAnimation = {
  initial: { opacity: 0, scale: 0.95 },
  animate: { opacity: 1, scale: 1 },
  exit: { opacity: 0, scale: 0.95 },
  transition: { duration: 0.2 }
}
```

---

## 📊 INVENTARIO DE MODALES POR MÓDULO

### **ADMIN - Módulo Pagos (AdminPaymentsPage)**

#### 1. **RegisterPaymentModal**
- **Complejidad:** 🔴 Alta
- **Formulario:** Multi-step wizard
- **Campos:** ~8 campos + file upload
- **Responsive:**
  - Full-screen en móvil
  - Steps horizontales → verticales
  - File upload touch-optimizado
  - Preview de imágenes responsive
  - Botones navegación full-width

#### 2. **VerifyPaymentModal**
- **Complejidad:** 🟠 Media
- **Formulario:** Aprobar/Rechazar con comentarios
- **Campos:** Textarea + botones acción
- **Responsive:**
  - Full-screen en móvil
  - Imagen comprobante fullscreen-able
  - Botones grandes y espaciados

#### 3. **PaymentDetailModal**
- **Complejidad:** 🟢 Baja
- **Formulario:** Solo visualización
- **Responsive:**
  - Stack vertical de info
  - Cards de paquetes en columna
  - Botón "Editar" prominente

#### 4. **EditPaymentModal**
- **Complejidad:** 🟠 Media
- **Formulario:** Edit form
- **Campos:** Monto, método pago, nota
- **Responsive:**
  - Campos apilados
  - Dropdowns touch-friendly
  - Botones full-width

---

### **ADMIN - Módulo Usuarios (UsersPage)**

#### 5. **CreateUserModal**
- **Complejidad:** 🔴 Alta
- **Formulario:** Wizard 3 steps
- **Campos:** Datos personales + roles + paquetes
- **Responsive:**
  - Full-screen móvil
  - Progress indicator arriba
  - Avatar upload táctil
  - Checkboxes grandes (roles)
  - Tabla paquetes → Cards

#### 6. **EditUserModal**
- **Complejidad:** 🟠 Media
- **Formulario:** Edit form
- **Campos:** Similar a Create pero pre-poblado
- **Responsive:**
  - Igual que CreateUserModal
  - Incluir botón "Desactivar"

---

### **ADMIN - Módulo Paquetes (AdminPackagesPage)**

#### 7. **CreatePackageModal**
- **Complejidad:** 🟠 Media
- **Formulario:** Form con cálculos
- **Campos:** Tipo, clases, precio, fechas
- **Responsive:**
  - Date pickers mobile-native
  - Number inputs con +/- buttons
  - Precio calculado prominente

#### 8. **EditPackageModal**
- **Complejidad:** 🟠 Media
- **Formulario:** Similar a Create
- **Responsive:**
  - Igual que CreatePackageModal
  - Mostrar historial de cambios colapsable

---

### **ADMIN - Módulo Nómina (AdminPayrollPage)**

#### 9. **LiquidarMesModal**
- **Complejidad:** 🔴 Alta
- **Formulario:** Selección profesor + mes/año
- **Tabla:** Clases del periodo
- **Responsive:**
  - Tabla → Cards apiladas
  - Totales sticky en bottom
  - Confirmación prominente

#### 10. **AprobarPagoModal**
- **Complejidad:** 🟢 Baja
- **Formulario:** Confirmación con valor adicional opcional
- **Responsive:**
  - Info de clase legible
  - Input valor adicional grande
  - Botones separados y claros

#### 11. **RegistrarPagoModal**
- **Complejidad:** 🟠 Media
- **Formulario:** Fecha pago + observaciones
- **Responsive:**
  - Date picker nativo móvil
  - Textarea espacioso
  - Resumen de liquidación visible

#### 12. **DetalleProfesorModal**
- **Complejidad:** 🟠 Media
- **Contenido:** Clases + historial pagos
- **Responsive:**
  - Tabs horizontales móvil
  - Listas scrolleables
  - Cards en lugar de tabla

---

### **ADMIN - Módulo Clases (ClassesPage)**

#### 13. **ClaseFormModal** (Create/Edit)
- **Complejidad:** 🔴 Alta
- **Formulario:** Datos clase + asignación profesores
- **Campos:** Nombre, fecha, hora, tipo, profesores, máximo alumnos
- **Responsive:**
  - Time pickers nativos
  - Multi-select profesores táctil
  - Calendar view para fecha

---

### **PROFESOR - Módulo Asistencias (ProfesorAttendancePage)**

#### 14. **ConfirmarAsistenciaModal**
- **Complejidad:** 🟠 Media
- **Formulario:** Lista alumnos con checkboxes
- **Responsive:**
  - Lista scrolleable
  - Checkboxes grandes (min 44px)
  - Filtro búsqueda sticky top
  - Botón guardar sticky bottom

---

### **ALUMNO - Módulo Pagos (StudentPaymentsPage)**

#### 15. **VisualizarComprobanteModal**
- **Complejidad:** 🟢 Baja
- **Contenido:** Imagen fullscreen
- **Responsive:**
  - Pinch to zoom
  - Swipe to close
  - Botones mínimos

---

## 🛠️ COMPONENTE BASE: ResponsiveModal

### Implementación Propuesta

```typescript
// ResponsiveModal.tsx
import { useBreakpoint } from '@/shared/hooks/useBreakpoint'
import { AnimatePresence, motion } from 'framer-motion'
import { X } from 'lucide-react'
import { useEffect } from 'react'

interface ResponsiveModalProps {
  isOpen: boolean
  onClose: () => void
  title: string
  children: React.ReactNode
  size?: 'sm' | 'md' | 'lg' | 'xl' | 'full'
  showHeader?: boolean
  allowSwipeClose?: boolean
}

export function ResponsiveModal({
  isOpen,
  onClose,
  title,
  children,
  size = 'md',
  showHeader = true,
  allowSwipeClose = true
}: ResponsiveModalProps) {
  const { isMobile, isTablet } = useBreakpoint()
  
  // Prevent body scroll when modal is open
  useEffect(() => {
    if (isOpen) {
      document.body.style.overflow = 'hidden'
    } else {
      document.body.style.overflow = 'unset'
    }
    return () => {
      document.body.style.overflow = 'unset'
    }
  }, [isOpen])

  const getModalClasses = () => {
    if (isMobile) return 'w-full h-full rounded-none'
    if (isTablet && size === 'full') return 'w-full h-full rounded-none'
    
    const sizeClasses = {
      sm: 'w-full max-w-md',
      md: 'w-full max-w-2xl',
      lg: 'w-full max-w-4xl',
      xl: 'w-full max-w-6xl',
      full: 'w-full h-full'
    }
    
    return `${sizeClasses[size]} max-h-[90vh] rounded-xl`
  }

  const modalAnimation = isMobile
    ? {
        initial: { y: '100%' },
        animate: { y: 0 },
        exit: { y: '100%' }
      }
    : {
        initial: { opacity: 0, scale: 0.95 },
        animate: { opacity: 1, scale: 1 },
        exit: { opacity: 0, scale: 0.95 }
      }

  return (
    <AnimatePresence>
      {isOpen && (
        <>
          {/* Overlay */}
          <motion.div
            className="fixed inset-0 bg-black/50 z-50"
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            onClick={onClose}
          />
          
          {/* Modal */}
          <div className="fixed inset-0 z-50 flex items-center justify-center p-0 md:p-4">
            <motion.div
              className={`bg-[#1a1a1a] border border-[#404040] ${getModalClasses()} flex flex-col`}
              {...modalAnimation}
              transition={{ type: 'spring', damping: 30 }}
            >
              {/* Header */}
              {showHeader && (
                <div className="flex items-center justify-between p-4 md:p-6 border-b border-[#404040]">
                  <h2 className="text-lg md:text-xl font-semibold text-white">
                    {title}
                  </h2>
                  <button
                    onClick={onClose}
                    className="p-2 hover:bg-[#2a2a2a] rounded-lg transition-colors min-w-[44px] min-h-[44px] flex items-center justify-center"
                    aria-label="Cerrar modal"
                  >
                    <X size={24} className="text-[#9ca3af]" />
                  </button>
                </div>
              )}
              
              {/* Content */}
              <div className="flex-1 overflow-y-auto p-4 md:p-6">
                {children}
              </div>
            </motion.div>
          </div>
        </>
      )}
    </AnimatePresence>
  )
}
```

---

## 📝 CHECKLIST DE IMPLEMENTACIÓN POR MODAL

### Para cada modal:

- [ ] **Estructura**
  - [ ] Usa ResponsiveModal como base
  - [ ] Header con título claro
  - [ ] Botón cerrar visible y accesible
  - [ ] Content area scrolleable

- [ ] **Formularios**
  - [ ] Campos apilados verticalmente en móvil
  - [ ] Labels sobre los inputs
  - [ ] Input height mínimo 48px
  - [ ] Spacing entre campos 16px+
  - [ ] Validación inline visible

- [ ] **Botones**
  - [ ] Height mínimo 44px
  - [ ] Full-width en móvil
  - [ ] Separación clara (primario vs secundario)
  - [ ] Loading states claros
  - [ ] Disabled states visuales

- [ ] **Imágenes/Archivos**
  - [ ] Upload area grande y táctil
  - [ ] Preview responsive
  - [ ] Indicador de progreso
  - [ ] Mensajes de error claros

- [ ] **Tablas/Listas**
  - [ ] Convertir a cards en móvil
  - [ ] Scroll horizontal si es necesario
  - [ ] Sticky headers si aplica
  - [ ] Empty states claros

- [ ] **Testing**
  - [ ] Probado en iPhone SE (viewport pequeño)
  - [ ] Probado en tablet
  - [ ] Keyboard navigation funciona
  - [ ] Touch gestures funcionan
  - [ ] No hay scroll issues

---

## 🎯 PRIORIZACIÓN

### 🔴 Críticos (Bloquean flujos principales)
1. RegisterPaymentModal
2. ConfirmarAsistenciaModal
3. CreateUserModal
4. LiquidarMesModal

### 🟠 Importantes (Uso frecuente)
5. VerifyPaymentModal
6. EditPaymentModal
7. ClaseFormModal
8. AprobarPagoModal

### 🟢 Secundarios (Uso ocasional)
9. Resto de modales

---

## 📊 ESTIMACIÓN DE ESFUERZO

| Modal | Complejidad | Tiempo Estimado |
|-------|-------------|-----------------|
| RegisterPaymentModal | Alta | 4h |
| CreateUserModal | Alta | 4h |
| LiquidarMesModal | Alta | 3h |
| ClaseFormModal | Alta | 3h |
| VerifyPaymentModal | Media | 2h |
| EditPaymentModal | Media | 2h |
| Resto (9 modales) | Baja-Media | 1-2h c/u |

**Total estimado:** ~25-30 horas de trabajo solo en modales

---

## ✅ SIGUIENTE PASO

Los modales se implementarán **dentro de cada fase** correspondiente a su módulo, no como una fase separada. Esto permite:

1. ✅ Probar el flujo completo de cada funcionalidad
2. ✅ Detectar issues de UX temprano
3. ✅ Reutilizar patrones dentro del mismo módulo
4. ✅ No bloquear el testing de cada fase

---

**Última actualización:** 13 Febrero 2026
