using System.Net;
using HttpServerLibrary;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.HttpResponce;
using Microsoft.Data.SqlClient;
using MyHTTPServer.models;
using MyORMLibrary;
using MyHTTPServer.Sessions;

namespace MyHTTPServer.EndPoints;

public class AuthEndPoint : BaseEndPoint
{
    [Get("login")]
    public IHttpResponceResult AuthGet()
    {
        if (SessionStorage.IsAuthorized(Context)) 
        {
            return Redirect("dashboard");
        }
        
        var file = File.ReadAllText(
            @"Templates/Pages/Auth/login.html");
        return Html(file);
    }

    [Post("login")] 
    public IHttpResponceResult AuthPost(string email, string password)
    {
        string connectionString =
            @"Server=localhost; Database=myDB; User Id=sa; Password=P@ssw0rd;TrustServerCertificate=true;";
        var connection = new SqlConnection(connectionString);
        var context = new TestORMContext<User>(connection);

        //var users = context.Where(x => x.email = email);
        var user = context.Where($"email = '{email}' AND login = '{password}'").FirstOrDefault();// && x.Password == password);
        if (user == null)
        {
            return Redirect("login");
        }

        string token = Guid.NewGuid().ToString();
        Cookie cookie = new Cookie("session-token", token);
        Context.Response.SetCookie(cookie);
        
        SessionStorage.SaveSession(token, user.Id.ToString());
        
        return Redirect("dashboard");
    }
}