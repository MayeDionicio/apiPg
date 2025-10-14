# 🧪 Casos de Prueba - Sistema de Gestión de Recursos

## 📋 Checklist de Funcionalidades

### ✅ **RECURSOS - Funcionalidades Básicas**
- [ ] Crear recurso nuevo
- [ ] Listar todos los recursos
- [ ] Obtener recurso por ID
- [ ] Actualizar información de recurso
- [ ] Desactivar recurso (no eliminar)
- [ ] Activar recurso desactivado
- [ ] Filtrar recursos activos solamente
- [ ] Buscar recursos por categoría
- [ ] Obtener recursos disponibles (quantity > 0)
- [ ] Obtener recursos con stock bajo
- [ ] Ver estadísticas de recursos
- [ ] Listar todas las categorías

### ✅ **ASIGNACIONES - Flujo Completo**
- [ ] Crear asignación de recurso a voluntario
- [ ] Validar disponibilidad antes de asignar
- [ ] Voluntario confirma recepción
- [ ] Voluntario inicia uso del recurso
- [ ] Voluntario devuelve recurso
- [ ] Cancelar asignación (admin/voluntario)
- [ ] Listar asignaciones por voluntario
- [ ] Listar asignaciones por recurso
- [ ] Ver asignaciones pendientes
- [ ] Ver asignaciones vencidas
- [ ] Ver mis asignaciones (voluntario)
- [ ] Estadísticas de asignaciones

### ✅ **REGISTROS DE USO E INCIDENTES**
- [ ] Confirmar uso normal del recurso
- [ ] Reportar incidente durante uso
- [ ] Crear registro de observación
- [ ] Resolver incidente reportado
- [ ] Ver historial por asignación
- [ ] Ver historial por recurso
- [ ] Ver historial por voluntario
- [ ] Listar todos los incidentes
- [ ] Listar incidentes sin resolver
- [ ] Ver actividades recientes
- [ ] Ver estadísticas de uso

---

## 🎯 Casos de Prueba Detallados

### **CASO 1: Flujo Completo Exitoso**

**Objetivo:** Verificar el flujo completo desde creación hasta devolución

**Pasos:**
1. **Admin crea recurso**
   ```http
   POST /api/Resources
   {
     "name": "Set Témperas Test",
     "category": "Materiales de Arte",
     "quantity": 10,
     "location": "Almacén Test"
   }
   ```
   **Resultado esperado:** Recurso creado con `availableQuantity = 10`

2. **Admin asigna a voluntario**
   ```http
   POST /api/ResourceAssignments
   {
     "resourceId": 1,
     "volunteerId": 2,
     "quantityAssigned": 3,
     "expectedReturnDate": "2024-12-31T23:59:59Z"
   }
   ```
   **Resultado esperado:** 
   - Asignación creada con status `Pending`
   - `availableQuantity` del recurso = 7

3. **Voluntario confirma**
   ```http
   PUT /api/ResourceAssignments/1/confirm
   {
     "volunteerNotes": "Materiales en buen estado"
   }
   ```
   **Resultado esperado:** Status cambia a `Confirmed`

4. **Voluntario inicia uso**
   ```http
   PUT /api/ResourceAssignments/1/start-use
   ```
   **Resultado esperado:** Status cambia a `InUse`

5. **Voluntario confirma uso**
   ```http
   POST /api/ResourceUsageLogs/confirm-usage
   {
     "resourceAssignmentId": 1,
     "description": "Uso exitoso",
     "conditionAfter": "Good"
   }
   ```
   **Resultado esperado:** Log creado con `isResolved = true`

6. **Voluntario devuelve**
   ```http
   PUT /api/ResourceAssignments/1/return
   ```
   **Resultado esperado:** 
   - Status cambia a `Returned`
   - `availableQuantity` del recurso = 10

### **CASO 2: Reporte de Incidente**

**Objetivo:** Verificar flujo de incidente y resolución

**Pasos:**
1. **Crear asignación y confirmar hasta "InUse"** (igual que Caso 1, pasos 1-4)

2. **Voluntario reporta incidente**
   ```http
   POST /api/ResourceUsageLogs/report-incident
   {
     "resourceAssignmentId": 1,
     "title": "Material incompleto",
     "description": "Falta tubo de color azul",
     "conditionBefore": "Good",
     "conditionAfter": "Fair",
     "quantityAffected": 1
   }
   ```
   **Resultado esperado:** Log creado con `isResolved = false`

3. **Admin ve incidentes sin resolver**
   ```http
   GET /api/ResourceUsageLogs/incidents/unresolved
   ```
   **Resultado esperado:** Lista incluye el incidente creado

4. **Admin resuelve incidente**
   ```http
   PUT /api/ResourceUsageLogs/1/resolve
   {
     "resolutionNotes": "Tubo reemplazado"
   }
   ```
   **Resultado esperado:** 
   - `isResolved = true`
   - `resolvedAt` establecido
   - `resolutionNotes` guardadas

### **CASO 3: Validaciones de Negocio**

**Objetivo:** Verificar que las validaciones funcionen correctamente

**Pruebas:**

1. **Asignar más cantidad de la disponible**
   ```http
   POST /api/ResourceAssignments
   {
     "resourceId": 1,
     "quantityAssigned": 999  // Más de lo disponible
   }
   ```
   **Resultado esperado:** Error 400 "No hay suficiente cantidad disponible"

2. **Voluntario incorrecto trata de confirmar**
   - Login como usuario diferente al asignado
   - Intentar confirmar asignación
   **Resultado esperado:** Error 403 "Solo el voluntario asignado puede confirmar"

3. **Confirmar asignación ya procesada**
   - Asignación ya en estado "Confirmed"
   - Intentar confirmar nuevamente
   **Resultado esperado:** Error 400 "La asignación ya ha sido procesada"

4. **Crear recurso con datos inválidos**
   ```http
   POST /api/Resources
   {
     "name": "",  // Vacío
     "quantity": -1  // Negativo
   }
   ```
   **Resultado esperado:** Error 400 con detalles de validación

### **CASO 4: Control de Inventario**

**Objetivo:** Verificar que el control de inventario funcione correctamente

**Escenario:**
- Recurso con quantity = 10
- Crear múltiples asignaciones hasta agotar stock

**Pasos:**
1. Asignar 4 unidades → `availableQuantity = 6`
2. Asignar 3 unidades → `availableQuantity = 3`
3. Asignar 3 unidades → `availableQuantity = 0`
4. Intentar asignar 1 unidad más → Error "No hay suficiente cantidad"
5. Devolver una asignación de 3 → `availableQuantity = 3`
6. Ahora sí se puede asignar hasta 3 unidades

### **CASO 5: Asignaciones Vencidas**

**Objetivo:** Verificar detección de asignaciones vencidas

**Pasos:**
1. Crear asignación con `expectedReturnDate` en el pasado
2. Cambiar estado a "InUse"
3. Llamar endpoint de asignaciones vencidas
   ```http
   GET /api/ResourceAssignments/overdue
   ```
   **Resultado esperado:** La asignación aparece en la lista

### **CASO 6: Estadísticas**

**Objetivo:** Verificar que las estadísticas se calculen correctamente

**Pasos:**
1. Crear 5 recursos en diferentes categorías
2. Hacer varias asignaciones
3. Generar algunos incidentes
4. Verificar estadísticas:
   ```http
   GET /api/Resources/statistics
   GET /api/ResourceAssignments/statistics
   GET /api/ResourceUsageLogs/statistics
   ```
   **Resultado esperado:** Números coherentes con los datos creados

---

## 🔍 Pruebas de Integración

### **Secuencia de Pruebas Automáticas**

```javascript
describe('Sistema de Recursos - Integración', () => {
  
  test('Flujo completo exitoso', async () => {
    // 1. Crear recurso
    const resource = await createResource({
      name: 'Test Resource',
      quantity: 10,
      category: 'Test'
    });
    expect(resource.availableQuantity).toBe(10);
    
    // 2. Asignar recurso
    const assignment = await createAssignment({
      resourceId: resource.id,
      volunteerId: testVolunteerId,
      quantityAssigned: 3
    });
    expect(assignment.status).toBe('Pending');
    
    // 3. Verificar cantidad actualizada
    const updatedResource = await getResource(resource.id);
    expect(updatedResource.availableQuantity).toBe(7);
    
    // 4. Confirmar asignación
    const confirmed = await confirmAssignment(assignment.id, 'Test notes');
    expect(confirmed.status).toBe('Confirmed');
    
    // 5. Iniciar uso
    const inUse = await startUse(assignment.id);
    expect(inUse.status).toBe('InUse');
    
    // 6. Confirmar uso
    const usageLog = await confirmUsage({
      resourceAssignmentId: assignment.id,
      description: 'Test usage'
    });
    expect(usageLog.eventType).toBe('ConfirmationOfUse');
    
    // 7. Devolver
    const returned = await returnResource(assignment.id);
    expect(returned.status).toBe('Returned');
    
    // 8. Verificar cantidad restaurada
    const finalResource = await getResource(resource.id);
    expect(finalResource.availableQuantity).toBe(10);
  });
  
  test('Flujo con incidente', async () => {
    // Setup similar al anterior hasta "InUse"
    
    // Reportar incidente
    const incident = await reportIncident({
      resourceAssignmentId: assignment.id,
      title: 'Test incident',
      description: 'Something went wrong'
    });
    expect(incident.isResolved).toBe(false);
    
    // Verificar en incidentes sin resolver
    const unresolvedIncidents = await getUnresolvedIncidents();
    expect(unresolvedIncidents).toContain(incident);
    
    // Resolver incidente
    const resolved = await resolveIncident(incident.id, 'Fixed');
    expect(resolved.isResolved).toBe(true);
  });
});
```

---

## 📊 Métricas de Aceptación

### **Performance:**
- [ ] Endpoints responden en < 500ms
- [ ] Búsquedas con filtros en < 200ms
- [ ] Carga de dashboard en < 1s

### **Usabilidad:**
- [ ] Formularios validan en tiempo real
- [ ] Mensajes de error claros y específicos
- [ ] Estados de carga visibles

### **Seguridad:**
- [ ] Todos los endpoints requieren autenticación
- [ ] Voluntarios solo ven sus asignaciones
- [ ] Admins pueden ver todo

### **Integridad de Datos:**
- [ ] Control de inventario siempre correcto
- [ ] No se pueden asignar cantidades negativas
- [ ] Estados de asignación siguen flujo correcto
- [ ] Registros de auditoría completos

Esta guía de pruebas asegura que el sistema funcione correctamente en todos los escenarios. ¿Necesitas que agregue algún caso de prueba específico o modifique alguna validación?