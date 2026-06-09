# Spec #5: Despliegue (Deployment)

## Estado
- [x] En especificación
- [ ] En revisión
- [ ] Aprobada
- [ ] En implementación
- [ ] Completada

## Objetivo
Definir la arquitectura de despliegue de la plataforma utilizando servicios gratuitos.

---

## Arquitectura

```
┌─────────────────────┐     ┌──────────────────────┐     ┌─────────────────────┐
│                     │     │                      │     │                     │
│   Frontend          │     │   Backend             │     │   Base de Datos     │
│   (HTML/CSS/JS)     │────▶│   ASP.NET Core Web    │────▶│   MongoDB Atlas     │
│                     │     │   API                 │     │   (M0 Free Tier)    │
│   Vercel            │     │   Railway             │     │   512MB             │
│   (Free Tier)       │     │   (Free Tier)         │     │                     │
│                     │     │                      │     │                     │
└─────────────────────┘     └──────────────────────┘     └─────────────────────┘
```

---

## Servicios

### 1. MongoDB Atlas (Base de Datos)

**Plan:** M0 Sandbox (gratuito)
**Región:** Preferiblemente la más cercana (us-east-1 o eu-west-1)
**Almacenamiento:** 512 MB compartidos
**Configuración:**
1. Crear cuenta en [https://www.mongodb.com/atlas](https://www.mongodb.com/atlas)
2. Crear clúster M0 (gratuito)
3. Crear usuario de base de datos con contraseña
4. Configurar Network Access:
   - En desarrollo: whitelist IP actual
   - En producción: whitelist 0.0.0.0/0 (Railway IPs variables)
5. Obtener URI de conexión:
   ```
   mongodb+srv://<usuario>:<password>@cluster0.xxxxx.mongodb.net/<dbname>?retryWrites=true&w=majority
   ```

---

### 2. Railway (Backend C#)

**Plan:** Free Tier ($5 de crédito gratis/mes, suficiente para proyecto pequeño)
**Servicio:** ASP.NET Core Web API
**Configuración:**
1. Conectar repositorio de GitHub
2. Seleccionar el proyecto `backend/`
3. Variables de entorno necesarias:

| Variable | Descripción |
|----------|-------------|
| `MONGODB_URI` | URI de conexión a Atlas |
| `MONGODB_DB_NAME` | Nombre de la base de datos |
| `JWT_SECRET` | Clave secreta para firmar JWT |
| `ASPNETCORE_ENVIRONMENT` | `Production` |

4. Railway detecta automáticamente .NET y compila/publica
5. URL asignada: `https://api-resenas-series.railway.app`

---

### 3. Vercel (Frontend)

**Plan:** Hobby (gratuito)
**Servicio:** Static site (HTML + CSS + JS)
**Configuración:**
1. Conectar repositorio de GitHub
2. Directorio: `frontend/`
3. Framework: `Other` (estático)
4. Variables de entorno:
   - `VITE_API_URL` → `https://api-resenas-series.railway.app/api`
5. URL asignada: `https://resenas-series.vercel.app`

---

## Variables de entorno

### Backend (Railway)
| Variable | Ejemplo |
|----------|---------|
| `MONGODB_URI` | `mongodb+srv://user:pass@cluster.xxxxx.mongodb.net` |
| `MONGODB_DB_NAME` | `resenas-series` |
| `JWT_SECRET` | `clave-super-segura-de-al-menos-32-caracteres` |
| `ASPNETCORE_ENVIRONMENT` | `Production` |

### Frontend (Vercel) — Para JS:
```javascript
const API_URL = process.env.API_URL || "http://localhost:5000/api";
```

---

## Pipeline de CI/CD

Sin CI/CD automatizado (proyecto escolar):
1. Desarrollo local
2. Commit a GitHub
3. Railway y Vercel detectan cambios y redeployan automáticamente

---

## Criterios de aceptación

- [ ] **CA-01**: La base de datos Atlas M0 está operativa y accesible desde Railway
- [ ] **CA-02**: El backend en Railway responde correctamente a las peticiones HTTP
- [ ] **CA-03**: El frontend en Vercel carga y se comunica con el backend
- [ ] **CA-04**: Las variables de entorno están configuradas (sin secrets en código)
- [ ] **CA-05**: El proyecto está en GitHub como repositorio público
- [ ] **CA-06**: Los deploys se actualizan automáticamente al hacer push a main

## Tareas derivadas

- [ ] Crear repositorio en GitHub
- [ ] Configurar MongoDB Atlas (clúster, usuario, network access)
- [ ] Configurar Railway (conectar repo, variables de entorno)
- [ ] Configurar Vercel (conectar repo, variable API_URL)
- [ ] Probar flujo completo (front → back → db)
