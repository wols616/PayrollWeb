using Microsoft.Data.SqlClient;

namespace PayrollWeb.Models
{
    public class Trienios
    {
        Conexion conexion = new Conexion();

        public string NombreCompleto { get; set; }
        public DateTime FechaInicio { get; set; }
        public int AnosTrabajando { get; set; }
        public decimal Monto { get; set; }

        //CONSTRUCTOR
        public Trienios(string nombreCompleto, DateTime fechaInicio, int anosTrabajando, decimal monto)
        {
            this.NombreCompleto = nombreCompleto;
            this.FechaInicio = fechaInicio;
            this.AnosTrabajando = anosTrabajando;
            this.Monto = monto;
        }

        public Trienios() { }

        // MÉTODO PARA OBTENER LOS TRIENIOS CON ESTADO 'S'
        public List<Trienios> MostrarTrienios()
        {
            List<Trienios> listaTrienios = new List<Trienios>();

            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = @"
                        SELECT 
                            CONCAT(e.nombre, ' ', e.apellidos) AS nombre_completo,
                            t.fecha_inicio,
                            DATEDIFF(YEAR, t.fecha_inicio, GETDATE()) AS años_trabajando,
                            t.monto
                        FROM 
                            trienios t
                        JOIN 
                            empleado e ON t.id_empleado = e.id_empleado
                        WHERE 
                            t.estado = 'S'";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Trienios trienio = new Trienios
                                {
                                    NombreCompleto = reader["nombre_completo"].ToString(),
                                    FechaInicio = Convert.ToDateTime(reader["fecha_inicio"]),
                                    AnosTrabajando = Convert.ToInt32(reader["años_trabajando"]),
                                    Monto = Convert.ToDecimal(reader["monto"])
                                };
                                listaTrienios.Add(trienio);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener los trienios: " + ex.Message);
            }

            return listaTrienios;
        }

        // MÉTODO PARA EJECUTAR EL PROCEDIMIENTO ALMACENADO
        public void CalcularTrienios()
        {
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    SqlCommand cmd = new SqlCommand("CalcularTrienios", con)
                    {
                        CommandType = System.Data.CommandType.StoredProcedure
                    };

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al calcular los trienios: " + ex.Message);
            }
        }
    }
}
