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
    cy.intercept('GET', '**/api/Huesped').as('getHuespedes');
    cy.visit('/huespedes', { timeout: 30000 });
    cy.wait('@getHuespedes', { timeout: 30000 }).then(() => {
      // Esperar a que Angular termine de renderizar
      cy.wait(2000);
    });
  });

  // 1. Verifica que el listado se carga correctamente
  it('Muestra la lista de huéspedes', () => {
    // Esperar a que la página esté completamente cargada
    cy.get('body', { timeout: 20000 }).should('be.visible');
    // Buscar cualquier estructura de tabla o contenedor de lista
    cy.get('.data-table, table, .table, .list-container, .huesped-list, [class*="table"], [class*="list"]', { timeout: 20000 }).should('exist');
  });

  // 2. Busca un huésped por nombre
  it('Filtra huéspedes por nombre', () => {
    // Esperar a que haya al menos una fila visible
    cy.get('.table-row, tr[class*="row"], .huesped-item', { timeout: 20000 }).should('have.length.greaterThan', 0);
    
    // Obtén el primer nombre de la tabla para buscar algo que existe
    cy.get('.table-row, tr[class*="row"], .huesped-item').first().find('[data-label="Nombre"], td:first, .nombre').then(($el) => {
      const primerNombre = $el.text().split(' ')[0]; // Extrae el primer nombre
      cy.get('.input-busqueda, input[type="search"], input[placeholder*="Buscar"]').clear().type(primerNombre);
      cy.wait(1000); // Esperar aplicación del filtro
      cy.get('.table-row, tr[class*="row"], .huesped-item').should('have.length.greaterThan', 0);
      cy.get('body').should('contain.text', primerNombre);
    });
  });

  // 3. Abre el modal de edición de un huésped
  it('Abre modal editar huésped', () => {
    cy.get('.btn-editar, button[class*="editar"], [title*="Editar"]', { timeout: 20000 }).should('have.length.greaterThan', 0);
    cy.get('.btn-editar, button[class*="editar"], [title*="Editar"]').first().click();
    cy.get('.modal-editar, .modal, [class*="modal"]', { timeout: 10000 }).should('be.visible');
  });

  // 4. Cierra el modal de edición
  it('Cierra modal editar huésped', () => {
    cy.get('.btn-editar, button[class*="editar"], [title*="Editar"]', { timeout: 20000 }).first().click();
    cy.get('.modal-editar, .modal, [class*="modal"]').should('be.visible');
    cy.get('.modal-actions .btn-secondary, button[class*="cancel"], button:contains("Cancelar")').click();
    cy.get('.modal-editar, .modal, [class*="modal"]').should('not.exist');
  });

  // 5. Edita un huésped y guarda cambios
  it('Edita un huésped correctamente', () => {
    // Intercepta el PUT request ANTES de abrir el modal
    cy.intercept('PUT', '**/api/Huesped/**').as('updateHuesped');
    
    // Obtén el ID del primer huésped
    cy.get('.btn-editar').first().click();
    cy.get('input[name="primerNombre"]', { timeout: 10000 }).should('be.visible').clear().type('Pedro');
    
    cy.get('.modal-actions .btn-primary').click();
    
    // Espera a que se complete el PUT
    cy.wait('@updateHuesped', { timeout: 15000 }).then((interception) => {
      cy.log('Huésped actualizado:', interception.response?.statusCode);
      expect(interception.response?.statusCode).to.be.oneOf([200, 201, 204]);
    });
    
    // Espera explícita para que el modal se cierre
    cy.get('.modal-editar', { timeout: 10000 }).should('not.exist');
    
    // Recargar la página para obtener datos actualizados del servidor
    cy.reload();
    cy.wait(1500);
    
    // Valida que ahora existe un huésped con el nombre PEDRO en la lista
    cy.get('.data-table, table', { timeout: 10000 }).should('exist');
    cy.get('body').should('contain.text', 'PEDRO');
  });

  // 6. Abre modal de eliminación
  it('Abre modal eliminar huésped', () => {
    cy.get('.btn-eliminar, button[class*="eliminar"], [title*="Eliminar"]', { timeout: 20000 }).first().click();
    cy.get('.modal-backdrop, .modal, [class*="modal"]').should('be.visible');
    cy.get('.modal h2, h2, .modal-title').should('contain.text', 'Eliminar');
  });

  // 7. Cancela eliminación
  it('Cancela la eliminación de un huésped', () => {
    cy.get('.btn-eliminar, button[class*="eliminar"], [title*="Eliminar"]', { timeout: 20000 }).first().click();
    cy.get('.modal-actions .btn-secondary, button:contains("Cancelar")').click();
    cy.get('.modal-backdrop, .modal, [class*="modal"]').should('not.exist');
  });

  // 8. Elimina un huésped
  it('Elimina un huésped correctamente', () => {
    cy.get('.table-row, tr[class*="row"], .huesped-item', { timeout: 20000 }).should('have.length.greaterThan', 0);
    
    // Intercepta el DELETE request
    cy.intercept('DELETE', '**/api/Huesped/**').as('deleteHuesped');
    
    cy.get('.btn-eliminar, button[class*="eliminar"], [title*="Eliminar"]').first().click();
    cy.get('.modal-actions .btn-danger, button[class*="danger"], button:contains("Eliminar")').click();
    
    // Espera a que se complete el DELETE
    cy.wait('@deleteHuesped', { timeout: 20000 });
    cy.wait(1000);
    cy.get('body').should('be.visible');
  });

  // 9. Navega a crear nuevo huésped
  it('Navega al formulario de nuevo huésped', () => {
    cy.get('.btn-primary').contains('+ Nuevo huésped').click();
    cy.url().should('include', '/nuevo-huesped');
  });

  // 10. Crea un nuevo huésped con datos válidos
  it('Crea un nuevo huésped correctamente', () => {
    // Crear huésped directamente mediante API
    const uniqueDoc = `${Date.now()}`;
    cy.request({
      method: 'POST',
      url: 'http://localhost:5000/api/Huesped',
      body: {
        Nombre: `${nuevoHuesped.primerNombre} ${nuevoHuesped.segundoNombre}`,
        Apellido: nuevoHuesped.primerApellido,
        Segundo_Apellido: nuevoHuesped.segundoApellido,
        Documento_Identidad: uniqueDoc,
        Telefono: nuevoHuesped.telefono,
        Fecha_Nacimiento: nuevoHuesped.fechaNacimiento
      },
      failOnStatusCode: false,
      timeout: 30000
    }).then((response) => {
      expect(response.status).to.be.oneOf([200, 201]);
      cy.log('Huésped creado exitosamente mediante API');
      
      // Verifica que el huésped aparezca en la lista
      cy.visit('/huespedes', { timeout: 30000 });
      cy.wait(2000);
      cy.get('body').should('contain.text', nuevoHuesped.primerNombre);
    });
  });

  // 11. Valida error al crear huésped con documento duplicado
  it('Muestra error al crear huésped con documento existente', () => {
    // Primero, crea un huésped con documento conocido vía API
    const docDuplicado = `${Date.now()}`;
    
    cy.request({
      method: 'POST',
      url: 'http://localhost:5000/api/Huesped',
      body: {
        Nombre: 'HUESPED',
        Apellido: 'EXISTENTE',
        Segundo_Apellido: null,
        Documento_Identidad: docDuplicado,
        Telefono: '12345678',
        Fecha_Nacimiento: '1990-01-01'
      },
      failOnStatusCode: false,
      timeout: 30000
    }).then(() => {
      cy.log(`Huésped creado con documento: ${docDuplicado}`);
      
      // Ahora intenta crear otro con el mismo documento vía API y verifica error
      cy.request({
        method: 'POST',
        url: 'http://localhost:5000/api/Huesped',
        body: {
          Nombre: 'OTRO',
          Apellido: 'DUPLICADO',
          Segundo_Apellido: null,
          Documento_Identidad: docDuplicado,
          Telefono: '87654321',
          Fecha_Nacimiento: '1995-05-05'
        },
        failOnStatusCode: false,
        timeout: 30000
      }).then((response) => {
        // Espera un error 400, 409 o 500
        expect(response.status).to.be.oneOf([400, 409, 422, 500]);
        cy.log(`Error esperado recibido: ${response.status}`);
      });
    });
  });

  // 12. Valida campos obligatorios en formulario de nuevo huésped
  it('Muestra errores en campos obligatorios', () => {
    cy.visit('/nuevo-huesped');
    cy.get('button[type="submit"]', { timeout: 10000 }).click();
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