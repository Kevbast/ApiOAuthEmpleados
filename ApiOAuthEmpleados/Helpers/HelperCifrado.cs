using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace ApiOAuthEmpleados.Helpers
{
    public class HelperCifrado
    {
        /// <summary>
        /// Cifra cualquier objeto serializable a JSON y devuelve el contenido cifrado en bytes.
        /// </summary>
        /// <typeparam name="T">Tipo del dato a cifrar.</typeparam>
        /// <param name="data">Dato de entrada.</param>
        /// <param name="key">Clave AES de 16, 24 o 32 bytes.</param>
        /// <returns>Bytes cifrados con el IV incluido al inicio.</returns>
        public byte[] Cifrar<T>(T data, byte[] key)
        {
            ArgumentNullException.ThrowIfNull(key);

            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            ValidarClave(key);

            string json = JsonConvert.SerializeObject(data);
            byte[] plainBytes = Encoding.UTF8.GetBytes(json);

            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.GenerateIV();

            using ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            byte[] resultado = new byte[aes.IV.Length + cipherBytes.Length];
            Buffer.BlockCopy(aes.IV, 0, resultado, 0, aes.IV.Length);
            Buffer.BlockCopy(cipherBytes, 0, resultado, aes.IV.Length, cipherBytes.Length);

            return resultado;
        }

        /// <summary>
        /// Descifra bytes generados por Cifrar y reconstruye el objeto original.
        /// </summary>
        /// <typeparam name="T">Tipo esperado tras el descifrado.</typeparam>
        /// <param name="encryptedData">Bytes cifrados con IV incluido al inicio.</param>
        /// <param name="key">Clave AES de 16, 24 o 32 bytes.</param>
        /// <returns>Objeto descifrado y deserializado.</returns>
        public T Descifrar<T>(byte[] encryptedData, byte[] key)
        {
            ArgumentNullException.ThrowIfNull(encryptedData);
            ArgumentNullException.ThrowIfNull(key);
            ValidarClave(key);

            if (encryptedData.Length <= 16)
            {
                throw new ArgumentException("El contenido cifrado no es válido.", nameof(encryptedData));
            }

            byte[] iv = new byte[16];
            byte[] cipherBytes = new byte[encryptedData.Length - iv.Length];

            Buffer.BlockCopy(encryptedData, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(encryptedData, iv.Length, cipherBytes, 0, cipherBytes.Length);

            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            using ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            string json = Encoding.UTF8.GetString(plainBytes);

            T? result = JsonConvert.DeserializeObject<T>(json);
            if (result is null)
            {
                throw new InvalidOperationException("No se pudo deserializar el contenido descifrado.");
            }

            return result;
        }

        private static void ValidarClave(byte[] key)
        {
            if (key.Length != 16 && key.Length != 24 && key.Length != 32)
            {
                throw new ArgumentException("La clave debe tener 16, 24 o 32 bytes para AES.", nameof(key));
            }
        }
    }
}
