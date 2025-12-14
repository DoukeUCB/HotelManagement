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


@UI @Update
Esquema del escenario: Actualizar un cliente existente mediante el modal de edición
	Dado que navego a la página de lista de clientes
	Cuando hago click en el botón editar del cliente con documento "<DocumentoOriginal>"
	Y actualizo el formulario del modal con Razón Social "<NuevaRazon>" y Email "<NuevoEmail>"
	Y guardo los cambios en el modal
	Entonces el cliente con documento "<DocumentoOriginal>" debería mostrar la Razón Social "<NuevaRazon>" en la lista

Examples:
	| DocumentoOriginal | NuevaRazon           | NuevoEmail              |
	| 444555666         | EMPRESA UPDATE A     | contacto.a@test.com     |
	| 777888999         | COMERCIAL UPDATE B   | ventas.b@test.com       |
	| 123123123         | SERVICIOS UPDATE C   | info.c@test.com