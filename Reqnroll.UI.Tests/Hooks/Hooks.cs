using Reqnroll;
using MySqlConnector;

namespace Reqnroll.UI.Tests.Hooks
{
    [Binding]
    public class Hooks
    {
        private const string ConnectionString = "Server=localhost;Database=HotelDB;User Id=root;Password=";

        [AfterScenario("@creacion")]
        public void LimpiarHabitacionesDePrueba()
        {
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        // Eliminar habitaciones que comienzan con "40" o "401" (usadas en tests)
                        command.CommandText = "DELETE FROM Habitacion WHERE Numero_Habitacion LIKE '40%'";
                        command.ExecuteNonQuery();
                        
                        Console.WriteLine("✓ Habitaciones de prueba eliminadas");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠ Error limpiando habitaciones: {ex.Message}");
            }
        }
    }
}
