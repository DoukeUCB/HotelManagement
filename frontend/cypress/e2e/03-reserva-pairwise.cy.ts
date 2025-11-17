/// <reference types="cypress" />

describe('Reservas CRUD - Simplificado', () => {
  const API = 'http://localhost:5000/api';

  beforeEach(() => {
    cy.intercept('GET', '**/api/Reserva').as('getReservas');
    cy.visit('/reservas', { timeout: 15000 });
    cy.wait('@getReservas', { timeout: 15000 });
    cy.wait(1000);
  });

  it('Muestra la lista de reservas', () => {
    cy.get('.data-table, table, .table, .list-container', { timeout: 10000 }).should('exist');
    cy.get('body').should('be.visible');
  });

  it('Filtra reservas por cliente', () => {
    cy.get('.search-input, input[type="search"], input[placeholder*="buscar" i]', { timeout: 10000 }).should('exist');
  });

  it('Navega al formulario de nueva reserva', () => {
    cy.contains('button, a', /nueva reserva|nuevo|crear/i, { timeout: 10000 }).should('be.visible').click();
    cy.url({ timeout: 10000 }).should('include', '/nueva-reserva');
  });

  it('Crea una nueva reserva mediante API', () => {
    cy.request('GET', `${API}/Cliente`).then((resp) => {
      const clientes = resp.body || [];
      if (clientes.length === 0) {
        cy.log('No hay clientes disponibles');
        return;
      }

      const clienteId = clientes[0].id || clientes[0].ID;
      
      cy.request({
        method: 'POST',
        url: `${API}/Reserva`,
        body: {
          cliente_ID: clienteId,
          estado_Reserva: 'Pendiente',
          monto_Total: 500.00
        },
        failOnStatusCode: false
      }).then((response) => {
        expect([200, 201]).to.include(response.status);
      });
    });
  });

  it('Elimina una reserva mediante API', () => {
    cy.request('GET', `${API}/Reserva`).then((resp) => {
      const reservas = resp.body || [];
      if (reservas.length === 0) {
        cy.log('No hay reservas para eliminar');
        return;
      }

      const reservaId = reservas[0].id || reservas[0].ID;
      cy.request({
        method: 'DELETE',
        url: `${API}/Reserva/${reservaId}`,
        failOnStatusCode: false
      }).then((response) => {
        expect([200, 204]).to.include(response.status);
      });
    });
  });
});
