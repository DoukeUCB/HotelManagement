using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Linq;

namespace Reqnroll.UI.Tests.Pages
{
    public class HabitacionesPage : BasePage
    {
        // Selectores correctos basados en el componente actual
        private readonly By _nuevaHabitacionBtn = By.XPath("//button[contains(text(), 'Nueva Habitación')]");
        private readonly By _numeroInput = By.CssSelector("input[formcontrolname='numero']");
        private readonly By _pisoInput = By.CssSelector("input[formcontrolname='piso']");
        private readonly By _tipoDropdown = By.CssSelector("select[formcontrolname='tipoHabitacionId']");
        private readonly By _estadoDropdown = By.CssSelector("select[formcontrolname='estado']");
        private readonly By _guardarBtn = By.XPath("//button[@type='submit']");
        private readonly By _tablaHabitaciones = By.CssSelector(".data-table .table-row");
        private readonly By _mensajeExito = By.CssSelector(".estado.ok");

        public HabitacionesPage(IWebDriver driver) : base(driver) { }

        public void NavegarAListado(string baseUrl) => NavigateTo($"{baseUrl}/habitaciones");
        public void ClickNuevaHabitacion() => ClickElement(_nuevaHabitacionBtn);
        public void IngresarNumero(string num) => TypeText(_numeroInput, num);
        public void IngresarPiso(string piso) => TypeText(_pisoInput, piso);
        
        public void SeleccionarTipo(string tipo) {
            var el = WaitForElement(_tipoDropdown);
            var selectElement = new OpenQA.Selenium.Support.UI.SelectElement(el);
            
            // Imprimir opciones disponibles para debug
            var allOptions = el.FindElements(By.TagName("option"));
            Console.WriteLine($"Opciones disponibles en el dropdown:");
            foreach (var option in allOptions)
            {
                Console.WriteLine($"  - Value: '{option.GetAttribute("value")}' | Text: '{option.Text}'");
            }
            
            // Intentar seleccionar por texto exacto primero
            try
            {
                selectElement.SelectByText(tipo);
                Console.WriteLine($"✓ Seleccionado por texto exacto: {tipo}");
            }
            catch
            {
                // Si no funciona, buscar por coincidencia parcial
                var matchingOption = allOptions.FirstOrDefault(o => o.Text.Contains(tipo, StringComparison.OrdinalIgnoreCase));
                
                if (matchingOption != null)
                {
                    selectElement.SelectByValue(matchingOption.GetAttribute("value"));
                    Console.WriteLine($"✓ Seleccionado por coincidencia parcial: {tipo}");
                }
                else
                {
                    throw new Exception($"No se encontró el tipo de habitación '{tipo}'. Opciones disponibles: {string.Join(", ", allOptions.Select(o => o.Text))}");
                }
            }
        }

        public void SeleccionarEstado(string estado) {
            var el = WaitForElement(_estadoDropdown);
            var selectElement = new OpenQA.Selenium.Support.UI.SelectElement(el);
            
            // Imprimir opciones disponibles para debug
            var allOptions = el.FindElements(By.TagName("option"));
            Console.WriteLine($"Opciones de estado disponibles:");
            foreach (var option in allOptions)
            {
                Console.WriteLine($"  - Value: '{option.GetAttribute("value")}' | Text: '{option.Text}'");
            }
            
            // Intentar seleccionar por texto exacto primero
            try
            {
                selectElement.SelectByText(estado);
                Console.WriteLine($"✓ Estado seleccionado por texto exacto: {estado}");
            }
            catch
            {
                // Si no funciona, buscar por coincidencia parcial
                var matchingOption = allOptions.FirstOrDefault(o => o.Text.Contains(estado, StringComparison.OrdinalIgnoreCase));
                
                if (matchingOption != null)
                {
                    selectElement.SelectByValue(matchingOption.GetAttribute("value"));
                    Console.WriteLine($"✓ Estado seleccionado por coincidencia parcial: {estado}");
                }
                else
                {
                    throw new Exception($"No se encontró el estado '{estado}'. Opciones disponibles: {string.Join(", ", allOptions.Select(o => o.Text))}");
                }
            }
        }

        public void ClickGuardar()
        {
            Console.WriteLine("🔍 Intentando hacer click en guardar...");
            ClickElement(_guardarBtn);
            Console.WriteLine("✅ Click en guardar realizado");
            
            // Esperar a que se procese la solicitud (desaparece el loader o aparece un mensaje)
            Console.WriteLine("⏳ Esperando a que desaparezca el loader...");
            WaitForLoadingToDisappear();
            Console.WriteLine("✅ Loader desapareció");
            
            // Esperar un poco más para que la página redireccionada cargue los datos
            Console.WriteLine("⏳ Esperando 1500ms para que se navegue y carguen datos...");
            System.Threading.Thread.Sleep(1500);
            Console.WriteLine("✅ Espera completada");
        }

        public bool ExisteHabitacionEnLista(string numero)
        {
            // Esperar a que la tabla se cargue después de la redirección
            try
            {
                // Esperar hasta que la tabla sea visible (máximo 10 segundos)
                WaitForElement(_tablaHabitaciones);
            }
            catch
            {
                // Si la tabla no aparece, retornar False
                return false;
            }
            
            return Driver.FindElements(_tablaHabitaciones).Any(f => f.Text.Contains(numero));
        }

        public bool IsMensajeExitoVisible()
        {
            try
            {
                // Crear un wait con timeout corto (5 segundos) para capturar el mensaje
                // que aparece antes de la redirección (1500ms después de guardar)
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(5));
                wait.Until(d => {
                    try
                    {
                        var element = Driver.FindElement(_mensajeExito);
                        return element.Displayed;
                    }
                    catch
                    {
                        return false;
                    }
                });
                return true;
            }
            catch (OpenQA.Selenium.WebDriverTimeoutException)
            {
                Console.WriteLine("⚠ Timeout esperando al mensaje de éxito - puede haber sido redirigido antes");
                return false;
            }
        }
    }
}