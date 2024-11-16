using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using MailKit.Search;
using MimeKit;

namespace EmailApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Sự kiện gửi email
        private void btnSend_Click(object sender, EventArgs e)
        {
            // Lấy thông tin email và kiểm tra nếu tất cả các trường đều trống
            string fromEmail = txtEmailFrom.Text.Trim();
            string toEmail = txtEmailTo.Text.Trim();
            string Subject = txtSubject.Text;
            string Body = txtBody.Text;
            string Password = txtPassword.Text.Trim();

            if (fromEmail == "" && toEmail == "" && Subject == "" && Body == "" && Password == "")
            {
                MessageBox.Show("Please fill in the information ! ");
                return;
            }

            // Gửi email và xử lý lỗi
            try
            {
                SendEmail(fromEmail, toEmail, Subject, Body, Password);
                MessageBox.Show("Email sent successfully !");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending email: " + ex.Message);
            }
        }

        // Sự kiện nhận email
        private void btnReceive_Click(object sender, EventArgs e)
        {
            string fromEmail = txtEmailFrom.Text;
            string Password = txtPassword.Text;

            // Kiểm tra nếu email và mật khẩu không hợp lệ
            if (fromEmail == "" && Password == "")
            {
                MessageBox.Show("Please enter valid email and password !");
                return;
            }

            // Nhận email qua POP3 và xử lý lỗi
            try
            {
                ReceiveEmailsByPop3(fromEmail, Password);
                MessageBox.Show("Emails received successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error receiving emails: " + ex.Message);
            }
        }

        // Phương thức gửi email qua SMTP
        private void SendEmail(string fromemail, string toemail, string subject, string body, string password)
        {
            // Tạo nội dung email
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromemail, fromemail));
            message.To.Add(new MailboxAddress(toemail, toemail));
            message.Subject = subject;
            message.Body = new TextPart("plain")
            {
                Text = body
            };

            // Thiết lập và gửi email qua máy chủ SMTP
            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                smtp.Connect("smtp.gmail.com", 587, false);
                smtp.Authenticate(fromemail, password);
                smtp.Send(message);
                smtp.Disconnect(true);
            }
        }

        // Phương thức nhận email qua IMAP
        private void ReceiveEmailsByImap(string email, string password)
        {
            using (var client = new ImapClient())
            {
                client.Connect("imap.gmail.com", 993, true);
                client.Authenticate(email, password);

                // Mở hộp thư đến để đọc
                client.Inbox.Open(MailKit.FolderAccess.ReadOnly);
                var uids = client.Inbox.Search(SearchQuery.All);

                // Xóa danh sách hiện tại và thêm email vào danh sách
                Information.Items.Clear();
                foreach (var uid in uids)
                {
                    var message = client.Inbox.GetMessage(uid);
                    Information.Items.Add($"From: {message.From}, Subject: {message.Subject}");
                }

                client.Disconnect(true);
            }
        }

        // Phương thức nhận email qua POP3
        private void ReceiveEmailsByPop3(string email, string password)
        {
            using (var client = new Pop3Client())
            {
                try
                {
                    // Kết nối với máy chủ POP3 (Gmail yêu cầu SSL)
                    client.Connect("pop.gmail.com", 995, true);
                    client.Authenticate(email, password);

                    // Xóa danh sách hiện tại
                    Information.Items.Clear();

                    // Lấy số lượng email và lặp qua từng email
                    int messageCount = client.Count;
                    for (int i = 0; i < messageCount; i++)
                    {
                        var message = client.GetMessage(i);
                        Information.Items.Add($"From: {message.From}, Subject: {message.Subject}");
                    }

                    MessageBox.Show("Emails received successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error receiving emails: {ex.Message}");
                }
                finally
                {
                    // Ngắt kết nối an toàn
                    client.Disconnect(true);
                }
            }
        }
    }
}
