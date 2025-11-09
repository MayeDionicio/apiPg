# 📧 Configuración de Envío de Correos - Recuperación de Contraseña

## 🔧 Configuración Actual

El módulo de recuperación de contraseña tiene **DOS MODOS**:

### 1️⃣ **MODO DESARROLLO** (Actual)
- ❌ **NO envía emails reales**
- ✅ Retorna el token directamente en la respuesta del API
- 🎯 Útil para pruebas locales sin configurar email

### 2️⃣ **MODO PRODUCCIÓN**
- ✅ **Envía emails reales** usando SMTP
- ❌ NO retorna el token en la respuesta
- 🔒 Usuario recibe token solo por correo

---

## 🚀 Cómo Activar el Envío de Emails

### Opción A: Usar Gmail (Recomendado)

#### Paso 1: Crear "App Password" en Gmail

1. Ve a tu cuenta de Google: https://myaccount.google.com/
2. Navega a **Seguridad** → **Verificación en dos pasos** (actívala si no está activa)
3. Busca **"Contraseñas de aplicaciones"** (App Passwords)
4. Selecciona:
   - App: **Correo**
   - Dispositivo: **Otro (nombre personalizado)** → escribe "API PG"
5. Google te dará una contraseña de 16 caracteres (ejemplo: `abcd efgh ijkl mnop`)
6. **Copia esta contraseña** (sin espacios: `abcdefghijklmnop`)

#### Paso 2: Configurar `appsettings.json`

Edita el archivo `appsettings.json` y completa:

```json
"EmailSettings": {
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": 587,
  "SenderEmail": "tu-email@gmail.com",           // ⚠️ Cambia esto
  "SenderName": "API PG - Recuperación",
  "SenderPassword": "abcdefghijklmnop",          // ⚠️ Usa la App Password (sin espacios)
  "EnableSsl": true
}
```

#### Paso 3: ¡Listo! Reinicia la API

```powershell
dotnet run
```

---

### Opción B: Usar Outlook/Hotmail

```json
"EmailSettings": {
  "SmtpServer": "smtp-mail.outlook.com",
  "SmtpPort": 587,
  "SenderEmail": "tu-email@outlook.com",
  "SenderName": "API PG - Recuperación",
  "SenderPassword": "tu-contraseña-outlook",
  "EnableSsl": true
}
```

---

### Opción C: Usar SendGrid (Recomendado para Producción)

SendGrid es un servicio profesional de emails con API key:

1. Regístrate en: https://sendgrid.com/
2. Crea una API Key
3. Configura:

```json
"EmailSettings": {
  "SmtpServer": "smtp.sendgrid.net",
  "SmtpPort": 587,
  "SenderEmail": "noreply@tudominio.com",
  "SenderName": "API PG",
  "SenderPassword": "TU_SENDGRID_API_KEY",
  "EnableSsl": true
}
```

---

## 🧪 Cómo Probar

### 1. **Modo Desarrollo** (Sin configurar email)

```http
POST http://localhost:5273/api/ControladorDeRecuperacion/solicitar
Content-Type: application/json

{
  "email": "usuario@ejemplo.com"
}
```

**Respuesta:**
```json
{
  "exitoso": true,
  "mensaje": "Token generado exitosamente. En producción se enviaría por email.",
  "token": "a7f3x9k2m8p1w5q4j6h3n9v7c2b8d5e1",  // ← Token visible para pruebas
  "fechaExpiracion": "2025-11-09T13:40:00Z"
}
```

---

### 2. **Modo Producción** (Con email configurado)

```http
POST http://localhost:5273/api/ControladorDeRecuperacion/solicitar
Content-Type: application/json

{
  "email": "usuario@ejemplo.com"
}
```

**Respuesta:**
```json
{
  "exitoso": true,
  "mensaje": "Se ha enviado un correo con las instrucciones de recuperación"
  // ⚠️ NO incluye el token (por seguridad)
}
```

**El usuario recibe un email como este:**

---

## 📨 Ejemplo del Email que Reciben los Usuarios

![Email Example](preview-email.png)

**Asunto:** Recuperación de Contraseña - API PG

**Contenido:**
```
🔐 Recuperación de Contraseña

Hola Juan,

Recibimos una solicitud para restablecer tu contraseña en API PG.

Usa el siguiente código para crear una nueva contraseña:

┌─────────────────────────────────────┐
│  a7f3x9k2m8p1w5q4j6h3n9v7c2b8d5e1  │
└─────────────────────────────────────┘

⏰ Este código expirará el: 09/11/2025 13:40 (UTC)
Tiempo restante: 24 horas

─────────────────────────────────────

⚠️ Si NO solicitaste este cambio:
• Ignora este correo electrónico
• Tu contraseña actual permanecerá sin cambios
• Considera cambiar tu contraseña por seguridad

Consejos de seguridad:
• Nunca compartas este código con nadie
• Usa una contraseña fuerte y única
• No uses la misma contraseña en múltiples sitios

─────────────────────────────────────
Este es un correo automático, no respondas.
© 2025 API PG. Todos los derechos reservados.
```

---

## 🔍 Verificar que el Email Funciona

Revisa los logs de la aplicación:

```
info: ApiPG.Services.ServicioDeRecuperacion[0]
      Email de recuperación enviado a usuario@ejemplo.com
```

Si ves este mensaje, el email se envió correctamente ✅

---

## ❌ Solución de Problemas

### Error: "Error SMTP al enviar email"

**Causa:** Credenciales incorrectas o configuración SMTP errónea

**Solución:**
1. Verifica que usas la **App Password** de Gmail (no tu contraseña normal)
2. Asegúrate que la verificación en dos pasos esté activa
3. Revisa que `SenderEmail` y `SenderPassword` sean correctos

---

### Error: "Error al enviar email: Authentication failed"

**Causa:** Gmail bloqueó el acceso

**Solución:**
1. Ve a: https://myaccount.google.com/lesssecureapps
2. O usa **App Password** (recomendado)

---

### No se envía el email pero no hay error

**Causa:** La configuración está vacía

**Solución:**
Verifica en `appsettings.json` que `SenderEmail` y `SenderPassword` **NO estén vacíos**

---

## 🔒 Seguridad en Producción

### ⚠️ IMPORTANTE: NO subir contraseñas a Git

Usa **variables de entorno** o **Azure Key Vault** para las credenciales:

```json
"EmailSettings": {
  "SenderEmail": "${EMAIL_SENDER}",
  "SenderPassword": "${EMAIL_PASSWORD}"
}
```

Luego en tu servidor:
```bash
export EMAIL_SENDER="tu-email@gmail.com"
export EMAIL_PASSWORD="tu-app-password"
```

---

## 📊 Estadísticas de Envío

Para monitorear los emails enviados, revisa la tabla `TokensRecuperacion`:

```sql
SELECT 
    COUNT(*) as TotalSolicitudes,
    COUNT(CASE WHEN Usado = true THEN 1 END) as TokensUsados,
    COUNT(CASE WHEN FechaExpiracion < NOW() THEN 1 END) as TokensExpirados
FROM "TokensRecuperacion"
WHERE "CreadoEn" > NOW() - INTERVAL '7 days';
```

---

## 🎯 Recomendaciones

1. **Desarrollo:** Usa modo sin email (más rápido para probar)
2. **Staging:** Usa Gmail SMTP para pruebas con emails reales
3. **Producción:** Usa SendGrid o Amazon SES (más confiable, mejor deliverability)

---

## 💡 Próximas Mejoras

- [ ] Cola de emails con retry automático
- [ ] Plantillas HTML personalizables
- [ ] Soporte para múltiples idiomas
- [ ] Notificación cuando se cambia la contraseña exitosamente
- [ ] Límite de intentos (rate limiting)
