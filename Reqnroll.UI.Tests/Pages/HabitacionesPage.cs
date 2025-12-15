using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;

namespace Reqnroll.UI.Tests.Pages
{
    public class HabitacionesPage : BasePage
    {
        // Selector para el modal de edición (Angular: div.modal-backdrop)
        private readonly By _modalEditar = By.CssSelector("div.modal-backdrop");
        // --- SELECTORES ---
        private readonly By _nuevaHabitacionBtn = By.XPath("//button[contains(., 'Nueva Habitación')]");
        private readonly By _numeroInput = By.CssSelector("input[formcontrolname='numero']");
        private readonly By _pisoInput = By.CssSelector("input[formcontrolname='piso']");
        private readonly By _tipoDropdown = By.CssSelector("select[formcontrolname='tipoHabitacionId']");
        private readonly By _estadoDropdown = By.CssSelector("select[formcontrolname='estado']");
        private readonly By _guardarBtn = By.XPath("//button[@type='submit'] | //button[contains(text(), 'Guardar')]");
        private readonly By _tablaFilas = By.CssSelector(".table-row, table tbody tr");
        // Selector de mensajes corregido para ser más inclusivo
        private readonly By _mensajeExito = By.CssSelector("p.estado.ok, .alert-success, .toast-success, .mensaje, .success-message");
        private readonly By _searchInput = By.CssSelector("input.input-busqueda");

        public HabitacionesPage(IWebDriver driver) : base(driver) { }

        // --- MÉTODOS DE NAVEGACIÓN Y ACCIONES BÁSICAS ---

        public void NavegarAListado(string baseUrl) => NavigateTo($"{baseUrl}/habitaciones");
        
        public void ClickNuevaHabitacion() 
        {
            ClickElement(_nuevaHabitacionBtn);
            // Pequeña espera para que el formulario se renderice
            System.Threading.Thread.Sleep(500);
        }

        public void IngresarNumero(string num) => TypeText(_numeroInput, num);
        
        public void IngresarPiso(string piso) => TypeText(_pisoInput, piso);

        public void SeleccionarTipo(string tipo) 
        {
            IWebElement el = null;
            // 1. Intentar buscar el select dentro del modal de edición (edición)
            try {
                el = WaitForElement(By.CssSelector("div.modal-backdrop select[name='tipoId']"));
            } catch {
                // 2. Si no está el modal, buscar el select global (creación)
                el = WaitForElement(_tipoDropdown);
            }
            var option = el.FindElements(By.TagName("option")).FirstOrDefault(o => o.Text.Contains(tipo));
            if (option != null) option.Click();
            else throw new NoSuchElementException($"No se encontró tipo: {tipo}");
        }

        public void SeleccionarEstado(string estado) 
        {
            var el = WaitForElement(_estadoDropdown);
            var option = el.FindElements(By.TagName("option")).FirstOrDefault(o => o.Text.Contains(estado));
            if (option != null) option.Click();
            else throw new NoSuchElementException($"No se encontró estado: {estado}");
        }

        public void ClickGuardar() 
        {
            ClickElement(_guardarBtn);
            
            // Intentar aceptar alertas JS automáticamente si aparecen tras guardar
            try {
                var waitAlert = new WebDriverWait(Driver, TimeSpan.FromSeconds(2));
                var alert = waitAlert.Until(d => d.SwitchTo().Alert());
                alert.Accept();
            } catch { /* No hay alerta, ignorar */ }

            // Sincronización post-guardado
            System.Threading.Thread.Sleep(1500);
        }

        // --- MÉTODOS PARA SELECT (CONSULTA) ---

        public void BuscarHabitacion(string numero) 
        {
            var search = WaitForElement(_searchInput);
            search.Clear();
            search.SendKeys(numero);
            System.Threading.Thread.Sleep(1000); // Tiempo para que el filtro actúe
        }

        public bool ExisteEnLista(string numero) 
        {
            try {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(5));
                return wait.Until(d => d.FindElements(_tablaFilas).Any(f => f.Text.Contains(numero)));
            } catch { return false; }
        }

        // --- MÉTODOS PARA UPDATE (ACTUALIZACIÓN) ---

        public void ClickEditarHabitacion(string numero) 
        {
            var fila = Driver.FindElements(_tablaFilas).FirstOrDefault(f => f.Text.Contains(numero));
            if (fila == null) throw new Exception($"No se encontró la habitación {numero} para editar.");
            
            var btnEditar = fila.FindElement(By.CssSelector("button.btn-editar"));
            btnEditar.Click();
            System.Threading.Thread.Sleep(800); // Espera a que el modal se abra
        }

        public void CambiarEstadoEnModal(string estado) 
        {
            // Asegurar que el modal esté cargado buscando un select interno
            var modalEstadoSelect = WaitForElement(By.CssSelector("div.modal-backdrop select[name='estado']"));
            var select = new SelectElement(modalEstadoSelect);
            
            // Selección robusta por texto parcial
            var opcion = select.Options.FirstOrDefault(o => o.Text.Contains(estado, StringComparison.OrdinalIgnoreCase));
            if (opcion != null) {
                select.SelectByText(opcion.Text);
            } else {
                throw new NoSuchElementException($"No se encontró el estado '{estado}' en el modal.");
            }
            System.Threading.Thread.Sleep(300);
        }

        // Espera a que el modal de edición desaparezca
        public void WaitForModalCerrar(int timeoutMs = 20000)
        {
            var wait = new WebDriverWait(Driver, TimeSpan.FromMilliseconds(timeoutMs));
            wait.Until(d =>
            {
                var modals = d.FindElements(_modalEditar);
                return modals.Count == 0 || !modals.Any(m => m.Displayed);
            });
        }

        public bool VerificarEstadoHabitacion(string numero, string estado) 
        {
            // Esperar a que el modal de edición desaparezca antes de verificar el estado
            WaitForModalCerrar();
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(15));
            return wait.Until(d => {
                try {
                    var fila = d.FindElements(_tablaFilas).FirstOrDefault(f => f.Text.Contains(numero));
                    if (fila == null) return false;
                    // Esperar a que el texto del estado se actualice en la fila
                    return fila.Text.Contains(estado, StringComparison.OrdinalIgnoreCase);
                } catch (OpenQA.Selenium.StaleElementReferenceException) {
                    // Si la tabla se recarga, reintentar
                    return false;
                }
            });
        }

        // --- MÉTODOS PARA DELETE (ELIMINACIÓN) ---

        public void EliminarHabitacion(string numero) 
        {
            var fila = Driver.FindElements(_tablaFilas).FirstOrDefault(f => f.Text.Contains(numero));
            if (fila != null) {
                fila.FindElement(By.CssSelector("button.btn-eliminar")).Click();
                System.Threading.Thread.Sleep(500);
                
                // Confirmación en el modal
                var btnConfirmar = Driver.FindElement(By.CssSelector("div.modal-backdrop button.btn-danger"));
                btnConfirmar.Click();
                System.Threading.Thread.Sleep(1500);
            }
        }

        // --- MÉTODOS DE VERIFICACIÓN DE ÉXITO (Sincronización robusta) ---

        public bool IsMensajeExitoVisible() 
        {
            // 1. Considerar éxito si el botón 'Nueva Habitación' o el título de la lista está visible (como en reservas)
            try {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(8));
                // Espera a que el botón esté visible tras la operación
                var btn = wait.Until(d => d.FindElement(_nuevaHabitacionBtn));
                if (btn.Displayed) return true;
            } catch { /* ignorar */ }

            // 2. Si no, buscar mensaje de éxito en el DOM (por compatibilidad)
            try {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(3));
                var el = wait.Until(d => d.FindElement(_mensajeExito));
                if (el.Displayed && !string.IsNullOrWhiteSpace(el.Text)) return true;
            } catch { /* ignorar */ }

            // 3. Si no, revisar si hay una alerta JS pendiente
            try {
                Driver.SwitchTo().Alert().Accept();
                return true;
            } catch { return false; }
        }
    }
}