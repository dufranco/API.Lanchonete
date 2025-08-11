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
            var senhaBstr = Marshal.SecureStringToBSTR(senha);
            var senhaStr = Marshal.PtrToStringBSTR(senhaBstr);
            var senhaBytes = Encoding.UTF8.GetBytes(senhaStr);

            Marshal.ZeroFreeBSTR(senhaBstr);

            var senhaComSalt = new byte[salt.Length + senhaBytes.Length];
            Buffer.BlockCopy(salt, 0, senhaComSalt, 0, salt.Length);
            Buffer.BlockCopy(senhaBytes, 0, senhaComSalt, salt.Length, senhaBytes.Length);

            return SHA256.HashData(senhaComSalt);
        }
    }
}
