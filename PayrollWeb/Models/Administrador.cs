using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

namespace PayrollWeb.Models
{
    public class Administrador
    {
        public int IdAdministrador { get; set; }
        public string Dui { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Contrasena { get; set; }

        public Administrador() { }

        Conexion conexion = new Conexion();

        public int LoginAdministrador(string correo, string contrasena)
        {
            int idAdministrador = -1;

            using (SqlConnection con = conexion.GetConnection())
            {
                try
                {
                    con.Open();
                    string query = "SELECT id_administrador FROM Administrador WHERE correo = @correo AND contrasena = @contrasena";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@correo", correo);
                        cmd.Parameters.AddWithValue("@contrasena", contrasena);

                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            idAdministrador = Convert.ToInt32(result);
                        }
                        else
                        {

                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al iniciar sesión: " + ex.Message, "Error");
                }
            }

            return idAdministrador;
        }

        //Método para obtener un administrador por su id
        public Administrador ObtenerAdministrador(int id)
        {
            Administrador administrador = new Administrador();
            using (SqlConnection con = conexion.GetConnection())
            {
                try
                {
                    con.Open();
                    string query = "SELECT * FROM Administrador WHERE id_administrador = @id";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                administrador.IdAdministrador = Convert.ToInt32(dr["id_administrador"]);
                                administrador.Dui = dr["dui"].ToString();
                                administrador.Nombre = dr["nombre"].ToString();
                                administrador.Apellidos = dr["apellidos"].ToString();
                                administrador.Telefono = dr["telefono"].ToString();
                                administrador.Correo = dr["correo"].ToString();
                                administrador.Contrasena = dr["contrasena"].ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener el administrador: " + ex.Message, "Error");
                }
            }
            return administrador;
        }

        public bool ActualizarDatosGenerales(int id, string dui, string nombre, string apellidos, string telefono)
        {
            using (SqlConnection con = conexion.GetConnection())
            {
                try
                {
                    con.Open();
                    string query = @"UPDATE Administrador 
                                     SET dui = @dui, nombre = @nombre, apellidos = @apellidos, telefono = @telefono 
                                     WHERE id_administrador = @id";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@dui", dui);
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        cmd.Parameters.AddWithValue("@apellidos", apellidos);
                        cmd.Parameters.AddWithValue("@telefono", telefono);
                        cmd.Parameters.AddWithValue("@id", id);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al actualizar datos generales: " + ex.Message);
                    return false;
                }
            }
        }

        public bool ActualizarDatosSensibles(int id, string correo, string nuevaContrasena)
        {
            using (SqlConnection con = conexion.GetConnection())
            {
                try
                {
                    con.Open();
                    string query;

                    if (!string.IsNullOrEmpty(nuevaContrasena))
                    {
                        query = "UPDATE Administrador SET correo = @correo, contrasena = @contrasena WHERE id_administrador = @id";
                    }
                    else
                    {
                        query = "UPDATE Administrador SET correo = @correo WHERE id_administrador = @id";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@correo", correo);
                        cmd.Parameters.AddWithValue("@id", id);

                        if (!string.IsNullOrEmpty(nuevaContrasena))
                        {
                            cmd.Parameters.AddWithValue("@contrasena", nuevaContrasena);
                        }

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al actualizar datos sensibles: " + ex.Message);
                    return false;
                }
            }
        }


        public bool ValidarPassword(int id, string contrasenaEncriptada)
        {
            using (SqlConnection con = conexion.GetConnection())
            {
                try
                {
                    con.Open();
                    string query = "SELECT COUNT(*) FROM Administrador WHERE id_administrador = @id AND contrasena = @contrasena";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@contrasena", contrasenaEncriptada);

                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al validar contraseña: " + ex.Message);
                    return false;
                }
            }
        }
        //contraseña encriptada

        public bool ValidarFormatoPassword(string password)
        {
            // Mínimo 8 caracteres, al menos una mayúscula, una minúscula, un número y un caracter especial
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$";
            return Regex.IsMatch(password, pattern);
        }

        //validar sin encrptar
        public bool ValidarPasswordSin(int id, string contrasenaTextoPlano)
        {
            using (SqlConnection con = conexion.GetConnection())
            {
                try
                {
                    con.Open();
                    string query = "SELECT COUNT(*) FROM Administrador WHERE id_administrador = @id AND contrasena = @contrasena";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@contrasena", contrasenaTextoPlano); // Sin encriptar
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al validar contraseña: " + ex.Message);
                    return false;
                }
            }
        }

    }
}
