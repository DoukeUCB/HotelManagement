/// <reference types="cypress" />

/**
 * PRUEBAS DE NAVEGACIÓN Y SMOKE TESTS
 * Verificar que la aplicación Angular esté funcionando correctamente
 */

describe('Navegación y Smoke Tests', () => {
  beforeEach(() => {
    cy.visit('/', { timeout: 30000 });
    cy.wait(2000); // Esperar que Angular se inicialice completamente
  });

  it('Debe cargar la página principal', () => {
    cy.url({ timeout: 20000 }).should('include', 'localhost:4200');
    cy.get('body', { timeout: 20000 }).should('be.visible');
    cy.get('body').should('not.be.empty');
  });

  it('Debe navegar a Clientes', () => {
    cy.contains('a, button, [routerLink]', 'Clientes', { timeout: 20000 }).should('be.visible').click();
    cy.url({ timeout: 20000 }).should('include', '/clientes');
    cy.wait(2000);
  });

  it('Debe navegar a Nuevo Cliente', () => {
    cy.visit('/clientes', { timeout: 30000 });
    cy.wait(2000);
    cy.contains('button, a', 'Nuevo cliente', { timeout: 20000 }).should('be.visible').click();
    cy.url({ timeout: 20000 }).should('include', '/nuevo-cliente');
    
    // Verificar que el formulario existe
    cy.get('form', { timeout: 15000 }).should('exist');
    cy.get('input[formControlName="razonSocial"], input[formcontrolname="razonSocial"]', { timeout: 10000 }).should('exist');
    cy.get('input[formControlName="nit"], input[formcontrolname="nit"]', { timeout: 10000 }).should('exist');
    cy.get('input[formControlName="email"], input[formcontrolname="email"]', { timeout: 10000 }).should('exist');
  });

  it('Debe navegar a Huéspedes', () => {
    cy.contains('a, button, [routerLink]', 'Huéspedes', { timeout: 20000 }).should('be.visible').click();
    cy.url({ timeout: 20000 }).should('include', '/huespedes');
    cy.wait(2000);
  });

  it('Debe navegar a Nuevo Huésped', () => {
    cy.visit('/huespedes', { timeout: 30000 });
    cy.wait(2000);
    cy.contains('button, a', 'Nuevo huésped', { timeout: 20000 }).should('be.visible').click();
    cy.url({ timeout: 20000 }).should('include', '/nuevo-huesped');
    
    // Verificar que el formulario existe
    cy.get('form', { timeout: 15000 }).should('exist');
    cy.get('input[formControlName="primerNombre"], input[formcontrolname="primerNombre"]', { timeout: 10000 }).should('exist');
    cy.get('input[formControlName="primerApellido"], input[formcontrolname="primerApellido"]', { timeout: 10000 }).should('exist');
    cy.get('input[formControlName="documento"], input[formcontrolname="documento"]', { timeout: 10000 }).should('exist');
  });

  it('Debe navegar a Reservas', () => {
    cy.contains('a, button, [routerLink]', 'Reservas', { timeout: 20000 }).should('be.visible').click();
    cy.url({ timeout: 20000 }).should('include', '/reservas');
    cy.wait(2000);
  });

  it('Debe navegar a Nueva Reserva', () => {
    cy.visit('/reservas', { timeout: 30000 });
    cy.wait(2000);
    cy.contains('button, a', 'Nueva Reserva', { timeout: 20000 }).should('be.visible').click();
    cy.url({ timeout: 20000 }).should('include', '/nueva-reserva');
    
    // Verificar que el formulario existe
    cy.get('form', { timeout: 15000 }).should('exist');
  });

  it('Debe navegar a Habitaciones', () => {
    cy.contains('a, button, [routerLink]', 'Habitaciones', { timeout: 20000 }).should('be.visible').click();
    cy.url({ timeout: 20000 }).should('include', '/habitaciones');
    cy.wait(2000);
  });

  it('Debe tener botones de volver funcionando', () => {
    cy.visit('/nuevo-cliente', { timeout: 30000 });
    cy.wait(2000);
    cy.get('button, a', { timeout: 20000 })
      .contains(/volver|atrás|back/i)
      .should('be.visible')
      .click();

    cy.url({ timeout: 20000 }).should('include', '/clientes');
  });
});
