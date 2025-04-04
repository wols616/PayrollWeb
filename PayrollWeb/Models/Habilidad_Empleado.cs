using Microsoft.Data.SqlClient;

namespace PayrollWeb.Models
{
    public class Habilidad_Empleado
    {
        public int IdHabilidadEmpleado { get; set; }
        public int IdEmpleado { get; set; }
        public int IdHabilidad { get; set; }
        public Empleado Empleado { get; set; }
        public Habilidad Habilidad { get; set; }

        public Habilidad_Empleado() { }

        // Método para obtener todas las habilidades de empleados
        public List<Habilidad_Empleado> ObtenerHabilidadesEmpleados()
        {
            List<Habilidad_Empleado> lista = new List<Habilidad_Empleado>();
            string query = "SELECT id_habilidad_empleado, id_empleado, id_habilidad FROM Habilidad_Empleado";
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
                                Habilidad_Empleado habilidadEmpleado = new Habilidad_Empleado
                                {
                                    IdHabilidadEmpleado = Convert.ToInt32(reader["id_habilidad_empleado"]),
                                    IdEmpleado = Convert.ToInt32(reader["id_empleado"]),
                                    IdHabilidad = Convert.ToInt32(reader["id_habilidad"]),
                                    Empleado = new Empleado().ObtenerEmpleado(Convert.ToInt32(reader["id_empleado"])),
                                    Habilidad = new Habilidad().ObtenerHabilidad(Convert.ToInt32(reader["id_habilidad"]))
                                };
                                lista.Add(habilidadEmpleado);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener habilidades de empleados: " + ex.Message);
                }
            }
            return lista;
        }

        // Método para obtener las habilidades de un empleado
        public List<Habilidad_Empleado> ObtenerHabilidadesPorEmpleado(int idEmpleado)
        {
            List<Habilidad_Empleado> lista = new List<Habilidad_Empleado>();
            string query = "SELECT id_habilidad_empleado, id_empleado, id_habilidad FROM Habilidad_Empleado WHERE id_empleado = @IdEmpleado";
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
                                Habilidad_Empleado habilidadEmpleado = new Habilidad_Empleado
                                {
                                    IdHabilidadEmpleado = Convert.ToInt32(reader["id_habilidad_empleado"]),
                                    IdEmpleado = Convert.ToInt32(reader["id_empleado"]),
                                    IdHabilidad = Convert.ToInt32(reader["id_habilidad"]),
                                    Habilidad = new Habilidad().ObtenerHabilidad(Convert.ToInt32(reader["id_habilidad"]))
                                };
                                lista.Add(habilidadEmpleado);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener habilidades del empleado: " + ex.Message);
                }
            }
            return lista;
        }

        // Método para agregar una habilidad a un empleado
        public bool AgregarHabilidadEmpleado()
        {
            bool exito = false;

            string verificarQuery = "SELECT COUNT(*) FROM Habilidad_Empleado WHERE id_empleado = @IdEmpleado AND id_habilidad = @IdHabilidad";
            string insertarQuery = "INSERT INTO Habilidad_Empleado (id_empleado, id_habilidad) VALUES (@IdEmpleado, @IdHabilidad)";
            Conexion conexion = new Conexion();

            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    connection.Open();

                    // Verificar si ya está asignada
                    using (SqlCommand verificarCmd = new SqlCommand(verificarQuery, connection))
                    {
                        verificarCmd.Parameters.AddWithValue("@IdEmpleado", IdEmpleado);
                        verificarCmd.Parameters.AddWithValue("@IdHabilidad", IdHabilidad);

                        int count = (int)verificarCmd.ExecuteScalar();
                        if (count > 0)
                        {
                            return false; // Ya existe la relación
                        }
                    }

                    // Insertar si no existe
                    using (SqlCommand insertCmd = new SqlCommand(insertarQuery, connection))
                    {
                        insertCmd.Parameters.AddWithValue("@IdEmpleado", IdEmpleado);
                        insertCmd.Parameters.AddWithValue("@IdHabilidad", IdHabilidad);

                        int rows = insertCmd.ExecuteNonQuery();
                        exito = rows > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al agregar habilidad al empleado: " + ex.Message);
                }
            }

            return exito;
        }


        // Método para eliminar una habilidad de un empleado
        public bool EliminarHabilidadEmpleado(int idHabilidadEmpleado)
        {
            bool exito = false;
            string query = "DELETE FROM Habilidad_Empleado WHERE id_habilidad_empleado = @IdHabilidadEmpleado";
            Conexion conexion = new Conexion();

            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdHabilidadEmpleado", idHabilidadEmpleado);

                        int rowsAffected = command.ExecuteNonQuery();
                        exito = rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al eliminar habilidad del empleado: " + ex.Message);
                }
            }
            return exito;
        }
    }
}
