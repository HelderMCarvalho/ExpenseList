using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DespesasLibrary
{
    public static class Hash
    {
        /// <summary>
        ///     Cria um Hash256 de uma string
        /// </summary>
        /// <param name="text">String a transformar</param>
        /// <returns>String inicial transformada em Sha256</returns>
        public static string GetHashSha256(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            return hash.Aggregate(string.Empty, (current, x) => current + $"{x:x2}");
        }
    }
}