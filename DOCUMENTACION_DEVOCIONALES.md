# 📖 Sistema de Gestión de Devocionales - API Endpoints

## 🎯 Resumen del Sistema

Sistema completo para gestionar **devocionales** con un **wizard de creación paso a paso** que incluye:
- **Información básica** (título, fecha, voluntario)
- **Texto bíblico** (pasaje, versículo clave)
- **Contenido completo** (objetivo, idea central, puntos principales, aplicación práctica)
- **Estados de workflow** (Borrador → Programado → En Progreso → Completado/Cancelado)

---

## 🏗️ Estados del Devocional

- `Borrador` (0) - En desarrollo, no publicado
- `Programado` (1) - Listo para realizarse
- `EnProgreso` (2) - Se está ejecutando actualmente
- `Completado` (3) - Finalizado exitosamente  
- `Cancelado` (4) - Cancelado por alguna razón

---

## 📋 ENDPOINTS PRINCIPALES

### 1. **Obtener todos los devocionales**
```http
GET /api/Devocional
Authorization: Bearer {token}
```

**Respuesta (200):**
```json
[
  {
    "id": 1,
    "titulo": "La Esperanza en Cristo",
    "fechaProgramada": "2024-12-15",
    "voluntarioAsignado": "Juan Pérez",
    "voluntarioAsignadoId": 2,
    "pasaje": "Juan 3:16-17",
    "textoClave": "Porque de tal manera amó Dios al mundo...",
    "objetivo": "Fortalecer la esperanza en los participantes",
    "idea": "Dios nos ama incondicionalmente",
    "puntosPrincipales": [
      {
        "t": "El amor de Dios",
        "v": "Juan 3:16",
        "d": "Explicar el amor incondicional de Dios"
      }
    ],
    "aplicacion": "Confiar en Dios en momentos difíciles",
    "reto": "Orar todos los días esta semana",
    "oracion": "Señor, ayúdanos a confiar más en ti...",
    "recursos": "Video: 'El amor de Dios', Cántico: 'Amazing Grace'",
    "estado": 1,
    "estadoTexto": "Programado",
    "createdAt": "2024-12-01T10:00:00Z",
    "createdByUserId": 1,
    "createdByUserName": "Admin Sistema",
    "isActive": true,
    "fechaFormateada": "15 de diciembre de 2024",
    "esBorrador": false,
    "esProgramado": true,
    "tienePuntosPrincipales": true
  }
]
```

### 2. **Obtener devocional por ID**
```http
GET /api/Devocional/{id}
Authorization: Bearer {token}
```

### 3. **Crear devocional completo**
```http
POST /api/Devocional
Authorization: Bearer {token}
Content-Type: application/json
```

**Body:**
```json
{
  "titulo": "La Fe que Transforma",
  "fechaProgramada": "2024-12-20",
  "voluntarioAsignado": "María García",
  "voluntarioAsignadoId": 3,
  "pasaje": "Hebreos 11:1",
  "textoClave": "Es, pues, la fe la certeza de lo que se espera...",
  "objetivo": "Enseñar sobre la importancia de la fe",
  "idea": "La fe nos conecta con Dios",
  "puntosPrincipales": [
    {
      "t": "¿Qué es la fe?",
      "v": "Hebreos 11:1",
      "d": "Definición bíblica de fe y su importancia"
    },
    {
      "t": "Ejemplos de fe",
      "v": "Hebreos 11:4-7",
      "d": "Abraham, Moisés y otros héroes de la fe"
    }
  ],
  "aplicacion": "Ejercitar la fe en situaciones cotidianas",
  "reto": "Leer un capítulo de Hebreos cada día",
  "oracion": "Padre, aumenta nuestra fe...",
  "recursos": "Libro: 'Héroes de la Fe', Video testimonial",
  "estado": 0
}
```

### 4. **Actualizar devocional**
```http
PUT /api/Devocional/{id}
Authorization: Bearer {token}
```

### 5. **Desactivar devocional**
```http
DELETE /api/Devocional/{id}
Authorization: Bearer {token}
```

---

## 🎨 ENDPOINTS DEL WIZARD

### **Paso 1: Información Básica**
```http
POST /api/Devocional/wizard/step1
Authorization: Bearer {token}
Content-Type: application/json
```

**Body:**
```json
{
  "titulo": "Mi Nuevo Devocional",
  "fechaProgramada": "2024-12-25",
  "voluntarioAsignado": "Carlos López",
  "voluntarioAsignadoId": 4
}
```

**Respuesta (200):** Devocional creado en estado "Borrador"

### **Paso 2: Texto Bíblico**
```http
PUT /api/Devocional/wizard/{id}/step2
Authorization: Bearer {token}
```

**Body:**
```json
{
  "pasaje": "Salmo 23:1-6",
  "textoClave": "Jehová es mi pastor; nada me faltará"
}
```

### **Paso 3: Contenido**
```http
PUT /api/Devocional/wizard/{id}/step3
Authorization: Bearer {token}
```

**Body:**
```json
{
  "objetivo": "Mostrar el cuidado de Dios como pastor",
  "idea": "Dios nos cuida como un pastor cuida sus ovejas",
  "puntosPrincipales": [
    {
      "t": "Dios como pastor",
      "v": "Salmo 23:1",
      "d": "Explicar la metáfora del pastor y las ovejas"
    },
    {
      "t": "Provisión divina",
      "v": "Salmo 23:2-3",
      "d": "Cómo Dios provee para nuestras necesidades"
    },
    {
      "t": "Protección en dificultades",
      "v": "Salmo 23:4",
      "d": "Dios nos acompaña en momentos difíciles"
    }
  ],
  "aplicacion": "Confiar en el cuidado de Dios en situaciones difíciles",
  "reto": "Memorizar el Salmo 23 durante esta semana",
  "oracion": "Señor, ayúdanos a reconocer tu cuidado pastoral...",
  "recursos": "Cántico: 'El Señor es mi Pastor', Ilustración: historia de pastores"
}
```

### **Guardar Borrador (en cualquier momento)**
```http
POST /api/Devocional/draft
Authorization: Bearer {token}
```

**Body (todos los campos opcionales):**
```json
{
  "devocionalId": 5,
  "titulo": "Título actualizado",
  "fechaProgramada": "2024-12-30",
  "pasaje": "Filipenses 4:13",
  "objetivo": "Nuevo objetivo",
  "// ... otros campos que se quieran guardar"
}
```

### **Finalizar Devocional (Borrador → Programado)**
```http
PUT /api/Devocional/{id}/finalize
Authorization: Bearer {token}
```

---

## 📊 FILTROS Y CONSULTAS

### **Por Estado**
```http
GET /api/Devocional/status/0    # Borradores
GET /api/Devocional/status/1    # Programados
GET /api/Devocional/status/2    # En progreso
GET /api/Devocional/status/3    # Completados
GET /api/Devocional/status/4    # Cancelados
```

### **Por Voluntario**
```http
GET /api/Devocional/volunteer/{voluntarioId}
```

### **Por Fecha**
```http
GET /api/Devocional/date/2024-12-15
GET /api/Devocional/date-range?fechaInicio=2024-12-01&fechaFin=2024-12-31
```

### **Consultas Específicas**
```http
GET /api/Devocional/drafts                   # Solo borradores
GET /api/Devocional/scheduled                # Solo programados
GET /api/Devocional/upcoming?dias=7          # Próximos 7 días
GET /api/Devocional/recent?take=5            # 5 más recientes
GET /api/Devocional/my-devotionals           # Creados por mí
```

### **Búsqueda**
```http
GET /api/Devocional/search?q=esperanza
```

---

## 🔄 CAMBIOS DE ESTADO

### **Programar Devocional**
```http
PUT /api/Devocional/{id}/schedule
Authorization: Bearer {token}
```

### **Iniciar Devocional**
```http
PUT /api/Devocional/{id}/start
Authorization: Bearer {token}
```

### **Completar Devocional**
```http
PUT /api/Devocional/{id}/complete
Authorization: Bearer {token}
```

### **Cancelar Devocional**
```http
PUT /api/Devocional/{id}/cancel
Authorization: Bearer {token}
```

### **Cambio Manual de Estado**
```http
PUT /api/Devocional/{id}/status/3
Authorization: Bearer {token}
```

---

## 🛠️ UTILIDADES

### **Preview del Devocional**
```http
GET /api/Devocional/{id}/preview
Authorization: Bearer {token}
```

**Respuesta:**
```json
{
  "titulo": "La Esperanza en Cristo",
  "fechaProgramada": "2024-12-15",
  "voluntarioAsignado": "Juan Pérez",
  "pasaje": "Juan 3:16-17",
  "objetivo": "Fortalecer la esperanza",
  "idea": "Dios nos ama incondicionalmente",
  "contentSummary": "objetivo, idea central, 3 puntos principales, aplicación práctica, reto semanal, oración, recursos adicionales",
  "hasContent": true
}
```

### **Lista de Voluntarios Disponibles**
```http
GET /api/Devocional/available-volunteers
Authorization: Bearer {token}
```

**Respuesta:**
```json
[
  "Juan Pérez",
  "María García", 
  "Carlos López",
  "Ana Martínez"
]
```

### **Estadísticas**
```http
GET /api/Devocional/statistics
Authorization: Bearer {token}
```

**Respuesta:**
```json
{
  "totalDevocionales": 25,
  "borradoresCount": 3,
  "programadosCount": 8,
  "completadosCount": 12,
  "cancelladosCount": 2,
  "devocionalesTodayCount": 1,
  "devocionalesTomorrow": 2,
  "devocionalessemanaCount": 5
}
```

---

## 🎯 FLUJO TÍPICO DE USO

### **1. Creación con Wizard:**
```javascript
// Paso 1
const step1Response = await fetch('/api/Devocional/wizard/step1', {
  method: 'POST',
  headers: { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' },
  body: JSON.stringify({
    titulo: "Mi Devocional",
    fechaProgramada: "2024-12-15",
    voluntarioAsignado: "Juan Pérez"
  })
});
const devocional = await step1Response.json();

// Paso 2
await fetch(`/api/Devocional/wizard/${devocional.id}/step2`, {
  method: 'PUT',
  headers: { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' },
  body: JSON.stringify({
    pasaje: "Juan 3:16",
    textoClave: "Porque de tal manera amó Dios al mundo..."
  })
});

// Paso 3
await fetch(`/api/Devocional/wizard/${devocional.id}/step3`, {
  method: 'PUT',
  headers: { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' },
  body: JSON.stringify({
    objetivo: "Mostrar el amor de Dios",
    idea: "Dios nos ama incondicionalmente",
    puntosPrincipales: [/* array de puntos */],
    aplicacion: "Amar a otros como Dios nos ama"
  })
});

// Finalizar
await fetch(`/api/Devocional/${devocional.id}/finalize`, {
  method: 'PUT',
  headers: { 'Authorization': `Bearer ${token}` }
});
```

### **2. Dashboard de Gestión:**
```javascript
// Obtener estadísticas para el dashboard
const stats = await fetch('/api/Devocional/statistics', {
  headers: { 'Authorization': `Bearer ${token}` }
}).then(r => r.json());

// Devocionales de hoy
const hoy = await fetch('/api/Devocional/date/2024-12-15', {
  headers: { 'Authorization': `Bearer ${token}` }
}).then(r => r.json());

// Próximos devocionales
const proximos = await fetch('/api/Devocional/upcoming?dias=7', {
  headers: { 'Authorization': `Bearer ${token}` }
}).then(r => r.json());

// Borradores pendientes
const borradores = await fetch('/api/Devocional/drafts', {
  headers: { 'Authorization': `Bearer ${token}` }
}).then(r => r.json());
```

---

## 📱 Integración Frontend

### **Estructura de Datos para el Wizard:**
```typescript
interface DevocionalWizard {
  // Paso 1: Información Básica
  titulo: string;
  fechaProgramada?: Date;
  voluntarioAsignado?: string;
  
  // Paso 2: Texto Bíblico  
  pasaje: string;
  textoClave?: string;
  
  // Paso 3: Contenido
  objetivo?: string;
  idea?: string;
  puntosPrincipales?: PuntoPrincipal[];
  aplicacion?: string;
  reto?: string;
  oracion?: string;
  recursos?: string;
}

interface PuntoPrincipal {
  t: string; // Título del punto
  v?: string; // Versículo de apoyo
  d?: string; // Desarrollo/descripción
}
```

### **Validaciones por Paso:**
```typescript
// Paso 1 válido
isStep1Valid(): boolean {
  return this.devocional.titulo?.length > 0;
}

// Paso 2 válido  
isStep2Valid(): boolean {
  return this.devocional.pasaje?.length > 0;
}

// Auto-guardado
onModelChange() {
  clearTimeout(this.saveTimeout);
  this.saveTimeout = setTimeout(() => {
    this.saveDraft();
  }, 2000);
}
```

El sistema está **completamente listo** con 30+ endpoints para gestionar devocionales con wizard paso a paso. ¿Necesitas que ajuste algo específico de la API?