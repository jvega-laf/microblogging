📘 **Documentación High Level - Microblogging Platform**

## 🤝 Visión General

Este proyecto implementa una versión simplificada de una red social tipo Twitter, permitiendo que los usuarios:

- Publiquen mensajes cortos (tweets).
- Sigan a otros usuarios.
- Vean un timeline con tweets de los usuarios que siguen.

La solución sigue una arquitectura limpia (Clean Architecture) basada en capas bien definidas y desacopladas, que permite alta testabilidad, mantenibilidad y escalabilidad.

---

## 🏩 Arquitectura General

### 🔹 Estilo Arquitectónico

- **Clean Architecture** con separación en:

  - **Domain**: entidades y lógica de negocio.
  - **Application**: casos de uso.
  - **Infrastructure**: acceso a datos y recursos externos.
  - **Api**: punto de entrada HTTP.
  - **WebClient**: frontend React/Vite.
  - **Validators**, **Contracts** y **Abstractions** como proyectos auxiliares.

---

## 📁 Estructura de la Solución

```
Microblogging.sln
├── Microblogging.Api                // API REST
├── Microblogging.Application        // Casos de uso
├── Microblogging.Application.Abstractions // Interfaces de persistencia
├── Microblogging.Application.Contracts   // DTOs y requests
├── Microblogging.Domain             // Entidades del dominio
├── Microblogging.Infrastructure     // Persistencia y Repositorios
├── Microblogging.Validators         // Validaciones con FluentValidation
├── Microblogging.IntegrationTests   // Pruebas de integración
├── Microblogging.Web                // Frontend React/Vite
└── docker-compose.yml               // Orquestación de servicios
```

---

## ⚙️ Componentes y Tecnologías

| Componente   | Tecnología                        |
| ------------ | --------------------------------- |
| API REST     | ASP.NET Core 9                    |
| Frontend     | React + Vite + CSS                |
| Validación   | FluentValidation                  |
| Persistencia | Redis (mock in-memory para tests) |
| Testing      | xUnit, Moq, WebApplicationFactory |
| Contenedores | Docker / Podman                   |
| Orquestación | Docker Compose / Podman Compose   |

---

## 🧍🏼 Detalles de cada proyecto

- **Microblogging.Api**: Define endpoints HTTP para publicar tweets, seguir usuarios y obtener timelines. Contiene middlewares y configuraciones.
- **Microblogging.Application**: Lógica de negocio: handlers de comandos y queries con MediatR.
- **Microblogging.Application.Abstractions**: Interfaces de repositorios (ITweetRepository, IFollowRepository).
- **Microblogging.Application.Contracts**: DTOs y requests.
- **Microblogging.Domain**: Entidades (Tweet, UserFollow, UserId) y ValueObjects.
- **Microblogging.Infrastructure**: Implementación de repositorios con Redis.
- **Microblogging.Validators**: Validaciones con FluentValidation.
- **Microblogging.IntegrationTests**: Pruebas de integración end-to-end.
- **Microblogging.Web**: SPA en React/Vite que consume la API. Tabs para Tweet, Timeline, Usuarios.

---

## 🧪 Endpoints REST definidos

- **POST /tweets** – Publicar un tweet (máx 280 caracteres).
- **POST /follow** – Seguir a un usuario.
- **POST /followable_users** – Obtener usuarios no seguidos.
- **GET /timeline** – Obtener timeline del usuario autenticado.

**Header común:** `X-User-Id: {Guid}`

---

## 🚀 Escalabilidad

### 🔹 Estado Actual

- La API maneja lectura y escritura en la misma capa.
- Endpoints de lectura/escritura CQRS para poder escalarlos y separarlos pero en la misma capa.

### 💡 Sugerencias para Futuras Mejoras

1. **Separación de Endpoints de Lectura y Escritura (CQRS)**

   - Desplegar servicios de lectura separados y optimizados para consultas (p. ej. caché, bases de datos especializadas).
   - Escalar independientemente la capa de lectura en picos altos de tráfico.

2. **Arquitectura Basada en Eventos**

   - Endpoints de escritura emiten eventos (p. ej. `TweetCreated`, `UserFollowed`) a un bus de mensajería (RabbitMQ/Kafka).
   - Servicios consumidores generan vistas denormalizadas o caches optimizados para lectura.

3. **Notificaciones en Tiempo Real**

   - Usar SignalR/WebSockets para comunicar nuevos tweets o seguimientos en tiempo real a los clientes.
   - Desacoplar notificaciones del flujo de negocio principal.

---

## 🤔 Futuras Extensiones

- Autenticación con JWT y gestión de sesiones.
- Paginación y filtrado en timelines.
- División en microservicios por bounded contexts.
- Métricas y monitoreo con Prometheus y Grafana.
- Pruebas de carga enfocadas en endpoints de lectura.
