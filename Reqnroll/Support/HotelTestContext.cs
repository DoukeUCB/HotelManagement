using HotelManagement.Datos.Config;
using HotelManagement.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Reqnroll.Support
{
   
    public class HotelTestContext
    {
        public HotelDbContext? DbContext { get; set; }
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

            var options = new DbContextOptionsBuilder<HotelDbContext>()
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .Options;

            return new HotelDbContext(options);
        }

        public static async Task LimpiarDatos(HotelDbContext context)
        {
            // Limpieza general de datos de RESERVA (ordenado por FK)
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Detalle_Reserva WHERE 1=1");
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Reserva WHERE 1=1");
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Huesped WHERE 1=1");
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Cliente WHERE Email LIKE '%test%'");

            // Limpieza de datos de HABITACION (incluyendo los de la feature de Reserva y Habitacion)
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Habitacion WHERE Numero_Habitacion IN ('101', '102', '999', '105', '201', '305')");
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Tipo_Habitacion WHERE Nombre = 'Suite Test'");
        }
    }
}