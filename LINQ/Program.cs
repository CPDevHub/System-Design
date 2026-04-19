using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class Department
{
    public int DepartmentId { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public decimal Budget { get; set; }
}

public class Employee
{
    public int EmployeeId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime HireDate { get; set; }
    public decimal Salary { get; set; }
    public int? DepartmentId { get; set; }
    public int? ManagerId { get; set; }
}

public class Project
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal Budget { get; set; }
    public int DepartmentId { get; set; }
}

public class EmployeeProject
{
    public int EmployeeId { get; set; }
    public int ProjectId { get; set; }
    public string Role { get; set; }
    public int HoursWorked { get; set; }
}

public class SalaryHistory
{
    public int HistoryId { get; set; }
    public int EmployeeId { get; set; }
    public decimal OldSalary { get; set; }
    public decimal NewSalary { get; set; }
    public DateTime ChangeDate { get; set; }
}

class Program
{
    static void Main()
    {
        var departments = GetDepartments();
        var employees = GetEmployees();
        var projects = GetProjects();
        var employeeProjects = GetEmployeeProjects();
        var salaryHistory = GetSalaryHistory();

        
        //Q1) List all employees showing their full name (FirstName and LastName combined into one column called FullName), email address, and salary. Order results by salary from highest to lowest.
        var empInfo=employees.Select(emp=> new {fulllName= $"{emp.FirstName} {emp.LastName}", emp.Email, emp.Salary}).OrderByDescending(emp=>emp.Salary);
        foreach(var emp in empInfo)
        {
            Console.WriteLine($"Name: {emp.fulllName}, Email: {emp.Email}, Salary: {emp.Salary}");
        }

        // Q2)Find all unique office locations across all departments. Return the distinct location names as a flat list, sorted alphabetically.
        var distinctDeptLocation=departments.GroupBy(dept=>dept.Name).Select(group=> new {Name=group.Key, Locations=group.Select(g=>g.Location).OrderBy(location=>location)});
        foreach(var dept in distinctDeptLocation)
        {
            Console.WriteLine($"Department: {dept.Name}, Locations: {string.Join(", ", dept.Locations)}");
        }

        // Q3: Calculate the average salary for each department. Show the department name and average salary. Only include departments that have at least one employee
        var avarageSalDepartment=departments.GroupJoin(employees, (dept)=>dept.DepartmentId, (emp)=>emp.DepartmentId, (dept, groupEmp)=>new {departmentId=dept.DepartmentId, departmentName=dept.Name, averageSalary=groupEmp.Count()>0 ? groupEmp.Average(emp=>emp.Salary) : 0}).OrderByDescending(dept=>dept.averageSalary);

        foreach(var emp in avarageSalDepartment)
        {
            Console.WriteLine($"Department: {emp.departmentName}, Average Salary: {emp.averageSalary}");    
        }

        // Q5)List all employees who are the direct manager of at least one other employee (i.e., their EmployeeId appears in another employee's ManagerId column). Show their full name and department name.
        var managers=employees.Join(employees, (emp)=>emp.EmployeeId, (mgr)=>mgr.ManagerId, (mgr, emp)=> new {EmployeeName= $"{emp.FirstName} {emp.LastName}", ManagerName=$"{mgr.FirstName} {mgr.LastName}"});

        foreach(var emp in managers)
        {
            Console.WriteLine($"Employee: {emp.EmployeeName}, Manager: {emp.ManagerName}");
        }

        //Q6)List every project along with the total number of employees assigned to it. Projects that have zero employees assigned must still appear and show 0. Order by employee count descending.
        var projTotalEmp=projects.GroupJoin(employeeProjects,(proj)=>proj.ProjectId, (emProj)=>emProj.ProjectId,(proj, emProjGroup)=> new {proj.ProjectId, proj.ProjectName, TotalEmployees=emProjGroup.Count()}).OrderByDescending(projects=>projects.TotalEmployees);

        foreach(var proj in projTotalEmp)
        {
            Console.WriteLine($"Project: {proj.ProjectName}, Total Employees: {proj.TotalEmployees}");
        }
    }

    // ============================================================
    // DEPARTMENTS
    // ============================================================
    static List<Department> GetDepartments() => new()
    {
        new Department { DepartmentId=1, Name="Engineering", Location="New York", Budget=500000m },
        new Department { DepartmentId=2, Name="Marketing", Location="Chicago", Budget=300000m },
        new Department { DepartmentId=3, Name="Sales", Location="New York", Budget=250000m },
        new Department { DepartmentId=4, Name="HR", Location="Chicago", Budget=150000m },
        new Department { DepartmentId=5, Name="Finance", Location="Boston", Budget=200000m },
        new Department { DepartmentId=6, Name="Legal", Location="Boston", Budget=180000m },
        new Department { DepartmentId=7, Name="Operations", Location="New York", Budget=220000m },
        new Department { DepartmentId=8, Name="Research", Location="Austin", Budget=400000m }
    };

    // ============================================================
    // EMPLOYEES
    // ============================================================
    static List<Employee> GetEmployees() => new()
    {
        new Employee { EmployeeId=1, FirstName="Alice", LastName="Johnson", Email="alice.johnson@corp.com", HireDate=new DateTime(2018,3,15), Salary=95000m, DepartmentId=1, ManagerId=null },
        new Employee { EmployeeId=2, FirstName="Bob", LastName="Smith", Email="bob.smith@corp.com", HireDate=new DateTime(2019,7,22), Salary=88000m, DepartmentId=1, ManagerId=1 },
        new Employee { EmployeeId=3, FirstName="Carol", LastName="White", Email="carol.white@corp.com", HireDate=new DateTime(2019,4,5), Salary=75000m, DepartmentId=1, ManagerId=1 },
        new Employee { EmployeeId=4, FirstName="David", LastName="Brown", Email="david.brown@corp.com", HireDate=new DateTime(2017,5,10), Salary=72000m, DepartmentId=2, ManagerId=null },
        new Employee { EmployeeId=5, FirstName="Eve", LastName="Davis", Email="eve.davis@corp.com", HireDate=new DateTime(2021,2,14), Salary=65000m, DepartmentId=2, ManagerId=4 },
        new Employee { EmployeeId=6, FirstName="Frank", LastName="Miller", Email="frank.miller@corp.com", HireDate=new DateTime(2021,8,30), Salary=65000m, DepartmentId=2, ManagerId=4 },
        new Employee { EmployeeId=7, FirstName="Grace", LastName="Wilson", Email="grace.wilson@corp.com", HireDate=new DateTime(2016,9,12), Salary=58000m, DepartmentId=3, ManagerId=null },
        new Employee { EmployeeId=8, FirstName="Henry", LastName="Moore", Email="henry.moore@corp.com", HireDate=new DateTime(2019,4,18), Salary=62000m, DepartmentId=3, ManagerId=7 },
        new Employee { EmployeeId=9, FirstName="Iris", LastName="Taylor", Email="iris.taylor@corp.com", HireDate=new DateTime(2019,8,15), Salary=60000m, DepartmentId=3, ManagerId=7 },
        new Employee { EmployeeId=10, FirstName="Jack", LastName="Anderson", Email="jack.anderson@corp.com", HireDate=new DateTime(2018,6,30), Salary=55000m, DepartmentId=4, ManagerId=null },
        new Employee { EmployeeId=11, FirstName="Kate", LastName="Thomas", Email="kate.thomas@corp.com", HireDate=new DateTime(2022,1,10), Salary=52000m, DepartmentId=4, ManagerId=10 },
        new Employee { EmployeeId=12, FirstName="Leo", LastName="Jackson", Email="leo.jackson@corp.com", HireDate=new DateTime(2019,9,5), Salary=80000m, DepartmentId=5, ManagerId=null },
        new Employee { EmployeeId=13, FirstName="Mia", LastName="Harris", Email="mia.harris@corp.com", HireDate=new DateTime(2020,3,18), Salary=77000m, DepartmentId=5, ManagerId=12 },
        new Employee { EmployeeId=14, FirstName="Noah", LastName="Martin", Email="noah.martin@corp.com", HireDate=new DateTime(2018,12,1), Salary=83000m, DepartmentId=5, ManagerId=12 },
        new Employee { EmployeeId=15, FirstName="Olivia", LastName="Garcia", Email="olivia.garcia@corp.com", HireDate=new DateTime(2015,11,28), Salary=90000m, DepartmentId=6, ManagerId=null },
        new Employee { EmployeeId=16, FirstName="Peter", LastName="Martinez", Email="peter.martinez@corp.com", HireDate=new DateTime(2020,7,14), Salary=85000m, DepartmentId=6, ManagerId=15 },
        new Employee { EmployeeId=17, FirstName="Quinn", LastName="Robinson", Email="quinn.robinson@corp.com", HireDate=new DateTime(2020,3,1), Salary=68000m, DepartmentId=7, ManagerId=null },
        new Employee { EmployeeId=18, FirstName="Rachel", LastName="Clark", Email="rachel.clark@corp.com", HireDate=new DateTime(2021,5,20), Salary=71000m, DepartmentId=null, ManagerId=1 }
    };

    // ============================================================
    // PROJECTS
    // ============================================================
    static List<Project> GetProjects() => new()
    {
        new Project { ProjectId=1, ProjectName="Cloud Migration", StartDate=new DateTime(2023,1,15), EndDate=new DateTime(2023,12,31), Budget=150000m, DepartmentId=1 },
        new Project { ProjectId=2, ProjectName="Brand Refresh", StartDate=new DateTime(2023,3,1), EndDate=new DateTime(2023,9,30), Budget=80000m, DepartmentId=2 },
        new Project { ProjectId=3, ProjectName="Sales Portal", StartDate=new DateTime(2023,6,1), EndDate=null, Budget=120000m, DepartmentId=3 },
        new Project { ProjectId=4, ProjectName="HR System Upgrade", StartDate=new DateTime(2024,1,1), EndDate=null, Budget=90000m, DepartmentId=4 },
        new Project { ProjectId=5, ProjectName="Financial Audit", StartDate=new DateTime(2023,8,1), EndDate=new DateTime(2023,11,30), Budget=60000m, DepartmentId=5 },
        new Project { ProjectId=6, ProjectName="Legal Compliance Platform", StartDate=new DateTime(2024,2,1), EndDate=null, Budget=200000m, DepartmentId=6 },
        new Project { ProjectId=7, ProjectName="Data Pipeline", StartDate=new DateTime(2023,5,1), EndDate=null, Budget=300000m, DepartmentId=1 }
    };

    // ============================================================
    // EMPLOYEE PROJECTS
    // ============================================================
    static List<EmployeeProject> GetEmployeeProjects() => new()
    {
        new EmployeeProject { EmployeeId=1, ProjectId=1, Role="Tech Lead", HoursWorked=200 },
        new EmployeeProject { EmployeeId=1, ProjectId=3, Role="Consultant", HoursWorked=150 },
        new EmployeeProject { EmployeeId=1, ProjectId=5, Role="Analyst", HoursWorked=80 },
        new EmployeeProject { EmployeeId=2, ProjectId=1, Role="Developer", HoursWorked=320 },
        new EmployeeProject { EmployeeId=2, ProjectId=2, Role="Consultant", HoursWorked=120 },
        new EmployeeProject { EmployeeId=3, ProjectId=1, Role="Developer", HoursWorked=280 },
        new EmployeeProject { EmployeeId=4, ProjectId=2, Role="Campaign Lead", HoursWorked=200 },
        new EmployeeProject { EmployeeId=4, ProjectId=5, Role="Reviewer", HoursWorked=60 },
        new EmployeeProject { EmployeeId=5, ProjectId=2, Role="Designer", HoursWorked=180 },
        new EmployeeProject { EmployeeId=6, ProjectId=2, Role="Copywriter", HoursWorked=160 },
        new EmployeeProject { EmployeeId=6, ProjectId=3, Role="Coordinator", HoursWorked=100 },
        new EmployeeProject { EmployeeId=7, ProjectId=3, Role="Sales Lead", HoursWorked=400 },
        new EmployeeProject { EmployeeId=8, ProjectId=3, Role="Sales Rep", HoursWorked=350 },
        new EmployeeProject { EmployeeId=8, ProjectId=4, Role="Tester", HoursWorked=140 },
        new EmployeeProject { EmployeeId=9, ProjectId=3, Role="Sales Rep", HoursWorked=300 },
        new EmployeeProject { EmployeeId=10, ProjectId=4, Role="Project Manager", HoursWorked=100 },
        new EmployeeProject { EmployeeId=11, ProjectId=4, Role="Developer", HoursWorked=80 },
        new EmployeeProject { EmployeeId=12, ProjectId=5, Role="Lead Auditor", HoursWorked=90 },
        new EmployeeProject { EmployeeId=12, ProjectId=6, Role="Compliance Lead", HoursWorked=250 },
        new EmployeeProject { EmployeeId=13, ProjectId=5, Role="Auditor", HoursWorked=70 },
        new EmployeeProject { EmployeeId=14, ProjectId=1, Role="Architect", HoursWorked=150 },
        new EmployeeProject { EmployeeId=14, ProjectId=2, Role="Analyst", HoursWorked=90 },
        new EmployeeProject { EmployeeId=14, ProjectId=6, Role="Legal Analyst", HoursWorked=300 },
        new EmployeeProject { EmployeeId=15, ProjectId=6, Role="Partner", HoursWorked=180 },
        new EmployeeProject { EmployeeId=16, ProjectId=6, Role="Associate", HoursWorked=160 }
    };

    // ============================================================
    // SALARY HISTORY
    // ============================================================
    static List<SalaryHistory> GetSalaryHistory() => new()
    {
        new SalaryHistory { HistoryId=1, EmployeeId=2, OldSalary=75000m, NewSalary=80000m, ChangeDate=new DateTime(2021,1,15) },
        new SalaryHistory { HistoryId=2, EmployeeId=2, OldSalary=80000m, NewSalary=88000m, ChangeDate=new DateTime(2022,3,10) },
        new SalaryHistory { HistoryId=3, EmployeeId=3, OldSalary=68000m, NewSalary=75000m, ChangeDate=new DateTime(2021,6,1) },
        new SalaryHistory { HistoryId=4, EmployeeId=5, OldSalary=60000m, NewSalary=65000m, ChangeDate=new DateTime(2022,9,1) },
        new SalaryHistory { HistoryId=5, EmployeeId=8, OldSalary=58000m, NewSalary=62000m, ChangeDate=new DateTime(2023,1,15) },
        new SalaryHistory { HistoryId=6, EmployeeId=12, OldSalary=75000m, NewSalary=80000m, ChangeDate=new DateTime(2022,5,1) },
        new SalaryHistory { HistoryId=7, EmployeeId=14, OldSalary=70000m, NewSalary=78000m, ChangeDate=new DateTime(2021,6,15) },
        new SalaryHistory { HistoryId=8, EmployeeId=14, OldSalary=78000m, NewSalary=83000m, ChangeDate=new DateTime(2023,3,1) },
        new SalaryHistory { HistoryId=9, EmployeeId=13, OldSalary=80000m, NewSalary=77000m, ChangeDate=new DateTime(2022,11,1) },
        new SalaryHistory { HistoryId=10, EmployeeId=16, OldSalary=90000m, NewSalary=85000m, ChangeDate=new DateTime(2023,6,15) }
    };
}