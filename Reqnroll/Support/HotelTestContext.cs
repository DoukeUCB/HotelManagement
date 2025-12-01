using System;
using System.Collections.Generic;
using System.IO;
using HotelManagement.Datos.Config;
using HotelManagement.DTOs;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace Reqnroll.Support
{
    public class HotelTestContext
    {
        public HotelDbContext? DbContext { get; set; }

        // Reservas y clientes
        public string? ClienteId { get; set; }
        public string? ReservaId { get; set; }
        public ReservaDTO? ReservaCreada { get; set; }
        public ReservaDTO? ReservaConsultada { get; set; }
        public IEnumerable<ReservaDTO>? ListaReservas { get; set; }

        // Huéspedes
        public string? HuespedId { get; set; }
        public HuespedDTO? HuespedCreado { get; set; }
        public IEnumerable<HuespedDTO>? ListaHuespedes { get; set; }

        // Habitaciones
        public string? TipoHabitacionId { get; set; }
        public string? HabitacionId { get; set; }
        public HabitacionDTO? HabitacionCreada { get; set; }

        // Estado compartido
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

            HuespedId = null;
            HuespedCreado = null;
            ListaHuespedes = null;

            TipoHabitacionId = null;
            HabitacionId = null;
            HabitacionCreada = null;

            UltimoError = null;
            MensajeError = null;
            CodigoEstadoHttp = null;
            OperacionExitosa = false;
        }
    }

    public static class TestDatabaseHelper
    {
        public static HotelDbContext CreateTestDbContext()
        {
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

            await context.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS=0;");

            try
            {
                // Remove rows from child tables first to avoid FK violations when running on a shared MySQL instance.
                await context.Database.ExecuteSqlRawAsync("DELETE FROM Detalle_Reserva");
                await context.Database.ExecuteSqlRawAsync("DELETE FROM Reserva");
                await context.Database.ExecuteSqlRawAsync("DELETE FROM Habitacion");
                await context.Database.ExecuteSqlRawAsync("DELETE FROM Tipo_Habitacion");
                await context.Database.ExecuteSqlRawAsync("DELETE FROM Huesped");
                await context.Database.ExecuteSqlRawAsync("DELETE FROM Cliente");
            }
            finally
            {
                await context.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS=1;");
            }
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