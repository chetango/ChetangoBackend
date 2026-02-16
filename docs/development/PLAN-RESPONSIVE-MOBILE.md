# 📱 PLAN DE IMPLEMENTACIÓN: DISEÑO RESPONSIVE PARA MÓVILES

**Proyecto:** Chetango Dance Studio Management  
**Fecha Inicio:** 16 Febrero 2026  
**Estado:** 🟢 En Desarrollo - Fase 0  
**Responsable:** Equipo Desarrollo Frontend  
**Última Actualización:** 16 Feb 2026

## 📊 PROGRESO GENERAL

```
Progreso Total: ██████░░░░░░░░░░░░░░ 25%

Fase 0: Setup Base        [✅ COMPLETADA]   ██████████ 100%
Fase 1: Módulo Alumno     [🟢 EN PROGRESO]  ████░░░░░░ 35%
Fase 2: Módulo Profesor   [⚪ PENDIENTE]    ░░░░░░░░░░ 0%
Fase 3: Módulo Admin      [⚪ PENDIENTE]    ░░░░░░░░░░ 0%
Fase 4: Componentes Shared[⚪ PENDIENTE]    ░░░░░░░░░░ 0%
Fase 5: Testing & Polish  [⚪ PENDIENTE]    ░░░░░░░░░░ 0%
```

**Días transcurridos:** 2.5 / 36  
**Fase actual:** 🟢 Fase 1 - Dashboard Alumno responsive  
**Próximo milestone:** Completar cards restantes del dashboard

---

## 📋 ÍNDICE

1. [Resumen Ejecutivo](#resumen-ejecutivo)
2. [Análisis de Situación Actual](#análisis-de-situación-actual)
3. [Arquitectura y Principios](#arquitectura-y-principios)
4. [Estrategia de Implementación](#estrategia-de-implementación)
5. [Plan de Trabajo Detallado](#plan-de-trabajo-detallado)
6. [Sistema de Componentes](#sistema-de-componentes)
7. [Guía de Desarrollo](#guía-de-desarrollo)
8. [Testing y Validación](#testing-y-validación)
9. [Control de Cambios](#control-de-cambios)

---

## 🎯 RESUMEN EJECUTIVO

### Objetivo
Implementar diseño responsive que permita el uso óptimo de la aplicación en dispositivos móviles, manteniendo intacta la experiencia desktop actual.

### Alcance
- **Frontend:** Ajustes visuales y de layout (React + TailwindCSS)
- **Backend:** Sin cambios (API REST permanece igual)
- **Usuarios Impactados:** Solo mejora para usuarios móviles
- **Desktop:** Sin cambios visuales ni funcionales

### Métricas de Éxito
- ✅ 0 errores en desktop después de cambios
- ✅ 0 scroll horizontal en móviles <400px
- ✅ 100% de funcionalidad accesible desde móvil
- ✅ Lighthouse Mobile Score > 85
- ✅ Touch targets > 44px
- ✅ Modales full-screen funcionales en móvil (15+ modales)
- ✅ Animaciones fluidas 60fps (micro-interacciones)
- ✅ Feedback táctil en <100ms
- ✅ Gestos intuitivos (swipe, pull-to-refresh)

### Alcance Ampliado
- **Total Páginas:** 20 páginas (5 Alumno + 6 Profesor + 9 Admin)
- **Total Modales:** 15+ modales críticos de funcionalidad
- **Diseño Visual:** Glassmorphism + micro-interacciones + animaciones modernas
- **UX Móvil:** Gestos táctiles, feedback visual, transiciones fluidas
- **Timeline:** ~36 días laborables (1.7 meses) - incluye polish visual

---

## 📊 ANÁLISIS DE SITUACIÓN ACTUAL

### Estado del Proyecto

```
Estructura Actual:
├── Backend (C# .NET 8)
│   ├── API REST funcionando ✅
│   ├── Azure AD B2C Auth ✅
│   └── Base de datos SQL Server ✅
├── Frontend (React 19 + TypeScript + Vite)
│   ├── TailwindCSS v4 configurado ✅
│   ├── Design System implementado ✅
│   ├── Atomic Design parcial ✅
│   └── Responsive PARCIALMENTE implementado ⚠️
└── Deploy
    ├── Azure Static Web Apps ✅
    ├── CI/CD GitHub Actions ✅
    └── Producción funcionando ✅
```

### Problemas Identificados en Móvil

#### 🔴 **Críticos (Impiden uso)**
1. **Sidebar sin menú móvil:** No se puede acceder a navegación
2. **Tablas desbordadas:** Scroll horizontal infinito
3. **Modales cortados:** Contenido fuera de viewport
4. **Cards con ancho fijo:** min-w-[380px] causa overflow

#### 🟠 **Importantes (Dificultan uso)**
5. **Spacing excesivo:** Padding/gaps muy grandes
6. **Touch targets pequeños:** Botones <44px
7. **Typography sin escalar:** Textos muy grandes
8. **Grids rígidos:** No se adaptan a columnas móvil

#### 🟡 **Mejorables (UX subóptima)**
9. **No hay gestos táctiles:** Swipe en carruseles
10. **Formularios apretados:** Campos muy juntos
11. **Estados hover:** No funcionan en móvil

---

## 🏗️ ARQUITECTURA Y PRINCIPIOS

### Principios de Diseño

#### 1. **Conservar Identidad Visual**
```typescript
/* 🎨 MANTENER: Diseño actual (glassmorphism, colores, estructura) */
// ✅ GlassPanel se mantiene en móvil
// ✅ Paleta de colores sin cambios
// ✅ Tipografía Montserrat se conserva
// ✅ Iconos y branding intactos

/* ✨ AGREGAR: Elementos modernos en móvil */
// + Animaciones suaves (slide, fade, scale)
// + Micro-interacciones (ripple, haptic)
// + Gestos táctiles (swipe, pull)
// + Feedback visual inmediato
// + Skeleton loaders elegantes
```

#### 2. **Mobile-First Approach**
```css
/* ✅ CORRECTO: Diseñar primero para móvil */
.component {
  padding: 1rem;              /* Base: móvil */
  gap: 0.5rem;
}

@media (min-width: 1024px) {
  .component {
    padding: 2rem;            /* Desktop override */
    gap: 1.5rem;
  }
}

/* ❌ INCORRECTO: Desktop-first */
.component {
  padding: 2rem;              /* Base: desktop */
}

@media (max-width: 768px) {
  .component {
    padding: 1rem;            /* Móvil override */
  }
}
```

#### 2. **Progressive Enhancement**
- Funcionalidad core funciona en todos los dispositivos
- Mejoras visuales según capacidades del dispositivo
- Degradación elegante en dispositivos antiguos

#### 3. **Atomic Design System**
```
Atoms (Elementales)
├── GlassPanel → Responsive props
├── Button → Touch-optimized sizes
├── Input → Mobile keyboard types
└── Badge → Adaptive sizing

Molecules (Combinaciones)
├── SearchBar → Collapsible en móvil
├── StatCard → Stacking layout
├── UserCard → Vertical/horizontal variants
└── FilterGroup → Drawer en móvil

Organisms (Secciones)
├── Sidebar → Hamburger menu
├── DataTable → Card view en móvil
├── Modal → Full-screen en móvil
└── Dashboard → Adaptive grid

Templates (Layouts)
├── MainLayout → Responsive container
├── AuthLayout → Centered en todas pantallas
└── DashboardLayout → Fluid grid system
```

#### 4. **Clean Code & Mantenibilidad**

##### Estructura de Archivos
```
src/
├── design-system/
│   ├── atoms/
│   │   ├── GlassPanel/
│   │   │   ├── GlassPanel.tsx
│   │   │   ├── GlassPanel.module.scss
│   │   │   └── GlassPanel.responsive.tsx (nuevo)
│   │   └── Button/
│   │       ├── Button.tsx
│   │       └── Button.responsive.tsx (nuevo)
│   ├── molecules/
│   │   └── ResponsiveTable/
│   │       ├── ResponsiveTable.tsx (nuevo)
│   │       ├── TableDesktop.tsx
│   │       └── TableMobile.tsx
│   └── templates/
│       └── MainLayout/
│           ├── MainLayout.tsx
│           ├── MainLayout.module.scss
│           └── components/
│               ├── MobileMenu.tsx (nuevo)
│               └── DesktopSidebar.tsx
├── shared/
│   ├── hooks/
│   │   ├── useBreakpoint.ts (nuevo)
│   │   ├── useMediaQuery.ts (nuevo)
│   │   └── useTouchGestures.ts (nuevo)
│   ├── utils/
│   │   └── responsive.ts (nuevo)
│   └── constants/
│       └── breakpoints.ts (nuevo)
└── features/
    ├── dashboard/
    │   ├── alumno/
    │   │   ├── components/
    │   │   │   ├── AlumnoHeader.tsx
    │   │   │   ├── AlumnoHeader.mobile.tsx (nuevo)
    │   │   │   └── AlumnoHeader.desktop.tsx (nuevo)
    │   │   └── hooks/
    │   └── admin/
    └── ...
```

##### Naming Conventions
```typescript
// Componentes responsive
ComponentName.tsx           // Componente principal
ComponentName.mobile.tsx    // Variante móvil (si muy diferente)
ComponentName.desktop.tsx   // Variante desktop (si muy diferente)

// Hooks
useBreakpoint()            // Hook de detección de viewport
useIsMobile()              // Simplificado para móvil
useIsTablet()              // Simplificado para tablet

// Utilidades
getResponsiveClasses()     // Helper para clases Tailwind
withResponsive(Component)  // HOC para responsive

// Constantes
BREAKPOINTS                // Definición de breakpoints
MOBILE_PADDING             // Espaciados móvil
TOUCH_TARGET_SIZE          // Tamaño mínimo touch
```

##### Patrones de Código
```typescript
// ============================================
// PATRÓN 1: Componente con variantes
// ============================================
interface ComponentProps {
  variant?: 'mobile' | 'desktop' | 'responsive'
  // ...
}

export const Component = ({ variant = 'responsive', ...props }) => {
  const { isMobile } = useBreakpoint()
  
  if (variant === 'responsive') {
    return isMobile ? <MobileView /> : <DesktopView />
  }
  
  return variant === 'mobile' ? <MobileView /> : <DesktopView />
}

// ============================================
// PATRÓN 2: Clases Tailwind condicionales
// ============================================
const responsiveClasses = clsx(
  'base-class',
  'p-4 sm:p-6 lg:p-8',           // Spacing
  'grid grid-cols-1 lg:grid-cols-2', // Layout
  'text-sm sm:text-base lg:text-lg'  // Typography
)

// ============================================
// PATRÓN 3: Hook de breakpoint
// ============================================
const { isMobile, isTablet, isDesktop, breakpoint } = useBreakpoint()

useEffect(() => {
  if (isMobile) {
    // Lógica específica móvil
  }
}, [isMobile])

// ============================================
// PATRÓN 4: Render condicional limpio
// ============================================
const renderContent = () => {
  if (isMobile) return <MobileContent />
  if (isTablet) return <TabletContent />
  return <DesktopContent />
}

return <div>{renderContent()}</div>
```

---

## 🎯 ESTRATEGIA DE IMPLEMENTACIÓN

### Enfoque: **Modular Secuencial por Rol**

**Decisión:** Implementar **un rol completo a la vez** para:
- ✅ Validar arquitectura con un caso completo
- ✅ Identificar patrones reutilizables temprano
- ✅ Facilitar testing por rol
- ✅ Permitir rollback por rol si es necesario

### Orden de Implementación

```
Fase 0: Setup Base (2 días)
  └─ Configuración y utilidades compartidas

Fase 1: Módulo Alumno (7 días)
  └─ Dashboard + Asistencias + Pagos + Clases + Perfil

Fase 2: Módulo Profesor (8 días)
  └─ Dashboard + Asistencias + Clases + Pagos + Reportes + Perfil

Fase 3: Módulo Admin (8 días)
  └─ Dashboard + Asistencias + Pagos + Clases + Paquetes + Nómina + Usuarios + Reportes + Perfil

Fase 4: Componentes Compartidos (4 días)
  └─ Login, navegación, design system

Fase 5: Testing & Refinamiento (3 días)
  └─ Testing exhaustivo, ajustes finales
```

### Branching Strategy

```
main (producción)
  └── develop (staging)
      ├── feature/responsive-setup       (Fase 0)
      ├── feature/responsive-alumno      (Fase 1)
      ├── feature/responsive-profesor    (Fase 2)
      ├── feature/responsive-admin       (Fase 3)
      ├── feature/responsive-shared      (Fase 4)
      └── feature/responsive-testing     (Fase 5)
```

### Workflow por Commit
```bash
# 1. Hacer cambios pequeños e incrementales
# 2. Commit descriptivo
git commit -m "feat(responsive): AlumnoHeader mobile layout (PC unchanged)"

# 3. Push a develop
git push origin develop

# 4. Esperar CI/CD (5-10 min)
# 5. Probar en móvil: https://app.corporacionchetango.com
# 6. Validar en PC: Debe verse IGUAL que antes
# 7. Si todo OK → siguiente componente
```

---

## 📅 PLAN DE TRABAJO DETALLADO

### **FASE 0: Setup Base** (2 días)
**Estado:** � EN PROGRESO (16 Feb 2026)  
**Responsable:** Equipo Frontend  
**Branch:** `feature/responsive-setup`  
**Progreso:** ████░░░░░░ 20% completado

#### ✅ Tareas Completadas
- [x] Actualizar documento con sistema de tracking de progreso
- [x] Definir estructura de carpetas para responsive
- [x] Crear branch `feature/responsive-setup`
- [x] Instalar dependencias: `framer-motion`, `@use-gesture/react`
- [x] Configurar Tailwind con breakpoints personalizados
- [x] Crear `src/shared/constants/breakpoints.ts`
- [x] Crear `src/shared/constants/responsive.ts`
- [x] Crear `src/shared/hooks/useBreakpoint.ts`
- [x] Crear `src/shared/hooks/useMediaQuery.ts`
- [x] Crear `src/shared/hooks/useTouchGestures.ts`
- [x] Crear `src/shared/components/responsive/ResponsiveContainer.tsx`
- [x] Actualizar exports en `index.ts` de hooks

#### 📦 Archivos Creados
```
src/
├── shared/
│   ├── constants/
│   │   ├── breakpoints.ts      ✅ (60 líneas)
│   │   └── responsive.ts       ✅ (180 líneas)
│   ├── hooks/
│   │   ├── useBreakpoint.ts    ✅ (140 líneas)
│   │   ├── useMediaQuery.ts    ✅ (90 líneas)
│   │   └── useTouchGestures.ts ✅ (230 líneas)
│   └── components/
│       └── responsive/
│           ├── ResponsiveContainer.tsx ✅ (180 líneas)
│           └── index.ts        ✅
└── tailwind.config.js          ✅ (actualizado)
```

---

### **FASE 1: Módulo Alumno** (7 días)
**Estado:** ⚪ PENDIENTE  
**Responsable:** TBD  
**Branch:** `feature/responsive-alumno`  
**Progreso:** ░░░░░░░░░░ 0% completado

#### 📋 Pendiente

##### Día 1: Configuración
- [ ] **Tailwind Config**
  - Definir breakpoints explícitos
  - Agregar utilidades responsive custom
  - Configurar spacing mobile-first
  
- [ ] **Constantes Globales**
  ```typescript
  // src/shared/constants/breakpoints.ts
  export const BREAKPOINTS = {
    xs: 375,
    sm: 640,
    md: 768,
    lg: 1024,
    xl: 1280,
    '2xl': 1536,
  }
  
  export const SPACING = {
    mobile: { padding: '1rem', gap: '0.5rem' },
    tablet: { padding: '1.5rem', gap: '0.75rem' },
    desktop: { padding: '2rem', gap: '1rem' },
  }
  
  export const TOUCH_TARGET_MIN = 44 // px
  ```

- [ ] **CSS Variables**
  ```css
  /* src/index.css */
  :root {
    --mobile-padding: 1rem;
    --tablet-padding: 1.5rem;
    --desktop-padding: 2rem;
    --touch-target-min: 44px;
  }
  ```

#### Día 2: Hooks y Utilidades
- [ ] **useBreakpoint Hook**
  ```typescript
  // src/shared/hooks/useBreakpoint.ts
  export function useBreakpoint() {
    const [breakpoint, setBreakpoint] = useState<Breakpoint>('lg')
    
    useEffect(() => {
      const handleResize = () => {
        const width = window.innerWidth
        if (width < 640) setBreakpoint('xs')
        else if (width < 768) setBreakpoint('sm')
        else if (width < 1024) setBreakpoint('md')
        else if (width < 1280) setBreakpoint('lg')
        else setBreakpoint('xl')
      }
      
      handleResize()
      window.addEventListener('resize', handleResize)
      return () => window.removeEventListener('resize', handleResize)
    }, [])
    
    return {
      breakpoint,
      isMobile: breakpoint === 'xs' || breakpoint === 'sm',
      isTablet: breakpoint === 'md',
      isDesktop: breakpoint === 'lg' || breakpoint === 'xl',
    }
  }
  ```

- [ ] **Utilidades Responsive**
  ```typescript
  // src/shared/utils/responsive.ts
  export const responsiveContainer = clsx(
    'px-4 sm:px-6 lg:px-8 xl:px-12',
    'max-w-7xl mx-auto'
  )
  
  export const responsivePadding = {
    sm: 'p-3 sm:p-4 lg:p-5',
    md: 'p-4 sm:p-5 lg:p-6',
    lg: 'p-5 sm:p-6 lg:p-8',
  }
  
  export const responsiveGrid = {
    '1-2': 'grid grid-cols-1 lg:grid-cols-2',
    '1-3': 'grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3',
    '1-4': 'grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-4',
  }
  ```

**Entregables:**
- ✅ Tailwind configurado con breakpoints
- ✅ Hooks responsive funcionando
- ✅ Utilidades documentadas
- ✅ Ejemplos de uso en Storybook (opcional)

---

### **FASE 1: Módulo Alumno Completo** (7 días)
**Estado:** 🔴 Pendiente  
**Responsable:** TBD  
**Branch:** `feature/responsive-alumno`  
**Prioridad:** 🔴 Alta (Usuarios más frecuentes)

#### Alcance
- StudentDashboardPage ✅
- StudentAttendancePage ✅
- StudentPaymentsPage ✅
- StudentClassesPage ✅
- StudentProfilePage ✅

#### Componentes a Modificar

##### Día 1: Layout Base Dashboard
- [ ] **StudentDashboardPage.tsx**
  - Ajustar padding contenedor: `px-4 sm:px-6 lg:px-12`
  - Ajustar gaps grids: `gap-4 sm:gap-5 lg:gap-6`
  - Validar spacing vertical

##### Día 2: Header y Cards Principales
- [ ] **AlumnoHeader.tsx**
  - Layout flexible: columna en móvil, fila en desktop
  - Card perfil full-width en móvil
  - Typography escalada
  
- [ ] **CredencialDigitalCard.tsx**
  - QR size responsivo: 180px → 240px → 280px
  - Padding interno adaptativo
  - Typography del nombre

##### Día 3: Cards de Progreso
- [ ] **MiPaqueteCard.tsx**
  - Progress bar full-width
  - Números grandes legibles
  - Spacing interno optimizado
  
- [ ] **ProximaClaseCard.tsx**
  - Layout vertical en móvil
  - Iconos y texto balanceados
  - Botones touch-friendly (min 44px)
  
- [ ] **MiAsistenciaCard.tsx**
  - Gráficos responsivos
  - Leyenda apilada en móvil

##### Día 4: Secciones Interactivas
- [ ] **RecomendadosSection.tsx**
  - Cards en columna en móvil
  - CTA buttons full-width
  - Código de referido copiable
  
- [ ] **LogrosSection.tsx**
  - Grid adaptativo
  - Badges tamaño móvil
  
- [ ] **EventosCarousel.tsx**
  - Swipe gestures
  - Snap scroll
  - Indicadores de posición

##### Día 5: Páginas Adicionales Alumno (Parte 1)
- [ ] **StudentAttendancePage**
  - Historial de asistencias responsive
  - Tabla adaptativa (desktop) → Cards (móvil)
  - Filtros en drawer móvil
  - Estados visuales claros (Presente/Ausente/Descontada)

- [ ] **StudentPaymentsPage**
  - Lista de pagos responsive
  - Cards con info clave
  - Botón "Registrar Pago" touch-friendly
  - Modales full-screen en móvil

##### Día 6: Páginas Adicionales Alumno (Parte 2)
- [ ] **StudentClassesPage**
  - Lista de clases disponibles
  - Cards responsivas
  - Filtros colapsables
  - Horarios legibles

- [ ] **StudentProfilePage**
  - Formulario en columna móvil
  - Avatar upload touch-friendly
  - Campos apilados
  - Botones full-width en móvil

##### Día 7: Testing y Ajustes Módulo Alumno
- [ ] Testing exhaustivo de todas las páginas alumno
- [ ] Validación desktop sin cambios
- [ ] Ajustes finos de spacing y navegación
- [ ] Documentación de patrones encontrados

**Entregables:**
- ✅ Módulo Alumno COMPLETO 100% funcional en móvil (5 páginas)
- ✅ Desktop sin cambios visuales
- ✅ Patrones documentados para reutilizar
- ✅ Screenshots antes/después

---

### **FASE 2: Módulo Profesor Completo** (8 días)
**Estado:** 🔴 Pendiente  
**Responsable:** TBD  
**Branch:** `feature/responsive-profesor`  
**Prioridad:** 🟠 Media

#### Componentes a Modificar

##### Día 1: Layout y Header
- [ ] **ProfesorDashboardPage.tsx**
  - Container responsive
  - Grid adaptativo
  
- [ ] **ProfesorHeader.tsx**
  - Saludo y stats en columna móvil
  - Avatar y nombre prominentes

##### Día 2: KPIs y Métricas
- [ ] **ProfesorKPIs.tsx**
  - Grid 1 columna móvil
  - 2 columnas tablet
  - 4 columnas desktop
  - Icons tamaño apropiado
  
- [ ] **AsistenciaChart.tsx**
  - Gráfico responsive (Recharts)
  - Leyenda optimizada móvil
  - Touch para detalles

##### Día 3: Clases y Acciones
- [ ] **ClasesHoySection.tsx**
  - Cards apiladas móvil
  - Horizontal desktop
  - Botón "Registrar" accesible
  
- [ ] **ProximasClasesSection.tsx**
  - Lista vertical móvil
  - Información condensada
  - Navegación rápida
  
- [ ] **QuickActionsProfesor.tsx**
  - Botones grandes touch
  - Icons y texto balanceados

##### Día 4: Testing y Refinamiento
- [ ] Testing en dispositivos reales
- [ ] Validación con profesor real
- [ ] Ajustes basados en feedback

**Entregables:**
- ✅ Dashboard Profesor responsive
- ✅ Patrones de gráficos responsive
- ✅ Componentes KPI reutilizables

---

### **FASE 3: Dashboard Admin** (6 días)
**Estado:** 🔴 Pendiente  
**Responsable:** TBD  
**Branch:** `feature/responsive-admin`  
**Prioridad:** 🟡 Media-Alta

#### Componentes a Modificar

##### Día 1-2: Tablas Responsivas
- [ ] **ResponsiveTable Component** (NUEVO)
  ```typescript
  interface ResponsiveTableProps {
    data: any[]
    columns: Column[]
    mobileView?: 'cards' | 'accordion' | 'horizontal-scroll'
  }
  ```
  - Desktop: Tabla normal
  - Tablet: Tabla con scroll horizontal
  - Móvil: Cards apiladas con info clave

- [ ] **Aplicar a:**
  - UsersPage
  - AdminPaymentsPage
  - AdminAttendancePage
  - AdminPackagesPage

##### Día 3-4: Modales Responsive
- [ ] **Modal Base Responsive**
  - Móvil: Full-screen modal
  - Desktop: Centered modal
  - Transiciones apropiadas
  
- [ ] **Actualizar modales:**
  - RegisterPaymentModal
  - VerifyPaymentModal
  - CreateUserModal
  - EditPackageModal
  - ClaseFormModal

##### Día 5: Formularios Complejos
- [ ] **RegisterPaymentModal**
  - Multi-step en móvil
  - Campos apilados
  - File upload touch-friendly
  - Preview de imágenes

- [ ] **CreateUserModal**
  - Wizard con steps
  - Progress indicator
  - Validación inline

##### Día 6: Testing y Refinamiento
- [ ] Testing de flujos completos
- [ ] Validación de permisos
- [ ] Performance en móvil

**Entregables:**
- ✅ Dashboard Admin funcional móvil
- ✅ Sistema de tablas responsive
- ✅ Modales optimizados
- ✅ Formularios usables en móvil

---

### **FASE 4: Componentes Compartidos** (4 días)
**Estado:** 🔴 Pendiente  
**Responsable:** TBD  
**Branch:** `feature/responsive-shared`

#### Día 1: Navegación y Layout
- [ ] **MainLayout**
  - Hamburger menu móvil
  - Sidebar overlay
  - Touch gestures para abrir/cerrar
  
- [ ] **MobileMenu Component** (NUEVO)
  - Slide-in animation
  - Navigation items
  - User profile section
  - Logout button

#### Día 2: Autenticación
- [ ] **LoginPage**
  - Form centrado responsive
  - Botones touch-friendly
  - Logo y branding adaptado
  
- [ ] **AuthCallback**
  - Loading states
  - Error messages legibles

#### Día 3: Páginas Generales
- [ ] **ClassesPage**
  - Filtros en drawer móvil
  - Cards responsivas
  
- [ ] **ReportsPage**
  - Gráficos responsive
  - Export buttons accesibles
  
- [ ] **ProfilePage**
  - Form en columna móvil
  - Avatar upload touch

#### Día 4: Design System
- [ ] **GlassPanel**
  - Padding responsive
  - Border radius adaptativo
  
- [ ] **Button**
  - Touch target min 44px
  - Variantes responsive
  
- [ ] **Input**
  - Mobile keyboard types
  - Touch-friendly

**Entregables:**
- ✅ Navegación móvil funcional
- ✅ Login responsive
- ✅ Design System actualizado

---

### **FASE 5: Testing & Refinamiento** (3 días)
**Estado:** 🔴 Pendiente  
**Responsable:** QA + Dev  
**Branch:** `feature/responsive-testing`

#### Día 1: Testing Automatizado
- [ ] **Playwright Tests**
  ```typescript
  const VIEWPORTS = [
    { name: 'iPhone SE', width: 375, height: 667 },
    { name: 'iPhone 12', width: 390, height: 844 },
    { name: 'iPad', width: 768, height: 1024 },
    { name: 'Desktop', width: 1280, height: 720 },
  ]
  
  VIEWPORTS.forEach(viewport => {
    test(`Dashboard loads on ${viewport.name}`, async ({ page }) => {
      await page.setViewportSize(viewport)
      await page.goto('/student/dashboard')
      await expect(page.getByText('Mi Dashboard')).toBeVisible()
    })
  })
  ```

- [ ] **Visual Regression Tests**
  - Screenshots de referencia
  - Comparación automática
  - Alertas de cambios

#### Día 2: Testing Manual
- [ ] **Checklist por Rol**
  ```
  Dashboard Alumno:
  - [ ] Login móvil funciona
  - [ ] QR visible y escaneab le
  - [ ] Próxima clase legible
  - [ ] Paquete con info clara
  - [ ] Navegación sidebar
  - [ ] Eventos swipeable
  - [ ] Código referido copiable
  
  Dashboard Profesor:
  - [ ] Clases de hoy visibles
  - [ ] Registrar asistencia funciona
  - [ ] Gráficos legibles
  - [ ] Quick actions accesibles
  
  Dashboard Admin:
  - [ ] Tablas usables (cards)
  - [ ] Modales completos
  - [ ] Formularios funcionales
  - [ ] Export funciona
  ```

- [ ] **Testing en Dispositivos Reales**
  - iPhone (Safari iOS)
  - Android (Chrome)
  - Tablet iPad
  - Tablet Android

#### Día 3: Optimización y Documentación
- [ ] **Performance**
  - Lighthouse Mobile Score > 85
  - First Contentful Paint < 2s
  - Time to Interactive < 3s
  - Bundle size optimizado
  
- [ ] **Accesibilidad**
  - WCAG AA compliance
  - Screen reader friendly
  - Touch targets > 44px
  
- [ ] **Documentación Final**
  - Guía de uso móvil
  - Patrones documentados
  - Screenshots finales
  - Video demo

**Entregables:**
- ✅ Test suite completo
- ✅ Performance optimizado
- ✅ Documentación actualizada
- ✅ App lista para producción

---

## 🧩 SISTEMA DE COMPONENTES

### Componentes Nuevos a Crear

#### 1. MobileMenu (Navegación)
```typescript
// src/design-system/templates/MainLayout/components/MobileMenu.tsx

interface MobileMenuProps {
  isOpen: boolean
  onClose: () => void
  navigationItems: NavItem[]
  user: User
  onLogout: () => void
}

export const MobileMenu = ({ 
  isOpen, 
  onClose, 
  navigationItems, 
  user, 
  onLogout 
}: MobileMenuProps) => {
  return (
    <div className={clsx(
      'fixed inset-0 z-50',
      'lg:hidden', // Solo visible en móvil/tablet
      isOpen ? 'block' : 'hidden'
    )}>
      {/* Backdrop */}
      <div 
        className="absolute inset-0 bg-black/60 backdrop-blur-sm"
        onClick={onClose}
      />
      
      {/* Menu Panel */}
      <div className={clsx(
        'absolute left-0 top-0 bottom-0',
        'w-[280px] bg-[#1a1a1a]',
        'shadow-2xl',
        'transform transition-transform duration-300',
        isOpen ? 'translate-x-0' : '-translate-x-full'
      )}>
        {/* Header */}
        <div className="p-4 border-b border-white/10">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-3">
              <div className="w-10 h-10 rounded-full bg-[#c93448] flex items-center justify-center">
                <span className="text-white font-bold text-lg">
                  {user.name?.charAt(0)}
                </span>
              </div>
              <div>
                <p className="text-white font-medium">{user.name}</p>
                <p className="text-gray-400 text-sm">{user.role}</p>
              </div>
            </div>
            <button onClick={onClose} className="p-2">
              <X className="w-5 h-5 text-gray-400" />
            </button>
          </div>
        </div>
        
        {/* Navigation */}
        <nav className="p-4 space-y-2">
          {navigationItems.map(item => (
            <Link
              key={item.path}
              to={item.path}
              onClick={onClose}
              className="flex items-center gap-3 p-3 rounded-lg hover:bg-white/5"
            >
              {item.icon && <item.icon className="w-5 h-5" />}
              <span>{item.label}</span>
            </Link>
          ))}
        </nav>
        
        {/* Footer */}
        <div className="absolute bottom-0 left-0 right-0 p-4 border-t border-white/10">
          <button
            onClick={onLogout}
            className="w-full flex items-center justify-center gap-2 p-3 bg-red-500/20 text-red-400 rounded-lg"
          >
            <LogOut className="w-5 h-5" />
            Cerrar Sesión
          </button>
        </div>
      </div>
    </div>
  )
}
```

#### 2. ResponsiveTable
```typescript
// src/shared/components/ResponsiveTable/ResponsiveTable.tsx

interface Column<T> {
  key: keyof T
  label: string
  render?: (value: any, item: T) => React.ReactNode
  hideOnMobile?: boolean
}

interface ResponsiveTableProps<T> {
  data: T[]
  columns: Column<T>[]
  keyExtractor: (item: T) => string
  onRowClick?: (item: T) => void
  mobileCardRender?: (item: T) => React.ReactNode
}

export function ResponsiveTable<T>({
  data,
  columns,
  keyExtractor,
  onRowClick,
  mobileCardRender
}: ResponsiveTableProps<T>) {
  const { isMobile } = useBreakpoint()
  
  if (isMobile && mobileCardRender) {
    return (
      <div className="space-y-3">
        {data.map(item => (
          <GlassPanel 
            key={keyExtractor(item)}
            onClick={() => onRowClick?.(item)}
            className="p-4 cursor-pointer"
          >
            {mobileCardRender(item)}
          </GlassPanel>
        ))}
      </div>
    )
  }
  
  if (isMobile) {
    return (
      <div className="space-y-3">
        {data.map(item => (
          <GlassPanel key={keyExtractor(item)} className="p-4">
            {columns.filter(col => !col.hideOnMobile).map(col => (
              <div key={String(col.key)} className="flex justify-between py-2">
                <span className="text-gray-400 text-sm">{col.label}</span>
                <span className="text-white font-medium">
                  {col.render 
                    ? col.render(item[col.key], item)
                    : String(item[col.key])}
                </span>
              </div>
            ))}
          </GlassPanel>
        ))}
      </div>
    )
  }
  
  return (
    <div className="overflow-x-auto">
      <table className="w-full min-w-[800px]">
        <thead>
          <tr>
            {columns.map(col => (
              <th key={String(col.key)}>{col.label}</th>
            ))}
          </tr>
        </thead>
        <tbody>
          {data.map(item => (
            <tr 
              key={keyExtractor(item)}
              onClick={() => onRowClick?.(item)}
            >
              {columns.map(col => (
                <td key={String(col.key)}>
                  {col.render 
                    ? col.render(item[col.key], item)
                    : String(item[col.key])}
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
```

#### 3. ResponsiveModal
```typescript
// src/shared/components/ResponsiveModal/ResponsiveModal.tsx

interface ResponsiveModalProps {
  isOpen: boolean
  onClose: () => void
  title: string
  children: React.ReactNode
  footer?: React.ReactNode
  size?: 'sm' | 'md' | 'lg' | 'xl'
}

export const ResponsiveModal = ({
  isOpen,
  onClose,
  title,
  children,
  footer,
  size = 'md'
}: ResponsiveModalProps) => {
  const { isMobile } = useBreakpoint()
  const containerRef = useModalScroll(isOpen)
  
  if (!isOpen) return null
  
  const sizeClasses = {
    sm: 'sm:max-w-md',
    md: 'sm:max-w-lg',
    lg: 'sm:max-w-2xl',
    xl: 'sm:max-w-4xl',
  }
  
  return (
    <div className="fixed inset-0 z-50 flex items-end sm:items-center justify-center">
      {/* Backdrop */}
      <div 
        className="absolute inset-0 bg-black/60 backdrop-blur-sm"
        onClick={onClose}
      />
      
      {/* Modal */}
      <div 
        ref={containerRef}
        className={clsx(
          'relative',
          'w-full',
          sizeClasses[size],
          'max-h-[90vh] sm:max-h-[85vh]',
          'overflow-y-auto',
          'm-0 sm:m-4',
          'rounded-t-2xl sm:rounded-2xl',
          'bg-[#1a1a1a]',
          'border border-white/10'
        )}
      >
        {/* Header */}
        <div className="sticky top-0 z-10 bg-[#1a1a1a] border-b border-white/10 p-4 sm:p-6">
          <div className="flex items-center justify-between">
            <h2 className="text-xl font-bold text-white">{title}</h2>
            <button
              onClick={onClose}
              className="p-2 hover:bg-white/5 rounded-lg"
            >
              <X className="w-5 h-5 text-gray-400" />
            </button>
          </div>
        </div>
        
        {/* Content */}
        <div className="p-4 sm:p-6">
          {children}
        </div>
        
        {/* Footer */}
        {footer && (
          <div className="sticky bottom-0 bg-[#1a1a1a] border-t border-white/10 p-4 sm:p-6">
            {footer}
          </div>
        )}
      </div>
    </div>
  )
}
```

---

## 📖 GUÍA DE DESARROLLO

### Checklist Pre-Commit

Antes de hacer commit, verificar:

```markdown
## Checklist de Desarrollo

### Código
- [ ] Clases Tailwind son mobile-first
- [ ] No hay anchos fijos (min-w-[XXXpx])
- [ ] Touch targets > 44px
- [ ] Spacing usa breakpoints
- [ ] Typography escala correctamente

### Testing
- [ ] Probado en móvil (<640px)
- [ ] Probado en tablet (768px)
- [ ] Probado en desktop (1024px+)
- [ ] Desktop se ve EXACTAMENTE igual que antes
- [ ] No hay scroll horizontal en móvil

### Performance
- [ ] No se agregaron paquetes innecesarios
- [ ] Imágenes optimizadas
- [ ] Lazy loading implementado

### Documentación
- [ ] Commit message descriptivo
- [ ] Comentarios en código complejo
- [ ] README actualizado si aplica
```

### Convenciones de Commit

```bash
# Tipos de commit
feat(responsive):     # Nueva funcionalidad responsive
fix(responsive):      # Corrección de bug responsive
refactor(responsive): # Refactorización sin cambio funcional
style(responsive):    # Cambios de estilo
docs(responsive):     # Documentación
test(responsive):     # Tests

# Ejemplos
git commit -m "feat(responsive): mobile menu hamburger"
git commit -m "fix(responsive): touch targets too small on buttons"
git commit -m "refactor(responsive): extract useBreakpoint hook"
git commit -m "style(responsive): adjust padding on AlumnoHeader mobile"
```

### Debugging Responsive

```typescript
// Agregar temporalmente para debug
const DebugViewport = () => {
  const { breakpoint, isMobile, isTablet, isDesktop } = useBreakpoint()
  
  if (process.env.NODE_ENV !== 'development') return null
  
  return (
    <div className="fixed bottom-4 right-4 p-3 bg-black/80 text-white text-xs rounded-lg z-50">
      <div>Breakpoint: {breakpoint}</div>
      <div>Width: {window.innerWidth}px</div>
      <div>isMobile: {isMobile ? '✅' : '❌'}</div>
      <div>isTablet: {isTablet ? '✅' : '❌'}</div>
      <div>isDesktop: {isDesktop ? '✅' : '❌'}</div>
    </div>
  )
}

// Agregar en App.tsx
<DebugViewport />
```

---

## 🧪 TESTING Y VALIDACIÓN

### Testing Automatizado

#### Playwright - Tests por Viewport
```typescript
// e2e/responsive/dashboard.spec.ts

import { test, expect } from '@playwright/test'

const VIEWPORTS = [
  { name: 'Mobile Small', width: 375, height: 667 },
  { name: 'Mobile Large', width: 414, height: 896 },
  { name: 'Tablet', width: 768, height: 1024 },
  { name: 'Desktop', width: 1280, height: 720 },
]

VIEWPORTS.forEach(({ name, width, height }) => {
  test.describe(`Dashboard Alumno - ${name}`, () => {
    test.beforeEach(async ({ page }) => {
      await page.setViewportSize({ width, height })
      await page.goto('/login')
      // Login flow...
      await page.goto('/student/dashboard')
    })
    
    test('header is visible and readable', async ({ page }) => {
      const header = page.locator('header')
      await expect(header).toBeVisible()
      
      const heading = page.getByRole('heading', { level: 1 })
      await expect(heading).toBeVisible()
      
      // Verify text is not truncated
      const box = await heading.boundingBox()
      expect(box?.width).toBeLessThan(width - 32) // minus padding
    })
    
    test('all cards are visible without horizontal scroll', async ({ page }) => {
      // Verificar no hay scroll horizontal
      const scrollWidth = await page.evaluate(() => document.documentElement.scrollWidth)
      expect(scrollWidth).toBeLessThanOrEqual(width + 1) // +1 por rounding
    })
    
    test('touch targets are at least 44px', async ({ page }) => {
      const buttons = await page.locator('button').all()
      
      for (const button of buttons) {
        const box = await button.boundingBox()
        if (box) {
          expect(box.height).toBeGreaterThanOrEqual(44)
          // Width puede ser menor si es icon-only, pero height siempre >= 44
        }
      }
    })
  })
})
```

#### Visual Regression
```typescript
// e2e/responsive/visual-regression.spec.ts

test.describe('Visual Regression - Responsive', () => {
  VIEWPORTS.forEach(({ name, width, height }) => {
    test(`screenshot ${name}`, async ({ page }) => {
      await page.setViewportSize({ width, height })
      await page.goto('/student/dashboard')
      
      // Screenshot full page
      await expect(page).toHaveScreenshot(`dashboard-alumno-${name}.png`, {
        fullPage: true,
        maxDiffPixels: 100,
      })
    })
  })
})
```

### Testing Manual

#### Checklist Funcional por Rol

**Dashboard Alumno:**
```markdown
## Testing Móvil - Dashboard Alumno

### Navegación
- [ ] Hamburger menu abre/cierra
- [ ] Links funcionan correctamente
- [ ] Logout funciona

### Dashboard Principal
- [ ] Header visible con nombre completo
- [ ] Card perfil legible
- [ ] QR code visible y escaneabile

### Próxima Clase
- [ ] Card completa visible
- [ ] Fecha y hora legibles
- [ ] Botón "Ver Detalles" funcional (>44px)

### Mi Paquete
- [ ] Clases restantes visible
- [ ] Progress bar muestra correctamente
- [ ] Fecha vencimiento legible

### Asistencias
- [ ] Gráfico visible
- [ ] Números legibles
- [ ] Touch funciona para detalles

### Logros
- [ ] Badges visibles
- [ ] Texto legible
- [ ] Animaciones suaves

### Eventos
- [ ] Carrusel swipeable
- [ ] Cards completas visibles
- [ ] Indicadores de posición

### Recomendaciones
- [ ] Cards en columna
- [ ] Botones grandes (>44px)
- [ ] Código referido copiable
```

**Dashboard Profesor:**
```markdown
## Testing Móvil - Dashboard Profesor

### Clases Hoy
- [ ] Lista visible
- [ ] Información completa por clase
- [ ] Botón "Registrar Asistencia" accesible

### KPIs
- [ ] 4 cards apiladas en móvil
- [ ] Números legibles
- [ ] Icons visibles

### Gráficos
- [ ] Chart responsive
- [ ] Leyenda legible
- [ ] Touch para detalles funciona

### Próximas Clases
- [ ] Lista completa
- [ ] Fechas legibles
- [ ] Navegación funciona

### Quick Actions
- [ ] 3 botones visibles
- [ ] Icons y texto balanceados
- [ ] Touch funciona (>44px)
```

**Dashboard Admin:**
```markdown
## Testing Móvil - Dashboard Admin

### Tablas
- [ ] Usuarios: Card view funciona
- [ ] Pagos: Card view funciona
- [ ] Asistencias: Card view funciona
- [ ] Info clave visible en cards
- [ ] Touch para ver detalles

### Modales
- [ ] RegisterPayment: Full-screen móvil
- [ ] CreateUser: Multi-step funciona
- [ ] EditPackage: Form usable
- [ ] Todos los campos accesibles
- [ ] Submit buttons visibles

### Filtros
- [ ] Drawer móvil funciona
- [ ] Filtros accesibles
- [ ] Botón aplicar visible

### Stats/KPIs
- [ ] Cards apiladas móvil
- [ ] Números legibles
- [ ] Colores visibles
```

---

## 📊 CONTROL DE CAMBIOS

### Registro de Progreso

| Fase | Estado | Inicio | Fin | Responsable | Notas |
|------|--------|--------|-----|-------------|-------|
| Fase 0: Setup | 🔴 Pendiente | - | - | - | - |
| Fase 1: Alumno | 🔴 Pendiente | - | - | - | - |
| Fase 2: Profesor | 🔴 Pendiente | - | - | - | - |
| Fase 3: Admin | 🔴 Pendiente | - | - | - | - |
| Fase 4: Shared | 🔴 Pendiente | - | - | - | - |
| Fase 5: Testing | 🔴 Pendiente | - | - | - | - |

### Leyenda de Estados
- 🔴 Pendiente
- 🟡 En Progreso
- 🟢 Completado
- 🔵 En Revisión
- 🟣 Bloqueado

### Issues y Bloqueos

| ID | Fecha | Descripción | Estado | Resolución |
|----|-------|-------------|--------|------------|
| - | - | - | - | - |

### Decisiones de Arquitectura

| ID | Fecha | Decisión | Razón | Impacto |
|----|-------|----------|-------|---------|
| ADR-001 | 13-Feb-2026 | Mobile-First Approach | Mejora UX móvil, no afecta desktop | Cambio en metodología CSS |
| ADR-002 | 13-Feb-2026 | Implementar por Rol (no paralelo) | Validación temprana, patrones reutilizables | Secuencial vs paralelo |
| ADR-003 | 13-Feb-2026 | Tailwind classes sobre SCSS | Consistencia, mantenibilidad | Reducir SCSS custom |

---

## 📚 RECURSOS Y REFERENCIAS

### Documentación Técnica
- [TailwindCSS Responsive Design](https://tailwindcss.com/docs/responsive-design)
- [React Responsive Patterns](https://web.dev/responsive-web-design-basics/)
- [Touch Target Sizes (Material Design)](https://material.io/design/usability/accessibility.html#layout-typography)
- [Mobile UX Best Practices](https://www.nngroup.com/articles/mobile-ux/)

### Herramientas
- Chrome DevTools Device Mode
- [Responsively App](https://responsively.app/) - Browser para testing responsive
- [BrowserStack](https://www.browserstack.com/) - Testing en dispositivos reales
- Lighthouse CI - Performance monitoring

### Diseño
- [Figma](https://www.figma.com/) - Para mockups responsive
- [Atomic Design Methodology](https://atomicdesign.bradfrost.com/)

---

## ✅ CHECKLIST FINAL

### Antes de Merge a Main

```markdown
## Pre-Merge Checklist

### Funcionalidad
- [ ] Todos los roles funcionan en móvil
- [ ] Desktop sin cambios visuales
- [ ] No hay regresiones

### Performance
- [ ] Lighthouse Mobile Score > 85
- [ ] First Contentful Paint < 2s
- [ ] Bundle size no aumentó >10%

### Accesibilidad
- [ ] WCAG AA compliance
- [ ] Screen reader tested
- [ ] Touch targets >= 44px

### Testing
- [ ] Tests automatizados pasan
- [ ] Testing manual completo
- [ ] Testeo en dispositivos reales

### Documentación
- [ ] Plan actualizado
- [ ] Screenshots agregados
- [ ] Changelog actualizado
- [ ] README actualizado

### Deploy
- [ ] Build de producción exitoso
- [ ] No hay console errors
- [ ] Analytics configurado
```

---

## 🎉 CONCLUSIÓN

Este plan proporciona una hoja de ruta clara y estructurada para implementar diseño responsive en la aplicación Chetango, garantizando:

✅ **Mantenibilidad:** Código limpio, patrones reutilizables  
✅ **Calidad:** Testing exhaustivo en cada fase  
✅ **Seguridad:** Desktop no se ve afectado  
✅ **Performance:** Optimizado para móviles  
✅ **Escalabilidad:** Arquitectura preparada para el futuro  

**Próximos Pasos:**
1. Asignar responsables por fase
2. Iniciar Fase 0 (Setup Base)
3. Actualizar este documento conforme avanzamos

---

**Última actualización:** 13 Febrero 2026  
**Versión:** 1.0  
**Mantenido por:** Equipo Desarrollo Chetango
