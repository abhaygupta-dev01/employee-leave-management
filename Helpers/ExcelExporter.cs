using ClosedXML.Excel;
using LeaveManagementSystem.Models;
using System.IO;

namespace LeaveManagementSystem.Helpers
{
    public static class ExcelExporter
    {
        public static MemoryStream GenerateLeaveExcel(List<LeaveRequest> data)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Leave Requests");

            worksheet.Cell(1, 1).Value = "ID";
            worksheet.Cell(1, 2).Value = "Employee ID";
            worksheet.Cell(1, 3).Value = "Leave Type";
            worksheet.Cell(1, 4).Value = "Start Date";
            worksheet.Cell(1, 5).Value = "End Date";
            worksheet.Cell(1, 6).Value = "Reason";
            worksheet.Cell(1, 7).Value = "Status";

            for (int i = 0; i < data.Count; i++)
            {
                var leave = data[i];
                worksheet.Cell(i + 2, 1).Value = leave.Id;
                worksheet.Cell(i + 2, 2).Value = leave.EmployeeId;
                worksheet.Cell(i + 2, 3).Value = leave.LeaveType;
                worksheet.Cell(i + 2, 4).Value = leave.StartDate.ToString("dd-MM-yyyy");
                worksheet.Cell(i + 2, 5).Value = leave.EndDate.ToString("dd-MM-yyyy");
                worksheet.Cell(i + 2, 6).Value = leave.Reason;
                worksheet.Cell(i + 2, 7).Value = leave.Status;
            }

            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }
    }
}
