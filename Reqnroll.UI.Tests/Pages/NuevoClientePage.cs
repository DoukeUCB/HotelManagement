using OpenQA.Selenium;
using Reqnroll.UI.Tests.Support;

namespace Reqnroll.UI.Tests.Pages
{
    public class NuevoClientePage : BasePage
    {
        // Selectores
        private readonly By _razonSocialInput = By.CssSelector("input[formControlName='razonSocial']");
        private readonly By _nitInput = By.CssSelector("input[formControlName='nit']");
        private readonly By _emailInput = By.CssSelector("input[formControlName='email']");
        private readonly By _btnGuardar = By.CssSelector("button[type='submit']");
        private readonly By _mensajeExito = By.CssSelector(".estado.ok");
        private readonly By _mensajeError = By.CssSelector(".estado.error");

        public NuevoClientePage(IWebDriver driver) : base(driver)
        {
        }

        public void NavegarAlFormulario(string baseUrl)
        {
            NavigateTo($"{baseUrl.TrimEnd('/')}/nuevo-cliente");
            // Esperamos a que el input principal sea visible antes de decir que cargó
            WaitForElement(_razonSocialInput);
        }

        // Métodos individuales con ESPERA AUTOMÁTICA (TypeText usa WaitForElement)
        public void IngresarRazonSocial(string razonSocial)
        {
            TypeText(_razonSocialInput, razonSocial);
        }

        public void IngresarNit(string nit)
        {
            TypeText(_nitInput, nit);
        }

        public void IngresarEmail(string email)
        {
            TypeText(_emailInput, email);
        }

        public void ClickGuardar()
        {
            // A veces el botón tarda en habilitarse, usamos click seguro
            ClickElement(_btnGuardar);
        }

        public bool EsMensajeExitoVisible()
        {
            try 
            {
                // Damos tiempo a que el servidor responda y muestre el mensaje
                WaitForElement(_mensajeExito);
                return true;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public string? TryObtenerMensajeError()
        {
            try
            {
                var el = Driver.FindElements(_mensajeError).FirstOrDefault();
                if (el == null) return null;
                var text = el.Text?.Trim();
                return string.IsNullOrWhiteSpace(text) ? null : text;
            }
            catch
            {
                return null;
            }
        }

        public string ObtenerMensajeExito()
        {
            return GetElementText(_mensajeExito);
        }
    }
}