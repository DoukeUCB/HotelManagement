using OpenQA.Selenium;
using System.Linq;

namespace Reqnroll.UI.Tests.Pages
{
    public class HabitacionesPage : BasePage
    {
        // Selectores corregidos con las mayúsculas de tu backend
        private readonly By _nuevaHabitacionBtn = By.XPath("//button[contains(., 'Nueva Habitación')]");
        private readonly By _numeroInput = By.CssSelector("input[formcontrolname='Numero_Habitacion']");
        private readonly By _pisoInput = By.CssSelector("input[formcontrolname='Piso']");
        private readonly By _tipoDropdown = By.CssSelector("select[formcontrolname='Tipo_Habitacion_ID']");
        private readonly By _estadoDropdown = By.CssSelector("select[formcontrolname='Estado_Habitacion']");
        private readonly By _guardarBtn = By.XPath("//button[@type='submit'] | //button[contains(text(), 'Guardar')]");
        private readonly By _tablaFilas = By.CssSelector("table tbody tr");
        private readonly By _mensajeExito = By.CssSelector(".alert-success, .toast-success");

        public HabitacionesPage(IWebDriver driver) : base(driver) { }

        public void NavegarAListado(string baseUrl) => NavigateTo($"{baseUrl}/habitaciones");
        
        public void ClickNuevaHabitacion() => ClickElement(_nuevaHabitacionBtn);

        public void IngresarNumero(string numero) => TypeText(_numeroInput, numero);

        public void IngresarPiso(string piso) => TypeText(_pisoInput, piso);

        public void SeleccionarTipo(string tipo) => SeleccionarDropdown(_tipoDropdown, tipo);

        public void SeleccionarEstado(string estado) => SeleccionarDropdown(_estadoDropdown, estado);

        public void LlenarFormulario(string numero, string piso, string tipo, string estado)
        {
            TypeText(_numeroInput, numero);
            TypeText(_pisoInput, piso);
            SeleccionarDropdown(_tipoDropdown, tipo);
            SeleccionarDropdown(_estadoDropdown, estado);
        }

        public void SeleccionarDropdown(By locator, string texto)
        {
            var element = WaitForElement(locator);
            new OpenQA.Selenium.Support.UI.SelectElement(element).SelectByText(texto);
        }

        public void ClickGuardar() => ClickElement(_guardarBtn);

        // --- Lógica para el UPDATE ---
        public void ClickEditarHabitacion(string numero)
        {
            // Busca la fila que contiene el número y hace click en su botón editar
            var fila = Driver.FindElements(_tablaFilas)
                .FirstOrDefault(f => f.Text.Contains(numero));
            
            if (fila == null) throw new Exception($"No se encontró la habitación {numero} para editar");
            
            var editBtn = fila.FindElement(By.XPath(".//button[contains(., 'Editar')]"));
            editBtn.Click();
        }

        public string ObtenerEstadoHabitacion(string numero)
        {
            var fila = Driver.FindElements(_tablaFilas).FirstOrDefault(f => f.Text.Contains(numero));
            return fila?.Text ?? "";
        }

        public bool ExisteEnLista(string numero) => Driver.FindElements(_tablaFilas).Any(f => f.Text.Contains(numero));
        public bool IsMensajeExitoVisible() => IsElementVisible(_mensajeExito);
    }
}