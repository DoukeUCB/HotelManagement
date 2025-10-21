# Guía de Ejecución de Pruebas E2E - Cypress

## 🚀 Inicio Rápido

### 1. Prerequisitos

Asegúrate de tener **tres terminales** abiertas:

#### Terminal 1: Backend (.NET)
```powershell
cd C:\Users\blink\Documents\calidad\HotelManagement
dotnet run
```
El backend debe estar corriendo en `https://localhost:5001` o `http://localhost:5000`

#### Terminal 2: Frontend (Angular)
```powershell
cd C:\Users\blink\Documents\calidad\HotelManagement\frontend
npm start
```
El frontend debe estar corriendo en `http://localhost:4200`

#### Terminal 3: Cypress
```powershell
cd C:\Users\blink\Documents\calidad\HotelManagement\frontend
npx cypress open
```

---

## 📋 Suite de Pruebas Disponibles

### Archivo: `00-navigation.cy.ts`
**Smoke Tests y Navegación**
- Pruebas básicas de navegación
- Verificación de que la aplicación carga correctamente
- Tests de los botones "Volver"

### Archivo: `01-cliente-pairwise.cy.ts`
**19 Casos de Prueba para Cliente**
- ✅ 10 casos PAIRWISE
- ✅ 9 casos de Valores Límite

**Validaciones cubiertas:**
- Razón Social (obligatoria, 1-20 caracteres)
- NIT (obligatorio, 1-20 caracteres)
- Email (obligatorio, formato válido, único, 1-30 caracteres)

### Archivo: `02-huesped-pairwise.cy.ts`
**30 Casos de Prueba para Huésped**
- ✅ 15 casos PAIRWISE
- ✅ 15 casos de Valores Límite

**Validaciones cubiertas:**
- Nombre (obligatorio, 1-30 caracteres)
- Apellido (obligatorio, 1-30 caracteres)
- Segundo Apellido (opcional, 0-30 caracteres)
- Documento (obligatorio, 1-20 caracteres)
- Teléfono (opcional, 0-20 caracteres)
- Fecha Nacimiento (opcional, formato válido)

### Archivo: `03-reserva-pairwise.cy.ts`
**19 Casos de Prueba para Reserva**
- ✅ 14 casos PAIRWISE
- ✅ 5 casos de Valores Límite

**Validaciones cubiertas:**
- Cliente_ID (obligatorio, GUID válido)
- Monto_Total (>= 0, calculado automáticamente)
- Estado_Reserva (Pendiente, Confirmada, Cancelada, Completada, No-Show)

---

## 🎯 Cómo Ejecutar las Pruebas

### Opción 1: Modo Interactivo (Recomendado)

```powershell
cd frontend
npx cypress open
```

1. Se abrirá la interfaz de Cypress
2. Selecciona "E2E Testing"
3. Elige el navegador (Chrome recomendado)
4. Click en "Start E2E Testing in Chrome"
5. Selecciona el archivo de prueba que deseas ejecutar

**Ventajas:**
- ✅ Ves las pruebas ejecutándose en tiempo real
- ✅ Puedes pausar y depurar
- ✅ Capturas de pantalla automáticas en errores
- ✅ Time-travel debugging

### Opción 2: Modo Headless (CI/CD)

Ejecutar todas las pruebas:
```powershell
cd frontend
npx cypress run
```

Ejecutar un archivo específico:
```powershell
npx cypress run --spec "cypress/e2e/01-cliente-pairwise.cy.ts"
```

Ejecutar con navegador específico:
```powershell
npx cypress run --browser chrome
```

### Opción 3: Scripts NPM

```powershell
# Abrir Cypress interactivo
npm run cypress:open

# Ejecutar todas las pruebas (headless)
npm run cypress:run

# Iniciar frontend + ejecutar pruebas automáticamente
npm run e2e
```

---

## 📊 Interpretación de Resultados

### ✅ Prueba Exitosa
```
✓ TC-C01: Cliente válido - ÉXITO (1234ms)
```

### ❌ Prueba Fallida
```
✗ TC-C02: Email formato inválido - ERROR (543ms)
  AssertionError: Expected to find element: '.estado.error', but never found it.
```

### ⚠️ Prueba Saltada
```
- TC-C08: Email duplicado - ERROR (skipped)
```

---

## 🔧 Comandos Personalizados Disponibles

### `cy.fillClienteForm(razonSocial, nit, email)`
```typescript
cy.fillClienteForm('Hotel Luna', '1234567890', 'cliente@mail.com');
```

### `cy.fillHuespedForm(nombre, apellido, documento, options?)`
```typescript
cy.fillHuespedForm('Carlos', 'Gómez', '7894561', {
  segundoApellido: 'Pérez',
  telefono: '71234567',
  fechaNacimiento: '1998-05-12'
});
```

### `cy.waitForAngular()`
```typescript
cy.waitForAngular(); // Espera 500ms para que Angular renderice
```

---

## 📁 Estructura de Resultados

```
frontend/
├── cypress/
│   ├── screenshots/          # Capturas de pruebas fallidas
│   │   └── 01-cliente-pairwise.cy.ts/
│   │       └── TC-C02 -- Email formato invalido.png
│   ├── videos/              # Videos de ejecución (si están habilitados)
│   └── downloads/           # Archivos descargados durante pruebas
```

---

## 🐛 Troubleshooting

### Problema: "Timed out waiting for element"

**Solución:**
```typescript
// Aumentar timeout específico
cy.get('.estado.ok', { timeout: 15000 }).should('be.visible');

// O usar waitForAngular
cy.waitForAngular();
cy.get('.estado.ok').should('be.visible');
```

### Problema: "Email already exists"

**Solución:** Las pruebas ya usan timestamps únicos:
```typescript
const uniqueEmail = `cliente-${Date.now()}@mail.com`;
```

Si persiste, limpiar la base de datos:
```sql
DELETE FROM Cliente WHERE Email LIKE '%@mail.com';
```

### Problema: "Cannot find element"

**Checklist:**
1. ✅ ¿El backend está corriendo?
2. ✅ ¿El frontend está corriendo en localhost:4200?
3. ✅ ¿El selector es correcto?
4. ✅ ¿Angular ha terminado de renderizar?

**Debug:**
```typescript
cy.get('input[formcontrolname="razonSocial"]').debug();
cy.pause(); // Pausa la ejecución
```

### Problema: Cypress no abre

**Soluciones:**
```powershell
# Limpiar caché
npx cypress cache clear
npx cypress install

# Verificar instalación
npx cypress verify

# Reinstalar
npm uninstall cypress
npm install --save-dev cypress
```

---

## 📈 Métricas de Cobertura

| Categoría | Cantidad | Estado |
|-----------|----------|--------|
| **Total Casos de Prueba** | 68 | ✅ Implementados |
| Casos PAIRWISE | 39 | ✅ Implementados |
| Casos Valores Límite | 29 | ✅ Implementados |
| Smoke Tests | 9 | ✅ Implementados |

### Desglose por Módulo

| Módulo | PAIRWISE | Límite | Total |
|--------|----------|--------|-------|
| Cliente | 10 | 9 | 19 |
| Huésped | 15 | 15 | 30 |
| Reserva | 14 | 5 | 19 |

---

## 🎨 Mejores Prácticas

### ✅ Hacer

1. **Ejecutar en orden:** Navegación → Cliente → Huésped → Reserva
2. **Limpiar datos:** Reset BD antes de ejecutar suite completa
3. **Usar timestamps:** Para evitar duplicados
4. **Esperar Angular:** Usar `cy.waitForAngular()` después de navegación
5. **Capturas de pantalla:** Revisar carpeta `screenshots/` en errores

### ❌ Evitar

1. **No hardcodear IDs:** Usar selectores semánticos
2. **No ignorar timeouts:** Investigar la causa raíz
3. **No ejecutar con BD productiva:** Usar BD de pruebas
4. **No omitir prerequisitos:** Backend y Frontend deben estar corriendo

---

## 📝 Registro de Resultados

Template para documentar ejecución:

```markdown
## Ejecución de Pruebas - [FECHA]

### Ambiente
- Backend: ✅ Corriendo
- Frontend: ✅ Corriendo
- Base de Datos: ✅ Limpia

### Resultados
- Total: 68 pruebas
- Pasadas: 65 ✅
- Fallidas: 3 ❌
- Saltadas: 0 ⚠️

### Fallos
1. TC-C08: Email duplicado - ERROR
   - Causa: Dato preexistente en BD
   - Solución: Limpiar BD

2. TC-H12: Formato fecha inválido - ERROR
   - Causa: Input HTML5 rechaza formato automáticamente
   - Acción: Revisar validación frontend

3. TC-R07: Monto negativo - ERROR
   - Causa: Input permite negativos
   - Acción: Agregar validación min="0"

### Observaciones
- Tiempo total ejecución: 5m 34s
- No se encontraron errores críticos
- Cobertura completa de casos PAIRWISE
```

---

## 🔗 Referencias

- **Plan de Pruebas:** `Tests.md` (raíz del proyecto)
- **Documentación Cypress:** `frontend/cypress/README.md`
- **Configuración:** `frontend/cypress.config.ts`
- **Comandos Custom:** `frontend/cypress/support/commands.ts`

---

## 📞 Soporte

Para reportar bugs o solicitar mejoras en las pruebas:
1. Crear issue en GitHub
2. Incluir logs de Cypress
3. Adjuntar capturas de pantalla
4. Especificar navegador y versión

---

**Creado:** 20 de octubre de 2025  
**Versión:** 1.0  
**Proyecto:** HotelManagement - Sistema de Gestión Hotelera
