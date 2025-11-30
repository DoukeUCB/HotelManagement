Feature: Gestión de Huéspedes
  Como recepcionista del hotel
  Quiero gestionar los huéspedes del sistema
  Para poder asociarlos correctamente a las reservas

  # ============================
  #        INSERT (3)
  # ============================

  # Insert Happy 1
  Scenario: HP07 - Crear huésped correctamente
    Given que tengo los siguientes datos de huésped:
      | Campo               | Valor      |
      | Nombre              | Juan       |
      | Apellido            | Pérez      |
      | Segundo_Apellido    | López      |
      | Documento_Identidad | 12345678   |
      | Telefono            | 77777777   |
      | Fecha_Nacimiento    | 1990-01-01 |
    When intento registrar el huésped
    Then el huésped debe crearse exitosamente
    And debo poder consultar el huésped por su documento "12345678"

  # Insert Happy 2
  Scenario: HP08 - Crear huésped solo con datos obligatorios
    Given que tengo los siguientes datos de huésped:
      | Campo               | Valor    |
      | Nombre              | Ana      |
      | Apellido            | López    |
      | Documento_Identidad | 22222222 |
    When intento registrar el huésped
    Then el huésped debe crearse exitosamente
    And debo poder consultar el huésped por su documento "22222222"

  # Insert Unhappy 1
  Scenario: HP09 - Intentar crear huésped sin nombre
    Given que tengo los siguientes datos de huésped:
      | Campo               | Valor    |
      | Nombre              |          |
      | Apellido            | Gómez    |
      | Documento_Identidad | 99999999 |
    When intento registrar el huésped
    Then la creación de huésped debe fallar
    And el error de huésped debe indicar "nombre"

  # ============================
  #        UPDATE (3)
  # ============================

  # Update Happy 1
  Scenario: HP10 - Actualizar teléfono de un huésped correctamente
    Given que ya existe un huésped con documento "33333333"
    When actualizo el huésped con los siguientes datos:
      | Campo    | Valor     |
      | Telefono | 77777777  |
    Then los datos del huésped deben actualizarse correctamente

  # Update Happy 2
  Scenario: HP11 - Actualizar nombre y apellidos de un huésped correctamente
    Given que ya existe un huésped con documento "44444444"
    When actualizo el huésped con los siguientes datos:
      | Campo    | Valor      |
      | Nombre   | Laura Ana  |
      | Apellido | Fernández  |
    Then los datos del huésped deben actualizarse correctamente

  # Update Unhappy 1
  Scenario: HP12 - Intentar actualizar huésped con teléfono inválido
    Given que ya existe un huésped con documento "77777777"
    When actualizo el huésped con los siguientes datos:
      | Campo    | Valor |
      | Telefono | 123   |
    Then la creación de huésped debe fallar
    And el error de huésped debe indicar "telefono"

  # ============================
  #      DELETE (1)
  # ============================

  Scenario: HP13 - Eliminar huésped correctamente
    Given que ya existe un huésped con documento "66666666"
    When elimino el huésped con documento "66666666"
    Then el huésped no debe existir en el sistema

  # ============================
  #       SELECT (1)
  # ============================

  Scenario: HP14 - Consultar huésped existente
    Given que ya existe un huésped con documento "11111111"
    When consulto el huésped con documento "11111111"
    Then el huésped debe existir en el sistema
