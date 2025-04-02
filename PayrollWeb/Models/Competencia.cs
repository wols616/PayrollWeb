using Microsoft.Data.SqlClient;

namespace PayrollWeb.Models
{
    public class Competencia
    {
        Conexion conexion = new Conexion();

        public int IdCompetencia { get; set; }
        public string Nombre { get; set; }

        public Competencia() { }

        public Competencia(string nombre)
        {
            this.Nombre = nombre;
        }

        public Competencia(int idCompetencia, string nombre)
        {
            this.IdCompetencia = idCompetencia;
            this.Nombre = nombre;
        }

        // Método para obtener todas las competencias
        public List<Competencia> ObtenerCompetencias()
        {
            List<Competencia> competenciasList = new List<Competencia>();

            // Consulta SQL para obtener todas las competencias
            string query = "SELECT * FROM Competencia";

            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Competencia competencia = new Competencia
                                {
                                    IdCompetencia = reader.GetInt32(0),
                                    Nombre = reader.GetString(1)        
                                };

                                competenciasList.Add(competencia);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener las competencias: " + ex.Message);
                }
            }
            return competenciasList;
        }

        // Método para obtener una competencia por su Id
        public Competencia ObtenerCompetencia(int idCompetencia)
        {
            Competencia competencia = new Competencia();

            // Consulta SQL para obtener una competencia por su Id
            string query = "SELECT * FROM Competencia WHERE id_competencia = @IdCompetencia";

            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    // Abrir la conexión
                    connection.Open();

                    // Crear el comando SQL y agregar los parámetros
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdCompetencia", idCompetencia);

                        // Ejecutar la consulta
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Procesar la fila y asignar los valores a la competencia
                            if (reader.Read())
                            {
                                competencia.IdCompetencia = reader.GetInt32(0);
                                competencia.Nombre = reader.GetString(1);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener la competencia: " + ex.Message);
                }
            }

            return competencia;
        }

        // Insertar nueva competencia
        //public bool AgregarCompetencia()
        //{
        //    bool exito = false;
        //    string query = "INSERT INTO Competencia (nombre) VALUES (@Nombre)";

        //    using (SqlConnection connection = conexion.GetConnection())
        //    {
        //        try
        //        {
        //            connection.Open();
        //            using (SqlCommand command = new SqlCommand(query, connection))
        //            {
        //                string nombrePrueba = "Pedro";
        //                command.Parameters.AddWithValue("@Nombre", nombrePrueba);

        //                int rowsAffected = command.ExecuteNonQuery();
        //                exito = rowsAffected > 0;

        //                Console.WriteLine($"Intentando insertar: {nombrePrueba}");
        //                Console.WriteLine($"Filas afectadas: {rowsAffected}");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("Error al agregar competencia: " + ex.Message);
        //        }
        //    }

        //    return exito;
        //}




        public bool AgregarCompetencia()
        {
            bool exito = false;
            string query = "INSERT INTO Competencia (nombre) VALUES (@Nombre)";

            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Nombre", Nombre);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            exito = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al agregar competencia: " + ex.Message);
                }
            }
            return exito;
        }

        // Actualizar competencia
        public bool EditarCompetencia()
        {
            bool exito = false;
            string query = "UPDATE Competencia SET nombre = @Nombre WHERE id_competencia = @IdCompetencia";

            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Nombre", Nombre);
                        command.Parameters.AddWithValue("@IdCompetencia", IdCompetencia);
                        int rowsAffected = command.ExecuteNonQuery();
                        exito = rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al actualizar competencia: " + ex.Message);
                }
            }

            return exito;
        }

        // Eliminar competencia
        public bool EliminarCompetencia()
        {
            bool exito = false;
            string verificarRelacion = "SELECT COUNT(*) FROM Competencia_Empleado WHERE id_competencia = @IdCompetencia";
            string query = "DELETE FROM Competencia WHERE id_competencia = @IdCompetencia";

            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    connection.Open();
                    // Verificar si hay relaciones
                    using (SqlCommand verificarCmd = new SqlCommand(verificarRelacion, connection))
                    {
                        verificarCmd.Parameters.AddWithValue("@IdCompetencia", IdCompetencia);
                        int relacionCount = (int)verificarCmd.ExecuteScalar();

                        if (relacionCount > 0)
                        {
                            Console.WriteLine("No se puede eliminar porque está asociada a empleados.");
                            return false;
                        }
                    }

                    // Eliminar si no hay relaciones
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdCompetencia", IdCompetencia);
                        int rowsAffected = command.ExecuteNonQuery();
                        exito = rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al eliminar competencia: " + ex.Message);
                }
            }
            return exito;
        }

    }
}
