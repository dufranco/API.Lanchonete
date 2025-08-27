using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace API.Lanchonete.Core.Utils
{
    public class SenhaSaltHash
    {
        public static byte[] GerarSalt(int tamanho = 16)
        {
            var salt = new byte[tamanho];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return salt;
        }

        public static byte[] GerarHashSenha(SecureString senha, byte[] salt)
        {
            ArgumentNullException.ThrowIfNull(salt);

            var senhaBstr = Marshal.SecureStringToBSTR(senha);
            var senhaStr = Marshal.PtrToStringBSTR(senhaBstr);
            var senhaBytes = Encoding.UTF8.GetBytes(senhaStr);

            Marshal.ZeroFreeBSTR(senhaBstr);

            var senhaComSalt = new byte[salt.Length + senhaBytes.Length];
            Buffer.BlockCopy(salt, 0, senhaComSalt, 0, salt.Length);
            Buffer.BlockCopy(senhaBytes, 0, senhaComSalt, salt.Length, senhaBytes.Length);

            return SHA256.HashData(senhaComSalt);
        }

        public static bool CompararSenha(SecureString senha, string saltBase64, string hashEsperadoBase64)
        {
            if (!IsBase64String(saltBase64))
                throw new FormatException("Salt não está no formato Base64.");

            if (!IsBase64String(hashEsperadoBase64))
                throw new FormatException("Hash não está no formato Base64.");

            var salt = Convert.FromBase64String(saltBase64);
            var senhaBstr = Marshal.SecureStringToBSTR(senha);
            var senhaStr = Marshal.PtrToStringBSTR(senhaBstr);
            var senhaBytes = Encoding.UTF8.GetBytes(senhaStr);

            Marshal.ZeroFreeBSTR(senhaBstr);

            var senhaComSalt = new byte[salt.Length + senhaBytes.Length];
            Buffer.BlockCopy(salt, 0, senhaComSalt, 0, salt.Length);
            Buffer.BlockCopy(senhaBytes, 0, senhaComSalt, salt.Length, senhaBytes.Length);

            var hashCalculado = SHA256.HashData(senhaComSalt);
            var hashEsperado = Convert.FromBase64String(hashEsperadoBase64);

            if (hashCalculado.Length != hashEsperado.Length)
                return false;

            var iguais = true;
            for (int i = 0; i < hashCalculado.Length; i++)
            {
                if (hashCalculado[i] != hashEsperado[i])
                {
                    iguais = false;
                    break;
                }
            }

            return iguais;
        }

        private static bool IsBase64String(string s)
        {
            if (string.IsNullOrWhiteSpace(s.Trim()))
                return false;

            Span<byte> buffer = new Span<byte>(new byte[s.Length]);
            return Convert.TryFromBase64String(s, buffer, out _);
        }
    }
}
