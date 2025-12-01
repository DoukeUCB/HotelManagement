using HotelManagement.Datos.Config;
using HotelManagement.DTOs;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace Reqnroll.Support
{
    /// <summary>
    /// Contexto compartido entre pasos para almacenar datos del escenario
    /// </summary>
    public class ReservaTestContext
    {
        public HotelDbContext? DbContext { get; set; }
        public string? ClienteId { get; set; }
        public string? ReservaId { get; set; }
        public ReservaDTO? ReservaCreada { get; set; }
        public ReservaDTO? ReservaConsultada { get; set; }
        public IEnumerable<ReservaDTO>? ListaReservas { get; set; }
        public Exception? UltimoError { get; set; }
        public string? MensajeError { get; set; }
        public int? CodigoEstadoHttp { get; set; }
        public bool OperacionExitosa { get; set; }

        public void Reset()
        {
            ClienteId = null;
            ReservaId = null;
            ReservaCreada = null;
            ReservaConsultada = null;
            ListaReservas = null;
            UltimoError = null;
            MensajeError = null;
            CodigoEstadoHttp = null;
            OperacionExitosa = false;
        }
    }

    /// <summary>
    /// Helper para configuración de base de datos de pruebas
    /// </summary>
    public static class TestDatabaseHelper
    {
        public static HotelDbContext CreateTestDbContext()
        {
            // Cargar .env desde el directorio raíz del proyecto
            var projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", ".."));
            var envPath = Path.Combine(projectRoot, ".env");
            
            if (File.Exists(envPath))
            {
                DotNetEnv.Env.Load(envPath);
            }

            var server = Environment.GetEnvironmentVariable("DB_SERVER") ?? "localhost";
            var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "3306";
            var database = Environment.GetEnvironmentVariable("DB_NAME") ?? "HotelDB";
            var user = Environment.GetEnvironmentVariable("DB_USER") ?? "root";
            var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "";

            var connectionString = $"Server={server};Port={port};Database={database};User={user};Password={password};";

            var useInMemory = string.Equals(Environment.GetEnvironmentVariable("USE_INMEMORY_DB"), "true", StringComparison.OrdinalIgnoreCase);

            if (useInMemory)
            {
                return CreateInMemoryContext();
            }

            try
            {
                EnsureDatabaseExists(server, port, database, user, password);

                var options = new DbContextOptionsBuilder<HotelDbContext>()
                    .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                    .Options;

                var context = new HotelDbContext(options);
                context.Database.EnsureCreated();
                return context;
            }
            catch (Exception ex) when (ex is MySqlException or InvalidOperationException or TimeoutException)
            {
                Console.WriteLine($"[WARN] No se pudo usar MySQL para las pruebas BDD ({ex.Message}). Se utilizará una base InMemory temporal.");
                return CreateInMemoryContext();
            }
        }

        public static async Task LimpiarDatos(HotelDbContext context)
        {
            if (context.Database.IsInMemory())
            {
                context.DetalleReservas.RemoveRange(context.DetalleReservas);
                context.Reservas.RemoveRange(context.Reservas);
                context.Huespedes.RemoveRange(context.Huespedes);
                context.Habitaciones.RemoveRange(context.Habitaciones);
                context.TipoHabitaciones.RemoveRange(context.TipoHabitaciones);
                context.Clientes.RemoveRange(context.Clientes);
                await context.SaveChangesAsync();
                return;
            }

            // Limpieza con SQL directo para evitar problemas de mapeo
            // El orden es importante por las FK
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Detalle_Reserva WHERE 1=1");
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Reserva WHERE 1=1");
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Huesped WHERE Documento_Identidad LIKE '%TEST%'");
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Habitacion WHERE Numero_Habitacion IN ('101', '102', '999')");
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Tipo_Habitacion WHERE Nombre = 'Suite Test'");
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Cliente WHERE Email LIKE '%test%'");
        }

        private static void EnsureDatabaseExists(string server, string port, string database, string user, string password)
        {
            var masterConnectionString = $"Server={server};Port={port};User={user};Password={password};";

            using var connection = new MySqlConnection(masterConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = $"CREATE DATABASE IF NOT EXISTS `{database}`;";
            command.ExecuteNonQuery();
        }

        private static HotelDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<HotelDbContext>()
                .UseInMemoryDatabase($"HotelManagementTests_{Guid.NewGuid():N}")
                .Options;

            var context = new HotelDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }
    }
}
