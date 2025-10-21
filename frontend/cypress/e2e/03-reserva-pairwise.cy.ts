/// <reference types="cypress" />

/**
 * PRUEBAS PAIRWISE - TABLA RESERVA
 * Basado en Tests.md - Sección 3: TABLA RESERVA
 * Total: 19 casos de prueba (14 PAIRWISE + 5 Valores Límite)
 * 
 * NOTA: Estas pruebas requieren que existan clientes en la BD
 */

describe('Reserva - Pruebas PAIRWISE', () => {
  let clienteId: string;

  before(() => {
    // Crear un cliente de prueba para usar en las reservas
    cy.visit('/nuevo-cliente');
    cy.waitForAngular();
    cy.fillClienteForm('Cliente Test Reserva', '9999999999', `test-reserva-${Date.now()}@mail.com`);
    cy.get('button[type="submit"]').click();
    cy.wait(2000);
    
    // Obtener el ID del cliente creado (esto depende de tu implementación)
    // Por ahora, asumimos que existe un cliente
  });

  beforeEach(() => {
    cy.visit('/nueva-reserva');
    cy.waitForAngular();
  });

  describe('Casos PAIRWISE', () => {
    it('TC-R01: Reserva Pendiente válida - ÉXITO', () => {
      // Seleccionar cliente
      cy.get('input.cliente-search').type('Cliente Test');
      cy.wait(500);
      cy.get('.suggestions li').first().click();

      // Estado por defecto es Pendiente
      cy.get('select[formcontrolname="estadoReserva"]').should('have.value', 'Pendiente');

      // Continuar al paso 2
      cy.contains('button', 'Continuar').click();
      cy.waitForAngular();

      // Paso 2: Seleccionar habitación
      cy.get('select[formcontrolname="habitacionId"]').first().select(1);
      cy.get('input[formcontrolname="fechaEntrada"]').first().type('2025-11-01');
      cy.get('input[formcontrolname="fechaSalida"]').first().type('2025-11-05');

      // Agregar huésped
      cy.get('input.cliente-search').eq(1).type('Carlos');
      cy.wait(500);
      cy.get('.suggestions li').first().click();

      // Continuar al paso 3
      cy.contains('button', 'Continuar').click();
      cy.waitForAngular();

      // Verificar monto calculado automáticamente
      cy.get('input[formcontrolname="montoTotal"]').should('not.have.value', '');

      // Crear reserva
      cy.contains('button', 'Crear Reserva').click();
      cy.waitForAngular();

      cy.get('.estado-ok', { timeout: 10000 }).should('be.visible');
    });

    it('TC-R02: Reserva Confirmada - ÉXITO', () => {
      cy.get('input.cliente-search').type('Cliente Test');
      cy.wait(500);
      cy.get('.suggestions li').first().click();

      cy.get('select[formcontrolname="estadoReserva"]').select('Confirmada');
      
      cy.contains('button', 'Continuar').click();
      cy.waitForAngular();

      cy.get('select[formcontrolname="habitacionId"]').first().select(1);
      cy.get('input[formcontrolname="fechaEntrada"]').first().type('2025-11-10');
      cy.get('input[formcontrolname="fechaSalida"]').first().type('2025-11-15');

      cy.get('input.cliente-search').eq(1).type('María');
      cy.wait(500);
      cy.get('.suggestions li').first().click();

      cy.contains('button', 'Continuar').click();
      cy.waitForAngular();

      cy.contains('button', 'Crear Reserva').click();
      cy.waitForAngular();

      cy.get('.estado-ok', { timeout: 10000 }).should('be.visible');
    });

    it('TC-R03: Monto cero válido - ÉXITO', () => {
      cy.get('input.cliente-search').type('Cliente Test');
      cy.wait(500);
      cy.get('.suggestions li').first().click();

      cy.get('select[formcontrolname="estadoReserva"]').select('Cancelada');
      
      cy.contains('button', 'Continuar').click();
      cy.waitForAngular();

      cy.get('select[formcontrolname="habitacionId"]').first().select(1);
      cy.get('input[formcontrolname="fechaEntrada"]').first().type('2025-11-20');
      cy.get('input[formcontrolname="fechaSalida"]').first().type('2025-11-20');

      cy.get('input.cliente-search').eq(1).type('Juan');
      cy.wait(500);
      cy.get('.suggestions li').first().click();

      cy.contains('button', 'Continuar').click();
      cy.waitForAngular();

      // Monto debería ser 0 (0 días)
      cy.get('input[formcontrolname="montoTotal"]').should('have.value', '0');

      cy.contains('button', 'Crear Reserva').click();
      cy.waitForAngular();

      cy.get('.estado-ok', { timeout: 10000 }).should('be.visible');
    });

    it('TC-R04: Reserva Completada - ÉXITO', () => {
      cy.get('input.cliente-search').type('Cliente Test');
      cy.wait(500);
      cy.get('.suggestions li').first().click();

      cy.get('select[formcontrolname="estadoReserva"]').select('Completada');
      
      cy.contains('button', 'Continuar').click();
      cy.waitForAngular();

      cy.get('select[formcontrolname="habitacionId"]').first().select(1);
      cy.get('input[formcontrolname="fechaEntrada"]').first().type('2025-12-01');
      cy.get('input[formcontrolname="fechaSalida"]').first().type('2025-12-05');

      cy.get('input.cliente-search').eq(1).type('Pedro');
      cy.wait(500);
      cy.get('.suggestions li').first().click();

      cy.contains('button', 'Continuar').click();
      cy.waitForAngular();

      cy.contains('button', 'Crear Reserva').click();
      cy.waitForAngular();

      cy.get('.estado-ok', { timeout: 10000 }).should('be.visible');
    });

    it('TC-R05: Estado No-Show - ÉXITO', () => {
      cy.get('input.cliente-search').type('Cliente Test');
      cy.wait(500);
      cy.get('.suggestions li').first().click();

      cy.get('select[formcontrolname="estadoReserva"]').select('No-Show');
      
      cy.contains('button', 'Continuar').click();
      cy.waitForAngular();

      cy.get('select[formcontrolname="habitacionId"]').first().select(1);
      cy.get('input[formcontrolname="fechaEntrada"]').first().type('2025-12-10');
      cy.get('input[formcontrolname="fechaSalida"]').first().type('2025-12-12');

      cy.get('input.cliente-search').eq(1).type('Ana');
      cy.wait(500);
      cy.get('.suggestions li').first().click();

      cy.contains('button', 'Continuar').click();
      cy.waitForAngular();

      cy.contains('button', 'Crear Reserva').click();
      cy.waitForAngular();

      cy.get('.estado-ok', { timeout: 10000 }).should('be.visible');
    });

    it('TC-R06: Estado Rechazada inválido - ERROR', () => {
      // Este test verificaría que no existe la opción "Rechazada"
      cy.get('select[formcontrolname="estadoReserva"] option').then(($options) => {
        const optionTexts = [...$options].map(opt => opt.textContent?.trim());
        expect(optionTexts).to.not.include('Rechazada');
      });
    });

    it('TC-R09: Cliente_ID vacío - ERROR', () => {
      // Intentar continuar sin seleccionar cliente
      cy.contains('button', 'Continuar').should('be.disabled');
    });

    it('TC-R11: Estado vacío - ERROR', () => {
      // Verificar que siempre hay un estado seleccionado por defecto
      cy.get('select[formcontrolname="estadoReserva"]').should('not.have.value', '');
    });
  });

  describe('Valores Límite - Monto', () => {
    it('TC-R17: Monto igual a 0 - VÁLIDO', () => {
      cy.get('input.cliente-search').type('Cliente Test');
      cy.wait(500);
      cy.get('.suggestions li').first().click();
      
      cy.contains('button', 'Continuar').click();
      cy.waitForAngular();

      cy.get('select[formcontrolname="habitacionId"]').first().select(1);
      cy.get('input[formcontrolname="fechaEntrada"]').first().type('2025-12-15');
      cy.get('input[formcontrolname="fechaSalida"]').first().type('2025-12-15');

      cy.get('input.cliente-search').eq(1).type('Luis');
      cy.wait(500);
      cy.get('.suggestions li').first().click();

      cy.contains('button', 'Continuar').click();
      cy.waitForAngular();

      cy.get('input[formcontrolname="montoTotal"]').should('have.value', '0');

      cy.contains('button', 'Crear Reserva').click();
      cy.waitForAngular();

      cy.get('.estado-ok', { timeout: 10000 }).should('be.visible');
    });

    it('TC-R18: Monto mínimo positivo - VÁLIDO', () => {
      cy.get('input.cliente-search').type('Cliente Test');
      cy.wait(500);
      cy.get('.suggestions li').first().click();
      
      cy.contains('button', 'Continuar').click();
      cy.waitForAngular();

      cy.get('select[formcontrolname="habitacionId"]').first().select(1);
      cy.get('input[formcontrolname="fechaEntrada"]').first().type('2025-12-20');
      cy.get('input[formcontrolname="fechaSalida"]').first().type('2025-12-21');

      cy.get('input.cliente-search').eq(1).type('Rosa');
      cy.wait(500);
      cy.get('.suggestions li').first().click();

      cy.contains('button', 'Continuar').click();
      cy.waitForAngular();

      // Verificar que el monto sea mayor a 0
      cy.get('input[formcontrolname="montoTotal"]').invoke('val').then((val) => {
        expect(parseFloat(val as string)).to.be.greaterThan(0);
      });

      cy.contains('button', 'Crear Reserva').click();
      cy.waitForAngular();

      cy.get('.estado-ok', { timeout: 10000 }).should('be.visible');
    });

    it('TC-R19: Monto muy alto - VÁLIDO', () => {
      cy.get('input.cliente-search').type('Cliente Test');
      cy.wait(500);
      cy.get('.suggestions li').first().click();
      
      cy.contains('button', 'Continuar').click();
      cy.waitForAngular();

      // Agregar múltiples habitaciones para aumentar el monto
      cy.get('select[formcontrolname="habitacionId"]').first().select(1);
      cy.get('input[formcontrolname="fechaEntrada"]').first().type('2025-12-25');
      cy.get('input[formcontrolname="fechaSalida"]').first().type('2026-01-25'); // 31 días

      cy.get('input.cliente-search').eq(1).type('Miguel');
      cy.wait(500);
      cy.get('.suggestions li').first().click();

      cy.contains('button', 'Continuar').click();
      cy.waitForAngular();

      // Verificar que el monto se calculó
      cy.get('input[formcontrolname="montoTotal"]').invoke('val').then((val) => {
        expect(parseFloat(val as string)).to.be.greaterThan(0);
      });

      cy.contains('button', 'Crear Reserva').click();
      cy.waitForAngular();

      cy.get('.estado-ok', { timeout: 10000 }).should('be.visible');
    });
  });
});
