Feature: Huéspedes UI
  Como recepcionista
  Quiero gestionar huéspedes a través de la interfaz web
  Para mantener el catálogo de huéspedes

  Background:
    Dado que la aplicación web está en ejecución
    Y que existen datos de prueba en el sistema

  @ui @huesped @insert @pairwise
  Scenario Outline: Crear huésped (Insert) - datos pairwise
    Dado que navego a la página de huéspedes
    Cuando ingreso el primer nombre "<PrimerNombre>"
    Y ingreso el segundo nombre "<SegundoNombre>"
    Y ingreso el primer apellido "<PrimerApellido>"
    Y ingreso el segundo apellido "<SegundoApellido>"
    Y ingreso el documento "<Documento>"
    Y ingreso el teléfono "<Telefono>"
    Y ingreso la fecha de nacimiento "<FechaNac>"
    Y hago click en guardar huésped
    Entonces debería ver un mensaje de éxito
    Y debería ver el ID del huésped creado

    Examples:
      | TC_ID | PrimerNombre               | SegundoNombre | PrimerApellido | SegundoApellido | Documento    | Telefono   | FechaNac    |
      | HU-P1 | Jorge                      |               | Quispe         | Flores          | CI200001     | 70011111   | 1985-03-10  |
      | HU-P2 | A                          | Luis          | Quispe         |                 | P1234567     |            | 1992-07-25  |
      | HU-P3 | NombreMuyLargoExceso       |               | Quispe         | García          | CI0          | 123        | 2000-09-09  |
      | HU-P4 | Jorge                      | Luis          | Quispe         |                 | P1234567     | 70022222   | 1900-01-01  |
      | HU-P5 | A                          |               | Quispe         | Salazar         | CI200003     | 70033333   | 1998-11-02  |
      | HU-P6 | NombreMuyLargoExceso       | Luis          | Quispe         | Flores          | CI200005     | 70055555   | 1980-01-15  |

  @ui @huesped @update @pairwise
  Scenario Outline: Actualizar huésped (Update) - datos pairwise
    Dado que navego a la página de huéspedes
    Y busco el huésped con documento "<NumDocBusqueda>"
    Y hago click en editar huésped
    Y modifico el teléfono a "<NuevoTelefono>"
    Y modifico el correo a "<NuevoEmail>"
    Y hago click en guardar cambios
    Entonces debería ver un mensaje de éxito
    Y debería ver el teléfono actualizado para el huésped

    Examples:
      | NumDocBusqueda | NuevoTelefono | NuevoEmail           |
      | CI200001       | 999888777     | nuevo@example.com    |
      | P1234567       |               | invalid@             |
      | CI0            | 1234567890    | contacto@dominio.com |
      | CI200003       | 70033333      | nuevo@example.com    |

  @ui @huesped @delete @happy-path
  Scenario: Eliminar huésped existente (Happy Path)
    Dado que navego a la página de huéspedes
    Cuando busco el huésped con documento "CI200001"
    Y hago click en eliminar huésped
    Y confirmo la eliminación
    Entonces debería ver un mensaje de confirmación de eliminación
    Y el huésped ya no debería aparecer en la lista

  @ui @huesped @select @happy-path
  Scenario: Buscar/Seleccionar huésped (Happy Path)
    Dado que navego a la página de huéspedes
    Cuando busco el huésped con documento "CI200001"
    Entonces debería ver el huésped en la lista
