using OpenQA.Selenium;

namespace Reqnroll.UI.Tests.Pages
{
    /// <summary>
    /// Page Object para la página de selección de cliente (Paso 1)
    /// </summary>
    public class ReservaClientePage : BasePage
    {
        // Locators
        private readonly By _clienteBuscadorInput = By.CssSelector("input.cliente-search[type='search']");
        private readonly By _clienteSuggestions = By.CssSelector("ul.suggestions li.select-like__option");
        private readonly By _estadoDropdown = By.CssSelector("select[formcontrolname='estadoReserva']");
        private readonly By _continuarBtn = By.XPath("//button[contains(text(), 'Continuar')]");
        private readonly By _pageTitle = By.CssSelector(".page-header h1");

        public ReservaClientePage(IWebDriver driver) : base(driver) { }

        /// <summary>
        /// Navegar a la página de nueva reserva
        /// </summary>
        public void NavigateToNuevaReserva(string baseUrl)
        {
            NavigateTo($"{baseUrl}/nueva-reserva");
            WaitForLoadingToDisappear();
            // Esperar a que cargue el formulario
            WaitForElement(_clienteBuscadorInput);
        }

        /// <summary>
        /// Seleccionar cliente por razón social - hacer click y seleccionar de lista desplegada
        /// </summary>
        public void SeleccionarCliente(string razonSocial)
        {
            var buscadorInput = WaitForElement(_clienteBuscadorInput);
            buscadorInput.Click();
            Thread.Sleep(800);
            
            var suggestions = Driver.FindElements(_clienteSuggestions);
            var clienteOption = suggestions.FirstOrDefault(s => s.Text.Contains(razonSocial, StringComparison.OrdinalIgnoreCase));
            
            if (clienteOption != null)
            {
                clienteOption.Click();
            }
            else
            {
                throw new Exception($"No se encontró el cliente '{razonSocial}'");
            }
        }

        /// <summary>
        /// Seleccionar estado de reserva
        /// </summary>
        public void SeleccionarEstado(string estado)
        {
            var dropdown = WaitForElement(_estadoDropdown);
            var select = new OpenQA.Selenium.Support.UI.SelectElement(dropdown);
            select.SelectByText(estado);
        }

        /// <summary>
        /// Click en botón Continuar para avanzar al siguiente paso
        /// </summary>
        public void ClickSiguiente()
        {
            WaitForClickable(_continuarBtn).Click();
            Thread.Sleep(500);
        }

        /// <summary>
        /// Verificar que estamos en la página correcta
        /// </summary>
        public bool IsPageLoaded()
        {
            try
            {
                var title = GetElementText(_pageTitle);
                return title.Contains("Cliente") || title.Contains("Reserva");
            }
            catch
            {
                return false;
            }
        }
    }
}
