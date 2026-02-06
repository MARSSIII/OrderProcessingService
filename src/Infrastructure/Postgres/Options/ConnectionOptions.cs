using Npgsql;

namespace Infrastructure.Postgres.Options;

public sealed class ConnectionOptions
{
    public string Host { get; set; } = "localhost";

    public int Port { get; set; } = 5435;

    public string Database { get; set; } = "local_db";

    public string Username { get; set; } = "local_user";

    public string Password { get; set; } = "local_password";

    public string BuildConnectionString()
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = Host,
            Port = Port,
            Database = Database,
            Username = Username,
            Password = Password,
        };

        return builder.ToString();
    }
}
