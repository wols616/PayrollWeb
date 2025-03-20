using Microsoft.Data.SqlClient;

namespace PayrollWeb.Models
{
    public class Complemento_Puesto
    {
        public int IdComplementoPuesto { get; set; }
        public string NombreComplemento { get; set; }
        public decimal Monto { get; set; }
        public int IdPuesto { get; set; }

        Conexion conexion = new Conexion();

        public List<Complemento_Puesto> ObtenerComplementosPuesto(int idPuesto)
        {
            List<Complemento_Puesto> complementos = new List<Complemento_Puesto>();
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = "SELECT * FROM Complemento_puesto WHERE id_puesto = @idPuesto";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@idPuesto", idPuesto);
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Complemento_Puesto complemento = new Complemento_Puesto
                                {
                                    IdComplementoPuesto = Convert.ToInt32(reader["id_complemento_puesto"]),
                                    IdPuesto = Convert.ToInt32(reader["id_puesto"]),
                                    NombreComplemento = reader["nombre_complemento"].ToString(),
                                    Monto = Convert.ToDecimal(reader["monto"])
                                };
                                complementos.Add(complemento);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener los complementos: " + ex.Message, "Error");
            }
            return complementos;
        }

        //Método para obtener un complemento por su id
        public Complemento_Puesto ObtenerComplementoPorId(int idComplementoPuesto)
        {
            Complemento_Puesto complemento = new Complemento_Puesto();
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = "SELECT * FROM Complemento_puesto WHERE id_complemento_puesto = @idComplementoPuesto";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@idComplementoPuesto", idComplementoPuesto);
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                complemento.IdComplementoPuesto = Convert.ToInt32(reader["id_complemento_puesto"]);
                                complemento.IdPuesto = Convert.ToInt32(reader["id_puesto"]);
                                complemento.NombreComplemento = reader["nombre_complemento"].ToString();
                                complemento.Monto = Convert.ToDecimal(reader["monto"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener el complemento: " + ex.Message, "Error");
            }
            return complemento;
        }

        //Método para agregar un complemento
        public bool AgregarComplemento()
        {
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = "INSERT INTO Complemento_puesto (nombre_complemento, monto, id_puesto) VALUES (@nombreComplemento, @monto, @idPuesto)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@nombreComplemento", NombreComplemento);
                        cmd.Parameters.AddWithValue("@monto", Monto);
                        cmd.Parameters.AddWithValue("@idPuesto", IdPuesto);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al agregar el complemento: " + ex.Message, "Error");
                return false;
            }
        }

        //Método para actualizar un complemento
        public bool ActualizarComplemento()
        {
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = "UPDATE Complemento_puesto SET nombre_complemento = @nombreComplemento, monto = @monto WHERE id_complemento_puesto = @idComplementoPuesto";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@nombreComplemento", NombreComplemento);
                        cmd.Parameters.AddWithValue("@monto", Monto);
                        cmd.Parameters.AddWithValue("@idComplementoPuesto", IdComplementoPuesto);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al actualizar el complemento: " + ex.Message, "Error");
                return false;
            }
        }

        //Método para eliminar un complemento
        public bool EliminarComplemento(int idComplementoPuesto)
        {
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = "DELETE FROM Complemento_puesto WHERE id_complemento_puesto = @idComplementoPuesto";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@idComplementoPuesto", idComplementoPuesto);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al eliminar el complemento: " + ex.Message, "Error");
                return false;
            }
        }

        //Método para saber si un complemento ya existe
        public bool ComplementoExistente()
        {
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = @"
                SELECT * 
                FROM Complemento_puesto 
                WHERE nombre_complemento = @nombreComplemento 
                AND id_complemento_puesto != @idComplementoPuesto";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@nombreComplemento", NombreComplemento);
                        cmd.Parameters.AddWithValue("@idComplementoPuesto", IdComplementoPuesto);

                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return true; // El nombre ya existe en otro registro
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al verificar si el complemento ya existe: " + ex.Message, "Error");
            }
            return false;
        }


    }
}
