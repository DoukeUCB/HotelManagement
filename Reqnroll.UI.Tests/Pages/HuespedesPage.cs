using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace Reqnroll.UI.Tests.Pages
{
    public class HuespedesPage
    {
        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;

        public HuespedesPage(IWebDriver driver, TimeSpan? timeout = null)
        {
            this.driver = driver ?? throw new ArgumentNullException(nameof(driver));
            this.wait = new WebDriverWait(driver, timeout ?? TimeSpan.FromSeconds(10));
        }

        // TODO: ajustar selectores según la aplicación
        private By BtnNuevoSelector => By.CssSelector("#btnNuevoHuesped");
        private By InputNombreSelector => By.CssSelector("#Nombre");
        private By InputSegundoNombreSelector => By.CssSelector("#SegundoNombre"); // ajustar si es distinto
        private By InputApellidoSelector => By.CssSelector("#Apellido");
        private By InputSegundoApellidoSelector => By.CssSelector("#SegundoApellido"); // ajustar si es distinto
        private By SelectTipoDocSelector => By.CssSelector("#TipoDocumento");
        private By InputNumDocSelector => By.CssSelector("#NumeroDocumento");
        private By InputTelefonoSelector => By.CssSelector("#Telefono");
        private By InputFechaNacSelector => By.CssSelector("#FechaNacimiento");
        private By BtnGuardarSelector => By.CssSelector("#btnGuardarHuesped");
        private By InputBuscarSelector => By.CssSelector("#buscarDocumento");
        private By BtnEditarSelector => By.CssSelector(".btn-editar");
        private By BtnEliminarSelector => By.CssSelector(".btn-eliminar");
        private By ConfirmEliminarSelector => By.CssSelector("#confirmEliminar");
        private By MensajeExitoSelector => By.CssSelector(".alert-success");

        // Navegación
        public void GoTo()
        {
            // TODO: ajustar la URL según entorno
            driver.Navigate().GoToUrl("http://localhost:5000/huespedes");
        }

        // Acciones principales
        public void ClickNuevo()
        {
            wait.Until(d => d.FindElement(BtnNuevoSelector)).Click();
        }

        public void EnterNombre(string nombre)
        {
            var el = wait.Until(d => d.FindElement(InputNombreSelector));
            el.Clear();
            el.SendKeys(nombre);
        }

        public void EnterSegundoNombre(string segundoNombre)
        {
            var el = wait.Until(d => d.FindElement(InputSegundoNombreSelector));
            el.Clear();
            el.SendKeys(segundoNombre ?? string.Empty);
        }

        public void EnterApellido(string apellido)
        {
            var el = wait.Until(d => d.FindElement(InputApellidoSelector));
            el.Clear();
            el.SendKeys(apellido);
        }

        public void EnterSegundoApellido(string segundoApellido)
        {
            var el = wait.Until(d => d.FindElement(InputSegundoApellidoSelector));
            el.Clear();
            el.SendKeys(segundoApellido ?? string.Empty);
        }

        public void SelectTipoDocumento(string tipo)
        {
            var el = wait.Until(d => d.FindElement(SelectTipoDocSelector));
            new SelectElement(el).SelectByText(tipo);
        }

        public void EnterNumeroDocumento(string numero)
        {
            var el = wait.Until(d => d.FindElement(InputNumDocSelector));
            el.Clear();
            el.SendKeys(numero);
        }

        public void EnterTelefono(string telefono)
        {
            var el = wait.Until(d => d.FindElement(InputTelefonoSelector));
            el.Clear();
            el.SendKeys(telefono ?? string.Empty);
        }

        public void EnterFechaNacimiento(string fecha)
        {
            var el = wait.Until(d => d.FindElement(InputFechaNacSelector));
            el.Clear();
            el.SendKeys(fecha ?? string.Empty);
        }

        public void ClickGuardar()
        {
            wait.Until(d => d.FindElement(BtnGuardarSelector)).Click();
        }

        public void BuscarPorDocumento(string numero)
        {
            var el = wait.Until(d => d.FindElement(InputBuscarSelector));
            el.Clear();
            el.SendKeys(numero);
            el.SendKeys(Keys.Enter);
        }

        public void ClickEditarFirst()
        {
            wait.Until(d => d.FindElement(BtnEditarSelector)).Click();
        }

        public void ClickEliminarFirst()
        {
            wait.Until(d => d.FindElement(BtnEliminarSelector)).Click();
        }

        public void ConfirmarEliminar()
        {
            wait.Until(d => d.FindElement(ConfirmEliminarSelector)).Click();
        }

        // Validaciones auxiliares
        public bool TieneMensajeExito()
        {
            try
            {
                var el = wait.Until(d => d.FindElement(MensajeExitoSelector));
                return el.Displayed && !string.IsNullOrEmpty(el.Text);
            }
            catch
            {
                return false;
            }
        }
        public bool IsPageLoaded()
        {
            try
            {
                return wait.Until(d => d.FindElement(BtnNuevoSelector).Displayed);
            }
            catch
            {
                return false;
            }
        }

        public string ObtenerIdDelHuesped()
        {
            // Supongamos que después de guardar, aparece en la primera fila de la tabla
            var idElement = wait.Until(d => d.FindElement(By.CssSelector(".tabla-huespedes tbody tr:first-child td.id")));
            return idElement.Text;
        }

        public string ObtenerDocumentoDelHuesped()
        {
            var docElement = wait.Until(d => d.FindElement(By.CssSelector(".tabla-huespedes tbody tr:first-child td.documento")));
            return docElement.Text;
        }

        public string ObtenerTelefonoPorDocumento(string doc)
        {
            // Buscar la fila por documento
            var row = wait.Until(d => d.FindElement(By.XPath($"//table[@class='tabla-huespedes']//td[text()='{doc}']/..")));
            var tel = row.FindElement(By.CssSelector("td.telefono"));
            return tel.Text;
        }

        public bool NoExisteHuesped(string doc)
        {
            try
            {
                driver.FindElement(By.XPath($"//table[@class='tabla-huespedes']//td[text()='{doc}']"));
                return false; // Si encuentra, no se eliminó
            }
            catch
            {
                return true; // No encontró → eliminado correctamente
            }
        }
        /// <summary>
/// Selecciona un huésped en la tabla por su número de documento.
/// Hace click en la fila correspondiente.
/// </summary>
        public void SeleccionarHuespedPorDocumento(string documento)
        {
            // Espera hasta que la fila con el documento exista y hace click
            var row = wait.Until(d => d.FindElement(By.XPath(
                $"//table[@class='tabla-huespedes']//td[text()='{documento}']/..")));
            row.Click(); // Ajusta si necesitas click en checkbox o columna específica
        }

        /// <summary>
        /// Obtiene el documento del huésped actualmente seleccionado en la tabla.
        /// </summary>
        public string ObtenerDocumentoDelHuespedSeleccionado()
        {
            // Suponemos que la fila seleccionada tiene la clase 'selected'
            var selectedRow = wait.Until(d => d.FindElement(By.CssSelector("tr.selected")));
            var docCell = selectedRow.FindElement(By.CssSelector("td.documento"));
            return docCell.Text;
        }



        // TODO: añadir helpers adicionales (obtener ID creado, validar teléfono actualizado, etc.)
    }
}
