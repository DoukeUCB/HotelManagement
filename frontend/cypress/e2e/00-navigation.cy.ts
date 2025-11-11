/// <reference types="cypress" />

/**
 * PRUEBAS DE NAVEGACIÓN Y SMOKE TESTS
 * Verificar que la aplicación Angular esté funcionando correctamente
 */

describe('Navegación y Smoke Tests', () => {
  beforeEach(() => {
    cy.visit('/');
  });

  it('Debe cargar la página principal', () => {
    cy.url().should('include', 'localhost:4200');
  });

  it('Debe navegar a Clientes', () => {
    cy.contains('Clientes').click();
    cy.url().should('include', '/clientes');
  });

  it('Debe navegar a Nuevo Cliente', () => {
    cy.visit('/clientes');
    cy.contains('Nuevo cliente').click();
    cy.url().should('include', '/nuevo-cliente');
    
    // Verificar que el formulario existe
    cy.get('form').should('exist');
    cy.get('input[formcontrolname="razonSocial"]').should('exist');
    cy.get('input[formcontrolname="nit"]').should('exist');
    cy.get('input[formcontrolname="email"]').should('exist');
  });

  it('Debe navegar a Huéspedes', () => {
    cy.contains('Huéspedes').click();
    cy.url().should('include', '/huespedes');
  });

  it('Debe navegar a Nuevo Huésped', () => {
    cy.visit('/huespedes');
    cy.contains('Nuevo huésped').click();
    cy.url().should('include', '/nuevo-huesped');
    
    // Verificar que el formulario existe
    cy.get('form').should('exist');
    cy.get('input[formcontrolname="primerNombre"]').should('exist');
    cy.get('input[formcontrolname="primerApellido"]').should('exist');
    cy.get('input[formcontrolname="documento"]').should('exist');
  });

  it('Debe navegar a Reservas', () => {
    cy.contains('Reservas').click();
    cy.url().should('include', '/reservas');
  });

  it('Debe navegar a Nueva Reserva', () => {
    cy.visit('/reservas');
    cy.contains('Nueva Reserva').click();
    cy.url().should('include', '/nueva-reserva');
    
    // Verificar que el formulario existe
    cy.get('form').should('exist');
  });

  it('Debe navegar a Habitaciones', () => {
    cy.contains('Habitaciones').click();
    cy.url().should('include', '/habitaciones');
  });

  it('Debe tener botones de volver funcionando', () => {
   cy.visit('/nuevo-cliente');        
    cy.get('.volver', { timeout: 10000 }) 
      .should('be.visible')
      .click({ force: true });           

    cy.url().should('include', '/clientes');  

  });
});
