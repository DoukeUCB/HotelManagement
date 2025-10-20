/// <reference types="cypress" />

/**
 * PRUEBAS PAIRWISE - TABLA CLIENTE
 * Basado en Tests.md - Sección 1: TABLA CLIENTE
 * Total: 19 casos de prueba (10 PAIRWISE + 9 Valores Límite)
 */

describe('Cliente - Pruebas PAIRWISE', () => {
  beforeEach(() => {
    cy.visit('/nuevo-cliente');
    cy.waitForAngular();
  });

  describe('Casos PAIRWISE', () => {
    it('TC-C01: Cliente válido - ÉXITO', () => {
      cy.fillClienteForm('Hotel Luna', '1234567890', `cliente1-${Date.now()}@mail.com`);
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      // Verificar mensaje de éxito
      cy.get('.estado.ok', { timeout: 10000 }).should('be.visible');
    });

    it('TC-C02: Email formato inválido - ERROR', () => {
      cy.fillClienteForm('Hotel Sol', '9876543210', 'cliente@');
      cy.get('button[type="submit"]').click();

      // Verificar error de validación
      cy.get('small').should('contain', 'Email inválido');
    });

    it('TC-C03: NIT vacío - ERROR', () => {
      cy.fillClienteForm('Hotel Mar', '', `cliente2-${Date.now()}@mail.com`);
      cy.get('button[type="submit"]').click();

      // Verificar error
      cy.get('small').should('contain', 'obligatorio');
    });

    it('TC-C04: Razón Social vacía - ERROR', () => {
      cy.fillClienteForm('', '5555555555', `cliente3-${Date.now()}@mail.com`);
      cy.get('button[type="submit"]').click();

      // Verificar error
      cy.get('small').should('contain', 'obligatorio');
    });

    it('TC-C05: NIT excede límite - ERROR', () => {
      cy.fillClienteForm('Hotel Paz', '123456789012345678901', `cliente4-${Date.now()}@mail.com`);
      cy.get('button[type="submit"]').click();

      // Verificar que no se permita o muestre error
      cy.waitForAngular();
      cy.get('.estado.error').should('exist');
    });

    it('TC-C06: Razón Social excede límite - ERROR', () => {
      cy.fillClienteForm(
        'Hotel Gran Majestuoso *****',
        '7777777777',
        `cliente5-${Date.now()}@mail.com`
      );
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.error').should('exist');
    });

    it('TC-C07: Email vacío - ERROR', () => {
      cy.fillClienteForm('Hotel Real', '8888888888', '');
      cy.get('button[type="submit"]').click();

      cy.get('small').should('contain', 'Email inválido');
    });

    it('TC-C08: Email duplicado - ERROR', () => {
      const duplicateEmail = `duplicate-${Date.now()}@mail.com`;

      // Crear primer cliente
      cy.fillClienteForm('Hotel First', '1111111111', duplicateEmail);
      cy.get('button[type="submit"]').click();
      cy.wait(2000);

      // Intentar crear segundo cliente con mismo email
      cy.visit('/clientes/nuevo');
      cy.waitForAngular();
      cy.fillClienteForm('Hotel Second', '2222222222', duplicateEmail);
      cy.get('button[type="submit"]').click();

      // Verificar error de duplicado
      cy.get('.estado.error', { timeout: 10000 }).should('be.visible');
    });

    it('TC-C09: Múltiples campos vacíos - ERROR', () => {
      cy.fillClienteForm('', '', 'invalido');
      cy.get('button[type="submit"]').click();

      // Verificar múltiples errores
      cy.get('small').should('have.length.at.least', 2);
    });

    it('TC-C10: Múltiples validaciones fallidas - ERROR', () => {
      cy.fillClienteForm('Hotel Internacional 5 Estrellas', '999999999999999999999', '');
      cy.get('button[type="submit"]').click();

      // Verificar que hay errores
      cy.get('small').should('exist');
    });
  });

  describe('Valores Límite', () => {
    it('TC-C11: Razón Social 1 carácter - VÁLIDO', () => {
      cy.fillClienteForm('A', '1234567890', `limite1-${Date.now()}@mail.com`);
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.ok', { timeout: 10000 }).should('be.visible');
    });

    it('TC-C12: Razón Social 20 caracteres - VÁLIDO', () => {
      cy.fillClienteForm('12345678901234567890', '1234567890', `limite2-${Date.now()}@mail.com`);
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.ok', { timeout: 10000 }).should('be.visible');
    });

    it('TC-C13: Razón Social 21 caracteres - ERROR', () => {
      cy.fillClienteForm('123456789012345678901', '1234567890', `limite3-${Date.now()}@mail.com`);
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.error').should('exist');
    });

    it('TC-C14: NIT 1 carácter - VÁLIDO', () => {
      cy.fillClienteForm('Hotel Test', '1', `limite4-${Date.now()}@mail.com`);
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.ok', { timeout: 10000 }).should('be.visible');
    });

    it('TC-C15: NIT 20 caracteres - VÁLIDO', () => {
      cy.fillClienteForm('Hotel Test', '12345678901234567890', `limite5-${Date.now()}@mail.com`);
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.ok', { timeout: 10000 }).should('be.visible');
    });

    it('TC-C16: NIT 21 caracteres - ERROR', () => {
      cy.fillClienteForm('Hotel Test', '123456789012345678901', `limite6-${Date.now()}@mail.com`);
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.error').should('exist');
    });

    it('TC-C17: Email mínimo válido - VÁLIDO', () => {
      cy.fillClienteForm('Hotel Test', '1234567890', 'a@b.c');
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.ok', { timeout: 10000 }).should('be.visible');
    });

    it('TC-C18: Email 30 caracteres - VÁLIDO', () => {
      cy.fillClienteForm('Hotel Test', '1234567890', 'a@b.commmmmmmmmmmmmmmmmmmmmm');
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.ok', { timeout: 10000 }).should('be.visible');
    });

    it('TC-C19: Email 31 caracteres - ERROR', () => {
      cy.fillClienteForm('Hotel Test', '1234567890', 'a@b.commmmmmmmmmmmmmmmmmmmmmm');
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.error').should('exist');
    });
  });
});
