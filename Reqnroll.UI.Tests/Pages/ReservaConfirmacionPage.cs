using OpenQA.Selenium;

namespace Reqnroll.UI.Tests.Pages
{
    /// <summary>
    /// Page Object para la página de confirmación (Paso 3)
    /// </summary>
    public class ReservaConfirmacionPage : BasePage
    {
        // Locators
        private readonly By _montoTotalLabel = By.CssSelector("input[readonly], .monto-total-value");
        private readonly By _crearReservaBtn = By.XPath("//button[contains(text(), 'Crear Reserva')]");
        private readonly By _clienteInfo = By.CssSelector(".resumen-reserva");
        private readonly By _habitacionesInfo = By.CssSelector(".resumen-reserva");

        public ReservaConfirmacionPage(IWebDriver driver) : base(driver) { }

        /// <summary>
        /// Obtener el monto total mostrado
        /// </summary>
        public string ObtenerMontoTotal()
        {
            return GetElementText(_montoTotalLabel);
        }

        /// <summary>
        /// Click en botón Crear Reserva
        /// </summary>
        public void ClickConfirmar()
        {
            Thread.Sleep(300);
            var btn = WaitForElement(_crearReservaBtn);
            ((OpenQA.Selenium.IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView(true); arguments[0].click();", btn);
            Thread.Sleep(300);
        }

        /// <summary>
        /// Verificar que la información del cliente se muestra
        /// </summary>
        public bool ClienteInfoVisible()
        {
            return IsElementVisible(_clienteInfo);
        }

        /// <summary>
        /// Verificar que la información de habitaciones se muestra
        /// </summary>
        public bool HabitacionesInfoVisible()
        {
            return IsElementVisible(_habitacionesInfo);
        }
    }
}
