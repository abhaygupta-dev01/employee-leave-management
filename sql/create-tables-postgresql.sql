-- PostgreSQL version of create-tables.sql
-- Run this in Railway's database interface

-- Create Employees table
CREATE TABLE Employees (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Password VARCHAR(100) NOT NULL,
    Role VARCHAR(20) NOT NULL
);

-- Create LeaveRequests table
CREATE TABLE LeaveRequests (
    Id SERIAL PRIMARY KEY,
    EmployeeId INT NOT NULL,
    LeaveType VARCHAR(50) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    Reason TEXT,
    Status VARCHAR(20) NOT NULL,
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id)
);

