using System.Security.Cryptography;
using System.Text;
using System.IO;
using System;

namespace PayrollWeb.Models
{
    public class Metodos
    {
        private static readonly string claveSecreta = "j3Zl5jP9zN4uUzE+6wXbL1G3Pdq3NvJ+XbVtCq1GQ3o=";

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
    }
}
