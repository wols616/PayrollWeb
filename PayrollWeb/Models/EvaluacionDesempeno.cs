using Microsoft.Data.SqlClient;
namespace PayrollWeb.Models
{
    public class EvaluacionDesempeno
    {
        public int IdEvaluacionDesempeno { get; set; }
        public int id_empleado { get; set; }
        public DateTime fecha { get; set; }
        public int id_kpi { get; set; }
        public int puntuacion { get; set; }

        Conexion conexion = new Conexion();

        //Método para obtener todas las evaluaciones de desempeño
        public List<EvaluacionDesempeno> ObtenerEvaluacionesDesempeno()
        {
            List<EvaluacionDesempeno> listaEvaluacionesDesempeno = new List<EvaluacionDesempeno>();
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = "SELECT * FROM Evaluacion_Desempeno";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                EvaluacionDesempeno evaluacionDesempeno = new EvaluacionDesempeno
                                {
                                    IdEvaluacionDesempeno = Convert.ToInt32(reader["id_evaluacion_desempeno"]),
                                    id_empleado = Convert.ToInt32(reader["id_empleado"]),
                                    fecha = Convert.ToDateTime(reader["fecha"]),
                                    id_kpi = Convert.ToInt32(reader["id_kpi"]),
                                    puntuacion = Convert.ToInt32(reader["puntuacion"])
                                };
                                listaEvaluacionesDesempeno.Add(evaluacionDesempeno);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener las evaluaciones de desempeño: " + ex.Message, "Error");
            }
            return listaEvaluacionesDesempeno;
        }

        //Método para obtener todas las evaluaciones de desempeño de un empleado
        public List<EvaluacionDesempenoViewModel> ObtenerEvaluacionesDeEmpleado(int idEmpleado)
        {
            List<EvaluacionDesempenoViewModel> listaEvaluacionesDesempeno = new List<EvaluacionDesempenoViewModel>();
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = "SELECT * FROM Evaluacion_Desempeno WHERE id_empleado = @idEmpleado";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@idEmpleado", idEmpleado);
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                EvaluacionDesempenoViewModel evaluacionDesempenoViewModel = new EvaluacionDesempenoViewModel
                                {
                                    IdEvaluacionDesempeno = Convert.ToInt32(reader["id_evaluacion_desempeno"]),
                                    id_empleado = Convert.ToInt32(reader["id_empleado"]),
                                    fecha = Convert.ToDateTime(reader["fecha"]),
                                    id_kpi = Convert.ToInt32(reader["id_kpi"]),
                                    puntuacion = Convert.ToInt32(reader["puntuacion"]),
                                    Empleado = new Empleado().ObtenerEmpleado(Convert.ToInt32(reader["id_empleado"])),
                                    KPI = new KPI().ObtenerKPI(Convert.ToInt32(reader["id_kpi"]))
                                };
                                listaEvaluacionesDesempeno.Add(evaluacionDesempenoViewModel);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener las evaluaciones de desempeño: " + ex.Message, "Error");
            }
            return listaEvaluacionesDesempeno;
        }

        //Método para obtener una evaluación de desempeño por su ID
        public EvaluacionDesempeno ObtenerEvaluacionDesempeno(int idEvaluacionDesempeno)
        {
            EvaluacionDesempeno evaluacionDesempeno = new EvaluacionDesempeno();
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = "SELECT * FROM Evaluacion_Desempeno WHERE id_evaluacion_desempeno = @idEvaluacionDesempeno";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@idEvaluacionDesempeno", idEvaluacionDesempeno);
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                evaluacionDesempeno.IdEvaluacionDesempeno = Convert.ToInt32(reader["id_evaluacion_desempeno"]);
                                evaluacionDesempeno.id_empleado = Convert.ToInt32(reader["id_empleado"]);
                                evaluacionDesempeno.fecha = Convert.ToDateTime(reader["fecha"]);
                                evaluacionDesempeno.id_kpi = Convert.ToInt32(reader["id_kpi"]);
                                evaluacionDesempeno.puntuacion = Convert.ToInt32(reader["puntuacion"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener la evaluación de desempeño: " + ex.Message, "Error");
            }
            return evaluacionDesempeno;
        }

        //Método para agregar una evaluación de desempeño
        public bool AgregarEvaluacionDesempeno(EvaluacionDesempeno evaluacionDesempeno)
        {
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = "INSERT INTO Evaluacion_Desempeno (id_empleado, fecha, id_kpi, puntuacion) VALUES (@idEmpleado, @fecha, @idKpi, @puntuacion)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@idEmpleado", evaluacionDesempeno.id_empleado);
                        cmd.Parameters.AddWithValue("@fecha", evaluacionDesempeno.fecha);
                        cmd.Parameters.AddWithValue("@idKpi", evaluacionDesempeno.id_kpi);
                        cmd.Parameters.AddWithValue("@puntuacion", evaluacionDesempeno.puntuacion);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al agregar la evaluación de desempeño: " + ex.Message, "Error");
                return false;
            }
        }

        //Método para actualizar una evaluación de desempeño
        public bool ActualizarEvaluacionDesempeno(EvaluacionDesempeno evaluacionDesempeno)
        {
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = "UPDATE Evaluacion_Desempeno SET fecha = @fecha, id_kpi = @idKpi, puntuacion = @puntuacion WHERE id_evaluacion_desempeno = @idEvaluacionDesempeno";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@fecha", evaluacionDesempeno.fecha);
                        cmd.Parameters.AddWithValue("@idKpi", evaluacionDesempeno.id_kpi);
                        cmd.Parameters.AddWithValue("@puntuacion", evaluacionDesempeno.puntuacion);
                        cmd.Parameters.AddWithValue("@idEvaluacionDesempeno", evaluacionDesempeno.IdEvaluacionDesempeno);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al actualizar la evaluación de desempeño: " + ex.Message, "Error");
                return false;
            }
        }
    }

    public class EvaluacionDesempenoViewModel
    {
        public int IdEvaluacionDesempeno { get; set; }
        public int id_empleado { get; set; }
        public DateTime fecha { get; set; }
        public int id_kpi { get; set; }
        public int puntuacion { get; set; }
        public Empleado Empleado { get; set; }
        public KPI KPI { get; set; }
    }
}
