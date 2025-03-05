using Microsoft.Data.SqlClient;

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
    }
}
