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
    
    // 1. Asegurar estado limpio
    buscadorInput.Clear();
    buscadorInput.Click();
    Thread.Sleep(500);

    // 2. Escribir letra por letra para disparar el evento 'input' del frontend
    foreach (char c in razonSocial)
    {
        buscadorInput.SendKeys(c.ToString());
        Thread.Sleep(100); // Pequeña pausa entre letras
    }

    // 3. ESPERA CRÍTICA: Darle tiempo al Backend/Frontend para mostrar la lista
    Thread.Sleep(2500); 

    // 4. Buscar las sugerencias que aparecieron
    var suggestions = Driver.FindElements(_clienteSuggestions);
    
    // 5. Intentar encontrar la opción que contenga el texto (ignorando mayúsculas)
    var clienteOption = suggestions.FirstOrDefault(s => 
        s.Text.Trim().Contains(razonSocial, StringComparison.OrdinalIgnoreCase));
    
    if (clienteOption != null)
    {
        // Usar JavaScript para el click por si la lista es flotante/transparente
        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", clienteOption);
        Thread.Sleep(500);
    }
    else
    {
        // Debug: Si falla, dime qué es lo que Selenium llegó a ver en la lista
        var visibles = string.Join(" | ", suggestions.Select(s => s.Text));
        throw new Exception($"No se encontró '{razonSocial}' en el dropdown. Opciones vistas: [{visibles}]");
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
            // Esperar hasta que el botón esté habilitado y clickeable (Angular puede tardar en validar)
            var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(Driver, TimeSpan.FromSeconds(15));
            
            int attemptCount = 0;
            wait.Until(driver => {
                attemptCount++;
                try
                {
                    var btn = driver.FindElement(_continuarBtn);
                    var disabled = btn.GetAttribute("disabled");
                    var classList = btn.GetAttribute("class");
                    var displayed = btn.Displayed;
                    var enabled = btn.Enabled;
                    
                    if (attemptCount % 5 == 0)
                    {
                        Console.WriteLine($"[{attemptCount}] Button state: displayed={displayed}, enabled={enabled}, disabled={disabled}, class={classList}");
                    }
                    
                    return btn.Displayed && btn.Enabled && string.IsNullOrEmpty(disabled);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{attemptCount}] Error checking button: {ex.GetType().Name} - {ex.Message}");
                    return false;
                }
            });

            Console.WriteLine("✓ Botón Continuar habilitado, haciendo click...");
            var clickable = WaitForClickable(_continuarBtn);
            clickable.Click();
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
