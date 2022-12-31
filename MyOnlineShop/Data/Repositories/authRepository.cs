using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Text;
using MimeKit;
using System.Net.Mail;
using MailKit.Net.Smtp;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace MyOnlineShop.Data.Repositories
{
    public class authRepository
    {
        public static void verificationCode(string phoneNumber , string message) {

            try
            {
                Kavenegar.KavenegarApi api = new Kavenegar.KavenegarApi("747A5A544864684F4A4D7035414E6A6A413962702B72556A6A7966534C334149714D496A357141657166593D");
                var result = api.Send("SenderLine", phoneNumber , message);

                
                Console.Write("resault.Messageid.ToString()");
                
                
            }
            catch (Kavenegar.Core.Exceptions.ApiException ex)
            {
                // در صورتی که خروجی وب سرویس 200 نباشد این خطارخ می دهد.
                Console.Write("Message : " + ex.Message);
            }
            catch (Kavenegar.Core.Exceptions.HttpException ex)
            {
                // در زمانی که مشکلی در برقرای ارتباط با وب سرویس وجود داشته باشد این خطا رخ می دهد
                Console.Write("Message : " + ex.Message);
            }

            
        }
        public static string SendEmail(string body)
        {

            var email = new MimeMessage();

            email.From.Add(MailboxAddress.Parse("erfanzadsoltani1@gmail.com"));

            email.To.Add(MailboxAddress.Parse("mahsafaramarzi1381@gmail.com"));
            email.Subject = "Verification Code From seven shop";
            email.Body = new TextPart(TextFormat.Html) { Text ="code: "+ body };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 25, SecureSocketOptions.StartTls);
            smtp.Authenticate("erfanzadsoltani1@gmail.com", "lbnfjxagolzcvqja");
            smtp.Send(email);
            smtp.Disconnect(true);

            return "sent";
        }
    }
}
