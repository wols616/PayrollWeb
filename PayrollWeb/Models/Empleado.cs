using Microsoft.Data.SqlClient;
using static System.Net.Mime.MediaTypeNames;
using System.Data;
using System.Text;

namespace PayrollWeb.Models
{
    public class Empleado
    {
        public int IdEmpleado { get; set; }
        public string Dui { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string CuentaCorriente { get; set; }
        public string Estado { get; set; }
        public string Correo { get; set; }
        public string Contrasena { get; set; }

        public Empleado(string dui, string nombre, string apellidos, string telefono, string direccion, string cuentaCorriente, string estado, string correo, string contrasena)
        {
            Dui = dui;
            Nombre = nombre;
            Apellidos = apellidos;
            Telefono = telefono;
            Direccion = direccion;
            CuentaCorriente = cuentaCorriente;
            Estado = estado;
            Correo = correo;
            Contrasena = contrasena;
            conexion = new Conexion();
        }

        public Empleado() { }

        //------------------------ Métodos a usar ----------------------------------------------------------

        Conexion conexion = new Conexion();


        //Agregar
        public bool AgregarEmpleado()
        {
            // Hacer validaciones

            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = "INSERT INTO Empleado (dui, nombre, apellidos, telefono, direccion, cuenta_corriente, estado, correo, contrasena ) " +
                                   "VALUES (@dui, @nombre, @apellidos, @telefono, @direccion, @cuenta_corriente, @estado, @correo, @contrasena)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@dui", SqlDbType.VarChar).Value = Dui;
                        cmd.Parameters.Add("@nombre", SqlDbType.VarChar).Value = Nombre;
                        cmd.Parameters.Add("@apellidos", SqlDbType.VarChar).Value = Apellidos;
                        cmd.Parameters.Add("@telefono", SqlDbType.VarChar).Value = Telefono;
                        cmd.Parameters.Add("@direccion", SqlDbType.VarChar).Value = Direccion;
                        cmd.Parameters.Add("@cuenta_corriente", SqlDbType.VarChar).Value = CuentaCorriente;
                        cmd.Parameters.Add("@estado", SqlDbType.VarChar).Value = Estado;
                        cmd.Parameters.Add("@correo", SqlDbType.VarChar).Value = Correo;
                        cmd.Parameters.Add("@contrasena", SqlDbType.VarChar).Value = Contrasena;

                        con.Open();
                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            Console.WriteLine("Empleado agregado correctamente", "Éxito");
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("No se pudo agregar el empleado", "Error");
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al agregar el empleado: " + ex.Message, "Error");
                return false;
            }
        }

        public bool EliminarEmpleado(int idEmpleado)
        {
            using (SqlConnection connection = conexion.GetConnection())
            {
                connection.Open();

                // Verificar si el empleado tiene contratos o deducciones asociados
                string queryVerificar = @"
                    IF EXISTS (SELECT 1 FROM Contrato WHERE id_empleado = @idEmpleado)
                        OR EXISTS (SELECT 1 FROM Deduccion_Personal WHERE id_empleado = @idEmpleado)
                    BEGIN
                        SELECT 1; -- Si tiene registros asociados, no eliminar
                    END
                    ELSE
                    BEGIN
                        SELECT 0; -- Si no tiene registros asociados, se puede eliminar
                    END";

                SqlCommand commandVerificar = new SqlCommand(queryVerificar, connection);
                commandVerificar.Parameters.AddWithValue("@idEmpleado", idEmpleado);

                var result = commandVerificar.ExecuteScalar();

                if (Convert.ToInt32(result) == 1)
                {
                    return false; // El empleado no puede ser eliminado porque tiene registros asociados
                }

                // Eliminar al empleado si no tiene registros asociados
                string queryEliminar = "DELETE FROM Empleado WHERE id_empleado = @idEmpleado";

                SqlCommand commandEliminar = new SqlCommand(queryEliminar, connection);
                commandEliminar.Parameters.AddWithValue("@idEmpleado", idEmpleado);

                int rowsAffected = commandEliminar.ExecuteNonQuery();

                return rowsAffected > 0; // Retorna true si se eliminó correctamente, de lo contrario false
            }
        }

        public Empleado ObtenerEmpleadoPorCorreo(string correo)
        {
            Empleado empleado = null;

            using (SqlConnection con = conexion.GetConnection())
            {
                try
                {
                    con.Open();
                    string query = "SELECT * FROM Empleado WHERE correo = @correo";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@correo", correo);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                empleado = new Empleado
                                {
                                    IdEmpleado = Convert.ToInt32(reader["id_empleado"]),
                                    Dui = reader["dui"].ToString(),
                                    Nombre = reader["nombre"].ToString(),
                                    Apellidos = reader["apellidos"].ToString(),
                                    Telefono = reader["telefono"].ToString(),
                                    Direccion = reader["direccion"].ToString(),
                                    CuentaCorriente = reader["cuenta_corriente"].ToString(),
                                    Estado = reader["estado"].ToString(),
                                    Correo = reader["correo"].ToString(),
                                    Contrasena = reader["contrasena"].ToString()
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al buscar empleado: " + ex.Message);
                }
            }

            return empleado;
        }


        //Mostrar
        public List<Empleado> ObtenerEmpleados()
        {

            List<Empleado> empleados = new List<Empleado>();

            using (SqlConnection con = conexion.GetConnection())
            {
                try
                {
                    con.Open();
                    string query = "SELECT * FROM Empleado";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Empleado empleado = new Empleado
                                {
                                    IdEmpleado = Convert.ToInt32(reader["id_empleado"]),
                                    Dui = reader["dui"].ToString(),
                                    Nombre = reader["nombre"].ToString(),
                                    Apellidos = reader["apellidos"].ToString(),
                                    Telefono = reader["telefono"].ToString(),
                                    Direccion = reader["direccion"].ToString(),
                                    CuentaCorriente = reader["cuenta_corriente"].ToString(),
                                    Estado = reader["estado"].ToString(),
                                    Correo = reader["correo"].ToString(),
                                    Contrasena = reader["contrasena"].ToString()
                                };
                                empleados.Add(empleado);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener datos: " + ex.Message, "Error");
                }
            }

            return empleados;
        }


        //Editar
        public void EditarEmpleado(Empleado empleado)
        {
            if (empleado == null)
            {
                Console.WriteLine("ID de empleado inválido", "Error");
                return;
            }

            List<SqlParameter> parametros = new List<SqlParameter>();
            StringBuilder query = new StringBuilder("UPDATE Empleado SET ");

            if (!string.IsNullOrEmpty(empleado.Dui))
            {
                query.Append("dui = @dui, ");
                parametros.Add(new SqlParameter("@dui", SqlDbType.VarChar) { Value = empleado.Dui });
            }
            if (!string.IsNullOrEmpty(empleado.Nombre))
            {
                query.Append("nombre = @nombre, ");
                parametros.Add(new SqlParameter("@nombre", SqlDbType.VarChar) { Value = empleado.Nombre });
            }
            if (!string.IsNullOrEmpty(empleado.Apellidos))
            {
                query.Append("apellidos = @apellidos, ");
                parametros.Add(new SqlParameter("@apellidos", SqlDbType.VarChar) { Value = empleado.Apellidos });
            }
            if (!string.IsNullOrEmpty(empleado.Telefono))
            {
                query.Append("telefono = @telefono, ");
                parametros.Add(new SqlParameter("@telefono", SqlDbType.VarChar) { Value = empleado.Telefono });
            }
            if (!string.IsNullOrEmpty(empleado.Direccion))
            {
                query.Append("direccion = @direccion, ");
                parametros.Add(new SqlParameter("@direccion", SqlDbType.VarChar) { Value = empleado.Direccion });
            }
            if (!string.IsNullOrEmpty(empleado.CuentaCorriente))
            {
                query.Append("cuenta_corriente = @cuenta_corriente, ");
                parametros.Add(new SqlParameter("@cuenta_corriente", SqlDbType.VarChar) { Value = empleado.CuentaCorriente });
            }
            if (!string.IsNullOrEmpty(empleado.Correo))
            {
                query.Append("correo = @correo, ");
                parametros.Add(new SqlParameter("@correo", SqlDbType.VarChar) { Value = empleado.Correo });
            }
            if (!string.IsNullOrEmpty(empleado.Contrasena))
            {
                query.Append("contrasena = @contrasena, ");
                parametros.Add(new SqlParameter("@contrasena", SqlDbType.VarChar) { Value = empleado.Contrasena });
            }
            if (!string.IsNullOrEmpty(empleado.Estado))
            {
                query.Append("estado = @estado, ");
                parametros.Add(new SqlParameter("@estado", SqlDbType.VarChar) { Value = empleado.Estado });
            }

            if (parametros.Count == 0)
            {
                Console.WriteLine("No hay cambios que guardar", "Advertencia");
                return;
            }

            query.Length -= 2;
            query.Append(" WHERE id_empleado = @idEmpleado");
            parametros.Add(new SqlParameter("@idEmpleado", SqlDbType.Int) { Value = empleado.IdEmpleado });

            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(query.ToString(), con))
                    {
                        cmd.Parameters.AddRange(parametros.ToArray());
                        con.Open();
                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            Console.WriteLine("Empleado editado correctamente", "Éxito");
                        }
                        else
                        {
                            Console.WriteLine("No se encontró el empleado o no hubo cambios", "Error");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al editar el empleado: " + ex.Message, "Error");
            }
        }




        //Login

        public int LoginEmpleado(string correo, string contrasena)
        {
            int idEmpleado = -1;

            using (SqlConnection con = conexion.GetConnection())
            {
                try
                {
                    con.Open();
                    string query = "SELECT id_empleado FROM Empleado WHERE correo = @correo AND contrasena = @contrasena";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@correo", correo);
                        cmd.Parameters.AddWithValue("@contrasena", contrasena);

                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            idEmpleado = Convert.ToInt32(result);
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

            return idEmpleado;
        }

        //Editar empleado (metodo para traer los datos del empleado especifico)
        public Empleado ObtenerEmpleado(int idEmpleado)
        {
            Empleado empleado = null;

            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = "SELECT dui, nombre, apellidos, telefono, direccion, cuenta_corriente, estado, correo, contrasena " +
                                   "FROM Empleado WHERE id_empleado = @id_empleado";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@id_empleado", SqlDbType.Int).Value = idEmpleado;

                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                empleado = new Empleado
                                {
                                    IdEmpleado = idEmpleado,
                                    Dui = reader["dui"].ToString(),
                                    Nombre = reader["nombre"].ToString(),
                                    Apellidos = reader["apellidos"].ToString(),
                                    Telefono = reader["telefono"].ToString(),
                                    Direccion = reader["direccion"].ToString(),
                                    CuentaCorriente = reader["cuenta_corriente"].ToString(),
                                    Estado = reader["estado"].ToString(),
                                    Correo = reader["correo"].ToString(),
                                    Contrasena = reader["contrasena"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener los datos del empleado: " + ex.Message, "Error");
            }

            return empleado;
        }



        public Empleado ObtenerEmpleado(string correo)
        {
            Empleado empleado = null;

            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = "SELECT id_empleado, dui, nombre, apellidos, telefono, direccion, cuenta_corriente, estado, correo, contrasena " +
                                   "FROM Empleado WHERE correo = @correo";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@correo", SqlDbType.NVarChar).Value = correo;

                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                empleado = new Empleado
                                {
                                    IdEmpleado = Convert.ToInt32(reader["id_empleado"]),
                                    Dui = reader["dui"].ToString(),
                                    Nombre = reader["nombre"].ToString(),
                                    Apellidos = reader["apellidos"].ToString(),
                                    Telefono = reader["telefono"].ToString(),
                                    Direccion = reader["direccion"].ToString(),
                                    CuentaCorriente = reader["cuenta_corriente"].ToString(),
                                    Estado = reader["estado"].ToString(),
                                    Correo = reader["correo"].ToString(),
                                    Contrasena = reader["contrasena"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener los datos del empleado: " + ex.Message);
            }

            return empleado;
        }

        public bool ActualizarDatosGenerales(int idEmpleado, string dui, string nombre, string apellidos, string telefono, string direccion)
        {
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = "UPDATE Empleado SET dui = @dui, nombre = @nombre, apellidos = @apellidos, " +
                                   "telefono = @telefono, direccion = @direccion WHERE id_empleado = @idEmpleado";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@idEmpleado", SqlDbType.Int).Value = idEmpleado;
                        cmd.Parameters.Add("@dui", SqlDbType.VarChar).Value = dui;
                        cmd.Parameters.Add("@nombre", SqlDbType.VarChar).Value = nombre;
                        cmd.Parameters.Add("@apellidos", SqlDbType.VarChar).Value = apellidos;
                        cmd.Parameters.Add("@telefono", SqlDbType.VarChar).Value = telefono;
                        cmd.Parameters.Add("@direccion", SqlDbType.VarChar).Value = direccion;

                        con.Open();
                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            Console.WriteLine("Datos personales actualizados correctamente", "Éxito");
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("No se pudo actualizar los datos personales", "Error");
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al actualizar los datos personales: " + ex.Message, "Error");
                return false;
            }
        }

        //Método para actualizar un campo en especifico del empleado
        public void ActualizarCampoEmpleado(int idEmpleado, string campo, string nuevoValor)
        {
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = $"UPDATE Empleado SET {campo} = @nuevoValor WHERE id_empleado = @idEmpleado";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@idEmpleado", SqlDbType.Int).Value = idEmpleado;
                        cmd.Parameters.Add("@nuevoValor", SqlDbType.VarChar).Value = nuevoValor;
                        con.Open();
                        int filasAfectadas = cmd.ExecuteNonQuery();
                        if (filasAfectadas > 0)
                        {
                            Console.WriteLine($"Campo {campo} actualizado correctamente", "Éxito");
                        }
                        else
                        {
                            Console.WriteLine($"No se pudo actualizar el campo {campo}", "Error");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al actualizar el campo: " + ex.Message, "Error");
            }
        }

        public void ActualizarDatosSensibles(int idEmpleado, string correo, string cuentaCorriente, string nuevaContrasena)
        {
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    // Construir la consulta dinámicamente según si se proporciona una nueva contraseña
                    string query = "UPDATE Empleado SET correo = @correo, cuenta_corriente = @cuentaCorriente";
                    if (!string.IsNullOrEmpty(nuevaContrasena))
                    {
                        query += ", contrasena = @contrasena";
                    }
                    query += " WHERE id_empleado = @idEmpleado";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@idEmpleado", SqlDbType.Int).Value = idEmpleado;
                        cmd.Parameters.Add("@correo", SqlDbType.VarChar).Value = correo;
                        cmd.Parameters.Add("@cuentaCorriente", SqlDbType.VarChar).Value = cuentaCorriente;

                        if (!string.IsNullOrEmpty(nuevaContrasena))
                        {
                            cmd.Parameters.Add("@contrasena", SqlDbType.VarChar).Value = nuevaContrasena;
                        }

                        con.Open();
                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            Console.WriteLine("Datos de cuenta actualizados correctamente", "Éxito");
                        }
                        else
                        {
                            Console.WriteLine("No se pudo actualizar los datos de cuenta", "Error");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al actualizar los datos de cuenta: " + ex.Message, "Error");
            }
        }

        public bool ValidarPassword(int idEmpleado, string passwordActual)
        {
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = "SELECT contrasena FROM Empleado WHERE id_empleado = @idEmpleado";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@idEmpleado", SqlDbType.Int).Value = idEmpleado;

                        con.Open();
                        string contrasenaAlmacenada = cmd.ExecuteScalar()?.ToString();

                        // Comparar la contraseña proporcionada con la almacenada
                        return contrasenaAlmacenada == passwordActual;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al validar la contraseña: " + ex.Message, "Error");
                return false;
            }
        }


        public bool EsDUIUnico(string dui)
        {
            string query = "SELECT COUNT(*) FROM ( " +
                           "SELECT dui FROM Empleado " +
                           "UNION " +
                           "SELECT dui FROM Administrador " +
                           ") AS Unicos WHERE dui = @dui";

            using (SqlConnection con = conexion.GetConnection()) // Usando tu método de conexión
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@dui", dui);
                    con.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count == 0; // Devuelve true si el DUI no existe, false si ya está registrado
                }
            }
        }
        //Sobrecarga de Dui
        public bool EsDUIUnico(string dui, int idEmpleado)
        {
            string query = "SELECT COUNT(*) FROM Empleado WHERE dui = @dui AND id_empleado <> @idEmpleado";

            try
            {
                using (SqlConnection con = conexion.GetConnection()) // Usando tu método de conexión
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@dui", dui);
                        cmd.Parameters.AddWithValue("@idEmpleado", idEmpleado);

                        con.Open();
                        int count = (int)cmd.ExecuteScalar();

                        // Devuelve true si el DUI no está en uso por otro empleado
                        return count == 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error al verificar el DUI: {ex.Message}", "Error");
                return false;
            }
        }

        public bool EsCuentaUnica(string cuenta)
        {
            string query = "SELECT COUNT(*) FROM Empleado WHERE cuenta_corriente = @cuenta";

            using (SqlConnection con = conexion.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@cuenta", cuenta);
                    con.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count == 0;
                }
            }
        }

        //sobrecarga de cuenta
        public bool EsCuentaUnica(string cuenta, int idEmpleado)
        {
            string query = "SELECT COUNT(*) FROM Empleado WHERE cuenta_corriente = @cuenta AND id_empleado <> @idEmpleado";

            using (SqlConnection con = conexion.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@cuenta", cuenta);
                    cmd.Parameters.AddWithValue("@idEmpleado", idEmpleado);
                    con.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count == 0; // Devuelve true si la cuenta no está en uso o pertenece al mismo empleado.
                }
            }
        }


        public bool EsCorreoUnico(string correo)
        {
            string query = "SELECT COUNT(*) FROM Empleado WHERE correo = @correo";

            try
            {
                using (SqlConnection con = conexion.GetConnection()) // Usando tu método de conexión
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@correo", correo);
                        con.Open();
                        int count = (int)cmd.ExecuteScalar();

                        // Devuelve true si el correo no está en uso
                        return count == 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error al verificar el correo: {ex.Message}", "Error");
                return false;
            }
        }

        //Sobrecarga de correo
        public bool EsCorreoUnico(string correo, int idEmpleado)
        {
            string query = "SELECT COUNT(*) FROM Empleado WHERE correo = @correo AND id_empleado <> @idEmpleado";

            using (SqlConnection con = conexion.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@correo", correo);
                    cmd.Parameters.AddWithValue("@idEmpleado", idEmpleado);
                    con.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count == 0; // Devuelve true si el correo no está en uso o pertenece al mismo empleado.
                }
            }
        }


        public bool EsTelefonoUnico(string telefono)
        {
            string query = "SELECT COUNT(*) FROM Empleado WHERE telefono = @telefono";

            try
            {
                using (SqlConnection con = conexion.GetConnection()) // Usando tu método de conexión
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@telefono", telefono);
                        con.Open();
                        int count = (int)cmd.ExecuteScalar();

                        // Devuelve true si el teléfono no está en uso
                        return count == 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error al verificar el teléfono: {ex.Message}", "Error");
                return false;
            }
        }


        //sobrecarga del método telefono
        public bool EsTelefonoUnico(string telefono, int idEmpleado)
        {
            string query = "SELECT COUNT(*) FROM Empleado WHERE telefono = @telefono AND id_empleado <> @idEmpleado";

            try
            {
                using (SqlConnection con = conexion.GetConnection()) // Usando tu método de conexión
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@telefono", telefono);
                        cmd.Parameters.AddWithValue("@idEmpleado", idEmpleado);
                        con.Open();
                        int count = (int)cmd.ExecuteScalar();

                        // Devuelve true si el teléfono no está en uso o pertenece al mismo empleado
                        return count == 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error al verificar el teléfono: {ex.Message}", "Error");
                return false;
            }
        }


        public string FormatearTelefono(string telefono)
        {
            if (telefono.Length > 8)
                telefono = telefono[..8];

            return telefono.Length > 4 ? $"{telefono[..4]}-{telefono[4..]}" : telefono;
        }

        public string FormatearDUI(string dui)
        {
            dui = new string(dui.Where(char.IsDigit).ToArray());
            dui = dui.Length > 9 ? dui.Substring(0, 9) : dui;
            return dui.Length == 9 ? $"{dui.Substring(0, 8)}-{dui.Substring(8, 1)}" : dui;
        }

        public string GenerarContrasena()
        {
            // Configuración básica
            const int longitudMinima = 8;
            const int longitudMaxima = 15; // Puedes ajustar este valor según necesites
            const string mayusculas = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string minusculas = "abcdefghijklmnopqrstuvwxyz";
            const string numeros = "0123456789";
            const string especiales = "!@#$%^&*()";

            var random = new Random();
            var longitud = random.Next(longitudMinima, longitudMaxima + 1);
            var contrasena = new List<char>(longitud);

            // Asegurar al menos un carácter de cada tipo
            contrasena.Add(mayusculas[random.Next(mayusculas.Length)]);
            contrasena.Add(minusculas[random.Next(minusculas.Length)]);
            contrasena.Add(numeros[random.Next(numeros.Length)]);
            contrasena.Add(especiales[random.Next(especiales.Length)]);

            // Rellenar el resto de la contraseña con caracteres aleatorios de todos los tipos
            var todosCaracteres = mayusculas + minusculas + numeros + especiales;
            while (contrasena.Count < longitud)
            {
                contrasena.Add(todosCaracteres[random.Next(todosCaracteres.Length)]);
            }

            // Mezclar los caracteres para mayor aleatoriedad
            for (int i = contrasena.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                var temp = contrasena[i];
                contrasena[i] = contrasena[j];
                contrasena[j] = temp;
            }

            return new string(contrasena.ToArray());
        }

        //Método para validar si una contraseña cumple con los requisitos de seguridad
        public bool ValidarFormatoPassword(string contrasena)
        {
            bool tieneMayuscula = false;
            bool tieneMinuscula = false;
            bool tieneNumero = false;
            bool tieneCaracterEspecial = false;
            foreach (char caracter in contrasena)
            {
                if (Char.IsUpper(caracter))
                    tieneMayuscula = true;
                if (Char.IsLower(caracter))
                    tieneMinuscula = true;
                if (Char.IsDigit(caracter))
                    tieneNumero = true;
                if ("!@#$%^&*()".Contains(caracter))
                    tieneCaracterEspecial = true;
            }
            return tieneMayuscula && tieneMinuscula && tieneNumero && tieneCaracterEspecial;
        }

        /// <summary>
        /// Busca empleados cuyo nombre o apellidos contengan el término de búsqueda.
        /// </summary>
        public List<Empleado> BuscarPorTexto(string texto)
        {
            var lista = new List<Empleado>();
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string sql = @"SELECT id_empleado, dui, nombre, apellidos, telefono, direccion, cuenta_corriente, estado, correo 
                                   FROM Empleado 
                                   WHERE nombre LIKE @texto + '%' 
                                      OR apellidos LIKE @texto + '%' 
                                      OR dui LIKE @texto + '%'";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.Add("@texto", SqlDbType.VarChar).Value = texto;
                        con.Open();
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                lista.Add(new Empleado
                                {
                                    IdEmpleado = Convert.ToInt32(rdr["id_empleado"]),
                                    Dui = rdr["dui"].ToString(),
                                    Nombre = rdr["nombre"].ToString(),
                                    Apellidos = rdr["apellidos"].ToString(),
                                    Telefono = rdr["telefono"].ToString(),
                                    Direccion = rdr["direccion"].ToString(),
                                    CuentaCorriente = rdr["cuenta_corriente"].ToString(),
                                    Estado = rdr["estado"].ToString(),
                                    Correo = rdr["correo"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Manejo de errores (opcional: loguear)
            }
            return lista;
        }
    }
}
