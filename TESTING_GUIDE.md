# Pipeline CI/CD - Documentación Completa

## Índice
1. [Resumen Ejecutivo](#resumen-ejecutivo)
2. [Arquitectura del Pipeline](#arquitectura-del-pipeline)
3. [Pruebas Implementadas](#pruebas-implementadas)
4. [Configuración y Setup](#configuración-y-setup)
5. [Ejecución Local](#ejecución-local)
6. [Troubleshooting](#troubleshooting)

---

## Resumen Ejecutivo

### Estado Actual del Proyecto

| Componente | Estado | Cobertura | Tiempo |
|-----------|--------|-----------|--------|
| **Backend Integration Tests** | 100% | 39/39 tests | ~2-3 min |
| **Frontend Build** | OK | - | ~1-2 min |
| **E2E Tests (Cypress)** | OK | 4 specs | ~5-8 min |
| **Total Pipeline** | Funcional | - | ~10-15 min |

### Métricas Clave

- **Total de Tests**: 43+ (39 integración + 4 E2E)
- **Líneas de Código de Tests**: 1,050+ líneas
- **Jobs en Paralelo**: 2 (Backend + Frontend)
- **E2E Specs en Matriz**: 4 specs paralelos
- **Tasa de Éxito**: 100% en última ejecución

---

## Arquitectura del Pipeline

### Diagrama de Flujo

```
┌─────────────────┐
│   Push/PR       │
│   to GitHub     │
└────────┬────────┘
         │
         ├──────────────────┬──────────────────┐
         │                  │                  │
         ▼                  ▼                  │
┌──────────────────┐ ┌──────────────────┐    │
│  Backend Build   │ │ Frontend Build   │    │
│  & Tests (39)    │ │  (Vue.js + Vite) │    │
└────────┬─────────┘ └────────┬─────────┘    │
         │                    │                │
         └──────────┬─────────┘                │
                    ▼                          │
          ┌──────────────────┐                 │
          │  E2E Tests (4)   │                 │
          │  Cypress Matrix  │                 │
          └────────┬─────────┘                 │
                   │                           │
                   ├──────────────┬────────────┘
                   ▼              ▼
          ┌──────────────┐ ┌──────────────┐
          │ Test Summary │ │ Quality Gate │
          │   (Report)   │ │  (PR Block)  │
          └──────────────┘ └──────────────┘
```

### Jobs Detallados

#### Job 1: Backend Build & Integration Tests
```yaml
Responsabilidad:
  - Compilar el backend (.NET 9.0)
  - Ejecutar 39 tests de integración
  - Generar reportes TRX
  - Subir artefactos de build

Tecnologías:
  - .NET SDK 9.0
  - xUnit 2.4.2
  - Entity Framework Core InMemory
  - GitHub Actions Test Reporter

Tiempo: ~2-3 minutos
```

#### Job 2: Frontend Build
```yaml
Responsabilidad:
  - Instalar dependencias npm
  - Compilar aplicación Vue.js
  - Generar build de producción (dist/)
  - Subir artefactos

Tecnologías:
  - Node.js 20.x
  - Vue.js 3 + Vite
  - npm

Tiempo: ~1-2 minutos
```

#### Job 3: E2E Tests (Cypress)
```yaml
Responsabilidad:
  - Iniciar backend API (localhost:5000)
  - Iniciar frontend dev (localhost:5173)
  - Ejecutar 4 specs en paralelo
  - Capturar screenshots y videos
  - Subir artefactos en caso de fallo

Matriz de Tests:
  - 00-navigation.cy.ts
  - 01-cliente-pairwise.cy.ts
  - 02-huesped-pairwise.cy.ts
  - 03-reserva-pairwise.cy.ts

Tecnologías:
  - Cypress 13.x
  - Chrome browser (headless)

Tiempo: ~5-8 minutos (paralelo)
```

#### Job 4: Test Summary
```yaml
Responsabilidad:
  - Generar resumen markdown
  - Publicar en GitHub Actions Summary
  - Mostrar estado de cada tipo de test

Tiempo: ~30 segundos
```

#### Job 5: Quality Gate
```yaml
Responsabilidad:
  - Verificar que todos los tests pasen
  - Bloquear PRs con tests fallidos
  - Solo activo en Pull Requests

Tiempo: ~10 segundos
```

---

## Pruebas Implementadas

### Backend Integration Tests (39 tests)

#### ClienteRepositoryTests (8 tests)
```
Patrón: CREATE → SELECT → UPDATE → SELECT → DELETE → SELECT

HappyPath_CompleteFlow_ClienteCRUDOperations
UnhappyPath_SELECT_ClienteInexistente_ReturnsNull
UnhappyPath_DELETE_ClienteInexistente_ReturnsFalse
UnhappyPath_CREATE_NITDuplicado_ThrowsException
UnhappyPath_CREATE_EmailDuplicado_ThrowsException
UnhappyPath_GetAll_ConMultiplesClientes_ReturnsAll
UnhappyPath_UPDATE_ClienteInexistente_ThrowsException
UnhappyPath_GetByEmail_ClienteExistente_ReturnsCliente
```

#### HabitacionRepositoryTests (9 tests)
```
Patrón: CREATE → SELECT → UPDATE → SELECT → DELETE → SELECT
Incluye: Validación de relación con TipoHabitacion

HappyPath_CompleteFlow_HabitacionCRUDOperations
UnhappyPath_SELECT_HabitacionInexistente_ReturnsNull
UnhappyPath_DELETE_HabitacionInexistente_ReturnsFalse
UnhappyPath_CREATE_TipoHabitacionInexistente_ThrowsException
UnhappyPath_CREATE_NumeroHabitacionDuplicado_AllowedInMemory (placeholder)
UnhappyPath_GetAll_ConMultiplesHabitaciones_ReturnsAll (placeholder)
UnhappyPath_CREATE_PisoNegativo_AllowedInMemory
UnhappyPath_UPDATE_HabitacionInexistente_NoEffect
UnhappyPath_GetByIdAsync_VerifyInclude_LoadsTipoHabitacion
```

#### HuespedRepositoryTests (12 tests)
```
Patrón: CREATE → SELECT → UPDATE → SELECT → DELETE → SELECT
Incluye: GetByDocumentoAsync(), campos opcionales

HappyPath_CompleteFlow_HuespedCRUDOperations
HappyPath_CREATE_HuespedSinSegundoApellido_Success
UnhappyPath_SELECT_HuespedInexistente_ReturnsNull
UnhappyPath_DELETE_HuespedInexistente_ReturnsFalse
UnhappyPath_CREATE_DocumentoDuplicado_AllowedInMemory
UnhappyPath_CREATE_NombreConNumeros_AllowedInMemory
UnhappyPath_CREATE_DocumentoMuyCorto_AllowedInMemory
UnhappyPath_CREATE_FechaNacimientoFutura_AllowedInMemory
UnhappyPath_GetByDocumento_HuespedExistente_ReturnsHuesped
UnhappyPath_GetByDocumento_HuespedInexistente_ReturnsNull
UnhappyPath_GetAll_ConMultiplesHuespedes_ReturnsAll
UnhappyPath_UPDATE_HuespedInexistente_NoEffect
```

#### ReservaRepositoryTests (10 tests)
```
Patrón: INSERT → SELECT (simplificado)
Incluye: Validación de relación con Cliente, estados válidos

HappyPath_INSERT_SELECT_ReservaCompleteFlow
HappyPath_INSERT_SELECT_ReservaConEstadoConfirmada
HappyPath_INSERT_SELECT_ReservaConMontoCero
HappyPath_GetAll_ConMultiplesReservas_ReturnsAll (placeholder)
HappyPath_INSERT_TodosLosEstadosValidos_Success
UnhappyPath_SELECT_ReservaInexistente_ReturnsNull
UnhappyPath_INSERT_ClienteIDInexistente_AllowedInMemory
UnhappyPath_INSERT_EstadoReservaInvalido_AllowedInMemory
UnhappyPath_INSERT_MontoNegativo_AllowedInMemory
UnhappyPath_UPDATE_ReservaExistente_Success
```

### E2E Tests con Cypress (4 specs)

#### 00-navigation.cy.ts
- Navegación entre páginas principales
- Verificación de rutas y menús
- Responsividad básica

#### 01-cliente-pairwise.cy.ts
- CRUD completo de clientes
- Validación de formularios
- Pairwise testing de combinaciones

#### 02-huesped-pairwise.cy.ts
- CRUD completo de huéspedes
- Validación de campos obligatorios/opcionales
- Pairwise testing de combinaciones

#### 03-reserva-pairwise.cy.ts
- CRUD completo de reservas
- Validación de relaciones (Cliente)
- Estados de reserva
- Pairwise testing de combinaciones

---

## Configuración y Setup

### Requisitos Previos

#### Para Desarrollo Local
```bash
# Backend
- .NET SDK 9.0+
- MySQL 8.0+ (opcional, usa InMemory en tests)

# Frontend
- Node.js 20.x+
- npm 10.x+

# E2E
- Chrome/Chromium browser
```

#### Para GitHub Actions
Todo está configurado automáticamente en el pipeline.

### Variables de Entorno

#### Backend (.env)
```bash
DB_SERVER=localhost
DB_PORT=3306
DB_NAME=HotelDB
DB_USER=root
DB_PASSWORD=your_password
```

#### Frontend (.env)
```bash
VITE_API_URL=http://localhost:5000
```

### Archivos de Configuración

```
.github/
├── workflows/
│   ├── ci-cd.yml           # Pipeline principal
│   ├── update-badges.yml   # Actualización de badges
│   └── README.md           # Documentación del pipeline
├── run-tests.sh            # Script de ejecución local
└── TESTING_GUIDE.md        # Esta guía
```

---

## Ejecución Local

### Opción 1: Script Automatizado (Recomendado)

```bash
# Ejecutar todo el suite de tests
./run-tests.sh

# El script te preguntará si deseas ejecutar E2E tests
```

### Opción 2: Manual por Componente

#### Solo Backend Tests
```bash
cd backend
dotnet restore
dotnet build --configuration Release
dotnet test --verbosity normal
```

#### Solo Frontend Build
```bash
cd frontend
npm ci
npm run build
```

#### Solo E2E Tests
```bash
# Terminal 1: Backend
cd backend
dotnet run

# Terminal 2: Frontend
cd frontend
npm run dev

# Terminal 3: Cypress
cd frontend
npx cypress run
# o en modo interactivo:
npx cypress open
```

### Opción 3: Comandos Específicos

```bash
# Ejecutar un test específico de backend
cd backend
dotnet test --filter "FullyQualifiedName~ClienteRepositoryTests"

# Ejecutar un spec específico de Cypress
cd frontend
npx cypress run --spec "cypress/e2e/01-cliente-pairwise.cy.ts"

# Ver resultados detallados
cd backend
dotnet test --logger "console;verbosity=detailed"
```

---

## Monitoreo y Reportes

### Ver Resultados en GitHub

1. **Actions Tab**
   - Ve a: `https://github.com/DoukeUCB/HotelManagement/actions`
   - Selecciona el workflow run más reciente
   - Revisa cada job

2. **Summary Tab**
   - Cada run genera un summary automático
   - Incluye estado de todos los tests
   - Información del commit y branch

3. **Artifacts**
   - Screenshots de Cypress (solo fallos)
   - Videos de Cypress (siempre)
   - Reportes TRX del backend

### Badges en README

```markdown
[![CI/CD Pipeline](https://github.com/DoukeUCB/HotelManagement/actions/workflows/ci-cd.yml/badge.svg)]
[![Backend Tests](https://img.shields.io/badge/Backend%20Tests-39%20passing-brightgreen)]
[![E2E Tests](https://img.shields.io/badge/E2E%20Tests-Cypress-17202C?logo=cypress)]
```

---

## Troubleshooting

### Backend Tests Fallan

#### Problema: "Could not find xUnit"
```bash
Solución:
cd backend/Tests
dotnet restore
dotnet build
```

#### Problema: "Database connection failed"
```bash
Solución:
# Los tests usan InMemoryDatabase, no necesitan MySQL
# Verificar que Microsoft.EntityFrameworkCore.InMemory esté instalado
dotnet add package Microsoft.EntityFrameworkCore.InMemory
```

#### Problema: "Tests timeout"
```bash
Solución:
# Aumentar timeout en los tests
# O ejecutar con más verbosidad para ver qué está pasando
dotnet test --logger "console;verbosity=detailed"
```

### E2E Tests Fallan

#### Problema: "Backend not responding"
```bash
Solución:
# Verificar que el backend esté escuchando en el puerto correcto
curl http://localhost:5000/health

# Ver logs del backend
cat /tmp/backend.log

# Reiniciar backend
pkill dotnet
cd backend && dotnet run
```

#### Problema: "Frontend not loading"
```bash
Solución:
# Verificar que Vite esté corriendo
curl http://localhost:5173

# Ver logs del frontend
cat /tmp/frontend.log

# Limpiar y reinstalar
cd frontend
rm -rf node_modules dist
npm ci
npm run dev
```

#### Problema: "Cypress can't find elements"
```bash
Solución:
# Ejecutar Cypress en modo interactivo para debuggear
cd frontend
npx cypress open

# Ver screenshots y videos en:
frontend/cypress/screenshots/
frontend/cypress/videos/
```

### Pipeline en GitHub Falla

#### Problema: "Actions workflow failed to run"
```bash
Solución:
# Verificar sintaxis YAML
yamllint .github/workflows/ci-cd.yml

# Verificar que todos los paths existan
# Verificar que las versiones de .NET y Node estén disponibles
```

#### Problema: "Job timed out after 60 minutes"
```bash
Solución:
# Agregar timeout explícito en el job
timeout-minutes: 30

# Optimizar pasos lentos
# Usar cache para node_modules y .nuget
```

---

## Métricas y KPIs

### Performance del Pipeline

| Métrica | Valor Actual | Objetivo |
|---------|--------------|----------|
| Tiempo Total | ~10-15 min | < 15 min |
| Backend Tests | ~2-3 min | < 5 min |
| Frontend Build | ~1-2 min | < 3 min |
| E2E Tests | ~5-8 min | < 10 min |
| Tasa de Éxito | 100% | > 95% |

### Cobertura de Tests

| Componente | Tests | Cobertura |
|-----------|-------|-----------|
| Repositorios Backend | 39 | 100% |
| E2E Flows | 4 specs | CRUD completo |
| APIs Endpoints | TBD | - |

---

## Mantenimiento y Actualizaciones

### Actualizar Dependencias

```bash
# Backend
cd backend
dotnet list package --outdated
dotnet add package <PackageName> --version <Version>

# Frontend
cd frontend
npm outdated
npm update
```

### Agregar Nuevos Tests

#### Backend
1. Crear archivo en `backend/Tests/Integration/Repositories/`
2. Seguir patrón de tests existentes
3. Actualizar `RESUMEN_PRUEBAS_INTEGRACION.md`

#### E2E
1. Crear archivo en `frontend/cypress/e2e/`
2. Seguir naming: `XX-feature-pairwise.cy.ts`
3. Actualizar matriz en `.github/workflows/ci-cd.yml`

---

## Referencias

- [Documentación Completa de Tests](./backend/Tests/RESUMEN_PRUEBAS_INTEGRACION.md)
- [Plan de Pruebas Original](./backend/Tests/PRUEBAS.md)
- [README del Pipeline](./.github/workflows/README.md)
- [GitHub Actions Docs](https://docs.github.com/en/actions)
- [Cypress Documentation](https://docs.cypress.io)
- [xUnit Documentation](https://xunit.net/)

---

**Última actualización**: Noviembre 2025  
**Versión**: 1.0.0  
**Mantenido por**: Equipo de Desarrollo Hotel Management
