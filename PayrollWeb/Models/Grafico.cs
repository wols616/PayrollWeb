using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

namespace PayrollWeb.Models
{
    public class Grafico
    {
        Conexion conexion = new Conexion();
        public List<Object> ObtenerSalarioPorCategoria()
        {
            var data = new List<object>();

            using (SqlConnection conn = conexion.GetConnection())
            {
                conn.Open();
                string query = @"
                SELECT c.nombre_categoria, AVG(p.sueldo_base) AS sueldo_promedio
                FROM Puesto p
                JOIN Categoria c ON p.id_categoria = c.id_categoria
                GROUP BY c.nombre_categoria";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.Add(new
                        {
                            Categoria = reader["nombre_categoria"].ToString(),
                            SueldoPromedio = reader["sueldo_promedio"]
                        });
                    }
                }
            }

            return data;
        }

        public List<object> ObtenerDatosParaGraficaPuestos()
        {
            var data = new List<object>();

            using (SqlConnection conn = conexion.GetConnection())
            {
                conn.Open();

                string query = @"
        SELECT
            p.id_puesto,
            p.nombre_puesto,
            p.sueldo_base,
            (
                SELECT
                    cp.nombre_complemento AS [nombre],
                    cp.monto AS [monto]
                FROM Complemento_puesto cp
                WHERE cp.id_puesto = p.id_puesto
                FOR JSON PATH
            ) AS complementos_json
        FROM Puesto p
        ORDER BY p.nombre_puesto;";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var complementosJson = reader["complementos_json"].ToString();
                        List<Complemento> complementos = new List<Complemento>();

                        if (!string.IsNullOrEmpty(complementosJson))
                        {
                            try
                            {
                                complementos = JsonConvert.DeserializeObject<List<Complemento>>(complementosJson);
                            }
                            catch (Exception ex)
                            {
                                // Logear el error si es necesario
                                Console.WriteLine($"Error al deserializar complementos: {ex.Message}");
                            }
                        }

                        data.Add(new
                        {
                            idPuesto = Convert.ToInt32(reader["id_puesto"]),
                            nombrePuesto = reader["nombre_puesto"].ToString(),
                            sueldoBase = Convert.ToDecimal(reader["sueldo_base"]),
                            complementos = complementos
                        });
                    }
                }
            }

            return data;
        }

        // Clase auxiliar para deserialización
        public class Complemento
        {
            public string nombre { get; set; }
            public string monto { get; set; }
        }

        public List<Object> ObtenerHistorialAscensos(int idEmpleado)
        {
            var data = new List<object>();

            using (SqlConnection conn = conexion.GetConnection())
            {
                conn.Open();
                string query = @"
                    WITH SueldosConsecutivos AS (
                    SELECT 
                        h.fecha, 
                        p.sueldo_base AS sueldo_actual,
                        p.nombre_puesto AS puesto_actual,
                        ph.sueldo_base AS sueldo_historico,
                        ph.nombre_puesto AS puesto_historico,
                        h.motivo, 
                        c.vigente,
                        -- Identificar cambios de sueldo
                        LAG(CASE WHEN c.vigente = 'S' THEN p.sueldo_base ELSE ph.sueldo_base END) 
                            OVER (ORDER BY h.fecha) AS sueldo_anterior,
                        -- Identificar cambios de puesto
                        LAG(CASE WHEN c.vigente = 'S' THEN p.nombre_puesto ELSE ph.nombre_puesto END) 
                            OVER (ORDER BY h.fecha) AS puesto_anterior
                    FROM Historial_Contrato h
                    JOIN Contrato c ON h.id_contrato_nuevo = c.id_contrato
                    JOIN Puesto p ON c.id_puesto = p.id_puesto
                    LEFT JOIN Puesto_Historico ph ON c.id_contrato = ph.id_contrato
                    WHERE c.id_empleado = @IdEmpleado AND h.cambio = 'Creación'
                ),
                SueldosFiltrados AS (
                    SELECT 
                        fecha,
                        CASE WHEN vigente = 'S' THEN sueldo_actual ELSE sueldo_historico END AS sueldo_base,
                        CASE WHEN vigente = 'S' THEN puesto_actual ELSE puesto_historico END AS nombre_puesto,
                        motivo,
                        vigente,
                        sueldo_anterior,
                        puesto_anterior
                    FROM SueldosConsecutivos
                    WHERE 
                        -- Mostrar primera fila siempre
                        sueldo_anterior IS NULL 
                        -- O cuando hay cambio de sueldo o de puesto
                        OR (CASE WHEN vigente = 'S' THEN sueldo_actual ELSE sueldo_historico END) != sueldo_anterior
                        OR (CASE WHEN vigente = 'S' THEN puesto_actual ELSE puesto_historico END) != puesto_anterior
                )
                SELECT 
                    fecha,
                    sueldo_base,
                    nombre_puesto,
                    motivo,
                    vigente
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
                                sueldoBase = reader["sueldo_base"],
                                nombrePuesto = reader["nombre_puesto"],
                                motivo = reader["motivo"],
                                vigente = reader["vigente"]
                            });
                        }
                    }
                }
            }

            return data;
        }

        public List <Object> ObtenerHistorialAscensosGlobal()
        {
            var data = new List<object>();

            using (SqlConnection conn = conexion.GetConnection())
            {
                conn.Open();
                string query = @"
                        WITH SueldosConsecutivos AS (
                        SELECT
                            e.nombre,
                            e.apellidos,
                            h.fecha, 
                            p.sueldo_base AS sueldo_actual,
                            p.nombre_puesto AS puesto_actual,
                            ph.sueldo_base AS sueldo_historico,
                            ph.nombre_puesto AS puesto_historico,
                            h.motivo, 
                            c.vigente,
                            -- Identificar cambios de sueldo
                            LAG(CASE WHEN c.vigente = 'S' THEN p.sueldo_base ELSE ph.sueldo_base END) 
                                OVER (PARTITION BY e.id_empleado ORDER BY h.fecha) AS sueldo_anterior,
                            -- Identificar cambios de puesto
                            LAG(CASE WHEN c.vigente = 'S' THEN p.nombre_puesto ELSE ph.nombre_puesto END) 
                                OVER (PARTITION BY e.id_empleado ORDER BY h.fecha) AS puesto_anterior,
                            -- Numerar los contratos por empleado para identificar el primero
                            ROW_NUMBER() OVER (PARTITION BY e.id_empleado ORDER BY h.fecha) AS num_contrato
                        FROM Historial_Contrato h
                        JOIN Contrato c ON h.id_contrato_nuevo = c.id_contrato
                        JOIN Puesto p ON c.id_puesto = p.id_puesto
                        LEFT JOIN Puesto_Historico ph ON c.id_contrato = ph.id_contrato
                        JOIN Empleado e ON e.id_empleado = c.id_empleado
                        WHERE h.cambio = 'Creación'
                    ),
                    SueldosFiltrados AS (
                        SELECT
                            nombre,
                            apellidos,
                            fecha,
                            CASE WHEN vigente = 'S' THEN sueldo_actual ELSE sueldo_historico END AS sueldo_base,
                            CASE WHEN vigente = 'S' THEN puesto_actual ELSE puesto_historico END AS nombre_puesto,
                            motivo,
                            vigente,
                            sueldo_anterior,
                            puesto_anterior,
                            num_contrato
                        FROM SueldosConsecutivos
                        WHERE 
                            -- Excluir el primer contrato de cada empleado
                            num_contrato > 1
                            -- O incluir cuando hay un cambio de sueldo o de puesto
                            AND (
                                sueldo_anterior IS NULL 
                                OR (CASE WHEN vigente = 'S' THEN sueldo_actual ELSE sueldo_historico END) != sueldo_anterior
                                OR (CASE WHEN vigente = 'S' THEN puesto_actual ELSE puesto_historico END) != puesto_anterior
                            )
                    )
                    SELECT
                        nombre,
                        apellidos,
                        fecha,
                        sueldo_base,
                        nombre_puesto,
                        motivo,
                        vigente
                    FROM SueldosFiltrados
                    ORDER BY fecha ASC;";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data.Add(new
                            {
                                nombre = reader["nombre"],
                                apellidos = reader["apellidos"],
                                fecha = reader["fecha"],
                                sueldoBase = reader["sueldo_base"],
                                nombrePuesto = reader["nombre_puesto"],
                                motivo = reader["motivo"],
                                vigente = reader["vigente"]
                            });
                        }
                    }
                }
            }

            return data;

        }

        public List<object> ObtenerEvaluaciones(int idEmpleado)
        {
            var data = new List<object>();

            using (SqlConnection conn = conexion.GetConnection())
            {
                conn.Open();
                string query = @"
                SELECT KPI.nombre, puntuacion 
                FROM Evaluacion_Desempeno 
                JOIN KPI ON KPI.id_kpi = Evaluacion_Desempeno.id_kpi
                WHERE id_empleado = @IdEmpleado";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@IdEmpleado", idEmpleado);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data.Add(new
                            {
                                nombre = reader["nombre"].ToString(),
                                puntuacion = Convert.ToInt32(reader["puntuacion"])
                            });
                        }
                    }
                }
            }

            return data;
        }

    }
}
