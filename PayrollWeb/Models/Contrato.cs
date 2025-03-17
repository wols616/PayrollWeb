using Microsoft.Data.SqlClient;
using System.Data;

namespace PayrollWeb.Models
{
    public class Contrato
    {
        Conexion conexion = new Conexion();

        public int IdContrato { get; set; }
        public int IdEmpleado { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime? FechaBaja { get; set; } // Nullable para contratos indefinidos
        public int IdPuesto { get; set; }
        public string TipoContrato { get; set; }
        public string Vigente { get; set; }

        public Contrato(int idEmpleado, DateTime fechaAlta, DateTime? fechaBaja, int idPuesto, string tipoContrato, string vigente)
        {
            IdEmpleado = idEmpleado;
            FechaAlta = fechaAlta;
            FechaBaja = fechaBaja;
            IdPuesto = idPuesto;
            TipoContrato = tipoContrato;
            Vigente = vigente;
        }
        public Contrato() { }

        public Contrato(int idContrato, int idEmpleado, DateTime fechaAlta, DateTime? fechaBaja, int idPuesto, string tipoContrato, string vigente)
        {
            IdContrato = idContrato;
            IdEmpleado = idEmpleado;
            FechaAlta = fechaAlta;
            FechaBaja = fechaBaja;
            IdPuesto = idPuesto;
            TipoContrato = tipoContrato;
            Vigente = vigente;
        }

        // MÉTODO PARA AGREGAR CONTRATO
        public bool AgregarContrato()
        {
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = "INSERT INTO Contrato (id_empleado, fecha_alta, fecha_baja, id_puesto, tipo_contrato, vigente) " +
                                   "OUTPUT INSERTED.id_contrato " +
                                   "VALUES (@id_empleado, @fecha_alta, @fecha_baja, @id_puesto, @tipo_contrato, @vigente)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@id_empleado", SqlDbType.Int).Value = IdEmpleado;
                        cmd.Parameters.Add("@fecha_alta", SqlDbType.DateTime).Value = FechaAlta;
                        cmd.Parameters.Add("@fecha_baja", SqlDbType.DateTime).Value = (object)FechaBaja ?? DBNull.Value; // Manejar nulos
                        cmd.Parameters.Add("@id_puesto", SqlDbType.Int).Value = IdPuesto;
                        cmd.Parameters.Add("@tipo_contrato", SqlDbType.VarChar).Value = TipoContrato;
                        cmd.Parameters.Add("@vigente", SqlDbType.VarChar).Value = Vigente;

                        con.Open();
                        var result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            IdContrato = Convert.ToInt32(result);
                            System.Diagnostics.Debug.WriteLine("Contrato agregado correctamente", "Éxito");
                            new Metodos().EjecutarActualizarPorcentajesDeducciones();
                            return true;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("No se pudo agregar el contrato", "Error");
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al agregar el contrato: " + ex.Message);
                return false;
            }
        }

        //Método para obtener un contrato por su id
        public Contrato ObtenerContrato(int idContrato)
        {
            Contrato contrato = null;
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = "SELECT * FROM Contrato WHERE id_contrato = @id_contrato";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@id_contrato", SqlDbType.Int).Value = idContrato;
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                contrato = new Contrato
                                {
                                    IdContrato = Convert.ToInt32(reader["id_contrato"]),
                                    IdEmpleado = Convert.ToInt32(reader["id_empleado"]),
                                    FechaAlta = Convert.ToDateTime(reader["fecha_alta"]),
                                    FechaBaja = reader["fecha_baja"] as DateTime?,
                                    IdPuesto = Convert.ToInt32(reader["id_puesto"]),
                                    TipoContrato = reader["tipo_contrato"].ToString(),
                                    Vigente = reader["vigente"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener el contrato: " + ex.Message);
            }
            return contrato;
        }

        public List<ContratoViewModel> ObtenerContratosYPuestos(int idEmpleado)
        {
            List<ContratoViewModel> listaContratos = new List<ContratoViewModel>();

            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = @"
                SELECT c.id_contrato, p.id_puesto, cat.id_categoria, c.fecha_alta, c.fecha_baja, c.tipo_contrato, c.vigente, 
                       p.nombre_puesto, cat.nombre_categoria, cat.sueldo_base
                FROM Contrato c
                JOIN Puesto p ON c.id_puesto = p.id_puesto
                JOIN Categoria cat ON p.id_categoria = cat.id_categoria
                WHERE c.id_empleado = @id_empleado";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@id_empleado", SqlDbType.Int).Value = idEmpleado;

                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var contrato = new ContratoViewModel
                                {
                                    IdContrato = Convert.ToInt32(reader["id_contrato"]),
                                    IdPuesto = Convert.ToInt32(reader["id_puesto"]),
                                    IdCategoria = Convert.ToInt32(reader["id_categoria"]),
                                    Puesto = reader["nombre_puesto"].ToString(),
                                    Categoria = reader["nombre_categoria"].ToString(),
                                    SueldoBase = Convert.ToDecimal(reader["sueldo_base"]),
                                    FechaAlta = Convert.ToDateTime(reader["fecha_alta"]),
                                    FechaBaja = reader["fecha_baja"] as DateTime?,
                                    TipoContrato = reader["tipo_contrato"].ToString(),
                                    Vigente = reader["vigente"].ToString()
                                };
                                listaContratos.Add(contrato);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener los contratos: " + ex.Message);
            }

            return listaContratos;
        }

        //MÉTODO PARA ACTUALIZAR UN CAMPO QUE YO ELIGA MEDIANTE UN PARAMETRO CON UN DATO QUE PASE POR PARAMETRO
        public void ActualizarContrato(string campo, object valor)
        {
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = $"UPDATE Contrato SET {campo} = @valor WHERE id_contrato = @id_contrato";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@id_contrato", SqlDbType.Int).Value = IdContrato;
                        cmd.Parameters.Add("@valor",
                            valor is DateTime ? SqlDbType.DateTime : SqlDbType.VarChar
                            ).Value = valor;

                        con.Open();
                        int filasAfectadas = cmd.ExecuteNonQuery();
                        if (filasAfectadas > 0)
                        {
                            Console.WriteLine("Contrato actualizado correctamente", "Éxito");
                        }
                        else
                        {
                            Console.WriteLine("No se pudo actualizar el contrato", "Error");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al actualizar el contrato: " + ex.Message, "Error");
            }
        }

        //MÉTODO PARA COMPROBAR QUE NO HAY NINGÚN CONTRATO ACTUALMENTE VIGENTE
        public bool ComprobarContratoVigente(int idEmpleado)
        {
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = "SELECT COUNT(*) FROM Contrato WHERE id_empleado = @id_empleado AND vigente = 'S'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@id_empleado", SqlDbType.Int).Value = idEmpleado;
                        con.Open();
                        int contratosVigentes = (int)cmd.ExecuteScalar();
                        if (contratosVigentes > 0)
                        {
                            Console.WriteLine("El empleado ya tiene un contrato vigente", "Error");
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al comprobar el contrato vigente: " + ex.Message, "Error");
                return false;
            }
        }



        //MÉTODOS QUE CREO NO ESTOY USANDO PERO POR LAS DUDAS NO BORRAR--------------------------------------------------------------

        public List<Contrato> ObtenerContratos()
        {
            List<Contrato> listaContratos = new List<Contrato>();

            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = "SELECT * FROM Contrato";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Contrato contrato = new Contrato
                                {
                                    IdContrato = Convert.ToInt32(reader["id_contrato"]),
                                    IdEmpleado = Convert.ToInt32(reader["id_empleado"]),
                                    FechaAlta = Convert.ToDateTime(reader["fecha_alta"]),
                                    FechaBaja = reader["fecha_baja"] as DateTime?,
                                    IdPuesto = Convert.ToInt32(reader["id_puesto"]),
                                    TipoContrato = reader["tipo_contrato"].ToString(),
                                    Vigente = reader["vigente"].ToString()
                                };
                                listaContratos.Add(contrato);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener los contratos: " + ex.Message, "Error");
            }

            return listaContratos;
        }
        public List<Contrato> ObtenerContratos(int idEmpleado)
        {
            List<Contrato> listaContratos = new List<Contrato>();

            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = "SELECT * FROM Contrato WHERE id_empleado = @id_empleado";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@id_empleado", SqlDbType.Int).Value = idEmpleado;

                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Contrato contrato = new Contrato
                                {
                                    IdContrato = Convert.ToInt32(reader["id_contrato"]),
                                    IdEmpleado = Convert.ToInt32(reader["id_empleado"]),
                                    FechaAlta = Convert.ToDateTime(reader["fecha_alta"]),
                                    FechaBaja = reader["fecha_baja"] as DateTime?,
                                    IdPuesto = Convert.ToInt32(reader["id_puesto"]),
                                    TipoContrato = reader["tipo_contrato"].ToString(),
                                    Vigente = reader["vigente"].ToString()
                                };
                                listaContratos.Add(contrato);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener los contratos: " + ex.Message, "Error");
            }

            return listaContratos;
        }
        public ContratoViewModel ObtenerContratoYPuesto(int idContrato)
        {
            ContratoViewModel contrato = null;
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = @"
                SELECT c.id_contrato, p.id_puesto, cat.id_categoria, c.fecha_alta, c.fecha_baja, c.tipo_contrato, c.vigente, 
                       p.nombre_puesto, cat.nombre_categoria, cat.sueldo_base
                FROM Contrato c
                JOIN Puesto p ON c.id_puesto = p.id_puesto
                JOIN Categoria cat ON p.id_categoria = cat.id_categoria
                WHERE c.id_contrato = @id_contrato";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@id_contrato", SqlDbType.Int).Value = idContrato;
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                contrato = new ContratoViewModel
                                {
                                    IdContrato = Convert.ToInt32(reader["id_contrato"]),
                                    IdPuesto = Convert.ToInt32(reader["id_puesto"]),
                                    IdCategoria = Convert.ToInt32(reader["id_categoria"]),
                                    Puesto = reader["nombre_puesto"].ToString(),
                                    Categoria = reader["nombre_categoria"].ToString(),
                                    SueldoBase = Convert.ToDecimal(reader["sueldo_base"]),
                                    FechaAlta = Convert.ToDateTime(reader["fecha_alta"]),
                                    FechaBaja = reader["fecha_baja"] as DateTime?,
                                    TipoContrato = reader["tipo_contrato"].ToString(),
                                    Vigente = reader["vigente"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener el contrato: " + ex.Message);
            }
            return contrato;
        }

        // MÉTODO PARA ACTUALIZAR CONTRATO
        public void ActualizarContrato()
        {
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = "UPDATE Contrato SET id_empleado = @id_empleado, fecha_alta = @fecha_alta, fecha_baja = @fecha_baja, " +
                                   "id_puesto = @id_puesto, tipo_contrato = @tipo_contrato WHERE id_contrato = @id_contrato";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@id_contrato", SqlDbType.Int).Value = IdContrato;
                        cmd.Parameters.Add("@id_empleado", SqlDbType.Int).Value = IdEmpleado;
                        cmd.Parameters.Add("@fecha_alta", SqlDbType.DateTime).Value = FechaAlta;
                        cmd.Parameters.Add("@fecha_baja", SqlDbType.DateTime).Value = (object)FechaBaja ?? DBNull.Value; // Manejar nulos
                        cmd.Parameters.Add("@id_puesto", SqlDbType.Int).Value = IdPuesto;
                        cmd.Parameters.Add("@tipo_contrato", SqlDbType.VarChar).Value = TipoContrato;

                        con.Open();
                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            Console.WriteLine("Contrato actualizado correctamente", "Éxito");
                        }
                        else
                        {
                            Console.WriteLine("No se pudo actualizar el contrato", "Error");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al actualizar el contrato: " + ex.Message, "Error");
            }
        }

        // MÉTODO PARA ACTUALIZAR LA FECHA DE BAJA Y VIGENTE DE UN CONTRATO
        public void ActualizarFechaBajaVigenteContrato()
        {
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    string query = "UPDATE Contrato SET fecha_baja = @fecha_baja, vigente = @vigente WHERE id_contrato = @id_contrato";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@id_contrato", SqlDbType.Int).Value = IdContrato;
                        cmd.Parameters.Add("@fecha_baja", SqlDbType.DateTime).Value = FechaBaja;
                        cmd.Parameters.Add("@vigente", SqlDbType.VarChar).Value = Vigente;
                        con.Open();
                        int filasAfectadas = cmd.ExecuteNonQuery();
                        if (filasAfectadas > 0)
                        {
                            Console.WriteLine("Contrato actualizado correctamente", "Éxito");
                        }
                        else
                        {
                            Console.WriteLine("No se pudo actualizar el contrato", "Error");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al actualizar el contrato: " + ex.Message, "Error");
            }
        }

        public bool ActualizarEstadoEmpleado(int idEmpleado)
        {
            try
            {
                using (SqlConnection con = conexion.GetConnection())
                {
                    // Verificar si todos los contratos del empleado tienen "Vigente" como "N"
                    string queryVerificar = "SELECT COUNT(*) FROM Contrato WHERE id_empleado = @id_empleado AND vigente = 'S'";
                    using (SqlCommand cmdVerificar = new SqlCommand(queryVerificar, con))
                    {
                        cmdVerificar.Parameters.Add("@id_empleado", SqlDbType.Int).Value = idEmpleado;
                        con.Open();
                        int contratosVigentes = (int)cmdVerificar.ExecuteScalar();
                        con.Close();

                        string nuevoEstado = contratosVigentes == 0 ? "Inactivo" : "Activo";

                        // Verificar el estado actual del empleado
                        string queryEstadoActual = "SELECT estado FROM Empleado WHERE id_empleado = @id_empleado";
                        using (SqlCommand cmdEstadoActual = new SqlCommand(queryEstadoActual, con))
                        {
                            cmdEstadoActual.Parameters.Add("@id_empleado", SqlDbType.Int).Value = idEmpleado;
                            con.Open();
                            string estadoActual = (string)cmdEstadoActual.ExecuteScalar();
                            con.Close();

                            // Actualizar el estado del empleado solo si es diferente del nuevo estado
                            if (estadoActual != nuevoEstado)
                            {
                                string queryActualizar = "UPDATE Empleado SET estado = @nuevo_estado WHERE id_empleado = @id_empleado";
                                using (SqlCommand cmdActualizar = new SqlCommand(queryActualizar, con))
                                {
                                    cmdActualizar.Parameters.Add("@id_empleado", SqlDbType.Int).Value = idEmpleado;
                                    cmdActualizar.Parameters.Add("@nuevo_estado", SqlDbType.VarChar).Value = nuevoEstado;
                                    con.Open();
                                    int filasAfectadas = cmdActualizar.ExecuteNonQuery();
                                    con.Close();

                                    if (filasAfectadas > 0)
                                    {
                                        Console.WriteLine($"Estado del empleado actualizado a {nuevoEstado}", "Éxito");
                                        return true;
                                    }
                                    else
                                    {
                                        Console.WriteLine("No se pudo actualizar el estado del empleado", "Error");
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                // El estado ya es el correcto, no es necesario actualizar
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al actualizar el estado del empleado: " + ex.Message, "Error");
                return false;
            }
        }
    }
    public class ContratoViewModel
    {
        public int IdContrato { get; set; }
        public int IdPuesto { get; set; }
        public int IdCategoria { get; set; }
        public int IdEmpleado { get; set; }
        public string Puesto { get; set; }
        public string Categoria { get; set; }
        public decimal SueldoBase { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime? FechaBaja { get; set; }
        public string TipoContrato { get; set; }
        public string Vigente { get; set; }
    }
}
