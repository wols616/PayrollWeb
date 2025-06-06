﻿using System.Security.Cryptography;
using System.Text;
using System.IO;
using System;
using Microsoft.Data.SqlClient;

namespace PayrollWeb.Models
{
    public class Metodos
    {
        private static readonly string claveSecreta = "j3Zl5jP9zN4uUzE+6wXbL1G3Pdq3NvJ+XbVtCq1GQ3o=";
        Conexion conexion = new Conexion();

        public static string EncriptarContrasena(string contrasena)
        {
            byte[] claveBytes = Convert.FromBase64String(claveSecreta); // ✅ Decodificar Base64
            byte[] ivBytes = new byte[16]; // IV de 16 bytes

            using (Aes aes = Aes.Create())
            {
                aes.Key = claveBytes;
                aes.IV = ivBytes;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        byte[] contrasenaBytes = Encoding.UTF8.GetBytes(contrasena);
                        cs.Write(contrasenaBytes, 0, contrasenaBytes.Length);
                        cs.FlushFinalBlock();
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string DesencriptarContrasena(string contrasenaEncriptada)
        {
            byte[] claveBytes = Convert.FromBase64String(claveSecreta); // ✅ Decodificar Base64
            byte[] ivBytes = new byte[16];

            using (Aes aes = Aes.Create())
            {
                aes.Key = claveBytes;
                aes.IV = ivBytes;

                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(contrasenaEncriptada)))
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }

        public bool EjecutarActualizarPorcentajesDeducciones()
        {
            bool exito = false;
            string storedProcedure = "ActualizarPorcentajesDeducciones";  // Nombre del procedimiento almacenado

            using (SqlConnection connection = conexion.GetConnection())
            {
                try
                {
                    // Abrir la conexión
                    connection.Open();

                    // Crear el comando para el procedimiento almacenado
                    using (SqlCommand command = new SqlCommand(storedProcedure, connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        // Puedes agregar parámetros si el procedimiento almacenado los requiere.
                        // Si tu procedimiento no tiene parámetros, puedes omitir esta parte.

                        // Ejecutar el procedimiento almacenado
                        int rowsAffected = command.ExecuteNonQuery();

                        // Si se afectaron filas, el procedimiento fue ejecutado correctamente
                        if (rowsAffected > 0)
                        {
                            exito = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al ejecutar el procedimiento: " + ex.Message);
                }
            }

            return exito;
        }

        //public bool EjecutarActualizarSueldosConComplementosCargo()
        //{
        //    bool exito = false;
        //    string storedProcedure = "ActualizarSueldosConComplementosCargo";  // Nombre del procedimiento almacenado

        //    using (SqlConnection connection = conexion.GetConnection())
        //    {
        //        try
        //        {
        //            // Abrir la conexión
        //            connection.Open();

        //            // Crear el comando para el procedimiento almacenado
        //            using (SqlCommand command = new SqlCommand(storedProcedure, connection))
        //            {
        //                command.CommandType = System.Data.CommandType.StoredProcedure;

        //                // Puedes agregar parámetros si el procedimiento almacenado los requiere.
        //                // Si tu procedimiento no tiene parámetros, puedes omitir esta parte.

        //                // Ejecutar el procedimiento almacenado
        //                int rowsAffected = command.ExecuteNonQuery();

        //                // Si se afectaron filas, el procedimiento fue ejecutado correctamente
        //                if (rowsAffected > 0)
        //                {
        //                    exito = true;
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("Error al ejecutar el procedimiento: " + ex.Message);
        //        }
        //    }

        //    return exito;
        //}
    }
}
