# 🎨 DISEÑO VISUAL Y MICRO-INTERACCIONES MÓVIL

**Proyecto:** Chetango Dance Studio - Responsive Mobile  
**Fecha:** 13 Febrero 2026  
**Propósito:** Especificación de elementos visuales avanzados para experiencia móvil moderna y elegante

---

## 📋 PRINCIPIO FUNDAMENTAL

> **CONSERVAR** el diseño actual (glassmorphism, colores, estructura)  
> **AGREGAR** elementos modernos que eleven la experiencia móvil

---

## 🎭 IDENTIDAD VISUAL ACTUAL (MANTENER)

### Elementos a Conservar 100%

```typescript
// ✅ GLASSMORPHISM - Se mantiene y mejora en móvil
const glassStyles = {
  background: 'rgba(255, 255, 255, 0.1)',
  backdropFilter: 'blur(10px)',
  border: '1px solid rgba(255, 255, 255, 0.2)',
  boxShadow: '0 8px 32px 0 rgba(31, 38, 135, 0.37)'
}

// ✅ PALETA DE COLORES - Sin cambios
const colors = {
  primary: '#FF6B9D',      // Rosa vibrante
  secondary: '#4A90E2',    // Azul
  accent: '#FFD93D',       // Amarillo
  dark: '#1A1A2E',         // Fondo oscuro
  glass: 'rgba(255, 255, 255, 0.1)'
}

// ✅ TIPOGRAFÍA - Montserrat se conserva
const typography = {
  fontFamily: 'Montserrat, sans-serif',
  // Solo ajustar tamaños para móvil, NO cambiar fuente
}

// ✅ ESTRUCTURA - Layout y componentes actuales
// MainLayout, DashboardLayout, GlassPanel → Intactos
// Solo agregar variantes móviles, NO reemplazar
```

---

## ✨ ELEMENTOS MODERNOS A AGREGAR

### 1. 🎬 Animaciones y Transiciones

#### A. Transiciones de Página (React Router)
```typescript
// Mobile page transitions
const pageTransitions = {
  initial: { opacity: 0, x: 20 },
  animate: { opacity: 1, x: 0 },
  exit: { opacity: 0, x: -20 },
  transition: { duration: 0.3, ease: 'easeInOut' }
}

// Aplicar en routes móviles
<AnimatePresence mode="wait">
  <motion.div
    key={location.pathname}
    initial={isMobile ? pageTransitions.initial : undefined}
    animate={isMobile ? pageTransitions.animate : undefined}
    exit={isMobile ? pageTransitions.exit : undefined}
  >
    {children}
  </motion.div>
</AnimatePresence>
```

#### B. Micro-interacciones en Botones
```typescript
// Ripple effect + scale en touch
const ButtonMobile = ({ children, ...props }) => {
  const [ripples, setRipples] = useState([])
  
  const handleTouchStart = (e) => {
    const rect = e.target.getBoundingClientRect()
    const x = e.touches[0].clientX - rect.left
    const y = e.touches[0].clientY - rect.top
    
    setRipples([...ripples, { x, y, id: Date.now() }])
    
    // Haptic feedback (si disponible)
    if ('vibrate' in navigator) {
      navigator.vibrate(10)
    }
  }
  
  return (
    <motion.button
      whileTap={{ scale: 0.95 }}
      onTouchStart={handleTouchStart}
      className="relative overflow-hidden"
      {...props}
    >
      {ripples.map(ripple => (
        <span
          key={ripple.id}
          className="absolute rounded-full bg-white/30 animate-ripple"
          style={{
            left: ripple.x,
            top: ripple.y,
            width: 0,
            height: 0
          }}
        />
      ))}
      {children}
    </motion.button>
  )
}

// Tailwind animation
@keyframes ripple {
  to {
    width: 200px;
    height: 200px;
    opacity: 0;
    transform: translate(-50%, -50%);
  }
}
```

#### C. Card Hover/Touch Effects
```typescript
// Glass card con efecto moderno en móvil
const GlassPanelMobile = ({ children, ...props }) => (
  <motion.div
    whileTap={{ scale: 0.98 }}
    transition={{ duration: 0.15 }}
    className={clsx(
      'glass-panel',
      'active:shadow-2xl active:shadow-primary/20', // Touch feedback
      'transition-shadow duration-150'
    )}
    {...props}
  >
    {children}
  </motion.div>
)
```

#### D. Modal Animations
```typescript
// Modal slide from bottom (móvil)
const modalVariants = {
  mobile: {
    hidden: { y: '100%', opacity: 0 },
    visible: { 
      y: 0, 
      opacity: 1,
      transition: { type: 'spring', damping: 25, stiffness: 300 }
    },
    exit: { 
      y: '100%', 
      opacity: 0,
      transition: { duration: 0.2 }
    }
  },
  desktop: {
    hidden: { scale: 0.9, opacity: 0 },
    visible: { scale: 1, opacity: 1 },
    exit: { scale: 0.9, opacity: 0 }
  }
}

<motion.div
  variants={modalVariants}
  initial="hidden"
  animate="visible"
  exit="exit"
  className={isMobile ? 'h-full' : 'max-h-[90vh]'}
>
  {/* Modal content */}
</motion.div>
```

---

### 2. 📱 Gestos Táctiles Intuitivos

#### A. Pull-to-Refresh
```typescript
// Pull to refresh en listas
import { usePullToRefresh } from '@/shared/hooks/usePullToRefresh'

const AlumnoClassesPage = () => {
  const { data, refetch } = useQuery('classes', fetchClasses)
  
  const { pullProgress, isPulling } = usePullToRefresh({
    onRefresh: refetch,
    threshold: 80
  })
  
  return (
    <div className="relative">
      {/* Pull indicator */}
      {isPulling && (
        <motion.div
          style={{ height: pullProgress }}
          className="absolute top-0 left-0 right-0 flex items-center justify-center"
        >
          <motion.div
            animate={{ rotate: pullProgress * 3.6 }}
            className="text-primary"
          >
            ↻
          </motion.div>
        </motion.div>
      )}
      
      {/* Content */}
      <div className="overflow-y-auto">
        {data?.map(clase => <ClassCard key={clase.id} {...clase} />)}
      </div>
    </div>
  )
}
```

#### B. Swipe Actions en Cards
```typescript
// Swipe para acciones rápidas (eliminar, editar, etc.)
import { useSwipeGesture } from '@/shared/hooks/useSwipeGesture'

const PaymentCard = ({ payment, onDelete, onEdit }) => {
  const { swipeX, handlers } = useSwipeGesture({
    onSwipeLeft: () => setShowActions(true),
    onSwipeRight: () => setShowActions(false),
    threshold: 50
  })
  
  return (
    <motion.div
      style={{ x: swipeX }}
      {...handlers}
      className="relative"
    >
      {/* Actions behind card */}
      <div className="absolute right-0 top-0 bottom-0 flex items-center gap-2 pr-4">
        <button onClick={onEdit} className="p-3 bg-blue-500 rounded-full">
          ✏️
        </button>
        <button onClick={onDelete} className="p-3 bg-red-500 rounded-full">
          🗑️
        </button>
      </div>
      
      {/* Card content */}
      <GlassPanel className="relative z-10">
        {/* ... */}
      </GlassPanel>
    </motion.div>
  )
}
```

#### C. Bottom Sheet con Swipe-to-Dismiss
```typescript
// Modal tipo bottom sheet con gesto de cierre
const BottomSheetModal = ({ isOpen, onClose, children }) => {
  const [dragY, setDragY] = useState(0)
  
  const handleDragEnd = (_, info) => {
    if (info.offset.y > 100) {
      onClose()
    } else {
      setDragY(0)
    }
  }
  
  return (
    <AnimatePresence>
      {isOpen && (
        <>
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            onClick={onClose}
            className="fixed inset-0 bg-black/50 z-50"
          />
          
          <motion.div
            drag="y"
            dragConstraints={{ top: 0, bottom: 0 }}
            dragElastic={0.2}
            onDragEnd={handleDragEnd}
            initial={{ y: '100%' }}
            animate={{ y: 0 }}
            exit={{ y: '100%' }}
            className="fixed bottom-0 left-0 right-0 z-50"
          >
            {/* Drag handle */}
            <div className="flex justify-center py-3">
              <div className="w-12 h-1 bg-gray-400 rounded-full" />
            </div>
            
            {/* Content */}
            <div className="glass-panel rounded-t-3xl min-h-[50vh] max-h-[90vh] overflow-y-auto">
              {children}
            </div>
          </motion.div>
        </>
      )}
    </AnimatePresence>
  )
}
```

---

### 3. 🎯 Feedback Visual Inmediato

#### A. Skeleton Loaders Elegantes
```typescript
// Skeleton con glassmorphism
const SkeletonCard = () => (
  <div className="glass-panel animate-pulse">
    <div className="h-6 bg-white/10 rounded w-3/4 mb-3" />
    <div className="h-4 bg-white/10 rounded w-1/2 mb-2" />
    <div className="h-4 bg-white/10 rounded w-full" />
  </div>
)

// Loading state con animación shimmer
@keyframes shimmer {
  0% { background-position: -468px 0; }
  100% { background-position: 468px 0; }
}

.skeleton-shimmer {
  background: linear-gradient(
    to right,
    rgba(255, 255, 255, 0.05) 0%,
    rgba(255, 255, 255, 0.15) 50%,
    rgba(255, 255, 255, 0.05) 100%
  );
  background-size: 800px 100%;
  animation: shimmer 2s infinite;
}
```

#### B. Toast Notifications Modernas
```typescript
// Toast con glassmorphism y auto-dismiss
const Toast = ({ message, type, onClose }) => (
  <motion.div
    initial={{ y: -100, opacity: 0 }}
    animate={{ y: 0, opacity: 1 }}
    exit={{ y: -100, opacity: 0 }}
    className={clsx(
      'glass-panel p-4 rounded-2xl shadow-2xl',
      'fixed top-4 left-4 right-4 z-[100]',
      type === 'success' && 'border-green-500',
      type === 'error' && 'border-red-500'
    )}
  >
    <div className="flex items-center gap-3">
      {type === 'success' ? '✅' : '❌'}
      <p className="text-white flex-1">{message}</p>
      <button onClick={onClose} className="text-white/70">✕</button>
    </div>
  </motion.div>
)
```

#### C. Progress Indicators
```typescript
// Progress bar elegante para uploads
const UploadProgress = ({ progress }) => (
  <div className="glass-panel p-4 rounded-xl">
    <div className="flex justify-between mb-2">
      <span className="text-white text-sm">Subiendo archivo...</span>
      <span className="text-primary text-sm font-bold">{progress}%</span>
    </div>
    <div className="h-2 bg-white/10 rounded-full overflow-hidden">
      <motion.div
        initial={{ width: 0 }}
        animate={{ width: `${progress}%` }}
        transition={{ duration: 0.3 }}
        className="h-full bg-gradient-to-r from-primary to-accent rounded-full"
      />
    </div>
  </div>
)
```

---

### 4. 🎨 Efectos Visuales Premium

#### A. Gradient Backgrounds Animados
```typescript
// Background animado para headers móviles
const AnimatedHeader = ({ title }) => (
  <div className="relative overflow-hidden rounded-2xl p-6 mb-4">
    {/* Animated gradient */}
    <motion.div
      animate={{
        backgroundPosition: ['0% 0%', '100% 100%'],
      }}
      transition={{
        duration: 10,
        repeat: Infinity,
        repeatType: 'reverse'
      }}
      className="absolute inset-0"
      style={{
        background: 'linear-gradient(45deg, #FF6B9D, #4A90E2, #FFD93D)',
        backgroundSize: '200% 200%',
        opacity: 0.2
      }}
    />
    
    {/* Glassmorphism overlay */}
    <div className="absolute inset-0 glass-panel" />
    
    {/* Content */}
    <h1 className="relative z-10 text-2xl font-bold text-white">{title}</h1>
  </div>
)
```

#### B. Parallax Scroll Effects
```typescript
// Efecto parallax sutil en hero sections
const HeroSection = ({ children }) => {
  const { scrollY } = useScroll()
  const y = useTransform(scrollY, [0, 300], [0, -50])
  
  return (
    <motion.div style={{ y }} className="relative">
      {children}
    </motion.div>
  )
}
```

#### C. Blur on Scroll
```typescript
// Header que se hace más glass al scroll
const StickyHeader = ({ title }) => {
  const { scrollY } = useScroll()
  const backdropBlur = useTransform(scrollY, [0, 100], [10, 20])
  const opacity = useTransform(scrollY, [0, 100], [0.1, 0.3])
  
  return (
    <motion.header
      style={{
        backdropFilter: useMotionTemplate`blur(${backdropBlur}px)`,
        backgroundColor: useMotionTemplate`rgba(255, 255, 255, ${opacity})`
      }}
      className="sticky top-0 z-40 border-b border-white/10"
    >
      <h1 className="text-xl font-bold text-white p-4">{title}</h1>
    </motion.header>
  )
}
```

---

### 5. 📊 Visualizaciones de Datos Atractivas

#### A. Charts Animados
```typescript
// Gráficas con entrada animada
const AnimatedChart = ({ data }) => (
  <motion.div
    initial={{ opacity: 0, scale: 0.9 }}
    animate={{ opacity: 1, scale: 1 }}
    transition={{ duration: 0.5 }}
  >
    <ResponsiveContainer width="100%" height={300}>
      <LineChart data={data}>
        <Line
          type="monotone"
          dataKey="value"
          stroke="#FF6B9D"
          strokeWidth={3}
          animationDuration={1000}
          animationEasing="ease-in-out"
        />
      </LineChart>
    </ResponsiveContainer>
  </motion.div>
)
```

#### B. Stats Cards con Counter Animation
```typescript
// Números que cuentan hacia arriba
const AnimatedStat = ({ value, label }) => {
  const [count, setCount] = useState(0)
  
  useEffect(() => {
    const timer = setInterval(() => {
      setCount(prev => {
        const next = prev + Math.ceil(value / 30)
        return next >= value ? value : next
      })
    }, 30)
    
    return () => clearInterval(timer)
  }, [value])
  
  return (
    <motion.div
      initial={{ scale: 0.8, opacity: 0 }}
      animate={{ scale: 1, opacity: 1 }}
      className="glass-panel p-6 rounded-xl text-center"
    >
      <motion.div
        key={count}
        initial={{ y: 20, opacity: 0 }}
        animate={{ y: 0, opacity: 1 }}
        className="text-4xl font-bold text-primary mb-2"
      >
        {count}
      </motion.div>
      <p className="text-white/70 text-sm">{label}</p>
    </motion.div>
  )
}
```

---

## 🛠️ IMPLEMENTACIÓN POR FASE

### Fase 1: Alumno (Días 1-7) - Elementos Visuales
```typescript
// Día 5-6: Agregar micro-interacciones
- ✨ ButtonMobile con ripple effect
- ✨ Card touch feedback (scale on tap)
- ✨ Pull-to-refresh en StudentClassesPage
- ✨ Skeleton loaders en todas las páginas
- ✨ Toast notifications modernas

// Día 7: Polish visual
- ✨ Page transitions entre vistas
- ✨ Animated stats en dashboard
- ✨ Smooth scroll behavior
```

### Fase 2: Profesor (Días 8-15) - Gestos
```typescript
// Día 13-14: Gestos táctiles
- ✨ Swipe actions en lista de clases
- ✨ Bottom sheet para ConfirmarAsistenciaModal
- ✨ Swipe-to-dismiss en modales
- ✨ Haptic feedback en botones críticos

// Día 15: Polish
- ✨ Parallax en hero sections
- ✨ Blur on scroll en headers
```

### Fase 3: Admin (Días 16-25) - Efectos Premium
```typescript
// Día 23-24: Efectos visuales
- ✨ Animated gradient backgrounds
- ✨ Charts con animaciones
- ✨ Progress bars elegantes (uploads)
- ✨ Complex modal animations (wizards)

// Día 25: Polish final
- ✨ Stagger animations en listas
- ✨ Exit animations en eliminaciones
- ✨ Loading states premium
```

---

## 📦 LIBRERÍAS REQUERIDAS

```json
{
  "dependencies": {
    "framer-motion": "^11.0.0",        // Animaciones React
    "react-use-gesture": "^9.1.3",     // Gestos táctiles
    "@tanstack/react-query": "^5.0.0"  // Loading states (ya instalado)
  }
}
```

---

## 🎯 CHECKLIST DE CALIDAD VISUAL

### Por Componente
- [ ] Animación de entrada (fade, slide, scale)
- [ ] Touch feedback (scale, ripple)
- [ ] Loading state con skeleton
- [ ] Error state visual
- [ ] Empty state ilustrado
- [ ] Transición suave entre estados

### Por Página
- [ ] Page transition al entrar/salir
- [ ] Pull-to-refresh funcional
- [ ] Smooth scroll
- [ ] Sticky header con blur
- [ ] Bottom navigation con active state

### Por Modal
- [ ] Slide from bottom (mobile)
- [ ] Swipe-to-dismiss
- [ ] Overlay fade
- [ ] Focus trap
- [ ] Teclado no cubre inputs

---

## 💡 MEJORES PRÁCTICAS

### Performance
```typescript
// ✅ USAR: Animaciones con GPU
transform, opacity // ← Estas no causan reflow

// ❌ EVITAR: Propiedades que causan reflow
width, height, top, left // ← Lentas en móvil
```

### Accesibilidad
```typescript
// Reducir animaciones si el usuario lo prefiere
const prefersReducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches

const variants = {
  hidden: { opacity: prefersReducedMotion ? 1 : 0 },
  visible: { opacity: 1 }
}
```

### Tamaño de Bundle
```typescript
// Lazy load Framer Motion en componentes pesados
const MotionDiv = lazy(() => 
  import('framer-motion').then(mod => ({ default: mod.motion.div }))
)
```

---

## 🎬 RESULTADO ESPERADO

### Experiencia Móvil Moderna
- **Fluida:** 60fps en animaciones
- **Intuitiva:** Gestos naturales (swipe, pull, tap)
- **Elegante:** Glassmorphism + transiciones suaves
- **Rápida:** Feedback visual <100ms
- **Coherente:** Conserva identidad visual actual

### Sin Comprometer Desktop
- Desktop mantiene su experiencia actual 100%
- Animaciones solo en móvil (condicional con `isMobile`)
- Bundle size optimizado (code splitting)

---

**Próximos Pasos:**
1. Instalar `framer-motion` y `react-use-gesture`
2. Crear hooks de gestos (`usePullToRefresh`, `useSwipeGesture`)
3. Implementar `ButtonMobile` con ripple effect
4. Agregar page transitions en Router
5. Integrar skeleton loaders en queries

**Documentos Relacionados:**
- [PLAN-RESPONSIVE-MOBILE.md](./PLAN-RESPONSIVE-MOBILE.md) - Plan maestro
- [RESPONSIVE-MODALES.md](./RESPONSIVE-MODALES.md) - Especificación de modales
