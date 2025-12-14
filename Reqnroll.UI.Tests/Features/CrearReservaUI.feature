# language: es
Característica: Crear Reserva UI
  Como recepcionista del hotel
  Quiero crear una reserva a través de la interfaz web
  Para gestionar las reservas de los huéspedes de forma visual

  Antecedentes:
    Dado que la aplicación web está en ejecución
    Y que existen datos de prueba en el sistema

  @ui @happy-path
  Esquema del escenario: Crear reserva exitosa con una habitación
    Dado que navego a la página de nueva reserva
    Cuando selecciono el cliente "<Cliente>"
    Y selecciono el estado de reserva "<Estado>"
    Y avanzo al paso de habitaciones
    Y selecciono la habitación "<Habitacion>"
    Y ingreso la fecha de entrada "<FechaEntrada>"
    Y ingreso la fecha de salida "<FechaSalida>"
    Y agrego el huésped "<Huesped>"
    Y confirmo la reserva
    Entonces debería ver un mensaje de éxito
    Y debería ver el ID de la reserva creada

  Ejemplos:
    | Cliente         | Estado    | Habitacion | FechaEntrada | FechaSalida | Huesped        |
    | Empresa ABC     | Pendiente | 101A       | 2025-12-15   | 2025-12-18  | Jorge Quispe   |
    | Hotel Viajeros  | Pendiente | 102A       | 2025-12-20   | 2025-12-23  | Andrea Mamani  |

  @ui @happy-path @multiple-rooms
  Escenario: Crear reserva con múltiples habitaciones
    Dado que navego a la página de nueva reserva
    Cuando selecciono el cliente "Agencia Andes"
    Y selecciono el estado de reserva "Pendiente"
    Y avanzo al paso de habitaciones
    Y selecciono la habitación "103A"
    Y ingreso la fecha de entrada "2025-12-25"
    Y ingreso la fecha de salida "2025-12-28"
    Y selecciono el huésped "Jorge Quispe"
    Y hago click en agregar otra habitación
    Y selecciono la segunda habitación "104A"
    Y ingreso la segunda fecha de entrada "2025-12-26"
    Y ingreso la segunda fecha de salida "2025-12-29"
    Y selecciono el segundo huésped "Diego Rojas"
    Y avanzo al paso de confirmación
    Y confirmo la reserva
    Entonces debería ver un mensaje de éxito
    Y debería ver el ID de la reserva creada
