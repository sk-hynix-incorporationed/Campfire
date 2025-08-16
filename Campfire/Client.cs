using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace Campfire
{
    internal class Client
    {
        public static void Start()
        {
            Console.Write("Enter fire IP: "); 
            string ip = Console.ReadLine();
            Console.WriteLine("Enter name"); 
            string name = Console.ReadLine();
            Console.WriteLine("Enter fire password"); 
            string password = Console.ReadLine();

            byte[] Key = SHA256.HashData(Encoding.UTF8.GetBytes(password));

            TcpClient client = new TcpClient(ip, 32865);
            NetworkStream stream = client.GetStream();

            new Thread(() => 
            { 
                byte[] buffer = new byte[4096]; 
                try 
                { 
                    while (true) 
                    { 
                        int len = stream.Read(buffer, 0, buffer.Length); 
                        if (len == 0) break;

                        string decrypted = Decrypt(buffer.Take(len).ToArray(), Key);
                        Console.WriteLine(decrypted);
                    } 
                } 
                catch { } 
            }).Start();

            while (true)
            {
                string line = Console.ReadLine();
                string mesg = $"{name}: {line}";
                byte[] encrypted = Encrypt(mesg, Key);
                stream.Write(encrypted, 0, encrypted.Length);
            }
        }

        private static byte[] Encrypt(string plainText, byte[] key)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.GenerateIV();
                using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using var ms = new MemoryStream();
                ms.Write(aes.IV, 0, aes.IV.Length);
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                    sw.Write(plainText);
                return ms.ToArray();
            }
        }

        private static string Decrypt(byte[] cipherData, byte[] key)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                byte[] iv = cipherData.Take(16).ToArray();
                byte[] ciphertext = cipherData.Skip(16).ToArray();
                aes.IV = iv;
                using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using var ms = new MemoryStream(ciphertext);
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var sr = new StreamReader(cs);
                return sr.ReadToEnd();
            }
        }
    }
}
