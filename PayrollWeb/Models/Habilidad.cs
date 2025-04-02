using Microsoft.Data.SqlClient;

namespace PayrollWeb.Models
{
    public class Habilidad
    {
        Conexion conexion = new Conexion();

        public int IdHabilidad { get; set; }
        public string Nombre { get; set; }


        public Habilidad(string nombre)
        {
            this.Nombre = nombre;
        }

        public Habilidad(int idHabilidad, string nombre)
        {
            this.IdHabilidad = idHabilidad;
            this.Nombre = nombre;
        }
        public Habilidad() { }


        // Obtener todas las habilidades
        public List<Habilidad> ObtenerHabilidades()
        {
            List<Habilidad> habilidadesList = new List<Habilidad>();
            string query = "SELECT * FROM Habilidad";

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
                                habilidadesList.Add(new Habilidad(reader.GetInt32(0), reader.GetString(1)));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener habilidades: " + ex.Message);
                }
            }

            return habilidadesList;
        }

        public Habilidad ObtenerHabilidad(int idHabilidad)
        {
            Habilidad habilidad = new Habilidad();

            // Consulta SQL para obtener una habilidad por su Id
            string query = "SELECT * FROM Habilidad WHERE id_habilidad = @IdHabilidad";

            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    // Abrir la conexión
                    connection.Open();

                    // Crear el comando SQL y agregar los parámetros
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdHabilidad", idHabilidad);

                        // Ejecutar la consulta
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Procesar la fila y asignar los valores a la habilidad
                            if (reader.Read())
                            {
                                habilidad.IdHabilidad = reader.GetInt32(0);
                                habilidad.Nombre = reader.GetString(1);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener la habilidad: " + ex.Message);
                }
            }

            return habilidad;
        }


        public bool AgregarHabilidad()
        {
            bool exito = false;
            string query = "INSERT INTO Habilidad (nombre) VALUES (@Nombre)";

            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Verificar que el nombre no sea nulo o vacío
                        if (string.IsNullOrEmpty(Nombre))
                        {
                            Console.WriteLine("El nombre de la habilidad está vacío.");
                            return false;
                        }

                        command.Parameters.AddWithValue("@Nombre", Nombre);
                        int rowsAffected = command.ExecuteNonQuery();
                        exito = rowsAffected > 0;

                        if (!exito)
                        {
                            Console.WriteLine("No se pudo insertar la habilidad.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al agregar habilidad: " + ex.Message);
                }
            }

            return exito;
        }


        // Actualizar habilidad
        public bool EditarHabilidad()
        {
            bool exito = false;
            string query = "UPDATE Habilidad SET nombre = @Nombre WHERE id_habilidad = @IdHabilidad";

            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Nombre", Nombre);
                        command.Parameters.AddWithValue("@IdHabilidad", IdHabilidad);
                        int rowsAffected = command.ExecuteNonQuery();
                        exito = rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al actualizar habilidad: " + ex.Message);
                }
            }

            return exito;
        }

        // Eliminar habilidad
        public bool EliminarHabilidad()
        {
            bool exito = false;
            string query = "UPDATE Habilidad SET activo = 0 WHERE id_habilidad = @IdHabilidad";

            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdHabilidad", IdHabilidad);
                        int rowsAffected = command.ExecuteNonQuery();
                        exito = rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al desactivar habilidad: " + ex.Message);
                }
            }
            return exito;
        }

    }
}
