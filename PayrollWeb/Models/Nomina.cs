// Models/Nomina.cs
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace PayrollWeb.Models
{
    public class Nomina
    {
        public int IdNomina { get; set; }
        public int IdEmpleado { get; set; }
        public DateTime FechaEmision { get; set; }
        public decimal TotalDeducciones { get; set; }
        public decimal TotalDevengos { get; set; }
        public decimal TotaNoSujetosDeRenta { get; set; }
        public decimal SalarioNeto { get; set; }
        public List<NominaDevengo> Devengos { get; set; }
        public List<NominaDeduccion> Deducciones { get; set; }
        public List<NominaNoSujeto> NoSujetos { get; set; }

        private Conexion conexion = new Conexion();

        public Nomina()
        {
            Devengos = new List<NominaDevengo>();
            Deducciones = new List<NominaDeduccion>();
            NoSujetos = new List<NominaNoSujeto>();
        }

        public bool GenerarNominas()
        {
            try
            {
                DateTime fechaHoy = DateTime.Today;

                using (SqlConnection con = conexion.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand("usp_GenerarNominaMensual", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Fecha", SqlDbType.Date).Value = fechaHoy;

                        con.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al generar las nóminas: " + ex.Message, "Error");
                return false;
            }
        }

        public static Nomina ObtenerDetalleNomina(int empleadoId, string mesAnno)
        {
            var nomina = new Nomina();
            using (SqlConnection con = new Conexion().GetConnection())
            {
                con.Open();

                // Obtener ID de nómina
                string query = @"
                    SELECT TOP 1 id_nomina
                    FROM Nomina
                    WHERE 
                        id_empleado = @EmpleadoId
                        AND CONVERT(CHAR(7), fecha_emision, 120) = @MesAnno
                    ORDER BY fecha_emision DESC, id_nomina DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@EmpleadoId", empleadoId);
                    cmd.Parameters.AddWithValue("@MesAnno", mesAnno);
                    var result = cmd.ExecuteScalar();
                    if (result == null) return null;
                    nomina.IdNomina = (int)result;
                }

                // Obtener datos principales
                query = "SELECT * FROM Nomina WHERE id_nomina = @IdNomina";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IdNomina", nomina.IdNomina);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            nomina.IdEmpleado = (int)reader["id_empleado"];
                            nomina.FechaEmision = (DateTime)reader["fecha_emision"];
                            nomina.TotalDeducciones = (decimal)reader["total_deducciones"];
                            nomina.TotalDevengos = (decimal)reader["total_devengos"];
                            nomina.TotaNoSujetosDeRenta = (decimal)reader["tota_no_sujetos_de_renta"];
                            nomina.SalarioNeto = (decimal)reader["salario_neto"];
                        }
                    }
                }

                // Obtener devengos
                query = "SELECT * FROM Nomina_devengos WHERE id_nomina = @IdNomina";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IdNomina", nomina.IdNomina);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            nomina.Devengos.Add(new NominaDevengo
                            {
                                IdNominaCargo = (int)reader["id_nomina_cargo"],
                                NombreDevengo = reader["nombre_devengo"].ToString(),
                                Monto = (decimal)reader["monto"]
                            });
                        }
                    }
                }

                // Obtener no sujetos
                query = "SELECT * FROM Nomina_no_sujetos_de_renta WHERE id_nomina = @IdNomina";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IdNomina", nomina.IdNomina);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            nomina.NoSujetos.Add(new NominaNoSujeto
                            {
                                IdNominaCargo = (int)reader["id_nomina_cargo"],
                                NombreDevengo = reader["nombre_devengo"].ToString(),
                                Monto = (decimal)reader["monto"]
                            });
                        }
                    }
                }

                // Obtener deducciones
                query = "SELECT * FROM Nomina_Deduccion WHERE id_nomina = @IdNomina";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IdNomina", nomina.IdNomina);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            nomina.Deducciones.Add(new NominaDeduccion
                            {
                                IdNominaDeduccion = (int)reader["id_nomina_deduccion"],
                                NombreDeduccion = reader["nombre_deduccion"].ToString(),
                                MontoDeduccion = (decimal)reader["monto_deduccion"]
                            });
                        }
                    }
                }
            }
            return nomina;
        }
    }

    public class NominaDevengo
    {
        public int IdNominaCargo { get; set; }
        public string NombreDevengo { get; set; }
        public decimal Monto { get; set; }
    }

    public class NominaNoSujeto
    {
        public int IdNominaCargo { get; set; }
        public string NombreDevengo { get; set; }
        public decimal Monto { get; set; }
    }

    public class NominaDeduccion
    {
        public int IdNominaDeduccion { get; set; }
        public string NombreDeduccion { get; set; }
        public decimal MontoDeduccion { get; set; }
    }
}