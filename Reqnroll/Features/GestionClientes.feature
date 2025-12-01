# language: es
Característica: Gestión de Clientes
  Como administrador
  Quiero registrar clientes
  Para poder asignarles reservas

  Antecedentes:
    Dado que el sistema de clientes está limpio

  # ============================================================
  # HAPPY PATH
  # ============================================================
  @gestion-clientes @crear-cliente
  Escenario: GC01 - Registrar cliente correctamente
    Dado que tengo los datos del nuevo cliente:
      | Razon_Social  | NIT        | Email                  |
      | NUEVO CLIENTE | 9876543210 | nuevo.cliente@test.com |
    

    Cuando ejecuto el registro del cliente
    

    Entonces el cliente debe crearse correctamente en el sistema
    Y al consultar el NIT "9876543210" debe existir el cliente

  
  @gestion-clientes @crear-otrocliente
  Escenario: GC02 - Registrar cliente correctamente otro
    Dado que tengo los datos del nuevo cliente:
      | Razon_Social | NIT        | Email                   |
      | OTRO CLIENTE | 123456789 | cliente@test.com | 
      # Asegúrate que 123456789 sea un NIT válido o deshabilita la validación estricta.

  
    Cuando ejecuto el registro del cliente

  
    Entonces el cliente debe crearse correctamente en el sistema
    Y al consultar el NIT "123456789" debe existir el cliente

  # ============================================================
  # SAD PATH (Casos de Error)
  # ============================================================
  @gestion-clientes @error-duplicado
  Escenario: GC03 - Intentar registrar un cliente con NIT duplicado
    # Primero, creamos un cliente existente en la base de datos
    Dado que ya existe un cliente en el sistema con:
      | Razon_Social   | NIT        | Email             |
      | CLIENTE ORIGINAL | 1234567890 | original@test.com |

    # Ahora intentamos registrar OTRO con el MISMO NIT
    Cuando intento registrar otro cliente con:
      | Razon_Social   | NIT        | Email             |
      | CLIENTE COPIA  | 1234567890 | copia@test.com    |

    # Verificaciones de rechazo
    Entonces el registro debe ser rechazado
    Y el mensaje de error debe mencionar "errores de validación" 


  @gestion-clientes @eliminar-cliente
  Escenario: GC04 - Eliminar un cliente existente
    Dado que ya existe un cliente en el sistema con:
      | Razon_Social   | NIT        | Email            |
      | CLIENTE BORRAR | 1122334455 | borrar@test.com  |
    
    Cuando elimino el cliente con NIT "1122334455"
    
    Entonces la eliminación debe ser exitosa
    Y el cliente con NIT "1122334455" ya no debe existir en el sistema

  @gestion-clientes @actualizar-cliente
  Escenario: GC05 - Actualizar datos de un cliente
    Dado que ya existe un cliente en el sistema con:
      | Razon_Social    | NIT       | Email             |
      | CLIENTE ANTIGUO | 777777777 | antiguo@test.com  |
    
    Cuando actualizo el cliente con NIT "777777777" con los nuevos datos:
      | Razon_Social  | Email           |
      | CLIENTE RENOVADO | nuevo@test.com |
    
    Entonces la actualización debe ser exitosa
    Y al consultar el NIT "777777777" el cliente debe tener la Razon Social "CLIENTE RENOVADO"
    Y el email debe ser "nuevo@test.com"

  @gestion-clientes @actualizar-correccion
  Escenario: GC06 - Corregir error tipográfico en Razon Social
    Dado que ya existe un cliente en el sistema con:
      | Razon_Social | NIT        | Email             |
      | CLINTE MALO  | 888888888  | error@test.com    |
    
    Cuando actualizo el cliente con NIT "888888888" con los nuevos datos:
      | Razon_Social      | Email           |
      | CLIENTE CORREGIDO | error@test.com  |
    
    Entonces la actualización debe ser exitosa
    Y al consultar el NIT "888888888" el cliente debe tener la Razon Social "CLIENTE CORREGIDO"

  @gestion-clientes @error-actualizacion
  Escenario: GC07 - Intentar actualizar con email inválido
    Dado que ya existe un cliente en el sistema con:
      | Razon_Social   | NIT        | Email             |
      | CLIENTE VALIDO | 999999999  | valido@test.com   |
    
    Cuando actualizo el cliente con NIT "999999999" con los nuevos datos:
      | Razon_Social   | Email          |
      | CLIENTE VALIDO | email-sin-arroba |
    
    Entonces la actualización debe ser rechazada
    Y el mensaje de error debe mencionar "Email"

  @gestion-clientes @consultar-cliente
  Escenario: GC08 - Consultar datos de un cliente existente
    Dado que ya existe un cliente en el sistema con:
      | Razon_Social   | NIT       | Email            |
      | CLIENTE SELECT | 555000555 | buscar@test.com  |
    
    Cuando consulto los datos del cliente con NIT "555000555"
    
    Entonces la consulta debe ser exitosa
    Y los datos del cliente obtenido deben coincidir:
      | Razon_Social   | Email           |
      | CLIENTE SELECT | buscar@test.com |