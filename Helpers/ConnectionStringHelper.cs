using Npgsql;

namespace LeaveManagementSystem.Helpers
{
    public static class ConnectionStringHelper
    {
        public static string CleanPostgresConnectionString(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            // If it's already a PostgreSQL URL format, use it as-is
            if (connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase) || 
                connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
            {
                return connectionString;
            }

            // Parse and rebuild connection string, removing SQL Server specific parameters
            var builder = new NpgsqlConnectionStringBuilder();
            
            // Split by semicolon and process each part
            var parts = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                if (string.IsNullOrWhiteSpace(part)) continue;
                
                var keyValue = part.Split('=', 2);
                if (keyValue.Length != 2) continue;
                
                var key = keyValue[0].Trim();
                var value = keyValue[1].Trim();
                var keyLower = key.ToLower();
                
                // Skip SQL Server specific parameters
                if (keyLower == "trusted_connection" || 
                    keyLower == "encrypt" || 
                    keyLower == "trustservercertificate" ||
                    (keyLower == "server" && value.Contains("\\")))
                {
                    continue;
                }
                
                // Map common connection string parameters to PostgreSQL
                switch (keyLower)
                {
                    case "host":
                    case "server":
                        if (!value.Contains("\\")) // Skip SQL Server named instances
                            builder.Host = value;
                        break;
                    case "port":
                        if (int.TryParse(value, out int port))
                            builder.Port = port;
                        break;
                    case "database":
                    case "initial catalog":
                        builder.Database = value;
                        break;
                    case "user id":
                    case "uid":
                    case "username":
                    case "user":
                        builder.Username = value;
                        break;
                    case "password":
                    case "pwd":
                        builder.Password = value;
                        break;
                }
            }
            
            return builder.ConnectionString;
        }
    }
}

