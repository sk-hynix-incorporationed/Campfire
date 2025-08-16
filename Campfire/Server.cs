using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Campfire
{
    internal class Server
    {
        static List<TcpClient> clients = new List<TcpClient>();
        static readonly byte[] Key = Encoding.UTF8.GetBytes("ThisIs32BytesLongForAES256Key!!!");

        public static void Start(int port)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 32865);
            listener.Start();
            Console.WriteLine("Campfire host running on port 32865");

            while (true)
            {
                var client = listener.AcceptTcpClient();
                lock (clients) { clients.Add(client); }
                new Thread(() => HandleClient(client)).Start();
            }
        }

        public static void HandleClient(TcpClient client)
        {
            var stream = client.GetStream();
            byte[] buffer = new byte[4096];

            try
            {
                while (true)
                {
                    int len = stream.Read(buffer, 0, buffer.Length);
                    if (len == 0) break;

                    string decrypted = Decrypt(buffer, len);
                    Console.WriteLine(decrypted);
                    Broadcast(decrypted);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client error: {ex.Message}");
            }
            finally
            {
                lock (clients) { clients.Remove(client); }
                client.Close();
            }
        }

        public static void Broadcast(string msg)
        {
            byte[] encrypted = Encrypt(msg);
            lock (clients)
            {
                foreach (var c in clients)
                {
                    try { c.GetStream().Write(encrypted, 0, encrypted.Length); } catch { }
                }
            }
        }

        public static byte[] Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = Key;
            aes.GenerateIV();

            using var ms = new MemoryStream();
            ms.Write(aes.IV, 0, aes.IV.Length);
            using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
                sw.Write(plainText);

            return ms.ToArray();
        }

        public static string Decrypt(byte[] data, int length)
        {
            using var aes = Aes.Create();
            aes.Key = Key;

            byte[] iv = new byte[16];
            Array.Copy(data, 0, iv, 0, iv.Length);
            aes.IV = iv;

            using var ms = new MemoryStream(data, iv.Length, length - iv.Length);
            using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
    }
}
