# API de Gestión de Citas Médicas

API RESTful desarrollada con **C# y .NET 10** para registrar, consultar, actualizar, cancelar y eliminar las citas médicas de una clínica privada. Los datos se guardan **en memoria** (`List<T>`), sin base de datos: se pierden al detener la aplicación.

> Curso: Desarrollo de Servicios Web — Zegel · Evaluación parcial
> Integrantes: _(completar: nombre – rol)_
> Repositorio: _(completar: enlace de GitHub)_

## Requisitos

- [.NET SDK 10.0](https://dotnet.microsoft.com/download) (si solo tienes .NET 8 instalado, cambia `TargetFramework` a `net8.0` y la versión de Swashbuckle a `6.6.2` en el `.csproj`)
- [Postman](https://www.postman.com/downloads/) (para las pruebas)

## Instalación y ejecución

```bash
git clone <URL-DEL-REPOSITORIO>
cd CitasMedicas.Api
dotnet restore
dotnet run
```

La API queda disponible en `http://localhost:5080` y la documentación interactiva (Swagger) en `http://localhost:5080/swagger`.

## Estructura del proyecto

```
CitasMedicas.Api/
├── Controllers/   → Capa HTTP: recibe peticiones y devuelve códigos de estado
│   ├── CitasController.cs
│   ├── MedicosController.cs
│   └── PacientesController.cs
├── Services/      → Lógica de negocio (interfaz + implementación) y almacenamiento en memoria
│   ├── ICitaService.cs / CitaService.cs
│   └── ICatalogoService.cs / CatalogoService.cs
├── Models/        → Entidades del dominio (Cita, Paciente, Medico, EstadoCita)
├── DTOs/          → Objetos de entrada/salida con validaciones por anotaciones
├── Validation/    → Validaciones personalizadas ([FechaFutura])
├── Exceptions/    → Excepciones de negocio y GlobalExceptionHandler
├── Postman/       → Colección de pruebas funcionales
└── Program.cs     → Configuración y registro de dependencias
```

Flujo: `Controller → ICitaService / ICatalogoService → List<T>`. Los controladores no contienen lógica de negocio.

## Endpoints

| Método | Ruta | Descripción | Respuestas |
|--------|------|-------------|------------|
| GET | `/api/citas?medicoId=&fecha=` | Lista citas (filtros opcionales) | 200 |
| GET | `/api/citas/{id}` | Obtiene una cita | 200, 404 |
| POST | `/api/citas` | Registra una cita | 201, 400, 404, 409 |
| PUT | `/api/citas/{id}` | Actualiza una cita | 200, 400, 404, 409 |
| PATCH | `/api/citas/{id}/cancelar` | Cancela una cita (cambia su estado) | 200, 400, 404 |
| DELETE | `/api/citas/{id}` | Elimina una cita | 204, 404 |
| GET | `/api/medicos` | Lista médicos (datos de prueba) | 200 |
| GET | `/api/medicos/{id}` | Obtiene un médico | 200, 404 |
| GET | `/api/medicos/{id}/citas` | Agenda de un médico | 200, 404 |
| GET | `/api/pacientes` | Lista pacientes (datos de prueba) | 200 |
| GET | `/api/pacientes/{id}` | Obtiene un paciente | 200, 404 |
| GET | `/api/pacientes/{id}/citas` | Historial de citas de un paciente | 200, 404 |

### Ejemplo: crear una cita

`POST /api/citas`

```json
{
  "pacienteId": 1,
  "medicoId": 2,
  "fechaHora": "2026-12-10T09:30:00",
  "motivo": "Control pediátrico"
}
```

Respuesta `201 Created`:

```json
{
  "id": 1,
  "pacienteId": 1,
  "paciente": "Ana Torres",
  "medicoId": 2,
  "medico": "Dra. María Quispe",
  "especialidad": "Pediatría",
  "fechaHora": "2026-12-10T09:30:00",
  "motivo": "Control pediátrico",
  "estado": "Pendiente"
}
```

### Ejemplo: actualizar una cita

`PUT /api/citas/1` — `estado` acepta número o texto: 0 Pendiente, 1 Confirmada, 2 Cancelada, 3 Atendida.

```json
{
  "medicoId": 2,
  "fechaHora": "2026-12-10T10:00:00",
  "motivo": "Control pediátrico",
  "estado": "Confirmada"
}
```

### Ejemplo: cancelar una cita

`PATCH /api/citas/1/cancelar` (sin cuerpo) → `200 OK` con `"estado": "Cancelada"`.

## Validaciones (anotaciones)

| Campo | Reglas |
|-------|--------|
| `pacienteId`, `medicoId` | `[Required]`, `[Range(1, int.MaxValue)]` |
| `fechaHora` | `[Required]`, `[FechaFutura]` (validación personalizada) |
| `motivo` | `[Required]`, `[StringLength(200, MinimumLength = 5)]` |
| `estado` | `[EnumDataType]` |

Reglas de negocio (en el servicio): el paciente y el médico deben existir; un médico no puede tener dos citas con menos de 30 minutos de diferencia; no se modifican citas atendidas o canceladas; no se cancela una cita ya cancelada o atendida.

## Manejo de errores

Todas las excepciones se capturan en `GlobalExceptionHandler` y responden con un formato uniforme (`ProblemDetails`, RFC 7807). Los errores de validación de las anotaciones usan el mismo formato, con el detalle por campo en `errors`.

```json
{
  "status": 404,
  "title": "Recurso no encontrado",
  "detail": "No existe una cita con id 9999.",
  "instance": "/api/citas/9999"
}
```

| Origen | HTTP |
|--------|------|
| Validación de anotaciones (automática) | 400 |
| `BusinessRuleException` | 400 |
| `NotFoundException` | 404 |
| `ConflictException` | 409 |
| Cualquier otra | 500 (mensaje genérico, detalle solo en logs) |

## Pruebas con Postman

1. Importar `Postman/CitasMedicas.postman_collection.json`.
2. Con la API en ejecución, abrir la colección → **Run collection** (ejecutar en orden).
3. La colección contiene 22 solicitudes con tests automáticos (`pm.test`) que cubren casos exitosos (200, 201, 204) y de error (400, 404, 409). La fecha de la cita se genera automáticamente (3 días a futuro).

### Capturas de pruebas

_(Las capturas de Postman están en el informe técnico, sección 8.)_

## Notas

- Los datos se reinician cada vez que se detiene la aplicación.
- Datos precargados: pacientes 1–3 y médicos 1–3.
