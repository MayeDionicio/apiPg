# 📊 Integración Frontend - Dashboard de Análisis

## 🎯 Endpoints Disponibles

```typescript
const API_BASE_URL = 'http://localhost:5000/api/analisis';

// 1. Métricas generales (todo en uno)
GET /api/analisis/metricas-generales

// 2. Crecimiento de usuarios
GET /api/analisis/crecimiento-usuarios?meses=6

// 3. Actividades completadas
GET /api/analisis/actividades-completadas

// 4. Tiempo de uso
GET /api/analisis/tiempo-uso

// 5. Estadísticas detalladas
GET /api/analisis/estadisticas-detalladas
```

---

## 🚀 Implementación en Angular

### 1. **Crear Interfaces (models/analisis.model.ts)**

```typescript
export interface MetricasGenerales {
  crecimientoUsuarios: CrecimientoUsuarios;
  actividadesCompletadas: ActividadesCompletadas;
  tiempoDeUso: TiempoDeUso;
}

export interface CrecimientoUsuarios {
  nuevosUsuariosEsteMes: number;
  tasaRetencion: number;
  usuariosTotales: number;
  usuariosActivosEsteMes: number;
  crecimientoPorMes: UsuariosPorMes[];
}

export interface UsuariosPorMes {
  mes: number;
  anio: number;
  nombreMes: string;
  cantidadNuevos: number;
  cantidadTotal: number;
}

export interface ActividadesCompletadas {
  actividadesTotales: number;
  actividadesCompletadas: number;
  porcentajeCompletacion: number;
  tareasCompletadas: number;
  tareasTotales: number;
  completacionPorMes: ActividadesPorMes[];
}

export interface ActividadesPorMes {
  mes: number;
  anio: number;
  nombreMes: string;
  completadas: number;
  totales: number;
  porcentaje: number;
}

export interface TiempoDeUso {
  promedioHorasDiarias: number;
  porcentajeUsuariosActivos: number;
  usuariosActivosHoy: number;
  usuariosTotales: number;
  sesionesTotales: number;
  tiempoTotalHoras: number;
}

export interface EstadisticasDetalladas {
  totalUsuarios: number;
  usuariosActivos: number;
  usuariosInactivos: number;
  usuariosPorRol: MetricasPorRol[];
  totalNiveles: number;
  nivelesActivos: number;
  promedioParticipantesPorNivel: number;
  totalActividades: number;
  actividadesActivas: number;
  actividadesMontessori: number;
  totalTareas: number;
  tareasCompletadas: number;
  tareasPendientes: number;
  tareasEnProceso: number;
  registrosAsistenciaHoy: number;
  registrosAsistenciaEsteMes: number;
  porcentajeAsistencia: number;
}

export interface MetricasPorRol {
  nombreRol: string;
  cantidadUsuarios: number;
  usuariosActivos: number;
  porcentajeActivos: number;
}
```

---

### 2. **Crear Servicio (services/dashboard.service.ts)**

```typescript
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MetricasGenerales, CrecimientoUsuarios, ActividadesCompletadas, TiempoDeUso, EstadisticasDetalladas } from '../models/analisis.model';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private baseUrl = 'http://localhost:5000/api/analisis';

  constructor(private http: HttpClient) {}

  /**
   * Obtener todas las métricas generales
   */
  obtenerMetricasGenerales(): Observable<MetricasGenerales> {
    return this.http.get<MetricasGenerales>(`${this.baseUrl}/metricas-generales`);
  }

  /**
   * Obtener crecimiento de usuarios
   * @param meses Cantidad de meses a consultar (default: 6)
   */
  obtenerCrecimientoUsuarios(meses: number = 6): Observable<CrecimientoUsuarios> {
    return this.http.get<CrecimientoUsuarios>(
      `${this.baseUrl}/crecimiento-usuarios?meses=${meses}`
    );
  }

  /**
   * Obtener actividades completadas
   */
  obtenerActividadesCompletadas(): Observable<ActividadesCompletadas> {
    return this.http.get<ActividadesCompletadas>(
      `${this.baseUrl}/actividades-completadas`
    );
  }

  /**
   * Obtener tiempo de uso
   */
  obtenerTiempoDeUso(): Observable<TiempoDeUso> {
    return this.http.get<TiempoDeUso>(`${this.baseUrl}/tiempo-uso`);
  }

  /**
   * Obtener estadísticas detalladas
   */
  obtenerEstadisticasDetalladas(): Observable<EstadisticasDetalladas> {
    return this.http.get<EstadisticasDetalladas>(
      `${this.baseUrl}/estadisticas-detalladas`
    );
  }
}
```

---

### 3. **Componente del Dashboard (dashboard.component.ts)**

```typescript
import { Component, OnInit } from '@angular/core';
import { DashboardService } from '../../services/dashboard.service';
import { MetricasGenerales } from '../../models/analisis.model';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  metricas: MetricasGenerales | null = null;
  loading = true;
  error: string | null = null;

  constructor(private dashboardService: DashboardService) {}

  ngOnInit(): void {
    this.cargarMetricas();
  }

  cargarMetricas(): void {
    this.loading = true;
    this.error = null;

    this.dashboardService.obtenerMetricasGenerales().subscribe({
      next: (data) => {
        this.metricas = data;
        this.loading = false;
        console.log('Métricas cargadas:', data);
      },
      error: (err) => {
        console.error('Error al cargar métricas:', err);
        this.error = 'Error al cargar las métricas del dashboard';
        this.loading = false;
      }
    });
  }

  recargarMetricas(): void {
    this.cargarMetricas();
  }
}
```

---

### 4. **Template HTML (dashboard.component.html)**

```html
<div class="dashboard-container">
  <!-- Header -->
  <div class="dashboard-header">
    <h1>📊 Panel de Administración</h1>
    <button (click)="recargarMetricas()" class="btn-refresh">
      🔄 Actualizar
    </button>
  </div>

  <!-- Loading -->
  <div *ngIf="loading" class="loading">
    <div class="spinner"></div>
    <p>Cargando métricas...</p>
  </div>

  <!-- Error -->
  <div *ngIf="error" class="error-message">
    <p>⚠️ {{ error }}</p>
    <button (click)="recargarMetricas()">Reintentar</button>
  </div>

  <!-- Dashboard Content -->
  <div *ngIf="metricas && !loading" class="dashboard-content">
    
    <!-- Fila 1: Métricas principales -->
    <div class="metrics-row">
      
      <!-- Card: Crecimiento de Usuarios -->
      <div class="metric-card">
        <div class="card-icon">📈</div>
        <h3>Crecimiento de Usuarios</h3>
        
        <div class="metric-main">
          <span class="metric-value">
            +{{ metricas.crecimientoUsuarios.nuevosUsuariosEsteMes }}
          </span>
          <span class="metric-label">Nuevos usuarios este mes</span>
        </div>

        <div class="metric-secondary">
          <div class="secondary-item">
            <span class="value">{{ metricas.crecimientoUsuarios.tasaRetencion }}%</span>
            <span class="label">Tasa de retención</span>
          </div>
          <div class="secondary-item">
            <span class="value">{{ metricas.crecimientoUsuarios.usuariosTotales }}</span>
            <span class="label">Total usuarios</span>
          </div>
        </div>
      </div>

      <!-- Card: Actividades Completadas -->
      <div class="metric-card">
        <div class="card-icon">🎯</div>
        <h3>Actividades Completadas</h3>
        
        <div class="metric-main">
          <span class="metric-value">
            {{ metricas.actividadesCompletadas.actividadesCompletadas }}
          </span>
          <span class="metric-label">Actividades totales</span>
        </div>

        <div class="metric-secondary">
          <div class="secondary-item">
            <span class="value">{{ metricas.actividadesCompletadas.porcentajeCompletacion }}%</span>
            <span class="label">Completación promedio</span>
          </div>
          <div class="secondary-item">
            <span class="value">{{ metricas.actividadesCompletadas.tareasCompletadas }}</span>
            <span class="label">Tareas completadas</span>
          </div>
        </div>
      </div>

      <!-- Card: Tiempo de Uso -->
      <div class="metric-card">
        <div class="card-icon">⏱️</div>
        <h3>Tiempo de Uso</h3>
        
        <div class="metric-main">
          <span class="metric-value">
            {{ metricas.tiempoDeUso.promedioHorasDiarias }}h
          </span>
          <span class="metric-label">Promedio diario</span>
        </div>

        <div class="metric-secondary">
          <div class="secondary-item">
            <span class="value">{{ metricas.tiempoDeUso.porcentajeUsuariosActivos }}%</span>
            <span class="label">Usuarios activos</span>
          </div>
          <div class="secondary-item">
            <span class="value">{{ metricas.tiempoDeUso.usuariosActivosHoy }}</span>
            <span class="label">Activos hoy</span>
          </div>
        </div>
      </div>

    </div>

    <!-- Fila 2: Análisis del Sistema -->
    <div class="analysis-section">
      <h2>📉 Análisis del Sistema</h2>
      <p>Métricas avanzadas y análisis de datos</p>
      <button routerLink="/analisis/detallado" class="btn-view-more">
        Ver análisis completo →
      </button>
    </div>

  </div>
</div>
```

---

### 5. **Estilos CSS/SCSS (dashboard.component.scss)**

```scss
.dashboard-container {
  padding: 2rem;
  background: #f5f7fa;
  min-height: 100vh;

  .dashboard-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 2rem;

    h1 {
      font-size: 2rem;
      color: #2c3e50;
    }

    .btn-refresh {
      padding: 0.75rem 1.5rem;
      background: #3498db;
      color: white;
      border: none;
      border-radius: 8px;
      cursor: pointer;
      font-size: 1rem;
      transition: all 0.3s;

      &:hover {
        background: #2980b9;
        transform: translateY(-2px);
      }
    }
  }

  .loading {
    text-align: center;
    padding: 4rem;

    .spinner {
      width: 50px;
      height: 50px;
      border: 4px solid #f3f3f3;
      border-top: 4px solid #3498db;
      border-radius: 50%;
      animation: spin 1s linear infinite;
      margin: 0 auto 1rem;
    }
  }

  .error-message {
    background: #fee;
    border: 1px solid #fcc;
    border-radius: 8px;
    padding: 2rem;
    text-align: center;
    color: #c33;

    button {
      margin-top: 1rem;
      padding: 0.75rem 1.5rem;
      background: #e74c3c;
      color: white;
      border: none;
      border-radius: 6px;
      cursor: pointer;

      &:hover {
        background: #c0392b;
      }
    }
  }

  .metrics-row {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
    gap: 1.5rem;
    margin-bottom: 2rem;

    .metric-card {
      background: white;
      border-radius: 12px;
      padding: 1.5rem;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
      transition: all 0.3s;

      &:hover {
        transform: translateY(-4px);
        box-shadow: 0 4px 12px rgba(0,0,0,0.15);
      }

      .card-icon {
        font-size: 2.5rem;
        margin-bottom: 0.5rem;
      }

      h3 {
        font-size: 1.1rem;
        color: #7f8c8d;
        margin-bottom: 1rem;
        font-weight: 500;
      }

      .metric-main {
        margin-bottom: 1rem;

        .metric-value {
          display: block;
          font-size: 2.5rem;
          font-weight: bold;
          color: #2c3e50;
          margin-bottom: 0.25rem;
        }

        .metric-label {
          display: block;
          font-size: 0.9rem;
          color: #95a5a6;
        }
      }

      .metric-secondary {
        display: flex;
        gap: 1rem;
        padding-top: 1rem;
        border-top: 1px solid #ecf0f1;

        .secondary-item {
          flex: 1;

          .value {
            display: block;
            font-size: 1.25rem;
            font-weight: 600;
            color: #3498db;
          }

          .label {
            display: block;
            font-size: 0.8rem;
            color: #95a5a6;
            margin-top: 0.25rem;
          }
        }
      }
    }
  }

  .analysis-section {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    color: white;
    padding: 2rem;
    border-radius: 12px;
    text-align: center;

    h2 {
      margin: 0 0 0.5rem 0;
      font-size: 1.5rem;
    }

    p {
      margin: 0 0 1rem 0;
      opacity: 0.9;
    }

    .btn-view-more {
      padding: 0.75rem 2rem;
      background: white;
      color: #667eea;
      border: none;
      border-radius: 8px;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.3s;

      &:hover {
        transform: scale(1.05);
        box-shadow: 0 4px 12px rgba(0,0,0,0.2);
      }
    }
  }
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}
```

---

## 🎨 Implementación en React

```typescript
// DashboardPage.tsx
import React, { useState, useEffect } from 'react';
import axios from 'axios';

interface MetricasGenerales {
  crecimientoUsuarios: CrecimientoUsuarios;
  actividadesCompletadas: ActividadesCompletadas;
  tiempoDeUso: TiempoDeUso;
}

const DashboardPage: React.FC = () => {
  const [metricas, setMetricas] = useState<MetricasGenerales | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const cargarMetricas = async () => {
    try {
      setLoading(true);
      const token = localStorage.getItem('token');
      const response = await axios.get(
        'http://localhost:5000/api/analisis/metricas-generales',
        {
          headers: { Authorization: `Bearer ${token}` }
        }
      );
      setMetricas(response.data);
      setError(null);
    } catch (err) {
      setError('Error al cargar las métricas');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    cargarMetricas();
  }, []);

  if (loading) return <div>Cargando...</div>;
  if (error) return <div>Error: {error}</div>;
  if (!metricas) return null;

  return (
    <div className="dashboard">
      <h1>📊 Panel de Administración</h1>
      
      <div className="metrics-grid">
        {/* Crecimiento de Usuarios */}
        <div className="metric-card">
          <h3>📈 Crecimiento de Usuarios</h3>
          <p className="value">
            +{metricas.crecimientoUsuarios.nuevosUsuariosEsteMes}
          </p>
          <p className="label">Nuevos usuarios este mes</p>
          <p className="secondary">
            {metricas.crecimientoUsuarios.tasaRetencion}% retención
          </p>
        </div>

        {/* Actividades Completadas */}
        <div className="metric-card">
          <h3>🎯 Actividades Completadas</h3>
          <p className="value">
            {metricas.actividadesCompletadas.actividadesCompletadas}
          </p>
          <p className="label">Actividades totales</p>
          <p className="secondary">
            {metricas.actividadesCompletadas.porcentajeCompletacion}% completación
          </p>
        </div>

        {/* Tiempo de Uso */}
        <div className="metric-card">
          <h3>⏱️ Tiempo de Uso</h3>
          <p className="value">
            {metricas.tiempoDeUso.promedioHorasDiarias}h
          </p>
          <p className="label">Promedio diario</p>
          <p className="secondary">
            {metricas.tiempoDeUso.porcentajeUsuariosActivos}% usuarios activos
          </p>
        </div>
      </div>
    </div>
  );
};

export default DashboardPage;
```

---

## 🔐 Configuración de Autenticación

### HTTP Interceptor (Angular)

```typescript
// auth.interceptor.ts
import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler } from '@angular/common/http';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler) {
    const token = localStorage.getItem('token');
    
    if (token) {
      req = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
    }
    
    return next.handle(req);
  }
}
```

### Axios Interceptor (React)

```typescript
// axiosConfig.ts
import axios from 'axios';

axios.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);
```

---

## 📌 Notas Importantes

1. **Autenticación**: Todos los endpoints requieren token JWT
2. **CORS**: Asegúrate de tener configurado CORS para tu dominio frontend
3. **Actualización**: Las métricas se calculan en tiempo real
4. **Performance**: Para grandes volúmenes, considera implementar caché
5. **Permisos**: Verifica que el usuario tenga rol de Coordinador/Admin

---

## ✅ Listo para usar

Los endpoints están funcionando y listos para consumir desde tu frontend Angular!
