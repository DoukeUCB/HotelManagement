using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HotelManagement.Datos.Config;
using HotelManagement.DTOs;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Reqnroll.Support
{
   
    public class HotelTestContext
    {
        public HotelDbContext? DbContext { get; set; }

        // Reserva / Cliente (ya los usaba tu compa)
        public Exception? UltimoError { get; set; }
        public string? MensajeError { get; set; }
        public int? CodigoEstadoHttp { get; set; }
        public bool OperacionExitosa { get; set; }

        // --- Propiedades para Reserva ---
        public string? ClienteId { get; set; }
        public string? ReservaId { get; set; }
        public ReservaDTO? ReservaCreada { get; set; }
        public ReservaDTO? ReservaConsultada { get; set; }
        public IEnumerable<ReservaDTO>? ListaReservas { get; set; }

        // Huesped (para tus escenarios)
        public string? HuespedId { get; set; }
        public HuespedDTO? HuespedCreado { get; set; }
        public IEnumerable<HuespedDTO>? ListaHuespedes { get; set; }

        public Exception? UltimoError { get; set; }
        public string? MensajeError { get; set; }
        public int? CodigoEstadoHttp { get; set; }
        public bool OperacionExitosa { get; set; }


        // --- Propiedades para Habitacion ---
        public string? TipoHabitacionId { get; set; } // ID del TipoHabitacion usado en el test
        public string? HabitacionId { get; set; } // ID de la Habitacion de la que se está haciendo CRUD
        public HabitacionDTO? HabitacionCreada { get; set; } // DTO de la Habitacion
        
        public void Reset()
        {
            // Reset Reserva
            ClienteId = null;
            ReservaId = null;
            ReservaCreada = null;
            ReservaConsultada = null;
            ListaReservas = null;

            HuespedId = null;
            HuespedCreado = null;
            ListaHuespedes = null;


            // Reset Habitacion
            TipoHabitacionId = null;
            HabitacionId = null;
            HabitacionCreada = null;

            // Reset Shared
            UltimoError = null;
            MensajeError = null;
            CodigoEstadoHttp = null;
            OperacionExitosa = false;
        }
    }

    /// <summary>
    /// Helper para configuración de base de datos de pruebas (UNIFICADO)
    /// </summary>
    public static class TestDatabaseHelper
    {
        public static HotelDbContext CreateTestDbContext()
        {
            // Lógica para cargar .env y crear el DbContext
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
            if (context == null) return;

            // Orden importante por FKs
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
            // Limpieza general de datos de RESERVA (ordenado por FK)
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Detalle_Reserva WHERE 1=1");
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Reserva WHERE 1=1");

            // -------------------------
            // LIMPIEZA SEGURA DE HUESPED
            // -------------------------
            // No borrar todos los huéspedes: solo los que usamos en tests (lista blanca)
            // y aquellos que tengan 'TEST' en Documento_Identidad.

            // Lista de documentos usados en los features (añade o quita según tus features)
            var testDocs = new[]
            {
                "12345678", "22222222", "33333333", "44444444", "55555555",
                "66666666", "77777777", "88888888", "99999999", "11111111"
            };

            // Construir IN clause seguro (todos son números en los features)
            var docsInClause = string.Join(",", testDocs.Select(d => $"'{d}'"));

            // Si estamos en MySQL, deshabilitar FK checks temporalmente
            try
            {
                await context.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 0;");
            }
            catch
            {
                // Si no soporta (por ejemplo in-memory), ignoramos
            }

            // Eliminar huéspedes con Documento_Identidad LIKE '%TEST%' (marca de prueba)
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Huesped WHERE Documento_Identidad LIKE '%TEST%'");

            // Eliminar huéspedes que están en la lista de documentos de prueba
            if (!string.IsNullOrEmpty(docsInClause))
            {
                await context.Database.ExecuteSqlRawAsync($"DELETE FROM Huesped WHERE Documento_Identidad IN ({docsInClause})");
            }

            // -------------------------
            // Limpiar las habitaciones/tipos/clientes de prueba (igual que antes)
            // -------------------------
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Habitacion WHERE Numero_Habitacion IN ('101', '102', '999')");
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Huesped WHERE 1=1");
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Cliente WHERE Email LIKE '%test%'");

            // Limpieza de datos de HABITACION (incluyendo los de la feature de Reserva y Habitacion)
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Habitacion WHERE Numero_Habitacion IN ('101', '102', '999', '105', '201', '305')");
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Tipo_Habitacion WHERE Nombre = 'Suite Test'");
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Cliente WHERE Email LIKE '%test%'");

            // Reactivar FK checks si fue deshabilitado
            try
            {
                await context.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 1;");
            }
            catch
            {
                // ignorar si no aplica
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