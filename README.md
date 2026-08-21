# Inmobiliaria Urbani Ferrando— Entrega 1

Sistema web de gestión inmobiliaria desarrollado con ASP.NET Core MVC, C# y MySQL. Esta primera entrega cubre el ABM (Alta, Baja, Modificación) completo de **Propietarios** e **Inquilinos**.

## Integrantes del grupo

- [Urbani Jose Maria 46260667]
- [Ferrando Carlos Enrique 28173863]

## Tecnologías utilizadas

- **Back-end:** ASP.NET Core MVC (.NET 10), C#
- **Acceso a datos:** ADO.NET puro con [MySqlConnector](https://mysqlconnector.net/)
- **Base de datos:** MySQL 8.x
- **Front-end:** Vistas Razor + JavaScript asincrónico (Fetch API / async-await)

## Arquitectura

```
Controllers/    → Orquestación HTTP. No conoce SQL ni reglas de negocio.
Services/       → Lógica de negocio (ej. validación de DNI único).
Repositories/   → Acceso a datos con ADO.NET, consultas 100% parametrizadas.
Models/         → Entidades de dominio con Data Annotations (validación server-side).
Common/         → Utilidades transversales (excepciones de negocio, etc.)
Views/          → Vistas Razor.
wwwroot/js/     → JavaScript asincrónico (fetch/async-await), validación de cliente.
Database/       → Script SQL de creación e inicialización.
```

Cada entidad (`Propietario`, `Inquilino`) implementa la interfaz genérica `IRepositorio<T>`, evitando duplicar la firma de las operaciones CRUD + paginación entre repositorios (principio DRY).

## Diagrama Entidad-Relación (DER)

```mermaid
erDiagram
    PROPIETARIO {
        int Id PK
        varchar Dni UK
        varchar Nombre
        varchar Apellido
        varchar Telefono
        varchar Email
        boolean Activo
        datetime FechaCreacion
    }

    INQUILINO {
        int Id PK
        varchar Dni UK
        varchar Nombre
        varchar Apellido
        varchar Telefono
        varchar Email
        boolean Activo
        datetime FechaCreacion
    }
```

> **Nota:** en esta entrega, `Propietario` e `Inquilino` son entidades independientes, sin relación entre sí todavía. En entregas futuras, `Propietario` se vinculará con `Inmueble` (1 a N) e `Inquilino` con `Reserva` (1 a N), según el diseño completo del proyecto.

## Instalación y puesta en marcha

### Requisitos previos

- [.NET SDK 10](https://dotnet.microsoft.com/download) o superior
- Servidor MySQL 8.x (recomendado: [Laragon](https://laragon.org/download/) para Windows, incluye MySQL + HeidiSQL)
- Un cliente SQL para ejecutar el script (HeidiSQL, DBeaver, MySQL Workbench, o el que prefieras)

### 1. Clonar el repositorio

```bash
git clone <https://github.com/FerrandoCarlos/inmobiliaria-Urbani-Ferrando>
cd inmobiliaria-Urbani-Ferrando
```

### 2. Crear la base de datos

1. Iniciá tu servidor MySQL (en Laragon: botón "Start All").
2. Abrí tu cliente SQL preferido y conectate con las credenciales de tu instalación local (por defecto en Laragon: usuario `root`, sin contraseña, puerto `3306`).
3. Ejecutá el script completo ubicado en `Database/script_inicial.sql`. Este script:
   - Crea la base de datos `inmobiliaria_db`.
   - Crea las tablas `Propietario` e `Inquilino`.
   - Inserta datos de prueba (3 registros en cada tabla).

### 3. Configurar la cadena de conexión

El repositorio **no incluye** el archivo `appsettings.Development.json` (está en `.gitignore` para no exponer credenciales). Creá ese archivo en la raíz del proyecto con el siguiente contenido, ajustando usuario/contraseña según tu instalación local:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=inmobiliaria_db;User=root;Password=;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### 4. Restaurar dependencias y ejecutar

```bash
dotnet restore
dotnet run
```

La consola va a indicar la URL local (por ejemplo `http://localhost:5104`). Abrí esa URL en el navegador y navegá a `/Propietarios` o `/Inquilinos` para ver el ABM funcionando.

## Funcionalidades de esta entrega

- ABM completo (Alta, Baja lógica, Modificación, Listado paginado) de Propietarios e Inquilinos.
- Validación doble: Data Annotations en el servidor (`ModelState`) + validación espejo en JavaScript antes de cada petición asincrónica.
- Protección contra inyección SQL: 100% de las consultas parametrizadas, sin concatenación de strings.
- Protección CSRF: `[ValidateAntiForgeryToken]` en cada endpoint de escritura, token incluido en cada `fetch` desde el cliente.
- Baja lógica (campo `Activo`) en vez de `DELETE` físico, para preservar integridad referencial en entregas futuras.
- Manejo de errores diferenciado: excepciones de negocio (`AppException`, ej. DNI duplicado) devuelven HTTP 400 con mensaje claro; errores técnicos inesperados devuelven HTTP 500 sin exponer detalles internos.

## Próximas entregas (fuera de alcance actual)

- Entidades `Inmueble`, `Reserva`, `Pago`, `Usuario` y `Rol`.
- Relación de `Propietario` con `Inmueble` y de `Inquilino` con `Reserva`.
- Autenticación y autorización de usuarios.
