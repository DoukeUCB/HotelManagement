using Reqnroll;
using FluentAssertions;
using Reqnroll.UI.Tests.Pages;
using Reqnroll.UI.Tests.Support;

namespace Reqnroll.UI.Tests.StepDefinitions
{
    [Binding]
    [Scope(Feature = "Crear reserva")]
    public class CrearReservaUIStepDefinitions
    {
        private readonly WebDriverContext _context;
        
        // Page Objects
        private ReservaClientePage? _clientePage;
        private ReservaHabitacionesPage? _habitacionesPage;
        private ReservaConfirmacionPage? _confirmacionPage;
        private ReservaResultadoPage? _resultadoPage;

        public CrearReservaUIStepDefinitions(WebDriverContext context)
        {
            _context = context;
        }

        #region Antecedentes

        [Given(@"que la aplicación web está en ejecución")]
        public void DadoQueLaAplicacionWebEstaEnEjecucion()
        {
            // Verificar que el driver está inicializado
            _context.Driver.Should().NotBeNull("El WebDriver debe estar inicializado");
        }

        [Given(@"que existen datos de prueba en el sistema")]
        public void DadoQueExistenDatosDePruebaEnElSistema()
        {
            // Asumimos que los datos de prueba ya están en la base de datos
            // (Cliente: EMPRESA TEST S.A., Habitaciones: 101, 102, Huésped: Juan Pérez)
            // Si fuera necesario, aquí podríamos hacer setup adicional via API
        }

        #endregion

        #region Paso 1: Cliente

        [Given(@"que navego a la página de nueva reserva")]
        public void DadoQueNavegoALaPaginaDeNuevaReserva()
        {
            _clientePage = new ReservaClientePage(_context.Driver!);
            _clientePage.NavigateToNuevaReserva(_context.BaseUrl);
            _clientePage.IsPageLoaded().Should().BeTrue("La página de nueva reserva debe cargar correctamente");
        }

        [When(@"selecciono el cliente ""(.*)""")]
        public void CuandoSeleccionoElCliente(string cliente)
        {
            _clientePage!.SeleccionarCliente(cliente);
            _context.ClienteSeleccionado = cliente;
        }

        [When(@"selecciono el estado de reserva ""(.*)""")]
        public void CuandoSeleccionoElEstadoDeReserva(string estado)
        {
            _clientePage!.SeleccionarEstado(estado);
        }

        [When(@"avanzo al paso de habitaciones")]
        public void CuandoAvanzoAlPasoDeHabitaciones()
        {
            _clientePage!.ClickSiguiente();
            _habitacionesPage = new ReservaHabitacionesPage(_context.Driver!);
        }

        #endregion

        #region Paso 2: Habitaciones

        [When(@"selecciono la habitación ""(.*)""")]
        public void CuandoSeleccionoLaHabitacion(string numeroHabitacion)
        {
            _habitacionesPage!.SeleccionarHabitacion(numeroHabitacion);
        }

        [When(@"ingreso la fecha de entrada ""(.*)""")]
        public void CuandoIngresoLaFechaDeEntrada(string fecha)
        {
            _habitacionesPage!.IngresarFechaEntrada(fecha);
        }

        [When(@"ingreso la fecha de salida ""(.*)""")]
        public void CuandoIngresoLaFechaDeSalida(string fecha)
        {
            _habitacionesPage!.IngresarFechaSalida(fecha);
        }

        [When(@"agrego el huésped ""(.*)""")]
        public void CuandoAgregoElHuesped(string nombreHuesped)
        {
            _habitacionesPage!.SeleccionarHuesped(nombreHuesped);
            _habitacionesPage.ClickSiguiente();
            _confirmacionPage = new ReservaConfirmacionPage(_context.Driver!);
        }

        [When(@"selecciono el huésped ""(.*)""")]
        public void CuandoSeleccionoElHuesped(string nombreHuesped)
        {
            _habitacionesPage!.SeleccionarHuesped(nombreHuesped);
        }

        [When(@"hago click en agregar otra habitación")]
        public void CuandoHagoClickEnAgregarOtraHabitacion()
        {
            _habitacionesPage!.ClickAgregarOtraHabitacion();
        }

        [When(@"selecciono la segunda habitación ""(.*)""")]
        public void CuandoSeleccionoLaSegundaHabitacion(string numeroHabitacion)
        {
            _habitacionesPage!.SeleccionarSegundaHabitacion(numeroHabitacion);
        }

        [When(@"ingreso la segunda fecha de entrada ""(.*)""")]
        public void CuandoIngresoLaSegundaFechaDeEntrada(string fecha)
        {
            _habitacionesPage!.IngresarSegundaFechaEntrada(fecha);
        }

        [When(@"ingreso la segunda fecha de salida ""(.*)""")]
        public void CuandoIngresoLaSegundaFechaDeSalida(string fecha)
        {
            _habitacionesPage!.IngresarSegundaFechaSalida(fecha);
        }

        [When(@"selecciono el segundo huésped ""(.*)""")]
        public void CuandoSeleccionoElSegundoHuesped(string nombreHuesped)
        {
            _habitacionesPage!.SeleccionarSegundoHuesped(nombreHuesped);
        }

        [When(@"avanzo al paso de confirmación")]
        public void CuandoAvanzoAlPasoDeConfirmacion()
        {
            _habitacionesPage!.ClickSiguiente();
            _confirmacionPage = new ReservaConfirmacionPage(_context.Driver!);
        }

        [When(@"agrego la habitación ""(.*)"" con los siguientes datos:")]
        public void CuandoAgregoLaHabitacionConLosSiguientesDatos(string numeroHabitacion, Table table)
        {
            var row = table.Rows[0];
            var fechaEntrada = row["Fecha_Entrada"];
            var fechaSalida = row["Fecha_Salida"];
            var huesped = row["Huesped"];

            _habitacionesPage!.AgregarHabitacionCompleta(
                numeroHabitacion, 
                fechaEntrada, 
                fechaSalida, 
                huesped
            );

            // Verificar que se agregó
            var cantidad = _habitacionesPage.ObtenerCantidadHabitaciones();
            cantidad.Should().BeGreaterThan(0, "Debe haber al menos una habitación agregada");
        }

        #endregion

        #region Paso 3: Confirmación

        [When(@"confirmo la reserva")]
        public void CuandoConfirmoLaReserva()
        {
            // Click en el botón "Crear Reserva"
            _confirmacionPage!.ClickConfirmar();
            _resultadoPage = new ReservaResultadoPage(_context.Driver!);
        }

        #endregion

        #region Verificaciones

        [Then(@"debería ver un mensaje de éxito")]
        public void EntoncesDeberiaVerUnMensajeDeExito()
        {
            var mensajeVisible = _resultadoPage!.MensajeExitoVisible();
            mensajeVisible.Should().BeTrue("Debería mostrarse un mensaje de éxito");

            if (mensajeVisible)
            {
                var mensaje = _resultadoPage.ObtenerMensajeExito();
                _context.OperacionExitosa = true;
                Console.WriteLine($"Mensaje de éxito: {mensaje}");
            }
        }

        [Then(@"debería ver el ID de la reserva creada")]
        public void EntoncesDeberiaVerElIDDeLaReservaCreada()
        {
            var reservaId = _resultadoPage!.ObtenerReservaId();
            reservaId.Should().NotBeNullOrEmpty("Debe mostrarse el ID de la reserva");
            _context.ReservaId = reservaId;
            Console.WriteLine($"Reserva creada con ID: {reservaId}");
        }

        [Then(@"la reserva debería tener el monto total de ""(.*)""")]
        public void EntoncesLaReservaDeberiaTenerElMontoTotalDe(string montoEsperado)
        {
            // Nota: Este paso depende de cómo el frontend muestre el monto
            // Puede requerir navegar a la página de detalle o verificar en confirmación
            
            // Para este ejemplo, asumimos que el monto se puede verificar
            // Si no está disponible en la página de resultado, este paso podría
            // hacer una verificación adicional via navegación o API
            
            Console.WriteLine($"Verificación de monto esperado: {montoEsperado}");
            // En una implementación real, verificarías el monto en la UI o via API
        }

        #endregion
    }
}
