using Reqnroll;
using FluentAssertions;
using Reqnroll.UI.Tests.Pages;
using Reqnroll.UI.Tests.Support;
using OpenQA.Selenium;
using System; // Necesario para Console.WriteLine

namespace Reqnroll.UI.Tests.StepDefinitions
{
    [Binding]
    public class GestionClientesStepDefinitions
    {
        private readonly WebDriverContext _context;
        private NuevoClientePage? _nuevoClientePage;

        public GestionClientesStepDefinitions(WebDriverContext context)
        {
            _context = context;
        }

        [Given(@"que estoy en la página de creación de clientes")]
        public void DadoQueEstoyEnLaPaginaDeCreacionDeClientes()
        {
            _nuevoClientePage = new NuevoClientePage(_context.Driver!);
            _nuevoClientePage.NavegarAlFormulario(_context.BaseUrl);
        }

        [When(@"ingreso la Razón Social ""(.*)""")]
        public void CuandoIngresoLaRazonSocial(string razonSocial)
        {
            Console.WriteLine($"[Step] Ingresando Razón Social: {razonSocial}");
            _nuevoClientePage!.IngresarRazonSocial(razonSocial);
        }

        [When(@"ingreso el NIT ""(.*)""")]
        public void CuandoIngresoElNIT(string nit)
        {
            // Se eliminó la lógica de Random. Se usa el valor exacto del Feature.
            Console.WriteLine($"[Step] Ingresando NIT: {nit}");
            _nuevoClientePage!.IngresarNit(nit);
        }

        [When(@"ingreso el Email ""(.*)""")]
        public void CuandoIngresoElEmail(string email)
        {
            // Se eliminó la lógica de Random y split. Se usa el valor exacto del Feature.
            // Esto garantiza que si el Feature dice 30 caracteres, se envíen exactamente 30.
            Console.WriteLine($"[Step] Ingresando Email: {email}");
            _nuevoClientePage!.IngresarEmail(email);
        }

        [When(@"hago click en guardar cliente")]
        public void CuandoHagoClickEnGuardarCliente()
        {
            _nuevoClientePage!.ClickGuardar();
        }

        [Then(@"debería ver un mensaje de éxito indicando que se guardó correctamente")]
        public void EntoncesDeberiaVerUnMensajeDeExito()
        {
            bool exito = _nuevoClientePage!.EsMensajeExitoVisible();
            
            if (!exito)
            {
                Console.WriteLine("DEBUG: No se encontró mensaje de éxito.");
            }

            exito.Should().BeTrue("El sistema debería mostrar un mensaje de éxito tras guardar un cliente válido.");
            
            if(exito)
            {
                string mensaje = _nuevoClientePage.ObtenerMensajeExito();
                Console.WriteLine($"[Assert] Mensaje recibido: {mensaje}");
                mensaje.Should().NotBeNullOrEmpty();
            }
        }
    }
}