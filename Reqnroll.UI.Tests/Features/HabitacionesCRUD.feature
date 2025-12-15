Feature: Habitaciones UI
  Como recepcionista
  Quiero gestionar habitaciones a través de la interfaz web
  Para mantener el inventario de habitaciones

  Background:
    Dado que la aplicación web está en ejecución
    Y que existen datos de prueba en el sistema

  @ui @habitaciones @insert @pairwise
  Scenario Outline: Crear habitación (Insert) - pruebas pairwise
    Dado que navego a la página de habitaciones
    Cuando ingreso el número de habitación "<Numero>"
    Y selecciono el tipo "<TipoHabitacion>"
    Y ingreso la tarifa "<Tarifa>"
    Y hago click en guardar habitación
    Entonces debería ver un mensaje de éxito
    Y debería ver el ID de la habitación creada

    Examples:
      | Numero | TipoHabitacion | Tarifa  |
      | 101A   | Simple         | 100.00  |
      | 102A   | Doble          | 0.00    |
      | 103A   | Suite          | 9999.99 |
      | 104A   | Simple         | -1.00   |

  @ui @habitaciones @update @pairwise
  Scenario Outline: Actualizar habitación (Update) - pruebas pairwise
    Dado que navego a la página de habitaciones
    Y busco la habitación "<NumeroBusqueda>"
    Y hago click en editar habitación
    Y modifico la tarifa a "<NuevaTarifa>"
    Y hago click en guardar cambios
    Entonces debería ver un mensaje de éxito

    Examples:
      | NumeroBusqueda | NuevaTarifa |
      | 101A           | 120.00      |
      | 102A           | 0.00        |
      | 103A           | 1500.50     |

  @ui @habitaciones @delete @happy-path
  Scenario: Eliminar habitación existente (Happy Path)
    Dado que navego a la página de habitaciones
    Cuando busco la habitación "101A"
    Y hago click en eliminar habitación
    Y confirmo la eliminación
    Entonces debería ver un mensaje de confirmación de eliminación

  @ui @habitaciones @select @happy-path
  Scenario: Buscar/Seleccionar habitación (Happy Path)
    Dado que navego a la página de habitaciones
    Cuando busco la habitación "101A"
    Entonces debería ver la habitación en la lista
