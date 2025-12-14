    using Reqnroll;
    using FluentAssertions;
    using Reqnroll.UI.Tests.Pages;
    using Reqnroll.UI.Tests.Support;
    using OpenQA.Selenium;

    namespace Reqnroll.UI.Tests.StepDefinitions
    {
        [Binding]
        public class GestionClientesStepDefinitions
        {
            private readonly WebDriverContext _context;
            private NuevoClientePage? _nuevoClientePage;

            public GestionClientesStepDefinitions(WebDriverContext context)
            {
                _context = context;
            }

            [Given(@"que estoy en la página de creación de clientes")]
            public void DadoQueEstoyEnLaPaginaDeCreacionDeClientes()
            {
                _nuevoClientePage = new NuevoClientePage(_context.Driver!);
                _nuevoClientePage.NavegarAlFormulario(_context.BaseUrl);
            }

            [When(@"ingreso la Razón Social ""(.*)""")]
            public void CuandoIngresoLaRazonSocial(string razonSocial)
            {
                _context.Driver!.FindElement(By.CssSelector("input[formControlName='razonSocial']")).SendKeys(razonSocial);
            }

            [When(@"ingreso el NIT ""(.*)""")]
            public void CuandoIngresoElNIT(string nit)
            {
                // 1. Generamos sufijo aleatorio para evitar error de duplicado en BD
                var random = new Random();
                string sufijo = random.Next(100, 999).ToString(); // 3 dígitos

                // 2. Definimos el límite de la BD (schema.sql: VARCHAR(20))
                int limiteMaximo = 20;

                // 3. Calculamos cuánto espacio nos queda para el NIT original
                // Si el NIT original + sufijo supera 20, recortamos el original.
                int longitudDisponible = limiteMaximo - sufijo.Length;

                string nitBase = nit;
                if (nitBase.Length > longitudDisponible)
                {
                    nitBase = nitBase.Substring(0, longitudDisponible);
                }

                // 4. Concatenamos
                string nitUnico = nitBase + sufijo;

                Console.WriteLine($"Escribiendo NIT ajustado al límite ({limiteMaximo}): {nitUnico}");
                
                // Enviamos el dato
                var nitInput = _context.Driver!.FindElement(By.CssSelector("input[formControlName='nit']"));
                nitInput.Clear(); // Buena práctica limpiar antes de escribir
                nitInput.SendKeys(nitUnico);
            }

            [When(@"ingreso el Email ""(.*)""")]
            public void CuandoIngresoElEmail(string email)
            {
                // 1. Generamos sufijo aleatorio
                var random = new Random();
                string sufijo = random.Next(100, 999).ToString(); // 3 dígitos

                // 2. Definimos el límite estricto (schema.sql: VARCHAR(30))
                int limiteMaximo = 30;

                // 3. Separamos usuario y dominio
                var partes = email.Split('@');
                if (partes.Length < 2) 
                {
                    // Fallback por si el dato de prueba no es un email válido
                    _context.Driver!.FindElement(By.CssSelector("input[formControlName='email']")).SendKeys(email);
                    return;
                }

                var usuario = partes[0];
                var dominio = partes[1];

                // 4. Cálculo inteligente de espacio
                // Estructura final: [Usuario][Sufijo]@[Dominio]
                // Longitud fija que NO podemos tocar: Sufijo + @ + Dominio
                int longitudIntocable = sufijo.Length + 1 + dominio.Length;
                
                // El espacio que le queda al nombre de usuario es:
                int espacioParaUsuario = limiteMaximo - longitudIntocable;

                if (espacioParaUsuario <= 0)
                {
                    throw new Exception($"El dominio '{dominio}' es demasiado largo para permitir un sufijo aleatorio dentro del límite de 30 caracteres.");
                }

                // Si el usuario original es más largo que el espacio disponible, lo recortamos
                if (usuario.Length > espacioParaUsuario)
                {
                    usuario = usuario.Substring(0, espacioParaUsuario);
                }

                // 5. Armamos el email final
                string emailUnico = $"{usuario}{sufijo}@{dominio}";

                Console.WriteLine($"Escribiendo Email ajustado al límite ({limiteMaximo}): {emailUnico}");

                var emailInput = _context.Driver!.FindElement(By.CssSelector("input[formControlName='email']"));
                emailInput.Clear();
                emailInput.SendKeys(emailUnico);
            }

            [When(@"hago click en guardar cliente")]
            public void CuandoHagoClickEnGuardarCliente()
            {
                _nuevoClientePage!.ClickGuardar();
            }

            [Then(@"debería ver un mensaje de éxito indicando que se guardó correctamente")]
            public void EntoncesDeberiaVerUnMensajeDeExito()
            {
                // Damos un pequeño respiro para que la animación del toast termine/aparezca
                Thread.Sleep(500); 

                bool exito = _nuevoClientePage!.EsMensajeExitoVisible();
                
                if (!exito)
                {
                    // Si falla, intentamos capturar si hay un mensaje de error visible para el log
                    try {
                        var errorMsg = _context.Driver!.FindElement(By.ClassName("toast-error")).Text; // Ajusta selector según tu librería de Toasts
                        Console.WriteLine($"ERROR EN UI: Se encontró un mensaje de error: {errorMsg}");
                    } catch {}
                }

                exito.Should().BeTrue("El mensaje de éxito debería aparecer después de guardar");
                
                if(exito)
                {
                    string mensaje = _nuevoClientePage.ObtenerMensajeExito();
                    Console.WriteLine($"Mensaje recibido: {mensaje}");
                }
            }
        }
    }