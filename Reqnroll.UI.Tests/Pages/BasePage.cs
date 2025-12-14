using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Reqnroll.UI.Tests.Pages
{
    /// <summary>
    /// Clase base para todos los Page Objects con métodos comunes
    /// </summary>
    public abstract class BasePage
    {
        protected readonly IWebDriver Driver;
        protected readonly WebDriverWait Wait;

        protected BasePage(IWebDriver driver)
        {
            Driver = driver;
            Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        /// <summary>
        /// Esperar hasta que un elemento sea visible
        /// </summary>
        protected IWebElement WaitForElement(By locator)
        {
            return Wait.Until(ExpectedConditions.ElementIsVisible(locator));
        }

        /// <summary>
        /// Esperar hasta que un elemento sea clickeable
        /// </summary>
        protected IWebElement WaitForClickable(By locator)
        {
            return Wait.Until(ExpectedConditions.ElementToBeClickable(locator));
        }

        /// <summary>
        /// Click con espera
        /// </summary>
        protected void ClickElement(By locator)
        {
            WaitForClickable(locator).Click();
        }

        /// <summary>
        /// Escribir texto en un input
        /// </summary>
        protected void TypeText(By locator, string text)
        {
            var element = WaitForElement(locator);
            element.Clear();
            element.SendKeys(text);
        }

        /// <summary>
        /// Seleccionar opción de dropdown por texto visible
        /// </summary>
        protected void SelectDropdownByText(By locator, string text)
        {
            var element = WaitForElement(locator);
            var select = new SelectElement(element);
            select.SelectByText(text);
        }

        /// <summary>
        /// Obtener texto de un elemento
        /// </summary>
        protected string GetElementText(By locator)
        {
            return WaitForElement(locator).Text;
        }

        /// <summary>
        /// Verificar si elemento está visible
        /// </summary>
        protected bool IsElementVisible(By locator)
        {
            try
            {
                return Driver.FindElement(locator).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>
        /// Esperar a que desaparezca el loader/spinner
        /// </summary>
        protected void WaitForLoadingToDisappear()
        {
            try
            {
                Wait.Until(driver => 
                {
                    var loaders = driver.FindElements(By.CssSelector(".loading, .spinner, mat-spinner"));
                    return loaders.Count == 0 || !loaders.Any(l => l.Displayed);
                });
            }
            catch (WebDriverTimeoutException)
            {
                // Timeout esperado si no hay loader
            }
        }

        /// <summary>
        /// Navegar a una URL
        /// </summary>
        public void NavigateTo(string url)
        {
            Driver.Navigate().GoToUrl(url);
        }

        /// <summary>
        /// Obtener URL actual
        /// </summary>
        public string GetCurrentUrl()
        {
            return Driver.Url;
        }
    }
}
