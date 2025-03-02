using Microsoft.Data.SqlClient;

namespace PayrollWeb.Models
{
    public class Deduccion
    {
        public int IdDeduccion { get; set; }
        public string NombreDeduccion { get; set; }
        public decimal Porcentaje { get; set; }
        public decimal Monto { get; set; }
        public Deduccion(string nombreDeduccion, decimal porcentaje)
        {
            this.NombreDeduccion = nombreDeduccion;
            this.Porcentaje = porcentaje;
        }
        public Deduccion(int idDeduccion, string nombreDeduccion, decimal porcentaje)
        {
            this.IdDeduccion = idDeduccion;
            this.NombreDeduccion = nombreDeduccion;
            this.Porcentaje = porcentaje;
        }

        public Deduccion() { }

        //MÉTODOS
        //SELECT
        public static List<Deduccion> ObtenerDeducciones()
        {
            List<Deduccion> deduccionesList = new List<Deduccion>();

            // Consulta SQL para obtener todas las deducciones
            string query = "SELECT * FROM Deduccion";

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
                                Deduccion deduccion = new Deduccion
                                {
                                    IdDeduccion = reader.GetInt32(0),  // Primer columna es IdDeduccion
                                    NombreDeduccion = reader.GetString(1), // Segunda columna es NombreDeduccion
                                    Porcentaje = reader.GetDecimal(2)  // Tercera columna es Porcentaje
                                };

                                // Agregar el objeto Deduccion a la lista
                                deduccionesList.Add(deduccion);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener las deducciones: " + ex.Message);
                }
            }

            // Retornar la lista de deducciones
            return deduccionesList;
        }

        //INSERT
        public bool AgregarDeduccion()
        {
            bool exito = false;

            // Consulta SQL para insertar una nueva deducción
            string query = "INSERT INTO Deduccion (nombre_deduccion, porcentaje) VALUES (@NombreDeduccion, @Porcentaje)";

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
                        command.Parameters.AddWithValue("@NombreDeduccion", NombreDeduccion);
                        command.Parameters.AddWithValue("@Porcentaje", Porcentaje);

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
                    Console.WriteLine("Error al agregar la deducción: " + ex.Message);
                }
            }

            return exito;
        }

        public static bool EliminarDeduccion(int idDeduccion)
        {
            bool exito = false;

            // Consulta SQL para eliminar una deducción por su Id
            string query = "DELETE FROM Deduccion WHERE id_deduccion = @IdDeduccion";

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

        // Método para actualizar una deducción
        public bool ActualizarDeduccion()
        {
            bool exito = false;

            // Consulta SQL para actualizar una deducción
            string query = "UPDATE Deduccion SET nombre_deduccion = @NuevoNombre, porcentaje = @NuevoPorcentaje WHERE id_deduccion = @IdDeduccion";

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
                        command.Parameters.AddWithValue("@NuevoNombre", NombreDeduccion);
                        command.Parameters.AddWithValue("@NuevoPorcentaje", Porcentaje);
                        command.Parameters.AddWithValue("@IdDeduccion", IdDeduccion);

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
                    Console.WriteLine("Error al actualizar la deducción: " + ex.Message);
                }
            }

            return exito;
        }
    }
}
