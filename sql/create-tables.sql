-- Create database
CREATE DATABASE LeaveManagementDB;
GO

USE LeaveManagementDB;
GO

-- Create Employees table
CREATE TABLE Employees (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL,
    Role NVARCHAR(20) NOT NULL
);
GO

-- Create LeaveRequests table
CREATE TABLE LeaveRequests (
    Id INT PRIMARY KEY IDENTITY(1,1),
    EmployeeId INT NOT NULL,
    LeaveType NVARCHAR(50) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    Reason NVARCHAR(MAX),
    Status NVARCHAR(20) NOT NULL,
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id)
);
GO
