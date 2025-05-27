using Microsoft.Data.SqlClient;

namespace PayrollWeb.Models
{
    public class ReporteCompetenciasHabilidades
    {
        Conexion conexion = new Conexion();

        public List<object> ObtenerCompetenciasYHabilidadesEmpleado(int idEmpleado)
        {
            var data = new List<object>();

            using (SqlConnection conn = conexion.GetConnection())
            {
                conn.Open();
                string query = @"
                SELECT 
                    e.nombre + ' ' + e.apellidos AS nombre_completo,
                    e.dui,
                    e.telefono,
                    e.direccion,
                    e.correo,
                    c.nombre AS competencia,
                    NULL AS habilidad
                FROM Competencia_Empleado ce
                JOIN Empleado e ON ce.id_empleado = e.id_empleado
                JOIN Competencia c ON ce.id_competencia = c.id_competencia
                WHERE ce.id_empleado = @IdEmpleado

                UNION ALL

                SELECT 
                    e.nombre + ' ' + e.apellidos AS nombre_completo,
                    e.dui,
                    e.telefono,
                    e.direccion,
                    e.correo,
                    NULL AS competencia,
                    h.nombre AS habilidad
                FROM Habilidad_Empleado he
                JOIN Empleado e ON he.id_empleado = e.id_empleado
                JOIN Habilidad h ON he.id_habilidad = h.id_habilidad
                WHERE he.id_empleado = @IdEmpleado
                ORDER BY competencia, habilidad";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@IdEmpleado", idEmpleado);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data.Add(new
                            {
                                nombreCompleto = reader["nombre_completo"],
                                dui = reader["dui"],
                                telefono = reader["telefono"],
                                direccion = reader["direccion"],
                                correo = reader["correo"],
                                competencia = reader["competencia"] != DBNull.Value ? reader["competencia"].ToString() : null,
                                habilidad = reader["habilidad"] != DBNull.Value ? reader["habilidad"].ToString() : null
                            });
                        }
                    }
                }
            }

            return data;
        }
    }
}
