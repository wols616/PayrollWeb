using Microsoft.Data.SqlClient;
using System.Data;

namespace PayrollWeb.Models
{
    public class Categoria
    {
        public int IdCategoria { get; set; }
        public string NombreCategoria { get; set; }
        public decimal SueldoBase { get; set; }

        //CONSTRUCTORES
        public Categoria(string nombreCategoria, decimal sueldobase)
        {
            this.NombreCategoria = nombreCategoria;
            this.SueldoBase = sueldobase;
        }
        public Categoria(int idCategoria, string nombreCategoria, decimal sueldobase)
        {
            this.IdCategoria = idCategoria;
            this.NombreCategoria = nombreCategoria;
            this.SueldoBase = sueldobase;
        }
        Conexion conexion = new Conexion();

        public Categoria() { }

        //METODOS CRUD CATEGORIA
        //SELECT CATEGORIA
        public List<Categoria> ObtenerCategorias()
        {
            List<Categoria> categoriaList = new List<Categoria>();

            // Consulta SQL para obtener todas las deducciones
            string query = "SELECT * FROM Categoria";

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
                                Categoria categoria = new Categoria
                                {
                                    IdCategoria = reader.GetInt32(0),
                                    NombreCategoria = reader.GetString(1),
                                    SueldoBase = reader.GetDecimal(2)
                                };

                                // Agregar el objeto Deduccion a la lista
                                categoriaList.Add(categoria);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener las categorias: " + ex.Message);
                }
            }

            // Retornar la lista de deducciones
            return categoriaList;
        }

        //INSERTAR CATEGORIA
        public bool AgregarCategoria()
        {
            bool exito = false;

            // Consulta SQL para insertar una nueva deducción
            string query = "INSERT INTO Categoria (nombre_categoria, sueldo_base) VALUES (@NombreCategoria, @SueldoBase)";

            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    // Abrir la conexión
                    connection.Open();

                    // Crear el comando SQL y agregar los parámetros
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NombreCategoria", NombreCategoria);
                        command.Parameters.AddWithValue("@SueldoBase", SueldoBase);

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
                    Console.WriteLine("Error al agregar la categoria: " + ex.Message);
                }
            }

            return exito;
        }

        // MÉTODO PARA OBTENER LA CATEGORIA DE UN PUESTO
        public Categoria ObtenerCategoria(int idCategoria)
        {
            Categoria categoria = null;

            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = "SELECT id_categoria, nombre_categoria, sueldo_base FROM Categoria WHERE id_categoria = @id_categoria";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@id_categoria", SqlDbType.Int).Value = idCategoria;
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                categoria = new Categoria
                                {
                                    IdCategoria = reader.GetInt32(0),
                                    NombreCategoria = reader.GetString(1),
                                    SueldoBase = reader.GetDecimal(2)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener la categoría: " + ex.Message, "Error");
            }
            return categoria;
        }

        // ELIMINAR CATEGORIA
        public bool EliminarCategoria(int idCategoria)
        {
            bool exito = false;

            // Consulta SQL para verificar si la categoría está referenciada en otras tablas
            string queryVerificar = @"
            IF EXISTS (SELECT 1 FROM Puesto WHERE id_categoria = @idCategoria)
            BEGIN
                SELECT 1; -- Si tiene registros asociados, no eliminar
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

                    // Verificar si la categoría está referenciada en otras tablas
                    using (SqlCommand commandVerificar = new SqlCommand(queryVerificar, connection))
                    {
                        commandVerificar.Parameters.AddWithValue("@idCategoria", idCategoria);
                        var result = commandVerificar.ExecuteScalar();

                        if (Convert.ToInt32(result) == 1)
                        {
                            return false; // La categoría no puede ser eliminada porque tiene registros asociados
                        }
                    }

                    // Consulta SQL para eliminar la categoría
                    string queryEliminar = "DELETE FROM Categoria WHERE id_categoria = @idCategoria";

                    // Eliminar la categoría si no tiene registros asociados
                    using (SqlCommand commandEliminar = new SqlCommand(queryEliminar, connection))
                    {
                        commandEliminar.Parameters.AddWithValue("@idCategoria", idCategoria);
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
                    Console.WriteLine("Error al eliminar la categoria: " + ex.Message);
                }
            }

            return exito;
        }

        // ACTUALIZAR CATEGORIA
        public bool ActualizarCategoria()
        {
            bool exito = false;

            // Consulta SQL para actualizar una deducción
            string query = "UPDATE Categoria SET nombre_categoria = @NuevoNombre, sueldo_base = @NuevoSueldo WHERE id_categoria = @IdCategoria";

            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    // Abrir la conexión
                    connection.Open();

                    // Crear el comando SQL y agregar los parámetros
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NuevoNombre", NombreCategoria);
                        command.Parameters.AddWithValue("@NuevoSueldo", SueldoBase);
                        command.Parameters.AddWithValue("@IdCategoria", IdCategoria);

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
                    Console.WriteLine("Error al actualizar la categoria: " + ex.Message);
                }
            }

            return exito;
        }

        //extras
        public bool ExisteCategoria()
        {
            string query = "SELECT COUNT(*) FROM Categoria WHERE nombre_categoria = @NombreCategoria";

            using (SqlConnection con = conexion.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@NombreCategoria", NombreCategoria);
                    cmd.Parameters.AddWithValue("@SueldoBase", SueldoBase);
                    con.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }
    }
}
