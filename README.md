# 💖 LoveCampus

**LoveCampus** es una aplicación de consola desarrollada en C# .NET Core que simula una plataforma de citas para estudiantes de Campuslands. Permite a los usuarios registrarse, gestionar su perfil, interactuar con otros mediante likes/dislikes, formar coincidencias (matches) y acceder a una tienda virtual. Todo esto, desde un entorno sencillo e intuitivo basado en consola.

> 📚 **Más información en la wiki del proyecto**:  
> 👉 [https://deepwiki.com/Designedby-Radol/LoveCampus](https://deepwiki.com/Designedby-Radol/LoveCampus)

---

## 📌 Características Principales

- **Gestión de usuarios**
  - Registro y autenticación
  - Edición de perfil
- **Sistema de Matching**
  - Visualización de perfiles
  - Like/Dislike entre usuarios
  - Coincidencias automáticas (match)
- **Tienda Virtual**
  - Sistema de créditos (capcoins)
  - Compra de likes extra y tokens
- **Panel de Administración**
  - Gestión de usuarios
  - Análisis general del sistema

---

## 🏗️ Estructura del Proyecto

LoveCampus/
├── Application/
│ └── UI/
│ ├── AdminUI.cs
│ ├── AuthUI.cs
│ ├── ConsoleUI.cs
│ ├── TiendaUI.cs
│ └── UsuarioUI.cs
├── Infrastructure/
│ ├── Data/
│ │ └── DbContext.cs
│ └── Repositories/
│ ├── Repository.cs
│ └── UsuarioRepository.cs
├── Program.cs
└── setup_database.sql

yaml
Copiar
Editar

---

## ⚙️ Requisitos Técnicos

- [.NET 6.0 SDK o superior](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- MySQL Server
- Paquetes NuGet:
  - `Dapper`
  - `MySql.Data`

---

## 🚀 Instalación y Configuración

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/Designedby-Radol/LoveCampus.git
   cd LoveCampus
Configurar la base de datos

Ejecuta el script setup_database.sql en tu servidor MySQL para crear las tablas y datos iniciales.

Actualizar cadena de conexión

En DbContext.cs, configura tu cadena de conexión con los datos de tu servidor MySQL.

Compilar y ejecutar la aplicación

bash
Copiar
Editar
dotnet run
🧩 Modelo de Datos
Entidad	Descripción
Usuarios	Información personal del usuario (nombre, edad, género, carrera, intereses)
Géneros	Opciones de identidad de género disponibles
Carreras	Programas académicos registrados
Intereses	Preferencias/intereses de los usuarios
Interacciones	Registro de likes/dislikes entre usuarios
Matches	Coincidencias entre usuarios con like mutuo
Tienda	Productos que se pueden comprar con capcoins

🧭 Navegación por la Aplicación
📍 Menú Principal
Opciones para registrarse, iniciar sesión o salir.

🙋 Menú de Usuario (una vez autenticado)
Ver perfiles

Dar like/dislike

Ver coincidencias

Acceder a la tienda

Editar perfil

🛒 Tienda
Comprar likes y tokens adicionales con capcoins

🛠️ Panel de Administración
Solo accesible para administradores

Permite gestión de usuarios, revisión de datos y configuración general del sistema

🛠️ Tecnologías Utilizadas
Lenguaje: C#

Framework: .NET Core 6

Base de Datos: MySQL

ORM: Dapper

Arquitectura: En capas (Presentación, Negocio, Datos)

Patrón de diseño: Repository Pattern

👥 Colaboradores
Omar Cardona

Raúl Ramírez