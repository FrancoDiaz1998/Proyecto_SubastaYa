# SubastaYa

Plataforma web de subastas con pujas en tiempo real, billetera virtual, autenticación JWT y cierre automático de subastas.

## Tecnologías

- **Backend:** ASP.NET Core 10, C#, Entity Framework Core 10, PostgreSQL, JWT.
- **Frontend:** React 19, TypeScript, Vite 8 y Tailwind CSS 4.
- **Arquitectura:** solución .NET separada en API, Application, Domain e Infrastructure.

## Requisitos

Instalá previamente:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js 22 LTS](https://nodejs.org/)
- [PostgreSQL](https://www.postgresql.org/download/) 16 o superior
- Git

Podés comprobar las versiones con:

```bash
dotnet --version
node --version
npm --version
psql --version
```

## Clonar el proyecto

```bash
git clone https://github.com/FrancoDiaz1998/Proyecto_SubastaYa
cd SubastaYa
```

## Configurar el backend

El backend usa PostgreSQL. Creá una base de datos vacía, por ejemplo `subastaya`, desde pgAdmin o con:

```bash
createdb -U postgres subastaya
```

La cadena de conexión y la clave JWT se guardan en **User Secrets**, por lo que no es necesario modificar `appsettings.json` ni subir credenciales al repositorio.

Desde la raíz del repositorio, ejecutá:

```bash
dotnet user-secrets set "ConnectionStrings:SubastaYa" "Host=localhost;Port=5432;Database=subastaya;Username=postgres;Password=TU_PASSWORD" --project backend/src/SubastaYa.Api
dotnet user-secrets set "Jwt:Key" "una-clave-local-de-al-menos-32-caracteres" --project backend/src/SubastaYa.Api
```

`appsettings.json` ya contiene la clave `ConnectionStrings:SubastaYa` vacía como valor por defecto. User Secrets la reemplaza automáticamente en el entorno de desarrollo.

### Crear las tablas

Instalá la herramienta de Entity Framework Core si todavía no la tenés:

```bash
dotnet tool install --global dotnet-ef --version 10.0.11
```

Aplicá las migraciones existentes:

```bash
dotnet ef database update --project backend/src/SubastaYa.Infrastructure --startup-project backend/src/SubastaYa.Api
```

En modo `Development`, la API carga automáticamente usuarios, categorías, billeteras y subastas de demostración al iniciar por primera vez.

## Ejecutar el proyecto

Abrí dos terminales desde la raíz.

### Terminal 1: API

```bash
dotnet run --project backend/src/SubastaYa.Api --launch-profile http
```

La API quedará disponible en `http://localhost:5124`.

### Terminal 2: frontend

```bash
cd frontend
npm install
```

Copiá las variables de entorno de ejemplo:

```bash
# macOS/Linux/Git Bash
cp .env.example .env

# Windows PowerShell
Copy-Item .env.example .env
```

El valor por defecto de `VITE_API_URL` ya apunta a `http://localhost:5124`. Si la API usa otra URL, editá `frontend/.env`.

Iniciá Vite:

```bash
npm run dev
```

Abrí la URL que muestre Vite, normalmente `http://localhost:5173`.

## Usuarios de demostración

La contraseña para todos los usuarios seed es `Demo123!`.

| Usuario | Email |
| --- | --- |
| Vendedor Demo | `vendedor@test.com` |
| Comprador 1 | `comprador1@test.com` |
| Comprador 2 | `comprador2@test.com` |
| Sin Fondos | `sinfondos@test.com` |



## Prueba de concurrencia de pujas

El proyecto incluye un script de PowerShell para comprobar el manejo de concurrencia optimista del backend. La prueba crea una subasta temporal y envía dos pujas iguales casi al mismo tiempo desde dos compradores distintos.

El resultado esperado es que una solicitud sea aceptada y la otra sea rechazada con `409 Conflict`, quedando una sola puja persistida en la base de datos.

Guardá el script en:

```text
scripts/prueba_concurrencia_subastaya.ps1
```

Primero levantá la API y dejala ejecutándose:

```powershell
cd backend
dotnet run --project .\src\SubastaYa.Api\SubastaYa.Api.csproj
```

En otra terminal PowerShell, desde la raíz del proyecto, ejecutá:

```powershell
powershell -ExecutionPolicy Bypass `
  -File .\scripts\prueba_concurrencia_subastaya.ps1
```

Si la API está usando un puerto distinto de `5124`, indicá la URL manualmente:

```powershell
powershell -ExecutionPolicy Bypass `
  -File .\scripts\prueba_concurrencia_subastaya.ps1 `
  -BaseUrl "http://localhost:TU_PUERTO"
```

Una ejecución correcta debería mostrar un resultado similar a:

```text
Comprador 1 -> HTTP 201 Created
Comprador 2 -> HTTP 409 Conflict
Cantidad de pujas de 11000 persistidas: 1
PRUEBA OK
```

También puede ocurrir al revés: el Comprador 2 puede recibir `201` y el Comprador 1 `409`. Lo importante es que solo una de las dos pujas simultáneas quede registrada.

## Estructura

```text
backend/
	SubastaYa.sln
	src/
		SubastaYa.Api/            # Controladores, configuración y proceso HTTP
		SubastaYa.Application/    # Casos de uso e interfaces
		SubastaYa.Domain/         # Entidades y reglas del dominio
		SubastaYa.Infrastructure/ # EF Core, PostgreSQL, JWT y repositorios
frontend/                     # Aplicación React + Vite + Tailwind
```

## Notas

- No subas contraseñas, claves JWT ni archivos `.env` reales al repositorio.
- La API aplica la migración solo cuando ejecutás explícitamente `dotnet ef database update`; el inicializador de desarrollo se ocupa de los datos demo.
- Para crear una nueva migración, ejecutá `dotnet ef migrations add NombreDeLaMigracion --project backend/src/SubastaYa.Infrastructure --startup-project backend/src/SubastaYa.Api`.