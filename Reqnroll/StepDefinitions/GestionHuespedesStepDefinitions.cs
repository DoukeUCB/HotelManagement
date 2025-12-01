using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Reqnroll;
using Reqnroll.Support; // ReservaTestContext
using HotelManagement.DTOs;
using HotelManagement.Repositories;
using HotelManagement.Aplicacion.Validators;
using HotelManagement.Application.Services; // HuespedService
using HotelManagement.Aplicacion.Exceptions; // ValidationException

namespace Reqnroll.StepDefinitions
{
    [Binding]
    public class GestionHuespedesStepDefinitions
    {
        private readonly ReservaTestContext _context;

        // Servicio concreto (tu proyecto no tiene IHuespedService en este momento)
        private HuespedService? _huespedService;

        private HuespedCreateDTO? _huespedCreateDto;
        private HuespedDTO? _huespedCreado;
        private bool _operacionExitosa;
        private string? _mensajeError;

        public GestionHuespedesStepDefinitions(ReservaTestContext context)
        {
            _context = context;
        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            _context.DbContext = TestDatabaseHelper.CreateTestDbContext();
            await TestDatabaseHelper.LimpiarDatos(_context.DbContext);

            var repo = new HuespedRepository(_context.DbContext);
            var val  = new HuespedValidator(_context.DbContext);
            _huespedService = new HuespedService(repo, val);

            // Reset estado del escenario
            _operacionExitosa = false;
            _mensajeError = null;
            _huespedCreado = null;
            _context.HuespedId = null;
        }

        [AfterScenario]
        public void AfterScenario()
        {
            _context.DbContext?.Dispose();
            _context.Reset();
        }

        // --------------------------
        // GIVEN (común para CREATE)
        // --------------------------
        [Given(@"que tengo los siguientes datos de huésped:")]
        public void DadoQueTengoLosSiguientesDatosDeHuesped(Table table)
        {
            string? Get(string campo) => table.Rows.FirstOrDefault(r => r["Campo"] == campo)?["Valor"];

            _huespedCreateDto = new HuespedCreateDTO
            {
                Nombre = Get("Nombre") ?? string.Empty,
                Apellido = Get("Apellido") ?? string.Empty,
                Segundo_Apellido = Get("Segundo_Apellido"),
                Documento_Identidad = Get("Documento_Identidad") ?? string.Empty,
                Telefono = Get("Telefono"),
                Fecha_Nacimiento = Get("Fecha_Nacimiento")
            };
        }

        // GIVEN helper: asegura que exista un huésped con ese documento (usado por updates/delete/select)
        [Given(@"que ya existe un huésped con documento ""(.*)""")]
        public async Task DadoQueYaExisteUnHuespedConDocumento(string documento)
        {
            // Si ya tenemos un id en contexto y el documento coincide, no crear duplicado
            if (!string.IsNullOrEmpty(_context.HuespedId))
            {
                var existing = (await _huespedService!.GetAllAsync())
                    .FirstOrDefault(h => h.Documento_Identidad == documento);
                if (existing != null)
                {
                    _context.HuespedId = existing.ID;
                    return;
                }
            }

            var dto = new HuespedCreateDTO
            {
                Nombre = "TEST",
                Apellido = "EXISTENTE",
                Documento_Identidad = documento,
                Telefono = "7777777",
                Fecha_Nacimiento = "1990-01-01"
            };

            // Si falla aquí, que explote: el escenario que crea datos previos no podrá continuar.
            var creado = await _huespedService!.CreateAsync(dto);
            _context.HuespedId = creado.ID;
        }

        // --------------------------
        // WHEN - CREATE
        // --------------------------
        [When(@"intento registrar el huésped")]
        public async Task CuandoIntentoRegistrarElHuesped()
        {
            _mensajeError = null;
            _operacionExitosa = false;
            _huespedCreado = null;

            try
            {
                _huespedCreado = await _huespedService!.CreateAsync(_huespedCreateDto!);
                _operacionExitosa = true;
                _context.HuespedId = _huespedCreado.ID;
            }
            catch (ValidationException vex)
            {
                _operacionExitosa = false;
                _mensajeError = CollectValidationMessage(vex);
            }
            catch (Exception ex)
            {
                _operacionExitosa = false;
                _mensajeError = ex.Message;
            }
        }

        // --------------------------
        // WHEN - UPDATE
        // --------------------------
        [When(@"actualizo el huésped con los siguientes datos:")]
        public async Task CuandoActualizoElHuespedConLosSiguientesDatos(Table table)
        {
            _mensajeError = null;
            _operacionExitosa = false;

            var id = _context.HuespedId;
            id.Should().NotBeNull("debe existir un huésped previo para actualizar");

            string? Get(string campo) => table.Rows.FirstOrDefault(r => r["Campo"] == campo)?["Valor"];

            var updateDto = new HuespedUpdateDTO
            {
                Nombre = Get("Nombre"),
                Apellido = Get("Apellido"),
                Segundo_Apellido = Get("Segundo_Apellido"),
                Documento_Identidad = Get("Documento_Identidad"),
                Telefono = Get("Telefono"),
                Fecha_Nacimiento = Get("Fecha_Nacimiento"),
                Activo = null
            };

            try
            {
                _huespedCreado = await _huespedService!.UpdateAsync(id!, updateDto);
                _operacionExitosa = true;
            }
            catch (ValidationException vex)
            {
                _operacionExitosa = false;
                _mensajeError = CollectValidationMessage(vex);
            }
            catch (Exception ex)
            {
                _operacionExitosa = false;
                _mensajeError = ex.Message;
            }
        }

        // --------------------------
        // WHEN - DELETE
        // --------------------------
        [When(@"elimino el huésped con documento ""(.*)""")]
        public async Task CuandoEliminoElHuespedConDocumento(string documento)
        {
            _mensajeError = null;
            _operacionExitosa = false;

            try
            {
                var lista = await _huespedService!.GetAllAsync();
                var existente = lista.FirstOrDefault(h => h.Documento_Identidad == documento);

                existente.Should().NotBeNull($"debe existir un huésped con documento {documento}");

                _context.HuespedId = existente!.ID;

                // Asumo DeleteAsync recibe ID (string) y elimina
                await _huespedService.DeleteAsync(existente.ID);

                _operacionExitosa = true;
            }
            catch (ValidationException vex)
            {
                _operacionExitosa = false;
                _mensajeError = CollectValidationMessage(vex);
            }
            catch (Exception ex)
            {
                _operacionExitosa = false;
                _mensajeError = ex.Message;
            }
        }

        // --------------------------
        // WHEN - SELECT (consultar por documento)
        // --------------------------
        [When(@"consulto el huésped con documento ""(.*)""")]
        public async Task CuandoConsultoElHuespedConDocumento(string documento)
        {
            _mensajeError = null;
            _operacionExitosa = false;

            try
            {
                // hacemos GetAll y filtramos por documento (mismo comportamiento que tus asserts)
                var lista = await _huespedService!.GetAllAsync();
                var encontrado = lista.FirstOrDefault(h => h.Documento_Identidad == documento);

                encontrado.Should().NotBeNull($"debe existir un huésped con documento {documento}");
                _context.HuespedId = encontrado!.ID;
                _operacionExitosa = true;
            }
            catch (Exception ex)
            {
                _operacionExitosa = false;
                _mensajeError = ex.Message;
            }
        }

        // --------------------------
        // THEN - Happy CREATE
        // --------------------------
        [Then(@"el huésped debe crearse exitosamente")]
        public void EntoncesElHuespedDebeCrearseExitosamente()
        {
            _operacionExitosa.Should().BeTrue("la creación debería ser exitosa");
            _huespedCreado.Should().NotBeNull("se debe haber creado un huésped");
            _context.HuespedId.Should().NotBeNull("debe haberse guardado el id en el contexto");
        }

        // --------------------------
        // THEN - Happy UPDATE
        // --------------------------
        [Then(@"los datos del huésped deben actualizarse correctamente")]
        public void EntoncesLosDatosDelHuespedDebenActualizarseCorrectamente()
        {
            _operacionExitosa.Should().BeTrue("la actualización debería ser exitosa");
            _huespedCreado.Should().NotBeNull("el huésped actualizado no debe ser null");
        }

        // --------------------------
        // THEN - SELECT helper (consulta por documento)
        // --------------------------
        [Then(@"debo poder consultar el huésped por su documento ""(.*)""")]
        public async Task EntoncesDeboPoderConsultarElHuespedPorSuDocumento(string documento)
        {
            var lista = await _huespedService!.GetAllAsync();
            lista.Should().Contain(
                h => h.Documento_Identidad == documento,
                $"debe existir un huésped con documento {documento}"
            );
        }

        // --------------------------
        // THEN - DELETE
        // --------------------------
        [Then(@"el huésped no debe existir en el sistema")]
        public async Task EntoncesElHuespedNoDebeExistirEnElSistema()
        {
            _operacionExitosa.Should().BeTrue("la eliminación debería ser exitosa");

            var lista = await _huespedService!.GetAllAsync();
            if (_context.HuespedId != null)
            {
                lista.Should().NotContain(h => h.ID == _context.HuespedId);
            }
        }

        // --------------------------
        // THEN - Unhappy (create/update)
        // --------------------------
        [Then(@"la creación de huésped debe fallar")]
        public void EntoncesLaCreacionDeHuespedDebeFallar()
        {
            _operacionExitosa.Should().BeFalse("la operación debería fallar por validación");
        }

        [Then(@"el error de huésped debe indicar ""(.*)""")]
        public void EntoncesElErrorDeHuespedDebeIndicar(string textoError)
        {
            _mensajeError.Should().NotBeNull("debe existir un mensaje de error");
            _mensajeError!
                .ToLower()
                .Should()
                .Contain(textoError.ToLower(), $"el mensaje debería mencionar {textoError}");
        }

        // --------------------------
        // Helper: formatea ValidationException
        // --------------------------
        private static string CollectValidationMessage(ValidationException vex)
        {
            if (vex.Errors != null && vex.Errors.Any())
            {
                return string.Join(" | ",
                    vex.Errors.Select(kvp => $"{kvp.Key}: {string.Join(", ", kvp.Value)}")
                );
            }

            return vex.Message;
        }
        // --------------------------
        // THEN - SELECT helper (verifica que el huésped exista)
        // --------------------------
        [Then(@"el huésped debe existir en el sistema")]
        public async Task EntoncesElHuespedDebeExistirEnElSistema()
        {
            _context.HuespedId.Should().NotBeNull("debe existir un huésped en el contexto");

            var lista = await _huespedService!.GetAllAsync();
            lista.Should().Contain(
                h => h.ID == _context.HuespedId,
                "el huésped debe existir en la base de datos"
            );
        }

    }
}
