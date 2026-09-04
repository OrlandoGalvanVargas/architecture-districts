# FacilityOS Frontend

Frontend moderno basado en React para **FacilityOS**, una plataforma empresarial para la gestión de instalaciones educativas: distritos, escuelas, usuarios, beacons y miembros del cuerpo docente. La aplicación sigue una arquitectura orientada a funcionalidades (feature-oriented), se integra sin problemas con la API de FacilityOS e incluye caché sin conexión y un modo simulado (mock) para pruebas locales.

---

## Tabla de Contenidos

- [Stack Tecnológico](#stack-tecnológico)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Funcionalidades](#funcionalidades)
- [Primeros Pasos](#primeros-pasos)
  - [Requisitos Previos](#requisitos-previos)
  - [Instalación](#instalación)
  - [Ejecutar la Aplicación](#ejecutar-la-aplicación)
  - [Compilación para Producción](#compilación-para-producción)
- [Variables de Entorno](#variables-de-entorno)
- [Arquitectura](#arquitectura)
  - [Autenticación y Autorización](#autenticación-y-autorización)
  - [Gestión de Estado](#gestión-de-estado)
  - [Capa de API](#capa-de-api)
  - [Enrutamiento](#enrutamiento)
  - [Manejo de Errores](#manejo-de-errores)
  - [Caché Sin Conexión](#caché-sin-conexión)
  - [Modo Simulado (Mock)](#modo-simulado-mock)
- [Roles y Permisos de Usuario](#roles-y-permisos-de-usuario)
- [UI/UX](#uiux)
- [Pruebas](#pruebas)
- [Calidad de Código](#calidad-de-código)
- [Despliegue](#despliegue)
- [Licencia](#licencia)

---

## Stack Tecnológico

- **React 19** con **Vite 7**
- **JavaScript (JSX)**
- **Ant Design 6** – Librería de componentes UI
- **TanStack Query 5** – Gestión y caché del estado del servidor
- **Zustand 5** – Gestión del estado del cliente
- **React Router 7** – Enrutamiento
- **Axios** – Cliente HTTP
- **MSW** – Mock Service Worker para simulación local
- **idb-keyval** – Wrapper de IndexedDB para persistencia
- **Vitest** + **Testing Library** – Pruebas
- **ESLint** + **Prettier** – Calidad de código
- **Docker** + **Nginx** – Despliegue en contenedores

---

## Estructura del Proyecto

```text
client/
├── public/                     # Assets estáticos y service worker
│   └── mockServiceWorker.js # Service worker MSW personalizado
├── src/
│   ├── api/                    # Cliente de API y definiciones de endpoints
│   │   ├── client.js           # Instancia de Axios con interceptores
│   │   ├── endpoints/          # Funciones de API por módulo
│   │   └── queryKeys.js        # Claves de TanStack Query
│   ├── assets/                 # Imágenes, fuentes, etc.
│   ├── components/             # Componentes UI reutilizables
│   │   ├── common/             # ConfirmDialog, EmptyState, LoadingSpinner, ErrorMessage, ErrorBoundary, QueryStateHandler, DistrictSelect, SchoolSelect, BeaconSelect, ProfileModal
│   │   └── Layout/             # MainLayout, PageHeader
│   ├── config/                 # Configuraciones globales
│   │   ├── queryClient.js      # Configuración de TanStack Query + persistencia
│   │   └── theme.js            # Tokens de tema de Ant Design
│   ├── contexts/               # Contextos de React
│   │   ├── AuthContext.jsx     # Estado y métodos de autenticación
│   │   └── Notification.jsx    # Proveedor de notificaciones/toast
│   ├── features/               # Módulos de funcionalidades (auth, districts, schools, users, beacons, faculties)
│   │   ├── auth/
│   │   ├── districts/
│   │   ├── schools/
│   │   ├── users/
│   │   ├── beacons/
│   │   └── faculties/
│   │       ├── components/     # Componentes específicos de la funcionalidad
│   │       ├── hooks/          # Hooks de TanStack Query para esa funcionalidad
│   │       ├── pages/          # Componentes de página
│   │       └── View.jsx        # Definiciones de rutas anidadas
│   ├── hooks/                  # Hooks personalizados compartidos (usePermission, useAppNavigation, useOnlineStatus)
│   ├── mocks/                  # Configuración de Mock Service Worker y datos semilla
│   ├── pages/                  # Páginas globales (Home, Forbidden, NotFound, ServerError)
│   ├── router/                 # Configuración de rutas y ProtectedRoute
│   ├── services/               # Servicio de logging
│   ├── store/                  # Stores de Zustand (ui.store)
│   ├── test/                   # Configuración y utilidades de pruebas
│   ├── utils/                  # Helpers (tokenManager, errorHandler, permissions, etc.)
│   ├── App.jsx
│   ├── main.jsx
│   └── index.css
├── .env.example                # Ejemplo de variables de entorno
├── Dockerfile
├── nginx.conf
├── docker-compose.production.yml
├── package.json
└── vite.config.js
```
---

## Funcionalidades

- **Autenticación** – Inicio de sesión, renovación de token, cierre de sesión, perfil del usuario actual.
- **Distritos** – CRUD completo con permisos basados en roles.
- **Escuelas** – CRUD completo con asignación de distritos y listados filtrados.
- **Usuarios** – CRUD completo con restricciones dinámicas de rol y entidad.
- **Beacons** – CRUD completo con filtros por tipo/estado y asignación opcional a distrito/escuela/facultad.
- **Facultades** – CRUD completo con asignación a distrito o escuela, beacon opcional y restricciones de rol.
- **Dashboard** – Página de inicio profesional con tarjetas de módulos según los permisos del usuario.
- **Caché Sin Conexión** – Persistencia de la caché de consultas en IndexedDB, mostrando datos en caché cuando no hay conexión.
- **Modo Simulado (Mock)** – Ejecuta toda la aplicación sin un backend usando Mock Service Worker y datos locales.
- **Modo Oscuro** – Alternancia entre temas claro y oscuro.

---

## Primeros Pasos

### Requisitos Previos

- Node.js >= 20
- npm >= 10

### Instalación

```bash
git clone https://github.com/OrlandoGalvanVargas/architecture-districts.git
cd client
npm install
```

### Ejecutar la Aplicación

Crea un archivo `.env` basado en `.env.example`. Para desarrollo local con el backend real, configura `VITE_ENABLE_MOCK=false`. Luego:

```bash
npm run dev
```

La aplicación estará disponible en `http://localhost:3000`.

### Compilación para Producción

```bash
npm run build
```

El paquete de producción quedará en la carpeta `dist/`.

---

## Variables de Entorno

| Variable               | Descripción                                    | Valor por defecto        |
|------------------------|-------------------------------------------------|---------------------------|
| `VITE_API_URL`         | URL base para las peticiones a la API           | `http://localhost:5000`  |
| `VITE_API_TIMEOUT`     | Tiempo de espera de Axios (ms)                  | `30000`                   |
| `VITE_ENABLE_LOGGING`  | Habilitar el logging en consola                | `false`                    |
| `VITE_ENABLE_MOCK`     | Habilitar Mock Service Worker para modo sin conexión | `false`               |
| `VITE_APP_ENV`         | Entorno (`development`, `production`)           | `development`             |
| `VITE_APP_VERSION`     | Buster de caché opcional para persistencia de consultas | —                  |

---

## Arquitectura

### Autenticación y Autorización

- **Access Token** almacenado en `localStorage` mediante `tokenManager`.
- **Refresh Token** guardado en una cookie HTTP-only; el interceptor de Axios lo renueva automáticamente ante un 401.
- `AuthContext` provee `user`, `isAuthenticated`, `login`, `logout`, `hasRole`, `hasPermission`.
- `ProtectedRoute` protege las rutas y verifica permisos usando `checkPermission`.
- El backend es la autoridad final; los permisos del frontend son solo para efectos de UX.

### Gestión de Estado

- **TanStack Query** gestiona todo el estado del servidor, con persistencia de caché en IndexedDB para soporte sin conexión.
- **Zustand** gestiona el estado del cliente:
  - `ui.store` – sidebar colapsado, tema (oscuro/claro), estado de modales, borradores de formularios.
  - Persistido en `localStorage` mediante `zustand/middleware`.

### Capa de API

- Instancia central de Axios (`src/api/client.js`) con interceptores para la inyección de tokens y normalización de errores.
- Los errores se transforman en objetos `ApiError` con `status`, `friendlyMessage` y `details`.
- Funciones de API por módulo en `src/api/endpoints/`.

### Enrutamiento

- Enrutamiento basado en funcionalidades usando `<Routes>` anidados en el `View.jsx` de cada módulo.
- Carga diferida (lazy loading) mediante `React.lazy` y `Suspense`.
- `ProtectedRoute` envuelve las rutas privadas, verificando autenticación y permisos.

### Manejo de Errores

- `ErrorBoundary` global captura errores en tiempo de ejecución y muestra una página de respaldo.
- El componente `ErrorMessage` muestra errores de la API con opción de reintento y detalles técnicos.
- El contexto `Notification` provee notificaciones tipo toast.
- `logger.service` centraliza el logging con habilitación según el entorno.

### Caché Sin Conexión

- Persistencia de TanStack Query usando `@tanstack/react-query-persist-client` con un adaptador de IndexedDB (`idb-keyval`).
- El hook `useOnlineStatus` detecta cambios en la conexión de red.
- El componente `QueryStateHandler` muestra datos en caché con una advertencia cuando no hay conexión, en lugar de un error grave.

### Modo Simulado (Mock)

- Cuando `VITE_ENABLE_MOCK=true`, `main.jsx` inicia el service worker de MSW.
- Los handlers simulados en `src/mocks/handlers.js` simulan todos los endpoints de la API.
- Los datos se generan (seed) y se persisten en IndexedDB (`idb-keyval`).
- Inicia sesión como `admin@facilityos.com` / `admin123` o cualquier usuario generado.

---

## Roles y Permisos de Usuario

| Capacidad           | Admin | DistrictAdmin | SchoolAdmin |
|---------------------|-------|----------------|-------------|
| CRUD de Distritos   | Todo  | Solo lectura   | Solo lectura|
| CRUD de Escuelas    | Todo  | Propio distrito| Propia escuela |
| CRUD de Usuarios    | Todo  | Propio distrito| Propia escuela |
| CRUD de Beacons     | Todo  | Solo lectura   | Solo lectura|
| CRUD de Facultades  | Todo  | Propio distrito| Propia escuela |

La jerarquía y el alcance de los roles se aplican tanto en el backend como en la interfaz del frontend.

---

## UI/UX

- Componentes de Ant Design con tokens de tema personalizados.
- Diseño responsivo con sidebar colapsable.
- Alternancia entre tema oscuro y claro.
- Componentes reutilizables: `ConfirmDialog`, `EmptyState`, `LoadingSpinner`, `ErrorMessage`, `QueryStateHandler`.
- Tarjetas de módulos en el dashboard principal para navegación rápida.

---

## Pruebas

- Pruebas unitarias con **Vitest** y **Testing Library**.
- Ejecutar pruebas: `npm run test`
- Cobertura: `npm run test:coverage`

---

## Calidad de Código

- Configuración plana de **ESLint** con plugins de React hooks y refresh.
- **Prettier** para un formato consistente.
- `lint-staged` se ejecuta en el pre-commit.
- `npm run validate` ejecuta lint + pruebas.

---

## Despliegue

El proyecto incluye un Dockerfile multi-stage y configuración de Nginx.

### Docker

Construir la imagen:

```bash
docker build -t facilityos-frontend .
```

Ejecutar el contenedor:

```bash
docker run -p 3000:80 facilityos-frontend
```

O usar Docker Compose:

```bash
docker-compose -f docker-compose.production.yml up -d
```

### Nginx

La configuración de Nginx sirve el build estático y actúa como proxy inverso de `/api` hacia el contenedor del backend. La variable de entorno `BACKEND_PRIVATE_URL` establece la dirección del backend.

---

## Licencia

**Copyright © 2026 FacilityOS. Todos los derechos reservados.**

Este software, incluyendo su código fuente, arquitectura y documentación, es propiedad exclusiva de FacilityOS y se distribuye bajo licencia **propietaria**. Queda prohibida su reproducción, distribución, modificación, ingeniería inversa o uso total o parcial fuera del alcance autorizado, sin el consentimiento previo y por escrito del propietario.

Para consultas sobre licenciamiento, colaboración o uso autorizado, contactar al equipo responsable del proyecto.
