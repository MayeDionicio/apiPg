# 🚀 Ejemplos de Implementación Frontend - Sistema de Recursos

## 📱 Interfaces Sugeridas

### 1. **Dashboard Administrador**
```typescript
interface DashboardData {
  resourceStats: {
    totalResources: number;
    totalAvailable: number; 
    lowStockCount: number;
  };
  assignmentStats: {
    pendingCount: number;
    overdueCount: number;
    inUseCount: number;
  };
  incidentStats: {
    unresolvedIncidents: number;
    totalIncidents: number;
  };
}
```

### 2. **Lista de Recursos**
```typescript
interface Resource {
  id: number;
  name: string;
  category: string;
  quantity: number;
  availableQuantity: number;
  location: string;
  isActive: boolean;
  assignmentsCount: number;
}
```

### 3. **Asignación de Recurso**
```typescript
interface ResourceAssignment {
  id: number;
  resourceName: string;
  volunteerName: string;
  quantityAssigned: number;
  status: 'Pending' | 'Confirmed' | 'InUse' | 'Returned' | 'Cancelled';
  assignedAt: string;
  expectedReturnDate?: string;
  hasIncidents: boolean;
}
```

---

## 🎨 Componentes Frontend Sugeridos

### **1. Crear Recurso - Modal/Formulario**
```html
<form (ngSubmit)="createResource()">
  <input 
    type="text" 
    [(ngModel)]="newResource.name" 
    placeholder="Nombre del recurso"
    required>
  
  <select [(ngModel)]="newResource.category" required>
    <option value="">Seleccionar categoría...</option>
    <option value="Materiales de Arte">Materiales de Arte</option>
    <option value="Herramientas">Herramientas</option>
    <option value="Material Didáctico">Material Didáctico</option>
  </select>
  
  <input 
    type="number" 
    [(ngModel)]="newResource.quantity" 
    placeholder="Cantidad"
    min="1"
    required>
  
  <input 
    type="text" 
    [(ngModel)]="newResource.location" 
    placeholder="Ubicación (opcional)">
  
  <textarea 
    [(ngModel)]="newResource.notes" 
    placeholder="Notas adicionales"></textarea>
  
  <button type="submit">Crear Recurso</button>
</form>
```

### **2. Asignar Recurso - Modal**
```html
<form (ngSubmit)="assignResource()">
  <select [(ngModel)]="assignment.resourceId" required>
    <option value="">Seleccionar recurso...</option>
    <option 
      *ngFor="let resource of availableResources" 
      [value]="resource.id">
      {{resource.name}} (Disponible: {{resource.availableQuantity}})
    </option>
  </select>
  
  <select [(ngModel)]="assignment.volunteerId" required>
    <option value="">Seleccionar voluntario...</option>
    <option 
      *ngFor="let volunteer of volunteers" 
      [value]="volunteer.id">
      {{volunteer.firstName}} {{volunteer.lastName}}
    </option>
  </select>
  
  <input 
    type="number" 
    [(ngModel)]="assignment.quantityAssigned" 
    placeholder="Cantidad a asignar"
    min="1"
    required>
  
  <input 
    type="date" 
    [(ngModel)]="assignment.expectedReturnDate" 
    placeholder="Fecha esperada de devolución">
  
  <textarea 
    [(ngModel)]="assignment.initialNotes" 
    placeholder="Notas para el voluntario"></textarea>
  
  <button type="submit">Asignar Recurso</button>
</form>
```

### **3. Confirmar Asignación (Vista Voluntario)**
```html
<div class="assignment-card" *ngFor="let assignment of pendingAssignments">
  <h3>{{assignment.resourceName}}</h3>
  <p>Cantidad: {{assignment.quantityAssigned}}</p>
  <p>Fecha límite: {{assignment.expectedReturnDate | date}}</p>
  <p>Notas: {{assignment.initialNotes}}</p>
  
  <form (ngSubmit)="confirmAssignment(assignment.id)">
    <textarea 
      [(ngModel)]="confirmationNotes[assignment.id]" 
      placeholder="Confirma el estado de los materiales recibidos..."></textarea>
    
    <button type="submit" class="btn-confirm">
      Confirmar Recepción
    </button>
  </form>
</div>
```

### **4. Reportar Incidente**
```html
<form (ngSubmit)="reportIncident()">
  <select [(ngModel)]="incident.resourceAssignmentId" required>
    <option value="">Seleccionar asignación...</option>
    <option 
      *ngFor="let assignment of myActiveAssignments" 
      [value]="assignment.id">
      {{assignment.resourceName}} - {{assignment.quantityAssigned}} unidades
    </option>
  </select>
  
  <input 
    type="text" 
    [(ngModel)]="incident.title" 
    placeholder="Título del incidente"
    required>
  
  <textarea 
    [(ngModel)]="incident.description" 
    placeholder="Describe qué pasó con el material..."
    required></textarea>
  
  <div class="condition-row">
    <label>Estado antes del uso:</label>
    <select [(ngModel)]="incident.conditionBefore">
      <option value="Excellent">Excelente</option>
      <option value="Good">Bueno</option>
      <option value="Fair">Regular</option>
      <option value="Poor">Malo</option>
    </select>
  </div>
  
  <div class="condition-row">
    <label>Estado actual:</label>
    <select [(ngModel)]="incident.conditionAfter">
      <option value="Excellent">Excelente</option>
      <option value="Good">Bueno</option>
      <option value="Fair">Regular</option>
      <option value="Poor">Malo</option>
      <option value="Damaged">Dañado</option>
      <option value="Unusable">Inservible</option>
    </select>
  </div>
  
  <input 
    type="number" 
    [(ngModel)]="incident.quantityAffected" 
    placeholder="Cantidad afectada (opcional)"
    min="1">
  
  <textarea 
    [(ngModel)]="incident.actionsTaken" 
    placeholder="¿Qué acciones tomaste?"></textarea>
  
  <textarea 
    [(ngModel)]="incident.recommendations" 
    placeholder="Recomendaciones para el futuro"></textarea>
  
  <button type="submit" class="btn-report">Reportar Incidente</button>
</form>
```

---

## 📊 Servicios Angular Sugeridos

### **ResourceService**
```typescript
@Injectable()
export class ResourceService {
  private apiUrl = 'https://localhost:7014/api/Resources';
  
  getResources(activeOnly: boolean = false): Observable<Resource[]> {
    const params = activeOnly ? '?activeOnly=true' : '';
    return this.http.get<Resource[]>(`${this.apiUrl}${params}`);
  }
  
  createResource(resource: CreateResourceDto): Observable<Resource> {
    return this.http.post<Resource>(this.apiUrl, resource);
  }
  
  getAvailableResources(): Observable<Resource[]> {
    return this.http.get<Resource[]>(`${this.apiUrl}/available`);
  }
  
  getResourceStatistics(): Observable<any> {
    return this.http.get(`${this.apiUrl}/statistics`);
  }
  
  getCategories(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/categories`);
  }
}
```

### **ResourceAssignmentService**
```typescript
@Injectable()
export class ResourceAssignmentService {
  private apiUrl = 'https://localhost:7014/api/ResourceAssignments';
  
  createAssignment(assignment: CreateResourceAssignmentDto): Observable<ResourceAssignment> {
    return this.http.post<ResourceAssignment>(this.apiUrl, assignment);
  }
  
  getMyAssignments(): Observable<ResourceAssignment[]> {
    return this.http.get<ResourceAssignment[]>(`${this.apiUrl}/my-assignments`);
  }
  
  confirmAssignment(id: number, notes: string): Observable<ResourceAssignment> {
    return this.http.put<ResourceAssignment>(`${this.apiUrl}/${id}/confirm`, {
      volunteerNotes: notes
    });
  }
  
  startUse(id: number): Observable<ResourceAssignment> {
    return this.http.put<ResourceAssignment>(`${this.apiUrl}/${id}/start-use`, {});
  }
  
  returnResource(id: number): Observable<ResourceAssignment> {
    return this.http.put<ResourceAssignment>(`${this.apiUrl}/${id}/return`, {});
  }
  
  getPendingAssignments(): Observable<ResourceAssignment[]> {
    return this.http.get<ResourceAssignment[]>(`${this.apiUrl}/pending`);
  }
  
  getOverdueAssignments(): Observable<ResourceAssignment[]> {
    return this.http.get<ResourceAssignment[]>(`${this.apiUrl}/overdue`);
  }
}
```

### **ResourceUsageLogService**
```typescript
@Injectable()
export class ResourceUsageLogService {
  private apiUrl = 'https://localhost:7014/api/ResourceUsageLogs';
  
  confirmUsage(data: ConfirmUsageDto): Observable<ResourceUsageLog> {
    return this.http.post<ResourceUsageLog>(`${this.apiUrl}/confirm-usage`, data);
  }
  
  reportIncident(incident: ReportIncidentDto): Observable<ResourceUsageLog> {
    return this.http.post<ResourceUsageLog>(`${this.apiUrl}/report-incident`, incident);
  }
  
  getUnresolvedIncidents(): Observable<ResourceUsageLog[]> {
    return this.http.get<ResourceUsageLog[]>(`${this.apiUrl}/incidents/unresolved`);
  }
  
  resolveIncident(id: number, notes: string): Observable<ResourceUsageLog> {
    return this.http.put<ResourceUsageLog>(`${this.apiUrl}/${id}/resolve`, {
      resolutionNotes: notes
    });
  }
  
  getRecentActivities(take: number = 20): Observable<ResourceUsageLog[]> {
    return this.http.get<ResourceUsageLog[]>(`${this.apiUrl}/recent?take=${take}`);
  }
}
```

---

## 🎯 Flujos de Trabajo Frontend

### **Flujo 1: Admin Crea y Asigna Recurso**
```typescript
// 1. Admin crea recurso
createResource() {
  this.resourceService.createResource(this.newResource).subscribe(
    resource => {
      this.showSuccess('Recurso creado exitosamente');
      this.loadResources();
    }
  );
}

// 2. Admin asigna a voluntario
assignResource() {
  this.assignmentService.createAssignment(this.assignment).subscribe(
    assignment => {
      this.showSuccess('Recurso asignado exitosamente');
      this.loadAssignments();
    }
  );
}
```

### **Flujo 2: Voluntario Confirma y Usa**
```typescript
// 1. Voluntario ve asignaciones pendientes
loadMyAssignments() {
  this.assignmentService.getMyAssignments().subscribe(
    assignments => {
      this.pendingAssignments = assignments.filter(a => a.status === 'Pending');
      this.confirmedAssignments = assignments.filter(a => a.status === 'Confirmed');
      this.inUseAssignments = assignments.filter(a => a.status === 'InUse');
    }
  );
}

// 2. Voluntario confirma recepción
confirmAssignment(assignmentId: number) {
  const notes = this.confirmationNotes[assignmentId];
  this.assignmentService.confirmAssignment(assignmentId, notes).subscribe(
    assignment => {
      this.showSuccess('Asignación confirmada');
      this.loadMyAssignments();
    }
  );
}

// 3. Voluntario inicia uso
startUse(assignmentId: number) {
  this.assignmentService.startUse(assignmentId).subscribe(
    assignment => {
      this.showSuccess('Uso iniciado');
      this.loadMyAssignments();
    }
  );
}

// 4. Voluntario confirma uso normal O reporta incidente
confirmUsage(assignmentId: number) {
  const usageData = {
    resourceAssignmentId: assignmentId,
    description: this.usageDescription,
    conditionBefore: 'Good',
    conditionAfter: 'Good'
  };
  
  this.usageLogService.confirmUsage(usageData).subscribe(
    log => this.showSuccess('Uso confirmado')
  );
}

reportIncident() {
  this.usageLogService.reportIncident(this.incident).subscribe(
    log => {
      this.showSuccess('Incidente reportado');
      this.clearIncidentForm();
    }
  );
}
```

### **Flujo 3: Dashboard Admin**
```typescript
loadDashboard() {
  // Cargar estadísticas
  this.resourceService.getResourceStatistics().subscribe(
    stats => this.resourceStats = stats
  );
  
  this.assignmentService.getAssignmentStatistics().subscribe(
    stats => this.assignmentStats = stats
  );
  
  // Cargar elementos que requieren atención
  this.assignmentService.getPendingAssignments().subscribe(
    assignments => this.pendingAssignments = assignments
  );
  
  this.assignmentService.getOverdueAssignments().subscribe(
    assignments => this.overdueAssignments = assignments
  );
  
  this.usageLogService.getUnresolvedIncidents().subscribe(
    incidents => this.unresolvedIncidents = incidents
  );
  
  this.resourceService.getLowStockResources().subscribe(
    resources => this.lowStockResources = resources
  );
}
```

---

## 🚨 Validaciones Frontend Sugeridas

### **Al Crear Recurso:**
- Nombre requerido y único
- Categoría requerida
- Cantidad > 0
- Valor estimado >= 0 (si se proporciona)

### **Al Asignar Recurso:**
- Recurso seleccionado debe tener cantidad disponible
- Cantidad asignada <= cantidad disponible
- Voluntario debe estar activo
- Fecha de devolución debe ser futura (si se proporciona)

### **Al Reportar Incidente:**
- Título y descripción requeridos
- Cantidad afectada <= cantidad asignada
- Solo asignaciones en estado "InUse" pueden reportar incidentes

### **Estados de Botones:**
```typescript
// Ejemplo de lógica para mostrar botones según estado
getAvailableActions(assignment: ResourceAssignment) {
  switch(assignment.status) {
    case 'Pending':
      return ['confirm', 'cancel'];
    case 'Confirmed':
      return ['start-use', 'cancel'];
    case 'InUse':
      return ['confirm-usage', 'report-incident', 'return'];
    case 'Returned':
      return ['view-logs'];
    default:
      return [];
  }
}
```

Esta documentación te da todo lo necesario para implementar el frontend del sistema. ¿Quieres que profundice en alguna parte específica o necesitas ejemplos adicionales?