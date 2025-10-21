# 🚀 Guía Rápida de Ejecución - Cypress Tests

## ✅ Checklist Pre-Ejecución

Antes de ejecutar las pruebas, verifica:

- [ ] Backend corriendo en `http://localhost:5000` o `https://localhost:5001`
- [ ] Frontend corriendo en `http://localhost:4200`
- [ ] Base de datos accesible y limpia (opcional para evitar conflictos)
- [ ] Node modules instalados en `frontend/`

---

## 🎯 Ejecución Rápida

### Opción 1: Script PowerShell (Recomendado)

```powershell
cd frontend
.\run-tests.ps1
```

Este script te mostrará un menú interactivo con opciones:
1. Ejecutar TODAS las pruebas (Headless)
2. Abrir Cypress (Modo Interactivo)
3. Ejecutar solo pruebas de Cliente
4. Ejecutar solo pruebas de Huésped
5. Ejecutar solo pruebas de Reserva
6. Ejecutar solo pruebas de Navegación

### Opción 2: Comandos Directos

**Abrir Cypress (Modo visual - RECOMENDADO para debugging)**
```powershell
cd frontend
npx cypress open
```

**Ejecutar todas las pruebas (Headless)**
```powershell
cd frontend
npx cypress run
```

**Ejecutar pruebas específicas**
```powershell
# Solo Cliente
npx cypress run --spec "cypress/e2e/01-cliente-pairwise.cy.ts"

# Solo Huésped
npx cypress run --spec "cypress/e2e/02-huesped-pairwise.cy.ts"

# Solo Reserva
npx cypress run --spec "cypress/e2e/03-reserva-pairwise.cy.ts"

# Solo Navegación
npx cypress run --spec "cypress/e2e/00-navigation.cy.ts"
```

---

## 📋 Verificación de Rutas

### Rutas Corregidas ✅

| Prueba | Ruta Correcta | Estado |
|--------|---------------|--------|
| Nuevo Cliente | `/nuevo-cliente` | ✅ Corregido |
| Nuevo Huésped | `/nuevo-huesped` | ✅ Corregido |
| Nueva Reserva | `/nueva-reserva` | ✅ Corregido |
| Listar Clientes | `/clientes` | ✅ OK |
| Listar Huéspedes | `/huespedes` | ✅ OK |
| Listar Reservas | `/reservas` | ✅ OK |

### APIs Backend Verificadas ✅

| Endpoint | Método | Ruta | Estado |
|----------|--------|------|--------|
| Cliente | POST | `/api/Cliente` | ✅ OK |
| Cliente | GET | `/api/Cliente` | ✅ OK |
| Huésped | POST | `/api/Huesped` | ✅ OK |
| Huésped | GET | `/api/Huesped` | ✅ OK |
| Reserva | POST | `/api/Reserva` | ✅ OK |
| Reserva | GET | `/api/Reserva` | ✅ OK |

---

## 🐛 Solución de Problemas Comunes

### ❌ Error: "Cannot GET /clientes/nuevo"

**Causa**: Ruta incorrecta (era `/clientes/nuevo`, ahora es `/nuevo-cliente`)

**Solución**: Las rutas ya están corregidas en los archivos de prueba.

### ❌ Error: "Timed out waiting for element"

**Causa**: Angular no ha terminado de renderizar o el selector es incorrecto

**Solución**:
```typescript
// Usar waitForAngular
cy.waitForAngular();
cy.get('input[formcontrolname="razonSocial"]').should('exist');
```

### ❌ Error: "Email already exists"

**Causa**: Email duplicado en la base de datos

**Solución**: Las pruebas usan timestamps únicos:
```typescript
const uniqueEmail = `cliente-${Date.now()}@mail.com`;
```

Si persiste, limpiar la BD:
```sql
DELETE FROM Cliente WHERE Email LIKE '%@mail.com';
DELETE FROM Huesped;
DELETE FROM Reserva;
```

### ❌ Error: "Cannot find module"

**Causa**: Dependencies no instaladas

**Solución**:
```powershell
cd frontend
npm install
```

### ❌ Backend no responde

**Verificar**:
```powershell
# Terminal 1 - Backend
cd C:\Users\blink\Documents\calidad\HotelManagement
dotnet run

# Debe mostrar:
# Now listening on: http://localhost:5000
# Now listening on: https://localhost:5001
```

### ❌ Frontend no responde

**Verificar**:
```powershell
# Terminal 2 - Frontend
cd C:\Users\blink\Documents\calidad\HotelManagement\frontend
npm start

# Debe mostrar:
# ** Angular Live Development Server is listening on localhost:4200
```

---

## 📊 Selectores Actualizados

### Formulario Cliente
```typescript
cy.get('input[formcontrolname="razonSocial"]')  // Razón Social
cy.get('input[formcontrolname="nit"]')          // NIT
cy.get('input[formcontrolname="email"]')        // Email
cy.get('button[type="submit"]')                  // Botón Guardar
cy.get('.estado.ok')                             // Mensaje éxito
cy.get('.estado.error')                          // Mensaje error
cy.get('small')                                  // Mensaje validación
```

### Formulario Huésped
```typescript
cy.get('input[formcontrolname="primerNombre"]')    // Nombre
cy.get('input[formcontrolname="segundoNombre"]')   // Segundo Nombre (opcional)
cy.get('input[formcontrolname="primerApellido"]')  // Apellido
cy.get('input[formcontrolname="segundoApellido"]') // Segundo Apellido (opcional)
cy.get('input[formcontrolname="documento"]')       // Documento
cy.get('input[formcontrolname="telefono"]')        // Teléfono (opcional)
cy.get('input[formcontrolname="fechaNacimiento"]') // Fecha Nacimiento (opcional)
cy.get('button[type="submit"]')                     // Botón Guardar
```

### Formulario Reserva (Multi-paso)
```typescript
// Paso 1: Cliente
cy.get('input.cliente-search')                  // Buscador de cliente
cy.get('.suggestions li')                       // Sugerencias de cliente
cy.get('select[formcontrolname="estadoReserva"]') // Estado
cy.contains('button', 'Continuar')              // Botón continuar

// Paso 2: Habitaciones
cy.get('select[formcontrolname="habitacionId"]')  // Selector habitación
cy.get('input[formcontrolname="fechaEntrada"]')   // Fecha entrada
cy.get('input[formcontrolname="fechaSalida"]')    // Fecha salida
cy.get('input.cliente-search').eq(1)              // Buscador huésped

// Paso 3: Confirmación
cy.get('input[formcontrolname="montoTotal"]')     // Monto total (readonly)
cy.contains('button', 'Crear Reserva')            // Botón crear
```

---

## 📈 Estructura de Pruebas

```
frontend/cypress/
├── e2e/
│   ├── 00-navigation.cy.ts        ✅ 9 tests - Navegación
│   ├── 01-cliente-pairwise.cy.ts  ✅ 19 tests - Cliente
│   ├── 02-huesped-pairwise.cy.ts  ✅ 30 tests - Huésped
│   └── 03-reserva-pairwise.cy.ts  ✅ 19 tests - Reserva
├── support/
│   ├── commands.ts                ✅ Comandos custom
│   └── e2e.ts                     ✅ Config global
└── README.md                      📖 Documentación
```

**Total**: 77 casos de prueba automatizados

---

## ✨ Comandos Custom Disponibles

### cy.fillClienteForm()
```typescript
cy.fillClienteForm('Hotel Luna', '1234567890', 'cliente@mail.com');
```

### cy.fillHuespedForm()
```typescript
cy.fillHuespedForm('Carlos', 'Gómez', '7894561', {
  segundoApellido: 'Pérez',
  telefono: '71234567',
  fechaNacimiento: '1998-05-12'
});
```

### cy.waitForAngular()
```typescript
cy.waitForAngular(); // Espera 500ms para renderizado
```

---

## 🎬 Flujo de Ejecución Típico

```powershell
# Terminal 1: Backend
cd C:\Users\blink\Documents\calidad\HotelManagement
dotnet run

# Terminal 2: Frontend
cd C:\Users\blink\Documents\calidad\HotelManagement\frontend
npm start

# Terminal 3: Cypress (espera que ambos estén corriendo)
cd C:\Users\blink\Documents\calidad\HotelManagement\frontend
npx cypress open

# En Cypress UI:
# 1. Click en "E2E Testing"
# 2. Seleccionar navegador (Chrome)
# 3. Click en "Start E2E Testing"
# 4. Seleccionar archivo de prueba (ej: 01-cliente-pairwise.cy.ts)
# 5. Ver ejecución en tiempo real
```

---

## 📝 Notas Importantes

1. **Timestamps Únicos**: Todas las pruebas usan `Date.now()` para evitar duplicados
2. **Rutas Corregidas**: Todas las rutas ahora apuntan a las URLs correctas
3. **Selectores Validados**: Todos los selectores coinciden con el HTML real
4. **Validaciones Backend**: Las pruebas consideran las validaciones del backend
5. **Manejo de Errores**: Las pruebas verifican tanto éxitos como errores esperados

---

## 🏆 Casos de Prueba

### Cliente (19 tests)
- ✅ TC-C01 a TC-C10: PAIRWISE
- ✅ TC-C11 a TC-C19: Valores Límite

### Huésped (30 tests)
- ✅ TC-H01 a TC-H15: PAIRWISE
- ✅ TC-H16 a TC-H30: Valores Límite

### Reserva (19 tests)
- ✅ TC-R01 a TC-R14: PAIRWISE
- ✅ TC-R15 a TC-R19: Valores Límite

### Navegación (9 tests)
- ✅ Smoke tests básicos
- ✅ Navegación entre páginas
- ✅ Verificación de formularios

---

## 📞 Soporte

Si encuentras errores:
1. Verifica que backend y frontend estén corriendo
2. Revisa los logs en la consola
3. Verifica las capturas de pantalla en `cypress/screenshots/`
4. Consulta el documento completo: `Tests.md`

---

**Última actualización**: 20 de octubre de 2025  
**Versión**: 1.1 - Rutas Corregidas  
**Estado**: ✅ Listo para ejecución
