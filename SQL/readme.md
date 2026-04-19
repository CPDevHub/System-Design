# SQL — Structured Query Language
### A Complete Reference from Basics to Advanced Patterns

---

## Table of Contents

1. [SQL Command Categories](#1-sql-command-categories)
2. [DDL — Data Definition Language](#2-ddl--data-definition-language)
3. [DML — Data Manipulation Language](#3-dml--data-manipulation-language)
4. [DQL — Data Query Language](#4-dql--data-query-language)
5. [TCL — Transaction Control Language](#5-tcl--transaction-control-language)
6. [DCL — Data Control Language](#6-dcl--data-control-language)
7. [Constraints](#7-constraints)
8. [Referential Integrity](#8-referential-integrity)
9. [SQL Execution Order](#9-sql-execution-order)
10. [GROUP BY, HAVING, ORDER BY](#10-group-by-having-order-by)
11. [Joins](#11-joins)
12. [Subqueries](#12-subqueries)
13. [Useful Functions — COALESCE, ISNULL, STRING_AGG](#13-useful-functions)

---

## 1. SQL Command Categories

```
SQL Commands
│
├── DDL  (Data Definition Language)   → structure of tables
│     CREATE, ALTER, DROP, TRUNCATE, RENAME
│
├── DML  (Data Manipulation Language) → data inside tables
│     INSERT, UPDATE, DELETE
│
├── DQL  (Data Query Language)        → reading data
│     SELECT
│
├── TCL  (Transaction Control Language) → grouping operations safely
│     COMMIT, ROLLBACK, SAVEPOINT
│
└── DCL  (Data Control Language)      → permissions
      GRANT, REVOKE
```

---

## 2. DDL — Data Definition Language

DDL commands define and manage the **structure** of database objects. They auto-commit — you cannot roll them back.

### CREATE TABLE

```sql
CREATE TABLE TableName (
    ColumnName   DataType       [Constraints],
    ColumnName   DataType       [Constraints],
    ...
    [Table-level constraints]
);
```

**Example:**

```sql
CREATE TABLE Employees (
    EmployeeId   INT             PRIMARY KEY,
    FirstName    VARCHAR(50)     NOT NULL,
    LastName     VARCHAR(50)     NOT NULL,
    Email        VARCHAR(150)    NOT NULL UNIQUE,
    HireDate     DATE            NOT NULL,
    Salary       DECIMAL(10,2)   NOT NULL DEFAULT 0,
    DepartmentId INT             NULL,
    ManagerId    INT             NULL,

    CONSTRAINT FK_Emp_Dept    FOREIGN KEY (DepartmentId) REFERENCES Departments(DepartmentId),
    CONSTRAINT FK_Emp_Manager FOREIGN KEY (ManagerId)    REFERENCES Employees(EmployeeId)
);
```

### ALTER TABLE

```sql
-- Add a column
ALTER TABLE TableName ADD ColumnName DataType [Constraints];

-- Drop a column
ALTER TABLE TableName DROP COLUMN ColumnName;

-- Modify a column's data type
ALTER TABLE TableName ALTER COLUMN ColumnName NewDataType;   -- SQL Server
ALTER TABLE TableName MODIFY COLUMN ColumnName NewDataType; -- MySQL

-- Add a constraint after creation
ALTER TABLE TableName ADD CONSTRAINT ConstraintName FOREIGN KEY (Col) REFERENCES Other(Col);

-- Drop a constraint
ALTER TABLE TableName DROP CONSTRAINT ConstraintName;
```

### DROP vs TRUNCATE

```sql
-- DROP: removes the table entirely (structure + data)
DROP TABLE TableName;

-- TRUNCATE: removes all rows but keeps the structure
-- Faster than DELETE (no row-by-row logging), resets identity columns
TRUNCATE TABLE TableName;
```

| | `DELETE` | `TRUNCATE` | `DROP` |
|---|---|---|---|
| Removes rows | Yes | Yes | Yes |
| Removes structure | No | No | Yes |
| Can use WHERE | Yes | No | No |
| Rollback possible | Yes | No (in most DBs) | No |
| Resets identity | No | Yes | — |

---

## 3. DML — Data Manipulation Language

DML commands modify **data inside tables**. They can be wrapped in transactions and rolled back.

### INSERT

```sql
-- Single row
INSERT INTO TableName (Col1, Col2, Col3)
VALUES (Val1, Val2, Val3);

-- Multiple rows
INSERT INTO TableName (Col1, Col2)
VALUES
    (Val1a, Val2a),
    (Val1b, Val2b),
    (Val1c, Val2c);

-- Insert from a SELECT
INSERT INTO ArchiveOrders (Id, CustomerId, OrderDate)
SELECT Id, CustomerId, OrderDate
FROM Orders
WHERE OrderDate < '2023-01-01';
```

### UPDATE

```sql
-- General syntax
UPDATE TableName
SET    Col1 = NewVal1,
       Col2 = NewVal2
WHERE  <condition>;         -- ← ALWAYS include WHERE or you update every row!

-- Example
UPDATE Employees
SET    Salary = Salary * 1.10,       -- 10% raise
       UpdatedAt = GETDATE()
WHERE  DepartmentId = 1
  AND  HireDate < '2020-01-01';
```

### DELETE

```sql
-- General syntax
DELETE FROM TableName
WHERE <condition>;           -- ← ALWAYS include WHERE or you delete every row!

-- Example
DELETE FROM Orders
WHERE OrderDate < '2023-01-01'
  AND TotalAmount < 100;
```

> **Safety habit:** Before running UPDATE or DELETE, always run the equivalent SELECT first to see exactly which rows will be affected.

---

## 4. DQL — Data Query Language

The `SELECT` statement is the most powerful and most complex command in SQL.

### General Syntax (all clauses)

```sql
SELECT   [DISTINCT] <columns or expressions>
FROM     <table>
[JOIN    <other table> ON <condition>]
[WHERE   <row filter condition>]
[GROUP BY <column(s)>]
[HAVING  <group filter condition>]
[ORDER BY <column(s)> [ASC|DESC]]
[OFFSET  n ROWS FETCH NEXT m ROWS ONLY];   -- pagination
```

### SELECT Expressions

```sql
-- All columns
SELECT * FROM Employees;

-- Specific columns with alias
SELECT FirstName, LastName, Salary AS MonthlyPay FROM Employees;

-- Computed column
SELECT FirstName + ' ' + LastName AS FullName, Salary * 12 AS AnnualSalary FROM Employees;

-- Distinct values only
SELECT DISTINCT City FROM Customers;

-- Constant / literal
SELECT 'Active' AS Status, EmployeeId FROM Employees;
```

---

## 5. TCL — Transaction Control Language

A **transaction** is a group of operations that must all succeed or all fail together. This guarantees data integrity.

### The ACID Properties

```
A — Atomicity    → All operations succeed or all are rolled back
C — Consistency  → Database moves from one valid state to another
I — Isolation    → Transactions don't interfere with each other
D — Durability   → Committed changes survive crashes
```

### TCL Commands

```sql
-- BEGIN a transaction (SQL Server uses BEGIN TRAN)
BEGIN TRANSACTION;

-- Confirm all changes permanently
COMMIT;

-- Undo all changes back to the start of the transaction
ROLLBACK;

-- Create a named restore point inside a transaction
SAVEPOINT SavepointName;

-- Roll back only to a savepoint (not the entire transaction)
ROLLBACK TO SavepointName;
```

### Practical Example

```sql
BEGIN TRANSACTION;

    UPDATE Accounts SET Balance = Balance - 5000 WHERE AccountId = 1;  -- debit
    UPDATE Accounts SET Balance = Balance + 5000 WHERE AccountId = 2;  -- credit

    -- Check if both succeeded before committing
    IF @@ERROR = 0
        COMMIT;       -- both worked — make permanent
    ELSE
        ROLLBACK;     -- something failed — undo everything
```

---

## 6. DCL — Data Control Language

DCL manages **who can do what** in the database.

```sql
-- Grant permission
GRANT SELECT, INSERT ON TableName TO UserName;
GRANT ALL PRIVILEGES ON DATABASE MyDb TO AdminUser;

-- Revoke permission
REVOKE INSERT ON TableName FROM UserName;

-- Grant with ability to pass the permission along
GRANT SELECT ON TableName TO UserName WITH GRANT OPTION;
```

| Permission | Allows |
|---|---|
| `SELECT` | Read data |
| `INSERT` | Add rows |
| `UPDATE` | Modify rows |
| `DELETE` | Remove rows |
| `EXECUTE` | Run stored procedures |
| `ALL PRIVILEGES` | Everything |

---

## 7. Constraints

Constraints are rules enforced at the database level. They prevent bad data from ever entering the table.

### PRIMARY KEY

```sql
-- Column-level
CREATE TABLE Orders (
    Id INT PRIMARY KEY,    -- single column PK
    ...
);

-- Table-level (required for composite PKs)
CREATE TABLE OrderItems (
    OrderId   INT NOT NULL,
    ProductId INT NOT NULL,
    PRIMARY KEY (OrderId, ProductId)    -- composite PK
);
```

- Uniquely identifies each row
- Cannot be NULL
- Only one per table
- Creates a clustered index automatically (in SQL Server)

### FOREIGN KEY

```sql
CREATE TABLE Orders (
    Id         INT PRIMARY KEY,
    CustomerId INT,
    CONSTRAINT FK_Orders_Customers
        FOREIGN KEY (CustomerId)
        REFERENCES Customers(Id)
        ON DELETE CASCADE      -- optional: what to do when parent row is deleted
        ON UPDATE CASCADE      -- optional: what to do when parent key changes
);
```

**Referential Actions:**

| Action | Meaning |
|---|---|
| `CASCADE` | Delete/update child rows automatically |
| `SET NULL` | Set FK column to NULL in child rows |
| `SET DEFAULT` | Set FK column to its default value |
| `RESTRICT` / `NO ACTION` | Prevent the parent delete/update if children exist |

### UNIQUE

```sql
-- Column-level
Email VARCHAR(150) UNIQUE

-- Table-level (named — easier to drop later)
CONSTRAINT UQ_Employees_Email UNIQUE (Email)

-- Composite unique (combination must be unique)
CONSTRAINT UQ_Emp_Phone UNIQUE (CountryCode, PhoneNumber)
```

Unlike PRIMARY KEY: allows ONE NULL value, and a table can have multiple UNIQUE constraints.

### NOT NULL

```sql
FirstName VARCHAR(50) NOT NULL    -- column-level, can't be omitted
```

### CHECK

```sql
-- Column-level
Age INT CHECK (Age >= 0 AND Age <= 120)

-- Table-level with name
CONSTRAINT CHK_Salary CHECK (Salary > 0)

-- Multiple conditions
CONSTRAINT CHK_Dates CHECK (EndDate IS NULL OR EndDate > StartDate)
```

### DEFAULT

```sql
Status      VARCHAR(20)  DEFAULT 'Active',
CreatedAt   DATETIME     DEFAULT GETDATE(),    -- SQL Server
CreatedAt   DATETIME     DEFAULT NOW()         -- MySQL
```

### Constraint Summary Table

| Constraint | Allows NULL | Allows Duplicate | Per Table |
|---|---|---|---|
| `PRIMARY KEY` | No | No | One only |
| `UNIQUE` | Yes (one NULL) | No | Many |
| `NOT NULL` | No | Yes | Many |
| `FOREIGN KEY` | Yes (nullable FK) | Yes | Many |
| `CHECK` | Yes | Yes | Many |
| `DEFAULT` | — | — | Per column |

---

## 8. Referential Integrity

Referential integrity means the **relationships between tables are always valid**. A foreign key value must either be NULL or match an existing primary key in the referenced table.

```
Customers table            Orders table
─────────────────          ─────────────────────────────
Id | Name                  Id | CustomerId | TotalAmount
───┼────────               ───┼───────────┼────────────
1  | Alice                 1  | 1          | 52000      ← valid (Alice exists)
2  | Bob                   2  | 1          | 20000      ← valid
3  | Charlie               3  | 999        | 15000      ← VIOLATION (999 doesn't exist)
```

### What Breaks Referential Integrity?

```sql
-- 1. Inserting a child row with a non-existent parent key
INSERT INTO Orders (Id, CustomerId) VALUES (10, 999);
-- Error: FK constraint violated — no customer with Id=999

-- 2. Deleting a parent row that has children (without CASCADE)
DELETE FROM Customers WHERE Id = 1;
-- Error: FK constraint violated — orders reference customer 1

-- 3. Updating a parent's PK that children reference (without CASCADE)
UPDATE Customers SET Id = 100 WHERE Id = 1;
-- Error: FK constraint violated
```

### ON DELETE / ON UPDATE Behavior

```sql
-- Example: what happens to Orders when a Customer is deleted?

ON DELETE CASCADE     -- Orders are auto-deleted too
ON DELETE SET NULL    -- Orders.CustomerId becomes NULL
ON DELETE RESTRICT    -- The customer delete is blocked (default)
ON DELETE NO ACTION   -- Same as RESTRICT (checked at end of statement)
```

---

## 9. SQL Execution Order

This is one of the most important concepts to understand. The order you **write** clauses is different from the order SQL **processes** them.

### Written Order vs Processing Order

```sql
-- Written order (what you type):
SELECT   ...          -- 6
FROM     ...          -- 1
JOIN     ...          -- 2
WHERE    ...          -- 3
GROUP BY ...          -- 4
HAVING   ...          -- 5
ORDER BY ...          -- 7
LIMIT    ...          -- 8
```

```sql
-- Processing order (what SQL actually does):
FROM     → determines the base dataset (all rows from the table)
JOIN     → combines with other tables
WHERE    → filters individual rows (before grouping)
GROUP BY → collapses rows into groups
HAVING   → filters groups (after grouping)
SELECT   → picks which columns/expressions to show
ORDER BY → sorts the final result
LIMIT    → truncates to n rows
```

### Why Processing Order Matters

```sql
-- This FAILS — WHERE runs before SELECT, so the alias doesn't exist yet
SELECT Salary * 12 AS AnnualSalary
FROM Employees
WHERE AnnualSalary > 600000;      -- Error: "AnnualSalary" is unknown here

-- Fix 1: repeat the expression
WHERE Salary * 12 > 600000;

-- Fix 2: use a subquery or CTE
WITH Calc AS (
    SELECT EmployeeId, Salary * 12 AS AnnualSalary FROM Employees
)
SELECT * FROM Calc WHERE AnnualSalary > 600000;
```

```sql
-- This FAILS — HAVING can only see columns in GROUP BY or aggregates
SELECT DepartmentId, COUNT(*) AS Cnt
FROM Employees
GROUP BY DepartmentId
HAVING FirstName = 'Alice';    -- Error: FirstName is not in GROUP BY or aggregate

-- HAVING is for group-level conditions only
HAVING COUNT(*) > 5           -- ✅ correct use of HAVING
```

### Execution Order Diagram

```
┌─────────────────────────────────────────────────────┐
│ Step 1: FROM + JOIN                                 │
│   → Load and combine all source rows                │
│   → Result: a big virtual table                     │
├─────────────────────────────────────────────────────┤
│ Step 2: WHERE                                       │
│   → Filter rows one by one                         │
│   → Aggregates (SUM, COUNT) NOT allowed here        │
├─────────────────────────────────────────────────────┤
│ Step 3: GROUP BY                                    │
│   → Collapse matching rows into groups              │
│   → Each group becomes one row                      │
├─────────────────────────────────────────────────────┤
│ Step 4: HAVING                                      │
│   → Filter groups (uses aggregate results)          │
│   → WHERE on groups                                 │
├─────────────────────────────────────────────────────┤
│ Step 5: SELECT                                      │
│   → Pick columns, compute expressions, apply aliases│
├─────────────────────────────────────────────────────┤
│ Step 6: DISTINCT                                    │
│   → Remove duplicate rows from SELECT output        │
├─────────────────────────────────────────────────────┤
│ Step 7: ORDER BY                                    │
│   → Sort the result (aliases from SELECT usable here│
├─────────────────────────────────────────────────────┤
│ Step 8: LIMIT / TOP / FETCH                         │
│   → Trim to n rows                                  │
└─────────────────────────────────────────────────────┘
```

---

## 10. GROUP BY, HAVING, ORDER BY

### GROUP BY

Collapses many rows that share the same value(s) into a single summary row. Every column in SELECT that is **not** an aggregate must appear in GROUP BY.

```sql
-- General syntax
SELECT   <group columns>, <aggregate functions>
FROM     <table>
[WHERE   <row filter>]
GROUP BY <same group columns>
[HAVING  <group filter>];

-- Example: average salary per department
SELECT   DepartmentId,
         COUNT(*)          AS EmployeeCount,
         AVG(Salary)       AS AvgSalary,
         MAX(Salary)       AS MaxSalary,
         MIN(Salary)       AS MinSalary,
         SUM(Salary)       AS TotalSalary
FROM     Employees
WHERE    IsActive = 1              -- filter rows BEFORE grouping
GROUP BY DepartmentId
HAVING   COUNT(*) >= 3;           -- filter groups AFTER grouping
```

### The Rule: What Goes in SELECT Must Go in GROUP BY

```sql
-- WRONG — City is not grouped or aggregated
SELECT DepartmentId, City, COUNT(*)
FROM Employees
GROUP BY DepartmentId;
-- Error: City must appear in GROUP BY or be wrapped in an aggregate

-- CORRECT
SELECT DepartmentId, COUNT(*)
FROM Employees
GROUP BY DepartmentId;

-- ALSO CORRECT — City is in GROUP BY
SELECT DepartmentId, City, COUNT(*)
FROM Employees
GROUP BY DepartmentId, City;
```

### GROUP BY with Multiple Columns

```sql
-- Group by two columns = unique combinations of both
SELECT DepartmentId, City, COUNT(*) AS Count
FROM Employees
GROUP BY DepartmentId, City;

-- One row per unique (DepartmentId, City) combination
```

### HAVING vs WHERE

```sql
-- WHERE — runs before grouping — filters individual rows
-- Cannot reference aggregate functions

-- HAVING — runs after grouping — filters entire groups
-- Can reference aggregates

-- Example combining both:
SELECT   DepartmentId, AVG(Salary) AS AvgSalary
FROM     Employees
WHERE    HireDate > '2020-01-01'      -- filter rows: only post-2020 hires
GROUP BY DepartmentId
HAVING   AVG(Salary) > 60000;         -- filter groups: only high-avg depts
```

### ORDER BY

```sql
-- Ascending (default)
ORDER BY LastName ASC

-- Descending
ORDER BY Salary DESC

-- Multiple columns: primary sort, then secondary
ORDER BY DepartmentId ASC, Salary DESC

-- By column position (not recommended — fragile if columns change)
ORDER BY 2 DESC    -- sorts by the 2nd column in SELECT

-- Using SELECT alias (allowed because ORDER BY runs after SELECT)
SELECT Salary * 12 AS AnnualSalary
FROM Employees
ORDER BY AnnualSalary DESC;    -- ✅ alias works here
```

---

## 11. Joins

A JOIN combines rows from two or more tables based on a related column.

### Venn Diagram Reference

```
Table A (Employees)          Table B (Departments)
   ┌──────────────┐              ┌──────────────┐
   │  Alice  10   │              │  10 Eng      │
   │  Bob    20   │◄────────────►│  20 Mkt      │
   │  Carol  99   │              │  30 HR       │
   └──────────────┘              └──────────────┘

INNER JOIN   = matched middle only
LEFT JOIN    = all of A + matched from B
RIGHT JOIN   = all of B + matched from A
FULL JOIN    = everything from both
CROSS JOIN   = every A × every B
```

### INNER JOIN

Returns only rows where the join condition matches in **both** tables.

```sql
-- General syntax
SELECT <columns>
FROM   TableA A
INNER JOIN TableB B ON A.Key = B.Key;

-- Example
SELECT c.Name, o.OrderDate, o.TotalAmount
FROM   Customers c
INNER JOIN Orders o ON c.Id = o.CustomerId;
-- Customers with NO orders are excluded
-- Orders with NULL CustomerId are excluded
```

### LEFT OUTER JOIN

Returns all rows from the left table. Unmatched right-side values = NULL.

```sql
SELECT c.Name, o.TotalAmount
FROM   Customers c
LEFT JOIN Orders o ON c.Id = o.CustomerId;
-- All customers returned, even those with no orders
-- o.TotalAmount = NULL for customers with no orders

-- Common pattern: find rows with NO match
SELECT c.Name
FROM   Customers c
LEFT JOIN Orders o ON c.Id = o.CustomerId
WHERE  o.Id IS NULL;    -- only customers with zero orders
```

### RIGHT OUTER JOIN

Returns all rows from the right table. Unmatched left-side values = NULL.

```sql
SELECT c.Name, o.TotalAmount
FROM   Customers c
RIGHT JOIN Orders o ON c.Id = o.CustomerId;
-- All orders returned, even the one with NULL CustomerId
-- c.Name = NULL for that orphaned order
```

> **Tip:** Right joins are rare. You can always rewrite a RIGHT JOIN as a LEFT JOIN by swapping the table order. Most teams prefer LEFT JOIN by convention.

### FULL OUTER JOIN

Returns everything from both tables. Unmatched sides = NULL.

```sql
SELECT c.Name, o.TotalAmount
FROM   Customers c
FULL OUTER JOIN Orders o ON c.Id = o.CustomerId;
-- Customers with no orders: o columns are NULL
-- Orders with no customer: c columns are NULL
```

### CROSS JOIN

Every row from A paired with every row from B. No condition. Result = m × n rows.

```sql
SELECT c.Name, p.Name AS ProductName
FROM   Customers c
CROSS JOIN Products p;
-- 6 customers × 5 products = 30 rows
-- Useful for generating combinations, test data
```

### SELF JOIN

A table joined to itself. Useful for hierarchical data (managers, categories).

```sql
-- Find each employee's manager name
SELECT e.FirstName   AS Employee,
       m.FirstName   AS Manager
FROM   Employees e
LEFT JOIN Employees m ON e.ManagerId = m.EmployeeId;
-- LEFT JOIN ensures employees with no manager still appear
```

### JOIN on Multiple Conditions

```sql
-- Join on more than one column
SELECT *
FROM   Orders o
JOIN   OrderItems oi ON o.Id = oi.OrderId
                    AND o.Year = oi.Year;    -- additional condition

-- Join with inequality (non-equijoin)
SELECT e.Name, s.Grade
FROM   Employees e
JOIN   SalaryGrades s ON e.Salary BETWEEN s.MinSalary AND s.MaxSalary;
```

### Three-Table Join

```sql
SELECT c.Name       AS Customer,
       p.Name       AS Product,
       oi.Quantity
FROM   Customers c
JOIN   Orders o     ON c.Id = o.CustomerId
JOIN   OrderItems oi ON o.Id = oi.OrderId
JOIN   Products p   ON oi.ProductId = p.Id;
```

---

## 12. Subqueries

A subquery is a SELECT statement nested inside another SQL statement. It is evaluated first, and its result is used by the outer query.

### Where You Can Write a Subquery

```sql
-- 1. In WHERE clause
SELECT Name FROM Products WHERE Price = (SELECT MAX(Price) FROM Products);

-- 2. In FROM clause (derived table — must have alias)
SELECT DeptStats.DeptId, DeptStats.AvgSalary
FROM (
    SELECT DepartmentId AS DeptId, AVG(Salary) AS AvgSalary
    FROM Employees
    GROUP BY DepartmentId
) AS DeptStats
WHERE DeptStats.AvgSalary > 60000;

-- 3. In SELECT clause (scalar subquery — must return exactly 1 row, 1 col)
SELECT Name,
       Salary,
       (SELECT AVG(Salary) FROM Employees) AS CompanyAvg
FROM   Employees;

-- 4. In HAVING clause
SELECT DepartmentId, AVG(Salary) AS AvgSalary
FROM   Employees
GROUP BY DepartmentId
HAVING AVG(Salary) > (SELECT AVG(Salary) FROM Employees);
```

### Subquery Types

#### Scalar Subquery — Returns exactly one value

```sql
-- Rule: must return exactly 1 row AND 1 column
SELECT Name, Salary
FROM   Employees
WHERE  Salary = (SELECT MAX(Salary) FROM Employees);
--              ──────────────────────────────────
--              Returns a single number — safe to use with =
```

#### Row Subquery — Returns one row, multiple columns

```sql
SELECT Name
FROM   Employees
WHERE  (DepartmentId, Salary) = (SELECT DepartmentId, MAX(Salary)
                                  FROM Employees
                                  WHERE DepartmentId = 1);
```

#### Column Subquery — Returns one column, multiple rows (used with IN, ANY, ALL)

```sql
-- IN: employee is in one of these departments
SELECT Name FROM Employees
WHERE DepartmentId IN (SELECT Id FROM Departments WHERE Location = 'New York');

-- NOT IN: watch out for NULLs!
-- If the subquery returns even one NULL, NOT IN returns no rows
-- Use NOT EXISTS instead when NULLs are possible
SELECT Name FROM Customers
WHERE Id NOT IN (SELECT CustomerId FROM Orders WHERE CustomerId IS NOT NULL);

-- ANY: at least one comparison must be true
SELECT Name FROM Employees
WHERE Salary > ANY (SELECT Salary FROM Employees WHERE DepartmentId = 2);
-- means: salary > the minimum salary in dept 2

-- ALL: every comparison must be true
SELECT Name FROM Employees
WHERE Salary > ALL (SELECT Salary FROM Employees WHERE DepartmentId = 2);
-- means: salary > the maximum salary in dept 2
```

#### Correlated Subquery — References the outer query

Runs once **per row** of the outer query. Slower but very expressive.

```sql
-- General pattern:
SELECT col1, col2
FROM   OuterTable ot
WHERE  EXISTS (
    SELECT 1
    FROM   InnerTable it
    WHERE  it.ForeignKey = ot.PrimaryKey    -- ← references outer table
);

-- Example: customers who have at least one order
SELECT Name FROM Customers c
WHERE EXISTS (
    SELECT 1 FROM Orders o WHERE o.CustomerId = c.Id
);

-- Example: customers with NO orders
SELECT Name FROM Customers c
WHERE NOT EXISTS (
    SELECT 1 FROM Orders o WHERE o.CustomerId = c.Id
);
```

#### Derived Table — Subquery in FROM clause

```sql
-- General pattern:
SELECT outer_cols
FROM (
    SELECT inner_cols
    FROM   SomeTable
    WHERE  ...
    GROUP BY ...
) AS AliasName               -- ← alias is REQUIRED
WHERE outer_cols condition;

-- Example: departments where the average salary is above 70000
SELECT d.Name, sub.AvgSalary
FROM   Departments d
JOIN (
    SELECT DepartmentId, AVG(Salary) AS AvgSalary
    FROM   Employees
    GROUP BY DepartmentId
) AS sub ON d.Id = sub.DepartmentId
WHERE sub.AvgSalary > 70000;
```

### Common Table Expression (CTE) — Cleaner Alternative to Derived Tables

```sql
-- General pattern:
WITH CTEName AS (
    SELECT ...
    FROM   ...
    WHERE  ...
),
SecondCTE AS (
    SELECT ... FROM CTEName ...   -- CTEs can reference earlier CTEs
)
SELECT * FROM SecondCTE;

-- Example: same as derived table above, but more readable
WITH DeptAverages AS (
    SELECT DepartmentId, AVG(Salary) AS AvgSalary
    FROM   Employees
    GROUP BY DepartmentId
)
SELECT d.Name, da.AvgSalary
FROM   Departments d
JOIN   DeptAverages da ON d.Id = da.DepartmentId
WHERE  da.AvgSalary > 70000;
```

### Subquery Rules and Gotchas

```sql
-- Rule 1: Scalar subquery must return exactly 1 row × 1 column
-- This FAILS if more than one product has the max price:
SELECT Name FROM Products WHERE Price = (SELECT MAX(Price) FROM Products);
-- Actually this is SAFE because MAX() always returns one row.
-- UNSAFE example:
WHERE Price = (SELECT Price FROM Products WHERE Name = 'Laptop' OR Name = 'Phone');
-- ↑ returns two rows — ERROR with =, use IN instead

-- Rule 2: NOT IN with NULLs in subquery returns zero rows
SELECT * FROM A WHERE Id NOT IN (SELECT ForeignId FROM B);
-- If B has even ONE NULL ForeignId, this returns nothing.
-- Always filter NULLs: NOT IN (SELECT ForeignId FROM B WHERE ForeignId IS NOT NULL)
-- Or safer: use NOT EXISTS instead

-- Rule 3: Correlated subqueries run once per row — can be slow on large tables
-- If performance matters, rewrite as a JOIN or use CTEs

-- Rule 4: Alias required for derived tables
FROM (SELECT ...) AS MustHaveAlias   -- ← alias is mandatory
```

---

## 13. Useful Functions

### ISNULL (SQL Server)

```sql
-- ISNULL(expression, replacement_value)
-- If expression is NULL, return replacement_value
-- Both arguments must be the same type

SELECT ISNULL(City, 'Unknown') AS City FROM Customers;
-- If City is NULL → 'Unknown', otherwise → actual City

SELECT ISNULL(TotalAmount, 0) FROM Orders;
-- Replaces NULL amounts with 0
```

**Limitation:** `ISNULL` only takes **two** arguments. For more flexibility, use `COALESCE`.

---

### COALESCE

```sql
-- COALESCE(val1, val2, val3, ..., valN)
-- Returns the FIRST non-NULL value from the list
-- Standard SQL — works in all databases

-- Basic usage
SELECT COALESCE(PhoneNumber, MobileNumber, Email, 'No Contact') AS ContactInfo
FROM   Customers;
-- Tries PhoneNumber first, then MobileNumber, then Email, then literal

-- Replacing NULL in joins
SELECT c.Name, COALESCE(SUM(o.TotalAmount), 0) AS TotalSpent
FROM   Customers c
LEFT JOIN Orders o ON c.Id = o.CustomerId
GROUP BY c.Id, c.Name;
-- SUM returns NULL when no orders exist; COALESCE converts it to 0

-- Prefer COALESCE over ISNULL when:
-- 1. You need more than two options
-- 2. You want standard SQL that works in all databases
-- 3. You need consistent type handling
```

| | `ISNULL` | `COALESCE` |
|---|---|---|
| Arguments | 2 only | 2 or more |
| Standard SQL | No (SQL Server specific) | Yes (ANSI) |
| Returns type | Type of first arg | Type of highest precedence |
| Works in MySQL/Postgres | No | Yes |

---

### STRING_AGG

Concatenates values from multiple rows into a single string, with a separator.

```sql
-- General syntax (SQL Server 2017+, PostgreSQL, MySQL 8+)
STRING_AGG(expression, separator)
    WITHIN GROUP (ORDER BY sort_expression)    -- optional ordering

-- Example: all product names per order as one string
SELECT   o.Id AS OrderId,
         STRING_AGG(p.Name, ', ') AS Products
FROM     Orders o
JOIN     OrderItems oi ON o.Id = oi.OrderId
JOIN     Products p    ON oi.ProductId = p.Id
GROUP BY o.Id;

-- Result:
-- OrderId | Products
-- 1       | Laptop, Headphones
-- 2       | Phone
-- 5       | Laptop, Phone

-- With ordering inside the aggregation
SELECT   DepartmentId,
         STRING_AGG(FirstName, ', ') WITHIN GROUP (ORDER BY FirstName) AS Members
FROM     Employees
GROUP BY DepartmentId;

-- With DISTINCT (remove duplicate values before aggregating)
SELECT STRING_AGG(DISTINCT City, ', ')
FROM   Customers;
-- 'Delhi, Mumbai, Pune'  (no duplicates)
```

---

### Other Practical Functions

```sql
-- ── String Functions ──────────────────────────────────────────
LEN('Hello')                    -- 5            (SQL Server)
LENGTH('Hello')                 -- 5            (MySQL, PostgreSQL)
UPPER('hello')                  -- 'HELLO'
LOWER('HELLO')                  -- 'hello'
TRIM('  hello  ')               -- 'hello'
LTRIM('  hello')                -- 'hello'
RTRIM('hello  ')                -- 'hello'
SUBSTRING('Hello World', 1, 5) -- 'Hello'
CHARINDEX('o', 'Hello World')  -- 5 (position of first 'o')
REPLACE('Hello', 'l', 'r')    -- 'Herro'
CONCAT(FirstName, ' ', LastName)-- 'Alice Johnson'

-- ── Numeric Functions ─────────────────────────────────────────
ROUND(12.456, 2)                -- 12.46
CEILING(12.1)                   -- 13  (round up)
FLOOR(12.9)                     -- 12  (round down)
ABS(-15)                        -- 15
POWER(2, 10)                    -- 1024

-- ── Date Functions ────────────────────────────────────────────
GETDATE()                       -- current datetime (SQL Server)
NOW()                           -- current datetime (MySQL)
CURRENT_TIMESTAMP               -- current datetime (standard)
YEAR(HireDate)                  -- extracts year
MONTH(HireDate)                 -- extracts month
DAY(HireDate)                   -- extracts day
DATEDIFF(DAY, HireDate, GETDATE())     -- days between dates
DATEADD(MONTH, 3, GETDATE())   -- 3 months from now

-- ── Conditional ───────────────────────────────────────────────
CASE
    WHEN Salary > 100000 THEN 'Senior'
    WHEN Salary > 60000  THEN 'Mid-Level'
    ELSE                      'Junior'
END AS SalaryBand

-- Simple CASE
CASE Status
    WHEN 'A' THEN 'Active'
    WHEN 'I' THEN 'Inactive'
    ELSE          'Unknown'
END
```

---

### Window Functions (Bonus — Powerful Ranking)

```sql
-- General syntax
FunctionName() OVER (
    [PARTITION BY column]    -- reset the function per group
    [ORDER BY column]        -- define order for ranking/running totals
)

-- ROW_NUMBER: unique sequential number (no ties)
SELECT Name, Salary,
       ROW_NUMBER() OVER (PARTITION BY DepartmentId ORDER BY Salary DESC) AS RowNum
FROM Employees;

-- RANK: same rank for ties, skips the next number
SELECT Name, Salary,
       RANK() OVER (ORDER BY Salary DESC) AS Rank
FROM Employees;
-- Salaries: 95k=1, 90k=2, 90k=2, 85k=4  (3 is skipped)

-- DENSE_RANK: same rank for ties, does NOT skip
SELECT Name, Salary,
       DENSE_RANK() OVER (ORDER BY Salary DESC) AS DenseRank
FROM Employees;
-- Salaries: 95k=1, 90k=2, 90k=2, 85k=3  (no skip)

-- Running total
SELECT Name, Salary,
       SUM(Salary) OVER (ORDER BY HireDate) AS RunningTotal
FROM Employees;

-- Get top 1 per department (using CTE)
WITH Ranked AS (
    SELECT Name, DepartmentId, Salary,
           RANK() OVER (PARTITION BY DepartmentId ORDER BY Salary DESC) AS rnk
    FROM Employees
)
SELECT Name, DepartmentId, Salary
FROM Ranked
WHERE rnk = 1;
```

---

### Quick-Reference: WHERE vs HAVING vs ON

| Clause | When it runs | What it filters | Can use aggregates? |
|---|---|---|---|
| `ON` | During JOIN | Rows before joining | No |
| `WHERE` | After JOIN, before GROUP BY | Individual rows | No |
| `HAVING` | After GROUP BY | Entire groups | Yes |

```sql
-- All three together:
SELECT d.Name, COUNT(e.Id) AS EmpCount, AVG(e.Salary) AS AvgSalary
FROM   Departments d
JOIN   Employees e  ON d.Id = e.DepartmentId   -- ON: join condition
                   AND e.IsActive = 1           -- ON: filter during join
WHERE  d.Location = 'New York'                 -- WHERE: row filter
GROUP BY d.Name
HAVING COUNT(e.Id) >= 3                        -- HAVING: group filter
ORDER BY AvgSalary DESC;
```

---

*End of SQL Reference — push questions to `/questions/sql/` in your repo.*