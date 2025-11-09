# Guía Rápida: Sistema de Reportes

## ¿Qué acabamos de crear?

Un sistema completo de reportes para que los **facilitadores** puedan ver estadísticas detalladas de todo el sistema en un solo lugar.

## Endpoints Creados

1. **`GET /api/reportes/general`** - Reporte completo con todo
2. **`GET /api/reportes/usuarios`** - Resumen de voluntarios y participantes
3. **`GET /api/reportes/actividades`** - Resumen de actividades regulares y Montessori
4. **`GET /api/reportes/niveles`** - Resumen de niveles con participantes
5. **`GET /api/reportes/asistencia`** - Resumen de asistencia (hoy, mes, año, últimos 7 días)
6. **`GET /api/reportes/tareas`** - Resumen de tareas y rendimiento de voluntarios

## ¿Qué muestran?

### Usuarios
- Total de usuarios, voluntarios, participantes, coordinadores
- Cuántos están activos/inactivos
- Desglose por cada rol con porcentajes

### Actividades
- Total de actividades (regulares + Montessori)
- Cuántas están activas/inactivas
- Logros Montessori obtenidos (con porcentaje)
- Distribución por área pedagógica (Vida Práctica, Sensorial, etc.)

### Niveles
- Total de niveles activos
- Participantes asignados a cada nivel
- Promedio de participantes por nivel
- Voluntario asignado a cada nivel

### Asistencia
- Registros de hoy, este mes, este año
- Porcentaje de asistencia promedio
- Últimos 7 días con presentes/ausentes cada día

### Tareas
- Total de tareas por estado (Pendiente, En Proceso, Completada, Aprobada, Rechazada)
- Porcentaje de tareas completadas
- Rendimiento de cada voluntario (cuántas tareas tiene asignadas y completadas)

## Cómo Probar

### 1. Iniciar la API

```powershell
cd c:\Users\mayer\Desktop\ApiPg\apiPg
dotnet run
```

La API debe estar corriendo en `http://localhost:5000`

### 2. Obtener un Token de Facilitador

Primero, necesitas hacer login con un usuario facilitador (IdRol = 3):

```http
POST http://localhost:5000/api/autenticacion/login
Content-Type: application/json

{
  "correo": "facilitador@ejemplo.com",
  "contrasena": "tu-contraseña"
}
```

Copia el token de la respuesta.

### 3. Probar el Reporte General

Abre el archivo `Reportes.http` en VS Code y:
1. Reemplaza `@token` con tu token real
2. Haz clic en "Send Request" sobre la línea que dice `GET {{baseUrl}}/general`

Deberías ver una respuesta JSON con todas las estadísticas.

## Archivos Creados/Modificados

### Nuevos Archivos
- ✅ `DTOs/ReportesDto.cs` - Todas las estructuras de datos para reportes
- ✅ `Services/ServicioDeReportes.cs` - Lógica de negocio para generar reportes
- ✅ `Controllers/ControladorDeReportes.cs` - 6 endpoints RESTful
- ✅ `Reportes.http` - Archivo de pruebas con ejemplos
- ✅ `DOCUMENTACION_REPORTES.md` - Documentación completa (¡léela!)

### Archivos Modificados
- ✅ `Program.cs` - Registrado `IServicioDeReportes` y `ServicioDeReportes`

## Seguridad

**IMPORTANTE:** Solo facilitadores (IdRol = 3) pueden acceder a estos endpoints.

Si intentas acceder con otro rol, obtendrás:
```
403 Forbidden: "Solo los facilitadores pueden acceder a los reportes generales"
```

## Integración con Angular

Para usar estos reportes en tu frontend Angular:

```typescript
// reportes.service.ts
export class ReportesService {
  private apiUrl = 'http://localhost:5000/api/reportes';

  constructor(private http: HttpClient) {}

  obtenerReporteGeneral(): Observable<ReporteGeneralDto> {
    return this.http.get<ReporteGeneralDto>(`${this.apiUrl}/general`);
  }

  obtenerResumenUsuarios(): Observable<ResumenUsuariosDto> {
    return this.http.get<ResumenUsuariosDto>(`${this.apiUrl}/usuarios`);
  }

  // ... otros métodos
}

// dashboard.component.ts
export class DashboardComponent implements OnInit {
  reporte: ReporteGeneralDto;

  constructor(private reportesService: ReportesService) {}

  async ngOnInit() {
    this.reporte = await this.reportesService.obtenerReporteGeneral().toPromise();
    console.log('Total usuarios:', this.reporte.usuarios.totalUsuarios);
    console.log('Total actividades:', this.reporte.actividades.totalActividades);
    console.log('Asistencia promedio:', this.reporte.asistencia.porcentajeAsistenciaPromedio);
  }
}
```

## Próximos Pasos

1. ✅ **Iniciar la API** (`dotnet run`)
2. ✅ **Probar endpoints** con `Reportes.http`
3. ⬜ **Verificar que hay datos** en tu base de datos
4. ⬜ **Crear interfaz en Angular** para mostrar los reportes
5. ⬜ **Agregar gráficas** (Chart.js, ngx-charts, etc.)

## Solución de Problemas

### "ERR_CONNECTION_REFUSED"
**Problema:** La API no está corriendo  
**Solución:** Ejecuta `dotnet run` en la terminal

### "401 Unauthorized"
**Problema:** Token inválido o expirado  
**Solución:** Haz login nuevamente y obtén un nuevo token

### "403 Forbidden"
**Problema:** No eres facilitador  
**Solución:** Usa un usuario con IdRol = 3 (Facilitador)

### "Datos vacíos o ceros"
**Problema:** No hay información en la base de datos  
**Solución:** Agrega algunos usuarios, actividades, asistencias, etc.

## Datos de Ejemplo

Si tu base de datos está vacía, puedes crear datos de prueba:

```sql
-- Insertar algunos usuarios (esto es solo un ejemplo, ajusta según tu schema)
INSERT INTO usuarios (primer_nombre, apellido, correo, contrasena_hash, id_rol, esta_activo)
VALUES 
  ('Juan', 'Pérez', 'voluntario1@test.com', 'hash', 3, true),
  ('María', 'García', 'voluntario2@test.com', 'hash', 3, true),
  ('Pedro', 'López', 'participante1@test.com', 'hash', 2, true);
```

## Conclusión

¡El sistema de reportes está listo! Ahora los facilitadores pueden:
- Ver estadísticas completas del sistema
- Monitorear el progreso de actividades
- Evaluar el rendimiento de voluntarios
- Analizar patrones de asistencia
- Tomar decisiones basadas en datos reales

Lee `DOCUMENTACION_REPORTES.md` para más detalles técnicos y ejemplos de uso.
