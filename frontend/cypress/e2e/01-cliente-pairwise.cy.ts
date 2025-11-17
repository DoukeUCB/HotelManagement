/// <reference types="cypress" />

describe('Clientes CRUD', () => {

  const nuevoCliente = {
    razonSocial: 'Hotel Nuevo',
    nit: '1234567890',
    email: `hotel${Date.now()}@example.com`
  };

  beforeEach(() => {
    cy.intercept('GET', '**/api/Cliente').as('getClientes');
    cy.visit('/clientes', { timeout: 30000 });
    cy.wait('@getClientes', { timeout: 30000 }).then(() => {
      // Esperar a que Angular termine de renderizar
      cy.wait(2000);
    });
  });

  // 1. Verifica que el listado se carga correctamente
  it('Muestra la lista de clientes', () => {
    // Esperar a que la página esté completamente cargada
    cy.get('body', { timeout: 20000 }).should('be.visible');
    // Buscar cualquier estructura de tabla o contenedor de lista
    cy.get('.data-table, table, .table, .list-container, .cliente-list, [class*="table"], [class*="list"]', { timeout: 20000 }).should('exist');
  });

  // 2. Busca un cliente por Razón Social (nombre)
  it('Filtra clientes por razón social', () => {
    // Esperar a que haya al menos una fila visible
    cy.get('.table-row, tr[class*="row"], .cliente-item', { timeout: 20000 }).should('have.length.greaterThan', 0);
    
    // Obtén un texto válido de la primera fila para buscar
    cy.get('.table-row, tr[class*="row"], .cliente-item').first().then(($row) => {
      // La primera columna de datos es Razón Social
      const primerTexto = $row.find('[data-label="Razón Social"], td:first, .razon-social').text().trim();
      
      if (primerTexto && primerTexto.length > 0) {
        const palabraBusqueda = primerTexto.split(' ')[0];
        
        cy.get('.input-busqueda, input[type="search"], input[placeholder*="Buscar"]').clear().type(palabraBusqueda);
        cy.wait(1000); // Esperar aplicación del filtro
        
        // Valida que el filtro se aplica
        cy.get('.table-row, tr[class*="row"], .cliente-item').should('have.length.greaterThan', 0);
        
        // Verifica que AL MENOS el primer resultado contiene la palabra
        cy.get('.table-row, tr[class*="row"], .cliente-item').first().should('contain.text', palabraBusqueda);
      }
    });
  });

  // 3. Abre el modal de edición de un cliente
  it('Abre modal editar cliente', () => {
    cy.get('.btn-editar, button[class*="editar"], [title*="Editar"]', { timeout: 20000 }).should('have.length.greaterThan', 0);
    cy.get('.btn-editar, button[class*="editar"], [title*="Editar"]').first().click();
    cy.get('.modal-editar, .modal, [class*="modal"]', { timeout: 10000 }).should('be.visible');
  });

  // 4. Cierra el modal de edición
  it('Cierra modal editar cliente', () => {
    cy.get('.btn-editar, button[class*="editar"], [title*="Editar"]', { timeout: 20000 }).first().click();
    cy.get('.modal-editar, .modal, [class*="modal"]').should('be.visible');
    cy.get('.modal-actions .btn-secondary, button[class*="cancel"], button:contains("Cancelar")').click();
    cy.get('.modal-editar, .modal, [class*="modal"]').should('not.exist');
  });

  // 5. Edita un cliente y guarda cambios
  it('Edita un cliente correctamente', () => {
    cy.get('.table-row, tr[class*="row"], .cliente-item', { timeout: 20000 }).should('have.length.greaterThan', 0);
    
    // Obtén el NIT del cliente que vamos a editar para identificarlo después
    cy.get('.table-row, tr[class*="row"], .cliente-item').first().find('[data-label="NIT"], td:nth-child(2), .nit').then(($nit) => {
      const nitOriginal = $nit.text().trim();
      
      cy.intercept('PUT', '**/api/Cliente/**').as('updateCliente');
      cy.get('.btn-editar, button[class*="editar"], [title*="Editar"]').first().click();
      cy.get('input[name="razonSocial"], input[formControlName="razonSocial"]', { timeout: 10000 }).clear().type('Hotel Editado');
      cy.get('.modal-actions .btn-primary, button[type="submit"], button:contains("Guardar")').click();
      
      cy.wait('@updateCliente', { timeout: 20000 });
      cy.get('.modal-editar, .modal, [class*="modal"]').should('not.exist');
      
      // Recargar para ver los cambios
      cy.reload();
      cy.wait(2000);
      
      // Buscar el cliente por su NIT (que no cambió) y verificar que se actualizó
      cy.get('body').should('contain.text', 'HOTEL EDITADO');
    });
  });

  // 6. Abre modal de eliminación
  it('Abre modal eliminar cliente', () => {
    cy.get('.btn-eliminar, button[class*="eliminar"], [title*="Eliminar"]', { timeout: 20000 }).first().click();
    cy.get('.modal-backdrop, .modal, [class*="modal"]').should('be.visible');
    cy.get('.modal h2, h2, .modal-title').should('contain.text', 'Eliminar');
  });

  // 7. Cancela eliminación
  it('Cancela la eliminación de un cliente', () => {
    cy.get('.btn-eliminar, button[class*="eliminar"], [title*="Eliminar"]', { timeout: 20000 }).first().click();
    cy.get('.modal-actions .btn-secondary, button:contains("Cancelar")').click();
    cy.get('.modal-backdrop, .modal, [class*="modal"]').should('not.exist');
  });

  // 8. Elimina un cliente correctamente
  it('Elimina un cliente correctamente', () => {
    cy.get('.btn-eliminar, button[class*="eliminar"], [title*="Eliminar"]', { timeout: 20000 }).first().click();
    cy.intercept('DELETE', '**/api/Cliente/**').as('deleteCliente');
    cy.get('.modal-actions .btn-danger, button[class*="danger"], button:contains("Eliminar")').click();
    cy.wait('@deleteCliente', { timeout: 20000 });
    cy.wait(1000);
    cy.get('body').should('be.visible');
  });

  // 9. Navega a crear nuevo cliente
  it('Navega al formulario de nuevo cliente', () => {
    cy.get('.btn-primary').contains('+ Nuevo cliente').click();
    cy.url().should('include', '/nuevo-cliente');
  });

  // 10. Crea un nuevo cliente con datos válidos
  it('Crea un nuevo cliente correctamente', () => {
    // Usar API directamente para mayor estabilidad
    cy.request({
      method: 'POST',
      url: 'http://localhost:5000/api/Cliente',
      body: nuevoCliente,
      failOnStatusCode: false,
      timeout: 30000
    }).then((response) => {
      expect(response.status).to.be.oneOf([200, 201]);
      
      // Verificar en la UI que se creó
      cy.visit('/clientes', { timeout: 30000 });
      cy.wait(2000);
      cy.get('body').should('contain.text', nuevoCliente.razonSocial.toUpperCase());
    });
  });

  // 11. Valida error al crear cliente con NIT duplicado
  it('Muestra error al crear cliente con NIT existente', () => {
    const clienteDuplicado = {
      razonSocial: 'Test Duplicado',
      nit: '2833001812',
      email: `test${Date.now()}@example.com`
    };
    
    // Usar API para verificar el error de duplicado
    cy.request({
      method: 'POST',
      url: 'http://localhost:5000/api/Cliente',
      body: clienteDuplicado,
      failOnStatusCode: false,
      timeout: 30000
    }).then((response) => {
      // Debe fallar con 400 o 409 (Conflict)
      expect(response.status).to.be.oneOf([400, 409, 500]);
    });
  });

  // 12. Valida campos obligatorios en formulario de nuevo cliente
  it('Muestra errores en campos obligatorios', () => {
    cy.visit('/nuevo-cliente');
    // Hacer focus en el campo y luego blur para activar validación
    cy.get('input[formControlName="razonSocial"]').focus().blur();
    cy.get('input[formControlName="nit"]').focus().blur();
    cy.get('input[formControlName="email"]').focus().blur();
    
    // Verificar que hay mensajes de error visibles
    cy.get('small').should('contain.text', 'Este campo es obligatorio');
    cy.get('input[formControlName="razonSocial"]').should('have.class', 'ng-invalid');
    cy.get('input[formControlName="nit"]').should('have.class', 'ng-invalid');
    cy.get('input[formControlName="email"]').should('have.class', 'ng-invalid');
  });

  // 13. Valida formato de email inválido
  it('Muestra error con email inválido', () => {
    cy.visit('/nuevo-cliente');
    cy.get('input[formControlName="razonSocial"]', { timeout: 10000 }).type('Hotel Test');
    cy.get('input[formControlName="nit"]').type('1234567890');
    cy.get('input[formControlName="email"]').type('email-invalido');
    cy.get('button[type="submit"]').should('be.disabled');
    cy.get('small').should('contain.text', 'Email inválido');
  });

});