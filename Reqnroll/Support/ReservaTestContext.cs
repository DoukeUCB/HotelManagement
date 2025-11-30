using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        // Reserva / Cliente (ya los usaba tu compa)
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
            if (context == null) return;

            // Orden importante por FKs
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
    }
}
