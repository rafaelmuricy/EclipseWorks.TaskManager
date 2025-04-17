using Microsoft.Data.Sqlite;
using Dapper;
using System.Reflection;

namespace EclipseWorks.TaskManager.Servico.Servico;
public class BaseDB : IDisposable
{
    private SqliteConnection connection;
    private string ConnectionString { get; init; }
    //private string ConnectionString = "Data Source=taskmanager.db";


    public BaseDB()
    {
        string? connectionString = Environment.GetEnvironmentVariable("ConnectionString");
        if (connectionString == null)
        {
            throw new Exception("A variável de ambiente 'ConnectionString' não está definida.");
        }
        ConnectionString = connectionString;

        connection = new SqliteConnection(ConnectionString);
        connection.Open();
    }

    public int ExecuteQuery(string query, Dictionary<string, object> parametros)
    {
        using (var command = new SqliteCommand(query, connection))
        {
            foreach (var param in parametros)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }

            return command.ExecuteNonQuery();
        }
    }

    public List<T> ExecuteQuery<T>(string query, Dictionary<string, object>? parametros = null)
    {
        using (var command = new SqliteCommand(query, connection))
        {
            List<SqliteParameter> listaParametros = new();

            if (parametros != null)
            {
                foreach (var param in parametros)
                {
                    listaParametros.Add(new SqliteParameter(param.Key, param.Value));
                }
            }
            
            var retorno = connection.Query<T>(query, listaParametros);

            return retorno.ToList();
        }
    }

    public void ExecuteNonQuery(string query, Dictionary<string, object> parametros)
    {
        using (var command = new SqliteCommand(query, connection))
        {
            List<SqliteParameter> listaParametros = new();

            foreach (var param in parametros)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }

            connection.Execute(query, listaParametros);
        }
    }

    public int? ExecuteScalar(string query, Dictionary<string, object> parametros)
    {
        using (var command = new SqliteCommand(query, connection))
        {
            List<SqliteParameter> listaParametros = new();

            foreach (var param in parametros)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }

            var retorno = connection.ExecuteScalar(query, listaParametros);

            if ( retorno != null)
                return (int)retorno;
            else
                return null;
        }
    }

    public static void InicializarBD()
    {
        string? connectionString = Environment.GetEnvironmentVariable("ConnectionString");
        if (connectionString == null)
        {
            throw new Exception("A variável de ambiente 'ConnectionString' não está definida.");
        }

        // Cria o banco de dados SQLite e as tabelas necessárias
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            //tabela de projetos
            var command = new SqliteCommand(@"
                CREATE TABLE IF NOT EXISTS Projeto (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    IdUsuario INTEGER NOT NULL,
                    Nome TEXT NOT NULL
                );
            ", connection);
            command.ExecuteNonQuery();

            //tabela de tarefas
            command = new SqliteCommand(@"
                CREATE TABLE IF NOT EXISTS Tarefas (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    IdProjeto INTEGER NOT NULL,
                    IdUsuario INTEGER NOT NULL,
                    Titulo TEXT NOT NULL,
                    Descricao TEXT NOT NULL,
                    DataVencimento DATETIME NOT NULL,
                    Prioridade TEXT NOT NULL,
                    Status TEXT NOT NULL
                );
            ", connection);
            command.ExecuteNonQuery();


            //histórico
            command = new SqliteCommand(@"
                CREATE TABLE IF NOT EXISTS Historico (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    IdTarefa INTEGER NOT NULL,
                    Comentario TEXT,
                    DataModificacao DATETIME NOT NULL,
                    IdUsuario INTEGER NOT NULL
                );
            ", connection);
            command.ExecuteNonQuery();

            //alteracoes
            command = new SqliteCommand(@"
                CREATE TABLE IF NOT EXISTS Alteracoes (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    IdHistorico INTEGER NOT NULL,
                    Campo TEXT,
                    ValorAntigo TEXT,
                    ValorNovo TEXT,
                );
            ", connection);
            command.ExecuteNonQuery();
        }
    }

    public static List<string> ValidaCampos(object model)
    {
        var camposVazios = new List<string>();

        var properties = model.GetType().GetProperties();

        foreach (PropertyInfo property in properties)
        {
            if (property.GetCustomAttributes(typeof(IgnorarValidacao), false).Length > 0)
            {
                continue;
            }

            //int
            if (model.GetType().GetProperty(property.Name).GetValue(model) == null || model.GetType().GetProperty(property.Name).GetValue(model).GetType() == typeof(int) && ((int)model.GetType().GetProperty(property.Name).GetValue(model)) == 0)
            {
                camposVazios.Add(property.Name.Replace("Id", ""));
            }
            //string
            else if (string.IsNullOrWhiteSpace(model.GetType().GetProperty(property.Name).GetValue(model)?.ToString()))
            {
                camposVazios.Add(property.Name.Replace("Id", ""));
            }

        }


        return camposVazios;
    }

    public void Dispose()
    {
        try
        {
            if (connection != null)
            {
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
                connection.Dispose();
            }
        }
        catch { }
    }
}
