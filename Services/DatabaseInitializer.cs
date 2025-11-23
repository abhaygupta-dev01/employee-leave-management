using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LeaveManagementSystem.Services
{
    public class DatabaseInitializer
    {
        private readonly string _connectionString;
        private readonly ILogger<DatabaseInitializer>? _logger;

        public DatabaseInitializer(IConfiguration configuration, ILogger<DatabaseInitializer>? logger = null)
        {
            var rawConnectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration), "Connection string not found.");
            
            // Clean the connection string - remove SQL Server specific parameters
            _connectionString = CleanPostgresConnectionString(rawConnectionString);
            _logger = logger;
        }

        private string CleanPostgresConnectionString(string connectionString)
        {
            // If it's already a PostgreSQL URL format (postgresql:// or postgres://), use it as-is
            if (connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase) || 
                connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
            {
                return connectionString;
            }

            // If it contains SQL Server parameters, we need to clean it
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

        public void InitializeDatabase()
        {
            try
            {
                _logger?.LogInformation("Starting database initialization...");
                _logger?.LogInformation("Connection string: {ConnectionString}", 
                    _connectionString?.Substring(0, Math.Min(50, _connectionString?.Length ?? 0)) + "...");

                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                _logger?.LogInformation("Database connection opened successfully.");

                // Create Employees table
                CreateEmployeesTable(connection);

                // Create LeaveRequests table
                CreateLeaveRequestsTable(connection);

                // Verify tables were created
                VerifyTablesExist(connection);

                _logger?.LogInformation("Database initialization completed successfully.");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error during database initialization: {Message}", ex.Message);
                _logger?.LogError(ex, "Stack trace: {StackTrace}", ex.StackTrace);
                // Don't throw - allow app to start even if initialization fails
                // Tables might already exist or connection might not be available yet
            }
        }

        private void VerifyTablesExist(NpgsqlConnection connection)
        {
            string checkTablesQuery = @"
                SELECT table_name 
                FROM information_schema.tables 
                WHERE table_schema = 'public' 
                AND table_name IN ('employees', 'Employees', 'leaverequests', 'LeaveRequests');";

            using var command = new NpgsqlCommand(checkTablesQuery, connection);
            using var reader = command.ExecuteReader();
            
            var tables = new List<string>();
            while (reader.Read())
            {
                tables.Add(reader.GetString(0));
            }
            
            _logger?.LogInformation("Tables found in database: {Tables}", string.Join(", ", tables));
        }

        private void CreateEmployeesTable(NpgsqlConnection connection)
        {
            string createEmployeesTable = @"
                CREATE TABLE IF NOT EXISTS Employees (
                    Id SERIAL PRIMARY KEY,
                    Name VARCHAR(100) NOT NULL,
                    Email VARCHAR(100) NOT NULL UNIQUE,
                    Password VARCHAR(100) NOT NULL,
                    Role VARCHAR(20) NOT NULL
                );";

            using var command = new NpgsqlCommand(createEmployeesTable, connection);
            command.ExecuteNonQuery();
            _logger?.LogInformation("Employees table created or already exists.");
        }

        private void CreateLeaveRequestsTable(NpgsqlConnection connection)
        {
            string createLeaveRequestsTable = @"
                CREATE TABLE IF NOT EXISTS LeaveRequests (
                    Id SERIAL PRIMARY KEY,
                    EmployeeId INT NOT NULL,
                    LeaveType VARCHAR(50) NOT NULL,
                    StartDate DATE NOT NULL,
                    EndDate DATE NOT NULL,
                    Reason TEXT,
                    Status VARCHAR(20) NOT NULL,
                    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id)
                );";

            using var command = new NpgsqlCommand(createLeaveRequestsTable, connection);
            command.ExecuteNonQuery();
            _logger?.LogInformation("LeaveRequests table created or already exists.");
        }
    }
}

