using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace PayrollWeb.Models
{
    public class ReporteEvaluacionEmpleado
    {
        Conexion conexion = new Conexion();

        public List<object> ObtenerEvaluacionEmpleado(int idEmpleado)
        {
            var data = new List<object>();

            using (SqlConnection conn = conexion.GetConnection())
            {
                conn.Open();

                // Obtener información personal del empleado
                string infoQuery = @"
                SELECT 
                    nombre + ' ' + apellidos AS nombre_completo,
                    dui,
                    telefono,
                    direccion,
                    correo
                FROM Empleado
                WHERE id_empleado = @IdEmpleado";

                dynamic empleadoInfo = null;

                using (SqlCommand cmd = new SqlCommand(infoQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@IdEmpleado", idEmpleado);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            empleadoInfo = new
                            {
                                nombreCompleto = reader["nombre_completo"].ToString(),
                                dui = reader["dui"].ToString(),
                                telefono = reader["telefono"].ToString(),
                                direccion = reader["direccion"].ToString(),
                                correo = reader["correo"].ToString()
                            };
                        }
                    }
                }

                if (empleadoInfo == null)
                {
                    // No existe el empleado
                    return data;
                }

                // Obtener evaluaciones del empleado en una fecha específica
                string evalQuery = @"
                SELECT 
                    k.nombre AS kpi,
                    ed.puntuacion,
                    ed.fecha
                FROM Evaluacion_Desempeno ed
                JOIN KPI k ON ed.id_kpi = k.id_kpi
                WHERE ed.id_empleado = @IdEmpleado";

                using (SqlCommand cmd = new SqlCommand(evalQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@IdEmpleado", idEmpleado);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data.Add(new
                            {
                                nombreCompleto = empleadoInfo.nombreCompleto,
                                dui = empleadoInfo.dui,
                                telefono = empleadoInfo.telefono,
                                direccion = empleadoInfo.direccion,
                                correo = empleadoInfo.correo,
                                kpi = reader["kpi"].ToString(),
                                puntuacion = Convert.ToInt32(reader["puntuacion"]),
                                fecha = Convert.ToDateTime(reader["fecha"]).ToString("yyyy-MM-dd") // o el formato que desees
                            });

                        }
                    }
                }

                // Si no hay evaluaciones, al menos devolvemos datos personales
                if (data.Count == 0)
                {
                    data.Add(new
                    {
                        nombreCompleto = empleadoInfo.nombreCompleto,
                        dui = empleadoInfo.dui,
                        telefono = empleadoInfo.telefono,
                        direccion = empleadoInfo.direccion,
                        correo = empleadoInfo.correo,
                        kpi = (string)null,
                        puntuacion = (int?)null
                    });

                }
            }

            return data;
        }
    }
}
