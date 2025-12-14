using Reqnroll;
using Reqnroll.UI.Tests.Support;

namespace Reqnroll.UI.Tests.Hooks
{
    [Binding]
    public class WebDriverHooks
    {
        private readonly WebDriverContext _context;
        private readonly ScenarioContext _scenarioContext;

        public WebDriverHooks(WebDriverContext context, ScenarioContext scenarioContext)
        {
            _context = context;
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            // Crear WebDriver (cambiar a true para headless)
            _context.Driver = _context.CreateChromeDriver(headless: false);
            _context.Reset();
        }

        [AfterScenario]
        public void AfterScenario()
        {
            // Capturar screenshot si el escenario falló
            if (_scenarioContext.TestError != null)
            {
                try
                {
                    var screenshot = ((OpenQA.Selenium.ITakesScreenshot)_context.Driver!)
                        .GetScreenshot();
                    var fileName = $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Screenshots", fileName);
                    
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                    screenshot.SaveAsFile(filePath);
                    
                    Console.WriteLine($"Screenshot guardado: {filePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al capturar screenshot: {ex.Message}");
                }
            }

            // Cerrar navegador
            _context.Driver?.Quit();
            _context.Driver?.Dispose();
        }
    }
}
