using Microsoft.Data.SqlClient;

namespace PayrollWeb.Models
{
    public class Puesto
    {
        Conexion conexion = new Conexion();
        public int IdPuesto { get; set; }
        public string NombrePuesto { get; set; }
        public int IdCategoria { get; set; }

        //CONSTRUCTORES
        public Puesto(int idPuesto, string nombrepuesto)
        {
            this.IdPuesto = idPuesto;
            this.NombrePuesto = nombrepuesto;
        }
        public Puesto(int idPuesto, string nombrepuesto, int idCategoria)
        {
            this.IdPuesto = idPuesto;
            this.NombrePuesto = nombrepuesto;
            this.IdCategoria = idCategoria;
        }

        public Puesto() { }

        // MÉTODO PARA OBTENER TODOS LOS PUESTOS
        public List<Puesto> MostrarPuestos()
        {
            List<Puesto> listaPuestos = new List<Puesto>();

            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = "SELECT * FROM Puesto";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Puesto puesto = new Puesto
                                {
                                    IdPuesto = Convert.ToInt32(reader["id_puesto"]),
                                    NombrePuesto = reader["nombre_puesto"].ToString(),
                                    IdCategoria = Convert.ToInt32(reader["id_categoria"])
                                };
                                listaPuestos.Add(puesto);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener los puestos: " + ex.Message, "Error");
            }

            return listaPuestos;
        }

        //Método para obtener un puesto por su ID
        public Puesto ObtenerPuesto(int idPuesto)
        {
            Puesto puesto = new Puesto();
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = "SELECT * FROM Puesto WHERE id_puesto = @idPuesto";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@idPuesto", idPuesto);
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                puesto.IdPuesto = Convert.ToInt32(reader["id_puesto"]);
                                puesto.NombrePuesto = reader["nombre_puesto"].ToString();
                                puesto.IdCategoria = Convert.ToInt32(reader["id_categoria"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener el puesto: " + ex.Message, "Error");
            }
            return puesto;
        }

        public List<Object> MostrarPuestosConCategoria()
        {
            List<Object> listaPuestos = new List<Object>();

            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = "SELECT Puesto.id_puesto, Puesto.nombre_puesto, Categoria.nombre_categoria, Puesto.sueldo_base, Categoria.id_categoria FROM Puesto " +
                        "JOIN Categoria ON Categoria.id_categoria = Puesto.id_categoria";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var puesto = new
                                {
                                    IdPuesto = Convert.ToInt32(reader["id_puesto"]),
                                    NombrePuesto = reader["nombre_puesto"].ToString(),
                                    IdCategoria = Convert.ToInt32(reader["id_categoria"]),
                                    Categoria = reader["nombre_categoria"].ToString(),
                                    SueldoBase = Convert.ToDecimal(reader["sueldo_base"])
                                };
                                listaPuestos.Add(puesto);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener los puestos: " + ex.Message, "Error");
            }

            return listaPuestos;
        }

        //agregar
        public bool AgregarPuesto()
        {
            if (string.IsNullOrWhiteSpace(NombrePuesto) || IdCategoria <= 0)
            {
                Console.WriteLine("El nombre del puesto y la categoría son obligatorios.");
                return false;
            }

            try
            {
                Conexion con = new Conexion();
                string query = "INSERT INTO Puesto (nombre_puesto, id_categoria) VALUES (@nombrePuesto, @idCategoria)";

                using (SqlConnection conexion = con.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@nombrePuesto", NombrePuesto);
                        cmd.Parameters.AddWithValue("@idCategoria", IdCategoria);

                        conexion.Open();
                        int filasAfectadas = cmd.ExecuteNonQuery();
                        conexion.Close();

                        if (filasAfectadas > 0)
                        {
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("No se pudo agregar el puesto. Verifique los datos.");
                            return false;
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine("Error SQL: " + sqlEx.Message);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al agregar puesto: " + ex.Message);
                return false;
            }
        }

        //editar
        public bool EditarPuesto()
        {
            if (IdPuesto <= 0 || string.IsNullOrWhiteSpace(NombrePuesto) || IdCategoria <= 0)
            {
                Console.WriteLine("El puesto, nombre y categoría son obligatorios.");
                return false;
            }

            try
            {
                Conexion con = new Conexion();
                string query = "UPDATE Puesto SET nombre_puesto = @nombrePuesto, id_categoria = @idCategoria WHERE id_puesto = @idPuesto";

                using (SqlConnection conexion = con.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@nombrePuesto", NombrePuesto);
                        cmd.Parameters.AddWithValue("@idCategoria", IdCategoria);
                        cmd.Parameters.AddWithValue("@idPuesto", IdPuesto);

                        conexion.Open();
                        int filasAfectadas = cmd.ExecuteNonQuery();
                        conexion.Close();

                        if (filasAfectadas > 0)
                        {
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("No se pudo actualizar el puesto. Verifique los datos.");
                            return false;
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine   ("Error SQL: " + sqlEx.Message);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al editar puesto: " + ex.Message);
                return false;
            }
        }

        public bool EliminarPuesto(int idPuesto)
        {
            bool exito = false;

            // Consulta SQL para verificar si el puesto está referenciado en otras tablas (en este caso, la tabla Contrato)
            string queryVerificar = @"
        IF EXISTS (SELECT 1 FROM Contrato WHERE id_puesto = @idPuesto)
        BEGIN
            SELECT 1; -- Si tiene registros asociados en Contrato, no se puede eliminar
        END
        ELSE
        BEGIN
            SELECT 0; -- Si no tiene registros asociados, se puede eliminar
        END";

            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    // Abrir la conexión
                    connection.Open();

                    // Verificar si el puesto está referenciado en la tabla Contrato
                    using (SqlCommand commandVerificar = new SqlCommand(queryVerificar, connection))
                    {
                        commandVerificar.Parameters.AddWithValue("@idPuesto", idPuesto);
                        var result = commandVerificar.ExecuteScalar();

                        if (Convert.ToInt32(result) == 1)
                        {
                            return false; // El puesto no puede ser eliminado porque tiene registros asociados en Contrato
                        }
                    }

                    // Consulta SQL para eliminar los complementos del puesto
                    string queryEliminarComplementos = "DELETE FROM Complemento_puesto WHERE id_puesto = @idPuesto";

                    // Eliminar los complementos del puesto
                    using (SqlCommand commandEliminarComplementos = new SqlCommand(queryEliminarComplementos, connection))
                    {
                        commandEliminarComplementos.Parameters.AddWithValue("@idPuesto", idPuesto);
                        commandEliminarComplementos.ExecuteNonQuery();
                    }

                    // Consulta SQL para eliminar el puesto si no tiene registros asociados
                    string queryEliminar = "DELETE FROM Puesto WHERE id_puesto = @idPuesto";

                    // Eliminar el puesto si no tiene registros asociados
                    using (SqlCommand commandEliminar = new SqlCommand(queryEliminar, connection))
                    {
                        commandEliminar.Parameters.AddWithValue("@idPuesto", idPuesto);
                        int rowsAffected = commandEliminar.ExecuteNonQuery();

                        // Si se afectaron filas, la eliminación fue exitosa
                        if (rowsAffected > 0)
                        {
                            exito = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al eliminar el puesto: " + ex.Message);
                }
            }

            return exito;
        }


        public bool ExistePuesto()
        {
            string query = "SELECT COUNT(*) FROM Puesto WHERE Puesto.Nombre_puesto = @NombrePuesto";

            using (SqlConnection con = conexion.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@NombrePuesto", NombrePuesto);
                    cmd.Parameters.AddWithValue("@IdCategoria", IdCategoria);
                    con.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public List<PuestoViewModel> ObtenerPuestosViewModel()
        {
            List<PuestoViewModel> listaPuestosViewModel = new List<PuestoViewModel>();

            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = @"
                SELECT p.id_puesto, p.nombre_puesto, c.id_categoria, c.nombre_categoria, p.sueldo_base
                FROM Puesto p
                JOIN Categoria c ON p.id_categoria = c.id_categoria Order BY p.sueldo_base";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PuestoViewModel puestoViewModel = new PuestoViewModel
                                {
                                    IdPuesto = Convert.ToInt32(reader["id_puesto"]),
                                    NombrePuesto = reader["nombre_puesto"].ToString(),
                                    IdCategoria = Convert.ToInt32(reader["id_categoria"]),
                                    Categoria = reader["nombre_categoria"].ToString(),
                                    SueldoBase = Convert.ToDecimal(reader["sueldo_base"])
                                };
                                listaPuestosViewModel.Add(puestoViewModel);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener los puestos: " + ex.Message, "Error");
            }

            return listaPuestosViewModel;
        }

        public decimal ObtenerSueldoBasePuesto(int idPuesto)
        {
            decimal sueldoBase = 0;
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = "SELECT sueldo_base FROM Puesto  WHERE id_puesto = @idPuesto";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@idPuesto", idPuesto);
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                sueldoBase = Convert.ToDecimal(reader["sueldo_base"]);
                            }
                        }
                    }
                }
                return sueldoBase;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener el sueldo base del puesto: " + ex.Message, "Error");
                return 0;
            }
        }

    }

    public class PuestoViewModel
    {
        public int IdPuesto { get; set; }
        public int IdCategoria { get; set; }
        public string NombrePuesto { get; set; }
        public string Categoria { get; set; }
        public decimal SueldoBase { get; set; }
    }
}
