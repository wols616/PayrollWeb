using iText.StyledXmlParser.Jsoup.Select;
using Microsoft.Data.SqlClient;

namespace PayrollWeb.Models
{
    public class Puesto_Historico
    {
        public int IdPuestoHistorico { get; set; }
        public string NombrePuesto { get; set; }
        public decimal SueldoBase { get; set; }
        public string NombreCategoria { get; set; }
        public int IdContrato { get; set; }
        public Contrato Contrato { get; set; }

        Conexion conexion = new Conexion();

        public Puesto_Historico ObtenerPuestoHistorico(int IdContrato)
        {
            Puesto_Historico puestoHistorico = new Puesto_Historico();
            string query = "SELECT id_puesto_historico, nombre_puesto, sueldo_base, nombre_categoria, id_contrato FROM Puesto_Historico WHERE id_contrato = @IdContrato";
            using (SqlConnection connection = conexion.GetConnection())
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdContrato", IdContrato);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            puestoHistorico.IdPuestoHistorico = reader.GetInt32(0);
                            puestoHistorico.NombrePuesto = reader.GetString(1);
                            puestoHistorico.SueldoBase = reader.GetDecimal(2);
                            puestoHistorico.NombreCategoria = reader.GetString(3);
                            puestoHistorico.IdContrato = reader.GetInt32(4);
                            puestoHistorico.Contrato = new Contrato().ObtenerContrato(puestoHistorico.IdContrato);
                        }
                    }
                }
            }
            return puestoHistorico;
        }

        public bool RegistrarPuestoHistorico(int idContrato)
        {
            try
            {
                using (SqlConnection connection = conexion.GetConnection())
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("sp_RegistrarPuestoHistorico", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        // Agregar parámetro
                        command.Parameters.Add(new SqlParameter("@id_contrato", idContrato));

                        // Ejecutar el procedimiento
                        int rowsAffected = command.ExecuteNonQuery();

                        // Si se afectaron filas, se insertó el registro histórico
                        return rowsAffected > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                // Loggear el error (puedes usar ILogger, Console, etc.)
                Console.WriteLine($"Error al registrar puesto histórico: {ex.Message}");
                return false;
            }
        }
    }
}
