using Npgsql;
using LeaveManagementSystem.Models;
using Microsoft.Extensions.Configuration;
using LeaveManagementSystem.Helpers;

namespace LeaveManagementSystem.DataAccessLayer
{
    public class EmployeeDAL
    {
        private readonly string _connectionString;

        public EmployeeDAL(IConfiguration config)
        {
            var rawConnectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
                ?? Environment.GetEnvironmentVariable("POSTGRES_URL")
                ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                ?? Environment.GetEnvironmentVariable("ConnectionStrings_DefaultConnection")
                ?? config.GetConnectionString("DefaultConnection")
                ?? "";
            
            // If it's a PostgreSQL URL, use it directly
            if (rawConnectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase) || 
                rawConnectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
            {
                _connectionString = rawConnectionString;
            }
            else
            {
                _connectionString = ConnectionStringHelper.CleanPostgresConnectionString(rawConnectionString);
            }
        }

        public Employee Login(string email, string password)
        {
            using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Employees WHERE Email=@Email AND Password=@Password";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);
                con.Open();
                NpgsqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    return new Employee
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        Name = dr["Name"].ToString(),
                        Email = dr["Email"].ToString(),
                        Role = dr["Role"].ToString()
                    };
                }
                return null;
            }
        }

        public void Register(Employee emp)
        {
            using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
            {
                string query = "INSERT INTO Employees (Name, Email, Password, Role) VALUES (@Name, @Email, @Password, @Role)";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Name", emp.Name);
                cmd.Parameters.AddWithValue("@Email", emp.Email);
                cmd.Parameters.AddWithValue("@Password", emp.Password);
                cmd.Parameters.AddWithValue("@Role", emp.Role);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<string> GetAdminEmails()
        {
            var adminEmails = new List<string>();
            using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
            {
                string query = "SELECT Email FROM Employees WHERE Role = 'Admin'";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                using (NpgsqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        adminEmails.Add(dr["Email"].ToString()!);
                    }
                }
            }
            return adminEmails;
        }

        public Employee GetEmployeeById(int employeeId)
        {
            using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
            {
                string query = "SELECT Id, Name, Email, Role FROM Employees WHERE Id = @Id";
                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", employeeId);
                con.Open();
                using (NpgsqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return new Employee
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            Name = dr["Name"].ToString(),
                            Email = dr["Email"].ToString(),
                            Role = dr["Role"].ToString()
                        };
                    }
                }
            }
            return null!;
        }
    }
}
