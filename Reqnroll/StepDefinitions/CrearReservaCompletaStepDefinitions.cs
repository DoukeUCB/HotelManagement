using Reqnroll;
using FluentAssertions;
using HotelManagement.DTOs;
using HotelManagement.Models;
using HotelManagement.Services;
using HotelManagement.Application.Services;
using HotelManagement.Repositories;
using HotelManagement.Datos.Repositories;
using Reqnroll.Support;
using Microsoft.EntityFrameworkCore;

namespace Reqnroll.StepDefinitions
{
    [Binding]
    public class CrearReservaCompletaStepDefinitions
    {
        private readonly HotelTestContext _context;
        
        // Servicios
        private IClienteService? _clienteService;
        private IReservaService? _reservaService;
        private IDetalleReservaService? _detalleReservaService;
        
        // Estado del flujo de creación
        private string? _clienteSeleccionadoId;
        private string? _estadoReserva;
        private decimal _montoTotal;
        private List<DetalleHabitacionDTO> _habitacionesAgregadas = new();
        
        // Datos de prueba
        private string? _tipoHabitacionId;
        private string? _habitacionId;
        private string? _huespedId;
        
        // Resultados
        private ReservaDTO? _reservaCreada;
        private List<DetalleReservaDTO>? _detallesReserva;
        private bool _operacionExitosa;

        public CrearReservaCompletaStepDefinitions(HotelTestContext context)
        {
            _context = context;
        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            _context.DbContext = TestDatabaseHelper.CreateTestDbContext();
            await TestDatabaseHelper.LimpiarDatos(_context.DbContext);
            
            // Inicializar repositorios
            var clienteRepo = new ClienteRepository(_context.DbContext);
            var reservaRepo = new ReservaRepository(_context.DbContext);
            var detalleRepo = new DetalleReservaRepository(_context.DbContext);
            
            // Inicializar servicios
            _clienteService = new ClienteService(
                clienteRepo,
                new HotelManagement.Aplicacion.Validators.ClienteValidator(_context.DbContext)
            );
            
            _reservaService = new ReservaService(reservaRepo, clienteRepo, _context.DbContext);
            
            _detalleReservaService = new DetalleReservaService(
                detalleRepo,
                new HotelManagement.Aplicacion.Validators.DetalleReservaValidator(_context.DbContext)
            );
            
            // Reset estado
            _habitacionesAgregadas = new List<DetalleHabitacionDTO>();
            _operacionExitosa = false;
            _context.MensajeError = null;
        }

        [AfterScenario]
        public void AfterScenario()
        {
            _context.DbContext?.Dispose();
        }

        #region Antecedentes - Setup de datos de prueba

        [Given(@"que existen los siguientes datos de prueba:")]
        public async Task DadoQueExistenLosSiguientesDatosDePrueba(Table table)
        {
            // Crear Cliente
            var clienteDto = new ClienteCreateDTO
            {
                Razon_Social = table.Rows.First(r => r["Entidad"] == "Cliente" && r["Campo"] == "Razon_Social")["Valor"],
                NIT = table.Rows.First(r => r["Entidad"] == "Cliente" && r["Campo"] == "NIT")["Valor"],
                Email = table.Rows.First(r => r["Entidad"] == "Cliente" && r["Campo"] == "Email")["Valor"]
            };
            var cliente = await _clienteService!.CreateAsync(clienteDto);
            _context.ClienteId = cliente.ID;

            // Crear TipoHabitacion
            var tipoHab = new TipoHabitacion
            {
                ID = Guid.NewGuid().ToByteArray(),
                Nombre = table.Rows.First(r => r["Entidad"] == "TipoHabitacion" && r["Campo"] == "Nombre")["Valor"],
                Capacidad_Maxima = byte.Parse(table.Rows.First(r => r["Entidad"] == "TipoHabitacion" && r["Campo"] == "Capacidad_Maxima")["Valor"]),
                Precio_Base = decimal.Parse(table.Rows.First(r => r["Entidad"] == "TipoHabitacion" && r["Campo"] == "Precio_Base")["Valor"]),
                Activo = true,
                Fecha_Creacion = DateTime.Now,
                Fecha_Actualizacion = DateTime.Now
            };
            _context.DbContext!.TipoHabitaciones.Add(tipoHab);
            await _context.DbContext.SaveChangesAsync();
            _tipoHabitacionId = new Guid(tipoHab.ID).ToString();

            // Crear Habitacion 101
            var hab101 = new Habitacion
            {
                ID = Guid.NewGuid().ToByteArray(),
                Tipo_Habitacion_ID = tipoHab.ID,
                Numero_Habitacion = table.Rows.First(r => r["Entidad"] == "Habitacion" && r["Campo"] == "Numero_Habitacion")["Valor"],
                Piso = short.Parse(table.Rows.First(r => r["Entidad"] == "Habitacion" && r["Campo"] == "Piso")["Valor"]),
                Estado_Habitacion = "Libre",
                Fecha_Creacion = DateTime.Now,
                Fecha_Actualizacion = DateTime.Now
            };
            _context.DbContext.Habitaciones.Add(hab101);
            
            // Crear Habitacion 102 adicional para tests de múltiples habitaciones
            var hab102 = new Habitacion
            {
                ID = Guid.NewGuid().ToByteArray(),
                Tipo_Habitacion_ID = tipoHab.ID,
                Numero_Habitacion = "102",
                Piso = 1,
                Estado_Habitacion = "Libre",
                Fecha_Creacion = DateTime.Now,
                Fecha_Actualizacion = DateTime.Now
            };
            _context.DbContext.Habitaciones.Add(hab102);
            await _context.DbContext.SaveChangesAsync();
            _habitacionId = new Guid(hab101.ID).ToString();

            // Crear Huesped
            var huesped = new Huesped
            {
                ID = Guid.NewGuid().ToByteArray(),
                Nombre = table.Rows.First(r => r["Entidad"] == "Huesped" && r["Campo"] == "Nombre")["Valor"],
                Apellido = table.Rows.First(r => r["Entidad"] == "Huesped" && r["Campo"] == "Apellido")["Valor"],
                Documento_Identidad = table.Rows.First(r => r["Entidad"] == "Huesped" && r["Campo"] == "Documento")["Valor"],
                Activo = true,
                Fecha_Creacion = DateTime.Now,
                Fecha_Actualizacion = DateTime.Now
            };
            _context.DbContext.Huespedes.Add(huesped);
            await _context.DbContext.SaveChangesAsync();
            _huespedId = new Guid(huesped.ID).ToString();
        }

        #endregion

        #region PASO 1: Cliente

        [Given(@"que selecciono el cliente ""(.*)""")]
        public async Task DadoQueSeleccionoElCliente(string razonSocial)
        {
            var clientes = await _clienteService!.GetAllAsync();
            var cliente = clientes.FirstOrDefault(c => c.Razon_Social == razonSocial);
            cliente.Should().NotBeNull($"El cliente '{razonSocial}' debe existir");
            _clienteSeleccionadoId = cliente!.ID;
        }

        [Given(@"el estado de reserva es ""(.*)""")]
        public void DadoElEstadoDeReservaEs(string estado)
        {
            _estadoReserva = estado;
        }

        [Given(@"que intento crear una reserva con cliente inexistente ""(.*)""")]
        public void DadoQueIntentoCrearUnaReservaConClienteInexistente(string clienteId)
        {
            _clienteSeleccionadoId = clienteId;
            _estadoReserva = "Pendiente";
        }

        #endregion

        #region PASO 2: Habitaciones

        [When(@"agrego la habitación ""(.*)"" con los siguientes datos:")]
        public async Task CuandoAgregoLaHabitacionConLosSiguientesDatos(string numeroHabitacion, Table table)
        {
            var habitacion = await _context.DbContext!.Habitaciones
                .FirstOrDefaultAsync(h => h.Numero_Habitacion == numeroHabitacion);
            
            if (habitacion == null)
            {
                _operacionExitosa = false;
                _context.MensajeError = $"No se encontró la habitación {numeroHabitacion}";
                return;
            }

            var row = table.Rows[0];
            var detalle = new DetalleHabitacionDTO
            {
                Habitacion_ID = new Guid(habitacion.ID).ToString(),
                Fecha_Entrada = DateTime.Parse(row["Fecha_Entrada"]),
                Fecha_Salida = DateTime.Parse(row["Fecha_Salida"]),
                Huesped_IDs = new List<string> { _huespedId! }
            };

            _habitacionesAgregadas.Add(detalle);
        }

        [When(@"agrego las siguientes habitaciones:")]
        public async Task CuandoAgregoLasSiguientesHabitaciones(Table table)
        {
            foreach (var row in table.Rows)
            {
                var habitacion = await _context.DbContext!.Habitaciones
                    .FirstOrDefaultAsync(h => h.Numero_Habitacion == row["Habitacion"]);
                
                if (habitacion != null)
                {
                    var detalle = new DetalleHabitacionDTO
                    {
                        Habitacion_ID = new Guid(habitacion.ID).ToString(),
                        Fecha_Entrada = DateTime.Parse(row["Fecha_Entrada"]),
                        Fecha_Salida = DateTime.Parse(row["Fecha_Salida"]),
                        Huesped_IDs = new List<string> { _huespedId! }
                    };
                    _habitacionesAgregadas.Add(detalle);
                }
            }
        }

        [When(@"intento agregar la habitación inexistente ""(.*)""")]
        public async Task CuandoIntentoAgregarLaHabitacionInexistente(string numeroHabitacion)
        {
            var habitacion = await _context.DbContext!.Habitaciones
                .FirstOrDefaultAsync(h => h.Numero_Habitacion == numeroHabitacion);
            
            if (habitacion == null)
            {
                _operacionExitosa = false;
                _context.MensajeError = $"No se encontró la habitación {numeroHabitacion}";
            }
        }

        [When(@"agrego la habitación ""(.*)"" con fechas inválidas:")]
        public async Task CuandoAgregoLaHabitacionConFechasInvalidas(string numeroHabitacion, Table table)
        {
            var habitacion = await _context.DbContext!.Habitaciones
                .FirstOrDefaultAsync(h => h.Numero_Habitacion == numeroHabitacion);
            
            var row = table.Rows[0];
            var fechaEntrada = DateTime.Parse(row["Fecha_Entrada"]);
            var fechaSalida = DateTime.Parse(row["Fecha_Salida"]);

            if (fechaSalida <= fechaEntrada)
            {
                _operacionExitosa = false;
                _context.MensajeError = "La fecha de salida debe ser posterior a la fecha de entrada";
            }
        }

        #endregion

        #region PASO 3: Confirmación

        [When(@"confirmo la reserva con monto total de (.*)")]
        public async Task CuandoConfirmoLaReservaConMontoTotalDe(decimal monto)
        {
            _montoTotal = monto;

            try
            {
                // Crear la reserva
                var reservaDto = new ReservaCreateDTO
                {
                    Cliente_ID = _clienteSeleccionadoId!,
                    Estado_Reserva = _estadoReserva ?? "Pendiente",
                    Monto_Total = _montoTotal
                };

                await _reservaService!.AddAsync(reservaDto);
                
                // Obtener la reserva creada
                var reservas = await _reservaService.GetAllAsync();
                _reservaCreada = reservas.LastOrDefault();
                
                if (_reservaCreada != null && _habitacionesAgregadas.Any())
                {
                    // Crear los detalles de reserva
                    foreach (var hab in _habitacionesAgregadas)
                    {
                        foreach (var huespedId in hab.Huesped_IDs)
                        {
                            var detalleDto = new DetalleReservaCreateDTO
                            {
                                Reserva_ID = _reservaCreada.ID,
                                Habitacion_ID = hab.Habitacion_ID,
                                Huesped_ID = huespedId,
                                Fecha_Entrada = hab.Fecha_Entrada,
                                Fecha_Salida = hab.Fecha_Salida
                            };
                            await _detalleReservaService!.CreateAsync(detalleDto);
                        }
                    }
                    
                    // Obtener los detalles creados
                    _detallesReserva = await _detalleReservaService!.GetByReservaIdAsync(_reservaCreada.ID);
                }
                
                _operacionExitosa = true;
            }
            catch (Exception ex)
            {
                _operacionExitosa = false;
                _context.MensajeError = ex.Message;
            }
        }

        #endregion

        #region Happy Path - Consultar Reserva

        [Given(@"que existe una reserva completa en el sistema")]
        public async Task DadoQueExisteUnaReservaCompletaEnElSistema()
        {
            // Crear la reserva
            var reservaDto = new ReservaCreateDTO
            {
                Cliente_ID = _context.ClienteId!,
                Estado_Reserva = "Confirmada",
                Monto_Total = 750.00m
            };
            await _reservaService!.AddAsync(reservaDto);
            
            var reservas = await _reservaService.GetAllAsync();
            _reservaCreada = reservas.LastOrDefault();
            _context.ReservaId = _reservaCreada?.ID;

            // Crear detalle de reserva
            if (_reservaCreada != null)
            {
                var detalleDto = new DetalleReservaCreateDTO
                {
                    Reserva_ID = _reservaCreada.ID,
                    Habitacion_ID = _habitacionId!,
                    Huesped_ID = _huespedId!,
                    Fecha_Entrada = DateTime.Now.AddDays(1),
                    Fecha_Salida = DateTime.Now.AddDays(5)
                };
                await _detalleReservaService!.CreateAsync(detalleDto);
            }
        }

        [When(@"consulto la reserva por su ID")]
        public async Task CuandoConsultoLaReservaPorSuID()
        {
            try
            {
                var reservaId = _context.ReservaId ?? _reservaCreada?.ID;
                if (reservaId != null)
                {
                    _reservaCreada = await _reservaService!.GetByIdAsync(Guid.Parse(reservaId));
                    _detallesReserva = await _detalleReservaService!.GetByReservaIdAsync(reservaId);
                    _operacionExitosa = _reservaCreada != null;
                }
            }
            catch (Exception ex)
            {
                _operacionExitosa = false;
                _context.MensajeError = ex.Message;
            }
        }

        #endregion

        #region Verificaciones Happy Path

        [Then(@"la reserva debe crearse exitosamente")]
        public void EntoncesLaReservaDebeCrearseExitosamente()
        {
            _operacionExitosa.Should().BeTrue("la reserva debería haberse creado exitosamente");
            _reservaCreada.Should().NotBeNull("la reserva creada no debe ser null");
        }

        [Then(@"debe contener (.*) detalle de reserva")]
        [Then(@"debe contener (.*) detalles de reserva")]
        public void EntoncesDebeContenerDetallesDeReserva(int cantidad)
        {
            _detallesReserva.Should().NotBeNull();
            _detallesReserva!.Count.Should().Be(cantidad);
        }

        [Then(@"el detalle debe tener la habitación ""(.*)""")]
        public void EntoncesElDetalleDebeTenerLaHabitacion(string numeroHabitacion)
        {
            _detallesReserva.Should().Contain(d => d.Numero_Habitacion == numeroHabitacion);
        }

        [Then(@"el detalle debe tener el huésped ""(.*)""")]
        public void EntoncesElDetalleDebeTenerElHuesped(string nombreHuesped)
        {
            var nombreBuscado = nombreHuesped.Split(' ')[0];
            _detallesReserva.Should().Contain(d => d.Nombre_Huesped != null && d.Nombre_Huesped.Contains(nombreBuscado));
        }

        [Then(@"debo obtener la información del cliente")]
        public void EntoncesDeboObtenerLaInformacionDelCliente()
        {
            _reservaCreada.Should().NotBeNull();
            _reservaCreada!.Cliente_Nombre.Should().NotBeNullOrEmpty();
        }

        [Then(@"debo obtener los detalles de habitaciones")]
        public void EntoncesDeboObtenerLosDetallesDeHabitaciones()
        {
            _detallesReserva.Should().NotBeNull();
            _detallesReserva!.Should().NotBeEmpty();
        }

        [Then(@"debo obtener el monto total")]
        public void EntoncesDeboObtenerElMontoTotal()
        {
            _reservaCreada.Should().NotBeNull();
            _reservaCreada!.Monto_Total.Should().BeGreaterThan(0);
        }

        #endregion

        #region Verificaciones Unhappy Path

        [Then(@"la creación debe fallar")]
        [Then(@"la operación debe fallar")]
        public void EntoncesLaOperacionDebeFallar()
        {
            _operacionExitosa.Should().BeFalse("la operación debería haber fallado");
        }

        [Then(@"el error debe indicar ""(.*)""")]
        public void EntoncesElErrorDebeIndicar(string textoError)
        {
            _context.MensajeError.Should().NotBeNull("debe existir un mensaje de error");
            _context.MensajeError!.ToLower().Should().Contain(textoError.ToLower());
        }

        #endregion
    }
}
