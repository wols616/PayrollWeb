using Microsoft.Data.SqlClient;

namespace PayrollWeb.Models
{
    public class KPI
    {
        public int IdKpi { get; set; }
        public string Nombre { get; set; }

        Conexion conexion = new Conexion();

        public KPI() { }

        public KPI(int idKpi, string nombre)
        {
            IdKpi = idKpi;
            Nombre = nombre;
        }

        public KPI(string Nombre) { this.Nombre = Nombre; }

        //Método para obtener todos los KPIs
        public List<KPI> ObtenerKPI()
        {
            List<KPI> listaKPI = new List<KPI>();

            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = "SELECT * FROM KPI";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                KPI kpi = new KPI
                                {
                                    IdKpi = Convert.ToInt32(reader["id_kpi"]),
                                    Nombre = reader["nombre"].ToString()
                                };
                                listaKPI.Add(kpi);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener los puestos: " + ex.Message, "Error");
            }

            return listaKPI;
        }

        //Método para obtener un KPI por su ID
        public KPI ObtenerKPI(int idKpi)
        {
            KPI kpi = new KPI();
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = "SELECT * FROM KPI WHERE id_kpi = @idKpi";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@idKpi", idKpi);
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                kpi.IdKpi = Convert.ToInt32(reader["id_kpi"]);
                                kpi.Nombre = reader["nombre"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener el KPI: " + ex.Message, "Error");
            }
            return kpi;
        }

        //Método para insertar un KPI
        public bool AgregarKPI()
        {
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = "INSERT INTO KPI (nombre) VALUES (@nombre)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@nombre", Nombre);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al insertar el KPI: " + ex.Message, "Error");
                return false;
            }
        }

        //Método para actualizar un KPI
        public bool ActualizarKPI()
        {
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = "UPDATE KPI SET nombre = @nombre WHERE id_kpi = @idKpi";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@idKpi", IdKpi);
                        cmd.Parameters.AddWithValue("@nombre", Nombre);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al actualizar el KPI: " + ex.Message, "Error");
                return false;
            }
        }

        //Método para eliminar un KPI
        public void EliminarKPI(int idKpi)
        {
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    // Verificar si el KPI está en la tabla Evaluacion_Desempeno
                    string checkQuery = "SELECT COUNT(*) FROM Evaluacion_Desempeno WHERE id_kpi = @idKpi";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                    {
                        checkCmd.Parameters.AddWithValue("@idKpi", idKpi);
                        con.Open();
                        int count = (int)checkCmd.ExecuteScalar();
                        con.Close();

                        if (count > 0)
                        {
                            Console.WriteLine("No se puede eliminar el KPI porque está asociado a una evaluación de desempeño.", "Error");
                            return;
                        }
                    }

                    // Eliminar el KPI si no está en la tabla Evaluacion_Desempeno
                    string deleteQuery = "DELETE FROM KPI WHERE id_kpi = @idKpi";
                    using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, con))
                    {
                        deleteCmd.Parameters.AddWithValue("@idKpi", idKpi);
                        con.Open();
                        deleteCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al eliminar el KPI: " + ex.Message, "Error");
            }
        }

    }
}
