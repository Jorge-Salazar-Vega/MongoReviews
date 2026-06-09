# Spec #1: Esquema de Base de Datos - MongoDB Atlas

## Estado
- [x] En especificación
- [ ] En revisión
- [ ] Aprobada
- [ ] En implementación
- [ ] Completada

## Objetivo
Definir la estructura de las colecciones de MongoDB para la plataforma de reseñas de series de TV.

## Stack técnico
- MongoDB Atlas (M0 - free tier)
- Driver: `MongoDB.Driver` para C#
- Conexión vía URI con autenticación

---

## Colecciones

### 1. `users` — Usuarios de la plataforma

```json
{
  "_id": "ObjectId",
  "nombre": "string",
  "email": "string",
  "passwordHash": "string",
  "avatar": "string | null",
  "rol": "string (user | admin)",
  "createdAt": "ISODate",
  "updatedAt": "ISODate"
}
```

**Restricciones:**
- `email` debe ser único (unique index)
- `passwordHash` almacena bcrypt (nunca texto plano)
- `rol` por defecto `"user"`

**Índices:**
| Campo | Tipo | Propósito |
|-------|------|-----------|
| `email` | Único (ascendente) | Login rápido y unicidad |

---

### 2. `series` — Series de TV

```json
{
  "_id": "ObjectId",
  "titulo": "string",
  "descripcion": "string",
  "generos": ["string"],
  "anioEstreno": "number",
  "poster": "string | null",
  "temporadas": "number",
  "ratingPromedio": "number",
  "totalResenas": "number",
  "createdAt": "ISODate",
  "updatedAt": "ISODate"
}
```

**Restricciones:**
- `titulo` no puede estar vacío
- `ratingPromedio` se recalcula al agregar/actualizar reseñas (0.0 - 10.0)
- `totalResenas` se actualiza al agregar/eliminar reseñas

**Índices:**
| Campo | Tipo | Propósito |
|-------|------|-----------|
| `titulo` | Texto ascendente | Búsqueda por nombre |
| `descripcion` | Texto ascendente | Búsqueda por descripción |
| `generos` | Multiclave (ascendente) | Filtrar por género |
| `ratingPromedio` | Ascendente/Descendente | Ordenar por rating |

---

### 3. `reviews` — Reseñas de usuarios

```json
{
  "_id": "ObjectId",
  "idUsuario": "ObjectId",
  "idSerie": "ObjectId",
  "puntuacion": "number (1-10)",
  "comentario": "string",
  "createdAt": "ISODate",
  "updatedAt": "ISODate"
}
```

**Restricciones:**
- `puntuacion` debe estar entre 1 y 10 (inclusive)
- Un usuario solo puede tener **una reseña por serie** (unique compound index)
- `comentario` máximo 2000 caracteres

**Índices:**
| Campo | Tipo | Propósito |
|-------|------|-----------|
| `{ idUsuario: 1, idSerie: 1 }` | Único compuesto | Una reseña por usuario/serie |
| `idSerie` | Ascendente | Obtener reseñas de una serie |
| `idUsuario` | Ascendente | Obtener reseñas de un usuario |

---

## Reglas de negocio (BD)

### Al crear una reseña (`INSERT`):
1. Validar que `idUsuario` exista en `users`
2. Validar que `idSerie` exista en `series`
3. Validar que no exista ya una reseña del mismo usuario para la misma serie
4. Actualizar `ratingPromedio` y `totalResenas` en la serie correspondiente

### Al actualizar una reseña (`UPDATE`):
1. Recalcular `ratingPromedio` de la serie con el nuevo valor

### Al eliminar una reseña (`DELETE`):
1. Recalcular `ratingPromedio` de la serie
2. Decrementar `totalResenas`

---

## Cálculo de ratingPromedio

```
ratingPromedio = SUM(puntuacion de todas las reseñas de la serie) / COUNT(reseñas de la serie)
```

Se redondea a 1 decimal.

---

## Criterios de aceptación

- [ ] **CA-01**: La colección `users` permite registro e inicio de sesión con email único
- [ ] **CA-02**: La colección `series` almacena toda la información de una serie de TV
- [ ] **CA-03**: La colección `reviews` garantiza una reseña por usuario por serie
- [ ] **CA-04**: Al crear/actualizar/eliminar una reseña, `ratingPromedio` se recalcula automáticamente
- [ ] **CA-05**: Las búsquedas por título, género y rating son eficientes gracias a los índices
- [ ] **CA-06**: La conexión a Atlas M0 funciona con la URI de conexión estándar

## Tareas derivadas

- [ ] Crear clúster M0 en MongoDB Atlas
- [ ] Configurar usuario de base de datos y whitelist de IPs
- [ ] Implementar modelos de C# (clases POCO)
- [ ] Implementar servicio de reseñas con lógica de recálculo
- [ ] Crear migración/seed de datos de prueba
