using Microsoft.Data.SqlClient;

namespace PayrollWeb.Models
{
    public class Meta
    {
        public int IdMeta { get; set; }
        public int IdEmpleado { get; set; }
        public string MetaDescripcion { get; set; }
        public string Estado { get; set; }
        public Empleado Empleado { get; set; }

        Conexion conexion = new Conexion();

        //MÉTODO PARA OBTENER TODAS LAS METAS DE UN EMPLEADO
        public List<Meta> ObtenerMetasDeEmpleado(int idEmpleado)
        {
            List<Meta> metas = new List<Meta>();
            SqlConnection con = conexion.GetConnection();
            conexion.OpenConnection(con);
            string query = "SELECT * FROM Meta WHERE id_empleado = @idEmpleado";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@idEmpleado", idEmpleado);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Meta meta = new Meta
                {
                    IdMeta = Convert.ToInt32(reader["id_meta"]),
                    IdEmpleado = Convert.ToInt32(reader["id_empleado"]),
                    MetaDescripcion = reader["meta_descripcion"].ToString(),
                    Estado = reader["estado"].ToString(),
                    Empleado = new Empleado().ObtenerEmpleado(Convert.ToInt32(reader["id_empleado"]))
                };
                metas.Add(meta);
            }
            conexion.CloseConnection(con);
            return metas;
        }

        //MÉTODO PARA AGREGAR UNA META
        public bool AgregarMeta()
        {
            SqlConnection con = conexion.GetConnection();
            conexion.OpenConnection(con);
            string query = "INSERT INTO Meta (id_empleado, meta_descripcion, estado) VALUES (@idEmpleado, @metaDescripcion, @estado)";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@idEmpleado", IdEmpleado);
            cmd.Parameters.AddWithValue("@metaDescripcion", MetaDescripcion);
            cmd.Parameters.AddWithValue("@estado", Estado);
            int result = cmd.ExecuteNonQuery();
            conexion.CloseConnection(con);
            return result > 0;
        }

        //MÉTODO PARA ACTUALIZAR UNA META
        public bool ActualizarMeta()
        {
            SqlConnection con = conexion.GetConnection();
            conexion.OpenConnection(con);
            string query = "UPDATE Meta SET meta_descripcion = @metaDescripcion, estado = @estado WHERE id_meta = @idMeta";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@metaDescripcion", MetaDescripcion);
            cmd.Parameters.AddWithValue("@estado", Estado);
            cmd.Parameters.AddWithValue("@idMeta", IdMeta);
            int result = cmd.ExecuteNonQuery();
            conexion.CloseConnection(con);
            return result > 0;
        }

        //MÉTODO PARA OBTENER UNA META
        public Meta ObtenerMeta(int idMeta)
        {
            Meta meta = new Meta();
            SqlConnection con = conexion.GetConnection();
            conexion.OpenConnection(con);
            string query = "SELECT * FROM Meta WHERE id_meta = @idMeta";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@idMeta", idMeta);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                meta.IdMeta = Convert.ToInt32(reader["id_meta"]);
                meta.IdEmpleado = Convert.ToInt32(reader["id_empleado"]);
                meta.MetaDescripcion = reader["meta_descripcion"].ToString();
                meta.Estado = reader["estado"].ToString();
                meta.Empleado = new Empleado().ObtenerEmpleado(Convert.ToInt32(reader["id_empleado"]));
            }
            conexion.CloseConnection(con);
            return meta;
        }

        //MÉTODO PARA ELIMINAR UNA META
        public bool EliminarMeta(int idMeta)
        {
            SqlConnection con = conexion.GetConnection();
            conexion.OpenConnection(con);
            string query = "DELETE FROM Meta WHERE id_meta = @idMeta";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@idMeta", idMeta);
            int result = cmd.ExecuteNonQuery();
            conexion.CloseConnection(con);
            return result > 0;
        }

    }
}
