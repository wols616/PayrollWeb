using Microsoft.Data.SqlClient;

namespace PayrollWeb.Models
{
    public class Deduccion
    {
        Conexion conexion = new Conexion();
        public int IdDeduccion { get; set; }
        public string NombreDeduccion { get; set; }
        public decimal Porcentaje { get; set; }
        //Será un solo caracter S o N, para trabajarlo como si fuera booleano
        public string Fija { get; set; }
        public Deduccion(string nombreDeduccion, decimal porcentaje, string fija)
        {
            this.NombreDeduccion = nombreDeduccion;
            this.Porcentaje = porcentaje;
            this.Fija = fija;
        }
        public Deduccion(int idDeduccion, string nombreDeduccion, decimal porcentaje, string fija)
        {
            this.IdDeduccion = idDeduccion;
            this.NombreDeduccion = nombreDeduccion;
            this.Porcentaje = porcentaje;
            this.Fija = fija;
        }

        public Deduccion() { }

        //MÉTODOS
        //SELECT
        public List<Deduccion> ObtenerDeducciones()
        {
            List<Deduccion> deduccionesList = new List<Deduccion>();

            // Consulta SQL para obtener todas las deducciones
            string query = "SELECT * FROM Deduccion";


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
                                    Porcentaje = reader.GetDecimal(2),  // Tercera columna es Porcentaje
                                    Fija = reader.GetString(3)  // Cuarta columna es Fija
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

        //Método para obtener una deducción por su Id
        public Deduccion ObtenerDeduccion(int idDeduccion)
        {
            Deduccion deduccion = new Deduccion();
            // Consulta SQL para obtener una deducción por su Id
            string query = "SELECT * FROM Deduccion WHERE id_deduccion = @IdDeduccion";
            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    // Abrir la conexión
                    connection.Open();
                    // Crear el comando SQL y agregar los parámetros
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdDeduccion", idDeduccion);
                        // Ejecutar la consulta
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Procesar la fila y asignar los valores a la deducción
                            if (reader.Read())
                            {
                                deduccion.IdDeduccion = reader.GetInt32(0);
                                deduccion.NombreDeduccion = reader.GetString(1);
                                deduccion.Porcentaje = reader.GetDecimal(2);
                                deduccion.Fija = reader.GetString(3);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener la deducción: " + ex.Message);
                }
            }
            return deduccion;
        }

        //INSERT
        public bool AgregarDeduccion()
        {
            bool exito = false;

            // Consulta SQL para insertar una nueva deducción
            string query = "INSERT INTO Deduccion (nombre_deduccion, porcentaje, fija) VALUES (@NombreDeduccion, @Porcentaje, @Fija)";

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
                        command.Parameters.AddWithValue("@Fija", Fija);

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
            new Metodos().EjecutarActualizarPorcentajesDeducciones();
            return exito;
        }

        public bool EliminarDeduccion(int idDeduccion)
        {
            bool exito = false;

            // Consulta SQL para verificar si la deducción está asociada a algún empleado
            string checkQuery = "SELECT COUNT(*) FROM Deduccion_Personal WHERE id_deduccion = @IdDeduccion";

            // Consulta SQL para eliminar la deducción si no está asociada
            string deleteQuery = "DELETE FROM Deduccion WHERE id_deduccion = @IdDeduccion";

            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    // Abrir la conexión
                    connection.Open();

                    // Verificar si existen registros en Deduccion_Personal
                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@IdDeduccion", idDeduccion);
                        int count = (int)checkCommand.ExecuteScalar();

                        // Solo eliminar si no hay registros asociados
                        if (count == 0)
                        {
                            using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection))
                            {
                                deleteCommand.Parameters.AddWithValue("@IdDeduccion", idDeduccion);
                                int rowsAffected = deleteCommand.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    exito = true;
                                }
                            }
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
        public bool EditarDeduccion()
        {
            bool exito = false;

            // Consulta SQL para actualizar una deducción
            string query = "UPDATE Deduccion SET nombre_deduccion = @NuevoNombre, porcentaje = @NuevoPorcentaje, fija = @NuevoFija WHERE id_deduccion = @IdDeduccion";

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
                        command.Parameters.AddWithValue("@NuevoFija", Fija);
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

            new Metodos().EjecutarActualizarPorcentajesDeducciones();
            return exito;
        }

        //Método para verificar si una deducción ya existe
        public bool ExisteDeduccion()
        {
            bool existe = false;
            // Consulta SQL para verificar si la deducción ya existe
            string query = "SELECT COUNT(*) FROM Deduccion WHERE nombre_deduccion = @NombreDeduccion";
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
                        // Ejecutar la consulta
                        int count = (int)command.ExecuteScalar();
                        // Si el conteo es mayor a 0, la deducción ya existe
                        if (count > 0)
                        {
                            existe = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al verificar si la deducción existe: " + ex.Message);
                }
            }
            return existe;
        }
    }
}
