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
            var headlessEnv = Environment.GetEnvironmentVariable("HEADLESS");
            var ciEnv = Environment.GetEnvironmentVariable("CI");

            var headless =
                string.Equals(headlessEnv, "true", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(ciEnv, "true", StringComparison.OrdinalIgnoreCase);

            _context.Driver = _context.CreateChromeDriver(headless: headless);
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
                    if (_context.Driver is OpenQA.Selenium.ITakesScreenshot takesScreenshot)
                    {
                        var screenshot = takesScreenshot.GetScreenshot();
                        var fileName = $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Screenshots", fileName);
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                        screenshot.SaveAsFile(filePath);

                        Console.WriteLine($"Screenshot guardado: {filePath}");
                    }
                    else
                    {
                        Console.WriteLine("Screenshot omitido: WebDriver no inicializado o no soporta screenshots.");
                    }
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
