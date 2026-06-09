# Spec #2: API REST Endpoints

## Estado
- [x] En especificación
- [ ] En revisión
- [ ] Aprobada
- [ ] En implementación
- [ ] Completada

## Objetivo
Definir los endpoints REST que expondrá la API de ASP.NET Core Web API para la plataforma de reseñas de series.

## URL Base
- Desarrollo: `http://localhost:5000/api`
- Producción: `https://api-resenas-series.railway.app/api`

## Formato General

### Requests
- Content-Type: `application/json`
- Autenticación vía Header: `Authorization: Bearer <token>`

### Responses
```json
{
  "exitoso": true | false,
  "datos": { ... },
  "mensaje": "string",
  "errores": ["string"]
}
```

### Códigos HTTP
| Código | Significado |
|--------|-------------|
| 200 | Éxito |
| 201 | Recurso creado |
| 400 | Error de validación |
| 401 | No autenticado |
| 403 | No autorizado |
| 404 | No encontrado |
| 500 | Error interno |

---

## Endpoints

### Autenticación (`/api/auth`)

| Método | Ruta | Autenticación | Descripción |
|--------|------|---------------|-------------|
| POST | `/register` | No | Registrar nuevo usuario |
| POST | `/login` | No | Iniciar sesión |

#### POST `/api/auth/register`
**Request:**
```json
{
  "nombre": "string (requerido, 3-50 chars)",
  "email": "string (requerido, email válido)",
  "password": "string (requerido, min 8 chars)"
}
```
**Response (201):**
```json
{
  "exitoso": true,
  "datos": {
    "id": "ObjectId",
    "nombre": "string",
    "email": "string",
    "token": "jwt_string"
  }
}
```
**Errores posibles:** 400 (email duplicado, validación)

#### POST `/api/auth/login`
**Request:**
```json
{
  "email": "string (requerido)",
  "password": "string (requerido)"
}
```
**Response (200):**
```json
{
  "exitoso": true,
  "datos": {
    "id": "ObjectId",
    "nombre": "string",
    "email": "string",
    "token": "jwt_string"
  }
}
```
**Errores posibles:** 401 (credenciales inválidas)

---

### Usuarios (`/api/usuarios`)

| Método | Ruta | Autenticación | Descripción |
|--------|------|---------------|-------------|
| GET | `/perfil` | Sí | Obtener perfil del usuario autenticado |
| PUT | `/perfil` | Sí | Actualizar perfil |
| GET | `/{id}/resenas` | No | Obtener reseñas de un usuario |

---

### Series (`/api/series`)

| Método | Ruta | Autenticación | Descripción |
|--------|------|---------------|-------------|
| GET | `/` | No | Listar series (con paginación y filtros) |
| GET | `/{id}` | No | Obtener detalle de una serie |
| POST | `/` | Admin | Crear nueva serie |
| PUT | `/{id}` | Admin | Actualizar serie |
| DELETE | `/{id}` | Admin | Eliminar serie |
| GET | `/buscar?q=` | No | Buscar series por título/descripción |

#### GET `/api/series`
**Query params:**
- `pagina` (int, default 1)
- `limite` (int, default 10, max 50)
- `genero` (string opcional)
- `ordenarPor` (string: "rating" | "titulo" | "anio", default "titulo")
- `orden` (string: "asc" | "desc", default "asc")

**Response (200):**
```json
{
  "exitoso": true,
  "datos": [
    {
      "id": "ObjectId",
      "titulo": "string",
      "descripcion": "string",
      "generos": ["string"],
      "anioEstreno": 2020,
      "poster": "string",
      "temporadas": 5,
      "ratingPromedio": 8.5,
      "totalResenas": 42
    }
  ],
  "total": 100,
  "pagina": 1,
  "totalPaginas": 10
}
```

#### GET `/api/series/{id}`
**Response (200):**
Incluye los mismos campos que el listado, más `createdAt`, `updatedAt`, y un array de las reseñas más recientes (top 5).

---

### Reseñas (`/api/resenas`)

| Método | Ruta | Autenticación | Descripción |
|--------|------|---------------|-------------|
| GET | `/serie/{idSerie}` | No | Obtener reseñas de una serie |
| POST | `/` | Sí | Crear reseña |
| PUT | `/{id}` | Sí (propietario) | Actualizar reseña |
| DELETE | `/{id}` | Sí (propietario) | Eliminar reseña |

#### POST `/api/resenas`
**Request:**
```json
{
  "idSerie": "ObjectId (requerido)",
  "puntuacion": "number (requerido, 1-10)",
  "comentario": "string (opcional, máx 2000 chars)"
}
```
**Response (201):** Reseña creada

**Errores:** 400 (ya existe reseña del usuario para esa serie), 404 (serie no existe)

#### GET `/api/resenas/serie/{idSerie}`
**Query params:** `pagina`, `limite`, `orden` (por fecha)

**Response (200):** Array paginado de reseñas, cada una incluye datos del usuario (nombre, avatar).

---

## Criterios de aceptación

- [ ] **CA-01**: Todos los endpoints responden con el formato JSON estándar
- [ ] **CA-02**: La autenticación vía JWT protege los endpoints marcados
- [ ] **CA-03**: La paginación funciona correctamente en listados
- [ ] **CA-04**: Un usuario no puede modificar/eliminar reseñas de otro usuario
- [ ] **CA-05**: Solo administradores pueden crear/editar/eliminar series
- [ ] **CA-06**: La búsqueda devuelve resultados relevantes por título

## Tareas derivadas

- [ ] Crear proyecto ASP.NET Core Web API
- [ ] Implementar controlador de autenticación
- [ ] Implementar controlador de series
- [ ] Implementar controlador de reseñas
- [ ] Implementar middleware de JWT
- [ ] Implementar paginación genérica
- [ ] Documentar endpoints con Swagger/OpenAPI
