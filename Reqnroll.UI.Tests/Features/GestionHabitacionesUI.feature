# language: es
@ui @habitaciones
Característica: Gestión de Habitaciones UI
  Como administrador del hotel
  Quiero gestionar las habitaciones a través de la interfaz web
  Para mantener el inventario actualizado

  Antecedentes:
    Dado que la aplicación web está en ejecución
    Y que navego a la página de listado de habitaciones

  @happy-path @insert
  Esquema del escenario: HP01 - Insertar nuevas habitaciones exitosamente
    Cuando hago click en el botón "Nueva Habitación"
    Y ingreso el número de habitación "<Numero>"
    Y ingreso el piso "<Piso>"
    Y selecciono el tipo de habitación "<Tipo>"
    Y selecciono el estado "Disponible"
    Y guardo la habitación
    Entonces debería ver la habitación "<Numero>" en la lista

    Ejemplos:
      | Numero | Piso | Tipo              |
      | 601A   | 6    | Habitación Simple |
      | 701B   | 7    | Suite Familiar    |

  @happy-path @update
  Escenario: HP02 - Actualizar el estado de una habitación existente
    Dado que busco la habitación "102A" en la lista
    Cuando hago click en el botón "Editar" de la habitación "102A"
    Y cambio el estado a "Fuera de Servicio"
    Y guardo los cambios
    Entonces el estado de la habitación "102A" debería ser "Fuera de Servicio"