using OpenQA.Selenium;
using Reqnroll.UI.Tests.Support; // Asegúrate de que este namespace coincida con tu WebDriverContext

namespace Reqnroll.UI.Tests.Pages
{
    public class ListadoClientesPage : BasePage
    {
        // Selectores basados en tu HTML (clientes-list.component.html)
        private readonly By _searchInput = By.CssSelector("input.input-busqueda");
        private readonly By _loadingState = By.CssSelector(".loading-state");
        private readonly By _tableRow = By.CssSelector(".table-row");
        private readonly By _emptyState = By.CssSelector(".empty-state");

        // URL relativa (ajusta si tu puerto o ruta base es diferente)
        private const string PageUrl = "http://localhost:4200/clientes"; 

        public ListadoClientesPage(IWebDriver driver) : base(driver)
        {
        }

        public void IrAListado()
        {
            NavigateTo(PageUrl);
            EsperarCargaCompleta();
        }

        public void BuscarCliente(string termino)
        {
            EsperarCargaCompleta();
            TypeText(_searchInput, termino);
            
            // Pequeña espera para que Angular filtre la lista (debounce)
            Thread.Sleep(500); 
            EsperarCargaCompleta();
        }

        /// <summary>
        /// Busca una fila que contenga la Razón Social exacta y devuelve sus datos
        /// </summary>
        public (string Razon, string Nit, string Email)? ObtenerDatosDeFila(string razonSocialBuscada)
        {
            // Esperar a que haya filas o el mensaje de vacío
            try 
            {
                Wait.Until(d => d.FindElements(_tableRow).Count > 0 || d.FindElements(_emptyState).Count > 0);
            }
            catch (WebDriverTimeoutException) { /* Puede que no haya resultados */ }

            var filas = Driver.FindElements(_tableRow);

            foreach (var fila in filas)
            {
                // Buscamos dentro de la fila los divs con data-label
                // Usamos CssSelector relativo al elemento fila
                var celdaRazon = fila.FindElement(By.CssSelector("div[data-label='Razón Social']")).Text;

                if (celdaRazon.Equals(razonSocialBuscada, StringComparison.OrdinalIgnoreCase))
                {
                    var celdaNit = fila.FindElement(By.CssSelector("div[data-label='NIT']")).Text;
                    var celdaEmail = fila.FindElement(By.CssSelector("div[data-label='Email']")).Text;

                    return (celdaRazon, celdaNit, celdaEmail);
                }
            }

            return null; // No se encontró
        }

        private void EsperarCargaCompleta()
        {
            // Tu HTML usa la clase 'loading-state'
            try
            {
                Wait.Until(d => !d.FindElement(_loadingState).Displayed);
            }
            catch (NoSuchElementException)
            {
                // Si no encuentra el elemento loading, es que ya cargó
            }
            catch (WebDriverTimeoutException)
            {
                // Si tarda mucho, continuamos (a veces desaparece muy rápido)
            }
        }
    }
}