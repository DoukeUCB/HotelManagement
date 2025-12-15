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
      | Limites Maximos           | Empresa Veinte Letra | 12345678901234567890 | 1234567890123456789@correo.com |
      
      # 3. RS Máximo, NIT Mínimo (Combinación Cruzada 1)
      | RS Max / NIT Min          | Industrias Del Sur O | 7654321              | info@tuhotel.com               |
      
      # 4. RS Mínimo, NIT Máximo (Combinación Cruzada 2)
      | RS Min / NIT Max          | SOL                  | 99999999999999999999 | reservas@web.bo                |
      
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
    Y actualizo el NIT a "<NIT>"
    Y actualizo el Email a "<Email>"
    Y guardo los cambios de la edicion
    
    Entonces debería ver los datos actualizados: Razón Social "<RazonSocial>", NIT "<NIT>" y Email "<Email>"
Ejemplos:
      | Descripcion Casos            | RazonSocial          | NIT                  | Email                          |
      # Pairwise para Update (Combinaciones ortogonales)
      | Update Limites Minimos       | AERFSF                    | 13333333                    | correo.corto@upd.com                        |
   #   | Update Limites Maximos       | Editado Veinte Carac | 9999999 | editado.largo.30ch@test.com.bo |
   #   | Update Mezcla 1 (RS Max)     | Industrias Editadas  | 5553333                  | correo.corto@upd.com           |
   #   | Update Mezcla 2 (Nit Max)    | EDIT                 | 8888888 | medio@hotel.com                |