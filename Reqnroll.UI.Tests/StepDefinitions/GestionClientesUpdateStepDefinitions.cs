using Reqnroll;
using FluentAssertions;
using Reqnroll.UI.Tests.Pages;
using Reqnroll.UI.Tests.Support;
using System;
using System.Threading;

namespace Reqnroll.UI.Tests.StepDefinitions
{
    [Binding]
    public class GestionClientesUpdateStepDefinitions
    {
        private readonly WebDriverContext _context;
        private readonly ListadoClientesPage _listadoPage;
        private readonly EditarClienteModal _editarModal;
        private readonly NuevoClientePage _nuevoClientePage;

        public GestionClientesUpdateStepDefinitions(WebDriverContext context)
        {
            _context = context;
            _listadoPage = new ListadoClientesPage(_context.Driver!);
            _editarModal = new EditarClienteModal(_context.Driver!);
            _nuevoClientePage = new NuevoClientePage(_context.Driver!);
        }

        // ... (Mantén los pasos Given y When anteriores igual) ...
        // ... Solo asegúrate de borrar el paso [Then] antiguo de este archivo y usar este nuevo:

        [Given(@"que existe un cliente registrado con Razón Social ""(.*)""")]
        public void DadoQueExisteUnClienteRegistrado(string nombreCliente)
        {
            _listadoPage.IrAListado();
            _listadoPage.BuscarCliente(nombreCliente);
            var datos = _listadoPage.ObtenerDatosDeFila(nombreCliente);
            
            if (datos == null)
            {
                Console.WriteLine($"[Setup] El cliente '{nombreCliente}' no existe. Creándolo...");
                _listadoPage.ClickNuevoCliente();
                _nuevoClientePage.IngresarRazonSocial(nombreCliente);
                _nuevoClientePage.IngresarNit("111222333");
                _nuevoClientePage.IngresarEmail("temp@setup.com");
                _nuevoClientePage.ClickGuardar();
                Thread.Sleep(1000); 
            }
        }

        [Given(@"navego a la página de listado de clientes")]
        public void DadoNavegoALaPaginaDeListado()
        {
            _listadoPage.IrAListado();
        }

        [When(@"busco el cliente ""(.*)""")]
        public void CuandoBuscoElCliente(string nombreCliente)
        {
            _listadoPage.BuscarCliente(nombreCliente);
        }

        [When(@"hago click en el boton editar del cliente encontrado")]
        public void CuandoHagoClickEnElBotonEditar()
        {
             // Usamos el nombre base conocido para encontrar la fila y dar click
             _listadoPage.ClickEditarEnFila("Cliente Base Edicion"); 
        }

        [When(@"actualizo la Razón Social a ""(.*)""")]
        public void CuandoActualizoLaRazonSocial(string nuevaRazon)
        {
            _editarModal.EsperarQueAparezca();
            _editarModal.EditarRazonSocial(nuevaRazon);
        }

        [When(@"actualizo el NIT a ""(.*)""")]
        public void CuandoActualizoElNIT(string nuevoNit)
        {
            _editarModal.EditarNit(nuevoNit);
        }

        [When(@"actualizo el Email a ""(.*)""")]
        public void CuandoActualizoElEmail(string nuevoEmail)
        {
            _editarModal.EditarEmail(nuevoEmail);
        }

        [When(@"guardo los cambios de la edicion")]
        public void CuandoGuardoLosCambios()
        {
            _editarModal.GuardarCambios();
            Thread.Sleep(1000); // Pequeña espera para refresco de tabla
        }

        // ESTE ES EL PASO CORREGIDO Y RENOMBRADO
        [Then(@"debería ver los datos actualizados: Razón Social ""(.*)"", NIT ""(.*)"" y Email ""(.*)""")]
        public void EntoncesDeberiaVerLosDatosActualizados(string razon, string nit, string email)
        {
            // 1. AQUÍ ESTÁ LA SOLUCIÓN A TU PROBLEMA DE BÚSQUEDA:
            // Al llamar a BuscarCliente con la NUEVA razón social, 
            // el PageObject hará .Clear() en el input y escribirá el nuevo valor.
            Console.WriteLine($"[Verificación] Buscando cliente editado: {razon}");
            _listadoPage.BuscarCliente(razon);
            
            // 2. Verificar datos
            var datos = _listadoPage.ObtenerDatosDeFila(razon);
            
            datos.Should().NotBeNull($"El cliente '{razon}' debería aparecer tras la edición.");
            datos!.Value.Razon.Should().Be(razon);
            datos!.Value.Nit.Should().Be(nit);
            datos!.Value.Email.Should().Be(email);
        }
    }
}