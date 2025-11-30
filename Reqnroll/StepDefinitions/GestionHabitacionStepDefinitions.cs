using Reqnroll;
using FluentAssertions;
using HotelManagement.DTOs;
using HotelManagement.Models;
using HotelManagement.Application.Services;
using HotelManagement.Services;
using HotelManagement.Repositories;
using HotelManagement.Datos.Config; 
using HotelManagement.Aplicacion.Validators;
using Reqnroll.Support;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;



namespace Reqnroll.StepDefinitions
{
    [Binding]
    public class GestionHabitacionStepDefinitions
    {
        private readonly HotelTestContext _context; // Usamos el contexto renombrado
        
        // Servicios
        private IHabitacionService? _habitacionService;
        private IHabitacionRepository? _habitacionRepository;
        
        // Estado del flujo de creación
        private string? _tipoHabitacionId;
        private string? _habitacionCreadaId;
        private HabitacionDTO? _habitacionConsultada;
        
        // Resultados
        private bool _operacionExitosa;
        private string? _mensajeError;

        public GestionHabitacionStepDefinitions(HotelTestContext context)
        {
            _context = context;
        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            _context.DbContext = TestDatabaseHelper.CreateTestDbContext();
            await TestDatabaseHelper.LimpiarDatos(_context.DbContext);
            
            // Inicializar Repositorios
            // Asumiendo que HabitacionRepository implementa IHabitacionRepository
            _habitacionRepository = new HabitacionRepository(_context.DbContext);
            
            // Inicializar Servicios
            // Nota: Se debe inyectar la dependencia de IHabitacionRepository en el Validator si es necesario
            var habitacionValidator = new HabitacionValidator(_context.DbContext);
            _habitacionService = new HabitacionService(_habitacionRepository, habitacionValidator);
            
            // Reset estado
            _habitacionCreadaId = null;
            _habitacionConsultada = null;
            _operacionExitosa = false;
            _mensajeError = null;
        }

        [AfterScenario]
        public void AfterScenario()
        {
            _context.DbContext?.Dispose();
        }

        #region Antecedentes

        [Given(@"que existe un Tipo de Habitación para pruebas:")]
        public async Task DadoQueExisteUnTipoDeHabitacionParaPruebas(Table table)
        {
            var row = table.Rows.First();
            var tipoHab = new TipoHabitacion
            {
                ID = Guid.NewGuid().ToByteArray(),
                Nombre = row["Nombre"],
                Capacidad_Maxima = byte.Parse(row["Capacidad_Maxima"]),
                Precio_Base = decimal.Parse(row["Precio_Base"]),
                Activo = true,
                Fecha_Creacion = DateTime.Now,
                Fecha_Actualizacion = DateTime.Now
            };
            _context.DbContext!.TipoHabitaciones.Add(tipoHab);
            await _context.DbContext.SaveChangesAsync();
            _tipoHabitacionId = new Guid(tipoHab.ID).ToString();
            _context.TipoHabitacionId = _tipoHabitacionId;
        }
        
        [Given(@"que el Tipo de Habitación para la prueba existe")]
        public void DadoQueElTipoDeHabitacionParaLaPruebaExiste()
        {
            _tipoHabitacionId.Should().NotBeNullOrEmpty("El TipoHabitacion debe haber sido creado en el Antecedente.");
        }

        [Given(@"que ya existe una habitación con el número ""(.*)""")]
        public async Task DadoQueYaExisteUnaHabitacionConElNumero(string numeroHabitacion)
        {
            var hab = new Habitacion
            {
                ID = Guid.NewGuid().ToByteArray(),
                // Se asume que _tipoHabitacionId fue seteado en el Antecedentes anterior
                Tipo_Habitacion_ID = Guid.Parse(_tipoHabitacionId!).ToByteArray(), 
                Numero_Habitacion = numeroHabitacion,
                Piso = 1,
                Estado_Habitacion = "Libre",
                Fecha_Creacion = DateTime.Now,
                Fecha_Actualizacion = DateTime.Now
            };
            _context.DbContext!.Habitaciones.Add(hab);
            await _context.DbContext.SaveChangesAsync();
            _habitacionCreadaId = new Guid(hab.ID).ToString();
        }

        [Given(@"que existe una habitación con número ""(.*)"" y piso ""(.*)"" en el sistema")]
        public async Task DadoQueExisteUnaHabitacionConNumeroYPisoEnElSistema(string numeroHabitacion, short piso)
        {
            await DadoQueYaExisteUnaHabitacionConElNumero(numeroHabitacion);
            var hab = await _context.DbContext!.Habitaciones.FirstAsync(h => new Guid(h.ID).ToString() == _habitacionCreadaId);
            hab.Piso = piso;
            await _context.DbContext.SaveChangesAsync();
        }

        [Given(@"que existe una habitación con número ""(.*)"" y estado ""(.*)"" en el sistema")]
        public async Task DadoQueExisteUnaHabitacionConNumeroYEstadoEnElSistema(string numeroHabitacion, string estado)
        {
            await DadoQueYaExisteUnaHabitacionConElNumero(numeroHabitacion);
            var hab = await _context.DbContext!.Habitaciones.FirstAsync(h => new Guid(h.ID).ToString() == _habitacionCreadaId);
            hab.Estado_Habitacion = estado;
            await _context.DbContext.SaveChangesAsync();
        }

        [Given(@"que el sistema no contiene ninguna habitación con ID ""(.*)""")]
        public void DadoQueElSistemaNoContieneNingunaHabitacionConID(string id)
        {
            _habitacionCreadaId = id; 
        }

        #endregion

        #region Pasos (When)

        [When(@"creo una nueva habitación con los siguientes datos:")]
        public async Task CuandoCreoUnaNuevaHabitacionConLosSiguientesDatos(Table table)
        {
            var row = table.Rows.First();
            var dto = new HabitacionCreateDTO
            {
                Tipo_Habitacion_ID = _tipoHabitacionId!,
                Numero_Habitacion = row["Numero_Habitacion"],
                Piso = short.Parse(row["Piso"]),
                Estado_Habitacion = row["Estado_Habitacion"]
            };

            try
            {
                _habitacionConsultada = await _habitacionService!.CreateAsync(dto);
                _habitacionCreadaId = _habitacionConsultada.ID;
                _operacionExitosa = true;
            }
            catch (Exception ex)
            {
                _operacionExitosa = false;
                _mensajeError = ex.Message;
            }
        }
        
        [When(@"intento crear otra habitación con el número ""(.*)""")]
        public async Task CuandoIntentoCrearOtraHabitacionConElNumero(string numeroHabitacion)
        {
            var dto = new HabitacionCreateDTO
            {
                Tipo_Habitacion_ID = _tipoHabitacionId!,
                Numero_Habitacion = numeroHabitacion,
                Piso = 1,
                Estado_Habitacion = "Libre"
            };

            try
            {
                await _habitacionService!.CreateAsync(dto);
                _operacionExitosa = true;
            }
            catch (Exception ex)
            {
                _operacionExitosa = false;
                _mensajeError = ex.Message;
            }
        }

        [When(@"consulto la habitación por su ID")]
        public async Task CuandoConsultoLaHabitacionPorSuID()
        {
            _habitacionCreadaId.Should().NotBeNullOrEmpty("Se necesita un ID de habitación para consultar.");
            try
            {
                _habitacionConsultada = await _habitacionService!.GetByIdAsync(_habitacionCreadaId!);
                _operacionExitosa = _habitacionConsultada != null;
            }
            catch (Exception ex)
            {
                _operacionExitosa = false;
                _mensajeError = ex.Message;
            }
        }

        [When(@"actualizo el estado de la habitación a ""(.*)""")]
        public async Task CuandoActualizoElEstadoDeLaHabitacionA(string nuevoEstado)
        {
            _habitacionCreadaId.Should().NotBeNullOrEmpty("Se necesita un ID de habitación para actualizar.");

            var dto = new HabitacionUpdateDTO
            {
                Estado_Habitacion = nuevoEstado
            };

            try
            {
                // Asumiendo un método PartialUpdateAsync en IHabitacionService
                _habitacionConsultada = await _habitacionService!.PartialUpdateAsync(_habitacionCreadaId!, dto); 
                _operacionExitosa = true;
            }
            catch (Exception ex)
            {
                _operacionExitosa = false;
                _mensajeError = ex.Message;
            }
        }

        [When(@"intento crear una nueva habitación con Tipo de Habitación inválido")]
        public async Task CuandoIntentoCrearUnaNuevaHabitacionConTipoDeHabitacionInvalido()
        {
            var dto = new HabitacionCreateDTO
            {
                Tipo_Habitacion_ID = Guid.Empty.ToString(), // ID inválido
                Numero_Habitacion = "100",
                Piso = 1,
                Estado_Habitacion = "Libre"
            };

            try
            {
                await _habitacionService!.CreateAsync(dto);
                _operacionExitosa = true;
            }
            catch (Exception ex)
            {
                _operacionExitosa = false;
                _mensajeError = ex.Message;
            }
        }

        [When(@"intento eliminar esa habitación")]
        public async Task CuandoIntentoEliminarEsaHabitacion()
        {
            _habitacionCreadaId.Should().NotBeNullOrEmpty("Se necesita un ID para intentar eliminar.");
            try
            {
                // Asumiendo un método DeleteAsync en IHabitacionService
                _operacionExitosa = await _habitacionService!.DeleteAsync(_habitacionCreadaId!);
            }
            catch (Exception ex)
            {
                _operacionExitosa = false;
                _mensajeError = ex.Message;
            }
        }

        #endregion

        #region Verificaciones (Then)

        [Then(@"la habitación debe crearse exitosamente")]
        [Then(@"la habitación debe actualizarse exitosamente")]
        public void EntoncesLaHabitacionDebeCrearseExitosamente()
        {
            _operacionExitosa.Should().BeTrue("la operación debería haber sido exitosa.");
            _habitacionConsultada.Should().NotBeNull("debería haber un DTO de Habitación consultado/creado.");
        }

        [Then(@"debe tener el número de habitación ""(.*)""")]
        public void EntoncesDebeTenerElNumeroDeHabitacion(string numero)
        {
            _habitacionConsultada!.Numero_Habitacion.Should().Be(numero);
        }

        [Then(@"debo obtener la información de la habitación")]
        public void EntoncesDeboObtenerLaInformacionDeLaHabitacion()
        {
            _habitacionConsultada.Should().NotBeNull();
            _habitacionConsultada!.ID.Should().NotBeNullOrEmpty();
        }

        [Then(@"la habitación consultada debe tener el piso ""(.*)""")]
        public void EntoncesLaHabitacionConsultadaDebeTenerElPiso(short piso)
        {
            _habitacionConsultada!.Piso.Should().Be(piso);
        }

        [Then(@"su nuevo estado debe ser ""(.*)""")]
        public void EntoncesSuNuevoEstadoDebeSer(string estado)
        {
            _habitacionConsultada!.Estado_Habitacion.Should().Be(estado);
        }

        [Then(@"la creación debe fallar")]
        [Then(@"la operación debe fallar")]
        public void EntoncesLaOperacionDebeFallar()
        {
            _operacionExitosa.Should().BeFalse("la operación debería haber fallado");
        }

        [Then(@"el error debe indicar ""(.*)""")]
        public void EntoncesElErrorDebeIndicar(string textoError)
        {
            _mensajeError.Should().NotBeNullOrEmpty("debe existir un mensaje de error");
            _mensajeError!.ToLower().Should().Contain(textoError.ToLower());
        }

        #endregion
    }
}