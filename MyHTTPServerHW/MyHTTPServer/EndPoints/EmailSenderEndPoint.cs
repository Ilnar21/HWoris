using System.Net.Http.Headers;
using System.Text;
using HttpServerLibrary;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.Configuration;
using HttpServerLibrary.HttpResponce;

namespace MyHttpServer.EndPoints
{
    public class EmailSenderEndpoint : BaseEndPoint
    {
        // lol GET
        [Get("lol")]
        public IHttpResponceResult LOLPage() //string email, string password)
        {
            Console.WriteLine("УРАААА!!!");
            //return Html(GetRequestType("lol.html"));
            return Html("<h1>Hello World!</h1>");
        }

        // request GET
        [Get("request")]
        public void RequestPage()
        {
        }

        // homework GET
        [Get("homework")]
        public void HomeworkPage()
        {
        }

        // login GET
        // [Get("login")]
        // public IHttpResponceResult LoginPage()
        // {
        //     Console.WriteLine("YES");
        //     return Html("<h1>Hello world</h1>");
        // }


        // lol POST
        [Post("lol")]
        public void SendEmailToLol(string email, string password)
        {
        }

        // request POST
        [Post("request")]
        public void SendEmailToRequest()
        {
        }

        // homework POST
        [Post("homework")]
        public void SendEmailToHomework()
        {
        }

        // login POST
        // [Post("login")]
        // public void SendEmailToLogin()
        // {
        // }
    }
}