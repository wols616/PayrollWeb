// /Models/Bonificacion.cs
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace PayrollWeb.Models
{
    public class Bonificacion
    {
        public int IdBonificacion { get; set; }
        public int IdEmpleado { get; set; }
        public int CategoriaId { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }

        public bool EsAguinaldo { get; set; }
        public decimal SalarioBase { get; set; }
        public int AniosServicio { get; set; }
        public bool ParaTodosEmpleados { get; set; }

        private Conexion conexion = new Conexion();

        public Bonificacion() { }

        public List<Bonificacion> ObtenerTodas(int idEmpleado = 0)
        {
            var lista = new List<Bonificacion>();
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    // Cambié "mes" por "Fecha" en el SELECT y en ORDER BY
                    string sql = @"SELECT id_bonificacion, id_empleado, categoria_id, monto, Fecha 
                                   FROM Bonificacion ";
                    if (idEmpleado > 0)
                        sql += "WHERE id_empleado = @idempleado ";
                    sql += "ORDER BY Fecha DESC, id_bonificacion DESC";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        if (idEmpleado > 0)
                            cmd.Parameters.Add("@idempleado", SqlDbType.Int).Value = idEmpleado;

                        con.Open();
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                lista.Add(new Bonificacion
                                {
                                    IdBonificacion = Convert.ToInt32(rdr["id_bonificacion"]),
                                    IdEmpleado = Convert.ToInt32(rdr["id_empleado"]),
                                    CategoriaId = Convert.ToInt32(rdr["categoria_id"]),
                                    Monto = Convert.ToDecimal(rdr["monto"]),
                                    Fecha = Convert.ToDateTime(rdr["Fecha"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Podrías loguear el error para investigar
            }
            return lista;
        }

        public bool AgregarSimple(int idEmpleado, int categoriaId, decimal monto, DateTime fecha)
        {
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    // Cambié "mes" por "Fecha" en el INSERT
                    string sql = @"INSERT INTO Bonificacion 
                                   (id_bonificacion, id_empleado, categoria_id, monto, Fecha) 
                                   VALUES (@idbono, @idempleado, @categoria, @monto, @fecha)";
                    int nuevoId = ObtenerMaximoId() + 1;

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.Add("@idbono", SqlDbType.Int).Value = nuevoId;
                        cmd.Parameters.Add("@idempleado", SqlDbType.Int).Value = idEmpleado;
                        cmd.Parameters.Add("@categoria", SqlDbType.Int).Value = categoriaId;
                        cmd.Parameters.Add("@monto", SqlDbType.Decimal).Value = monto;
                        cmd.Parameters.Add("@fecha", SqlDbType.Date).Value = fecha;

                        con.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool AgregarAguinaldo(int idEmpleado, decimal salarioBase, int aniosServicio, DateTime fecha)
        {
            try
            {
                int dias;
                if (aniosServicio < 1)
                {
                    decimal fraccion = (decimal)aniosServicio;
                    decimal monto = (salarioBase / 365M) * (15M * fraccion);
                    return GuardarAguinaldoEnTabla(idEmpleado, monto, fecha);
                }
                else if (aniosServicio >= 1 && aniosServicio < 3)
                {
                    dias = 15;
                }
                else if (aniosServicio >= 3 && aniosServicio < 9)
                {
                    dias = 19;
                }
                else
                {
                    dias = 21;
                }

                decimal montoAguinaldo = (salarioBase / 30M) * dias;
                return GuardarAguinaldoEnTabla(idEmpleado, montoAguinaldo, fecha);
            }
            catch (Exception)
            {

                return false;
            }
        }

        private bool GuardarAguinaldoEnTabla(int idEmpleado, decimal monto, DateTime fecha)
        {
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    // Aquí también reemplacé "mes" por "Fecha"
                    string sql = @"INSERT INTO Bonificacion 
                                   (id_bonificacion, id_empleado, categoria_id, monto, Fecha) 
                                   VALUES (@idbono, @idempleado, @categoria, @monto, @fecha)";
                    int nuevoId = ObtenerMaximoId() + 1;
                    int categoriaAguinaldo = 1; // Asegúrate de que exista esa categoría con ID = 1

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.Add("@idbono", SqlDbType.Int).Value = nuevoId;
                        cmd.Parameters.Add("@idempleado", SqlDbType.Int).Value = idEmpleado;
                        cmd.Parameters.Add("@categoria", SqlDbType.Int).Value = categoriaAguinaldo;
                        cmd.Parameters.Add("@monto", SqlDbType.Decimal).Value = monto;
                        cmd.Parameters.Add("@fecha", SqlDbType.Date).Value = fecha;

                        con.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool AgregarATodos(int categoriaId, decimal monto, DateTime fecha)
        {
            try
            {
                var emp = new Empleado();
                var todos = emp.BuscarPorTexto(string.Empty);

                foreach (var e in todos)
                {
                    if (!AgregarSimple(e.IdEmpleado, categoriaId, monto, fecha))
                        return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Eliminar(int idBonificacion)
        {
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string sql = "DELETE FROM Bonificacion WHERE id_bonificacion = @id";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = idBonificacion;
                        con.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private int ObtenerMaximoId()
        {
            int max = 0;
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string sql = "SELECT ISNULL(MAX(id_bonificacion), 0) FROM Bonificacion";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        con.Open();
                        max = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception)
            {
                // Ignorar u ocurra excepción
            }
            return max;
        }
    }
}
