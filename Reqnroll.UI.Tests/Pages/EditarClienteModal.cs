using OpenQA.Selenium;
using Reqnroll.UI.Tests.Support;
using System.Linq; // Necesario para .Any()

namespace Reqnroll.UI.Tests.Pages
{
    public class EditarClienteModal : BasePage
    {
        private readonly By _modalContainer = By.CssSelector(".modal.modal-editar");
        private readonly By _inputRazonSocial = By.CssSelector(".modal-editar input[name='razonSocial']");
        private readonly By _inputNit = By.CssSelector(".modal-editar input[name='documento']");
        private readonly By _inputEmail = By.CssSelector(".modal-editar input[name='email']");
        private readonly By _btnGuardar = By.CssSelector(".modal-editar button[type='submit']");

        public EditarClienteModal(IWebDriver driver) : base(driver)
        {
        }

        public void EsperarQueAparezca()
        {
            Wait.Until(d => d.FindElement(_modalContainer).Displayed);
        }

        public void EditarRazonSocial(string nuevaRazon)
        {
            var input = Driver.FindElement(_inputRazonSocial);
            input.Clear();
            input.SendKeys(nuevaRazon);
        }

        public void EditarNit(string nuevoNit)
        {
            var input = Driver.FindElement(_inputNit);
            input.Clear();
            input.SendKeys(nuevoNit);
        }

        public void EditarEmail(string nuevoEmail)
        {
            var input = Driver.FindElement(_inputEmail);
            input.Clear();
            input.SendKeys(nuevoEmail);
        }

        public void GuardarCambios()
        {
            var btn = Driver.FindElement(_btnGuardar);
            btn.Click();
            
            // CORRECCIÓN STALE ELEMENT:
            // En lugar de verificar .Displayed (que falla si el elemento ya no existe),
            // esperamos hasta que la cantidad de modales encontrados sea 0.
            try 
            {
                Wait.Until(d => d.FindElements(_modalContainer).Count == 0);
            }
            catch (WebDriverTimeoutException)
            {
                // Si falla por timeout, verificamos si al menos ya no es visible
                // (útil si el elemento persiste pero oculto con display: none)
                try {
                    Wait.Until(d => !d.FindElement(_modalContainer).Displayed);
                } catch (StaleElementReferenceException) {
                    // Si da Stale aquí, es buena noticia: el elemento desapareció, así que terminamos.
                    return; 
                }
            }
        }
    }
}