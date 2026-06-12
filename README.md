# Supermarket Rubik - Proyecto de Arquitectura Multiplataforma

Aplicación web empresarial multiplataforma desarrollada con **ASP.NET Core MVC**, **Entity Framework Core** y **PostgreSQL**. El sistema implementa las operaciones centrales de un minimarket, soportando un alto volumen de datos (500,000 registros).

## 📊 Diagrama Entidad-Relación (Base de Datos)
[![Diagrama ER](https://i.postimg.cc/HsNVvSct/image-(1).jpg)](https://postimg.cc/Mvy6vmYc)

## 🚀 Tecnologías Utilizadas
- **Backend:** C# / .NET 10.0 / ASP.NET Core MVC
- **ORM:** Entity Framework Core
- **Base de Datos:** PostgreSQL
- **Frontend:** HTML5, Bootstrap 5, Razor Views, jQuery
- **Autenticación:** ASP.NET Core Identity (Session y Roles)

## 🎯 Características Principales
1. **Arquitectura Organizada:** Separación lógica en Controllers, Models, Data y Views.
2. **Rendimiento:** Implementación estricta de paginación desde la base de datos usando `Skip()`, `Take()`, `AsNoTracking()` y consultas `Where()` filtradas, evitando cargar información masiva en memoria.
3. **Carga Masiva:** Script SQL nativo para la generación de exactamente 500,000 registros de prueba de alta variedad.
4. **Eliminación Lógica:** Todos los registros conservan integridad mediante el uso de campos booleanos (`Activo`) y marcas de tiempo sin borrado físico.
5. **Autenticación y Roles:** Sistema de inicio de sesión. Acceso diferenciado entre Clientes (solo Tienda y Carrito) y Administradores (Módulos CRUD).
6. **Reportes:** Dashboard administrativo con consultas LINQ complejas (Sum, Count, GroupBy, Average, OrderByDescending).
7. **E-Commerce Integrado:** Módulo de tienda y carrito de compras manejado en caché de sesión para evitar saturar la BD hasta completar el checkout.

## 🛠️ Requisitos de Instalación
- [.NET SDK 10.0](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)

## ⚙️ Pasos de Ejecución

1. **Clonar el repositorio:**
   ```bash
   git clone https://github.com/Sahid-B/ProyectoU2_Web
   cd MiniMarketApp_SB
   ```

2. **Configurar la Cadena de Conexión:**
   Renombrar el archivo `appsettings.example.json` a `appsettings.json` y configurar las credenciales de PostgreSQL:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Port=5432;Database=SupermarketRubikDB;Username=postgres;Password=tu_contraseña"
   }
   ```

3. **Restaurar Dependencias y Aplicar Migraciones:**
   ```bash
   dotnet restore
   dotnet ef database update
   ```

4. **Carga Masiva de Datos (Seeder):**
   Ejecutar el script `seed_postgres.sql` directamente en PostgreSQL (mediante pgAdmin, DBeaver o psql) sobre la base de datos `SupermarketRubikDB`. Esto poblará la BD con los 500,000 registros en segundos.

5. **Iniciar la Aplicación:**
   ```bash
   dotnet run
   ```
   Abrir un navegador e ingresar a `http://localhost:5000` (o el puerto indicado en la consola).

## 📊 Distribución de Datos (500,000 Registros)
La base de datos contiene la siguiente distribución exacta:
- **Categorías:** 100
- **Proveedores:** 1,000
- **Empleados:** 1,000
- **Clientes:** 80,000
- **Productos:** 30,000
- **Ventas:** 150,000
- **DetalleVentas:** 237,900
*(Nota: Ajustable según necesidades)*

## 👥 Cuentas de Acceso (Prueba)
- **Administrador:** `Sahid3` / Contraseña: `TuContraseña123!` (o la configurada en tu registro).
- **Cliente:** Puedes registrar una cuenta nueva en la plataforma.
