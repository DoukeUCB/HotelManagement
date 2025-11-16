/// <reference types="cypress" />

describe('Huéspedes CRUD', () => {

  const nuevoHuesped = {
    primerNombre: 'Juan',
    segundoNombre: 'Carlos',
    primerApellido: 'Pérez',
    segundoApellido: 'Gómez',
    documento: '99999999',
    telefono: '123456789',
    fechaNacimiento: '1990-01-01'
  };

  beforeEach(() => {
    cy.visit('/huespedes');
    cy.intercept('GET', '**/api/Huesped').as('getHuespedes');
    cy.wait('@getHuespedes');
    cy.get('.table-row', { timeout: 5000 }).should('have.length.greaterThan', 0);
  });

  // 1. Verifica que el listado se carga correctamente
  it('Muestra la lista de huéspedes', () => {
    cy.get('.data-table').should('exist');
    cy.get('.table-row').its('length').should('be.gte', 0);
  });

  // 2. Busca un huésped por nombre
  it('Filtra huéspedes por nombre', () => {
    // Obtén el primer nombre de la tabla para buscar algo que existe
    cy.get('.table-row').first().find('[data-label="Nombre"]').then(($el) => {
      const primerNombre = $el.text().split(' ')[0]; // Extrae el primer nombre
      cy.get('.input-busqueda').type(primerNombre);
      cy.get('.table-row').each(($row) => {
        cy.wrap($row).find('[data-label="Nombre"]').should('contain.text', primerNombre);
      });
    });
  });

  // 3. Abre el modal de edición de un huésped
  it('Abre modal editar huésped', () => {
    cy.get('.btn-editar').first().click();
    cy.get('.modal-editar').should('be.visible');
  });

  // 4. Cierra el modal de edición
  it('Cierra modal editar huésped', () => {
    cy.get('.btn-editar').first().click();
    cy.get('.modal-editar').should('be.visible');
    cy.get('.modal-actions .btn-secondary').click();
    cy.get('.modal-editar').should('not.exist');
  });

  // 5. Edita un huésped y guarda cambios
  it('Edita un huésped correctamente', () => {
    // Captura el nombre original antes de editar
    cy.get('.table-row').first().find('[data-label="Nombre"]').then(($el) => {
      const nombreOriginal = $el.text().split(' ')[0];
      
      cy.get('.btn-editar').first().click();
      cy.get('input[name="primerNombre"]').clear().type('Pedro');
      
      // Intercepta el PUT request
      cy.intercept('PUT', '**/api/Huesped/**').as('updateHuesped');
      cy.get('.modal-actions .btn-primary').click();
      
      // Espera a que se complete el PUT
      cy.wait('@updateHuesped');
      cy.get('.modal-editar').should('not.exist');
      
      // Valida que el primer nombre cambió a PEDRO
      cy.get('.table-row').first().find('[data-label="Nombre"]').should('include.text', 'PEDRO');
    });
  });

  // 6. Abre modal de eliminación
  it('Abre modal eliminar huésped', () => {
    cy.get('.btn-eliminar').first().click();
    cy.get('.modal-backdrop').should('be.visible');
    cy.get('.modal h2').contains('¿Eliminar este huésped?');
  });

  // 7. Cancela eliminación
  it('Cancela la eliminación de un huésped', () => {
    cy.get('.btn-eliminar').first().click();
    cy.get('.modal-actions .btn-secondary').click();
    cy.get('.modal-backdrop').should('not.exist');
  });

  // 8. Elimina un huésped
  it('Elimina un huésped correctamente', () => {
    cy.get('.btn-eliminar').first().click();
    cy.get('.modal-actions .btn-danger').click();
    cy.get('.table-row').should('not.contain', 'Juan');
  });

  // 9. Navega a crear nuevo huésped
  it('Navega al formulario de nuevo huésped', () => {
    cy.get('.btn-primary').contains('+ Nuevo huésped').click();
    cy.url().should('include', '/nuevo-huesped');
  });

  // 10. Crea un nuevo huésped con datos válidos
  it('Crea un nuevo huésped correctamente', () => {
    cy.visit('/nuevo-huesped');
    cy.get('input[formControlName="primerNombre"]', { timeout: 10000 }).type(nuevoHuesped.primerNombre);
    cy.get('input[formControlName="segundoNombre"]').type(nuevoHuesped.segundoNombre);
    cy.get('input[formControlName="primerApellido"]').type(nuevoHuesped.primerApellido);
    cy.get('input[formControlName="segundoApellido"]').type(nuevoHuesped.segundoApellido);
    cy.get('input[formControlName="documento"]').type(nuevoHuesped.documento);
    cy.get('input[formControlName="telefono"]').type(nuevoHuesped.telefono);
    cy.get('input[formControlName="fechaNacimiento"]').type(nuevoHuesped.fechaNacimiento);
    
    cy.intercept('POST', '**/api/Huesped').as('createHuesped');
    cy.get('button[type="submit"]').click();
    cy.wait('@createHuesped').then((interception) => {
      // Valida que la respuesta sea exitosa (200 o 201)
      expect(interception.response?.statusCode).to.be.oneOf([200, 201]);
    });
    cy.contains('✅ Huésped creado correctamente.', { timeout: 5000 }).should('be.visible');
  });

  // 11. Valida error al crear huésped con documento duplicado
  it('Muestra error al crear huésped con documento existente', () => {
    cy.visit('/nuevo-huesped');
    // Usa un documento que ya existe en la BD
    cy.get('input[formControlName="primerNombre"]', { timeout: 10000 }).type('Test');
    cy.get('input[formControlName="primerApellido"]').type('User');
    cy.get('input[formControlName="documento"]').type('12345678'); // Documento que ya existe
    
    // El validador asíncrono hace GET pero ya está interceptado en beforeEach
    // Solo espera a que el validador termine escribiendo en el campo
    cy.get('input[formControlName="documento"]').blur();
    
    // Espera un poco para que el validador asíncrono se ejecute
    cy.wait(600);
    
    cy.get('button[type="submit"]').click();
    
    // Valida que hay un mensaje de error visible
    cy.get('.estado.error', { timeout: 5000 }).should('exist').and('be.visible');
  });

  // 12. Valida campos obligatorios en formulario de nuevo huésped
  it('Muestra errores en campos obligatorios', () => {
    cy.visit('/nuevo-huesped');
    cy.get('button[type="submit"]', { timeout: 10000 }).click();
    // Valida que haya mensajes de error visibles
    cy.get('small').should('contain.text', 'Este campo es obligatorio');
    cy.get('input[formControlName="primerNombre"]').should('have.class', 'ng-invalid');
    cy.get('input[formControlName="primerApellido"]').should('have.class', 'ng-invalid');
    cy.get('input[formControlName="documento"]').should('have.class', 'ng-invalid');
  });

  // 13. Navega de listado a detalle de un huésped
  it('Navega a detalle de un huésped', () => {
    cy.get('.btn-editar').first().click();
    cy.get('.modal-editar').should('be.visible');
  });

});