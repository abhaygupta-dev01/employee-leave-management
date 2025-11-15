USE LeaveManagementDB;
GO

-- View all employees
SELECT * FROM Employees;

-- View all leave requests
SELECT * FROM LeaveRequests;

-- Count leave requests by status
SELECT Status, COUNT(*) AS Total
FROM LeaveRequests
GROUP BY Status;

-- Monthly leave trends
SELECT 
    FORMAT(StartDate, 'yyyy-MM') AS Month,
    COUNT(*) AS LeaveCount
FROM LeaveRequests
GROUP BY FORMAT(StartDate, 'yyyy-MM')
ORDER BY Month;

-- Filter leave requests by employee
SELECT * FROM LeaveRequests
WHERE EmployeeId = 1;

-- Filter by status
SELECT * FROM LeaveRequests
WHERE Status = 'Pending';
GO
