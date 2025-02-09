using System.Collections;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using Microsoft.Data.SqlClient;

namespace MyORMLibrary;

public class TestORMContext<T> where T : class, new()
{
    private readonly string _connectionString;
    private readonly IDbConnection _dbConnection;

    public TestORMContext(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public T GetById(int id)
    {
        string query =
            $"SELECT * FROM {typeof(T).Name}s WHERE Id = @Id"; 

        _dbConnection.Open();

        using (var command = _dbConnection.CreateCommand())
        {
            command.CommandText = query;
            
            var parameter = command.CreateParameter();
            parameter.ParameterName = "@Id";
            parameter.Value = id;
            command.Parameters.Add(parameter);
            //command.Parameters.AddWithValue("@Id", id);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return Map(reader);
                }
            }
        }

        return null;
    }
    public T GetByAll()
    {
        string query =
            $"SELECT * FROM {typeof(T).Name}s"; 

        _dbConnection.Open();

        using (var command = _dbConnection.CreateCommand())
        {
            command.CommandText = query;

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return Map(reader);
                }
            }
        }
        return null;
    }
    public void Update(int id)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string sql = $"UPDATE {typeof(T).Name}s SET Column1 = data WHERE Id = @id";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@value1", "значение");
    
            command.ExecuteNonQuery();
        }
    }
    public void Delete(int id)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string sql = $"DELETE FROM {typeof(T).Name}s WHERE Id = @id";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();
        }
    }
    private T Map(IDataReader reader)
    {
        var entity = new T();
        var properties = typeof(T).GetProperties();

        foreach (var property in properties)
        {
            if (reader[property.Name] != DBNull.Value)
            {
                property.SetValue(entity, reader[property.Name]);
            }
        }

        return entity;
    }
    
    // new
    private string ParseExpression(Expression expression)
    {
        if (expression is BinaryExpression binary)
        {
            // разбираем выражение на составляющие
            var left = ParseExpression(binary.Left);  
            var right = ParseExpression(binary.Right); 
            var op = GetSqlOperator(binary.NodeType);  
            return $"({left} {op} {right})";
        }
        else if (expression is MemberExpression member)
        {
            return member.Member.Name; 
        }
        else if (expression is ConstantExpression constant)
        {
            return FormatConstant(constant.Value); 
        }
        throw new NotSupportedException($"Unsupported expression type: {expression.GetType().Name}");
    }
 
    private string GetSqlOperator(ExpressionType nodeType)
    {
        return nodeType switch
        {
            ExpressionType.Equal => "=",
            ExpressionType.AndAlso => "AND",
            ExpressionType.NotEqual => "<>",
            ExpressionType.GreaterThan => ">",
            ExpressionType.LessThan => "<",
            ExpressionType.GreaterThanOrEqual => ">=",
            ExpressionType.LessThanOrEqual => "<=",
            _ => throw new NotSupportedException($"Unsupported node type: {nodeType}")
        };
    }
 
    private string FormatConstant(object value)
    {
        return value is string ? $"'{value}'" : value.ToString();
    }
 
    private string BuildSqlQuery(Expression<Func<T, bool>> predicate, bool singleResult)
    {
        var tableName = typeof(T).Name + "s";
        var whereClause = ParseExpression(predicate.Body);
        var limitClause = singleResult ? "TOP 1" : string.Empty;
 
        return $"SELECT * FROM {tableName} WHERE {whereClause} {limitClause}".Trim(); // TODO
    }
    
    public T FirstOrDefault(Expression<Func<T, bool>> predicate)
    {
        var sqlQuery = BuildSqlQuery(predicate, singleResult: true);
        return ExecuteQuerySingle(sqlQuery);
    }
    
    public IEnumerable<T> Where(string query) 
    {
        string sqlQuery = "SELECT * FROM Users WHERE "+ query;
        return ExecuteQueryMultiple(sqlQuery);
    }
    
    private T ExecuteQuerySingle(string query)
    {
        using (var command = _dbConnection.CreateCommand())
        {
            command.CommandText = query;
            _dbConnection.Open();
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return Map(reader);
                }
            }
            _dbConnection.Close();
        }
 
        return null;
    }
 
    private IEnumerable<T> ExecuteQueryMultiple(string query)
    {
        var results = new List<T>();
        using (var command =  _dbConnection.CreateCommand())
        {
            command.CommandText = query;
            _dbConnection.Open();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    results.Add(Map(reader));
                }
            }
            _dbConnection.Close();
        }
        return results;
    }
    
    public IEnumerable<T> Where2(string request)
    {
        List<T> list = new List<T>();
        string query =
            $"SELECT * FROM {typeof(T).Name}s WHERE {request}"; 
        _dbConnection.Open();

        using (var command = _dbConnection.CreateCommand())
        {
            command.CommandText = query;

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    list.Add(Map(reader));
                }
            }
        }
        return list;
    }
}