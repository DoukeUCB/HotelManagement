/// <reference types="cypress" />

/**
 * PRUEBAS PAIRWISE - TABLA CLIENTE
 * Total: 13 casos de prueba 
 */

describe('Cliente - Pruebas PAIRWISE', () => {
  beforeEach(() => {
    cy.visit('/nuevo-cliente');
  });

  // Función auxiliar para generar datos únicos
  const uniqueEmail = () => `c${Date.now().toString().slice(-6)}@m.co`;
  const uniqueNIT = () => `${Date.now().toString().slice(-10)}`;

  describe('Casos PAIRWISE', () => {
    it('TC-C01: Cliente válido - ÉXITO', () => {
      const email = uniqueEmail();
      const nit = uniqueNIT();

      cy.intercept('POST', '/api/Cliente').as('createCliente');

      cy.fillClienteForm('Hotel Luna', nit, email);
      cy.get('button[type="submit"]').click();

      cy.wait('@createCliente').its('response.statusCode').should('eq', 201);
      cy.get('.estado.ok', { timeout: 10000 }).should('be.visible');
    });

    it('TC-C02: Email formato inválido - ERROR', () => {
      const invalidEmail = 'cliente@';
      const nit = '2833001812';
      cy.fillClienteForm('Hotel Sol', nit, invalidEmail);
      cy.get('button[type="submit"]').should('be.disabled');
      cy.get('small').should('contain', 'Email inválido');
    });

   it('TC-C03: NIT vacío - ERROR', () => {
      const razon = 'Hotel Mar';
      const nit = ''; 
      const email = `c${Date.now()}@m.co`;

      cy.fillClienteForm(razon, nit, email);
      cy.get('button[type="submit"]').should('be.disabled');
      cy.get('small').should('contain', 'obligatorio');
    });

    it('TC-C04: Razón Social vacía - ERROR', () => {
      const razon = ''; 
      const nit = '2833483860';
      const email = `c${Date.now()}@m.co`;
      cy.fillClienteForm(razon, nit, email);
      cy.get('button[type="submit"]').should('be.disabled');
      cy.get('small').should('contain', 'obligatorio');
    });


    it('TC-C05: NIT excede límite - ERROR', () => {
      cy.fillClienteForm('Hotel Paz', '123456789012345678901', uniqueEmail());
      cy.get('button[type="submit"]').click();
      cy.get('.estado.error', { timeout: 10000 }).should('exist');
    });

   it('TC-C06: Razón Social excede límite - ERROR', () => {
      const razon = 'Hotel Gran Majestuoso 1'; // 21 caracteres
      const nit = '2833625857';
      const email = `c${Date.now()}@m.co`;
      cy.get('input[formcontrolname="razonSocial"]')
        .clear()
        .type(razon);

      cy.get('input[formcontrolname="nit"]')
        .clear()
        .type(nit);
      cy.get('input[formcontrolname="email"]')
        .clear()
        .type(email);
      cy.get('button[type="submit"]').should('be.disabled');
      cy.get('small.error-message')
        .should('contain.text', 'La Razón Social no puede exceder 20 caracteres');
    });

     it('TC-C07: No permite enviar el formulario si el email está vacío', () => {
      cy.visit('/nuevo-cliente');
      cy.get('input[formcontrolname="razonSocial"]')
        .clear()
        .type('Hotel Real');
      cy.get('input[formcontrolname="nit"]')
        .clear()
        .type('2836372852');
      cy.get('input[formcontrolname="email"]')
        .clear()
        .blur(); 
      cy.get('button[type="submit"]').should('be.disabled');
      cy.get('small.error-message')
        .should('contain.text', 'Este campo es obligatorio');
    });

    it('TC-C08: Email duplicado - ERROR', () => {
      const dupEmail = uniqueEmail();

      
      cy.intercept('POST', '/api/Cliente').as('createFirst');
      cy.fillClienteForm('Hotel First', uniqueNIT(), dupEmail);
      cy.get('button[type="submit"]').click();
      cy.wait('@createFirst');

      
      cy.visit('/nuevo-cliente');
      cy.intercept('POST', '/api/Cliente').as('createSecond');
      cy.fillClienteForm('Hotel Second', uniqueNIT(), dupEmail);
      cy.get('button[type="submit"]').click();
      cy.wait('@createSecond');
      cy.get('.estado.error', { timeout: 10000 }).should('be.visible');
    });

    it('TC-C09: No permite enviar el formulario si múltiples campos están vacíos', () => {
      cy.visit('/nuevo-cliente');
      cy.get('input[formcontrolname="razonSocial"]').clear().blur();
      cy.get('input[formcontrolname="nit"]').clear().blur();
      cy.get('input[formcontrolname="email"]').clear().blur();
      cy.get('button[type="submit"]').should('be.disabled');
      cy.get('input[formcontrolname="razonSocial"]')
        .parent()
        .find('small.error-message')
        .should('contain.text', 'Este campo es obligatorio');

      cy.get('input[formcontrolname="nit"]')
        .parent()
        .find('small.error-message')
        .should('contain.text', 'Este campo es obligatorio');

      cy.get('input[formcontrolname="email"]')
        .parent()
        .find('small.error-message')
        .should('contain.text', 'Este campo es obligatorio');
    });

    // TC-C10: Múltiples errores en formulario - ERROR
it('TC-C10: Múltiples errores en formulario - ERROR', () => {
   
    cy.get('input[formcontrolname="razonSocial"]')
      .clear()
      .type('Hotel Internacional 5 Estrellas')
      .blur();

  
    cy.get('input[formcontrolname="nit"]')
      .clear()
      .blur()
      .should('have.class', 'ng-invalid'); 

  
    cy.get('input[formcontrolname="email"]')
      .clear()
      .blur()
      .should('have.class', 'ng-invalid');

    
    cy.get('button[type="submit"]').should('be.disabled');

    
    cy.get('input[formcontrolname="razonSocial"]')
      .parent()
      .find('small.error-message')
      .should('be.visible')
      .and('contain.text', 'La Razón Social no puede exceder 20 caracteres');

    // Mensaje de error NIT
    cy.get('input[formcontrolname="nit"]')
      .parent()
      .find('small.error-message')
      .should('be.visible')
      .and('contain.text', 'Este campo es obligatorio');

    // Mensaje de error Email
    cy.get('input[formcontrolname="email"]')
      .parent()
      .find('small.error-message')
      .should('be.visible')
      .and('contain.text', 'Este campo es obligatorio');
});

it('TC-C11: Razón Social mínima válida y NIT válido - ÉXITO', () => {
  // Email único para evitar duplicados
  const email = `c${Date.now().toString().slice(-6)}@m.co`;

  // NIT nuevo que no exista
  const nitNuevo = Date.now().toString().slice(-10);

  cy.intercept('POST', '/api/Cliente').as('createCliente');

  // Llenar el formulario con la estructura que usa Angular,
  // pero ENVIAMOS lo que el backend espera -> razon_Social
  cy.get('input[formcontrolname="razonSocial"]').clear().type('Sky');
  cy.get('input[formcontrolname="nit"]').clear().type(nitNuevo);
  cy.get('input[formcontrolname="email"]').clear().type(email);

  cy.get('button[type="submit"]').should('not.be.disabled').click();

  // Validar que el backend responda correctamente
  cy.wait('@createCliente')
    .its('response.statusCode')
    .should('eq', 201);

  cy.get('.estado.ok', { timeout: 10000 }).should('be.visible');
});


  // TC-C12: Razón Social máximo 20 caracteres, NIT válido - ÉXITO
  it('TC-C12: Razón Social máximo 20 caracteres, NIT válido - ÉXITO', () => {
    const email = uniqueEmail();
    cy.fillClienteForm('12345678901234567890', uniqueNIT(), email);

    cy.intercept('POST', '/api/Cliente').as('createCliente');
    cy.get('button[type="submit"]').click();

    cy.wait('@createCliente').its('response.statusCode').should('eq', 201);
    cy.get('.estado.ok', { timeout: 10000 }).should('be.visible');
  });

  // TC-C13: Razón Social > 20 caracteres - ERROR
  it('TC-C13: Razón Social > 20 caracteres - ERROR', () => {
    const email = uniqueEmail();
    cy.fillClienteForm('123456789012345678901', uniqueNIT(), email);

    cy.get('button[type="submit"]').should('be.disabled');

    cy.get('input[formcontrolname="razonSocial"]')
      .parent()
      .find('small.error-message')
      .should('contain.text', 'La Razón Social no puede exceder 20 caracteres');
  });

  });

});