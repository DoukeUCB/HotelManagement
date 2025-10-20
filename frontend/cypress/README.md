# Pruebas E2E con Cypress - HotelManagement

## Descripción

Este directorio contiene las pruebas end-to-end (E2E) automatizadas con Cypress para el sistema de gestión hotelera, basadas en el plan de pruebas PAIRWISE documentado en `Tests.md`.

## Estructura de Archivos

```
cypress/
├── e2e/
│   ├── 00-navigation.cy.ts          # Pruebas de navegación y smoke tests
│   ├── 01-cliente-pairwise.cy.ts    # 19 casos de prueba para Cliente
│   ├── 02-huesped-pairwise.cy.ts    # 30 casos de prueba para Huésped
│   └── 03-reserva-pairwise.cy.ts    # 19 casos de prueba para Reserva
├── support/
│   ├── commands.ts                   # Comandos personalizados de Cypress
│   └── e2e.ts                       # Configuración global
└── README.md                         # Este archivo
```

## Cobertura de Pruebas

### Total: 68 Casos de Prueba Automatizados

| Módulo | Casos PAIRWISE | Casos Límite | Total |
|--------|----------------|--------------|-------|
| Cliente | 10 | 9 | 19 |
| Huésped | 15 | 15 | 30 |
| Reserva | 14 | 5 | 19 |

## Prerrequisitos

1. **Backend en ejecución**
   ```bash
   cd ..
   dotnet run
   ```

2. **Frontend en ejecución**
   ```bash
   npm start
   ```
   La aplicación debe estar corriendo en `http://localhost:4200`

3. **Base de datos limpia** (opcional)
   - Para evitar conflictos con emails duplicados y GUIDs
   - Ejecutar script de reset antes de las pruebas

## Instalación

Cypress ya está instalado como dependencia de desarrollo:

```bash
npm install
```

## Ejecutar Pruebas

### Modo Interactivo (Recomendado para desarrollo)

```bash
npx cypress open
```

Esto abrirá la interfaz gráfica de Cypress donde puedes:
- Seleccionar archivos de prueba específicos
- Ver las pruebas ejecutándose en tiempo real
- Depurar con DevTools
- Ver capturas de pantalla de errores

### Modo Headless (Para CI/CD)

Ejecutar todas las pruebas:
```bash
npx cypress run
```

Ejecutar un archivo específico:
```bash
npx cypress run --spec "cypress/e2e/01-cliente-pairwise.cy.ts"
```

Ejecutar con navegador específico:
```bash
npx cypress run --browser chrome
```

## Comandos Personalizados

Se han creado comandos personalizados para facilitar las pruebas:

### `cy.fillClienteForm(razonSocial, nit, email)`
Llena el formulario de cliente con los datos proporcionados.

```typescript
cy.fillClienteForm('Hotel Luna', '1234567890', 'cliente@mail.com');
```

### `cy.fillHuespedForm(nombre, apellido, documento, options?)`
Llena el formulario de huésped con datos obligatorios y opcionales.

```typescript
cy.fillHuespedForm('Carlos', 'Gómez', '7894561', {
  segundoApellido: 'Pérez',
  telefono: '71234567',
  fechaNacimiento: '1998-05-12'
});
```

### `cy.waitForAngular()`
Espera a que Angular termine de renderizar.

```typescript
cy.waitForAngular();
```

## Estructura de Pruebas

Cada archivo de prueba sigue esta estructura:

```typescript
describe('Módulo - Pruebas PAIRWISE', () => {
  beforeEach(() => {
    // Setup antes de cada prueba
  });

  describe('Casos PAIRWISE', () => {
    it('TC-XX: Descripción del caso - ÉXITO/ERROR', () => {
      // Implementación del test
    });
  });

  describe('Valores Límite', () => {
    it('TC-XX: Validación de límite - VÁLIDO/ERROR', () => {
      // Implementación del test
    });
  });
});
```

## Convenciones de Nomenclatura

- **TC-C##**: Test Cases para Cliente
- **TC-H##**: Test Cases para Huésped
- **TC-R##**: Test Cases para Reserva

Los números coinciden con los casos definidos en `Tests.md`.

## Datos de Prueba

Para evitar conflictos con datos duplicados, se utilizan timestamps:

```typescript
const uniqueEmail = `cliente-${Date.now()}@mail.com`;
const uniqueDoc = `DOC-${Date.now()}`;
```

## Resultados y Reportes

### Capturas de Pantalla
Las capturas de pantalla de pruebas fallidas se guardan en:
```
cypress/screenshots/
```

### Videos (Desactivados por defecto)
Para habilitar videos, modificar `cypress.config.ts`:
```typescript
video: true
```

Los videos se guardarán en:
```
cypress/videos/
```

## Debugging

### Ver elementos en la interfaz
```typescript
cy.get('input[formcontrolname="razonSocial"]').debug();
```

### Pausar la ejecución
```typescript
cy.pause();
```

### Console logs
```typescript
cy.log('Mensaje de debug');
```

## Consideraciones Importantes

1. **Orden de Ejecución**
   - Las pruebas de Reserva requieren que existan Clientes en la BD
   - Se recomienda ejecutar primero las pruebas de Cliente

2. **Estado de la Base de Datos**
   - Algunas pruebas pueden fallar si hay datos preexistentes
   - TC-C08 (email duplicado) crea su propio dato para evitar dependencias

3. **Timeouts**
   - Los comandos tienen un timeout de 10 segundos por defecto
   - Ajustar en `cypress.config.ts` si es necesario

4. **Selectores**
   - Se usan `formcontrolname` para mayor estabilidad
   - Los selectores de clase pueden cambiar con estilos

## Integración Continua (CI/CD)

Ejemplo de script para GitHub Actions:

```yaml
- name: Run Cypress Tests
  run: |
    npm start &
    npx wait-on http://localhost:4200
    npx cypress run
```

## Mantenimiento

Para actualizar las pruebas:

1. Modificar los casos en `Tests.md`
2. Actualizar los archivos `.cy.ts` correspondientes
3. Ejecutar las pruebas para verificar
4. Documentar cambios en este README

## Troubleshooting

### Error: "Timed out waiting for element"
- Aumentar el timeout en `cypress.config.ts`
- Verificar que el selector es correcto
- Usar `cy.waitForAngular()` antes del selector

### Error: "Email already exists"
- Limpiar la base de datos
- Usar timestamps únicos en los datos

### Error: "Cannot find element"
- Verificar que la aplicación Angular está corriendo
- Revisar que las rutas coinciden con `app.routes.ts`
- Inspeccionar el DOM con DevTools

## Recursos

- [Documentación oficial de Cypress](https://docs.cypress.io)
- [Mejores prácticas](https://docs.cypress.io/guides/references/best-practices)
- [Plan de pruebas PAIRWISE](../../Tests.md)

## Contacto

Para reportar issues o sugerencias sobre las pruebas automatizadas, crear un issue en el repositorio.

---

**Última actualización**: 20 de octubre de 2025
**Versión**: 1.0
