using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer.Services
{
    /// <summary>
    /// Class for sending emails using smtp
    /// </summary>
    public class MailService : IMailService
    {
        public async Task SendAsync(string email, string message, string path)
        {   
            Console.WriteLine($"{path} path in mail"); 

            // конфигурация smtp для отправки сообщений
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);

            using (smtp) 
            {
                smtp.Credentials = new NetworkCredential("ilnar.val21@yandex.ru", "sgcczeiwnsxblmal"); //Gmail credentials - Replace with your credentials.
                smtp.EnableSsl = true;                                                                                                          

                using (MailMessage m = new MailMessage()) // создание сообщения
                {
                    m.From = new MailAddress("valiullinilnar2005@gmail.com"); // куда отправить сообщение
                    m.To.Add(email); // куда отправить сообщение
                    m.Subject = "Тест"; // "Тема" сообщения

                    // задает разные тела сообщений в зависимости от пути файла
                    switch (path.Split(@"\")[^1])
                    {
                        case "logIn.html":
                            m.Body = $"Добро пожаловать в систему ваш логин {email} ваш пароль {message}";
                            break;

                        case "lol.html":
                            m.Body =
                                $"Ха-ха вы попались, ваш логин {email} ваш пароль {message} теперь знаю я Низамов Алмаз";
                            break;

                        case "requests.html":
                            m.Body = $"Вы подписались на рассылку ваш логин {email} ваш пароль {message}";
                            break;

                        case "home-work.html":
                            // добавление файла к сообщению
                            m.Attachments.Add(new Attachment("C:\\Users\\valiu\\OneDrive\\Рабочий стол\\MyHttpServer\\MyHttpServer\\MyHttpServer\\public\\MyHttpServerHomeWork.zip"));
                            m.Body = "Мое ДЗ. Валиуллин Ильнар Ильгизович";
                            break;
                    }

                    try
                    {
                        await smtp.SendMailAsync(m); // асинхронная отправка сообщения
                        Console.WriteLine("Сообщение отправлено"); //
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Ошибка отправки сообщения: {e.Message}"); 
                        Console.WriteLine($"Ошибка отправки сообщения: {e.StackTrace}"); 
                    }
                }
            }
        }
    }
}