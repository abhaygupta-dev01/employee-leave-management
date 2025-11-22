USE LeaveManagementDB;
GO

-- Insert sample employees
INSERT INTO Employees (Name, Email, Password, Role)
VALUES
('Abhay Gupta', 'abhaygupta.in1@gmail.com', '123', 'Employee'),
('Alok Gupta', 'alok@demo.com', '123', 'Admin'),
('Ravi Kumar', 'ravi@demo.com', '123', 'Employee'),
('Sneha Sharma', 'sneha@demo.com', '123', 'Employee');
GO

-- Insert sample leave requests
INSERT INTO LeaveRequests (EmployeeId, LeaveType, StartDate, EndDate, Reason, Status)
VALUES
(1, 'Sick', '2025-10-11', '2025-10-13', 'any', 'Rejected'),
(2, 'Casual', '2025-10-18', '2025-10-25', 'any', 'Approved'),
(3, 'Annual', '2025-10-13', '2025-10-13', 'urgent', 'Approved'),
(4, 'Casual', '2025-11-03', '2025-11-04', 'home', 'Approved');
GO
