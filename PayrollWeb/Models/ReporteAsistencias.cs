using Microsoft.Data.SqlClient;

namespace PayrollWeb.Models
{
    public class ReporteAsistencias
    {
        Conexion conexion = new Conexion();

        public List<object> ObtenerAsistenciasEmpleado(int idEmpleado)
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
                        a.fecha,
                        a.hora_entrada,
                        a.hora_salida,
                        a.ausencia
                    FROM Asistencia a
                    JOIN Empleado e ON a.id_empleado = e.id_empleado
                    WHERE a.id_empleado = @IdEmpleado
                    ORDER BY a.fecha DESC";

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
                                fecha = Convert.ToDateTime(reader["fecha"]),
                                horaEntrada = reader["hora_entrada"].ToString(),
                                horaSalida = reader["hora_salida"].ToString(),
                                ausencia = reader["ausencia"].ToString()
                            });
                        }
                    }
                }
            }

            return data;
        }
    }
}
