# language: es
@ui @habitaciones
Característica: Gestión de Habitaciones UI
  Como administrador del hotel
  Quiero gestionar las habitaciones (CRUD) bajo el patrón POM
  Para validar el sistema con datos de Pairwise y Happy Path

  Antecedentes:
    Dado que la aplicación web está en ejecución
    Y que navego a la página de listado de habitaciones

  @insert @pairwise
  Esquema del escenario: HP01 - Insertar nuevas habitaciones usando Pairwise
    Cuando hago click en el botón "Nueva Habitación"
    Y ingreso el número de habitación "<Numero>"
    Y ingreso el piso "<Piso>"
    Y selecciono el tipo de habitación "<Tipo>"
    Y selecciono el estado "Disponible"
    Y guardo la habitación
    Entonces debería ver la habitación "<Numero>" en la lista
    Y el mensaje de éxito debería ser visible

    Ejemplos:
      | TC_ID | Numero | Piso | Tipo   |
      | H-01  | 701A   | 1    | Suite  |
      | H-02  | 502A   | 1    | Simple |
      | H-03  | 801B   | 2    | Doble  |
      | H-04  | 301D   | 3    | Familiar |

  @select @happy-path
  Escenario: HP02 - Consultar una habitación existente
    Cuando busco la habitación "102A" en la lista
    Entonces debería ver la habitación "102A" en la lista

  @update @pairwise
  Escenario: HP03 - Actualizar estado de habitación usando Pairwise
    Dado que busco la habitación "101A" en la lista
    Cuando hago click en el botón "Editar" de la habitación "101A"
    Y selecciono el tipo de habitación "Simple"
    Y cambio el estado a "Fuera de Servicio"
    Y guardo los cambios
    Entonces el estado de la habitación "101A" debería ser "Fuera de Servicio"

  @delete @happy-path
  Escenario: HP04 - Eliminar una habitación existente
    Dado que busco la habitación "301C" en la lista
    Cuando elimino la habitación "301C" y confirmo
    Entonces la habitación "301C" ya no debería aparecer en la lista