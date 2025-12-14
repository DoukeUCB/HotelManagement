# language: es
@ui @habitaciones
Característica: Gestión de Habitaciones UI
  Como administrador del hotel
  Quiero gestionar las habitaciones a través de la interfaz web
  Para mantener el inventario de habitaciones actualizado

  Antecedentes:
    Dado que la aplicación web está en ejecución
    Y que navego a la página de listado de habitaciones

  @happy-path @creacion
  Escenario: Crear una nueva habitación exitosamente
    Cuando hago click en el botón "Nueva Habitación"
    Y ingreso el número de habitación "401"
    Y ingreso el piso "4"
    Y selecciono el tipo de habitación "Simple"
    Y selecciono el estado "Disponible"
    Y guardo la habitación
    Entonces debería ver la habitación "401" en la lista