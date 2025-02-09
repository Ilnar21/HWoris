using System.Net.Http.Headers;
using System.Text;
using System.Web;
using HttpServerLibrary;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.Configuration;
using HttpServerLibrary.HttpResponce;
using MyHTTPServer.Sessions;

namespace MyHTTPServer.EndPoints;

public class DashBoardEndpoint : BaseEndPoint
{
    [Get("dashboard")]
    public IHttpResponceResult GetPage()
    {
        if (!SessionStorage.IsAuthorized(Context)) // Используем метод проверки авторизации
        {
            return Redirect("/login");
        }

        var file = File.ReadAllText(
            @"Templates/Pages/Dashboard/index.html"); //$"{Directory.GetCurrentDirectory()}\\{AppConfig.StaticDirectoryPath}\\theme\\templates\\admin\\login.html");
        return Html(file);
    }
}