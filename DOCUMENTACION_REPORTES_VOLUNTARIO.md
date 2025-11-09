# Sistema de Reportes para Voluntarios

## Descripción General

El sistema de reportes para voluntarios les permite ver estadísticas y datos **únicamente de su nivel asignado**. A diferencia de los reportes de facilitadores que ven todo el sistema, los voluntarios tienen una vista enfocada y segura de solo su nivel.

## Características Principales

### 🔒 Seguridad
- **Cada voluntario solo ve datos de SU nivel**
- No requiere validación de rol específico (cualquier usuario autenticado)
- El sistema automáticamente busca el nivel donde el usuario es voluntario
- Si no es voluntario de ningún nivel, retorna 404

### 📊 Información Disponible
1. **Información del Nivel**: Nombre, descripción, rango de edad, total de participantes
2. **Estadísticas de Asistencia**: Hoy, esta semana, este mes, últimos 7 días
3. **Lista de Participantes**: Con estadísticas individuales de asistencia
4. **Asistencias Detalladas**: Historial completo con filtros

---

## Endpoints Disponibles

### 1. GET `/api/reportesvoluntario/mi-reporte` - Reporte Completo

Obtiene todo en una sola llamada: nivel, estadísticas de asistencia y participantes.

**Query Parameters (opcionales):**
- `fechaInicio` (DateTime): Para filtrar datos
- `fechaFin` (DateTime): Para filtrar datos

**Respuesta:**
```json
{
  "miNivel": {
    "idNivel": 2,
    "nombreNivel": "Nivel 2 (4-5 años)",
    "descripcion": "Nivel para niños de 4 a 5 años",
    "edadMinima": 4.0,
    "edadMaxima": 5.0,
    "totalParticipantes": 12,
    "participantesActivos": 11
  },
  "asistencia": {
    "totalRegistrosHoy": 10,
    "presentesHoy": 9,
    "ausentesHoy": 1,
    "porcentajeAsistenciaHoy": 90.0,
    "totalRegistrosEstaSemana": 48,
    "presentesEstaSemana": 43,
    "porcentajeAsistenciaEstaSemana": 89.6,
    "totalRegistrosEsteMes": 198,
    "presentesEsteMes": 175,
    "porcentajeAsistenciaEsteMes": 88.4,
    "ultimosDias": [...]
  },
  "participantes": [...],
  "fechaGeneracion": "2025-11-04T15:30:00Z"
}
```

**Uso recomendado:** Dashboard principal del voluntario.

---

### 2. GET `/api/reportesvoluntario/estadisticas-asistencia` - Solo Estadísticas

Obtiene únicamente las estadísticas de asistencia del nivel.

**Respuesta:**
```json
{
  "totalRegistrosHoy": 10,
  "presentesHoy": 9,
  "ausentesHoy": 1,
  "porcentajeAsistenciaHoy": 90.0,
  "totalRegistrosEstaSemana": 48,
  "presentesEstaSemana": 43,
  "porcentajeAsistenciaEstaSemana": 89.6,
  "totalRegistrosEsteMes": 198,
  "presentesEsteMes": 175,
  "porcentajeAsistenciaEsteMes": 88.4,
  "ultimosDias": [
    {
      "fecha": "2025-11-04",
      "totalRegistros": 10,
      "presentes": 9,
      "ausentes": 1,
      "porcentajeAsistencia": 90.0
    }
  ]
}
```

**Uso recomendado:** Widget de estadísticas rápidas.

---

### 3. GET `/api/reportesvoluntario/mis-participantes` - Lista de Participantes

Obtiene todos los participantes del nivel con sus estadísticas individuales.

**Respuesta:**
```json
[
  {
    "idUsuario": 5,
    "nombreCompleto": "Ana María López",
    "edad": 4,
    "estaActivo": true,
    "totalAsistencias": 18,
    "totalPresencias": 16,
    "totalAusencias": 2,
    "porcentajeAsistencia": 88.9,
    "ultimaAsistencia": "2025-11-04T00:00:00Z"
  }
]
```

**Métricas por participante:**
- **totalAsistencias**: Total de registros de asistencia
- **totalPresencias**: Veces que estuvo presente
- **totalAusencias**: Veces que estuvo ausente
- **porcentajeAsistencia**: % de presencias sobre total
- **ultimaAsistencia**: Última fecha registrada

**Uso recomendado:** Lista de participantes con alertas de baja asistencia.

---

### 4. GET `/api/reportesvoluntario/asistencias-detalladas` - Historial Detallado

Obtiene el historial completo de asistencias con filtros opcionales.

**Query Parameters (opcionales):**
- `fechaInicio` (DateTime): Filtrar desde fecha
- `fechaFin` (DateTime): Filtrar hasta fecha
- `idParticipante` (int): Filtrar por participante específico

**Respuesta:**
```json
[
  {
    "idAsistencia": 123,
    "idParticipante": 5,
    "nombreParticipante": "Ana María López",
    "fecha": "2025-11-04T00:00:00Z",
    "presente": true,
    "observaciones": "Llegó puntual",
    "creadoEn": "2025-11-04T08:15:00Z"
  }
]
```

**Uso recomendado:** Tabla de historial, reportes detallados, búsqueda de registros.

---

## Ejemplos de Integración Frontend

### Dashboard del Voluntario

```typescript
// Angular Component
export class VoluntarioDashboardComponent implements OnInit {
  reporte: ReporteVoluntarioDto;

  constructor(private reportesService: ReportesVoluntarioService) {}

  async ngOnInit() {
    this.reporte = await this.reportesService.obtenerMiReporte().toPromise();
    
    // Mostrar información
    console.log('Mi nivel:', this.reporte.miNivel.nombreNivel);
    console.log('Participantes:', this.reporte.miNivel.totalParticipantes);
    console.log('Asistencia hoy:', this.reporte.asistencia.porcentajeAsistenciaHoy + '%');
  }
}
```

### Widget de Asistencia Rápida

```typescript
async cargarEstadisticas() {
  const stats = await this.reportesService.obtenerEstadisticasAsistencia().toPromise();
  
  this.presentesHoy = stats.presentesHoy;
  this.ausentesHoy = stats.ausentesHoy;
  this.porcentajeHoy = stats.porcentajeAsistenciaHoy;
  
  // Crear gráfica de últimos 7 días
  this.crearGraficaSemanal(stats.ultimosDias);
}
```

### Tabla de Participantes con Alertas

```html
<div class="participantes-list">
  <div *ngFor="let p of participantes" class="participante-card">
    <h3>{{ p.nombreCompleto }} ({{ p.edad }} años)</h3>
    
    <div class="estadisticas">
      <span>Asistencia: {{ p.porcentajeAsistencia }}%</span>
      <span>Presencias: {{ p.totalPresencias }}/{{ p.totalAsistencias }}</span>
    </div>
    
    <!-- Alerta si asistencia < 80% -->
    <div *ngIf="p.porcentajeAsistencia < 80" class="alerta-baja-asistencia">
      ⚠️ Asistencia baja - Considerar seguimiento
    </div>
    
    <button (click)="verDetalle(p.idUsuario)">Ver Historial</button>
  </div>
</div>
```

### Historial con Filtros

```typescript
async verHistorialParticipante(idParticipante: number) {
  const filtros: FiltrosReporteVoluntarioDto = {
    idParticipante: idParticipante,
    fechaInicio: this.fechaInicio,
    fechaFin: this.fechaFin
  };
  
  const historial = await this.reportesService
    .obtenerAsistenciasDetalladas(filtros)
    .toPromise();
  
  this.mostrarHistorial(historial);
}
```

---

## Comparación: Voluntario vs Facilitador

| Característica | Voluntario | Facilitador |
|---------------|-----------|-------------|
| **Alcance** | Solo su nivel | Todo el sistema |
| **Rol requerido** | No (cualquier usuario) | Sí (IdRol = 3) |
| **Datos visibles** | Solo participantes de su nivel | Todos los usuarios |
| **Estadísticas** | De su nivel únicamente | De todos los niveles |
| **Seguridad** | Automática por nivel asignado | Por validación de rol |

---

## Casos de Uso

### 1. Tomar Asistencia Diaria
El voluntario:
1. Entra a su dashboard
2. Ve la lista de sus participantes
3. Marca presente/ausente para cada uno
4. Ve estadísticas actualizadas en tiempo real

### 2. Identificar Participantes con Baja Asistencia
El voluntario:
1. Va a "Mis Participantes"
2. Ve la lista ordenada por porcentaje de asistencia
3. Identifica participantes con < 80% asistencia
4. Toma acciones de seguimiento

### 3. Generar Reporte Mensual
El voluntario:
1. Va a "Asistencias Detalladas"
2. Filtra por el mes actual
3. Exporta los datos
4. Comparte con el facilitador o coordinador

### 4. Revisar Historial de un Participante
El voluntario:
1. Selecciona un participante
2. Filtra por fechas específicas
3. Ve todas las asistencias con observaciones
4. Identifica patrones (ausencias los lunes, etc.)

---

## Mejores Prácticas

### 1. Actualización en Tiempo Real
```typescript
// Recargar estadísticas cada 5 minutos
setInterval(() => {
  this.cargarEstadisticas();
}, 5 * 60 * 1000);
```

### 2. Caché Inteligente
```typescript
// Cachear lista de participantes (cambia poco)
const participantesCache = localStorage.getItem('mis-participantes');
if (participantesCache) {
  this.participantes = JSON.parse(participantesCache);
} else {
  await this.cargarParticipantes();
}
```

### 3. Alertas Visuales
```typescript
mostrarAlertas(participantes: ParticipanteDelNivelDto[]) {
  const bajaAsistencia = participantes.filter(p => p.porcentajeAsistencia < 80);
  
  if (bajaAsistencia.length > 0) {
    this.toastr.warning(
      `${bajaAsistencia.length} participante(s) con asistencia baja`,
      'Atención Requerida'
    );
  }
}
```

### 4. Gráficas Visuales
```typescript
crearGraficaSemanal(dias: AsistenciaDiariaVoluntarioDto[]) {
  const chart = new Chart(this.canvas, {
    type: 'line',
    data: {
      labels: dias.map(d => this.formatearFecha(d.fecha)),
      datasets: [{
        label: '% Asistencia',
        data: dias.map(d => d.porcentajeAsistencia),
        borderColor: 'rgb(75, 192, 192)',
        tension: 0.1
      }]
    },
    options: {
      responsive: true,
      plugins: {
        title: {
          display: true,
          text: 'Asistencia Últimos 7 Días'
        }
      }
    }
  });
}
```

---

## Solución de Problemas

### Error 404: "No tienes un nivel asignado"
**Causa:** El usuario no es voluntario de ningún nivel activo  
**Solución:** 
1. Verificar que el usuario esté asignado como voluntario en un nivel
2. Verificar que el nivel esté activo (EstaActivo = true)

### Error 401: "No se pudo identificar al usuario"
**Causa:** Token inválido o sin claim NameIdentifier  
**Solución:** Renovar el token mediante login

### Datos Vacíos en Participantes
**Causa:** El nivel no tiene participantes asignados  
**Solución:** Asignar participantes al nivel en el sistema

### Estadísticas en Cero
**Causa:** No se han registrado asistencias aún  
**Solución:** Comenzar a registrar asistencias diarias

---

## Conclusión

El sistema de reportes para voluntarios proporciona una vista clara, segura y enfocada de la información que necesitan para su trabajo diario. Al limitar el acceso solo a su nivel, garantizamos privacidad y simplicidad, mientras les damos todas las herramientas necesarias para monitorear y apoyar a sus participantes.

---

## Próximos Pasos

1. ✅ Implementar endpoints backend
2. ⬜ Crear servicio Angular para reportes voluntario
3. ⬜ Diseñar dashboard del voluntario
4. ⬜ Implementar gráficas de asistencia
5. ⬜ Agregar sistema de alertas para baja asistencia
6. ⬜ Implementar exportación a PDF/Excel
