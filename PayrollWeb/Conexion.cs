using Microsoft.Data.SqlClient;
using System.Data;

namespace PayrollWeb
{
    public class Conexion
    {

        private readonly string connectionString = "Server=LOCALHOST;Database=payroll_web1;User Id=brenda;Password=Beds123;Encrypt=False;";

        // Método para obtener la conexión
        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        // Método para abrir la conexión
        public void OpenConnection(SqlConnection con)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al abrir la conexión: " + ex.Message);
            }
        }

        // Método para cerrar la conexión
        public void CloseConnection(SqlConnection con)
        {
            try
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al cerrar la conexión: " + ex.Message);
            }
        }
    }
}
