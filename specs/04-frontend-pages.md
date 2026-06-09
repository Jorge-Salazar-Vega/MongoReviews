# Spec #4: Frontend - Páginas y Componentes

## Estado
- [x] En especificación
- [ ] En revisión
- [ ] Aprobada
- [ ] En implementación
- [ ] Completada

## Objetivo
Definir las páginas del frontend (HTML + CSS + JS vanilla) y su interacción con la API REST.

---

## Stack
- HTML5 semántico
- CSS3 (sin frameworks, diseño responsive)
- JavaScript vanilla (fetch API)
- Sin frameworks JS (SPA ligero con navegación manual)

---

## Páginas

### 1. Landing / Inicio (`index.html`)

**Funcionalidad:**
- Hero section con el nombre de la plataforma
- Listado de series destacadas (top 10 por rating)
- Barra de búsqueda
- Navbar con login/registro o perfil si está autenticado

**API calls:**
- `GET /api/series?ordenarPor=rating&orden=desc&limite=10`

---

### 2. Registro (`register.html`)

**Funcionalidad:**
- Formulario: nombre, email, password, confirmar password
- Validación en cliente (email formato, password match, min 8 chars)
- Mensajes de error inline

**API calls:**
- `POST /api/auth/register`

---

### 3. Login (`login.html`)

**Funcionalidad:**
- Formulario: email, password
- Enlace a registro
- Mensajes de error inline

**API calls:**
- `POST /api/auth/login`

---

### 4. Detalle de Serie (`serie.html?id=X`)

**Funcionalidad:**
- Información completa de la serie (póster, título, descripción, género, año, temporadas, rating)
- Sección de reseñas con paginación
- Formulario para dejar reseña (solo si autenticado y no ha reseñado)
- Botón de editar/eliminar reseña propia

**API calls:**
- `GET /api/series/{id}`
- `GET /api/resenas/serie/{idSerie}`
- `POST /api/resenas`
- `PUT /api/resenas/{id}`
- `DELETE /api/resenas/{id}`

---

### 5. Perfil de Usuario (`perfil.html`)

**Funcionalidad:**
- Datos del perfil (nombre, email, avatar)
- Listado de reseñas del usuario
- Editar nombre/avatar

**API calls:**
- `GET /api/usuarios/perfil`
- `PUT /api/usuarios/perfil`

---

### 6. Administración (`admin.html`) — Solo rol admin

**Funcionalidad:**
- CRUD de series (tabla con crear/editar/eliminar)
- Modal/formulario para agregar/editar serie

**API calls:**
- `GET /api/series`
- `POST /api/series`
- `PUT /api/series/{id}`
- `DELETE /api/series/{id}`

---

## Diseño responsive

| Breakpoint | Target |
|------------|--------|
| < 576px | Móviles |
| 576px - 992px | Tablets |
| > 992px | Desktop |

---

## Manejo de estado (JS)

El frontend mantendrá un objeto global `AppState`:
```javascript
const AppState = {
  usuario: null,        // { id, nombre, email, rol }
  token: null,          // JWT string
  autenticado: false
};
```

- Al login/registro exitoso: guardar token en `localStorage` y actualizar `AppState`
- Al cargar la página: verificar `localStorage` para restaurar sesión
- Todas las llamadas API incluyen `Authorization: Bearer <token>` si `AppState.token` existe

---

## Criterios de aceptación

- [ ] **CA-01**: El layout es responsive (móvil, tablet, desktop)
- [ ] **CA-02**: Navegación funcional entre todas las páginas
- [ ] **CA-03**: Registro y login redirigen al inicio con sesión iniciada
- [ ] **CA-04**: Un usuario no autenticado no ve el formulario de reseña
- [ ] **CA-05**: Las reseñas de una serie se cargan con paginación (scroll o botón)
- [ ] **CA-06**: La página de admin solo es accesible si el usuario es admin
- [ ] **CA-07**: Los errores de API se muestran al usuario (toast o mensaje inline)
- [ ] **CA-08**: Los ratings se muestran visualmente (estrellas o barra)

## Tareas derivadas

- [ ] Crear estructura de archivos del frontend
- [ ] Implementar CSS base y sistema de componentes
- [ ] Implementar página de inicio con listado de series
- [ ] Implementar registro y login
- [ ] Implementar detalle de serie con reseñas
- [ ] Implementar perfil de usuario
- [ ] Implementar panel de administración
- [ ] Implementar manejo de estado y localStorage
