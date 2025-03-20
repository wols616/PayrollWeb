using Microsoft.Data.SqlClient;

namespace PayrollWeb.Models
{
    public class Complemento_Cargo
    {
        public int IdComplementoCargo { get; set; }
        public int IdCargo { get; set; }
        public int IdEmpleado { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public Decimal MontoComplemento { get; set; }
        public Cargo Cargo { get; set; }
        public Empleado Empleado { get; set; }

        Conexion conexion = new Conexion();

        //MÉTODO PARA OBTENER TODOS LOS CARGOS DE UN EMPLEADO
        public List<Complemento_Cargo> ObtenerComplementosCargos(int idEmpleado)
        {
            List<Complemento_Cargo> ComplementosCargosList = new List<Complemento_Cargo>();
            // Consulta SQL para obtener todas las deducciones
            string query = "SELECT id_complemento_cargo, id_cargo, id_empleado, fecha_inicio, fecha_fin, monto_complemento FROM Complemento_Cargo WHERE id_empleado = @idEmpleado";
            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    // Abrir la conexión
                    connection.Open();
                    // Ejecutar la consulta
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@idEmpleado", idEmpleado);
                        // Leer los datos de la base de datos
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Procesar cada fila y agregarla a la lista
                            while (reader.Read())
                            {
                                Complemento_Cargo complementoCargo = new Complemento_Cargo
                                {
                                    IdComplementoCargo = Convert.ToInt32(reader["id_complemento_cargo"]),
                                    IdCargo = Convert.ToInt32(reader["id_cargo"]),
                                    IdEmpleado = Convert.ToInt32(reader["id_empleado"]),
                                    FechaInicio = Convert.ToDateTime(reader["fecha_inicio"]),
                                    FechaFin = Convert.ToDateTime(reader["fecha_fin"]),
                                    MontoComplemento = Convert.ToDecimal(reader["monto_complemento"]),
                                    Cargo = new Cargo().ObtenerCargo(Convert.ToInt32(reader["id_cargo"])),
                                    Empleado = new Empleado().ObtenerEmpleado(Convert.ToInt32(reader["id_empleado"]))
                                };
                                // Agregar el objeto Deduccion a la lista
                                ComplementosCargosList.Add(complementoCargo);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener los complementos de cargo: " + ex.Message);
                }
            }
            // Retornar la lista de deducciones
            return ComplementosCargosList;
        }

        //MÉTODO PARA OBTENER UN COMPLEMENTO DE CARGO POR SU ID
        public Complemento_Cargo ObtenerComplementoCargo(int idComplementoCargo)
        {
            Complemento_Cargo complementoCargo = new Complemento_Cargo();
            // Consulta SQL para obtener todas las deducciones
            string query = "SELECT id_complemento_cargo, id_cargo, id_empleado, fecha_inicio, fecha_fin, monto_complemento FROM Complemento_Cargo WHERE id_complemento_cargo = @idComplementoCargo";
            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    // Abrir la conexión
                    connection.Open();
                    // Ejecutar la consulta
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@idComplementoCargo", idComplementoCargo);
                        // Leer los datos de la base de datos
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Procesar cada fila y agregarla a la lista
                            while (reader.Read())
                            {
                                complementoCargo = new Complemento_Cargo
                                {
                                    IdComplementoCargo = Convert.ToInt32(reader["id_complemento_cargo"]),
                                    IdCargo = Convert.ToInt32(reader["id_cargo"]),
                                    IdEmpleado = Convert.ToInt32(reader["id_empleado"]),
                                    FechaInicio = Convert.ToDateTime(reader["fecha_inicio"]),
                                    FechaFin = Convert.ToDateTime(reader["fecha_fin"]),
                                    MontoComplemento = Convert.ToDecimal(reader["monto_complemento"]),
                                    Cargo = new Cargo().ObtenerCargo(Convert.ToInt32(reader["id_cargo"])),
                                    Empleado = new Empleado().ObtenerEmpleado(Convert.ToInt32(reader["id_empleado"]))
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener el complemento de cargo: " + ex.Message);
                }
            }
            // Retornar la lista de deducciones
            return complementoCargo;
        }

        //MÉTODO PARA AGREGAR UN COMPLEMENTO DE CARGO
        public void AgregarComplementoCargo(Complemento_Cargo complementoCargo)
        {
            // Consulta SQL para agregar un complemento de cargo
            string query = "INSERT INTO Complemento_Cargo (id_cargo, id_empleado, fecha_inicio, fecha_fin, monto_complemento) VALUES (@IdCargo, @IdEmpleado, @FechaInicio, @FechaFin, @MontoComplemento)";
            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    // Abrir la conexión
                    connection.Open();
                    // Ejecutar la consulta
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdCargo", complementoCargo.IdCargo);
                        command.Parameters.AddWithValue("@IdEmpleado", complementoCargo.IdEmpleado);
                        command.Parameters.AddWithValue("@FechaInicio", complementoCargo.FechaInicio);
                        command.Parameters.AddWithValue("@FechaFin", complementoCargo.FechaFin);
                        command.Parameters.AddWithValue("@MontoComplemento", complementoCargo.MontoComplemento);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al agregar el complemento de cargo: " + ex.Message);
                }
            }
        }

        //MÉTODO PARA ELIMINAR UN COMPLEMENTO DE CARGO
        public bool EliminarComplementoCargo(int idComplementoCargo)
        {
            // Consulta SQL para eliminar un complemento de cargo
            string query = "DELETE FROM Complemento_Cargo WHERE id_complemento_cargo = @IdComplementoCargo";
            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    // Abrir la conexión
                    connection.Open();
                    // Ejecutar la consulta
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdComplementoCargo", idComplementoCargo);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al eliminar el complemento de cargo: " + ex.Message);
                    return false;
                }
            }
            return true;
        }
    }
}
