# ANÁLISIS DE COSTOS DE INFRAESTRUCTURA
## AdminProyectos NATURA - Google Cloud Platform

**Fecha de análisis:** 2025-10-06
**Región:** us-central1 (Iowa, USA)
**Servidor:** dev-server (e2-standard-2)

---

## RESUMEN EJECUTIVO

### 💰 Costo Estimado Mensual Total
**$40 - $50 USD/mes** (solo para AdminProyectos)

> **Nota:** El servidor actual (dev-server) es compartido con múltiples aplicaciones. El costo real prorrateado para AdminProyectos específicamente es de **$15-20 USD/mes**.

---

## 1. RECURSOS UTILIZADOS POR ADMINPROYECTOS

### 📊 Uso de Recursos en Servidor Compartido

| Componente | CPU | Memoria RAM | Disco | % del Servidor |
|------------|-----|-------------|-------|----------------|
| **adminproyectos-web** | 0.08% | 113 MB | 307 MB | ~2% CPU, 1.4% RAM |
| **adminproyectos-sqlserver** | 1.67% | 808 MB | - | ~3% CPU, 10.4% RAM |
| **Volúmenes de datos** | - | - | 166 MB | - |
| **TOTAL AdminProyectos** | ~1.75% | 921 MB | 473 MB | ~5% CPU, 11.8% RAM |

### 💾 Almacenamiento Detallado
```
Imagen Docker (web):         307 MB
SQL Server base:              ~2 GB (del sistema)
Volumen sqlserver-data:       95.52 MB (base de datos)
Volumen logs-data:            62.41 MB (logs aplicación)
Volumen uploads-data:         8.45 MB (archivos Brief)
─────────────────────────────────────
TOTAL estimado:               ~2.5 GB
```

---

## 2. SERVIDOR ACTUAL (COMPARTIDO)

### Especificaciones: e2-standard-2
- **vCPUs:** 2 cores
- **RAM:** 8 GB
- **Disco:** 50 GB SSD
- **Región:** us-central1-c (Iowa)
- **IP Estática:** Incluida

### 📈 Uso Total del Servidor (Todas las Apps)
```
Aplicaciones activas: 24 contenedores
- AdminProyectos: 2 contenedores (5% recursos)
- Matomo + MySQL: 2 contenedores
- Mautic + MySQL: 2 contenedores
- n8n Marketing: 1 contenedor
- WikiJS + DB: 2 contenedores
- Monitoring Stack: 11 contenedores (Prometheus, Grafana, etc.)
- Traefik + Watchtower: 2 contenedores
```

### 💵 Costo del Servidor Completo
| Concepto | Precio Mensual |
|----------|----------------|
| e2-standard-2 (730 hrs/mes) | $48.91 USD |
| Disco SSD 50 GB | $8.50 USD |
| IP Estática | $7.20 USD |
| Tráfico Egress (~50 GB/mes) | $4.50 USD |
| **TOTAL SERVIDOR** | **$69.11 USD/mes** |

### 💡 Costo Prorrateado AdminProyectos
Si se prorratea por uso de recursos (11.8% RAM):
- **$69.11 × 11.8% = ~$8.15 USD/mes**

---

## 3. ESCENARIOS DE INFRAESTRUCTURA DEDICADA

### 🏢 OPCIÓN 1: Servidor Dedicado Pequeño (e2-micro)
**Especificaciones:**
- vCPUs: 2 cores compartidos (0.25-0.5 vCPU burst)
- RAM: 1 GB
- Disco: 20 GB SSD

**⚠️ NO RECOMENDADO** - SQL Server 2022 requiere mínimo 2 GB RAM

### 🏢 OPCIÓN 2: Servidor Dedicado Básico (e2-small) ⭐ RECOMENDADO
**Especificaciones:**
- vCPUs: 2 cores compartidos (0.5-1.0 vCPU burst)
- RAM: 2 GB
- Disco: 20 GB SSD

| Concepto | Precio Mensual |
|----------|----------------|
| e2-small (730 hrs/mes) | $13.36 USD |
| Disco SSD 20 GB | $3.40 USD |
| IP Estática | $7.20 USD |
| Tráfico Egress (~10 GB/mes) | $0.90 USD |
| Backup automatizado (opcional) | $2.00 USD |
| **TOTAL MENSUAL** | **$24.86 USD/mes** |
| **TOTAL ANUAL** | **$298.32 USD/año** |

**✅ Justificación:**
- Cumple requisitos mínimos SQL Server Express (2 GB RAM)
- Suficiente para 10-20 usuarios concurrentes
- Margen para crecimiento de base de datos
- 100% recursos dedicados

### 🏢 OPCIÓN 3: Servidor Dedicado Óptimo (e2-medium)
**Especificaciones:**
- vCPUs: 2 cores compartidos (1.0-2.0 vCPU burst)
- RAM: 4 GB
- Disco: 30 GB SSD

| Concepto | Precio Mensual |
|----------|----------------|
| e2-medium (730 hrs/mes) | $26.73 USD |
| Disco SSD 30 GB | $5.10 USD |
| IP Estática | $7.20 USD |
| Tráfico Egress (~15 GB/mes) | $1.35 USD |
| Backup automatizado | $3.00 USD |
| **TOTAL MENSUAL** | **$43.38 USD/mes** |
| **TOTAL ANUAL** | **$520.56 USD/año** |

**✅ Ventajas:**
- Rendimiento mejorado para SQL Server
- Soporte para 30-50 usuarios concurrentes
- Mayor capacidad para crecimiento de datos
- Respuesta más rápida en consultas complejas

### 🏢 OPCIÓN 4: Servidor Dedicado Robusto (e2-standard-2)
**Especificaciones:**
- vCPUs: 2 cores dedicados
- RAM: 8 GB
- Disco: 50 GB SSD

| Concepto | Precio Mensual |
|----------|----------------|
| e2-standard-2 (730 hrs/mes) | $48.91 USD |
| Disco SSD 50 GB | $8.50 USD |
| IP Estática | $7.20 USD |
| Tráfico Egress (~20 GB/mes) | $1.80 USD |
| Backup automatizado | $5.00 USD |
| **TOTAL MENSUAL** | **$71.41 USD/mes** |
| **TOTAL ANUAL** | **$856.92 USD/año** |

**✅ Ventajas:**
- Alta disponibilidad y rendimiento
- 50-100 usuarios concurrentes
- Espacio para múltiples entornos (dev/staging/prod)
- Ideal para crecimiento a largo plazo

---

## 4. COSTOS ADICIONALES POTENCIALES

### 📧 Servicio de Email (Actual: Gmail SMTP - GRATIS)
Alternativas profesionales:
- **SendGrid:** $15-20 USD/mes (40,000 emails/mes)
- **AWS SES:** $0.10 por 1,000 emails
- **Gmail Workspace:** Incluido si ya tienen suscripción

### 💾 Backups Incrementales
- **Google Cloud Storage (Nearline):** $0.01/GB/mes
- Backup diario estimado: ~500 MB → **$0.50 USD/mes**
- Retención 30 días: ~15 GB → **$1.50 USD/mes**

### 🔐 Certificado SSL
- **Let's Encrypt vía Traefik:** GRATIS ✅
- Renovación automática incluida

### 📊 Monitoreo (Actual: Stack Prometheus/Grafana - GRATIS)
Alternativas:
- **Google Cloud Monitoring:** Incluido básico, $0.30/GB logs adicionales
- **New Relic:** $99 USD/mes (overkill para esta app)
- **Datadog:** $15 USD/mes/host

---

## 5. COMPARATIVA DE ESCENARIOS

### 📊 Tabla Comparativa Anualizada

| Escenario | Mensual | Anual | RAM | Usuarios | Recomendación |
|-----------|---------|-------|-----|----------|---------------|
| **Servidor Compartido (actual)** | $8-15 | $96-180 | 921 MB usado de 8 GB | 10-20 | ✅ Costo-efectivo actual |
| **e2-small Dedicado** | $25 | $300 | 2 GB | 10-20 | ⭐ **RECOMENDADO inicio** |
| **e2-medium Dedicado** | $43 | $520 | 4 GB | 30-50 | ✅ Balance precio/rendimiento |
| **e2-standard-2 Dedicado** | $71 | $857 | 8 GB | 50-100 | 💼 Producción enterprise |

---

## 6. PROYECCIÓN DE COSTOS POR CRECIMIENTO

### 📈 Escenario: Crecimiento de Usuarios y Datos

| Fase | Usuarios | Briefs/mes | Almacenamiento | Servidor Recomendado | Costo/mes |
|------|----------|------------|----------------|----------------------|-----------|
| **Inicial** | 5-10 | 20-50 | 5 GB | e2-small | $25 |
| **Crecimiento** | 15-30 | 100-200 | 15 GB | e2-medium | $43 |
| **Consolidado** | 30-50 | 300-500 | 30 GB | e2-medium (30GB disk) | $46 |
| **Expansión** | 50-100 | 500+ | 50 GB | e2-standard-2 | $71 |

### 💡 Estimación Conservadora 12 Meses
```
Mes 1-3:   e2-small     → $25/mes × 3  = $75
Mes 4-8:   e2-medium    → $43/mes × 5  = $215
Mes 9-12:  e2-medium    → $46/mes × 4  = $184
────────────────────────────────────────────
TOTAL AÑO 1:                              $474 USD
```

---

## 7. OPTIMIZACIONES PARA REDUCIR COSTOS

### 💰 Estrategias de Ahorro

1. **Commitment Use Discounts (1 año)**
   - e2-small: $13.36 → **$9.16/mes** (31% descuento)
   - e2-medium: $26.73 → **$18.33/mes** (31% descuento)
   - **Ahorro anual: $100-120 USD**

2. **Preemptible VMs (NO recomendado para producción)**
   - Hasta 80% descuento
   - Se pueden interrumpir en cualquier momento
   - **NO aplicable para aplicación crítica**

3. **Almacenamiento Nearline para Backups**
   - Standard: $0.020/GB/mes
   - Nearline: $0.010/GB/mes
   - **Ahorro: 50% en backups históricos**

4. **Tráfico Regional**
   - Usar CDN de Cloudflare (gratis)
   - Cachear contenido estático
   - **Reducción 30-50% tráfico egress**

5. **Consolidación de Logs**
   - Rotación logs cada 7 días
   - Compresión automática
   - **Reducción 40% uso disco**

---

## 8. RECOMENDACIÓN FINAL

### ⭐ RECOMENDACIÓN OFICIAL: e2-medium Dedicado

**Justificación:**
1. ✅ Balance óptimo precio/rendimiento: **$43 USD/mes**
2. ✅ 4 GB RAM suficiente para SQL Server + aplicación
3. ✅ Escalabilidad para 30-50 usuarios sin cambios
4. ✅ Margen para picos de carga y crecimiento de datos
5. ✅ Permite ambiente staging opcional en mismo servidor

### 📋 Plan de Implementación Sugerido

**FASE 1 (Mes 1-3): Mantener Servidor Compartido**
- Costo: $8-15 USD/mes
- Objetivo: Validar adopción y uso real

**FASE 2 (Mes 4): Migrar a e2-medium Dedicado**
- Costo: $43 USD/mes
- Activar Commitment de 1 año → $30 USD/mes
- Configurar backups automatizados

**FASE 3 (Mes 6+): Optimizar según métricas**
- Revisar uso real de recursos
- Ajustar plan si es necesario
- Considerar escalado vertical/horizontal

---

## 9. COMPARATIVA CON OTRAS PLATAFORMAS

### 🔄 Google Cloud vs Competencia (e2-medium equivalente)

| Proveedor | Especificaciones | Precio Mensual |
|-----------|------------------|----------------|
| **Google Cloud** (e2-medium) | 2 vCPU, 4 GB RAM, 30 GB | $43 USD |
| **AWS** (t3.medium) | 2 vCPU, 4 GB RAM, 30 GB | $52 USD |
| **Azure** (B2s) | 2 vCPU, 4 GB RAM, 30 GB | $48 USD |
| **DigitalOcean** (4 GB Droplet) | 2 vCPU, 4 GB RAM, 80 GB | $24 USD ⭐ |
| **Linode** (4 GB Plan) | 2 vCPU, 4 GB RAM, 80 GB | $24 USD ⭐ |
| **Vultr** (4 GB Plan) | 2 vCPU, 4 GB RAM, 80 GB | $24 USD ⭐ |

**💡 Alternativa Económica:**
- **DigitalOcean/Linode/Vultr:** ~$24 USD/mes (ahorro $19/mes)
- **Contras:** Sin integración GCP, migración necesaria
- **Pros:** Más almacenamiento, mejor precio/rendimiento

---

## 10. CONCLUSIONES Y RESUMEN

### 📊 Costo Real Actual
- **Servidor compartido:** $8-15 USD/mes (prorrateado)
- **Uso de recursos:** 11.8% del servidor
- **Estado:** ✅ Funcional y costo-efectivo

### 💰 Costo Proyectado Servidor Dedicado
- **Recomendado:** e2-medium = **$43 USD/mes** ($30/mes con commitment)
- **Primera opción alternativa:** e2-small = $25 USD/mes
- **Opción premium:** e2-standard-2 = $71 USD/mes

### 🎯 Decisión de Negocio

**Mantener servidor compartido SI:**
- ✅ Presupuesto ajustado
- ✅ Menos de 15 usuarios activos
- ✅ No hay requisitos de SLA estrictos

**Migrar a servidor dedicado SI:**
- ✅ Más de 20 usuarios concurrentes
- ✅ Datos sensibles/críticos
- ✅ Necesidad de control total
- ✅ Presupuesto disponible de $40-50 USD/mes

### 📈 ROI Estimado
Con 20 usuarios ahorrando 2 horas/mes c/u:
- Ahorro tiempo: 40 horas/mes
- Valor hora promedio: $15 USD
- **ROI mensual: $600 USD vs $43 costo = 1,295% ROI**

---

**Reporte generado:** 2025-10-06
**Próxima revisión recomendada:** 2025-11-06 (1 mes)
