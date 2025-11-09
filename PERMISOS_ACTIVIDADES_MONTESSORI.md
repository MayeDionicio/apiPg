# 🎯 Sistema de Permisos: Actividades Montessori por Nivel

## 📋 Funcionamiento

### **Voluntarios (Creadores)**
Los voluntarios pueden:
- ✅ **Crear** actividades Montessori con rango de edad (EdadMinima - EdadMaxima)
- ✅ **Ver todas** sus actividades creadas: `GET /api/actividades-montessori/mis-actividades`
- ✅ **Editar** sus propias actividades
- ✅ **Eliminar** sus propias actividades
- ✅ **Ver estadísticas** de sus actividades

**Ejemplo de actividad:**
```json
{
  "nombre": "Trasvasar agua con jarritas",
  "areaPedagogica": "Vida Práctica",
  "edadMinima": 3,    // Edad mínima: 3 años
  "edadMaxima": 6,    // Edad máxima: 6 años
  ...
}
```

---

### **Participantes (Visualizadores)**
Los participantes pueden:
- ✅ **Ver solo actividades** que corresponden a su edad/nivel
- ✅ El sistema **calcula automáticamente** su edad desde su `FechaDeNacimiento`
- ✅ Filtra actividades donde: `EdadMinima <= EdadParticipante <= EdadMaxima`
- ✅ **Ver logros** asociados a esas actividades

**Endpoint especial para participantes:**
```http
GET /api/actividades-montessori/para-mi-nivel
Authorization: Bearer {token-del-participante}
```

---

## 🔢 Cálculo de Edad y Filtrado

### **Paso 1: Obtener edad del participante**
```csharp
// El sistema obtiene la fecha de nacimiento del participante
var participante = Usuario con Id del token JWT
var fechaNacimiento = participante.FechaDeNacimiento

// Calcula la edad actual
Edad = Año actual - Año de nacimiento
// Ajusta si aún no cumplió años este año
```

### **Paso 2: Filtrar actividades**
```sql
SELECT * FROM ActividadesMontessori
WHERE EstaActivo = true
  AND EdadMinima <= {EdadParticipante}
  AND EdadMaxima >= {EdadParticipante}
ORDER BY CreadoEn DESC
```

---

## 📊 Ejemplos Prácticos

### **Ejemplo 1: Participante de 4 años**

**Participante:**
- Nombre: Juan Pérez
- Fecha de Nacimiento: 15/03/2021
- **Edad calculada: 4 años**

**Actividades que puede ver:**

| ID | Nombre Actividad | Edad Min | Edad Max | ¿Visible? |
|----|-----------------|----------|----------|-----------|
| 1  | Trasvasar agua  | 3        | 6        | ✅ Sí     |
| 2  | Contar perlas   | 4        | 7        | ✅ Sí     |
| 3  | Leer cuentos    | 6        | 10       | ❌ No     |
| 4  | Torre rosa      | 2        | 4        | ✅ Sí     |
| 5  | Fracciones      | 8        | 12       | ❌ No     |

**Resultado:** Juan verá las actividades 1, 2 y 4 solamente.

---

### **Ejemplo 2: Participante de 7 años**

**Participante:**
- Nombre: María López
- Fecha de Nacimiento: 20/08/2018
- **Edad calculada: 7 años**

**Actividades que puede ver:**

| ID | Nombre Actividad | Edad Min | Edad Max | ¿Visible? |
|----|-----------------|----------|----------|-----------|
| 1  | Trasvasar agua  | 3        | 6        | ❌ No     |
| 2  | Contar perlas   | 4        | 7        | ✅ Sí     |
| 3  | Leer cuentos    | 6        | 10       | ✅ Sí     |
| 4  | Torre rosa      | 2        | 4        | ❌ No     |
| 5  | Fracciones      | 8        | 12       | ❌ No     |

**Resultado:** María verá las actividades 2 y 3 solamente.

---

## 🔐 Seguridad y Validaciones

### **Validaciones al consultar actividades:**

1. ✅ **Usuario debe existir** y estar activo
2. ✅ **Debe tener FechaDeNacimiento** registrada
3. ✅ **Token JWT válido** requerido
4. ✅ Solo retorna actividades **activas** (EstaActivo = true)

### **Mensajes de error:**

```json
// Si el participante no existe
{
  "mensaje": "Participante no encontrado"
}

// Si no tiene fecha de nacimiento
{
  "mensaje": "El participante no tiene fecha de nacimiento registrada"
}
```

---

## 📝 Flujo Completo

```
1. VOLUNTARIO CREA ACTIVIDAD
   ↓
   POST /api/actividades-montessori
   {
     "nombre": "Trasvasar agua",
     "edadMinima": 3,
     "edadMaxima": 6,
     ...
   }
   ↓
   Actividad guardada en BD

2. PARTICIPANTE SE REGISTRA
   ↓
   Incluye FechaDeNacimiento
   ↓
   Sistema calcula edad: 4 años

3. PARTICIPANTE CONSULTA SUS ACTIVIDADES
   ↓
   GET /api/actividades-montessori/para-mi-nivel
   ↓
   Sistema filtra: EdadMin ≤ 4 ≤ EdadMax
   ↓
   Retorna solo actividades de 3-6 años, 4-7 años, 2-5 años, etc.
```

---

## 🎓 Niveles y Rangos Típicos Montessori

| Nivel              | Rango de Edad | Ejemplo de Actividades          |
|--------------------|---------------|----------------------------------|
| Casa de Niños      | 3-6 años      | Vida práctica, sensorial básico |
| Taller I           | 6-9 años      | Lectoescritura, matemáticas     |
| Taller II          | 9-12 años     | Ciencias, historia, geometría   |

El sistema permite cualquier rango personalizado entre **0 y 12 años** con decimales (Ej: 3.5 años).

---

## 🧪 Pruebas de Ejemplo

### **Test 1: Participante sin fecha de nacimiento**
```http
GET /api/actividades-montessori/para-mi-nivel
Authorization: Bearer {token-sin-fecha}

Respuesta:
400 Bad Request
{
  "mensaje": "El participante no tiene fecha de nacimiento registrada"
}
```

### **Test 2: Participante de 5 años**
```http
GET /api/actividades-montessori/para-mi-nivel
Authorization: Bearer {token-participante-5-años}

Respuesta:
200 OK
[
  {
    "id": 1,
    "nombre": "Trasvasar agua",
    "edadMinima": 3,
    "edadMaxima": 6,
    ...
  },
  {
    "id": 2,
    "nombre": "Torre rosa",
    "edadMinima": 3,
    "edadMaxima": 7,
    ...
  }
]
```

---

## 📌 Resumen

- ✅ **Voluntarios** crean actividades con rangos de edad
- ✅ **Participantes** solo ven actividades de su nivel (edad)
- ✅ Cálculo automático de edad desde `FechaDeNacimiento`
- ✅ Filtrado en base de datos con consulta eficiente
- ✅ Sin necesidad de gestionar niveles manualmente
- ✅ Sistema flexible con rangos personalizables

---

## 🔗 Endpoint Adicional

Si necesitas obtener actividades para un participante específico (desde admin/coordinador):

```http
# Nota: Este endpoint aún no está implementado, pero se puede agregar:
GET /api/actividades-montessori/para-participante/{participanteId}
Authorization: Bearer {token-coordinador}
```
