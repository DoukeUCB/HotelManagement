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
        public void DadoQueNavegoALaPaginaDeListadoDeHabitaciones() => _habitacionesPage.NavegarAListado(_context.BaseUrl);

        [When(@"hago click en el botón ""Nueva Habitación""")]
        public void CuandoClickNuevo() => _habitacionesPage.ClickNuevaHabitacion();

        [When(@"ingreso el número de habitación ""(.*)""")]
        public void CuandoIngresoNumero(string num) => _habitacionesPage.IngresarNumero(num);

        [When(@"ingreso el piso ""(.*)""")]
        public void CuandoIngresoPiso(string piso) => _habitacionesPage.IngresarPiso(piso);

        [When(@"selecciono el tipo de habitación ""(.*)""")]
        public void CuandoSeleccionoTipo(string tipo) => _habitacionesPage.SeleccionarTipo(tipo);

        [When(@"selecciono el estado ""(.*)""")]
        public void CuandoSeleccionoEstado(string estado) => _habitacionesPage.SeleccionarEstado(estado);

        [When(@"guardo la habitación")]
        [When(@"guardo los cambios")]
        public void CuandoGuardo() => _habitacionesPage.ClickGuardar();

        [Then(@"debería ver la habitación ""(.*)"" en la lista")]
        public void EntoncesDeberiaVerla(string num) => _habitacionesPage.ExisteEnLista(num).Should().BeTrue();

        [Then(@"el mensaje de éxito debería ser visible")]
        public void EntoncesMensajeExito() => _habitacionesPage.IsMensajeExitoVisible().Should().BeTrue();

        [Given(@"que busco la habitación ""(.*)"" en la lista")]
        public void DadoQueBuscoLaHabitacion(string numero) => _habitacionesPage.BuscarHabitacion(numero);

        [When(@"hago click en el botón ""Editar"" de la habitación ""(.*)""")]
        public void CuandoClickEditarHabitacion(string numero) => _habitacionesPage.ClickEditarHabitacion(numero);

        [When(@"cambio el estado a ""(.*)""")]
        public void CuandoCambioElEstado(string estado) => _habitacionesPage.CambiarEstadoEnModal(estado);

        [Then(@"el estado de la habitación ""(.*)"" debería ser ""(.*)""")]
        public void EntoncesElEstadoDeberiaSerb(string numero, string estado) => _habitacionesPage.VerificarEstadoHabitacion(numero, estado).Should().BeTrue();
    }
}