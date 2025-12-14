using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Reqnroll.UI.Tests.Support
{
    /// <summary>
    /// Contexto compartido para WebDriver y datos del escenario
    /// </summary>
    public class WebDriverContext
    {
        public IWebDriver? Driver { get; set; }
        
        // URLs base
        public string BaseUrl { get; set; } = "http://localhost:4200";
        public string ApiUrl { get; set; } = "http://localhost:5000";
        
        // Datos del escenario actual
        public string? ClienteSeleccionado { get; set; }
        public string? ReservaId { get; set; }
        public bool OperacionExitosa { get; set; }
        public string? MensajeError { get; set; }

        public void Reset()
        {
            ClienteSeleccionado = null;
            ReservaId = null;
            OperacionExitosa = false;
            MensajeError = null;
        }

        /// <summary>
        /// Crear ChromeDriver con opciones optimizadas para testing
        /// </summary>
        public IWebDriver CreateChromeDriver(bool headless = false)
        {
            var options = new ChromeOptions();
            
            if (headless)
            {
                options.AddArgument("--headless=new");
            }
            
            // Opciones para estabilidad en tests
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--disable-infobars");
            
            // Desactivar logging excesivo
            options.AddArgument("--log-level=3");
            options.AddExcludedArgument("enable-logging");
            
            var driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);
            
            return driver;
        }
    }
}
