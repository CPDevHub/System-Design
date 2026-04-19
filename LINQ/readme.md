# LINQ — Language Integrated Query
### A Complete Reference from Internals to Practice

---

## Table of Contents

1. [What is LINQ?](#1-what-is-linq)
2. [Method Syntax — General Structure](#2-method-syntax--general-structure)
3. [IEnumerable vs IQueryable — Deep Dive](#3-ienumerable-vs-iqueryable--deep-dive)
4. [Deferred vs Immediate Execution — Internal Workings](#4-deferred-vs-immediate-execution--internal-workings)
5. [All LINQ Operators — Categorized](#5-all-linq-operators--categorized)
6. [LINQ Joins — All Types with Diagrams](#6-linq-joins--all-types-with-diagrams)

---

## 1. What is LINQ?

LINQ stands for **Language Integrated Query**. It lets you query any data source — in-memory collections, databases, XML — using the same C# syntax.

```
┌─────────────────────────────────────────────────────┐
│                  Your C# Code                       │
│         (Query Syntax or Method Syntax)             │
└──────────────────────┬──────────────────────────────┘
                       │
          ┌────────────▼────────────┐
          │      LINQ Provider      │
          │  (translates the query) │
          └────────────┬────────────┘
                       │
        ┌──────────────┼──────────────┐
        ▼              ▼              ▼
  In-Memory       SQL Server        XML
  (Enumerable)   (EF / L2SQL)    (XDocument)
```

---

## 2. Method Syntax — General Structure

Method Syntax is the most widely used style in real codebases. Every operation is a chained method call using **lambda expressions**.

### General Syntax Pattern

```csharp
dataSource
    .Where(x => <filter condition>)
    .OrderBy(x => <sort key>)
    .Select(x => <projection>)
    .TerminalOperator();          // e.g. ToList(), First(), Count()
```

### Anatomy of a Lambda

```
x  =>  x.Salary > 50000
│       │
│       └── Body (the expression to evaluate)
│
└── Parameter (represents each element in the collection)
```

### Complete Example with Every Clause

```csharp
var result = employees
    .Where(e => e.IsActive)                          // Filter
    .OrderBy(e => e.LastName)                        // Sort ascending
    .ThenByDescending(e => e.Salary)                 // Then sort descending
    .GroupBy(e => e.DepartmentId)                    // Group
    .Select(g => new                                 // Project / Shape output
    {
        Department = g.Key,
        Count      = g.Count(),
        MaxSalary  = g.Max(e => e.Salary),
        Names      = g.Select(e => e.FirstName).ToList()
    })
    .ToList();                                       // Execute immediately
```

### Method Syntax vs Query Syntax — Full Mapping

| Query Syntax                              | Method Syntax                                          |
|-------------------------------------------|--------------------------------------------------------|
| `from x in source`                        | `source` (starting point)                              |
| `where x.Age > 18`                        | `.Where(x => x.Age > 18)`                              |
| `select x.Name`                           | `.Select(x => x.Name)`                                 |
| `select new { x.A, x.B }`                | `.Select(x => new { x.A, x.B })`                       |
| `orderby x.Age`                           | `.OrderBy(x => x.Age)`                                 |
| `orderby x.Age descending`               | `.OrderByDescending(x => x.Age)`                       |
| `orderby x.Age, x.Name`                  | `.OrderBy(x => x.Age).ThenBy(x => x.Name)`            |
| `group x by x.City`                      | `.GroupBy(x => x.City)`                                |
| `join y in src2 on x.K equals y.K`       | `.Join(src2, x=>x.K, y=>y.K, (x,y)=>...)`            |
| `join y in src2 on x.K equals y.K into g`| `.GroupJoin(src2, x=>x.K, y=>y.K, (x,g)=>...)`       |
| `let temp = x.A + x.B`                   | `.Select(x => new { x, temp = x.A + x.B })`           |
| `from x in src from y in x.Sub`          | `.SelectMany(x => x.Sub, (x, y) => ...)`               |

> **Rule:** Everything query syntax can do, method syntax can also do. The reverse is not true — operators like `Zip`, `Aggregate`, `Chunk` are method-only.

---

## 3. IEnumerable vs IQueryable — Deep Dive

### Interface Definitions

```csharp
// System.Collections.Generic
public interface IEnumerable<T>
{
    IEnumerator<T> GetEnumerator();   // gives you a cursor to walk the data
}

// System.Linq
public interface IQueryable<T> : IEnumerable<T>
{
    Expression   Expression  { get; }  // the full query as a data structure
    Type         ElementType { get; }  // type T being queried
    IQueryProvider Provider  { get; }  // the thing that translates + executes
}
```

`IQueryable<T>` **extends** `IEnumerable<T>`. This means every `IQueryable` is also an `IEnumerable`, but an `IEnumerable` is NOT an `IQueryable`.

---

### Structural Comparison

```
IEnumerable<T>                         IQueryable<T>
──────────────────────────────         ──────────────────────────────────────
Lives in: System.Collections           Lives in: System.Linq
Backed by: Enumerable class            Backed by: Queryable class
Lambda type: Func<T, bool>             Lambda type: Expression<Func<T, bool>>
Where logic runs: C# app memory        Where logic runs: Database server (SQL)
Query stored as: Compiled IL code      Query stored as: Expression Tree (data)
```

---

### The Lambda Compilation Fork — The Most Important Difference

When the compiler sees `e => e.Salary > 50000`, it compiles it **differently** depending on who receives it:

```csharp
// Path A — IEnumerable receiver
// Lambda becomes a real .NET method (executable code)
Func<Employee, bool> delegateA = e => e.Salary > 50000;
//  ↑ Compiled to IL. Runs directly in C# memory.

// Path B — IQueryable receiver
// Lambda becomes a data structure (NOT executable, just a description)
Expression<Func<Employee, bool>> exprB = e => e.Salary > 50000;
//  ↑ NOT compiled to IL. Stored as an object graph describing the code.
```

The `Expression<Func<T,bool>>` for `e => e.Salary > 50000` looks like this as a tree:

```
LambdaExpression
├── Parameters: [ParameterExpression "e" of type Employee]
└── Body: BinaryExpression (GreaterThan)
          ├── Left:  MemberExpression
          │           ├── Object: ParameterExpression "e"
          │           └── Member: Property "Salary"
          └── Right: ConstantExpression
                      └── Value: 50000
```

Entity Framework **walks this tree** at runtime and generates:
```sql
WHERE Salary > 50000
```

---

### Side-by-Side Execution Flow

```
IEnumerable (in-memory)                  IQueryable (database)
────────────────────────                 ─────────────────────────────────────
1. Lambda → Func<T,bool> delegate        1. Lambda → Expression<Func<T,bool>> tree
2. WhereIterator object created          2. Queryable.Where() adds node to tree
3. Nothing executes yet                  3. Nothing executes yet
4. foreach triggers GetEnumerator()      4. foreach triggers GetEnumerator()
5. MoveNext() pulls one item at a time   5. Provider.Execute(expressionTree) called
6. Func delegate runs on each item       6. Tree is walked → SQL is generated
7. Filtered in C# memory                 7. SQL sent to DB → only results returned
```

---

### The Critical Danger: Accidental IEnumerable Downgrade

```csharp
// WRONG — loads ALL employees into memory, THEN filters in C#
IEnumerable<Employee> employees = context.Employees; // ← downgraded here
var result = employees.Where(e => e.Salary > 50000).ToList();
// SQL generated: SELECT * FROM Employees  (all rows, no WHERE)

// CORRECT — filter runs on the database
IQueryable<Employee> employees = context.Employees;
var result = employees.Where(e => e.Salary > 50000).ToList();
// SQL generated: SELECT * FROM Employees WHERE Salary > 50000
```

### When to Use Which

| Situation                              | Use            |
|----------------------------------------|----------------|
| Querying a `List<T>`, array, or custom in-memory collection | `IEnumerable<T>` |
| Querying a database via Entity Framework | `IQueryable<T>` |
| Building up filters from user input (dynamic queries) | `IQueryable<T>` |
| Applying C# logic that can't be translated to SQL | `IEnumerable<T>` (after filtering in DB first) |

---

## 4. Deferred vs Immediate Execution — Internal Workings

### The Core Idea

Think of a deferred LINQ query as a **recipe card**. Writing the recipe doesn't cook the food. The cooking only happens when someone actually reads it and acts on it.

```
IEnumerable<int> query = numbers.Where(n => n % 2 == 0);
//                        ──────────────────────────────
//                        This creates a WhereIterator object.
//                        Zero numbers have been checked.
//                        This is the recipe card — not the meal.

foreach (var n in query)   // ← This is when cooking starts
{ ... }
```

---

### How Deferred Execution Works: The Iterator State Machine

When you call `.Where()`, this is what the runtime actually creates:

```csharp
// Simplified WhereIterator — what Where() returns internally
class WhereIterator<T> : IEnumerable<T>, IEnumerator<T>
{
    private IEnumerable<T> _source;     // the list
    private Func<T, bool>  _predicate;  // the rule
    private IEnumerator<T> _enumerator; // cursor into source
    private T              _current;    // currently held value
    private int            _state;      // 0=not started, 2=running, -1=done

    public bool MoveNext()
    {
        if (_state == 1) { _enumerator = _source.GetEnumerator(); _state = 2; }

        while (_enumerator.MoveNext())
        {
            var item = _enumerator.Current;
            if (_predicate(item))      // ← check the rule
            {
                _current = item;
                return true;           // ← PAUSE here. Give item to consumer.
                // Next MoveNext() call resumes from exactly this point.
            }
        }
        _state = -1;
        return false;                  // exhausted
    }

    public T Current => _current;
}
```

**The pause-and-resume mechanism:** `_state` acts as a bookmark. The object stays alive between `MoveNext()` calls, so the while loop position and enumerator position are preserved. The next call resumes exactly where it left off.

---

### Complete Dry Run: `numbers.Where(n => n % 2 == 0)` where numbers = `[1,2,3,4,5]`

```
Setup:   WhereIterator created. _state=0. Nothing touched.

──── foreach starts: calls GetEnumerator() → _state = 1 ────

MoveNext() call #1:
  Opens source list. _state → 2.
  Checks 1 → 1%2=1 → false → skip
  Checks 2 → 2%2=0 → true  → _current=2, return true ← PAUSE
  Consumer receives: Current = 2

MoveNext() call #2 (resumes where paused, source cursor at 2):
  Checks 3 → 3%2=1 → false → skip
  Checks 4 → 4%2=0 → true  → _current=4, return true ← PAUSE
  Consumer receives: Current = 4

MoveNext() call #3:
  Checks 5 → 5%2=1 → false → skip
  Source exhausted → _state = -1, return false
  foreach exits.

Total work: checked 5 numbers. No intermediate list created.
Output: 2, 4
```

---

### How `ToList()` Triggers Immediate Execution

```csharp
// ToList() source (simplified):
public static List<T> ToList<T>(this IEnumerable<T> source)
{
    return new List<T>(source);
    // List<T>(IEnumerable) constructor runs a tight while loop:
    //   var e = source.GetEnumerator();
    //   while (e.MoveNext()) { this.Add(e.Current); }
    // It drains the entire iterator before returning.
}
```

```
ToList() call:
  GetEnumerator() → robot wakes up
  MoveNext() → gets 2  → adds to list → [2]
  MoveNext() → gets 4  → adds to list → [2, 4]
  MoveNext() → false   → loop exits
  Returns: new List<int> { 2, 4 }

The iterator is done. Disposed. Garbage collected.
You now have a standalone List<int> in memory.
```

---

### Deferred Operators (Create iterator objects, do NO work)

```csharp
.Where()        .Select()       .SelectMany()
.OrderBy()      .OrderByDescending()
.ThenBy()       .GroupBy()
.Take()         .Skip()         .TakeWhile()    .SkipWhile()
.Distinct()     .Union()        .Intersect()    .Except()
.Join()         .GroupJoin()    .Zip()
```

### Immediate Operators (Execute right now, return actual data)

```csharp
.ToList()       .ToArray()      .ToDictionary()
.Count()        .Sum()          .Min()          .Max()      .Average()
.First()        .FirstOrDefault()
.Last()         .LastOrDefault()
.Single()       .SingleOrDefault()
.Any()          .All()          .Contains()
.Aggregate()
```

### The Danger of Multiple Enumeration

```csharp
// BAD — deferred query runs TWICE (two database calls if IQueryable!)
IEnumerable<Employee> highEarners = employees.Where(e => e.Salary > 50000);
int count  = highEarners.Count();           // ← full iteration #1
var names  = highEarners.Select(e => e.Name).ToList(); // ← full iteration #2

// GOOD — materialize once, reuse the list
List<Employee> highEarners = employees.Where(e => e.Salary > 50000).ToList(); // ← once
int count  = highEarners.Count;            // O(1) — property, not a query
var names  = highEarners.Select(e => e.Name).ToList(); // in-memory, instant
```

---

## 5. All LINQ Operators — Categorized

### Category 1 — Filtering

| Operator       | Description                                        | Example                                          |
|----------------|----------------------------------------------------|--------------------------------------------------|
| `Where`        | Keep elements matching a condition                 | `.Where(x => x.Age > 18)`                       |
| `OfType<T>`    | Keep only elements of a specific type              | `.OfType<string>()`                              |
| `Distinct`     | Remove duplicates                                  | `.Distinct()`                                    |
| `DistinctBy`   | Remove duplicates by a key (.NET 6+)               | `.DistinctBy(x => x.Name)`                       |

### Category 2 — Projection

| Operator       | Description                                        | Example                                          |
|----------------|----------------------------------------------------|--------------------------------------------------|
| `Select`       | Transform each element                             | `.Select(x => x.Name)`                           |
| `SelectMany`   | Flatten a collection of collections                | `.SelectMany(x => x.Orders)`                     |

```csharp
// SelectMany — before and after
// Before: [[1,2], [3,4], [5]]
// After:  [1, 2, 3, 4, 5]
var allOrders = customers.SelectMany(c => c.Orders);
```

### Category 3 — Sorting

| Operator              | Description                          | Example                                    |
|-----------------------|--------------------------------------|--------------------------------------------|
| `OrderBy`             | Sort ascending                       | `.OrderBy(x => x.Name)`                   |
| `OrderByDescending`   | Sort descending                      | `.OrderByDescending(x => x.Salary)`       |
| `ThenBy`              | Secondary sort ascending             | `.ThenBy(x => x.Age)`                     |
| `ThenByDescending`    | Secondary sort descending            | `.ThenByDescending(x => x.HireDate)`      |
| `Reverse`             | Flip the sequence                    | `.Reverse()`                               |

### Category 4 — Grouping

```csharp
// GroupBy — produces IEnumerable<IGrouping<TKey, TElement>>
var groups = employees
    .GroupBy(e => e.DepartmentId)
    .Select(g => new
    {
        DeptId    = g.Key,              // the grouped-by value
        Count     = g.Count(),
        Employees = g.ToList()
    });
```

### Category 5 — Aggregation

| Operator    | Description                        | Example                                |
|-------------|------------------------------------|----------------------------------------|
| `Count`     | Number of elements                 | `.Count()` or `.Count(x => x.Active)` |
| `Sum`       | Sum of a numeric property          | `.Sum(x => x.Salary)`                 |
| `Min`       | Smallest value                     | `.Min(x => x.Age)`                    |
| `Max`       | Largest value                      | `.Max(x => x.Salary)`                 |
| `Average`   | Mean value                         | `.Average(x => x.Score)`              |
| `Aggregate` | Custom fold operation              | `.Aggregate((a, b) => a + ", " + b)`  |

### Category 6 — Quantifiers

| Operator    | Returns  | Description                                    |
|-------------|----------|------------------------------------------------|
| `Any()`     | `bool`   | Is the collection non-empty?                   |
| `Any(pred)` | `bool`   | Does ANY element satisfy the condition?        |
| `All(pred)` | `bool`   | Do ALL elements satisfy the condition?         |
| `Contains`  | `bool`   | Is a specific value present?                   |

```csharp
bool hasEmployees  = list.Any();
bool anyHighEarner = list.Any(e => e.Salary > 100000);
bool allActive     = list.All(e => e.IsActive);
bool hasAlice      = names.Contains("Alice");
```

> **Performance rule:** `Any()` short-circuits after the first match. `Count() > 0` scans everything. Always use `Any()` for existence checks.

### Category 7 — Element Access

| Operator              | Throws if empty? | Throws if multiple? | Returns if empty |
|-----------------------|------------------|---------------------|------------------|
| `First()`             | Yes              | No                  | —                |
| `FirstOrDefault()`    | No               | No                  | `null` / `0`     |
| `Last()`              | Yes              | No                  | —                |
| `LastOrDefault()`     | No               | No                  | `null` / `0`     |
| `Single()`            | Yes              | Yes                 | —                |
| `SingleOrDefault()`   | No               | Yes                 | `null` / `0`     |
| `ElementAt(i)`        | Yes              | No                  | —                |
| `ElementAtOrDefault`  | No               | No                  | `null` / `0`     |

### Category 8 — Partitioning

| Operator      | Description                                      |
|---------------|--------------------------------------------------|
| `Take(n)`     | First n elements                                 |
| `TakeWhile`   | Elements while condition is true (then stop)     |
| `Skip(n)`     | Skip first n elements, return the rest           |
| `SkipWhile`   | Skip while condition is true, return the rest    |
| `Chunk(n)`    | Split into chunks of size n (.NET 6+)            |

```csharp
// Pagination pattern
var page3 = items
    .OrderBy(x => x.Id)
    .Skip((3 - 1) * 20)  // skip pages 1 and 2
    .Take(20)             // take page 3
    .ToList();
```

### Category 9 — Set Operations

| Operator     | Description                                    |
|--------------|------------------------------------------------|
| `Distinct`   | Remove duplicates from one sequence            |
| `Union`      | All unique elements from both sequences        |
| `Intersect`  | Elements present in BOTH sequences             |
| `Except`     | Elements in first but NOT in second            |
| `Concat`     | Combine both sequences (keeps duplicates)      |

### Category 10 — Conversion

| Operator         | Returns                    |
|------------------|----------------------------|
| `ToList()`       | `List<T>`                  |
| `ToArray()`      | `T[]`                      |
| `ToDictionary`   | `Dictionary<TKey, TValue>` |
| `ToHashSet`      | `HashSet<T>`               |
| `AsEnumerable`   | `IEnumerable<T>`           |
| `AsQueryable`    | `IQueryable<T>`            |
| `Cast<T>`        | Cast all elements to T     |
| `OfType<T>`      | Filter and cast            |

### Category 11 — Generation

```csharp
Enumerable.Range(1, 10)        // [1, 2, 3, ..., 10]
Enumerable.Repeat("hi", 3)     // ["hi", "hi", "hi"]
Enumerable.Empty<int>()        // []  — empty sequence
```

---

## 6. LINQ Joins — All Types with Diagrams

We use this data for all examples:

```csharp
var employees = new List<Employee>
{
    new() { Id=1, Name="Alice",   DeptId=10 },
    new() { Id=2, Name="Bob",     DeptId=20 },
    new() { Id=3, Name="Charlie", DeptId=99 },  // ← no matching dept
};

var departments = new List<Department>
{
    new() { Id=10, Name="Engineering" },
    new() { Id=20, Name="Marketing"   },
    new() { Id=30, Name="HR"          },         // ← no matching employee
};
```

---

### Inner Join

Returns only rows that have a match in **both** collections.

```
Employees:      [Alice-10]  [Bob-20]  [Charlie-99]
Departments:    [Eng-10]    [Mkt-20]  [HR-30]

Result:         [Alice + Engineering]   [Bob + Marketing]
                Charlie excluded (no dept 99)
                HR excluded (no employee in dept 30)
```

```csharp
// Method Syntax
var result = employees
    .Join(
        departments,                          // inner source
        emp  => emp.DeptId,                   // outer key
        dept => dept.Id,                      // inner key
        (emp, dept) => new                    // result shape
        {
            Employee   = emp.Name,
            Department = dept.Name
        }
    );

// Query Syntax
var result = from emp  in employees
             join dept in departments
             on emp.DeptId equals dept.Id
             select new { emp.Name, DeptName = dept.Name };
```

---

### Left Outer Join

Returns **all rows from the left** collection, plus matched rows from the right. Unmatched right side = `null`.

```
Employees:      [Alice-10]  [Bob-20]  [Charlie-99]
Departments:    [Eng-10]    [Mkt-20]  [HR-30]

Result:         [Alice + Engineering]
                [Bob + Marketing]
                [Charlie + NULL]      ← Charlie kept, dept is null
                HR excluded           ← HR is right side, no match
```

```csharp
// Method Syntax
var result = employees
    .GroupJoin(
        departments,
        emp  => emp.DeptId,
        dept => dept.Id,
        (emp, deptGroup) => new { emp, deptGroup }
    )
    .SelectMany(
        x => x.deptGroup.DefaultIfEmpty(),    // ← null if no match
        (x, dept) => new
        {
            Employee   = x.emp.Name,
            Department = dept?.Name ?? "No Department"
        }
    );

// Query Syntax (cleaner for left join)
var result = from emp  in employees
             join dept in departments
             on emp.DeptId equals dept.Id
             into deptGroup                           // step 1: group join
             from dept in deptGroup.DefaultIfEmpty()  // step 2: flatten with null
             select new
             {
                 emp.Name,
                 DeptName = dept?.Name ?? "No Department"
             };
```

> **Key concept:** `DefaultIfEmpty()` is what makes it a Left Join. On empty groups (no match), it injects a single `null`, keeping the left-side row alive.

---

### Right Outer Join

LINQ has no native right join. **Swap the sources** and do a Left Join instead.

```csharp
// Right Join = swap employees and departments, then Left Join
var result = from dept in departments             // ← now the LEFT source
             join emp  in employees
             on dept.Id equals emp.DeptId
             into empGroup
             from emp in empGroup.DefaultIfEmpty()
             select new
             {
                 DeptName = dept.Name,
                 Employee = emp?.Name ?? "No Employee"
             };

// Result includes HR + "No Employee" — HR is from the "right" original perspective
```

---

### Group Join (one-to-many hierarchical result)

Returns each outer element paired with a **collection** of matching inner elements. Perfect for parent-child relationships.

```
Result shape:
  Engineering → [Alice]
  Marketing   → [Bob]
  HR          → []           ← empty list, NOT excluded
```

```csharp
// Method Syntax
var result = departments
    .GroupJoin(
        employees,
        dept => dept.Id,
        emp  => emp.DeptId,
        (dept, empGroup) => new
        {
            Department = dept.Name,
            Employees  = empGroup.ToList()    // collection, not a single item
        }
    );

// Query Syntax
var result = from dept in departments
             join emp in employees
             on dept.Id equals emp.DeptId
             into empGroup
             select new
             {
                 Department = dept.Name,
                 Employees  = empGroup.ToList()
             };
```

---

### Full Outer Join

Returns **everything from both** sides. No row is excluded. Unmatched sides are `null`.

LINQ has no native Full Outer Join. Build it as: **Left Join UNION Right Join**.

```
Result:
  Alice + Engineering
  Bob + Marketing
  Charlie + null          ← from left join
  null + HR               ← from right join
```

```csharp
var leftJoin = from emp  in employees
               join dept in departments on emp.DeptId equals dept.Id
               into dg
               from dept in dg.DefaultIfEmpty()
               select new { EmpName = emp.Name, DeptName = dept?.Name };

var rightJoin = from dept in departments
                join emp  in employees on dept.Id equals emp.DeptId
                into eg
                from emp in eg.DefaultIfEmpty()
                select new { EmpName = emp?.Name, DeptName = dept.Name };

var fullJoin = leftJoin.Union(rightJoin);    // Union removes duplicates
```

---

### Cross Join (Cartesian Product)

Every element from the left × every element from the right. No join condition. Result size = m × n.

```
3 employees × 3 departments = 9 pairs
Alice-Engineering, Alice-Marketing, Alice-HR,
Bob-Engineering,   Bob-Marketing,   Bob-HR,
Charlie-Engineering, Charlie-Marketing, Charlie-HR
```

```csharp
// Query Syntax — double 'from' (no 'join' keyword)
var result = from emp  in employees
             from dept in departments
             select new { emp.Name, DeptName = dept.Name };

// Method Syntax — SelectMany
var result = employees
    .SelectMany(
        emp  => departments,
        (emp, dept) => new { emp.Name, DeptName = dept.Name }
    );
```

---

### Join Type Summary

```
Employees:  A(10)  B(20)  C(99)
Depts:      ●(10)  ●(20)  ●(30)

Inner Join:        A-10   B-20
                   (C and dept-30 excluded)

Left Join:         A-10   B-20   C-null
                   (C kept, dept-30 excluded)

Right Join:        A-10   B-20   null-30
                   (C excluded, dept-30 kept)

Full Join:         A-10   B-20   C-null   null-30
                   (nothing excluded)

Cross Join:        A-10  A-20  A-30
                   B-10  B-20  B-30
                   C-10  C-20  C-30
                   (every combination)

Group Join:        10 → [A]
                   20 → [B]
                   30 → []    (dept with no employees, not excluded)
```

---

### Joining Multiple Sources (3-way Join)

```csharp
// Query Syntax is significantly cleaner for 3+ sources
var result = from emp  in employees
             join dept in departments on emp.DeptId equals dept.Id
             join proj in projects    on emp.Id     equals proj.LeadId
             select new
             {
                 Employee   = emp.Name,
                 Department = dept.Name,
                 Project    = proj.Name
             };

// Method Syntax — requires nested anonymous types (harder to read)
var result = employees
    .Join(departments,
          emp  => emp.DeptId,
          dept => dept.Id,
          (emp, dept) => new { emp, dept })             // level 1 result
    .Join(projects,
          x    => x.emp.Id,
          proj => proj.LeadId,
          (x, proj) => new                              // level 2 result
          {
              Employee   = x.emp.Name,
              Department = x.dept.Name,
              Project    = proj.Name
          });
```

> **Tip:** For 3 or more sources, always prefer Query Syntax. Method Syntax works but becomes deeply nested and hard to maintain.

---

### Common LINQ Pitfalls

```csharp
// 1. Forgetting DefaultIfEmpty in left joins → behaves like inner join
from emp in employees
join dept in departments on emp.DeptId equals dept.Id
into dg
// Missing: from dept in dg.DefaultIfEmpty()  ← without this, unmatched rows vanish

// 2. Multiple enumeration — iterating a deferred query twice
IEnumerable<Employee> q = employees.Where(e => e.Salary > 50000);
int n = q.Count();   // runs once
q.ToList();          // runs again — fix: call ToList() first

// 3. Premature ToList() — filtering happens in C#, not DB
context.Employees.ToList().Where(e => e.Dept == "IT")  // loads ALL rows first!
// Fix:
context.Employees.Where(e => e.Dept == "IT").ToList()  // SQL filters, then loads

// 4. Any() vs Count() > 0
employees.Count() > 0   // iterates everything
employees.Any()         // stops at first element — always prefer Any()
```

---

*End of LINQ Reference — push questions to `/questions/linq/` in your repo.*