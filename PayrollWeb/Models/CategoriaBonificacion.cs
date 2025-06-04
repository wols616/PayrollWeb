using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace PayrollWeb.Models
{
    public class CategoriaBonificacion
    {
        public int IdCategoriaBono { get; set; }
        public string Nombre { get; set; }

        private Conexion conexion = new Conexion();

        public CategoriaBonificacion() { }

        public List<CategoriaBonificacion> ObtenerTodas()
        {
            var lista = new List<CategoriaBonificacion>();
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string sql = "SELECT id_categoria_bono, nombre FROM Categoria_bonificacion WHERE id_categoria_bono > 1 ORDER BY nombre";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        con.Open();
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                lista.Add(new CategoriaBonificacion
                                {
                                    IdCategoriaBono = Convert.ToInt32(rdr["id_categoria_bono"]),
                                    Nombre = rdr["nombre"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Manejo de errores
            }
            return lista;
        }

        /// <summary>
        /// Agrega una nueva categoría.
        /// </summary>
        public bool Agregar(string nombre)
        {
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string sql = "INSERT INTO Categoria_bonificacion (id_categoria_bono, nombre) VALUES (@id, @nombre)";
                    // Se asume que se maneja identity en BD, pero en tu script pones PK sin IDENTITY.
                    // Si quieres auto-incremental, modifica la tabla; si no, calcula un nuevo id aquí:
                    int nuevoId = ObtenerMaximoId() + 1;
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = nuevoId;
                        cmd.Parameters.Add("@nombre", SqlDbType.VarChar).Value = nombre;
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

        /// Edita el nombre de una categoría existente.
        public bool Editar(int id, string nombre)
        {
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string sql = "UPDATE Categoria_bonificacion SET nombre = @nombre WHERE id_categoria_bono = @id";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.Add("@nombre", SqlDbType.VarChar).Value = nombre;
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
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

        /// Elimina una categoría (y opcionalmente los bonos asociados, si lo desean).
        public bool Eliminar(int id)
        {
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    // Primero, opcionalmente comprobar si hay bonificaciones con esta categoría antes de eliminar. 
                    // En este ejemplo, si existen, cancelamos la operación:
                    string validar = "SELECT COUNT(*) FROM Bonificacion WHERE categoria_id = @id";
                    using (SqlCommand cmdVal = new SqlCommand(validar, con))
                    {
                        cmdVal.Parameters.Add("@id", SqlDbType.Int).Value = id;
                        con.Open();
                        int count = Convert.ToInt32(cmdVal.ExecuteScalar());
                        con.Close();
                        if (count > 0) return false; // No dejar eliminar si hay bonos asociados
                    }

                    string sql = "DELETE FROM Categoria_bonificacion WHERE id_categoria_bono = @id";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
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
                    string sql = "SELECT ISNULL(MAX(id_categoria_bono), 0) FROM Categoria_bonificacion";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        con.Open();
                        max = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception)
            {
            }
            return max;
        }
    }
}
