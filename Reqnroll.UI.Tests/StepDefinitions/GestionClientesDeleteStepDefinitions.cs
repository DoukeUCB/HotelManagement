using Reqnroll;
using FluentAssertions;
using Reqnroll.UI.Tests.Pages;
using Reqnroll.UI.Tests.Support;
using System;
using System.Threading;

namespace Reqnroll.UI.Tests.StepDefinitions
{
    [Binding]
    public class GestionClientesDeleteStepDefinitions
    {
        private readonly WebDriverContext _context;
        private readonly ListadoClientesPage _listadoPage;
        private readonly EliminarClienteModal _eliminarModal;
        private readonly NuevoClientePage _nuevoClientePage;
        private readonly ScenarioContext _scenarioContext;

        public GestionClientesDeleteStepDefinitions(WebDriverContext context, ScenarioContext scenarioContext)
        {
            _context = context;
            _listadoPage = new ListadoClientesPage(_context.Driver!);
            _eliminarModal = new EliminarClienteModal(_context.Driver!);
            _nuevoClientePage = new NuevoClientePage(_context.Driver!);
            _scenarioContext = scenarioContext;
        }

        [Given(@"que existe un cliente registrado para eliminar con Razón Social ""(.*)""")]
        public void DadoQueExisteUnClienteParaEliminar(string nombreCliente)
        {
            _listadoPage.IrAListado();
            _listadoPage.BuscarCliente(nombreCliente);
            var datos = _listadoPage.ObtenerDatosDeFila(nombreCliente);

            // Si NO existe, lo creamos primero (Patrón Lazy Setup)
            if (datos == null)
            {
                Console.WriteLine($"[Setup Delete] El cliente '{nombreCliente}' no existía. Creándolo...");
                
                // Generar datos aleatorios para que no choque con validaciones de unicidad
                string nitRandom = DateTime.Now.Ticks.ToString().Substring(12) + new Random().Next(100, 999);
                
                _listadoPage.ClickNuevoCliente();
                _nuevoClientePage.IngresarRazonSocial(nombreCliente);
                _nuevoClientePage.IngresarNit(nitRandom);
                _nuevoClientePage.IngresarEmail($"borrar.{nitRandom}@test.com");
                _nuevoClientePage.ClickGuardar();
                
                // Volvemos al listado para asegurar que estamos en la posición correcta
                _listadoPage.IrAListado();
            }
        }

        [When(@"hago click en el boton eliminar del cliente encontrado")]
        public void CuandoHagoClickEnElBotonEliminar()
        {
            // Nota: Asumimos que el paso anterior "Cuando busco el cliente..." ya filtró la tabla
            // Usamos el nombre que sabemos que está en pantalla. 
            // Podríamos pasar el nombre por parámetro, pero usaremos una constante o contexto si es necesario.
            // Para simplificar, asumimos que buscamos "Cliente A Borrar" en el paso previo.
            
            // Truco: Para no hardcodear el string aquí, buscamos la primera fila visible,
            // ya que la búsqueda debió dejar solo 1 resultado.
            
            // O, más explícito (recomendado):
            var nombreCliente = _scenarioContext.ContainsKey("UltimoClienteBuscado")
                ? _scenarioContext.Get<string>("UltimoClienteBuscado")
                : "Cliente A Borrar";

            _listadoPage.ClickEliminarEnFila(nombreCliente);
        }

        [When(@"confirmo la eliminación en la ventana modal")]
        public void CuandoConfirmoLaEliminacion()
        {
            _eliminarModal.EsperarQueAparezca();
            _eliminarModal.ConfirmarEliminacion();
            
            // Pequeña pausa técnica para dar tiempo a la tabla de Angular a refrescarse (eliminar la fila del DOM)
            Thread.Sleep(500); 
        }

        [Then(@"el cliente ""(.*)"" ya no debería aparecer en el listado")]
        public void EntoncesElClienteYaNoDeberiaAparecer(string nombreCliente)
        {
            // 1. Buscamos de nuevo
            _listadoPage.BuscarCliente(nombreCliente);

            // 2. Intentamos obtener datos
            var datos = _listadoPage.ObtenerDatosDeFila(nombreCliente);

            // 3. Aserción: Debe ser null
            datos.Should().BeNull($"el cliente '{nombreCliente}' debería haber sido eliminado del sistema");
            
            // Opcional: Verificar mensaje de "No hay clientes" si la tabla quedó vacía
            // var emptyState = _context.Driver.FindElements(By.CssSelector(".empty-state"));
            // if(emptyState.Count > 0) emptyState[0].Displayed.Should().BeTrue();
        }
    }
}