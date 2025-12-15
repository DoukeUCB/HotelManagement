using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;
using Reqnroll.UI.Tests.Pages;
using Reqnroll.UI.Tests.Support;
using System;

namespace Reqnroll.UI.Tests.StepDefinitions
{
    [Binding]
    public class VisualizarClientesUIStepDefinitions
    {
        private readonly ListadoClientesPage _listadoPage;

        public VisualizarClientesUIStepDefinitions(WebDriverContext context)
        {
            // El signo de exclamación (!) le dice al compilador: "Confía en mí, Driver no es null aquí"
            // Esto elimina la advertencia CS8604
            _listadoPage = new ListadoClientesPage(context.Driver!);
        }

        [Given(@"que navego a la página de listado de clientes")]
        public void DadoQueNavegoALaPaginaDeListadoDeClientes()
        {
            _listadoPage.IrAListado();
        }

        [When(@"busco el cliente por Razón Social ""(.*)""")]
        public void CuandoBuscoElClientePorRazonSocial(string razonSocial)
        {
            _listadoPage.BuscarCliente(razonSocial);
        }

        [Then(@"debería ver en la grilla al cliente con Razón Social ""(.*)"", NIT ""(.*)"" y Email ""(.*)""")]
        public void EntoncesDeberiaVerEnLaGrillaAlClienteCon(string razonEsperada, string nitEsperado, string emailEsperado)
        {
            // Usamos la Razón Social esperada para buscar en la tabla, pero ignorando mayúsculas al buscar la fila
            var datosEncontrados = _listadoPage.ObtenerDatosDeFila(razonEsperada);

            Assert.IsNotNull(datosEncontrados, $"No se encontró ninguna fila que contenga '{razonEsperada}'");

            // Validaciones Robustas (Ignorando mayúsculas/minúsculas)
            // StringComparison.OrdinalIgnoreCase es la clave aquí
            
            bool razonesCoinciden = string.Equals(razonEsperada, datosEncontrados.Value.Razon, StringComparison.OrdinalIgnoreCase);
            Assert.IsTrue(razonesCoinciden, $"La Razón Social no coincide. Esperado: {razonEsperada}, Actual: {datosEncontrados.Value.Razon}");

            // El NIT suele ser números, pero lo tratamos igual por seguridad
            bool nitsCoinciden = string.Equals(nitEsperado, datosEncontrados.Value.Nit, StringComparison.OrdinalIgnoreCase);
            Assert.IsTrue(nitsCoinciden, $"El NIT no coincide. Esperado: {nitEsperado}, Actual: {datosEncontrados.Value.Nit}");

            // El Email también lo comparamos ignorando mayúsculas
            bool emailsCoinciden = string.Equals(emailEsperado, datosEncontrados.Value.Email, StringComparison.OrdinalIgnoreCase);
            Assert.IsTrue(emailsCoinciden, $"El Email no coincide. Esperado: {emailEsperado}, Actual: {datosEncontrados.Value.Email}");
        }
    }
}