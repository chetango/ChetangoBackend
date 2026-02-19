# 🔐 Guía de Autenticación Frontend - Microsoft Entra ID (CIAM)

**Documento para el equipo Frontend**  
**Última actualización:** Enero 11, 2026  
**Backend API:** https://localhost:7194

---

## 📋 Tabla de Contenidos

1. [Configuración de Microsoft Entra ID](#configuración-de-microsoft-entra-id)
2. [Flujo de Autenticación OAuth 2.0](#flujo-de-autenticación-oauth-20)
3. [Implementación en Frontend](#implementación-en-frontend)
4. [Estructura del Token JWT](#estructura-del-token-jwt)
5. [Envío del Token en Peticiones](#envío-del-token-en-peticiones)
6. [Manejo de Roles y Permisos](#manejo-de-roles-y-permisos)
7. [Usuarios de Prueba](#usuarios-de-prueba)
8. [Ejemplos de Código](#ejemplos-de-código)
9. [Troubleshooting](#troubleshooting)

---

## 🔧 Configuración de Microsoft Entra ID

### **Información del Tenant**

```json
{
  "tenantId": "8a57ec5a-e2e3-44ad-9494-77fbc7467251",
  "domain": "chetangoprueba.onmicrosoft.com",
  "clientId": "d35c1d4d-9ddc-4a8b-bb89-1964b37ff573",
  "authority": "https://chetangoprueba.ciamlogin.com/8a57ec5a-e2e3-44ad-9494-77fbc7467251"
}
```

### **Endpoints OAuth 2.0**

| Endpoint | URL |
|----------|-----|
| **Authorization** | `https://chetangoprueba.ciamlogin.com/8a57ec5a-e2e3-44ad-9494-77fbc7467251/oauth2/v2.0/authorize` |
| **Token** | `https://chetangoprueba.ciamlogin.com/8a57ec5a-e2e3-44ad-9494-77fbc7467251/oauth2/v2.0/token` |
| **Logout** | `https://chetangoprueba.ciamlogin.com/8a57ec5a-e2e3-44ad-9494-77fbc7467251/oauth2/v2.0/logout` |

### **Scopes Requeridos**

```
openid profile email api://d35c1d4d-9ddc-4a8b-bb89-1964b37ff573/chetango.api
```

**Desglose:**
- `openid`: ID token con información del usuario
- `profile`: Claims básicos del perfil (nombre, etc.)
- `email`: Email del usuario
- `api://d35c1d4d-9ddc-4a8b-bb89-1964b37ff573/chetango.api`: Scope personalizado para acceso a la API

### **Redirect URIs Registradas**

Para desarrollo local:
```
http://localhost:3000/auth/callback
http://localhost:5173/auth/callback  (Vite)
```

⚠️ **IMPORTANTE:** Si tu frontend usa otro puerto, debes solicitarlo para agregarlo en Entra ID.

---

## 🔄 Flujo de Autenticación OAuth 2.0

Chetango usa **Authorization Code Flow con PKCE** (recomendado para aplicaciones SPA).

### **Diagrama del Flujo**

```
┌─────────┐                                  ┌──────────────┐                      ┌─────────┐
│Frontend │                                  │  Entra CIAM  │                      │ Backend │
└────┬────┘                                  └──────┬───────┘                      └────┬────┘
     │                                              │                                    │
     │ 1. Redirigir a /authorize                   │                                    │
     │────────────────────────────────────────────>│                                    │
     │    + code_challenge (PKCE)                  │                                    │
     │    + redirect_uri                           │                                    │
     │                                              │                                    │
     │ 2. Usuario ingresa credenciales             │                                    │
     │<─────────────────────────────────────────── │                                    │
     │                                              │                                    │
     │ 3. Redirigir con código                     │                                    │
     │<─────────────────────────────────────────── │                                    │
     │    ?code=ABC123&state=xyz                   │                                    │
     │                                              │                                    │
     │ 4. Intercambiar código por token            │                                    │
     │────────────────────────────────────────────>│                                    │
     │    + code_verifier (PKCE)                   │                                    │
     │                                              │                                    │
     │ 5. Retornar access_token + id_token         │                                    │
     │<─────────────────────────────────────────── │                                    │
     │                                              │                                    │
     │ 6. Llamar API con Bearer token              │                                    │
     │────────────────────────────────────────────────────────────────────────────────>│
     │    Authorization: Bearer {access_token}     │                                    │
     │                                              │                                    │
     │ 7. Respuesta con datos                      │                                    │
     │<────────────────────────────────────────────────────────────────────────────────│
     │                                              │                                    │
```

### **Pasos Detallados**

#### **Paso 1: Generar Code Challenge (PKCE)**

```javascript
// Generar code_verifier (random string de 43-128 caracteres)
const codeVerifier = generateRandomString(128);

// Generar code_challenge (SHA256 del verifier, base64url encoded)
const codeChallenge = base64UrlEncode(sha256(codeVerifier));

// Guardar code_verifier en sessionStorage para paso 4
sessionStorage.setItem('code_verifier', codeVerifier);
```

#### **Paso 2: Redirigir a Authorize Endpoint**

```javascript
const params = new URLSearchParams({
  client_id: 'd35c1d4d-9ddc-4a8b-bb89-1964b37ff573',
  response_type: 'code',
  redirect_uri: 'http://localhost:3000/auth/callback',
  response_mode: 'query',
  scope: 'openid profile email api://d35c1d4d-9ddc-4a8b-bb89-1964b37ff573/chetango.api',
  state: generateRandomString(32), // Anti-CSRF
  code_challenge: codeChallenge,
  code_challenge_method: 'S256'
});

window.location.href = `https://chetangoprueba.ciamlogin.com/8a57ec5a-e2e3-44ad-9494-77fbc7467251/oauth2/v2.0/authorize?${params}`;
```

#### **Paso 3: Recibir Authorization Code**

Después del login, Entra ID redirige a tu `redirect_uri`:

```
http://localhost:3000/auth/callback?code=ABC123...&state=xyz123
```

#### **Paso 4: Intercambiar Código por Token**

```javascript
const codeVerifier = sessionStorage.getItem('code_verifier');

const response = await fetch('https://chetangoprueba.ciamlogin.com/8a57ec5a-e2e3-44ad-9494-77fbc7467251/oauth2/v2.0/token', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/x-www-form-urlencoded'
  },
  body: new URLSearchParams({
    client_id: 'd35c1d4d-9ddc-4a8b-bb89-1964b37ff573',
    grant_type: 'authorization_code',
    code: authorizationCode,
    redirect_uri: 'http://localhost:3000/auth/callback',
    code_verifier: codeVerifier
  })
});

const tokens = await response.json();
// tokens.access_token -> para llamar a la API
// tokens.id_token -> información del usuario
// tokens.refresh_token -> renovar tokens (si se solicitó offline_access)
```

#### **Paso 5: Guardar Tokens**

```javascript
// Guardar en sessionStorage o localStorage (sessionStorage más seguro)
sessionStorage.setItem('access_token', tokens.access_token);
sessionStorage.setItem('id_token', tokens.id_token);
sessionStorage.setItem('expires_at', Date.now() + tokens.expires_in * 1000);
```

---

## 🎯 Implementación en Frontend

### **Opción 1: Usar MSAL.js (Recomendado)**

Microsoft Authentication Library simplifica el flujo OAuth.

**Instalación:**
```bash
npm install @azure/msal-browser
```

**Configuración:**
```javascript
// authConfig.js
import { PublicClientApplication } from '@azure/msal-browser';

const msalConfig = {
  auth: {
    clientId: 'd35c1d4d-9ddc-4a8b-bb89-1964b37ff573',
    authority: 'https://chetangoprueba.ciamlogin.com/8a57ec5a-e2e3-44ad-9494-77fbc7467251',
    redirectUri: 'http://localhost:3000/auth/callback',
    postLogoutRedirectUri: 'http://localhost:3000'
  },
  cache: {
    cacheLocation: 'sessionStorage',
    storeAuthStateInCookie: false
  }
};

const loginRequest = {
  scopes: [
    'openid',
    'profile',
    'email',
    'api://d35c1d4d-9ddc-4a8b-bb89-1964b37ff573/chetango.api'
  ]
};

export const msalInstance = new PublicClientApplication(msalConfig);
export { loginRequest };
```

**Login:**
```javascript
// Login.jsx
import { msalInstance, loginRequest } from './authConfig';

async function handleLogin() {
  try {
    await msalInstance.loginRedirect(loginRequest);
  } catch (error) {
    console.error('Login error:', error);
  }
}
```

**Obtener Token:**
```javascript
// apiClient.js
import { msalInstance, loginRequest } from './authConfig';

async function getAccessToken() {
  const accounts = msalInstance.getAllAccounts();
  if (accounts.length === 0) {
    throw new Error('No hay usuario autenticado');
  }

  const request = {
    ...loginRequest,
    account: accounts[0]
  };

  try {
    const response = await msalInstance.acquireTokenSilent(request);
    return response.accessToken;
  } catch (error) {
    // Si falla silent, hacer interactive
    const response = await msalInstance.acquireTokenPopup(request);
    return response.accessToken;
  }
}

export { getAccessToken };
```

### **Opción 2: Implementación Manual**

Si prefieres no usar MSAL, implementa el flujo PKCE manualmente (código mostrado en sección anterior).

---

## 🔑 Estructura del Token JWT

### **Access Token Decodificado**

El `access_token` es un JWT con esta estructura:

```json
{
  "aud": "api://d35c1d4d-9ddc-4a8b-bb89-1964b37ff573",
  "iss": "https://chetangoprueba.ciamlogin.com/8a57ec5a-e2e3-44ad-9494-77fbc7467251/v2.0",
  "iat": 1736640000,
  "nbf": 1736640000,
  "exp": 1736643600,
  "name": "Jorge Padilla",
  "oid": "8472bc4a-f83e-4a84-ab5b-abd8c7d3e2ab",
  "preferred_username": "Jorgepadilla@chetangoprueba.onmicrosoft.com",
  "roles": [
    "profesor"
  ],
  "scp": "chetango.api",
  "sub": "8472bc4a-f83e-4a84-ab5b-abd8c7d3e2ab",
  "tid": "8a57ec5a-e2e3-44ad-9494-77fbc7467251",
  "ver": "2.0"
}
```

### **Claims Importantes**

| Claim | Descripción | Uso |
|-------|-------------|-----|
| `oid` | Object ID del usuario en Entra ID | Identificador único del usuario |
| `preferred_username` | Email del usuario | Mostrar en UI, ownership validation |
| `roles` | Array de roles asignados | Mostrar/ocultar componentes según rol |
| `name` | Nombre completo del usuario | Mostrar en UI |
| `exp` | Fecha de expiración (Unix timestamp) | Validar si token está vigente |
| `scp` | Scopes otorgados | Validar permisos |

### **Roles Disponibles**

- `admin`: Administrador (acceso total)
- `profesor`: Profesor (gestiona sus clases y asistencias)
- `alumno`: Alumno (consulta sus datos)

---

## 📤 Envío del Token en Peticiones

### **Header de Autorización**

Todas las peticiones a la API deben incluir el token en el header `Authorization`:

```
Authorization: Bearer {access_token}
```

### **Ejemplo con Fetch**

```javascript
const accessToken = await getAccessToken();

const response = await fetch('https://localhost:7194/api/clases', {
  method: 'GET',
  headers: {
    'Authorization': `Bearer ${accessToken}`,
    'Content-Type': 'application/json'
  }
});

const clases = await response.json();
```

### **Ejemplo con Axios**

```javascript
import axios from 'axios';

// Crear instancia con interceptor
const apiClient = axios.create({
  baseURL: 'https://localhost:7194/api'
});

// Interceptor para agregar token automáticamente
apiClient.interceptors.request.use(
  async (config) => {
    const accessToken = await getAccessToken();
    config.headers.Authorization = `Bearer ${accessToken}`;
    return config;
  },
  (error) => Promise.reject(error)
);

// Uso
const { data } = await apiClient.get('/clases');
```

### **Manejo de Token Expirado (401)**

```javascript
apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response?.status === 401) {
      // Token expirado, renovar
      try {
        const newToken = await msalInstance.acquireTokenSilent(loginRequest);
        sessionStorage.setItem('access_token', newToken.accessToken);
        
        // Reintentar petición original
        error.config.headers.Authorization = `Bearer ${newToken.accessToken}`;
        return apiClient.request(error.config);
      } catch {
        // Forzar re-login
        await msalInstance.loginRedirect(loginRequest);
      }
    }
    return Promise.reject(error);
  }
);
```

---

## 👥 Manejo de Roles y Permisos

### **Decodificar Token en Frontend**

```javascript
// utils/jwtDecode.js
function parseJwt(token) {
  const base64Url = token.split('.')[1];
  const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
  const jsonPayload = decodeURIComponent(
    atob(base64)
      .split('')
      .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
      .join('')
  );
  return JSON.parse(jsonPayload);
}

export function getUserRoles(token) {
  const decoded = parseJwt(token);
  return decoded.roles || [];
}

export function hasRole(token, role) {
  const roles = getUserRoles(token);
  return roles.includes(role);
}
```

### **Mostrar/Ocultar Componentes por Rol**

```javascript
// components/ProtectedComponent.jsx
import { getUserRoles } from '../utils/jwtDecode';

function ProtectedComponent({ allowedRoles, children }) {
  const token = sessionStorage.getItem('access_token');
  const userRoles = getUserRoles(token);
  
  const hasPermission = allowedRoles.some(role => userRoles.includes(role));
  
  if (!hasPermission) {
    return null; // O componente de acceso denegado
  }
  
  return <>{children}</>;
}

// Uso
<ProtectedComponent allowedRoles={['admin', 'profesor']}>
  <button>Crear Clase</button>
</ProtectedComponent>
```

### **Routing por Rol (React Router)**

```javascript
// routes/ProtectedRoute.jsx
import { Navigate } from 'react-router-dom';
import { hasRole } from '../utils/jwtDecode';

function ProtectedRoute({ children, allowedRoles }) {
  const token = sessionStorage.getItem('access_token');
  
  if (!token) {
    return <Navigate to="/login" />;
  }
  
  const hasPermission = allowedRoles.some(role => hasRole(token, role));
  
  if (!hasPermission) {
    return <Navigate to="/unauthorized" />;
  }
  
  return children;
}

// Uso en App.jsx
<Route 
  path="/admin" 
  element={
    <ProtectedRoute allowedRoles={['admin']}>
      <AdminDashboard />
    </ProtectedRoute>
  } 
/>
```

---

## 👤 Usuarios de Prueba

### **Credenciales de Usuarios Reales**

| Rol | Email | Contraseña | Sede | IdUsuario | Datos Adicionales |
|-----|-------|------------|------|-----------|-------------------|
| **Admin Medellín** | Chetango@chetangoprueba.onmicrosoft.com | Chet4ngo20# | Medellín (1) | b91e51b9-4094-441e-a5b6-062a846b3868 | - |
| **Admin Manizales** | chetango.manizales@chetangoprueba.onmicrosoft.com | Maniz4les20# | Manizales (2) | c91e51b9-4094-441e-a5b6-062a846b3869 | - |
| **Profesor** | Jorgepadilla@chetangoprueba.onmicrosoft.com | Jorge2026 | Medellín (1) | 8472BC4A-F83E-4A84-AB5B-ABD8C7D3E2AB | IdProfesor: 8f6e460d-328d-4a40-89e3-b8effa76829c |
| **Alumno** | JuanDavid@chetangoprueba.onmicrosoft.com | Juaj0rge20# | Medellín (1) | 71462106-9863-4fd0-b13d-9878ed231aa6 | IdAlumno: 295093d5-b36f-4737-b68a-ab40ca871b2e |

### **Usar en Pruebas**

```javascript
// Para desarrollo, puedes hardcodear temporalmente
const testUsers = {
  adminMedellin: {
    email: 'Chetango@chetangoprueba.onmicrosoft.com',
    password: 'Chet4ngo20#',
    sede: 1
  },
  adminManizales: {
    email: 'chetango.manizales@chetangoprueba.onmicrosoft.com',
    password: 'Maniz4les20#',
    sede: 2
  },
  profesor: {
    email: 'Jorgepadilla@chetangoprueba.onmicrosoft.com',
    password: 'Jorge2026'
  },
  alumno: {
    email: 'JuanDavid@chetangoprueba.onmicrosoft.com',
    password: 'Juaj0rge20#'
  }
};
```

---

## 💻 Ejemplos de Código

### **Ejemplo Completo: React + MSAL**

```javascript
// App.jsx
import { useEffect, useState } from 'react';
import { msalInstance, loginRequest } from './authConfig';
import { MsalProvider, useMsal, useIsAuthenticated } from '@azure/msal-react';

function App() {
  return (
    <MsalProvider instance={msalInstance}>
      <AppContent />
    </MsalProvider>
  );
}

function AppContent() {
  const { instance, accounts } = useMsal();
  const isAuthenticated = useIsAuthenticated();

  const handleLogin = () => {
    instance.loginRedirect(loginRequest);
  };

  const handleLogout = () => {
    instance.logoutRedirect();
  };

  if (!isAuthenticated) {
    return (
      <div>
        <h1>Chetango App</h1>
        <button onClick={handleLogin}>Iniciar Sesión</button>
      </div>
    );
  }

  return (
    <div>
      <h1>Bienvenido, {accounts[0]?.name}</h1>
      <button onClick={handleLogout}>Cerrar Sesión</button>
      <ClasesList />
    </div>
  );
}

function ClasesList() {
  const { instance, accounts } = useMsal();
  const [clases, setClases] = useState([]);

  useEffect(() => {
    async function fetchClases() {
      const request = {
        ...loginRequest,
        account: accounts[0]
      };

      try {
        const response = await instance.acquireTokenSilent(request);
        
        const apiResponse = await fetch('https://localhost:7194/api/clases', {
          headers: {
            'Authorization': `Bearer ${response.accessToken}`
          }
        });
        
        const data = await apiResponse.json();
        setClases(data);
      } catch (error) {
        console.error('Error fetching clases:', error);
      }
    }

    fetchClases();
  }, [instance, accounts]);

  return (
    <ul>
      {clases.map(clase => (
        <li key={clase.idClase}>{clase.tipoClase} - {clase.fecha}</li>
      ))}
    </ul>
  );
}
```

### **Ejemplo: Vue 3 + MSAL**

```javascript
// main.js
import { createApp } from 'vue';
import { msalPlugin } from '@azure/msal-vue';
import App from './App.vue';

const msalConfig = {
  auth: {
    clientId: 'd35c1d4d-9ddc-4a8b-bb89-1964b37ff573',
    authority: 'https://chetangoprueba.ciamlogin.com/8a57ec5a-e2e3-44ad-9494-77fbc7467251',
    redirectUri: 'http://localhost:3000'
  }
};

const app = createApp(App);
app.use(msalPlugin, msalConfig);
app.mount('#app');
```

```vue
<!-- App.vue -->
<template>
  <div>
    <button v-if="!isAuthenticated" @click="login">Login</button>
    <button v-else @click="logout">Logout</button>
    
    <div v-if="isAuthenticated">
      <h1>Hola {{ userName }}</h1>
      <ClasesList />
    </div>
  </div>
</template>

<script setup>
import { useMsal } from '@azure/msal-vue';
import { computed } from 'vue';

const { instance, accounts } = useMsal();

const isAuthenticated = computed(() => accounts.value.length > 0);
const userName = computed(() => accounts.value[0]?.name);

const login = () => {
  instance.loginRedirect({
    scopes: ['openid', 'profile', 'email', 'api://d35c1d4d-9ddc-4a8b-bb89-1964b37ff573/chetango.api']
  });
};

const logout = () => {
  instance.logoutRedirect();
};
</script>
```

---

## 🔍 Troubleshooting

### **Problema: CORS Error**

```
Access to fetch at 'https://localhost:7194/api/clases' from origin 'http://localhost:3000' 
has been blocked by CORS policy
```

**Solución:** Solicita al equipo Backend agregar tu origen a la configuración CORS.

Orígenes actuales permitidos:
- `http://localhost:5129`
- `https://localhost:7194`
- `https://qa.chetango.local`

### **Problema: Invalid Redirect URI**

```
AADSTS50011: The redirect URI 'http://localhost:3000/callback' 
specified in the request does not match
```

**Solución:** Solicita al equipo Backend registrar tu redirect URI en Entra ID.

### **Problema: Token No Contiene Roles**

**Causa:** El usuario no tiene roles asignados en Entra ID.

**Solución:** Solicita al admin asignar el rol correspondiente:
1. Ir a Azure Portal → Entra ID → Enterprise Applications
2. Buscar "Chetango API"
3. Usuarios y Grupos → Asignar rol al usuario

### **Problema: 401 Unauthorized**

**Verificar:**
1. ¿El token está en el header? `Authorization: Bearer {token}`
2. ¿El token expiró? Verificar claim `exp`
3. ¿El endpoint requiere un rol específico? Verificar claim `roles`

### **Problema: 403 Forbidden**

**Causa:** Usuario autenticado pero sin permisos para el recurso.

**Ejemplo:** Alumno intentando acceder a endpoint AdminOnly.

**Solución:** Verificar roles requeridos por el endpoint en documentación API.

---

## 📚 Recursos Adicionales

- **Documentación MSAL.js:** https://github.com/AzureAD/microsoft-authentication-library-for-js
- **Documentación API Backend:** Ver archivos en `/docs`:
  - `API-CONTRACT-CLASES.md`
  - `API-CONTRACT-ASISTENCIAS.md`
  - `API-CONTRACT-PAQUETES.md`
- **OAuth 2.0 PKCE Flow:** https://oauth.net/2/pkce/
- **JWT.io (decodificar tokens):** https://jwt.io

---

## 📞 Contacto

Para solicitar:
- Agregar nuevos redirect URIs
- Crear usuarios de prueba adicionales
- Asignar roles a usuarios
- Modificar configuración CORS

Contactar al equipo Backend con los detalles necesarios.

---

**Documento generado:** Enero 11, 2026  
**Versión:** 1.0  
**Mantenedor:** Equipo Backend Chetango
