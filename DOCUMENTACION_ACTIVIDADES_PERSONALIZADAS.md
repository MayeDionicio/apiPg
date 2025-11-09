# 📚 Actividades Montessori Personalizadas - Documentación

## 🎯 Objetivo

El módulo de **Actividades Montessori Personalizadas** permite crear y gestionar actividades adaptadas específicamente para un **estudiante individual**, con seguimiento de progreso, evaluación personalizada y recordatorios.

---

## 🌟 Características Principales

### ✅ Asignación Individual
- Cada actividad se asigna a **UN solo estudiante**
- Adaptaciones específicas según las necesidades del niño
- Objetivos personalizados

### ✅ Seguimiento de Progreso
- Estados: `Pendiente`, `EnProgreso`, `Completada`, `Cancelada`
- Porcentaje de progreso (0-100%)
- Duración estimada vs real

### ✅ Evaluación Detallada
- Observaciones del profesor
- Logros alcanzados (lista)
- Dificultades encontradas
- Sugerencias para seguimiento
- Nivel de desempeño: `Excelente`, `Bueno`, `Regular`, `NecesitaApoyo`
- Marcar si requiere refuerzo

### ✅ Evidencias
- URL de fotos
- URL de videos
- Notas adicionales

### ✅ Recordatorios
- Sistema de recordatorios automáticos
- Fecha personalizada para cada actividad

### ✅ Priorización
- Prioridad: `Alta`, `Normal`, `Baja`
- Ordenamiento automático por prioridad

### ✅ Actividades Recurrentes
- Frecuencia: `Diaria`, `Semanal`, `Mensual`
- Fecha de fin de recurrencia

---

## 📊 Estructura de Datos

### ActividadMontessoriPersonalizada

```csharp
{
  "id": 1,
  "idActividadMontessoriBase": 5,        // Opcional: basada en actividad existente
  "idEstudiante": 2,                     // Requerido
  "idNivel": 1,                          // Opcional
  "nombre": "Torre Rosa - Práctica",
  "areaPedagogica": "Matemáticas",
  "fechaAsignacion": "2025-11-08",
  "fechaInicio": "2025-11-09",
  "fechaCompletada": null,
  "horaActividad": "09:00:00",
  "duracionMinutosEstimada": 30,
  "duracionMinutosReal": null,
  "objetivoPersonalizado": "Desarrollar percepción visual...",
  "materialesNecesarios": "Torre Rosa Montessori...",
  "presentacionPersonalizada": "Pasos adaptados...",
  "adaptacionesEspeciales": "Reducir a 5 cubos...",
  "estado": "Pendiente",
  "progresosPorcentaje": 0,
  "observacionesProfesor": null,
  "logrosAlcanzados": [],
  "dificultadesEncontradas": null,
  "sugerenciasParaSeguimiento": null,
  "nivelDesempeno": null,
  "requiereRefuerzo": false,
  "evidenciaUrlFoto": null,
  "evidenciaUrlVideo": null,
  "prioridad": "Alta",
  "enviarRecordatorio": true,
  "fechaRecordatorio": "2025-11-09T08:30:00",
  "recordatorioEnviado": false,
  "esRecurrente": false,
  "frecuenciaRecurrencia": null
}
```

---

## 🔌 Endpoints Disponibles

### 1. **POST** `/api/ControladorDeActividadesMontessoriPersonalizadas`
Crea una nueva actividad personalizada para un estudiante.

**Requiere:** Token JWT (autenticado)

**Body:**
```json
{
  "idEstudiante": 2,
  "nombre": "Torre Rosa",
  "areaPedagogica": "Matemáticas",
  "duracionMinutosEstimada": 30,
  "objetivoPersonalizado": "Desarrollar...",
  "prioridad": "Alta"
}
```

---

### 2. **GET** `/api/ControladorDeActividadesMontessoriPersonalizadas/{id}`
Obtiene una actividad específica por ID.

---

### 3. **GET** `/api/ControladorDeActividadesMontessoriPersonalizadas/estudiante/{estudianteId}`
Obtiene todas las actividades de un estudiante.

**Query params:**
- `soloActivas`: `true` (default) | `false`

---

### 4. **POST** `/api/ControladorDeActividadesMontessoriPersonalizadas/buscar`
Búsqueda avanzada con filtros.

**Body:**
```json
{
  "idEstudiante": 2,
  "estado": "Pendiente",
  "areaPedagogica": "Matemáticas",
  "prioridad": "Alta",
  "fechaDesde": "2025-11-01",
  "fechaHasta": "2025-11-30",
  "requiereRefuerzo": false,
  "soloActivas": true,
  "pagina": 1,
  "registrosPorPagina": 20
}
```

---

### 5. **PUT** `/api/ControladorDeActividadesMontessoriPersonalizadas/{id}`
Actualiza información general de la actividad.

**Body:**
```json
{
  "nombre": "Nuevo nombre",
  "duracionMinutosEstimada": 45,
  "prioridad": "Normal"
}
```

---

### 6. **PUT** `/api/ControladorDeActividadesMontessoriPersonalizadas/{id}/progreso`
Actualiza el progreso y evaluación.

**Body:**
```json
{
  "estado": "Completada",
  "progresosPorcentaje": 100,
  "fechaCompletada": "2025-11-09T10:00:00",
  "duracionMinutosReal": 35,
  "observacionesProfesor": "Excelente trabajo",
  "logrosAlcanzados": [
    "Apiló correctamente",
    "Mostró concentración"
  ],
  "nivelDesempeno": "Excelente",
  "requiereRefuerzo": false
}
```

---

### 7. **DELETE** `/api/ControladorDeActividadesMontessoriPersonalizadas/{id}`
Elimina (desactiva) una actividad.

---

### 8. **GET** `/api/ControladorDeActividadesMontessoriPersonalizadas/estudiante/{id}/estadisticas`
Obtiene estadísticas completas del estudiante.

**Respuesta:**
```json
{
  "idEstudiante": 2,
  "nombreEstudiante": "Juan Pérez",
  "totalActividades": 15,
  "actividadesPendientes": 3,
  "actividadesEnProgreso": 2,
  "actividadesCompletadas": 9,
  "actividadesCanceladas": 1,
  "promedioProgreso": 78.5,
  "actividadesConRefuerzo": 2,
  "actividadesPorArea": {
    "Matemáticas": 6,
    "Vida Práctica": 5,
    "Lenguaje": 4
  },
  "actividadesPorNivelDesempeno": {
    "Excelente": 5,
    "Bueno": 3,
    "Regular": 1
  },
  "ultimaActividadCompletada": "2025-11-08T15:30:00",
  "totalMinutosTrabajados": 450
}
```

---

### 9. **GET** `/api/ControladorDeActividadesMontessoriPersonalizadas/pendientes`
Obtiene todas las actividades pendientes o en progreso (ordenadas por prioridad).

---

### 10. **GET** `/api/ControladorDeActividadesMontessoriPersonalizadas/recordatorios`
Obtiene actividades que requieren envío de recordatorio.

---

## 🎭 Casos de Uso

### **Caso 1: Crear Actividad Personalizada Nueva**

Un profesor quiere crear una actividad específica para María (ID: 3) que tiene dificultades con motricidad fina.

```json
POST /api/ControladorDeActividadesMontessoriPersonalizadas

{
  "idEstudiante": 3,
  "nombre": "Verter Arroz - Práctica Inicial",
  "areaPedagogica": "Vida Práctica",
  "duracionMinutosEstimada": 15,
  "objetivoPersonalizado": "Mejorar control de movimientos al verter",
  "materialesNecesarios": "2 cuencos pequeños, 100g de arroz",
  "adaptacionesEspeciales": "Usar cuencos con marcas de colores para guiar el vertido",
  "prioridad": "Alta",
  "notasAdicionales": "María mostró interés en actividades con materiales sueltos"
}
```

---

### **Caso 2: Basar Actividad en una Existente**

El profesor quiere asignar una actividad Montessori estándar pero adaptada:

```json
POST /api/ControladorDeActividadesMontessoriPersonalizadas

{
  "idActividadMontessoriBase": 5,
  "idEstudiante": 4,
  "nombre": "Torre Rosa - Versión Adaptada",
  "areaPedagogica": "Matemáticas - Geometría",
  "duracionMinutosEstimada": 25,
  "objetivoPersonalizado": "Introducir conceptos de grande/pequeño",
  "adaptacionesEspeciales": "Usar solo 5 cubos de la torre, comenzar con los más grandes",
  "prioridad": "Normal"
}
```

---

### **Caso 3: Registrar Progreso Durante la Actividad**

María comenzó su actividad:

```json
PUT /api/ControladorDeActividadesMontessoriPersonalizadas/1/progreso

{
  "estado": "EnProgreso",
  "progresosPorcentaje": 40,
  "observacionesProfesor": "María está concentrada. Ha vertido el arroz 3 veces con algunos derrames."
}
```

---

### **Caso 4: Completar y Evaluar**

María terminó la actividad:

```json
PUT /api/ControladorDeActividadesMontessoriPersonalizadas/1/progreso

{
  "estado": "Completada",
  "progresosPorcentaje": 100,
  "fechaCompletada": "2025-11-09T10:15:00",
  "duracionMinutosReal": 18,
  "observacionesProfesor": "María completó la actividad con éxito. Mostró mejora notable en el control del movimiento.",
  "logrosAlcanzados": [
    "Vertió el arroz sin derrames las últimas 3 veces",
    "Mantuvo concentración durante 15 minutos",
    "Limpió el material después de usar"
  ],
  "dificultadesEncontradas": "Al inicio derramó arroz 2 veces",
  "sugerenciasParaSeguimiento": "Continuar con actividades de verter. Próximo paso: verter líquidos coloreados",
  "nivelDesempeno": "Bueno",
  "requiereRefuerzo": false,
  "evidenciaUrlFoto": "https://drive.google.com/foto-maria-vertido.jpg"
}
```

---

### **Caso 5: Actividad que Necesita Refuerzo**

Un estudiante no logró completar satisfactoriamente:

```json
PUT /api/ControladorDeActividadesMontessoriPersonalizadas/2/progreso

{
  "estado": "Completada",
  "progresosPorcentaje": 100,
  "duracionMinutosReal": 30,
  "observacionesProfesor": "Pedro tuvo dificultades. Perdió interés a mitad de la actividad.",
  "dificultadesEncontradas": "No logró apilar más de 3 cubos. Se distrajo fácilmente.",
  "sugerenciasParaSeguimiento": "Repetir con actividades más simples primero. Considerar trabajar concentración.",
  "nivelDesempeno": "NecesitaApoyo",
  "requiereRefuerzo": true
}
```

Luego buscar todas las actividades que requieren refuerzo:

```json
POST /api/ControladorDeActividadesMontessoriPersonalizadas/buscar

{
  "requiereRefuerzo": true,
  "soloActivas": true
}
```

---

### **Caso 6: Actividades Recurrentes**

Crear una actividad que se repite semanalmente:

```json
POST /api/ControladorDeActividadesMontessoriPersonalizadas

{
  "idEstudiante": 5,
  "nombre": "Práctica de Escritura Cursiva",
  "areaPedagogica": "Lenguaje",
  "duracionMinutosEstimada": 20,
  "objetivoPersonalizado": "Mejorar trazos de letras cursivas",
  "prioridad": "Normal",
  "esRecurrente": true,
  "frecuenciaRecurrencia": "Semanal",
  "fechaFinRecurrencia": "2025-12-31T00:00:00"
}
```

---

## 📈 Estadísticas y Reportes

### Ver Progreso General del Estudiante

```http
GET /api/ControladorDeActividadesMontessoriPersonalizadas/estudiante/2/estadisticas
```

**Información que obtienes:**
- ✅ Total de actividades asignadas
- ✅ Actividades por estado
- ✅ Promedio de progreso
- ✅ Actividades que requieren refuerzo
- ✅ Distribución por área pedagógica
- ✅ Distribución por nivel de desempeño
- ✅ Última actividad completada
- ✅ Total de minutos trabajados

---

## 🔔 Sistema de Recordatorios

### Configurar Recordatorio

Al crear la actividad:

```json
{
  "enviarRecordatorio": true,
  "fechaRecordatorio": "2025-11-09T08:30:00"
}
```

### Obtener Recordatorios Pendientes

```http
GET /api/ControladorDeActividadesMontessoriPersonalizadas/recordatorios
```

Esto retorna todas las actividades donde:
- `enviarRecordatorio = true`
- `recordatorioEnviado = false`
- `fechaRecordatorio <= AHORA`

---

## 🎨 Áreas Pedagógicas Montessori

Ejemplos de áreas que puedes usar:

- **Vida Práctica**: Verter, trasvasar, abotonar, cuidado personal
- **Sensorial**: Torre Rosa, Escalera Marrón, Cilindros con botón
- **Matemáticas**: Numerales, Sistema Decimal, Operaciones
- **Lenguaje**: Letras de lija, Alfabeto móvil, Lectura
- **Geografía**: Mapas, Globo terráqueo, Banderas
- **Ciencias**: Botánica, Zoología, Experimentos
- **Arte**: Pintura, Modelado, Música

---

## 💡 Mejores Prácticas

### ✅ Crear Actividades
1. **Objetivos claros**: Define qué esperas que el niño logre
2. **Adaptaciones**: Documenta cómo adaptaste la actividad
3. **Materiales**: Lista completa de lo que se necesita
4. **Prioridad**: Marca como Alta si es urgente o esencial

### ✅ Durante la Actividad
1. **Actualizar progreso**: Usa `EnProgreso` cuando el niño comienza
2. **Observaciones**: Anota comportamientos relevantes
3. **Tiempo real**: Registra cuánto tiempo realmente tomó

### ✅ Al Completar
1. **Logros específicos**: Lista logros concretos, no generales
2. **Dificultades**: Registra problemas para el seguimiento
3. **Sugerencias**: Guía para la próxima actividad
4. **Evidencias**: Agrega fotos/videos cuando sea posible

### ✅ Refuerzo
1. **Marcar refuerzo**: Si el niño no logró los objetivos
2. **Buscar patrones**: Revisa qué áreas necesitan más apoyo
3. **Planificar**: Crea actividades preparatorias

---

## 🔒 Permisos

- **Cualquier usuario autenticado** puede:
  - Ver actividades
  - Crear actividades
  - Actualizar progreso
  - Ver estadísticas

- **Recomendación**: En producción, restringir por roles:
  - **Profesores/Facilitadores**: CRUD completo
  - **Coordinadores**: Solo lectura y reportes
  - **Voluntarios**: Solo sus estudiantes asignados

---

## 📊 Diferencias: Actividades Normales vs Personalizadas

| Característica | Actividad Normal | Actividad Personalizada |
|----------------|------------------|-------------------------|
| **Asignación** | Grupo/Nivel | Un solo estudiante |
| **Adaptaciones** | Generales | Específicas del niño |
| **Seguimiento** | Básico | Detallado |
| **Evaluación** | Simple | Completa con logros |
| **Refuerzo** | No | Sí (flag) |
| **Evidencias** | No | Fotos y videos |
| **Recurrente** | No | Sí |
| **Prioridad** | No | Sí |
| **Recordatorios** | No | Sí |

---

## 🚀 Próximas Mejoras Sugeridas

- [ ] Dashboard visual de progreso del estudiante
- [ ] Gráficos de evolución por área pedagógica
- [ ] Exportar reportes PDF con evidencias
- [ ] Sistema de notificaciones push para recordatorios
- [ ] Plantillas de actividades frecuentes
- [ ] Comparación entre estudiantes (anónima)
- [ ] IA para sugerir próximas actividades basadas en historial

---

## 🎯 Resumen

Este módulo permite un **seguimiento individualizado** del progreso de cada estudiante en el método Montessori, con:

✅ **Personalización total** de cada actividad  
✅ **Evaluación detallada** con logros y dificultades  
✅ **Evidencias multimedia**  
✅ **Sistema de refuerzo** para identificar áreas de mejora  
✅ **Estadísticas completas** del progreso del niño  
✅ **Recordatorios automáticos**  
✅ **Actividades recurrentes**

**¡Perfecto para seguimiento pedagógico detallado!** 🎓📚
