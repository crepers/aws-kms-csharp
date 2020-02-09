using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Amazon.KeyManagementService;
using Amazon.KeyManagementService.Model;

namespace KMSApplication
{
    class Program
    {
        AmazonKeyManagementServiceClient kmsClient = new AmazonKeyManagementServiceClient(Amazon.RegionEndpoint.APNortheast2);

        static void Main(string[] args)
        {
            Program kmsApp = new Program();

            try
            {
                string encryptText = kmsApp.Encrypt("Hello world");
                Console.WriteLine(encryptText);

                string plainText = kmsApp.Decrypt(encryptText);
                Console.WriteLine(plainText);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private string Encrypt(string text)
        {
            string keyId = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxx";

            MemoryStream plaintext = new MemoryStream(Encoding.UTF8.GetBytes(text));
            EncryptRequest encryptRequest = new EncryptRequest()
            {
                KeyId = keyId,
                Plaintext = plaintext
            };
            MemoryStream ciphertext = kmsClient.Encrypt(encryptRequest).CiphertextBlob;
            var buffer = new byte[ciphertext.Length];
            ciphertext.Read(buffer, 0, (int)ciphertext.Length);

            return Convert.ToBase64String(buffer);
        }

        private string Decrypt(string encryptText)
        {
            var ciphertestStream = new MemoryStream(Convert.FromBase64String(encryptText)) { Position = 0 };
            var decryptRequest = new DecryptRequest { CiphertextBlob = ciphertestStream };

            var response = kmsClient.Decrypt(decryptRequest);
            var buffer = new byte[response.Plaintext.Length];
            var bytesRead = response.Plaintext.Read(buffer, 0, (int)response.Plaintext.Length);

            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }
    }
}
