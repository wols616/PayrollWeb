using Microsoft.Data.SqlClient;

namespace PayrollWeb.Models
{
    public class Cargo
    {
        public int IdCargo { get; set; }
        public string NombreCargo { get; set; }
        public string Descripcion { get; set; }

        Conexion conexion = new Conexion();
        
        // MÉTODO PARA OBTENER TODOS LOS CARGOS
        public List<Cargo> ObtenerCargos()
        {
            List<Cargo> CargosList = new List<Cargo>();

            // Consulta SQL para obtener todas las deducciones
            string query = "SELECT id_cargo, nombre_cargo, descripcion FROM Cargo";

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
                                Cargo cargo = new Cargo
                                {
                                    IdCargo = Convert.ToInt32(reader["id_cargo"]),
                                    NombreCargo = reader["nombre_cargo"].ToString(),
                                    Descripcion = reader["descripcion"].ToString()
                                };

                                // Agregar el objeto Deduccion a la lista
                                CargosList.Add(cargo);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener los cargos: " + ex.Message);
                }
            }

            // Retornar la lista de deducciones
            return CargosList;
        }

        // MÉTODO PARA OBTENER UN CARGO POR SU ID
        public Cargo ObtenerCargo(int idCargo)
        {
            Cargo cargo = new Cargo();
            // Consulta SQL para obtener todas las deducciones
            string query = "SELECT id_cargo, nombre_cargo, descripcion FROM Cargo WHERE id_cargo = @idCargo";
            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    // Abrir la conexión
                    connection.Open();
                    // Ejecutar la consulta
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@idCargo", idCargo);
                        // Leer los datos de la base de datos
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Procesar cada fila y agregarla a la lista
                            while (reader.Read())
                            {
                                cargo.IdCargo = Convert.ToInt32(reader["id_cargo"]);
                                cargo.NombreCargo = reader["nombre_cargo"].ToString();
                                cargo.Descripcion = reader["descripcion"].ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener el cargo: " + ex.Message);
                }
            }
            // Retornar la lista de deducciones
            return cargo;
        }

        // MÉTODO PARA AGREGAR UN CARGO NUEVO
        public bool AgregarCargo()
        {
            // Consulta SQL para insertar un nuevo cargo
            string query = "INSERT INTO Cargo (nombre_cargo, descripcion) VALUES (@NombreCargo, @Descripcion)";
            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    // Abrir la conexión
                    connection.Open();
                    // Ejecutar la consulta
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NombreCargo", NombreCargo);
                        command.Parameters.AddWithValue("@Descripcion", Descripcion);
                        command.ExecuteNonQuery();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al agregar el cargo: " + ex.Message);
                    return false;
                }
            }
        }

        // MÉTODO PARA ACTUALIZAR UN CARGO
        public bool ActualizarCargo()
        {
            if (EsNombreDuplicado())
            {
                return false; // No permite actualizar si el nombre ya existe en otro cargo
            }

            // Consulta SQL para actualizar el cargo
            string query = "UPDATE Cargo SET nombre_cargo = @NombreCargo, descripcion = @Descripcion WHERE id_cargo = @IdCargo";

            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdCargo", IdCargo);
                        command.Parameters.AddWithValue("@NombreCargo", NombreCargo);
                        command.Parameters.AddWithValue("@Descripcion", Descripcion);
                        command.ExecuteNonQuery();
                        return true; // Cargo actualizado correctamente
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al actualizar el cargo: " + ex.Message);
                    return false;
                }
            }
        }


        public bool EsNombreDuplicado()
        {
            using (SqlConnection con = conexion.GetConnection())
            {
                string query = @"
            SELECT COUNT(*)
            FROM Cargo
            WHERE nombre_cargo = @NombreCargo AND id_cargo != @IdCargo";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@NombreCargo", NombreCargo);
                    cmd.Parameters.AddWithValue("@IdCargo", IdCargo);

                    con.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0; // Retorna true si ya existe un cargo con el mismo nombre
                }
            }
        }


        public bool CargoExiste()
        {
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = @"
                SELECT * 
                FROM Cargo 
                WHERE nombre_cargo = @NombreCargo";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@NombreCargo", NombreCargo);

                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return true; // El nombre ya existe en otro cargo
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al verificar si el cargo ya existe: " + ex.Message);
            }
            return false;
        }

        // MÉTODO PARA ELIMINAR UN CARGO
        public bool EliminarCargo(int idCargo)
        {
            // Consulta SQL para verificar si el cargo está presente en Complemento_Cargo
            string checkQuery = "SELECT COUNT(*) FROM Complemento_Cargo WHERE id_cargo = @IdCargo";
            string deleteQuery = "DELETE FROM Cargo WHERE id_cargo = @IdCargo";

            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    connection.Open();

                    // Verificar si el cargo está siendo utilizado en Complemento_Cargo
                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@IdCargo", idCargo);
                        int count = (int)checkCommand.ExecuteScalar();

                        if (count > 0)
                        {
                            Console.WriteLine("No se puede eliminar el cargo porque está siendo utilizado en Complemento_Cargo.");
                            return false; // El cargo no se puede eliminar porque está en uso
                        }
                    }

                    // Si no está en uso, proceder a eliminar el cargo
                    using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@IdCargo", idCargo);
                        deleteCommand.ExecuteNonQuery();
                        return true; // Cargo eliminado correctamente
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al eliminar el cargo: " + ex.Message);
                    return false;
                }
            }
        }



    }
}
