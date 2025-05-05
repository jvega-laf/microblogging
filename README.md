# 🐦 Microblogging Platform (Twitter-like Clone)

Este proyecto es una plataforma de microblogging simplificada que permite a los usuarios publicar tweets, seguir a otros usuarios y ver un timeline.

---

## 📦 Tecnologías

- **Backend:** C# con .NET 9
- **Arquitectura:** Clean Architecture + DDD
- **Infraestructura:** Docker, Docker Compose
- **Tests:** xUnit
- **Base de datos:** Redis (por ahora), pero pensado para migrar a MongoDB / PostgreSQL
- **Validaciones:** FluentValidation
- **Estilo de desarrollo:** Optimizado para lectura (Read-heavy)

---

## ▶️ Requisitos Previos

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- (Opcional) [Visual Studio Code](https://code.visualstudio.com/) con extensiones de C# y Docker/Podman

Necesitás tener instalado al menos uno de los siguientes entornos de contenedores:

- [Docker](https://www.docker.com/)
- [Docker Compose](https://docs.docker.com/compose/)
  O bien, como alternativa:

- [Podman](https://podman.io/)
- [Podman Compose](https://github.com/containers/podman-compose)

📌 podman-compose es un reemplazo de docker-compose que usa Podman como motor.

---

## 🚀 Levantar el Proyecto

### 🐳 Usando Docker/Podman Compose

1. Cloná el repositorio:

   git clone https://github.com/tuusuario/microblogging.git
   cd microblogging

2. Construí y levantá los servicios:

   docker-compose up --build
   podman-compose up --build

📌 Tener en cuenta para actualizarlos hacer un down mediante el compose

3. La API estará disponible en:

   http://localhost:5000

📌 Si estás en Linux/Mac y el puerto 5000 está ocupado, podés cambiarlo en el docker-compose.yml.
