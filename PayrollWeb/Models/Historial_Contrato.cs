using Microsoft.Data.SqlClient;

namespace PayrollWeb.Models
{
    public class Historial_Contrato
    {
        public int IdHistorialContrato { get; set; }
        public int? IdContratoAnterior { get; set; }
        public int IdContratoNuevo { get; set; }
        public DateTime Fecha { get; set; }
        public string Cambio { get; set; }
        public string Motivo { get; set; }
        public int IdAdministrador { get; set; }
        public Contrato ContratoAnterior { get; set; }
        public Contrato ContratoNuevo { get; set; }
        public Administrador Administrador { get; set; }

        Conexion conexion = new Conexion();

        //MÉTODO PARA MOSTRAR EL HISTORIAL DE CONTRATOS
        public List<Historial_Contrato> ObtenerHistorialContratos()
        {
            List<Historial_Contrato> historialList = new List<Historial_Contrato>();

            // Consulta SQL para obtener todas las deducciones
            string query = "SELECT id_historial_contrato, id_contrato_anterior, id_contrato_nuevo, fecha, cambio, motivo, id_administrador FROM Historial_Contrato";

            Conexion conexion = new Conexion();

            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    // Abrir la conexión
                    connection.Open();

                    // Ejecutar la consulta
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Leer los datos de la base de datos
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Procesar cada fila y agregarla a la lista
                            while (reader.Read())
                            {
                                Historial_Contrato historial = new Historial_Contrato
                                {
                                    IdHistorialContrato = Convert.ToInt32(reader["id_historial_contrato"]),
                                    IdContratoAnterior = Convert.ToInt32(reader["id_contrato_anterior"]),
                                    IdContratoNuevo = Convert.ToInt32(reader["id_contrato_nuevo"]),
                                    Fecha = Convert.ToDateTime(reader["fecha"]),
                                    Cambio = reader["cambio"].ToString(),
                                    Motivo = reader["motivo"].ToString(),
                                    IdAdministrador = Convert.ToInt32(reader["id_administrador"]),
                                    ContratoAnterior = new Contrato().ObtenerContrato(Convert.ToInt32(reader["id_contrato_anterior"])),
                                    ContratoNuevo = new Contrato().ObtenerContrato(Convert.ToInt32(reader["id_contrato_nuevo"])),
                                    Administrador = new Administrador().ObtenerAdministrador(Convert.ToInt32(reader["id_administrador"]))
                                };

                                // Agregar el objeto Historial_Contrato a la lista
                                historialList.Add(historial);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener el historial de contratos: " + ex.Message);
                }
            }

            // Retornar la lista de historial de contratos
            return historialList;
        }

        //MÉTODO PARA AGREGAR UN NUEVO REGISTRO AL HISTORIAL DE CONTRATOS
        public bool AgregarHistorialContrato()
        {
            bool success = false;
            // Consulta SQL para insertar un nuevo registro en el historial de contratos
            string query = "INSERT INTO Historial_Contrato (id_contrato_anterior, id_contrato_nuevo, fecha, cambio, motivo, id_administrador) VALUES (@id_contrato_anterior, @id_contrato_nuevo, @fecha, @cambio, @motivo, @id_administrador)";
            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    // Abrir la conexión
                    connection.Open();
                    // Ejecutar la consulta
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Agregar los parámetros
                        command.Parameters.AddWithValue("@id_contrato_anterior", IdContratoAnterior);
                        command.Parameters.AddWithValue("@id_contrato_nuevo", IdContratoNuevo);
                        command.Parameters.AddWithValue("@fecha", Fecha);
                        command.Parameters.AddWithValue("@cambio", Cambio);
                        command.Parameters.AddWithValue("@motivo", Motivo);
                        command.Parameters.AddWithValue("@id_administrador", IdAdministrador);
                        // Ejecutar la consulta
                        command.ExecuteNonQuery();
                        // Indicar que la operación fue exitosa
                        success = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al agregar un nuevo registro al historial de contratos: " + ex.Message);
                }
            }
            // Retornar si la operación fue exitosa
            return success;
        }
    }
}
