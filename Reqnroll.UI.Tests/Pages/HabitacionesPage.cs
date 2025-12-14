using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Linq;

namespace Reqnroll.UI.Tests.Pages
{
    public class HabitacionesPage : BasePage
    {
        private readonly By _nuevaHabitacionBtn = By.XPath("//button[contains(., 'Nueva Habitación')]");
        // Selectores para el formulario de nueva habitación
        private readonly By _numeroInput = By.CssSelector("input[formcontrolname='numero']");
        private readonly By _pisoInput = By.CssSelector("input[formcontrolname='piso']");
        private readonly By _tipoDropdown = By.CssSelector("select[formcontrolname='tipoHabitacionId']");
        private readonly By _estadoDropdown = By.CssSelector("select[formcontrolname='estado']");
        private readonly By _guardarBtn = By.XPath("//button[@type='submit'] | //button[contains(text(), 'Guardar')]");
        private readonly By _tablaFilas = By.CssSelector(".table-row, table tbody tr");
        private readonly By _mensajeExito = By.CssSelector("p.estado.ok, .alert-success, .toast-success, .mensaje");

        public HabitacionesPage(IWebDriver driver) : base(driver) { }

        public void NavegarAListado(string baseUrl) => NavigateTo($"{baseUrl}/habitaciones");
        public void ClickNuevaHabitacion() => ClickElement(_nuevaHabitacionBtn);
        
        public void IngresarNumero(string num) => TypeText(_numeroInput, num);
        public void IngresarPiso(string piso) => TypeText(_pisoInput, piso);

        public void SeleccionarTipo(string tipo) {
            var el = WaitForElement(_tipoDropdown);
            // Buscar la opción que contenga el texto del tipo (busca dentro del texto de la opción)
            var option = el.FindElements(By.TagName("option")).FirstOrDefault(o => o.Text.Contains(tipo));
            if (option != null) {
                option.Click();
            } else {
                throw new NoSuchElementException($"No se encontró la opción '{tipo}' en el dropdown de tipos");
            }
        }

        public void SeleccionarEstado(string estado) {
            var el = WaitForElement(_estadoDropdown);
            // Buscar la opción que contenga el texto del estado (busca dentro del texto de la opción)
            var option = el.FindElements(By.TagName("option")).FirstOrDefault(o => o.Text.Contains(estado));
            if (option != null) {
                option.Click();
            } else {
                throw new NoSuchElementException($"No se encontró la opción '{estado}' en el dropdown de estados");
            }
        }

        public void ClickGuardar() {
            ClickElement(_guardarBtn);
            
            // Intentar manejar alertas que puedan aparecer
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(3));
                var alert = wait.Until(d => {
                    try
                    {
                        return d.SwitchTo().Alert();
                    }
                    catch
                    {
                        return null;
                    }
                });
                if (alert != null)
                {
                    alert.Accept();
                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch
            {
                // No hay alert, continuar
            }

            // Esperar a que se navegue a la página de listado de habitaciones después de guardar
            System.Threading.Thread.Sleep(2000);
            // Esperar a que la tabla de habitaciones esté visible
            WaitForElement(_tablaFilas);
        }
        public bool ExisteEnLista(string numero) => Driver.FindElements(_tablaFilas).Any(f => f.Text.Contains(numero));
        public bool IsMensajeExitoVisible() => IsElementVisible(_mensajeExito);

        public void BuscarHabitacion(string numero)
        {
            // Buscar en el input de búsqueda
            var searchInput = Driver.FindElement(By.CssSelector("input.input-busqueda"));
            searchInput.Clear();
            searchInput.SendKeys(numero);
            System.Threading.Thread.Sleep(500); // Esperar a que se filtre
        }

        public void ClickEditarHabitacion(string numero)
        {
            // Buscar la fila de la habitación y hacer click en el botón editar
            var filas = Driver.FindElements(_tablaFilas);
            var fila = filas.FirstOrDefault(f => f.Text.Contains(numero));
            if (fila != null)
            {
                var btnEditar = fila.FindElement(By.CssSelector("button.btn-editar"));
                btnEditar.Click();
                System.Threading.Thread.Sleep(500); // Esperar a que se abra el modal
            }
            else
            {
                throw new NoSuchElementException($"No se encontró la habitación '{numero}' en la lista");
            }
        }

        public void CambiarEstadoEnModal(string estado)
        {
            // Buscar el select de tipo en el modal y asegurar que tiene un valor
            var modalTipoSelect = Driver.FindElement(By.CssSelector("div.modal-backdrop select[name='tipoId']"));
            var tipoOptions = modalTipoSelect.FindElements(By.TagName("option"));
            // Seleccionar la primera opción válida (no la que dice "Seleccione un tipo")
            var validOption = tipoOptions.FirstOrDefault(o => !o.Text.Contains("Seleccione"));
            if (validOption != null)
            {
                validOption.Click();
                System.Threading.Thread.Sleep(300);
            }

            // Ahora cambiar el estado
            var modalEstadoSelect = Driver.FindElement(By.CssSelector("div.modal-backdrop select[name='estado']"));
            var option = modalEstadoSelect.FindElements(By.TagName("option")).FirstOrDefault(o => o.Text.Contains(estado));
            if (option != null)
            {
                option.Click();
            }
            else
            {
                throw new NoSuchElementException($"No se encontró la opción '{estado}' en el dropdown de estados del modal");
            }
            System.Threading.Thread.Sleep(300); // Pequeña pausa
        }

        public bool VerificarEstadoHabitacion(string numero, string estado)
        {
            // Esperar a que el modal se cierre y se actualice la lista
            System.Threading.Thread.Sleep(2000);
            var filas = Driver.FindElements(_tablaFilas);
            var fila = filas.FirstOrDefault(f => f.Text.Contains(numero));
            if (fila != null)
            {
                return fila.Text.Contains(estado);
            }
            return false;
        }
    }
}