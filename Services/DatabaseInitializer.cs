using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using LeaveManagementSystem.Helpers;

namespace LeaveManagementSystem.Services
{
    public class DatabaseInitializer
    {
        private readonly string _connectionString;
        private readonly ILogger<DatabaseInitializer>? _logger;

        public DatabaseInitializer(IConfiguration configuration, ILogger<DatabaseInitializer>? logger = null)
        {
            _logger = logger;
            
            // CRITICAL DEBUG: Log ALL environment variables to see what Railway is providing
            _logger?.LogError("=== DEBUG: ALL ENVIRONMENT VARIABLES ===");
            var allVars = Environment.GetEnvironmentVariables();
            foreach (string key in allVars.Keys.Cast<string>().OrderBy(k => k))
            {
                var value = allVars[key]?.ToString() ?? "";
                if (value.Length > 100) value = value.Substring(0, 100) + "...";
                _logger?.LogError("ENV VAR: {Key} = {Value}", key, value);
            }
            _logger?.LogError("=== END DEBUG ===");
            
            string? rawConnectionString = null;
            string? source = null;
            
            // Try multiple ways to get the connection string
            // Priority order: Environment variables first (they override appsettings.json)
            
            // 1. Try Railway's DATABASE_URL first (most common for Railway deployments)
            var dbUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            var postgresUrl = Environment.GetEnvironmentVariable("POSTGRES_URL");
            _logger?.LogError("DATABASE_URL exists: {Exists}, POSTGRES_URL exists: {Exists2}", 
                !string.IsNullOrWhiteSpace(dbUrl), !string.IsNullOrWhiteSpace(postgresUrl));
            
            rawConnectionString = dbUrl ?? postgresUrl;
            if (!string.IsNullOrWhiteSpace(rawConnectionString))
            {
                source = "DATABASE_URL or POSTGRES_URL environment variable";
                _logger?.LogError("USING DATABASE_URL: {Value}", rawConnectionString.Substring(0, Math.Min(50, rawConnectionString.Length)));
            }
            
            // 2. Direct environment variable (double underscore)
            if (string.IsNullOrWhiteSpace(rawConnectionString))
            {
                rawConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
                if (!string.IsNullOrWhiteSpace(rawConnectionString))
                {
                    source = "Environment variable (ConnectionStrings__DefaultConnection)";
                }
            }
            
            // 3. Direct environment variable (single underscore)
            if (string.IsNullOrWhiteSpace(rawConnectionString))
            {
                rawConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings_DefaultConnection");
                if (!string.IsNullOrWhiteSpace(rawConnectionString))
                {
                    source = "Environment variable (ConnectionStrings_DefaultConnection)";
                }
            }
            
            // 4. From configuration system (should read env vars automatically, but may fall back to appsettings.json)
            if (string.IsNullOrWhiteSpace(rawConnectionString))
            {
                rawConnectionString = configuration.GetConnectionString("DefaultConnection");
                if (!string.IsNullOrWhiteSpace(rawConnectionString))
                {
                    source = "Configuration system (appsettings.json or env var)";
                }
            }
            
            // 5. Try reading from configuration section directly
            if (string.IsNullOrWhiteSpace(rawConnectionString))
            {
                rawConnectionString = configuration["ConnectionStrings:DefaultConnection"];
                if (!string.IsNullOrWhiteSpace(rawConnectionString))
                {
                    source = "Configuration section (ConnectionStrings:DefaultConnection)";
                }
            }
            
            // Log what we found
            if (!string.IsNullOrWhiteSpace(rawConnectionString))
            {
                var preview = rawConnectionString.Length > 50 
                    ? rawConnectionString.Substring(0, 50) + "..." 
                    : rawConnectionString;
                _logger?.LogInformation("Found connection string from {Source}: {ConnectionString}", source, preview);
            }
            else
            {
                _logger?.LogError("Connection string not found in any location!");
                _logger?.LogError("Checked: ConnectionStrings__DefaultConnection, ConnectionStrings_DefaultConnection, Configuration, DATABASE_URL, POSTGRES_URL");
            }
            
            if (string.IsNullOrWhiteSpace(rawConnectionString))
            {
                throw new ArgumentNullException(nameof(configuration), 
                    "Connection string not found. Please set ConnectionStrings__DefaultConnection or DATABASE_URL environment variable.");
            }
            
            _logger?.LogError("Raw connection string from {Source}: {ConnectionString}", source, 
                rawConnectionString.Substring(0, Math.Min(80, rawConnectionString.Length)));
            
            // If it's already a PostgreSQL URL, use it directly (Npgsql supports URLs)
            if (rawConnectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase) || 
                rawConnectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
            {
                _logger?.LogError("Using PostgreSQL URL directly (no parsing needed)");
                _connectionString = rawConnectionString;
            }
            else
            {
                // Clean the connection string - remove SQL Server specific parameters
                _connectionString = ConnectionStringHelper.CleanPostgresConnectionString(rawConnectionString);
            }
        }

        public void InitializeDatabase()
        {
            try
            {
                _logger?.LogInformation("Starting database initialization...");
                // Log connection string info (mask password)
                var connectionStringForLog = _connectionString ?? "";
                if (connectionStringForLog.Contains("Password="))
                {
                    var parts = connectionStringForLog.Split(';');
                    var maskedParts = parts.Select(p => 
                        p.Trim().StartsWith("Password=", StringComparison.OrdinalIgnoreCase) 
                            ? "Password=***" 
                            : p);
                    connectionStringForLog = string.Join(";", maskedParts);
                }
                _logger?.LogInformation("Connection string (masked): {ConnectionString}", 
                    connectionStringForLog.Substring(0, Math.Min(100, connectionStringForLog.Length)));

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

