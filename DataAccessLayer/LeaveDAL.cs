using Microsoft.Data.SqlClient;
using LeaveManagementSystem.Models;

namespace LeaveManagementSystem.DataAccessLayer
{
    public class LeaveDAL
    {
        private readonly string _connectionString;

        public LeaveDAL(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string not found.");
        }

        public void SubmitLeave(LeaveRequest leave)
        {
            using var con = new SqlConnection(_connectionString);
            string query = @"
                INSERT INTO LeaveRequests (EmployeeId, LeaveType, StartDate, EndDate, Reason, Status)
                VALUES (@EmployeeId, @LeaveType, @StartDate, @EndDate, @Reason, 'Pending')";
            using var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@EmployeeId", leave.EmployeeId);
            cmd.Parameters.AddWithValue("@LeaveType", leave.LeaveType);
            cmd.Parameters.AddWithValue("@StartDate", leave.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", leave.EndDate);
            cmd.Parameters.AddWithValue("@Reason", leave.Reason);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        public List<LeaveRequest> GetLeaveHistoryByEmployee(int employeeId)
        {
            var list = new List<LeaveRequest>();
            using var con = new SqlConnection(_connectionString);
            string query = @"
                SELECT lr.*, e.Name AS EmployeeName
                FROM LeaveRequests lr
                INNER JOIN Employees e ON lr.EmployeeId = e.Id
                WHERE lr.EmployeeId = @EmployeeId
                ORDER BY lr.StartDate DESC";
            using var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                list.Add(new LeaveRequest
                {
                    Id = Convert.ToInt32(dr["Id"]),
                    EmployeeId = Convert.ToInt32(dr["EmployeeId"]),
                    LeaveType = dr["LeaveType"].ToString()!,
                    StartDate = Convert.ToDateTime(dr["StartDate"]),
                    EndDate = Convert.ToDateTime(dr["EndDate"]),
                    Reason = dr["Reason"].ToString()!,
                    Status = dr["Status"].ToString()!,
                    EmployeeName = dr["EmployeeName"].ToString()!
                });
            }
            return list;
        }

        public List<LeaveRequest> GetFilteredLeaveRequests(string status, string keyword)
        {
            var list = new List<LeaveRequest>();
            using var con = new SqlConnection(_connectionString);
            string query = @"
                SELECT lr.Id, lr.EmployeeId, lr.LeaveType, lr.StartDate, lr.EndDate, lr.Reason, lr.Status,
                       e.Name AS EmployeeName
                FROM LeaveRequests lr
                INNER JOIN Employees e ON lr.EmployeeId = e.Id
                WHERE (@Status = '' OR lr.Status = @Status)
                  AND (@Keyword = '' OR e.Name LIKE '%' + @Keyword + '%' OR CAST(lr.EmployeeId AS NVARCHAR) LIKE '%' + @Keyword + '%')
                ORDER BY lr.StartDate DESC";
            using var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Status", status ?? "");
            cmd.Parameters.AddWithValue("@Keyword", keyword ?? "");
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                list.Add(new LeaveRequest
                {
                    Id = Convert.ToInt32(dr["Id"]),
                    EmployeeId = Convert.ToInt32(dr["EmployeeId"]),
                    EmployeeName = dr["EmployeeName"].ToString()!,
                    LeaveType = dr["LeaveType"].ToString()!,
                    StartDate = Convert.ToDateTime(dr["StartDate"]),
                    EndDate = Convert.ToDateTime(dr["EndDate"]),
                    Reason = dr["Reason"].ToString()!,
                    Status = dr["Status"].ToString()!
                });
            }
            return list;
        }

        public LeaveRequest GetLeaveRequestById(int id)
        {
            using var con = new SqlConnection(_connectionString);
            string query = @"
                SELECT lr.Id, lr.EmployeeId, lr.LeaveType, lr.StartDate, lr.EndDate, lr.Reason, lr.Status,
                       e.Email AS EmployeeEmail,
                       e.Name AS EmployeeName
                FROM LeaveRequests lr
                INNER JOIN Employees e ON lr.EmployeeId = e.Id
                WHERE lr.Id = @Id";
            using var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            using var dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                return new LeaveRequest
                {
                    Id = Convert.ToInt32(dr["Id"]),
                    EmployeeId = Convert.ToInt32(dr["EmployeeId"]),
                    LeaveType = dr["LeaveType"].ToString()!,
                    StartDate = Convert.ToDateTime(dr["StartDate"]),
                    EndDate = Convert.ToDateTime(dr["EndDate"]),
                    Reason = dr["Reason"].ToString()!,
                    Status = dr["Status"].ToString()!,
                    EmployeeEmail = dr["EmployeeEmail"].ToString()!,
                    EmployeeName = dr["EmployeeName"].ToString()!
                };
            }
            return null!;
        }

        public void UpdateLeaveStatus(int id, string status)
        {
            using var con = new SqlConnection(_connectionString);
            string query = @"UPDATE LeaveRequests SET Status = @Status WHERE Id = @Id";
            using var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Status", status);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        public Dictionary<string, int> GetLeaveSummary()
        {
            var result = new Dictionary<string, int>();
            using var con = new SqlConnection(_connectionString);
            string query = @"SELECT Status, COUNT(*) AS Count FROM LeaveRequests GROUP BY Status";
            using var cmd = new SqlCommand(query, con);
            con.Open();
            using var dr = cmd.ExecuteReader();
            int total = 0;
            while (dr.Read())
            {
                string status = dr["Status"].ToString()!;
                int count = Convert.ToInt32(dr["Count"]);
                result[status] = count;
                total += count;
            }
            result["Total"] = total;
            return result;
        }

        public Dictionary<string, int> GetMonthlyLeaveCounts()
        {
            var result = new Dictionary<string, int>();
            using var con = new SqlConnection(_connectionString);
            string query = @"
                SELECT FORMAT(StartDate, 'MMM yyyy') AS Month, COUNT(*) AS Count
                FROM LeaveRequests
                GROUP BY FORMAT(StartDate, 'MMM yyyy')
                ORDER BY MIN(StartDate)";
            using var cmd = new SqlCommand(query, con);
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                result[dr["Month"].ToString()!] = Convert.ToInt32(dr["Count"]);
            }
            return result;
        }
    }
}
