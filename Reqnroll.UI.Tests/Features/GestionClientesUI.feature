# language: es
Característica: Gestión de Clientes UI
    Como administrador del sistema
    Quiero registrar nuevos clientes
    Para poder realizar reservas a su nombre

@UI @Insert
Esquema del escenario: Insertar nuevo cliente con datos variados (Pairwise)
    Dado que estoy en la página de creación de clientes
    Cuando ingreso la Razón Social "<RazonSocial>"
    Y ingreso el NIT "<NIT>"
    Y ingreso el Email "<Email>"
    Y hago click en guardar cliente
    Entonces debería ver un mensaje de éxito indicando que se guardó correctamente

Ejemplos:
      | Descripcion Casos         | RazonSocial          | NIT                  | Email                          |
      # 1. Todo al Mínimo permitido (Frontera Inferior)
      | Limites Minimos           | ABC                  | 1234567              | a@b.co                         |
      
      # 2. Todo al Máximo permitido (Frontera Superior) - 20 chars RS, 20 chars NIT, 30 chars Email
    | Limites Maximos           | Empresa Veinte Letra | 1234567890123         | 1234567890123456789@correo.com |
      
      # 3. RS Máximo, NIT Mínimo (Combinación Cruzada 1)
      | RS Max / NIT Min          | Industrias Del Sur O | 7654321              | info@tuhotel.com               |
      
      # 4. RS Mínimo, NIT Máximo (Combinación Cruzada 2)
    | RS Min / NIT Max          | SOL                  | 9999999999999         | reservas@web.bo                |
      
      # 5. Valor Medio (Happy Path Típico)
      | Valores Tipicos           | Hotel Central        | 4578120              | contacto@hotel.com             |
      
      # 6. Prueba de Límite de Email (30 chars exactos)
      | Email Maximo Exacto       | Cliente Normal       | 88776655             | un.cliente.largo@dominio.co.bo |

# ... (Los escenarios de Insert se quedan arriba igual) ...

@UI @Select
Escenario: Crear un cliente nuevo y verificar su visualización en el listado
    # --- FASE 1: CREACIÓN (Reutilizando tus steps de Insert) ---
    Dado que estoy en la página de creación de clientes
    Cuando ingreso la Razón Social "Cliente Auto Test"
    Y ingreso el NIT "99887766"
    Y ingreso el Email "auto.test@hotel.com"
    Y hago click en guardar cliente
    
    # --- FASE 2: VERIFICACIÓN (Usando los steps nuevos de Select) ---
    # Asumimos que al guardar, el sistema nos redirige al listado o nos quedamos listos para ir.
    # Por seguridad, forzamos la navegación al listado para asegurar que la tabla se refresque.
    
    Dado que navego a la página de listado de clientes
    Cuando busco el cliente por Razón Social "Cliente Auto Test"
    Entonces debería ver en la grilla al cliente con Razón Social "Cliente Auto Test", NIT "99887766" y Email "auto.test@hotel.com"

# ... (Mantén todo lo anterior igual) ...

@UI @Update
Esquema del escenario: Editar un cliente existente desde la ventana modal (Pairwise)
    # Precondición: Necesitamos un cliente específico para editar.
    # En un entorno real, esto se crea por API o SQL antes del test.
    # Aquí asumimos que el sistema tiene un cliente "Cliente Base Edicion" o lo creamos al vuelo.
    Dado que existe un cliente registrado con Razón Social "Cliente Base Edicion"
    Y navego a la página de listado de clientes
    
    Cuando busco el cliente "Cliente Base Edicion"
    Y hago click en el boton editar del cliente encontrado
    
    # Aquí interactuamos con la "ventana pequeña"
    Y actualizo la Razón Social a "<RazonSocial>"
    #Y actualizo el NIT a "<NIT>"
    Y actualizo el Email a "<Email>"
    Y guardo los cambios de la edicion
    
    Entonces debería ver los datos actualizados: Razón Social "<RazonSocial>", NIT "<NIT>" y Email "<Email>"
Ejemplos:
 | Descripcion Casos            | RazonSocial          | NIT                  | Email                          |
      # 1. RS Max (20) / NIT Min (7) / Email Max (30)
      # Prueba si el Update soporta llenar campos de texto y vaciar numéricos al límite
      | Update Frontera Cruzada 1    | Empresa Modificada X | 1112223              | actualizacion.limite@correo.bo |
      
      # 2. RS Min (3) / NIT Max (20) / Email Corto
      # Prueba lo inverso: Nombre muy corto con documento muy largo
      | Update Frontera Cruzada 2    | UPD                  | 99999999999999999999 | nuevo@min.com                  |
      
      # 3. Todo al Mínimo (Frontera Inferior)
      # Verifica que la edición no rompa reglas al reducir datos a su mínima expresión
      | Update Limites Minimos       | Eco                  | 1000001              | a@b.eu                         |
      
      # 4. Todo al Máximo (Frontera Superior)
      # Verifica persistencia correcta cuando todos los campos usan su capacidad total
      | Update Limites Maximos       | Inversiones Globales | 88888888888888888888 | gerencia.general@inversiones.c |

      # 5. Valores Medios (Happy Path)
      # Datos estándar sin bordes, lo más común en producción
      | Update Valores Tipicos       | Comercial Norte      | 44556677             | ventas@norte.com               |

      # 6. RS Medio / NIT Mínimo / Email Máximo (Mix Frontera)
      # Combinación para detectar fallos cuando un campo es muy largo y otro muy corto
      | Update Mix Frontera          | Farmacia Vida        | 1002003              | atencion.cliente.vip@correo.bo |

@UI @Delete
Escenario: Eliminar un cliente existente desde el listado
    # Precondición: Creamos un cliente específico para borrar y no afectar otros tests
    Dado que existe un cliente registrado para eliminar con Razón Social "Cliente A Borrar"
    Y navego a la página de listado de clientes
    
    Cuando busco el cliente "Cliente A Borrar"
    Y hago click en el boton eliminar del cliente encontrado
    Y confirmo la eliminación en la ventana modal
    
    Entonces el cliente "Cliente A Borrar" ya no debería aparecer en el listado