using Microsoft.Data.SqlClient;

namespace PayrollWeb.Models
{
    public class Competencia_Empleado
    {
        public int IdCompetenciaEmpleado { get; set; }
        public int IdEmpleado { get; set; }
        public int IdCompetencia { get; set; }
        public Empleado Empleado { get; set; }
        public Competencia Competencia { get; set; }

        public Competencia_Empleado() { }

        // Método para obtener todas las competencias de empleados
        public List<Competencia_Empleado> ObtenerCompetenciasEmpleados()
        {
            List<Competencia_Empleado> lista = new List<Competencia_Empleado>();

            string query = "SELECT id_competencia_empleado, id_empleado, id_competencia FROM Competencia_Empleado";
            Conexion conexion = new Conexion();

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
                                Competencia_Empleado competenciaEmpleado = new Competencia_Empleado
                                {
                                    IdCompetenciaEmpleado = Convert.ToInt32(reader["id_competencia_empleado"]),
                                    IdEmpleado = Convert.ToInt32(reader["id_empleado"]),
                                    IdCompetencia = Convert.ToInt32(reader["id_competencia"]),
                                    Empleado = new Empleado().ObtenerEmpleado(Convert.ToInt32(reader["id_empleado"])),
                                    Competencia = new Competencia().ObtenerCompetencia(Convert.ToInt32(reader["id_competencia"]))
                                };
                                lista.Add(competenciaEmpleado);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener competencias de empleados: " + ex.Message);
                }
            }

            return lista;
        }

        // Método para obtener las competencias de un empleado
        public List<Competencia_Empleado> ObtenerCompetenciasPorEmpleado(int idEmpleado)
        {
            List<Competencia_Empleado> lista = new List<Competencia_Empleado>();

            string query = "SELECT id_competencia_empleado, id_empleado, id_competencia FROM Competencia_Empleado WHERE id_empleado = @IdEmpleado";
            Conexion conexion = new Conexion();

            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdEmpleado", idEmpleado);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Competencia_Empleado competenciaEmpleado = new Competencia_Empleado
                                {
                                    IdCompetenciaEmpleado = Convert.ToInt32(reader["id_competencia_empleado"]),
                                    IdEmpleado = Convert.ToInt32(reader["id_empleado"]),
                                    IdCompetencia = Convert.ToInt32(reader["id_competencia"]),
                                    Competencia = new Competencia().ObtenerCompetencia(Convert.ToInt32(reader["id_competencia"]))
                                };
                                lista.Add(competenciaEmpleado);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener competencias del empleado: " + ex.Message);
                }
            }

            return lista;
        }

        // Método para agregar una competencia a un empleado
        public bool AgregarCompetenciaEmpleado()
        {
            bool exito = false;

            string verificarQuery = "SELECT COUNT(*) FROM Competencia_Empleado WHERE id_empleado = @IdEmpleado AND id_competencia = @IdCompetencia";
            string insertarQuery = "INSERT INTO Competencia_Empleado (id_empleado, id_competencia) VALUES (@IdEmpleado, @IdCompetencia)";
            Conexion conexion = new Conexion();

            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    connection.Open();

                    // Verificar si ya existe esa combinación
                    using (SqlCommand verificarCommand = new SqlCommand(verificarQuery, connection))
                    {
                        verificarCommand.Parameters.AddWithValue("@IdEmpleado", IdEmpleado);
                        verificarCommand.Parameters.AddWithValue("@IdCompetencia", IdCompetencia);

                        int count = (int)verificarCommand.ExecuteScalar();

                        if (count > 0)
                        {
                            Console.WriteLine("La competencia ya está asignada al empleado.");
                            return false;
                        }
                    }

                    // Insertar si no existe
                    using (SqlCommand command = new SqlCommand(insertarQuery, connection))
                    {
                        command.Parameters.AddWithValue("@IdEmpleado", IdEmpleado);
                        command.Parameters.AddWithValue("@IdCompetencia", IdCompetencia);

                        int rowsAffected = command.ExecuteNonQuery();
                        exito = rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al agregar competencia al empleado: " + ex.Message);
                }
            }

            return exito;
        }


        // Método para eliminar una competencia de un empleado
        public bool EliminarCompetenciaEmpleado(int idCompetenciaEmpleado)
        {
            bool exito = false;

            string query = "DELETE FROM Competencia_Empleado WHERE id_competencia_empleado = @IdCompetenciaEmpleado";
            Conexion conexion = new Conexion();

            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdCompetenciaEmpleado", idCompetenciaEmpleado);

                        int rowsAffected = command.ExecuteNonQuery();
                        exito = rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al eliminar competencia del empleado: " + ex.Message);
                }
            }

            return exito;
        }
    }
}
