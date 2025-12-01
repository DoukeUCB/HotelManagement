# language: es
Característica: Gestión de Habitaciones
  Como administrador del hotel
  Quiero realizar el CRUD completo de habitaciones
  Para mantener el inventario de cuartos actualizado

  Antecedentes:
    # Se necesita un TipoHabitacion existente para crear una Habitacion
    Dado que existe un Tipo de Habitación para pruebas:
      | Campo            | Valor      |
      | Nombre           | Suite Test |
      | Capacidad_Maxima | 2          |
      | Precio_Base      | 150.00     |

  # ============================================================
  # HAPPY PATH - 3 Escenarios Exitosos
  # ============================================================

  @happy-path @creacion
  Escenario: HP01 - Crear una nueva habitación
    Dado que el Tipo de Habitación para la prueba existe
    Cuando creo una nueva habitación con los siguientes datos:
      | Numero_Habitacion | Piso | Estado_Habitacion |
      | 105               | 1    | Libre             |
      
    Entonces la habitación debe crearse exitosamente
    Y debe tener el número de habitación "105"

  @happy-path @consulta
  Escenario: HP02 - Consultar y obtener una habitación por ID
    Dado que existe una habitación con número "201" y piso "2" en el sistema
    Cuando consulto la habitación por su ID
    Entonces debo obtener la información de la habitación
    Y la habitación consultada debe tener el piso "2"

  @happy-path @actualizacion
  Escenario: HP03 - Actualizar el estado de una habitación
    Dado que existe una habitación con número "305" y estado "Libre" en el sistema
    Cuando actualizo el estado de la habitación a "Mantenimiento"
    Entonces la habitación debe actualizarse exitosamente
    Y su nuevo estado debe ser "Mantenimiento"

  # ============================================================
  # UNHAPPY PATH - 3 Escenarios de Error
  # ============================================================

  @unhappy-path @duplicado
  Escenario: UP01 - Error al crear una habitación con número duplicado
    Dado que ya existe una habitación con el número "101"
    Cuando intento crear otra habitación con el número "101"
    Entonces la creación debe fallar
    Y el error debe indicar "número de habitación"

  @unhappy-path @tipo-habitacion-invalido
  Escenario: UP02 - Error al crear habitación con TipoHabitacion inexistente
    Cuando intento crear una nueva habitación con Tipo de Habitación inválido
    Entonces la operación debe fallar
    Y el error debe indicar "tipo de habitación"

  @unhappy-path @eliminacion-invalida
  Escenario: UP03 - Error al eliminar una habitación inexistente
    Dado que el sistema no contiene ninguna habitación con ID "00000000-0000-0000-0000-000000000000"
    Cuando intento eliminar esa habitación
    Entonces la operación debe fallar
    Y el error debe indicar "No se encontró"