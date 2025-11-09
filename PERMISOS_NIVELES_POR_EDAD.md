# Sistema de Filtrado de Participantes por Edad en Niveles

## 📋 Descripción General

El sistema de niveles ahora incluye funcionalidad de **filtrado automático por edad**, permitiendo que los voluntarios solo vean y asignen participantes cuya edad está dentro del rango definido para cada nivel.

---

## 🎯 Características Principales

### 1. **Rango de Edad por Nivel**
Cada nivel tiene dos campos:
- **EdadMinima**: Edad mínima en años (decimal 3,1)
- **EdadMaxima**: Edad máxima en años (decimal 3,1)

**Rango válido**: 0.0 a 12.0 años

**Validaciones**:
- EdadMinima debe ser >= 0.0
- EdadMaxima debe ser <= 12.0
- EdadMinima debe ser < EdadMaxima

### 2. **Cálculo Automático de Edad**
El sistema calcula la edad de cada participante basándose en su `FechaDeNacimiento`:

```csharp
var hoy = DateTime.Today;
var edad = hoy.Year - fechaNacimiento.Year;

// Ajustar si el cumpleaños no ha ocurrido este año
if (fechaNacimiento.Date > hoy.AddYears(-edad))
    edad--;
```

### 3. **Filtrado Inteligente**
El endpoint `/api/niveles/{nivelId}/participantes-elegibles` aplica múltiples filtros:

✅ Solo usuarios con rol **Participante** (RolId = 2)  
✅ Solo participantes **activos** (EstaActivo = true)  
✅ Solo participantes con **FechaDeNacimiento registrada**  
✅ Solo participantes cuya **edad está dentro del rango** del nivel

---

## 🔄 Flujo de Trabajo

### Escenario: Voluntario asigna participantes a su nivel

```
┌─────────────────────────────────────────────────────┐
│ 1. Voluntario crea Nivel                           │
│    - Nombre: "Nivel 3 - Preescolares"              │
│    - Rango: 3 a 5 años                             │
└────────────────┬────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────┐
│ 2. API valida el rango de edad                     │
│    ✓ EdadMinima (3) < EdadMaxima (5)               │
│    ✓ Rango dentro de 0-12 años                     │
└────────────────┬────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────┐
│ 3. Frontend solicita participantes elegibles       │
│    GET /api/niveles/1/participantes-elegibles       │
└────────────────┬────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────┐
│ 4. API filtra participantes por edad               │
│    Base de datos:                                   │
│    • María (2 años) ❌ Muy joven                   │
│    • Juan (3 años) ✅ Elegible                     │
│    • Sofía (4 años) ✅ Elegible                    │
│    • Pedro (5 años) ✅ Elegible                    │
│    • Ana (6 años) ❌ Muy mayor                     │
└────────────────┬────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────┐
│ 5. Frontend muestra lista filtrada                 │
│    Participantes elegibles:                         │
│    □ Juan (3 años)                                  │
│    □ Sofía (4 años)                                 │
│    □ Pedro (5 años)                                 │
└────────────────┬────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────┐
│ 6. Voluntario selecciona y asigna                  │
│    POST /api/niveles/1/participantes                │
│    { "usuarioId": 3 } // Juan                       │
└─────────────────────────────────────────────────────┘
```

---

## 📊 Ejemplos Prácticos

### Ejemplo 1: Nivel para Bebés

**Configuración del Nivel**
```json
{
  "nombre": "Nivel 1 - Bebés",
  "descripcion": "Estimulación temprana para bebés",
  "edadMinima": 0.0,
  "edadMaxima": 2.0,
  "voluntarioId": 5
}
```

**Base de Datos de Participantes**

| Nombre    | Fecha Nacimiento | Edad Calculada | ¿Elegible? |
|-----------|------------------|----------------|------------|
| Emma      | 2024-06-15       | 0 años         | ✅ Sí      |
| Lucas     | 2023-03-20       | 1 año          | ✅ Sí      |
| Isabella  | 2022-09-10       | 2 años         | ✅ Sí      |
| Mateo     | 2021-01-25       | 3 años         | ❌ No      |

**Resultado del Filtrado**: 3 participantes elegibles (Emma, Lucas, Isabella)

---

### Ejemplo 2: Nivel para Escolares

**Configuración del Nivel**
```json
{
  "nombre": "Nivel 4 - Escolares",
  "descripcion": "Educación primaria",
  "edadMinima": 6.0,
  "edadMaxima": 10.0,
  "voluntarioId": 7
}
```

**Base de Datos de Participantes**

| Nombre      | Fecha Nacimiento | Edad Calculada | ¿Elegible? |
|-------------|------------------|----------------|------------|
| Valentina   | 2019-11-05       | 5 años         | ❌ No      |
| Santiago    | 2018-04-22       | 6 años         | ✅ Sí      |
| Camila      | 2016-08-30       | 8 años         | ✅ Sí      |
| Sebastián   | 2014-12-15       | 10 años        | ✅ Sí      |
| Mariana     | 2013-02-10       | 11 años        | ❌ No      |

**Resultado del Filtrado**: 3 participantes elegibles (Santiago, Camila, Sebastián)

---

### Ejemplo 3: Nivel con Rango Decimal

**Configuración del Nivel**
```json
{
  "nombre": "Nivel 2.5 - Transición",
  "descripcion": "Periodo de transición entre niveles",
  "edadMinima": 2.5,
  "edadMaxima": 3.5,
  "voluntarioId": 9
}
```

**Cálculo de Edad**:
- Participante nacido el **2022-01-01**
- Fecha actual: **2025-07-01**
- Edad: **3 años y 6 meses** (3.5 años en decimal)
- ✅ **Elegible** (3.5 está entre 2.5 y 3.5)

---

## 🔐 Permisos y Roles

### Rol: Voluntario (Tutor del Nivel)
**Puede:**
- ✅ Crear niveles con rangos de edad
- ✅ Ver participantes elegibles para sus niveles
- ✅ Asignar/remover participantes de sus niveles
- ✅ Actualizar rangos de edad de sus niveles

**No puede:**
- ❌ Ver participantes fuera del rango de edad
- ❌ Asignar participantes a niveles de otros voluntarios

### Rol: Coordinador
**Puede:**
- ✅ Crear niveles y asignar voluntarios
- ✅ Ver todos los niveles y participantes
- ✅ Cambiar rangos de edad de cualquier nivel
- ✅ Asignar/remover participantes de cualquier nivel

### Rol: Participante
**Puede:**
- ✅ Ver los niveles en los que está asignado
- ❌ No tiene acceso a endpoints de gestión de niveles

---

## 📡 Endpoints de la API

### 1. **Crear Nivel con Rango de Edad**
```http
POST /api/niveles
Authorization: Bearer {token}
Content-Type: application/json

{
  "nombre": "Nivel 3 - Preescolares",
  "descripcion": "Nivel para edad preescolar",
  "edadMinima": 3.0,
  "edadMaxima": 5.0,
  "voluntarioId": 2
}
```

**Respuesta (201 Created)**:
```json
{
  "id": 1,
  "nombre": "Nivel 3 - Preescolares",
  "descripcion": "Nivel para edad preescolar",
  "edadMinima": 3.0,
  "edadMaxima": 5.0,
  "voluntarioId": 2,
  "nombreVoluntario": "Carlos Mendez",
  "creadoEn": "2025-01-28T01:30:00Z",
  "estaActivo": true,
  "cantidadParticipantes": 0
}
```

---

### 2. **Obtener Participantes Elegibles por Edad**
```http
GET /api/niveles/{nivelId}/participantes-elegibles
Authorization: Bearer {token}
```

**Respuesta (200 OK)**:
```json
[
  {
    "id": 10,
    "primerNombre": "Juan",
    "apellido": "Pérez",
    "nombreCompleto": "Juan Pérez",
    "email": "juan.perez@example.com",
    "nombreDeUsuario": "juanp",
    "rolId": 2,
    "nombreRol": "Participante",
    "fechaDeNacimiento": "2020-03-15T00:00:00Z",
    "edad": 4,
    "estaActivo": true,
    "creadoEn": "2024-01-10T00:00:00Z"
  },
  {
    "id": 11,
    "primerNombre": "María",
    "apellido": "González",
    "nombreCompleto": "María González",
    "email": "maria.gonzalez@example.com",
    "nombreDeUsuario": "mariag",
    "rolId": 2,
    "nombreRol": "Participante",
    "fechaDeNacimiento": "2019-11-20T00:00:00Z",
    "edad": 5,
    "estaActivo": true,
    "creadoEn": "2024-01-10T00:00:00Z"
  }
]
```

---

### 3. **Actualizar Rango de Edad de un Nivel**
```http
PUT /api/niveles/{nivelId}
Authorization: Bearer {token}
Content-Type: application/json

{
  "edadMinima": 3.5,
  "edadMaxima": 6.0
}
```

**Respuesta (200 OK)**:
```json
{
  "id": 1,
  "nombre": "Nivel 3 - Preescolares",
  "descripcion": "Nivel para edad preescolar",
  "edadMinima": 3.5,
  "edadMaxima": 6.0,
  "voluntarioId": 2,
  "nombreVoluntario": "Carlos Mendez",
  "creadoEn": "2025-01-28T01:30:00Z",
  "estaActivo": true,
  "cantidadParticipantes": 3
}
```

---

## ⚠️ Validaciones y Errores

### Error 1: Rango de Edad Inválido
**Solicitud**:
```json
{
  "nombre": "Nivel Inválido",
  "edadMinima": 8.0,
  "edadMaxima": 5.0,  // ❌ Menor que EdadMinima
  "voluntarioId": 2
}
```

**Respuesta (400 Bad Request)**:
```json
{
  "message": "El rango de edad debe estar entre 0 y 12 años, y la edad mínima debe ser menor que la máxima"
}
```

---

### Error 2: Edad Fuera de Rango Permitido
**Solicitud**:
```json
{
  "nombre": "Nivel Fuera de Rango",
  "edadMinima": 0.0,
  "edadMaxima": 15.0,  // ❌ Mayor a 12.0
  "voluntarioId": 2
}
```

**Respuesta (400 Bad Request)**:
```json
{
  "message": "El rango de edad debe estar entre 0 y 12 años, y la edad mínima debe ser menor que la máxima"
}
```

---

### Error 3: Nivel No Encontrado
**Solicitud**:
```http
GET /api/niveles/999/participantes-elegibles
```

**Respuesta (404 Not Found)**:
```json
{
  "message": "Nivel con ID 999 no encontrado"
}
```

---

## 🗃️ Estructura de Base de Datos

### Tabla: Niveles

| Columna        | Tipo           | Descripción                          |
|----------------|----------------|--------------------------------------|
| Id             | int            | Clave primaria                       |
| Nombre         | varchar(100)   | Nombre del nivel                     |
| Descripcion    | varchar(500)   | Descripción del nivel                |
| **EdadMinima** | **decimal(3,1)** | **Edad mínima permitida (0.0-12.0)** |
| **EdadMaxima** | **decimal(3,1)** | **Edad máxima permitida (0.0-12.0)** |
| IdVoluntario   | int            | FK a Usuarios (voluntario tutor)     |
| CreadoEn       | timestamp      | Fecha de creación                    |
| EstaActivo     | bool           | Estado activo/inactivo               |

**Índices**:
- `IX_Niveles_EdadMinima` (mejora búsquedas por edad mínima)
- `IX_Niveles_EdadMaxima` (mejora búsquedas por edad máxima)

---

## 🚀 Casos de Uso Avanzados

### Caso 1: Ajuste Automático de Participantes
Si un participante cumple años y sale del rango de edad del nivel:
- El sistema **NO lo remueve automáticamente**
- El voluntario puede ver que el participante ya no está en el rango en la lista de participantes elegibles
- El voluntario puede reasignar manualmente al participante a un nivel apropiado

### Caso 2: Niveles Superpuestos
Es posible crear niveles con rangos superpuestos:
- **Nivel A**: 3-6 años
- **Nivel B**: 5-8 años
- **Participante de 5.5 años**: Elegible para ambos niveles

### Caso 3: Transición entre Niveles
**Flujo recomendado**:
1. Participante cumple años y sale del rango del nivel actual
2. Coordinador/Voluntario consulta participantes elegibles para el siguiente nivel
3. Asigna al participante al nuevo nivel
4. Opcionalmente, desactiva la asignación al nivel anterior

---

## 🎓 Beneficios del Sistema

✅ **Automatización**: Filtrado automático sin intervención manual  
✅ **Precisión**: Cálculo exacto de edad considerando fechas  
✅ **Flexibilidad**: Rangos decimales para mayor precisión  
✅ **Rendimiento**: Índices en columnas de edad para búsquedas rápidas  
✅ **Escalabilidad**: Manejo eficiente de grandes cantidades de participantes  
✅ **Integridad**: Validaciones estrictas de rangos de edad  

---

## 📝 Notas de Implementación

1. **Precisión Decimal**: Los rangos admiten un decimal (3.5 = 3 años y 6 meses)
2. **Tipo de Dato**: `decimal(3,1)` soporta valores de 0.0 a 99.9
3. **Rango Práctico**: Limitado a 0.0-12.0 años por validación de negocio
4. **Índices de Base de Datos**: Mejoran rendimiento en búsquedas por edad
5. **Cálculo de Edad**: Se recalcula en cada consulta para precisión en tiempo real
6. **Zona Horaria**: Fechas se manejan en UTC para consistencia global

---

## 🔗 Archivos Relacionados

- **Modelo**: `Models/Nivel.cs`
- **DTOs**: `DTOs/NivelesDto.cs`
- **Servicio**: `Services/ServicioDeNivel.cs`
- **Interfaz**: `Services/InterfazDeServicioDeNivel.cs`
- **Controlador**: `Controllers/ControladorDeNiveles.cs`
- **DbContext**: `Data/ApiPGContext.cs`
- **Migración**: `Migrations/[timestamp]_AgregarRangoDeEdadANiveles.cs`
- **Tests HTTP**: `Niveles.http`

---

## 📞 Soporte

Para más información sobre el sistema de niveles y filtrado por edad, consulte:
- Documentación de Actividades Montessori: `PERMISOS_ACTIVIDADES_MONTESSORI.md`
- Documentación de API: `README.md`
- Casos de prueba: `Niveles.http`
