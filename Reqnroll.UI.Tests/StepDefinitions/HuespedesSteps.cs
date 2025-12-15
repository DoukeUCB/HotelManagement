using OpenQA.Selenium;
using Reqnroll.UI.Tests.Pages;
using Reqnroll.UI.Tests.Support;
using FluentAssertions;

namespace Reqnroll.UI.Tests.StepDefinitions
{
    [Binding]
    [Scope(Feature = "Gestión de Huéspedes")] 
    public class HuespedesSteps
    {
        private readonly WebDriverContext _context;
        private HuespedesPage _page;

        public HuespedesSteps(WebDriverContext context)
        {
            _context = context;
            _page = new HuespedesPage(_context.Driver!);
        }

        #region Background / Setup

        [Given(@"que la aplicación web está en ejecución")]
        public void DadoQueLaAplicacionWebEstaEnEjecucion()
        {
            // Verificar que el driver está inicializado
            _context.Driver.Should().NotBeNull("El WebDriver debe estar inicializado por el Hook");
        }

        [Given(@"que existen datos de prueba en el sistema")]
        public void DadoQueExistenDatosDePruebaEnElSistema()
        {
            // Se asume que los datos de prueba ya existen en DB
        }

        #endregion

        #region Navegación

        [Given(@"navego a la página de huéspedes")]
        public void DadoQueNavegoALaPaginaDeHuespedes()
        {
            _page.GoTo();
            _page.IsPageLoaded().Should().BeTrue("La página de Huéspedes debe cargar correctamente");
        }

        #endregion

        #region Insert / Crear Huésped

        [When(@"ingreso el primer nombre ""(.*)""")]
        public void CuandoIngresoElPrimerNombre(string primerNombre)
        {
            _page.ClickNuevo();
            _page.EnterNombre(primerNombre);
        }

        [When(@"ingreso el segundo nombre ""(.*)""")]
        public void CuandoIngresoElSegundoNombre(string segundoNombre)
        {
            _page.EnterSegundoNombre(segundoNombre);
        }

        [When(@"ingreso el primer apellido ""(.*)""")]
        public void CuandoIngresoElPrimerApellido(string primerApellido)
        {
            _page.EnterApellido(primerApellido);
        }

        [When(@"ingreso el segundo apellido ""(.*)""")]
        public void CuandoIngresoElSegundoApellido(string segundoApellido)
        {
            _page.EnterSegundoApellido(segundoApellido);
        }

        [When(@"ingreso el documento ""(.*)""")]
        public void CuandoIngresoElDocumento(string documento)
        {
            _page.EnterNumeroDocumento(documento);
        }

        [When(@"ingreso el teléfono ""(.*)""")]
        public void CuandoIngresoElTelefono(string telefono)
        {
            _page.EnterTelefono(telefono);
        }

        [When(@"ingreso la fecha de nacimiento ""(.*)""")]
        public void CuandoIngresoLaFechaDeNacimiento(string fechaNac)
        {
            _page.EnterFechaNacimiento(fechaNac);
        }

        [When(@"hago click en guardar huésped")]
        public void CuandoHagoClickEnGuardarHuesped()
        {
            _page.ClickGuardar();
        }

        #endregion

        #region Validaciones

        [Then(@"debería ver un mensaje de éxito")]
        public void EntoncesDeberiaVerUnMensajeDeExito()
        {
            _page.TieneMensajeExito().Should().BeTrue("Debe aparecer mensaje de éxito al guardar");
        }

        [Then(@"debería ver el ID del huésped creado")]
        public void EntoncesDeberiaVerElIDDelHuespedCreado()
        {
            var id = _page.ObtenerIdDelHuesped(); // Necesitas implementarlo en POM
            id.Should().NotBeNullOrEmpty("El huésped creado debe tener un ID visible en la UI");
        }

        #endregion

        #region Update / Edit

        [When(@"busco el huésped con documento ""(.*)""")]
        public void CuandoBuscoElHuespedConDocumento(string documento)
        {
            _page.BuscarPorDocumento(documento);
        }

        [When(@"hago click en editar huésped")]
        public void CuandoHagoClickEnEditarHuesped()
        {
            _page.ClickEditarFirst();
        }

        [When(@"modifico el teléfono a ""(.*)""")]
        public void CuandoModificoElTelefonoA(string telefono)
        {
            _page.EnterTelefono(telefono);
        }

        [When(@"hago click en guardar cambios")]
        public void CuandoHagoClickEnGuardarCambios()
        {
            _page.ClickGuardar();
        }

        [Then(@"debería ver el teléfono actualizado para el huésped")]
        public void EntoncesDeberiaVerElTelefonoActualizadoParaElHuesped()
        {
            var tel = _page.ObtenerTelefonoPorDocumento(_page.ObtenerDocumentoDelHuesped()); // Implementar en POM
            tel.Should().NotBeNullOrEmpty("El teléfono debe mostrarse actualizado");
        }

        #endregion

        #region Delete / Eliminar

        [When(@"hago click en eliminar huésped")]
        public void CuandoHagoClickEnEliminarHuesped()
        {
            _page.ClickEliminarFirst();
        }

        [When(@"confirmo la eliminación")]
        public void CuandoConfirmoLaEliminacion()
        {
            _page.ConfirmarEliminar();
        }

        [Then(@"el huésped ya no debería aparecer en la lista")]
        public void EntoncesElHuespedYaNoDeberiaAparecerEnLaLista()
        {
            _page.NoExisteHuesped(_page.ObtenerDocumentoDelHuesped()).Should().BeTrue("El huésped debe ser eliminado de la lista");
        }

        #endregion
        #region Select / Seleccionar Huésped

        [When(@"selecciono el huésped con documento ""(.*)""")]
        public void CuandoSeleccionoElHuespedPorDocumento(string documento)
        {
            _page.SeleccionarHuespedPorDocumento(documento); // Este método lo implementas en HuespedesPage
        }

        [Then(@"debería ver el huésped seleccionado en la lista")]
        public void EntoncesDeberiaVerElHuespedSeleccionado()
        {
            var documento = _page.ObtenerDocumentoDelHuespedSeleccionado();
            documento.Should().NotBeNullOrEmpty("Debe haber un huésped seleccionado en la lista");
        }

        #endregion

    }
    
}
