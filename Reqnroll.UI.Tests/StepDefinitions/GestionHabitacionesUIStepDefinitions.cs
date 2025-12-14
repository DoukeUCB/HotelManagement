using Reqnroll;
using FluentAssertions;
using Reqnroll.UI.Tests.Pages;
using Reqnroll.UI.Tests.Support;

namespace Reqnroll.UI.Tests.StepDefinitions
{
    [Binding]
    public class GestionHabitacionesUIStepDefinitions
    {
        private readonly WebDriverContext _context;
        private readonly HabitacionesPage _habitacionesPage;

        public GestionHabitacionesUIStepDefinitions(WebDriverContext context)
        {
            _context = context;
            _habitacionesPage = new HabitacionesPage(_context.Driver!);
        }

        [Given(@"que navego a la página de listado de habitaciones")]
        public void DadoQueNavegoALaPaginaDeListadoDeHabitaciones()
        {
            _habitacionesPage.NavegarAListado(_context.BaseUrl);
        }

        [When(@"hago click en el botón ""(.*)""")]
        public void CuandoHagoClickEnElBoton(string nombreBoton)
        {
            _habitacionesPage.ClickNuevaHabitacion();
        }

        [When(@"ingreso el número de habitación ""(.*)""")]
        public void CuandoIngresoElNumeroDeHabitacion(string numero)
        {
            _context.HabitacionNumero = numero;
            _habitacionesPage.IngresarNumero(numero);
        }

        [When(@"ingreso el piso ""(.*)""")]
        public void CuandoIngresoElPiso(string piso)
        {
            _habitacionesPage.IngresarPiso(piso);
        }

        [When(@"selecciono el tipo de habitación ""(.*)""")]
        public void CuandoSeleccionoElTipoDeHabitacion(string tipo)
        {
            _habitacionesPage.SeleccionarTipo(tipo);
        }

        [When(@"selecciono el estado ""(.*)""")]
        public void CuandoSeleccionoElEstado(string estado)
        {
            _habitacionesPage.SeleccionarEstado(estado);
        }

        [When(@"guardo la habitación")]
        public void CuandoGuardoLaHabitacion()
        {
            _habitacionesPage.ClickGuardar();
        }

        [Then(@"debería ver la habitación ""(.*)"" en la lista")]
        public void EntoncesDeberiaVerLaHabitacionEnLaLista(string numero)
        {
            _habitacionesPage.ExisteHabitacionEnLista(numero).Should().BeTrue();
        }
    }
}