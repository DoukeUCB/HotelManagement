/// <reference types="cypress" />

// ***********************************************
// This example commands.ts shows you how to
// create various custom commands and overwrite
// existing commands.
//
// For more comprehensive examples of custom
// commands please read more here:
// https://on.cypress.io/custom-commands
// ***********************************************

declare global {
  namespace Cypress {
    interface Chainable {
      /**
       * Custom command to fill cliente form
       * @example cy.fillClienteForm('Hotel Luna', '1234567890', 'cliente1@mail.com')
       */
      fillClienteForm(razonSocial: string, nit: string, email: string): Chainable<void>;

      /**
       * Custom command to fill huesped form
       * @example cy.fillHuespedForm('Carlos', 'Gómez', '7894561')
       */
      fillHuespedForm(
        nombre: string,
        apellido: string,
        documento: string,
        options?: {
          segundoNombre?: string;
          segundoApellido?: string;
          telefono?: string;
          fechaNacimiento?: string;
        }
      ): Chainable<void>;

      /**
       * Custom command to reset database
       * @example cy.resetDatabase()
       */
      resetDatabase(): Chainable<void>;

      /**
       * Custom command to wait for Angular to be ready
       * @example cy.waitForAngular()
       */
      waitForAngular(): Chainable<void>;
    }
  }
}

Cypress.Commands.add('fillClienteForm', (razonSocial: string, nit: string, email: string) => {
  if (razonSocial !== '') {
    cy.get('input[formcontrolname="razonSocial"]').clear().type(razonSocial);
  } else {
    cy.get('input[formcontrolname="razonSocial"]').clear();
  }

  if (nit !== '') {
    cy.get('input[formcontrolname="nit"]').clear().type(nit);
  } else {
    cy.get('input[formcontrolname="nit"]').clear();
  }

  if (email !== '') {
    cy.get('input[formcontrolname="email"]').clear().type(email);
  } else {
    cy.get('input[formcontrolname="email"]').clear();
  }
});

Cypress.Commands.add(
  'fillHuespedForm',
  (
    nombre: string,
    apellido: string,
    documento: string,
    options?: {
      segundoNombre?: string;
      segundoApellido?: string;
      telefono?: string;
      fechaNacimiento?: string;
    }
  ) => {
    if (nombre !== '') {
      cy.get('input[formcontrolname="primerNombre"]').clear().type(nombre);
    } else {
      cy.get('input[formcontrolname="primerNombre"]').clear();
    }

    if (apellido !== '') {
      cy.get('input[formcontrolname="primerApellido"]').clear().type(apellido);
    } else {
      cy.get('input[formcontrolname="primerApellido"]').clear();
    }

    if (documento !== '') {
      cy.get('input[formcontrolname="documento"]').clear().type(documento);
    } else {
      cy.get('input[formcontrolname="documento"]').clear();
    }

    if (options?.segundoNombre) {
      cy.get('input[formcontrolname="segundoNombre"]').clear().type(options.segundoNombre);
    }

    if (options?.segundoApellido) {
      cy.get('input[formcontrolname="segundoApellido"]').clear().type(options.segundoApellido);
    }

    if (options?.telefono) {
      cy.get('input[formcontrolname="telefono"]').clear().type(options.telefono);
    }

    if (options?.fechaNacimiento) {
      cy.get('input[formcontrolname="fechaNacimiento"]').clear().type(options.fechaNacimiento);
    }
  }
);

Cypress.Commands.add('resetDatabase', () => {
  // Implementar según tu API
  cy.log('Database reset - Implement according to your API');
});

Cypress.Commands.add('waitForAngular', () => {
  cy.wait(500); // Simple wait for Angular rendering
});

export {};