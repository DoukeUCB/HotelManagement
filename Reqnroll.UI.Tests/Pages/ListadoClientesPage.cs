using OpenQA.Selenium;
using Reqnroll.UI.Tests.Support;

namespace Reqnroll.UI.Tests.Pages
{
    public class ListadoClientesPage : BasePage
    {
        private readonly By _searchInput = By.CssSelector("input.input-busqueda");
        private readonly By _loadingState = By.CssSelector(".loading-state");
        private readonly By _tableRow = By.CssSelector(".table-row");
        private readonly By _btnNuevoCliente = By.CssSelector("button[routerLink='/nuevo-cliente']");

        // URL relativa
        private const string PageUrl = "http://localhost:4200/clientes"; 

        public ListadoClientesPage(IWebDriver driver) : base(driver)
        {
        }

        public void IrAListado()
        {
            NavigateTo(PageUrl);
            EsperarCargaCompleta();
        }

        public void ClickNuevoCliente()
        {
            Driver.FindElement(_btnNuevoCliente).Click();
        }

        public void BuscarCliente(string termino)
        {
            EsperarCargaCompleta();
            var input = Driver.FindElement(_searchInput);
            input.Clear();
            input.SendKeys(termino);
            
            // Esperar debounce de Angular y recarga de tabla
            Thread.Sleep(800); 
            EsperarCargaCompleta();
        }

        public void ClickEditarEnFila(string razonSocial)
        {
            // Busca la fila que contiene la razón social
            var fila = ObtenerFilaPorRazonSocial(razonSocial);
            
            if (fila == null)
                throw new NotFoundException($"No se encontró el cliente '{razonSocial}' para editar.");

            // Dentro de esa fila, busca el botón editar
            var btnEditar = fila.FindElement(By.CssSelector(".btn-editar"));
            btnEditar.Click();
        }

        public (string Razon, string Nit, string Email)? ObtenerDatosDeFila(string razonSocialBuscada)
        {
            var fila = ObtenerFilaPorRazonSocial(razonSocialBuscada);
            if (fila == null) return null;

            var celdaRazon = fila.FindElement(By.CssSelector("div[data-label='Razón Social']")).Text;
            var celdaNit = fila.FindElement(By.CssSelector("div[data-label='NIT']")).Text;
            var celdaEmail = fila.FindElement(By.CssSelector("div[data-label='Email']")).Text;

            return (celdaRazon, celdaNit, celdaEmail);
        }

        // Método auxiliar privado para reutilizar lógica de búsqueda de filas
        private IWebElement? ObtenerFilaPorRazonSocial(string razonSocial)
        {
            try 
            {
                // Esperar a que haya filas visibles
                Wait.Until(d => d.FindElements(_tableRow).Count > 0);
            }
            catch (WebDriverTimeoutException) { return null; }

            var filas = Driver.FindElements(_tableRow);
            foreach (var fila in filas)
            {
                var celdaRazon = fila.FindElement(By.CssSelector("div[data-label='Razón Social']")).Text;
                if (celdaRazon.Equals(razonSocial, StringComparison.OrdinalIgnoreCase))
                {
                    return fila;
                }
            }
            return null;
        }

        private void EsperarCargaCompleta()
        {
            try
            {
                // Esperar a que el loading desaparezca
                Wait.Until(d => d.FindElements(_loadingState).Count == 0 || !d.FindElement(_loadingState).Displayed);
            }
            catch (WebDriverTimeoutException) { }
        }
    }
}