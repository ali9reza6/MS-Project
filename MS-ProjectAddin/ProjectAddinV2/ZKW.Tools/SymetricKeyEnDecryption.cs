using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ZKW.Polarion.AddIn.Tools
{
    public class SymetricKeyEnDecryption
    {
#if DEBUG
        private static readonly string SECRET = "B4DEDDDC1C08412CAD915DDA22DFA3AC";
#else
        private static readonly string SECRET = "9E5C0CB8EE134FDAB5246ED0F8F19158";
#endif

        public static string EncryptString(string plainText)
        {
            byte[] array;
            byte[] iv = GenerateIV();

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(SECRET);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        private static byte[] GenerateIV()
        {
            byte[] iv = new byte[16];
            var ivinit = BitConverter.GetBytes((ulong)DateTime.Today.Ticks).Reverse().ToArray();
            Enumerable.Range(0, 8).ToList().ForEach(x => { iv[15 - x] = iv[x] = ivinit[x]; });
            return iv;
        }

        public static string DecryptString(string cipherText)
        {
            byte[] iv = GenerateIV();
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(SECRET);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
