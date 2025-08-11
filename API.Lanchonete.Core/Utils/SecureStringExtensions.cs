using System.Security;

namespace API.Lanchonete.Core.Utils
{
    public static class SecureStringExtensions
    {
        public static SecureString ToSecureString(this string input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var secure = new SecureString();
            foreach (char c in input)
                secure.AppendChar(c);

            secure.MakeReadOnly();
            return secure;
        }
    }
}