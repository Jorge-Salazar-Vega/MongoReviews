# Spec #3: Autenticación y Usuarios

## Estado
- [x] En especificación
- [ ] En revisión
- [ ] Aprobada
- [ ] En implementación
- [ ] Completada

## Objetivo
Definir el sistema de autenticación de la plataforma: registro, inicio de sesión, manejo de sesiones con JWT y control de acceso por roles.

---

## Flujo de autenticación

### Registro
```
Usuario → Frontend (formulario) → Backend POST /api/auth/register
                                     ↓
                               Validar datos (email, password, nombre)
                                     ↓
                               Verificar email no existente
                                     ↓
                               Hash de password (BCrypt)
                                     ↓
                               Insertar en colección users
                                     ↓
                               Generar JWT
                                     ↓
                               Response con token + datos usuario
```

### Login
```
Usuario → Frontend (formulario) → Backend POST /api/auth/login
                                     ↓
                               Buscar usuario por email
                                     ↓
                               Verificar password con BCrypt
                                     ↓
                               Generar JWT
                                     ↓
                               Response con token + datos usuario
```

### Acceso a rutas protegidas
```
Request → Header: Authorization Bearer <token>
              ↓
         Middleware JWT valida token
              ↓
         Extraer claims (id, rol)
              ↓
         Ejecutar acción si tiene permisos
```

---

## JWT (JSON Web Token)

### Payload
```json
{
  "sub": "ObjectId del usuario",
  "email": "email",
  "rol": "user | admin",
  "iat": 1234567890,
  "exp": 1234567890
}
```

### Configuración
| Parámetro | Valor |
|-----------|-------|
| Algoritmo | HS256 |
| Expiración | 24 horas desde emisión |
| Secret | Variable de entorno `JWT_SECRET` (mín 32 chars) |

### Claims incluidos
- `sub` (subject): ID del usuario
- `email`: Email del usuario
- `rol`: Rol para autorización

---

## Seguridad de contraseñas

| Aspecto | Especificación |
|---------|----------------|
| Hash | BCrypt (work factor: 12) |
| Almacenamiento | Solo hash, nunca texto plano |
| Longitud mínima | 8 caracteres |
| Requisitos | Al menos 1 letra y 1 número |

---

## Roles

| Rol | Permisos |
|-----|----------|
| `user` | Ver series, crear/editar/eliminar **sus propias** reseñas, editar su perfil |
| `admin` | Todo lo de `user` + crear/editar/eliminar **series**, gestionar usuarios |

---

## Protección de rutas en C#

```csharp
// Ejemplo de atributos a usar en los controladores

[Authorize]                    // Cualquier usuario autenticado
[Authorize(Roles = "admin")]   // Solo administradores
[AllowAnonymous]               // Sin autenticación
```

---

## Criterios de aceptación

- [ ] **CA-01**: Un usuario puede registrarse con nombre, email y password
- [ ] **CA-02**: No se permiten emails duplicados (devuelve 400)
- [ ] **CA-03**: Un usuario registrado puede iniciar sesión y recibe un JWT válido
- [ ] **CA-04**: Credenciales inválidas devuelven 401
- [ ] **CA-05**: Las rutas protegidas rechazan requests sin token (401)
- [ ] **CA-06**: Las rutas de admin rechazan usuarios sin rol admin (403)
- [ ] **CA-07**: El token expira después de 24h
- [ ] **CA-08**: La contraseña nunca se devuelve en ninguna response

## Tareas derivadas

- [ ] Implementar servicio de hash con BCrypt
- [ ] Implementar generación y validación de JWT
- [ ] Implementar middleware de autenticación
- [ ] Implementar política de autorización por roles
- [ ] Implementar endpoint de perfil de usuario
