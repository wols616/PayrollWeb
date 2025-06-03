using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace PayrollWeb.Models
{
    public class ReporteAntiguedad
    {
        Conexion conexion = new Conexion();
        public List<object> ObtenerHistorialAscensos(int idEmpleado)
        {
            var data = new List<object>();

            using (SqlConnection conn = conexion.GetConnection())
            {
                conn.Open();
                string query = @"
                WITH SueldosConsecutivos AS (
                    SELECT
                        h.fecha,
                        COALESCE(e.nombre + ' ' + e.apellidos, 'Ninguno') AS nombre_completo,
                        COALESCE(e.dui, 'Ninguno') AS dui,
                        COALESCE(e.telefono, 'Ninguno') AS telefono,
                        COALESCE(e.direccion, 'Ninguno') AS direccion,
                        COALESCE(e.correo, 'Ninguno') AS correo,
                        COALESCE(p.sueldo_base, 0) AS sueldo_actual,
                        COALESCE(p.nombre_puesto, 'Ninguno') AS puesto_actual,
                        COALESCE(c.id_puesto, 0) AS id_puesto_actual,
                        COALESCE(ph.sueldo_base, 0) AS sueldo_historico,
                        COALESCE(ph.nombre_puesto, 'Ninguno') AS puesto_historico,
                        COALESCE(h.motivo, 'Ninguno') AS motivo,
                        COALESCE(c.vigente, 'N') AS vigente,
                        COALESCE(c.tipo_contrato, 'Ninguno') AS tipo_contrato,
                        COALESCE(c.fecha_alta, '1900-01-01') AS fecha_alta,
                        COALESCE(c.fecha_baja, '1900-01-01') AS fecha_baja,
                        -- Identificar cambios de sueldo
                        COALESCE(LAG(CASE WHEN c.vigente = 'S' THEN p.sueldo_base ELSE ph.sueldo_base END)
                            OVER (ORDER BY h.fecha), 0) AS sueldo_anterior,
                        -- Identificar cambios de puesto
                        COALESCE(LAG(CASE WHEN c.vigente = 'S' THEN p.nombre_puesto ELSE ph.nombre_puesto END)
                            OVER (ORDER BY h.fecha), 'Ninguno') AS puesto_anterior,
                        -- Calcular diferencia de sueldo
                        COALESCE(CASE WHEN c.vigente = 'S' THEN p.sueldo_base ELSE ph.sueldo_base END -
                        LAG(CASE WHEN c.vigente = 'S' THEN p.sueldo_base ELSE ph.sueldo_base END)
                            OVER (ORDER BY h.fecha), 0) AS diferencia_sueldo,
                        -- Porcentaje de aumento
                        COALESCE(CASE
                            WHEN LAG(CASE WHEN c.vigente = 'S' THEN p.sueldo_base ELSE ph.sueldo_base END)
                                 OVER (ORDER BY h.fecha) = 0 THEN 0
                            ELSE ((CASE WHEN c.vigente = 'S' THEN p.sueldo_base ELSE ph.sueldo_base END -
                                     LAG(CASE WHEN c.vigente = 'S' THEN p.sueldo_base ELSE ph.sueldo_base END)
                                         OVER (ORDER BY h.fecha)) * 100.0 /
                                     LAG(CASE WHEN c.vigente = 'S' THEN p.sueldo_base ELSE ph.sueldo_base END)
                                         OVER (ORDER BY h.fecha))
                        END, 0) AS porcentaje_aumento
                    FROM Historial_Contrato h
                    JOIN Contrato c ON h.id_contrato_nuevo = c.id_contrato
                    JOIN Empleado e ON c.id_empleado = e.id_empleado
                    JOIN Puesto p ON c.id_puesto = p.id_puesto
                    LEFT JOIN Puesto_Historico ph ON c.id_contrato = ph.id_contrato
                    WHERE c.id_empleado = @IdEmpleado AND h.cambio = 'Creación'
                ),
                SueldosFiltrados AS (
                    SELECT
                        fecha,
                        nombre_completo,
                        dui,
                        telefono,
                        direccion,
                        correo,
                        COALESCE(CASE WHEN vigente = 'S' THEN sueldo_actual ELSE sueldo_historico END, 0) AS sueldo_base,
                        COALESCE(CASE WHEN vigente = 'S' THEN puesto_actual ELSE puesto_historico END, 'Ninguno') AS nombre_puesto,
                        COALESCE(motivo, 'Ninguno') AS motivo,
                        COALESCE(vigente, 'N') AS vigente,
                        COALESCE(tipo_contrato, 'Ninguno') AS tipo_contrato,
                        COALESCE(fecha_alta, '1900-01-01') AS fecha_alta,
                        COALESCE(fecha_baja, '1900-01-01') AS fecha_baja,
                        COALESCE(sueldo_anterior, 0) AS sueldo_anterior,
                        COALESCE(puesto_anterior, 'Ninguno') AS puesto_anterior,
                        COALESCE(diferencia_sueldo, 0) AS diferencia_sueldo,
                        COALESCE(porcentaje_aumento, 0) AS porcentaje_aumento,
                        -- Identificar si fue ascenso (cambio de puesto)
                        CASE WHEN COALESCE(puesto_anterior, 'Ninguno') IS NOT NULL AND
                                     COALESCE(puesto_anterior, 'Ninguno') != 'Ninguno' AND
                                     (CASE WHEN vigente = 'S' THEN COALESCE(puesto_actual, 'Ninguno') ELSE COALESCE(puesto_historico, 'Ninguno') END) != COALESCE(puesto_anterior, 'Ninguno')
                                THEN 'S' ELSE 'N' END AS es_ascenso,
                        -- Identificar si fue aumento de sueldo
                        CASE WHEN COALESCE(sueldo_anterior, 0) IS NOT NULL AND
                                     (CASE WHEN vigente = 'S' THEN COALESCE(sueldo_actual, 0) ELSE COALESCE(sueldo_historico, 0) END) > COALESCE(sueldo_anterior, 0)
                                THEN 'S' ELSE 'N' END AS es_aumento
                    FROM SueldosConsecutivos
                    WHERE
                        -- Mostrar primera fila siempre
                        sueldo_anterior IS NULL
                        -- O cuando hay cambio de sueldo o de puesto
                        OR (CASE WHEN vigente = 'S' THEN COALESCE(sueldo_actual, 0) ELSE COALESCE(sueldo_historico, 0) END) != COALESCE(sueldo_anterior, 0)
                        OR (CASE WHEN vigente = 'S' THEN COALESCE(puesto_actual, 'Ninguno') ELSE COALESCE(puesto_historico, 'Ninguno') END) != COALESCE(puesto_anterior, 'Ninguno')
                )
                SELECT
                    COALESCE(fecha, '1900-01-01') AS fecha,
                    COALESCE(nombre_completo, 'Ninguno') AS nombre_completo,
                    COALESCE(dui, 'Ninguno') AS dui,
                    COALESCE(telefono, 'Ninguno') AS telefono,
                    COALESCE(direccion, 'Ninguno') AS direccion,
                    COALESCE(correo, 'Ninguno') AS correo,
                    COALESCE(sueldo_base, 0) AS sueldo_base,
                    COALESCE(sueldo_anterior, 0) AS sueldo_anterior,
                    COALESCE(diferencia_sueldo, 0) AS diferencia_sueldo,
                    COALESCE(porcentaje_aumento, 0) AS porcentaje_aumento,
                    COALESCE(nombre_puesto, 'Ninguno') AS puesto_actual,
                    COALESCE(puesto_anterior, 'Ninguno') AS puesto_anterior,
                    COALESCE(motivo, 'Ninguno') AS motivo,
                    COALESCE(tipo_contrato, 'Ninguno') AS tipo_contrato,
                    COALESCE(fecha_alta, '1900-01-01') AS fecha_alta,
                    COALESCE(fecha_baja, '1900-01-01') AS fecha_baja,
                    COALESCE(vigente, 'N') AS vigente,
                    COALESCE(es_ascenso, 'N') AS es_ascenso,
                    COALESCE(es_aumento, 'N') AS es_aumento
                FROM SueldosFiltrados
                ORDER BY fecha ASC;";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@IdEmpleado", idEmpleado);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data.Add(new
                            {
                                fecha = reader["fecha"],
                                nombreCompleto = reader["nombre_completo"],
                                dui = reader["dui"],
                                telefono = reader["telefono"],
                                direccion = reader["direccion"],
                                correo = reader["correo"],
                                sueldoBase = reader["sueldo_base"],
                                sueldoAnterior = GetNullableDecimal(reader["sueldo_anterior"]),
                                aumentoMonto = GetNullableDecimal(reader["diferencia_sueldo"]),
                                aumentoPorcentaje = GetNullableDecimal(reader["porcentaje_aumento"]),
                                puestoActual = reader["puesto_actual"],
                                puestoAnterior = GetNullableString(reader["puesto_anterior"]),
                                motivo = reader["motivo"],
                                tipoContrato = reader["tipo_contrato"],
                                fechaInicioContrato = reader["fecha_alta"],
                                fechaFinContrato = GetNullableDateTime(reader["fecha_baja"]),
                                vigente = reader["vigente"]
                            });
                        }
                    }
                }
            }

            return data;
        }

        // Helper functions to safely handle nullable values from the database
        private decimal? GetNullableDecimal(object value)
        {
            if (value != DBNull.Value && value is decimal decimalValue)
            {
                return decimalValue;
            }
            return null;
        }

        private DateTime? GetNullableDateTime(object value)
        {
            if (value != DBNull.Value && value is DateTime dateTimeValue)
            {
                return dateTimeValue;
            }
            return null;
        }

        private string GetNullableString(object value)
        {
            if (value != DBNull.Value && value is string stringValue)
            {
                return stringValue;
            }
            return null;
        }
    }
}