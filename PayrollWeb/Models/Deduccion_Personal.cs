using Microsoft.Data.SqlClient;

namespace PayrollWeb.Models
{
    public class Deduccion_Personal
    {
        public int IdDeduccionPersonal { get; set; }
        public int IdDeduccion { get; set; }
        public int IdEmpleado { get; set; }
        public Decimal PorcentajePersonal { get; set; }
        public Deduccion Deduccion { get; set; }
        public Empleado Empleado { get; set; }

        public Deduccion_Personal() { }

        //Métodos

        public List<Deduccion_Personal> ObtenerDeduccionesPersonales()
        {
            List<Deduccion_Personal> deduccionesList = new List<Deduccion_Personal>();

            // Consulta SQL para obtener todas las deducciones
            string query = "SELECT id_deduccion_personal, id_deduccion, id_empleado FROM Deduccion_Personal";

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
                                Deduccion_Personal deduccion = new Deduccion_Personal
                                {
                                    IdDeduccionPersonal = Convert.ToInt32(reader["id_deduccion_personal"]),
                                    IdDeduccion = Convert.ToInt32(reader["id_deduccion"]),
                                    IdEmpleado = Convert.ToInt32(reader["id_empleado"]),
                                    Deduccion = new Deduccion().ObtenerDeduccion(Convert.ToInt32(reader["id_deduccion"])),
                                    Empleado = new Empleado().ObtenerEmpleado(Convert.ToInt32(reader["id_empleado"]))
                                };

                                // Agregar el objeto Deduccion a la lista
                                deduccionesList.Add(deduccion);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener las deducciones personales: " + ex.Message);
                }
            }

            // Retornar la lista de deducciones
            return deduccionesList;
        }

        //Métodos para obtener las deducciones personales de un empleado
        public List<Deduccion_Personal> ObtenerDeduccionesPersonalesEmpleado(int idEmpleado)
        {
            List<Deduccion_Personal> deduccionesList = new List<Deduccion_Personal>();
            // Consulta SQL para obtener todas las deducciones personales de un empleado
            string query = "SELECT id_deduccion_personal, id_deduccion, id_empleado, porcentaje_personal FROM Deduccion_Personal WHERE id_empleado = @IdEmpleado";
            Conexion conexion = new Conexion();
            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    // Abrir la conexión
                    connection.Open();
                    // Crear el comando SQL y agregar el parámetro
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdEmpleado", idEmpleado);
                        // Ejecutar la consulta
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Procesar cada fila y agregarla a la lista
                            while (reader.Read())
                            {
                                Deduccion_Personal deduccion = new Deduccion_Personal
                                {
                                    IdDeduccionPersonal = Convert.ToInt32(reader["id_deduccion_personal"]),
                                    IdDeduccion = Convert.ToInt32(reader["id_deduccion"]),
                                    IdEmpleado = Convert.ToInt32(reader["id_empleado"]),
                                    PorcentajePersonal = Convert.ToDecimal(reader["porcentaje_personal"]),
                                    Deduccion = new Deduccion().ObtenerDeduccion(Convert.ToInt32(reader["id_deduccion"])),
                                    Empleado = new Empleado().ObtenerEmpleado(Convert.ToInt32(reader["id_empleado"]))
                                };
                                // Agregar el objeto Deduccion a la lista
                                deduccionesList.Add(deduccion);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener las deducciones personales del empleado: " + ex.Message);
                }
            }
            // Retornar la lista de deducciones
            return deduccionesList;
        }

        public bool AgregarDeduccionPersonal(decimal porcentajePersonal)
        {
            bool exito = false;

            // Consulta SQL para insertar una nueva deducción
            string query = "INSERT INTO Deduccion_Personal (id_deduccion, id_empleado, porcentaje_personal) VALUES (@IdDeduccion, @IdEmpleado, @PorcentajePersonal)";

            Conexion conexion = new Conexion();
            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    // Abrir la conexión
                    connection.Open();

                    // Crear el comando SQL y agregar los parámetros
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdDeduccion", IdDeduccion);
                        command.Parameters.AddWithValue("@IdEmpleado", IdEmpleado);
                        command.Parameters.AddWithValue("@PorcentajePersonal", porcentajePersonal);

                        // Ejecutar la consulta
                        int rowsAffected = command.ExecuteNonQuery();

                        // Si se afectaron filas, la inserción fue exitosa
                        if (rowsAffected > 0)
                        {
                            exito = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al agregar la deducción personal: " + ex.Message);
                }
            }
            new Metodos().EjecutarActualizarPorcentajesDeducciones();
            return exito;
        }
        public bool EliminarDeduccionPersonal(int idDeduccion, int idEmpleado)
        {
            bool exito = false;

            // Consulta SQL para eliminar una deducción por su Id
            string query = "DELETE FROM Deduccion_Personal WHERE id_deduccion = @IdDeduccion AND id_empleado = @IdEmpleado";

            Conexion conexion = new Conexion();
            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    // Abrir la conexión
                    connection.Open();

                    // Crear el comando SQL y agregar el parámetro
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdDeduccion", idDeduccion);
                        command.Parameters.AddWithValue("@IdEmpleado", idEmpleado);


                        // Ejecutar la consulta
                        int rowsAffected = command.ExecuteNonQuery();

                        // Si se afectaron filas, la eliminación fue exitosa
                        if (rowsAffected > 0)
                        {
                            exito = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al eliminar la deducción: " + ex.Message);
                }
            }

            return exito;
        }

        //Editar deducción personal
        public bool EditarDeduccionPersonal()
        {
            bool exito = false;
            // Consulta SQL para actualizar una deducción personal
            string query = "UPDATE Deduccion_Personal SET id_deduccion = @IdDeduccion, id_empleado = @IdEmpleado, porcentaje_personal = @PorcentajePersonal WHERE id_deduccion_personal = @IdDeduccionPersonal";
            Conexion conexion = new Conexion();
            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    // Abrir la conexión
                    connection.Open();
                    // Crear el comando SQL y agregar los parámetros
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdDeduccion", IdDeduccion);
                        command.Parameters.AddWithValue("@IdEmpleado", IdEmpleado);
                        command.Parameters.AddWithValue("@PorcentajePersonal", PorcentajePersonal);
                        command.Parameters.AddWithValue("@IdDeduccionPersonal", IdDeduccionPersonal);
                        // Ejecutar la consulta
                        int rowsAffected = command.ExecuteNonQuery();
                        // Si se afectaron filas, la actualización fue exitosa
                        if (rowsAffected > 0)
                        {
                            exito = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al editar la deducción personal: " + ex.Message);
                }
            }
            return exito;
        }
    }
}
