using Microsoft.Data.SqlClient;

namespace PayrollWeb.Models
{
    public class Deduccion_Personal
    {
        public int IdDeduccionPersonal { get; set; }
        public int IdDeduccion { get; set; }
        public int IdEmpleado { get; set; }
        //public Deduccion Deduccion { get; set; }
        //public Empleado Empleado { get; set; }

        public Deduccion_Personal() { }

        public Deduccion_Personal(int idDeduccionPersona, int idDeduccion, int idEmpleado)
        {
            IdDeduccionPersonal = idDeduccionPersona;
            IdDeduccion = idDeduccion;
            IdEmpleado = idEmpleado;
        }
        public Deduccion_Personal(int idDeduccion, int idEmpleado)
        {
            IdDeduccion = idDeduccion;
            IdEmpleado = idEmpleado;
        }

        //Métodos
        //SELECT
        public static List<Deduccion_Personal> ObtenerDeduccionesPersonales()
        {
            List<Deduccion_Personal> deduccionesList = new List<Deduccion_Personal>();

            // Consulta SQL para obtener todas las deducciones
            string query = "SELECT * FROM Deduccion_Personal";

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
                                    IdDeduccionPersonal = reader.GetInt32(0),
                                    IdDeduccion = reader.GetInt32(1),
                                    IdEmpleado = reader.GetInt32(2)
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

        //public static List<Deduccion_Personal> ObtenerDeduccionesPersonales(int IdEmpleado)
        //{
        //    List<Deduccion_Personal> deduccionesList = new List<Deduccion_Personal>();

        //    // Consulta SQL para obtener todas las deducciones
        //    string query = "SELECT * FROM Deduccion_Personal WHERE id_empleado = @IdEmpleado";

        //    Conexion conexion = new Conexion();

        //    using (SqlConnection connection = conexion.GetConnection())
        //    {
        //        try
        //        {
        //            // Abrir la conexión
        //            connection.Open();

        //            // Ejecutar la consulta
        //            using (SqlCommand command = new SqlCommand(query, connection))
        //            {
        //                command.Parameters.AddWithValue("@IdEmpleado", IdEmpleado);
        //                // Leer los datos de la base de datos
        //                using (SqlDataReader reader = command.ExecuteReader())
        //                {
        //                    // Procesar cada fila y agregarla a la lista
        //                    while (reader.Read())
        //                    {
        //                        Deduccion_Personal deduccion = new Deduccion_Personal
        //                        {
        //                            IdDeduccionPersonal = reader.GetInt32(0),
        //                            IdDeduccion = reader.GetInt32(1),
        //                            IdEmpleado = reader.GetInt32(2)
        //                        };

        //                        // Agregar el objeto Deduccion a la lista
        //                        deduccionesList.Add(deduccion);
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("Error al obtener las deducciones personales: " + ex.Message);
        //        }
        //    }

        //    // Retornar la lista de deducciones
        //    return deduccionesList;
        //}

        public static List<Deduccion> ObtenerDeduccionesPersonales(int IdEmpleado)
        {
            List<Deduccion> deduccionesList = new List<Deduccion>();

            // Consulta SQL para obtener todas las deducciones
            string query = "SELECT d.id_deduccion, d.nombre_deduccion, d.porcentaje FROM Deduccion_Personal dp " +
                "JOIN Deduccion d ON d.id_deduccion = dp.id_deduccion " +
                "WHERE id_empleado = @IdEmpleado";

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
                        command.Parameters.AddWithValue("@IdEmpleado", IdEmpleado);
                        // Leer los datos de la base de datos
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Procesar cada fila y agregarla a la lista
                            while (reader.Read())
                            {
                                Deduccion deduccion = new Deduccion
                                {
                                    IdDeduccion = reader.GetInt32(0),
                                    NombreDeduccion = reader.GetString(1),
                                    Porcentaje = reader.GetDecimal(2)
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

        public bool AgregarDeduccionPersonal()
        {
            bool exito = false;

            // Consulta SQL para insertar una nueva deducción
            string query = "INSERT INTO Deduccion_Personal (id_deduccion, id_empleado) VALUES (@IdDeduccion, @IdEmpleado)";

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

            return exito;
        }
        public static bool EliminarDeduccionPersonal(int idDeduccion, int idEmpleado)
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
    }
}
