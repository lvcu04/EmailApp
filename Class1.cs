using System;
using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using MailKit.Security;
using MimeKit;

namespace EmailServerExample
{
    class Program
    {
        static void Main(string[] args)
        {
            string server = "imap.gmail.com"; // hoặc "pop.gmail.com" cho POP3
            int imapPort = 993;
            int pop3Port = 995;
            string email = "2251120275@gmail.com";
            string password = "levancu2004";

            Console.WriteLine("Chọn giao thức để kết nối: ");
            Console.WriteLine("1. IMAP");
            Console.WriteLine("2. POP3");
            int choice = Convert.ToInt32(Console.ReadLine());

            if (choice == 1)
            {
                ConnectToImap(server, imapPort, email, password);
            }
            else if (choice == 2)
            {
                ConnectToPop3(server, pop3Port, email, password);
            }
            else
            {
                Console.WriteLine("Lựa chọn không hợp lệ.");
            }
        }

        static void ConnectToImap(string server, int port, string email, string password)
        {
            using (var client = new ImapClient())
            {
                try
                {
                    // Kết nối đến IMAP server
                    client.Connect(server, port, SecureSocketOptions.SslOnConnect);

                    // Đăng nhập
                    client.Authenticate(email, password);
                    Console.WriteLine("Đã kết nối tới IMAP server.");

                    // Mở hộp thư đến
                    var inbox = client.Inbox;
                    inbox.Open(MailKit.FolderAccess.ReadOnly);

                    Console.WriteLine($"Bạn có {inbox.Count} email và {inbox.Unread} email chưa đọc.");

                    // Lấy và in nội dung của email đầu tiên
                    for (int i = 0; i < inbox.Count; i++)
                    {
                        var message = inbox.GetMessage(i);
                        Console.WriteLine($"Subject: {message.Subject}");
                        Console.WriteLine($"From: {message.From}");
                        Console.WriteLine($"Date: {message.Date}");
                        Console.WriteLine($"Body: {message.TextBody}");
                    }

                    // Ngắt kết nối
                    client.Disconnect(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi khi kết nối IMAP: " + ex.Message);
                }
            }
        }

        static void ConnectToPop3(string server, int port, string email, string password)
        {
            using (var client = new Pop3Client())
            {
                try
                {
                    // Kết nối đến POP3 server
                    client.Connect(server, port, SecureSocketOptions.SslOnConnect);

                    // Đăng nhập
                    client.Authenticate(email, password);
                    Console.WriteLine("Đã kết nối tới POP3 server.");

                    // In số lượng email có trong hộp thư
                    Console.WriteLine($"Bạn có {client.Count} email.");

                    // Lấy và in nội dung của email đầu tiên
                    for (int i = 0; i < client.Count; i++)
                    {
                        var message = client.GetMessage(i);
                        Console.WriteLine($"Subject: {message.Subject}");
                        Console.WriteLine($"From: {message.From}");
                        Console.WriteLine($"Date: {message.Date}");
                        Console.WriteLine($"Body: {message.TextBody}");
                    }

                    // Ngắt kết nối
                    client.Disconnect(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi khi kết nối POP3: " + ex.Message);
                }
            }
        }
    }
}
