//using System.Data.SqlClient;
using Microsoft.Data.SqlClient;

using LeaveManagementSystem.Models;
using Microsoft.Extensions.Configuration;

namespace LeaveManagementSystem.DataAccessLayer
{
    public class EmployeeDAL
    {
        private readonly string _connectionString;

        public EmployeeDAL(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public Employee Login(string email, string password)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Employees WHERE Email=@Email AND Password=@Password";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
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
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Employees (Name, Email, Password, Role) VALUES (@Name, @Email, @Password, @Role)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Name", emp.Name);
                cmd.Parameters.AddWithValue("@Email", emp.Email);
                cmd.Parameters.AddWithValue("@Password", emp.Password);
                cmd.Parameters.AddWithValue("@Role", emp.Role);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
