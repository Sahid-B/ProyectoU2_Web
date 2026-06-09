# MiniMarket App - Integración de Arquitectura Multiplataforma

Este proyecto es la primera fase organizativa del sistema "MiniMarket App", desarrollado con **ASP.NET Core MVC** y **Entity Framework Core**. Su finalidad es preparar correctamente la estructura del proyecto, validar la conexión con PostgreSQL y dejar una base técnica clara para la futura implementación completa.

## 💡 Idea de Negocio

- **Nombre del proyecto:** MiniMarket App
- **Problema que resuelve:** Necesidad de un control eficiente del inventario, ventas y proveedores del minimercado, reemplazando registros manuales y optimizando la atención.
- **Usuarios principales:** Administradores, cajeros y personal de inventario/bodega.
- **Módulos previstos:** Productos, Categorías, Proveedores, Clientes y Ventas.
- **Datos principales:** Información estructurada del catálogo de productos, stock, datos de contacto de proveedores y clientes, y registro detallado de transacciones.

## 🗄️ Modelo de Base de Datos Inicial

El sistema cuenta con un diseño de base de datos relacional inicial pensado para escalar en la fase final. Incluye llaves primarias, foráneas, campos de auditoría (FechaCreacion) y eliminación lógica (Activo).

![Estructura de la Base de Datos](https://i.postimg.cc/9fS9N2Ny/image.jpg)

## 📦 Módulos Principales (CRUD Iniciales)

Se han implementado operaciones CRUD (Crear, Listar, Editar, Detalle, Eliminar - con eliminación lógica) para validar la estructura funcional:

- **Categorías (`CategoriasController`)**: Gestión y clasificación de productos.
- **Clientes (`ClientesController`)**: Registro y administración de clientes.
- **Productos (`ProductosController`)**: Control del catálogo e inventario.
- **Proveedores (`ProveedoresController`)**: Gestión de proveedores de los productos.
- **Ventas (`VentasController`)**: Registro general de ventas.
- **Detalles de Venta (`DetallesVentaController`)**: Registro detallado por producto.

### Preparación para el Proyecto Final
Esta estructura base permitirá posteriormente:
- **Carga masiva:** Soportar la carga de hasta 500.000 registros mediante el modelo relacional de PostgreSQL.
- **Paginación y Filtros:** Implementación para evitar sobrecarga en la consulta de todos los registros simultáneamente.
- **Eliminación lógica:** Todos los módulos utilizan el campo `Activo` para ocultar registros sin eliminarlos físicamente, conservando el historial.
- **Sesión y Reportes:** La base está preparada para agregar protección de rutas y cálculo de estadísticas financieras en las próximas fases.

## 🚀 Tecnologías Utilizadas

- **Framework:** .NET 10.0 (ASP.NET Core MVC)
- **Patrón de Arquitectura:** MVC
- **ORM:** Entity Framework Core 10.0
- **Base de Datos:** PostgreSQL

## 🛠️ Requisitos Previos

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)

## ⚙️ Configuración y Ejecución

1. **Configurar la Cadena de Conexión:**
   En la carpeta `MiniMarketApp`, renombra o copia el archivo `appsettings.example.json` como `appsettings.json` o `appsettings.Development.json` y configura tus credenciales reales de PostgreSQL:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Port=5432;Database=minimarket_db;Username=tu_usuario;Password=tu_contrasena"
   }
   ```

2. **Aplicar Migraciones:**
   Abre una terminal en la carpeta del proyecto (`MiniMarketApp`) y ejecuta:
   ```bash
   dotnet ef database update
   ```

3. **Ejecutar el Proyecto:**
   Para compilar e iniciar la aplicación web:
   ```bash
   dotnet run
   ```
   El sistema estará disponible en `http://localhost:5000` o `https://localhost:5001`.

## 📁 Estructura del Proyecto

- `Models/`: Entidades del negocio (Productos, Categorías, etc.).
- `Data/`: DbContext y configuración de Entity Framework.
- `Controllers/`: Controladores MVC para manejar las peticiones HTTP.
- `Views/`: Interfaces generadas con Razor.
- `Migrations/`: Archivos generados para mantener sincronizada la BD con PostgreSQL.
- `appsettings.example.json`: Plantilla para configurar la cadena de conexión sin exponer claves reales.
