/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 08/07/2021
/// ----------------------------------------------------------------------------------------------------- 

using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace eVaSys.Utils
{
    /// <summary>
    /// Encryption/decryption utilities
    /// </summary>
    //------------------------------------------------------------------------------------------------------------------
    //CLASS
    public class CryptoUtils
    {
        public static string EncryptAesHexa(string plainText, string k, string v)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            //Init
            byte[] encrypted;
            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(Encoding.UTF8.GetBytes(k), Encoding.UTF8.GetBytes(v));

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new())
                {
                    using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            //return HttpUtility.UrlEncode(Convert.ToBase64String(encrypted));
            return BitConverter.ToString(encrypted).Replace("-", string.Empty); 
        }

        public static string DecryptAesHexa(string cipherText, string k, string v)
        {
            // Declare the string used to hold
            // the decrypted text.
            string plaintext = "";

            try
            {
                if (cipherText != null && cipherText.Length > 0)
                {
                    // Create an Aes object
                    // with the specified key and IV.
                    using (Aes aesAlg = Aes.Create())
                    {
                        aesAlg.Key = Encoding.UTF8.GetBytes(k);
                        aesAlg.IV = Encoding.UTF8.GetBytes(v);

                        // Create a decryptor to perform the stream transform.
                        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                        //Convert Hexa string to byte array

                        var cipherByte = Enumerable.Range(0, cipherText.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(cipherText.Substring(x, 2), 16))
                             .ToArray();
                        // Create the streams used for decryption.
                        using (MemoryStream msDecrypt = new(cipherByte))
                        {
                            using (CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read))
                            {
                                using (StreamReader srDecrypt = new(csDecrypt))
                                {

                                    // Read the decrypted bytes from the decrypting stream
                                    // and place them in a string.
                                    plaintext = srDecrypt.ReadToEnd();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { }

            return plaintext;
        }
    }
}