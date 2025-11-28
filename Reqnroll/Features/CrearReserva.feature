# language: es
Característica: Crear Reserva Completa
  Como recepcionista del hotel
  Quiero crear reservas siguiendo los pasos: Cliente, Habitaciones y Confirmación
  Para gestionar correctamente las reservas de los huéspedes

  Antecedentes:
    Dado que existen los siguientes datos de prueba:
      | Entidad        | Campo             | Valor                     |
      | Cliente        | Razon_Social      | EMPRESA TEST S.A.         |
      | Cliente        | NIT               | 1234567890                |
      | Cliente        | Email             | empresa.test@hotel.com    |
      | TipoHabitacion | Nombre            | Suite Test                |
      | TipoHabitacion | Capacidad_Maxima  | 2                         |
      | TipoHabitacion | Precio_Base       | 150.00                    |
      | Habitacion     | Numero_Habitacion | 101                       |
      | Habitacion     | Piso              | 1                         |
      | Huesped        | Nombre            | Juan                      |
      | Huesped        | Apellido          | Pérez                     |
      | Huesped        | Documento         | 12345678                  |

  # ============================================================
  # HAPPY PATH - 3 Escenarios Exitosos
  # ============================================================

  @happy-path @reserva-simple
  Escenario: HP01 - Crear reserva simple con una habitación y un huésped
    # PASO 1: Cliente
    Dado que selecciono el cliente "EMPRESA TEST S.A."
    Y el estado de reserva es "Pendiente"
    
    # PASO 2: Habitaciones
    Cuando agrego la habitación "101" con los siguientes datos:
      | Fecha_Entrada | Fecha_Salida | Huesped        |
      | 2025-12-01    | 2025-12-05   | Juan Pérez     |
    
    # PASO 3: Confirmación
    Y confirmo la reserva con monto total de 600.00
    
    Entonces la reserva debe crearse exitosamente
    Y debe contener 1 detalle de reserva
    Y el detalle debe tener la habitación "101"
    Y el detalle debe tener el huésped "Juan Pérez"

  @happy-path @reserva-multiple-habitaciones
  Escenario: HP02 - Crear reserva con múltiples habitaciones
    # PASO 1: Cliente
    Dado que selecciono el cliente "EMPRESA TEST S.A."
    Y el estado de reserva es "Confirmada"
    
    # PASO 2: Habitaciones (múltiples)
    Cuando agrego las siguientes habitaciones:
      | Habitacion | Fecha_Entrada | Fecha_Salida | Huesped    |
      | 101        | 2025-12-10    | 2025-12-15   | Juan Pérez |
      | 102        | 2025-12-10    | 2025-12-15   | Juan Pérez |
    
    # PASO 3: Confirmación
    Y confirmo la reserva con monto total de 1500.00
    
    Entonces la reserva debe crearse exitosamente
    Y debe contener 2 detalles de reserva

  @happy-path @consulta-reserva
  Escenario: HP03 - Consultar reserva existente con todos sus detalles
    Dado que existe una reserva completa en el sistema
    Cuando consulto la reserva por su ID
    Entonces debo obtener la información del cliente
    Y debo obtener los detalles de habitaciones
    Y debo obtener el monto total

  # ============================================================
  # UNHAPPY PATH - 3 Escenarios de Error
  # ============================================================

  @unhappy-path @cliente-invalido
  Escenario: UP01 - Error al crear reserva sin cliente válido
    # PASO 1: Cliente inválido
    Dado que intento crear una reserva con cliente inexistente "00000000-0000-0000-0000-000000000000"
    
    # PASO 2: Habitaciones
    Cuando agrego la habitación "101" con los siguientes datos:
      | Fecha_Entrada | Fecha_Salida | Huesped    |
      | 2025-12-01    | 2025-12-05   | Juan Pérez |
    
    # PASO 3: Confirmación
    Y confirmo la reserva con monto total de 600.00
    
    Entonces la creación debe fallar
    Y el error debe indicar "No se encontró un cliente"

  @unhappy-path @habitacion-invalida  
  Escenario: UP02 - Error al crear reserva con habitación inexistente
    # PASO 1: Cliente
    Dado que selecciono el cliente "EMPRESA TEST S.A."
    Y el estado de reserva es "Pendiente"
    
    # PASO 2: Habitación inválida
    Cuando intento agregar la habitación inexistente "999"
    
    Entonces la operación debe fallar
    Y el error debe indicar "habitación"

  @unhappy-path @fechas-invalidas
  Escenario: UP03 - Error al crear reserva con fechas inválidas
    # PASO 1: Cliente
    Dado que selecciono el cliente "EMPRESA TEST S.A."
    Y el estado de reserva es "Pendiente"
    
    # PASO 2: Fechas inválidas (salida antes de entrada)
    Cuando agrego la habitación "101" con fechas inválidas:
      | Fecha_Entrada | Fecha_Salida |
      | 2025-12-10    | 2025-12-05   |
    
    Entonces la operación debe fallar
    Y el error debe indicar "fecha"
