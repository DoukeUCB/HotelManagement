using HotelManagement.Datos.Config;
using HotelManagement.DTOs;
using Microsoft.EntityFrameworkCore;

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

            var options = new DbContextOptionsBuilder<HotelDbContext>()
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .Options;

            return new HotelDbContext(options);
        }

        public static async Task LimpiarDatos(HotelDbContext context)
        {
            // Limpieza con SQL directo para evitar problemas de mapeo
            // El orden es importante por las FK
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Detalle_Reserva WHERE 1=1");
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Reserva WHERE 1=1");
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Huesped WHERE Documento_Identidad LIKE '%TEST%'");
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Habitacion WHERE Numero_Habitacion IN ('101', '102', '999')");
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Tipo_Habitacion WHERE Nombre = 'Suite Test'");
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Cliente WHERE Email LIKE '%test%'");
        }
    }
}
