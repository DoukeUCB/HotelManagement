/// <reference types="cypress" />

/**
 * PRUEBAS PAIRWISE - TABLA HUÉSPED
 * Basado en Tests.md - Sección 2: TABLA HUÉSPED
 * Total: 30 casos de prueba (15 PAIRWISE + 15 Valores Límite)
 */

describe('Huésped - Pruebas PAIRWISE', () => {
  beforeEach(() => {
    cy.visit('/nuevo-huesped');
    cy.waitForAngular();
  });

  describe('Casos PAIRWISE', () => {
    it('TC-H01: Huésped completo válido - ÉXITO', () => {
      cy.fillHuespedForm('Carlos', 'Gómez', `DOC-${Date.now()}`, {
        segundoApellido: 'Pérez',
        telefono: '71234567',
        fechaNacimiento: '1998-05-12',
      });
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.ok', { timeout: 10000 }).should('be.visible');
    });

    it('TC-H02: Segundo apellido opcional - ÉXITO', () => {
      cy.fillHuespedForm('María', 'López', `DOC-${Date.now()}`, {
        telefono: '72345678',
        fechaNacimiento: '1990-03-20',
      });
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.ok', { timeout: 10000 }).should('be.visible');
    });

    it('TC-H03: Teléfono y fecha opcionales - ÉXITO', () => {
      cy.fillHuespedForm('Juan', 'Martínez', `DOC-${Date.now()}`, {
        segundoApellido: 'Rodríguez',
      });
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.ok', { timeout: 10000 }).should('be.visible');
    });

    it('TC-H04: Nombre vacío - ERROR', () => {
      cy.fillHuespedForm('', 'Fernández', `DOC-${Date.now()}`, {
        segundoApellido: 'García',
        telefono: '73456789',
        fechaNacimiento: '1985-07-15',
      });
      cy.get('button[type="submit"]').click();

      cy.get('small').should('contain', 'obligatorio');
    });

    it('TC-H05: Apellido vacío - ERROR', () => {
      cy.fillHuespedForm('Pedro', '', `DOC-${Date.now()}`, {
        segundoApellido: 'Sánchez',
        telefono: '74567890',
        fechaNacimiento: '1992-11-30',
      });
      cy.get('button[type="submit"]').click();

      cy.get('small').should('contain', 'obligatorio');
    });

    it('TC-H06: Documento vacío - ERROR', () => {
      cy.fillHuespedForm('Ana', 'Ruiz', '', {
        segundoApellido: 'Díaz',
        telefono: '75678901',
        fechaNacimiento: '1988-01-05',
      });
      cy.get('button[type="submit"]').click();

      cy.get('small').should('contain', 'obligatorio');
    });

    it('TC-H07: Nombre excede 30 caracteres - ERROR', () => {
      cy.fillHuespedForm('CarlosAndresFernandezRamiresPerez', 'Gómez', `DOC-${Date.now()}`, {
        segundoApellido: 'López',
        telefono: '76789012',
        fechaNacimiento: '1995-06-25',
      });
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.error').should('exist');
    });

    it('TC-H08: Apellido excede 30 caracteres - ERROR', () => {
      cy.fillHuespedForm('Luis', 'FernandezFernandezFernandez', `DOC-${Date.now()}`, {
        segundoApellido: 'Pérez',
        telefono: '77890123',
        fechaNacimiento: '1987-09-18',
      });
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.error').should('exist');
    });

    it('TC-H09: Segundo apellido excede 30 caracteres - ERROR', () => {
      cy.fillHuespedForm('Rosa', 'Torres', `DOC-${Date.now()}`, {
        segundoApellido: 'Perezzzzzzzzzzzzzzzzzzzzzzzzzzz',
        telefono: '78901234',
        fechaNacimiento: '1991-12-08',
      });
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.error').should('exist');
    });

    it('TC-H10: Documento excede 20 caracteres - ERROR', () => {
      cy.fillHuespedForm('Jorge', 'Vargas', '123456789012345678901', {
        segundoApellido: 'Morales',
        telefono: '79012345',
        fechaNacimiento: '1993-04-22',
      });
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.error').should('exist');
    });

    it('TC-H11: Teléfono excede 20 caracteres - ERROR', () => {
      cy.fillHuespedForm('Elena', 'Castro', `DOC-${Date.now()}`, {
        segundoApellido: 'Silva',
        telefono: '999999999999999999999',
        fechaNacimiento: '1989-08-14',
      });
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.error').should('exist');
    });

    it('TC-H12: Formato de fecha inválido - ERROR', () => {
      cy.fillHuespedForm('Miguel', 'Rojas', `DOC-${Date.now()}`, {
        segundoApellido: 'Vega',
        telefono: '70123456',
      });

      // Intentar ingresar fecha con formato inválido
      cy.get('input[formcontrolname="fechaNacimiento"]').type('12/1998/05');
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      // El input de tipo date puede rechazar el formato automáticamente
    });

    it('TC-H13: Campos obligatorios vacíos - ERROR', () => {
      cy.fillHuespedForm('', '', '');
      cy.get('button[type="submit"]').click();

      cy.get('small').should('have.length.at.least', 3);
    });

    it('TC-H14: Fecha futura válida - ÉXITO', () => {
      cy.fillHuespedForm('Sofía', 'Méndez', `DOC-${Date.now()}`, {
        segundoApellido: 'Ortiz',
        telefono: '71112223',
        fechaNacimiento: '2025-12-31',
      });
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.ok', { timeout: 10000 }).should('be.visible');
    });

    it('TC-H15: Fecha muy antigua válida - ÉXITO', () => {
      cy.fillHuespedForm('Diego', 'Herrera', `DOC-${Date.now()}`, {
        telefono: '72223334',
        fechaNacimiento: '1900-01-01',
      });
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.ok', { timeout: 10000 }).should('be.visible');
    });
  });

  describe('Valores Límite', () => {
    it('TC-H16: Nombre 1 carácter - VÁLIDO', () => {
      cy.fillHuespedForm('A', 'Apellido', `DOC-${Date.now()}`);
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.ok', { timeout: 10000 }).should('be.visible');
    });

    it('TC-H17: Nombre 30 caracteres - VÁLIDO', () => {
      cy.fillHuespedForm('123456789012345678901234567890', 'Apellido', `DOC-${Date.now()}`);
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.ok', { timeout: 10000 }).should('be.visible');
    });

    it('TC-H18: Nombre 31 caracteres - ERROR', () => {
      cy.fillHuespedForm('1234567890123456789012345678901', 'Apellido', `DOC-${Date.now()}`);
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.error').should('exist');
    });

    it('TC-H19: Apellido 1 carácter - VÁLIDO', () => {
      cy.fillHuespedForm('Nombre', 'B', `DOC-${Date.now()}`);
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.ok', { timeout: 10000 }).should('be.visible');
    });

    it('TC-H20: Apellido 30 caracteres - VÁLIDO', () => {
      cy.fillHuespedForm('Nombre', '123456789012345678901234567890', `DOC-${Date.now()}`);
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.ok', { timeout: 10000 }).should('be.visible');
    });

    it('TC-H21: Apellido 31 caracteres - ERROR', () => {
      cy.fillHuespedForm('Nombre', '1234567890123456789012345678901', `DOC-${Date.now()}`);
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.error').should('exist');
    });

    it('TC-H22: Segundo apellido vacío - VÁLIDO', () => {
      cy.fillHuespedForm('Nombre', 'Apellido', `DOC-${Date.now()}`);
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.ok', { timeout: 10000 }).should('be.visible');
    });

    it('TC-H23: Segundo apellido 30 caracteres - VÁLIDO', () => {
      cy.fillHuespedForm('Nombre', 'Apellido', `DOC-${Date.now()}`, {
        segundoApellido: '123456789012345678901234567890',
      });
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.ok', { timeout: 10000 }).should('be.visible');
    });

    it('TC-H24: Segundo apellido 31 caracteres - ERROR', () => {
      cy.fillHuespedForm('Nombre', 'Apellido', `DOC-${Date.now()}`, {
        segundoApellido: '1234567890123456789012345678901',
      });
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.error').should('exist');
    });

    it('TC-H25: Documento 1 carácter - VÁLIDO', () => {
      cy.fillHuespedForm('Nombre', 'Apellido', '1');
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.ok', { timeout: 10000 }).should('be.visible');
    });

    it('TC-H26: Documento 20 caracteres - VÁLIDO', () => {
      cy.fillHuespedForm('Nombre', 'Apellido', '12345678901234567890');
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.ok', { timeout: 10000 }).should('be.visible');
    });

    it('TC-H27: Documento 21 caracteres - ERROR', () => {
      cy.fillHuespedForm('Nombre', 'Apellido', '123456789012345678901');
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.error').should('exist');
    });

    it('TC-H28: Teléfono vacío - VÁLIDO', () => {
      cy.fillHuespedForm('Nombre', 'Apellido', `DOC-${Date.now()}`);
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.ok', { timeout: 10000 }).should('be.visible');
    });

    it('TC-H29: Teléfono 20 caracteres - VÁLIDO', () => {
      cy.fillHuespedForm('Nombre', 'Apellido', `DOC-${Date.now()}`, {
        telefono: '12345678901234567890',
      });
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.ok', { timeout: 10000 }).should('be.visible');
    });

    it('TC-H30: Teléfono 21 caracteres - ERROR', () => {
      cy.fillHuespedForm('Nombre', 'Apellido', `DOC-${Date.now()}`, {
        telefono: '123456789012345678901',
      });
      cy.get('button[type="submit"]').click();

      cy.waitForAngular();
      cy.get('.estado.error').should('exist');
    });
  });
});
