/// <reference types="cypress" />

/**
 * PRUEBAS DE NAVEGACIÓN Y SMOKE TESTS
 * Verificar que la aplicación Angular esté funcionando correctamente
 */

describe('Navegación y Smoke Tests', () => {
  beforeEach(() => {
    cy.visit('/', { timeout: 15000 });
    cy.wait(500); // Esperar que Angular se inicialice
  });

  it('Debe cargar la página principal', () => {
    cy.url().should('include', 'localhost:4200');
    cy.get('body').should('be.visible');
  });

  it('Debe navegar a Clientes', () => {
    cy.contains('a', 'Clientes', { timeout: 10000 }).should('be.visible').click();
    cy.url({ timeout: 10000 }).should('include', '/clientes');
  });

  it('Debe navegar a Nuevo Cliente', () => {
    cy.visit('/clientes');
    cy.contains('button, a', 'Nuevo cliente', { timeout: 10000 }).should('be.visible').click();
    cy.url({ timeout: 10000 }).should('include', '/nuevo-cliente');
    
    // Verificar que el formulario existe
    cy.get('form', { timeout: 10000 }).should('exist');
    cy.get('input[formControlName="razonSocial"], input[formcontrolname="razonSocial"]', { timeout: 5000 }).should('exist');
    cy.get('input[formControlName="nit"], input[formcontrolname="nit"]', { timeout: 5000 }).should('exist');
    cy.get('input[formControlName="email"], input[formcontrolname="email"]', { timeout: 5000 }).should('exist');
  });

  it('Debe navegar a Huéspedes', () => {
    cy.contains('a', 'Huéspedes', { timeout: 10000 }).should('be.visible').click();
    cy.url({ timeout: 10000 }).should('include', '/huespedes');
  });

  it('Debe navegar a Nuevo Huésped', () => {
    cy.visit('/huespedes');
    cy.contains('button, a', 'Nuevo huésped', { timeout: 10000 }).should('be.visible').click();
    cy.url({ timeout: 10000 }).should('include', '/nuevo-huesped');
    
    // Verificar que el formulario existe
    cy.get('form', { timeout: 10000 }).should('exist');
    cy.get('input[formControlName="primerNombre"], input[formcontrolname="primerNombre"]', { timeout: 5000 }).should('exist');
    cy.get('input[formControlName="primerApellido"], input[formcontrolname="primerApellido"]', { timeout: 5000 }).should('exist');
    cy.get('input[formControlName="documento"], input[formcontrolname="documento"]', { timeout: 5000 }).should('exist');
  });

  it('Debe navegar a Reservas', () => {
    cy.contains('a', 'Reservas', { timeout: 10000 }).should('be.visible').click();
    cy.url({ timeout: 10000 }).should('include', '/reservas');
  });

  it('Debe navegar a Nueva Reserva', () => {
    cy.visit('/reservas');
    cy.contains('button, a', 'Nueva Reserva', { timeout: 10000 }).should('be.visible').click();
    cy.url({ timeout: 10000 }).should('include', '/nueva-reserva');
    
    // Verificar que el formulario existe
    cy.get('form', { timeout: 10000 }).should('exist');
  });

  it('Debe navegar a Habitaciones', () => {
    cy.contains('a', 'Habitaciones', { timeout: 10000 }).should('be.visible').click();
    cy.url({ timeout: 10000 }).should('include', '/habitaciones');
  });

  it('Debe tener botones de volver funcionando', () => {
    cy.visit('/nuevo-cliente', { timeout: 15000 });
    cy.get('button, a', { timeout: 10000 })
      .contains(/volver|atrás|back/i)
      .should('be.visible')
      .click();

    cy.url({ timeout: 10000 }).should('include', '/clientes');
  });
});
