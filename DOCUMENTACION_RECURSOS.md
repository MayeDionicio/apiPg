# 📋 Sistema de Gestión de Recursos - Guía de Implementación

## 🎯 Resumen del Sistema

El sistema gestiona **recursos** (materiales, témperas, herramientas) que se **asignan** a **voluntarios** y permite **rastrear su uso** e **incidentes**.

### Flujo Principal:
1. **cordinador** crea recursos → 2. **cordinador** asigna a voluntario → 3. **Voluntario** confirma → 4. **Voluntario** usa y reporta → 5. **Voluntario** devuelve

---

## 🏗️ Estructura de Datos

### Estados de Asignación:
- `Pending` - Asignado, esperando confirmación del voluntario
- `Confirmed` - Voluntario confirmó recepción
- `InUse` - Recurso en uso actualmente
- `Returned` - Devuelto correctamente
- `Cancelled` - Asignación cancelada

### Estados de Condición del Material:
- `Excellent` - Excelente
- `Good` - Bueno
- `Fair` - Regular
- `Poor` - Malo
- `Damaged` - Dañado
- `Unusable` - Inservible

### Tipos de Eventos:
- `ConfirmationOfUse` - Confirmación de uso normal
- `IncidentReport` - Reporte de incidente
- `StateObservation` - Observación del estado
- `DamageReport` - Reporte de daño
- `UsageCompletion` - Finalización de uso

---

## 🔗 ENDPOINTS - RECURSOS

### 1. **Listar Recursos**
```http
GET /api/Resources?activeOnly=true
Authorization: Bearer {token}
```

**Query Parameters:**
- `activeOnly` (opcional): `true`/`false` - Solo recursos activos

**Respuesta Exitosa (200):**
```json
[
  {
    "id": 1,
    "name": "Témperas Artesco",
    "description": "Set de témperas de 12 colores",
    "category": "Materiales de Arte",
    "quantity": 50,
    "availableQuantity": 45,
    "unit": "sets",
    "location": "Almacén A - Estante 3",
    "estimatedValue": 15.99,
    "notes": "Verificar completitud antes de asignar",
    "createdAt": "2024-12-01T10:00:00Z",
    "updatedAt": "2024-12-05T14:30:00Z",
    "isActive": true,
    "assignmentsCount": 8,
    "activeAssignmentsCount": 2
  }
]
```

### 2. **Obtener Recurso por ID**
```http
GET /api/Resources/{id}
Authorization: Bearer {token}
```

**Path Parameters:**
- `id` (requerido): ID del recurso

**Respuesta Exitosa (200):** Mismo objeto que arriba
**Respuesta Error (404):**
```json
{
  "message": "Recurso con ID 1 no encontrado"
}
```

### 3. **Crear Recurso**
```http
POST /api/Resources
Authorization: Bearer {token}
Content-Type: application/json
```

**Body (JSON):**
```json
{
  "name": "Témperas Artesco",
  "description": "Set de témperas de 12 colores para actividades artísticas",
  "category": "Materiales de Arte",
  "quantity": 50,
  "unit": "sets",
  "location": "Almacén A - Estante 3",
  "estimatedValue": 15.99,
  "notes": "Verificar que esté completo antes de asignar"
}
```

**Campos Requeridos:**
- `name` (string, max 100) - Nombre del recurso
- `category` (string, max 50) - Categoría
- `quantity` (int, min 1) - Cantidad total

**Campos Opcionales:**
- `description` (string, max 500)
- `unit` (string, max 50) - Unidad de medida
- `location` (string, max 200) - Ubicación
- `estimatedValue` (decimal) - Valor estimado
- `notes` (string, max 1000) - Notas adicionales

**Respuesta Exitosa (201):** Objeto recurso creado

### 4. **Actualizar Recurso**
```http
PUT /api/Resources/{id}
Authorization: Bearer {token}
Content-Type: application/json
```

**Body (JSON) - Todos los campos opcionales:**
```json
{
  "name": "Témperas Artesco - Actualizado",
  "description": "Set actualizado de témperas",
  "category": "Materiales de Arte",
  "quantity": 45,
  "unit": "sets",
  "location": "Almacén B - Estante 1",
  "estimatedValue": 17.99,
  "notes": "Notas actualizadas",
  "isActive": true
}
```

### 5. **Recursos por Categoría**
```http
GET /api/Resources/category/{category}
Authorization: Bearer {token}
```

**Path Parameters:**
- `category` (requerido): Nombre de la categoría (URL encoded)

**Ejemplo:** `/api/Resources/category/Materiales%20de%20Arte`

### 6. **Recursos Disponibles**
```http
GET /api/Resources/available
Authorization: Bearer {token}
```
Retorna recursos con `availableQuantity > 0`

### 7. **Recursos con Stock Bajo**
```http
GET /api/Resources/low-stock?threshold=5
Authorization: Bearer {token}
```

**Query Parameters:**
- `threshold` (opcional, default 5): Umbral de stock bajo

### 8. **Estadísticas de Recursos**
```http
GET /api/Resources/statistics
Authorization: Bearer {token}
```

**Respuesta:**
```json
{
  "totalResources": 25,
  "totalAvailable": 150,
  "totalAssigned": 50,
  "lowStockCount": 3,
  "categoriesCount": 5,
  "categoryStatistics": [
    {
      "category": "Materiales de Arte",
      "count": 10,
      "totalQuantity": 100,
      "availableQuantity": 80,
      "estimatedValue": 250.50
    }
  ]
}
```

### 9. **Obtener Categorías**
```http
GET /api/Resources/categories
Authorization: Bearer {token}
```

**Respuesta:**
```json
[
  "Materiales de Arte",
  "Herramientas",
  "Temperas",
  "Material Didáctico"
]
```

### 10. **Desactivar Recurso**
```http
DELETE /api/Resources/{id}
Authorization: Bearer {token}
```

**Respuesta (200):**
```json
{
  "message": "Recurso desactivado correctamente"
}
```

---

## 🔗 ENDPOINTS - ASIGNACIONES

### 1. **Crear Asignación**
```http
POST /api/ResourceAssignments
Authorization: Bearer {token}
Content-Type: application/json
```

**Body (JSON):**
```json
{
  "resourceId": 1,
  "volunteerId": 2,
  "quantityAssigned": 5,
  "expectedReturnDate": "2024-12-31T23:59:59Z",
  "initialNotes": "Para actividad de pintura con niños nivel básico"
}
```

**Campos Requeridos:**
- `resourceId` (int) - ID del recurso
- `volunteerId` (int) - ID del voluntario
- `quantityAssigned` (int, min 1) - Cantidad a asignar

**Campos Opcionales:**
- `expectedReturnDate` (datetime) - Fecha esperada de devolución
- `initialNotes` (string, max 1000) - Notas iniciales

**Respuesta Exitosa (201):**
```json
{
  "id": 1,
  "resourceId": 1,
  "resourceName": "Témperas Artesco",
  "resourceCategory": "Materiales de Arte",
  "volunteerId": 2,
  "volunteerName": "Juan Pérez",
  "quantityAssigned": 5,
  "status": "Pending",
  "assignedAt": "2024-12-01T10:00:00Z",
  "confirmedAt": null,
  "startedAt": null,
  "returnedAt": null,
  "expectedReturnDate": "2024-12-31T23:59:59Z",
  "initialNotes": "Para actividad de pintura con niños nivel básico",
  "volunteerNotes": null,
  "assignedByUserId": 1,
  "assignedByUserName": "Admin Sistema",
  "isActive": true,
  "usageLogsCount": 0,
  "hasIncidents": false
}
```

### 2. **Confirmar Asignación (Voluntario)**
```http
PUT /api/ResourceAssignments/{id}/confirm
Authorization: Bearer {token}
Content-Type: application/json
```

**Body (JSON):**
```json
{
  "volunteerNotes": "Materiales recibidos en buen estado. Listos para usar."
}
```

**Campos Opcionales:**
- `volunteerNotes` (string, max 1000) - Notas del voluntario

### 3. **Iniciar Uso**
```http
PUT /api/ResourceAssignments/{id}/start-use
Authorization: Bearer {token}
```

**Nota:** Solo el voluntario asignado puede iniciar el uso

### 4. **Devolver Recurso**
```http
PUT /api/ResourceAssignments/{id}/return
Authorization: Bearer {token}
```

### 5. **Mis Asignaciones**
```http
GET /api/ResourceAssignments/my-assignments
Authorization: Bearer {token}
```

### 6. **Asignaciones por Voluntario**
```http
GET /api/ResourceAssignments/volunteer/{volunteerId}
Authorization: Bearer {token}
```

### 7. **Asignaciones Pendientes**
```http
GET /api/ResourceAssignments/pending
Authorization: Bearer {token}
```

### 8. **Asignaciones Vencidas**
```http
GET /api/ResourceAssignments/overdue
Authorization: Bearer {token}
```

### 9. **Asignaciones Activas**
```http
GET /api/ResourceAssignments/active
Authorization: Bearer {token}
```

### 10. **Estadísticas de Asignaciones**
```http
GET /api/ResourceAssignments/statistics
Authorization: Bearer {token}
```

**Respuesta:**
```json
{
  "totalAssignments": 50,
  "pendingCount": 5,
  "confirmedCount": 8,
  "inUseCount": 12,
  "returnedCount": 20,
  "cancelledCount": 3,
  "overdueCount": 2
}
```

---

## 🔗 ENDPOINTS - REGISTROS DE USO E INCIDENTES

### 1. **Confirmar Uso Normal**
```http
POST /api/ResourceUsageLogs/confirm-usage
Authorization: Bearer {token}
Content-Type: application/json
```

**Body (JSON):**
```json
{
  "resourceAssignmentId": 1,
  "description": "Actividad de pintura realizada exitosamente con los niños",
  "conditionBefore": "Good",
  "conditionAfter": "Good",
  "actionsTaken": "Se utilizaron 3 sets de témperas para la actividad"
}
```

**Campos Requeridos:**
- `resourceAssignmentId` (int) - ID de la asignación

**Campos Opcionales:**
- `description` (string) - Descripción del uso
- `conditionBefore` (enum) - Estado antes del uso
- `conditionAfter` (enum) - Estado después del uso
- `actionsTaken` (string) - Acciones realizadas
- `photoUrls` (string) - URLs de fotos (separadas por comas)

### 2. **Reportar Incidente**
```http
POST /api/ResourceUsageLogs/report-incident
Authorization: Bearer {token}
Content-Type: application/json
```

**Body (JSON):**
```json
{
  "resourceAssignmentId": 1,
  "title": "Falta de color rojo en un set",
  "description": "Se detectó que uno de los sets asignados no tenía el tubo de color rojo completo",
  "conditionBefore": "Good",
  "conditionAfter": "Fair",
  "quantityAffected": 1,
  "actionsTaken": "Se utilizó otro set completo para completar la actividad",
  "recommendations": "Verificar completitud de sets antes de futuros préstamos"
}
```

**Campos Requeridos:**
- `resourceAssignmentId` (int)
- `title` (string, max 200) - Título del incidente
- `description` (string, max 2000) - Descripción detallada

**Campos Opcionales:**
- `conditionBefore/After` (enum)
- `quantityAffected` (int) - Cantidad afectada
- `actionsTaken` (string, max 1000)
- `recommendations` (string, max 1000)
- `photoUrls` (string, max 500)

### 3. **Resolver Incidente**
```http
PUT /api/ResourceUsageLogs/{id}/resolve
Authorization: Bearer {token}
Content-Type: application/json
```

**Body (JSON):**
```json
{
  "resolutionNotes": "Se reemplazó el tubo faltante y se verificó la completitud del set"
}
```

### 4. **Obtener Incidentes Sin Resolver**
```http
GET /api/ResourceUsageLogs/incidents/unresolved
Authorization: Bearer {token}
```

### 5. **Actividades Recientes**
```http
GET /api/ResourceUsageLogs/recent?take=10
Authorization: Bearer {token}
```

### 6. **Estadísticas de Uso**
```http
GET /api/ResourceUsageLogs/statistics
Authorization: Bearer {token}
```

**Respuesta:**
```json
{
  "totalLogs": 150,
  "incidentsCount": 12,
  "unresolvedIncidents": 3,
  "confirmationsCount": 89,
  "completionsCount": 76,
  "eventTypeStatistics": [
    {
      "eventType": "ConfirmationOfUse",
      "count": 89,
      "resolvedCount": 89,
      "unresolvedCount": 0
    },
    {
      "eventType": "IncidentReport", 
      "count": 12,
      "resolvedCount": 9,
      "unresolvedCount": 3
    }
  ],
  "recentActivities": 25
}
```

---

## 🔒 Autenticación

Todos los endpoints requieren autenticación Bearer Token:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## ❌ Respuestas de Error Comunes

### 400 - Bad Request
```json
{
  "message": "Error en los datos enviados",
  "errors": {
    "name": ["El campo nombre es requerido"]
  }
}
```

### 401 - Unauthorized
```json
{
  "message": "Token de autenticación requerido"
}
```

### 403 - Forbidden
```json
{
  "message": "Solo el voluntario asignado puede confirmar la asignación"
}
```

### 404 - Not Found
```json
{
  "message": "Recurso con ID 1 no encontrado"
}
```

### 500 - Internal Server Error
```json
{
  "message": "Error interno del servidor",
  "error": "Detalle técnico del error"
}
```

---

## 🎯 Casos de Uso Comunes para Frontend

### Dashboard Administrador:
1. `GET /api/Resources/statistics` - Resumen de recursos
2. `GET /api/ResourceAssignments/pending` - Asignaciones pendientes
3. `GET /api/ResourceUsageLogs/incidents/unresolved` - Incidentes por resolver
4. `GET /api/Resources/low-stock` - Recursos con stock bajo

### Panel Voluntario:
1. `GET /api/ResourceAssignments/my-assignments` - Mis asignaciones
2. `PUT /api/ResourceAssignments/{id}/confirm` - Confirmar recepción
3. `POST /api/ResourceUsageLogs/confirm-usage` - Confirmar uso
4. `POST /api/ResourceUsageLogs/report-incident` - Reportar problema

### Gestión de Inventario:
1. `GET /api/Resources/categories` - Para dropdown de categorías
2. `POST /api/Resources` - Crear nuevo recurso
3. `PUT /api/Resources/{id}` - Actualizar recurso
4. `GET /api/Resources/available` - Para asignaciones

Esta documentación cubre todos los endpoints necesarios para implementar el sistema completo. ¿Necesitas que profundice en alguna sección específica?