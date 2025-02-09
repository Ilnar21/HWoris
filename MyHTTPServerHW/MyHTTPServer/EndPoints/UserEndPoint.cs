using HttpServerLibrary;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.HttpResponce;
using Microsoft.Data.SqlClient;
using MyHTTPServer.models;

namespace MyHTTPServer.EndPoints;

internal class UserEndpoint : BaseEndPoint
{
    [Get("users")]
    public IHttpResponceResult GetUser()
    {
        string connectionString =
            @"Server=localhost; Database=myDB; User Id=sa; Password=P@ssw0rd;TrustServerCertificate=true;";

        var dBcontext = new ORMContext(connectionString);
        var users = dBcontext.ReadById<User>(10);
        return Json(users);
    }
    
    [Get("usersall")]
    public IHttpResponceResult GetUserAll()
    {
        string connectionString =
            @"Server=localhost; Database=myDB; User Id=sa; Password=P@ssw0rd;TrustServerCertificate=true;";

        var dBcontext = new ORMContext(connectionString);
        var users = dBcontext.ReadByAll<User>();
        return Json(users);
    }
    
    [Get("usersbyname")]
    public IHttpResponceResult GetUserByName()
    {
        string connectionString =
            @"Server=localhost; Database=myDB; User Id=sa; Password=P@ssw0rd;TrustServerCertificate=true;";

        var dBcontext = new ORMContext(connectionString);
        var users = dBcontext.ReadByName<User>("Ilham");
        return Json(users);
    }
}