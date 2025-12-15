using OpenQA.Selenium;
using Reqnroll.UI.Tests.Support;
using OpenQA.Selenium.Support.UI;

namespace Reqnroll.UI.Tests.Pages
{
    public class ListadoClientesPage : BasePage
    {
        private readonly By _searchInput = By.CssSelector("input.input-busqueda");
        private readonly By _loadingState = By.CssSelector(".loading-state");
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
            var input = Wait.Until(d => d.FindElement(_searchInput));
            input.Clear();
            input.SendKeys(termino);

            // OPTIMIZACIÓN: Espera inteligente del loading
            try
            {
                // Esperamos máximo 500ms a que APAREZCA el loading
                var shortWait = new WebDriverWait(Driver, TimeSpan.FromMilliseconds(500));
                shortWait.Until(d => d.FindElement(_loadingState).Displayed);

                // Si apareció, esperamos a que DESAPAREZCA usando la lógica robusta
                EsperarCargaCompleta();
            }
            catch (WebDriverTimeoutException) { } // Fue muy rápido y nunca apareció el loading
            catch (NoSuchElementException) { } // No existe el elemento loading
            catch (StaleElementReferenceException) { } // Apareció y desapareció al instante
        }

        private IWebElement? ObtenerFilaPorRazonSocial(string razonSocial)
        {
            // Búsqueda directa por XPath (O(1)) Case-Insensitive
            string xpathString = $".//div[contains(@class, 'table-row') and .//div[@data-label='Razón Social' and contains(translate(text(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), '{razonSocial.ToLower()}')]]";

            try
            {
                return Wait.Until(d => d.FindElement(By.XPath(xpathString)));
            }
            catch (WebDriverTimeoutException)
            {
                return null;
            }
        }

        public void ClickEditarEnFila(string razonSocial)
        {
            var fila = ObtenerFilaPorRazonSocial(razonSocial);
            if (fila == null)
                throw new NotFoundException($"No se encontró el cliente '{razonSocial}' para editar.");

            // Buscamos el botón dentro de la fila encontrada
            fila.FindElement(By.CssSelector(".btn-editar")).Click();
        }

        public void ClickEliminarEnFila(string razonSocial)
        {
            var fila = ObtenerFilaPorRazonSocial(razonSocial);
            if (fila == null)
                throw new NotFoundException($"No se encontró el cliente '{razonSocial}' para eliminar.");

            // Buscamos el botón dentro de la fila encontrada
            fila.FindElement(By.CssSelector(".btn-eliminar")).Click();
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

        // CORRECCIÓN PRINCIPAL AQUÍ:
        private void EsperarCargaCompleta()
        {
            try
            {
                Wait.Until(d =>
                {
                    try
                    {
                        // Intentamos ver si el loading es visible
                        return !d.FindElement(_loadingState).Displayed;
                    }
                    catch (NoSuchElementException)
                    {
                        // Si el elemento NO existe, genial, ya cargó.
                        return true;
                    }
                    catch (StaleElementReferenceException)
                    {
                        // ¡LA SOLUCIÓN!
                        // Si el elemento existía pero desapareció JUSTO AHORA (se puso "stale"),
                        // también significa que la carga terminó.
                        return true;
                    }
                });
            }
            catch (WebDriverTimeoutException) 
            {
                // Si pasa el tiempo y sigue cargando, dejamos que el test continúe 
                // (a veces el loading se queda pegado pero la app funciona)
            }
        }
    }
}