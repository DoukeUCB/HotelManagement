using OpenQA.Selenium;
using Reqnroll.UI.Tests.Support;

namespace Reqnroll.UI.Tests.Pages
{
    public class EliminarClienteModal : BasePage
    {
        // Selectores basados en tu HTML:
        // <div *ngIf="modalEliminarAbierto" class="modal-backdrop"> ... <h2>¿Eliminar este cliente?</h2> ...
        private readonly By _modalContainer = By.CssSelector(".modal-backdrop .modal");
        
        // Botón: <button class="btn btn-danger" (click)="confirmarEliminar()">Eliminar</button>
        // Usamos XPath para ser específicos con el texto y la clase, ya que hay dos botones.
        private readonly By _btnConfirmarEliminar = By.XPath("//div[contains(@class,'modal-backdrop')]//button[contains(@class,'btn-danger') and contains(text(),'Eliminar')]");
        
        // Botón cancelar (opcional, por si quisieras testear cancelación)
        private readonly By _btnCancelar = By.XPath("//div[contains(@class,'modal-backdrop')]//button[contains(text(),'Cancelar')]");

        public EliminarClienteModal(IWebDriver driver) : base(driver)
        {
        }

        public void EsperarQueAparezca()
        {
            Wait.Until(d => d.FindElement(_modalContainer).Displayed);
        }

        public void ConfirmarEliminacion()
        {
            var btn = Driver.FindElement(_btnConfirmarEliminar);
            btn.Click();

            // Esperamos a que el modal desaparezca para asegurar que la acción terminó
            try 
            {
                Wait.Until(d => d.FindElements(_modalContainer).Count == 0);
            }
            catch (WebDriverTimeoutException)
            {
                // Fallback: verificar si ya no es visible
                Wait.Until(d => !d.FindElement(_modalContainer).Displayed);
            }
        }
    }
}