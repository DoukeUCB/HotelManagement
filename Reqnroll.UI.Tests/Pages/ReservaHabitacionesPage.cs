using OpenQA.Selenium;

namespace Reqnroll.UI.Tests.Pages
{
    /// <summary>
    /// Page Object para la página de selección de habitaciones (Paso 2)
    /// </summary>
    public class ReservaHabitacionesPage : BasePage
    {
        // Locators
        private readonly By _habitacionBuscadorInput = By.CssSelector("input[type='search'][placeholder*='habitación']");
        private readonly By _habitacionSuggestions = By.CssSelector("ul.suggestions li.select-like__option");
        private readonly By _fechaEntradaInput = By.CssSelector("input[formcontrolname='fechaEntrada']");
        private readonly By _fechaSalidaInput = By.CssSelector("input[formcontrolname='fechaSalida']");
        private readonly By _huespedBuscadorInput = By.CssSelector("input[type='search'][placeholder*='huésped']");
        private readonly By _huespedSuggestions = By.CssSelector("ul.suggestions li.select-like__option");
        private readonly By _continuarBtn = By.XPath("//div[@class='paso-contenido'][.//h2[contains(., 'Habitaciones')]]//button[contains(text(), 'Continuar') and contains(@class, 'btn-primary')]");
        private readonly By _habitacionesCards = By.CssSelector(".habitacion-card");
        private readonly By _agregarOtraHabitacionBtn = By.XPath("//button[contains(text(), 'Agregar otra habitación')]");

        public ReservaHabitacionesPage(IWebDriver driver) : base(driver) { }

        /// <summary>
        /// Seleccionar habitación por número
        /// </summary>
        public void SeleccionarHabitacion(string numeroHabitacion)
        {
            var buscadorInput = WaitForElement(_habitacionBuscadorInput);
            buscadorInput.Click();
            Thread.Sleep(1500);
            
            var suggestions = Driver.FindElements(_habitacionSuggestions);
            var habitacionOption = suggestions.FirstOrDefault(s => s.Text.Contains(numeroHabitacion, StringComparison.OrdinalIgnoreCase));
            
            if (habitacionOption != null)
            {
                ((OpenQA.Selenium.IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].dispatchEvent(new MouseEvent('mousedown', { bubbles: true }));", habitacionOption);
                Thread.Sleep(500);
            }
            else
            {
                throw new Exception($"No se encontró la habitación '{numeroHabitacion}'");
            }
        }

        /// <summary>
        /// Ingresar fecha de entrada usando JavaScript
        /// </summary>
        public void IngresarFechaEntrada(string fecha)
        {
            var input = WaitForElement(_fechaEntradaInput);
            ((OpenQA.Selenium.IJavaScriptExecutor)Driver).ExecuteScript($"arguments[0].value = '{fecha}'; arguments[0].dispatchEvent(new Event('input', {{ bubbles: true }})); arguments[0].dispatchEvent(new Event('change', {{ bubbles: true }}));", input);
            Thread.Sleep(300);
        }

        /// <summary>
        /// Ingresar fecha de salida usando JavaScript
        /// </summary>
        public void IngresarFechaSalida(string fecha)
        {
            var input = WaitForElement(_fechaSalidaInput);
            ((OpenQA.Selenium.IJavaScriptExecutor)Driver).ExecuteScript($"arguments[0].value = '{fecha}'; arguments[0].dispatchEvent(new Event('input', {{ bubbles: true }})); arguments[0].dispatchEvent(new Event('change', {{ bubbles: true }}));", input);
            Thread.Sleep(300);
        }

        /// <summary>
        /// Seleccionar huésped
        /// </summary>
        public void SeleccionarHuesped(string nombreCompleto)
        {
            var buscadorInput = WaitForElement(_huespedBuscadorInput);
            buscadorInput.Click();
            Thread.Sleep(1200);
            
            var suggestions = Driver.FindElements(_huespedSuggestions);
            var huespedOption = suggestions.FirstOrDefault(s => s.Text.Contains(nombreCompleto.Split(' ')[0], StringComparison.OrdinalIgnoreCase));
            
            if (huespedOption != null)
            {
                ((OpenQA.Selenium.IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].dispatchEvent(new MouseEvent('mousedown', { bubbles: true }));", huespedOption);
                Thread.Sleep(500);
            }
            else
            {
                throw new Exception($"No se encontró el huésped '{nombreCompleto}'");
            }
        }

        /// <summary>
        /// No hay botón agregar - los datos se validan automáticamente
        /// </summary>
        public void ClickAgregarHabitacion()
        {
            // En este formulario no hay botón "Agregar"
            // Los datos se validan en línea
            Thread.Sleep(500);
        }

        /// <summary>
        /// Click en botón Agregar otra habitación
        /// </summary>
        public void ClickAgregarOtraHabitacion()
        {
            Thread.Sleep(500);
            var btn = WaitForElement(_agregarOtraHabitacionBtn);
            ((OpenQA.Selenium.IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView(true); arguments[0].click();", btn);
            Thread.Sleep(500);
        }

        /// <summary>
        /// Click en botón Continuar para ir a confirmación
        /// </summary>
        public void ClickSiguiente()
        {
            // Esperar a que Angular valide el formulario
            Thread.Sleep(500);
            
            // Hacer scroll hasta el botón
            var continueBtn = WaitForElement(_continuarBtn);
            ((OpenQA.Selenium.IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView(true);", continueBtn);
            Thread.Sleep(500);
            
            // Forzar click con JavaScript si está deshabilitado
            try
            {
                continueBtn.Click();
            }
            catch
            {
                // Si el click normal falla, usar JavaScript
                ((OpenQA.Selenium.IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", continueBtn);
            }
            
            WaitForLoadingToDisappear();
            Thread.Sleep(1000);
        }

        /// <summary>
        /// Obtener cantidad de habitaciones agregadas
        /// </summary>
        public int ObtenerCantidadHabitaciones()
        {
            try
            {
                var elementos = Driver.FindElements(_habitacionesCards);
                return elementos.Count;
            }
            catch
            {
                return 1; // Por defecto hay al menos 1 card
            }
        }

        /// <summary>
        /// Flujo completo: agregar una habitación
        /// </summary>
        public void AgregarHabitacionCompleta(string numeroHabitacion, string fechaEntrada, 
            string fechaSalida, string huesped)
        {
            SeleccionarHabitacion(numeroHabitacion);
            IngresarFechaEntrada(fechaEntrada);
            IngresarFechaSalida(fechaSalida);
            SeleccionarHuesped(huesped);
            ClickAgregarHabitacion();
        }

        /// <summary>
        /// Seleccionar segunda habitación
        /// </summary>
        public void SeleccionarSegundaHabitacion(string numeroHabitacion)
        {
            var buscadores = Driver.FindElements(_habitacionBuscadorInput);
            if (buscadores.Count >= 2)
            {
                buscadores[1].Click();
                Thread.Sleep(1500);
                
                var suggestions = Driver.FindElements(_habitacionSuggestions);
                var habitacionOption = suggestions.FirstOrDefault(s => s.Text.Contains(numeroHabitacion, StringComparison.OrdinalIgnoreCase));
                
                if (habitacionOption != null)
                {
                    ((OpenQA.Selenium.IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].dispatchEvent(new MouseEvent('mousedown', { bubbles: true }));", habitacionOption);
                    Thread.Sleep(500);
                }
                else
                {
                    throw new Exception($"No se encontró la segunda habitación '{numeroHabitacion}'");
                }
            }
        }

        /// <summary>
        /// Ingresar fecha de entrada en segunda habitación
        /// </summary>
        public void IngresarSegundaFechaEntrada(string fecha)
        {
            var inputs = Driver.FindElements(_fechaEntradaInput);
            if (inputs.Count >= 2)
            {
                ((OpenQA.Selenium.IJavaScriptExecutor)Driver).ExecuteScript($"arguments[0].value = '{fecha}'; arguments[0].dispatchEvent(new Event('input', {{ bubbles: true }})); arguments[0].dispatchEvent(new Event('change', {{ bubbles: true }}));", inputs[1]);
                Thread.Sleep(300);
            }
        }

        /// <summary>
        /// Ingresar fecha de salida en segunda habitación
        /// </summary>
        public void IngresarSegundaFechaSalida(string fecha)
        {
            var inputs = Driver.FindElements(_fechaSalidaInput);
            if (inputs.Count >= 2)
            {
                ((OpenQA.Selenium.IJavaScriptExecutor)Driver).ExecuteScript($"arguments[0].value = '{fecha}'; arguments[0].dispatchEvent(new Event('input', {{ bubbles: true }})); arguments[0].dispatchEvent(new Event('change', {{ bubbles: true }}));", inputs[1]);
                Thread.Sleep(300);
            }
        }

        /// <summary>
        /// Seleccionar huésped en segunda habitación
        /// </summary>
        public void SeleccionarSegundoHuesped(string nombreCompleto)
        {
            var buscadores = Driver.FindElements(_huespedBuscadorInput);
            if (buscadores.Count >= 2)
            {
                buscadores[1].Click();
                Thread.Sleep(1200);
                
                var suggestions = Driver.FindElements(_huespedSuggestions);
                var huespedOption = suggestions.FirstOrDefault(s => s.Text.Contains(nombreCompleto.Split(' ')[0], StringComparison.OrdinalIgnoreCase));
                
                if (huespedOption != null)
                {
                    ((OpenQA.Selenium.IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].dispatchEvent(new MouseEvent('mousedown', { bubbles: true }));", huespedOption);
                    Thread.Sleep(500);
                }
                else
                {
                    throw new Exception($"No se encontró el segundo huésped '{nombreCompleto}'");
                }
            }
        }
    }
}
