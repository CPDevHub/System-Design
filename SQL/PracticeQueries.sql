Create Database TestDb
use TestDb

-- Customers
CREATE TABLE Customers (
    Id INT PRIMARY KEY,
    Name VARCHAR(50) NOT NULL,
    City VARCHAR(50),
    Age INT
);


-- Products
CREATE TABLE Products (
    Id INT PRIMARY KEY,
    Name VARCHAR(50) NOT NULL,
    Price DECIMAL(10,2) NOT NULL
);

-- Orders
CREATE TABLE Orders (
    Id INT PRIMARY KEY,
    CustomerId INT,
    OrderDate DATE,
    TotalAmount DECIMAL(10,2),
    FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
);

-- OrderItems
CREATE TABLE OrderItems (
    Id INT PRIMARY KEY,
    OrderId INT,
    ProductId INT,
    Quantity INT,
    FOREIGN KEY (OrderId) REFERENCES Orders(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

-- Customers
INSERT INTO Customers VALUES
(1, 'Alice', 'Delhi', 30),
(2, 'Bob', 'Mumbai', 25),
(3, 'Charlie', 'Delhi', 35),
(4, 'David', 'Pune', 40),
(5, 'Eva', NULL, 28),
(6, 'Frank', 'Delhi', NULL);

-- Products
INSERT INTO Products VALUES
(1, 'Laptop', 50000),
(2, 'Phone', 20000),
(3, 'Tablet', 15000),
(4, 'Headphones', 2000),
(5, 'Mouse', 500);

-- Orders
INSERT INTO Orders VALUES
(1, 1, '2024-01-01', 52000),
(2, 1, '2024-01-10', 20000),
(3, 2, '2024-01-05', 15000),
(4, 3, '2024-02-01', 500),
(5, 4, '2024-02-10', 70000),
(6, NULL, '2024-03-01', 10000); -- edge: no customer

-- OrderItems
INSERT INTO OrderItems VALUES
(1, 1, 1, 1),
(2, 1, 4, 1),
(3, 2, 2, 1),
(4, 3, 3, 1),
(5, 4, 5, 1),
(6, 5, 1, 1),
(7, 5, 2, 1),
(8, 6, 4, 2);

Select * from Customers
Select * from Orders
Select * from OrderItems
Select * from Products

--Q1) Get Customer with their Orders
Select Customer.Name, Ord.OrderDate, Ord.TotalAmount from Customers Customer Join Orders Ord On Customer.Id=Ord.CustomerId;

--Q2 Get total number of orders per customer
Select Customer.Id, Customer.Name, IsNull(Ord.TotalOrders,0) AS Total from Customers Customer Left Join (Select CustomerId, Count(*) As TotalOrders from Orders Group by CustomerId) Ord On Customer.Id=Ord.CustomerId;
Select Customer.Id, Customer.Name, Count(Ord.CustomerId) As TotalOrders from Customers Customer  Left Join Orders Ord On Customer.Id=Ord.CustomerId Group By Customer.Id,Customer.Name;

--Q3 Get total amount spent by each customer
Select Customer.Id, Customer.Name, COALESCE(Ord.Total,0) As Total from Customers Customer Left Join (Select CustomerId, SUM(TotalAmount) As Total from Orders Group by CustomerId) Ord On Customer.Id=Ord.CustomerId;
Select Customer.Id, Customer.Name, COALESCE(SUM(Ord.TotalAmount),0) As Total from Customers Customer  Left Join Orders Ord On Customer.Id=Ord.CustomerId Group By Customer.Id,Customer.Name;

--Q4)Get customers who have NOT placed any orders
Select Customer.Id, Customer.Name from Customers Customer left join Orders Ord on Customer.Id = Ord.CustomerId where Ord.Id Is NULL;
Select Customer.Id, Customer.Name from Customers Customer where Not Exists(Select 1 from Orders Ord where Customer.Id=Ord.CustomerId)

--Q5) Get products that were ordered at least once(Also with Corelated Subquery)
Select * from Products Prod left join OrderItems OrdItem on Prod.Id = OrdItem.ProductId where OrdItem.ProductId Is Not NULL;
Select Prod.Id, Prod.Name from Products Prod where Exists(Select 1 from OrderItems OrdItem where OrdItem.ProductId=Prod.Id)

--Q6)Get most expensive product
Select Name, Price from Products where Price = (Select Max(Price) from Products);

--Q7)Get all orders with their product names(Multiple Joins)
Select Ord.Id, STRING_AGG(Prod.Name,', ') As ProdNames from Orders Ord JOIN OrderItems OrdItem On Ord.Id = OrdItem.OrderId Join Products Prod On Prod.Id = OrdItem.ProductId Group By Ord.Id

--Q8)Get customers whose total spending is above average spending
Select Customer.Name, Sum(Ord.TotalAmount), (Select AVG(TotalAmount) As AverageSpending from Orders) from Customers Customer Join Orders Ord On Customer.Id=Ord.CustomerId group By Customer.Id, Customer.Name having Sum(Ord.TotalAmount)> (Select AVG(TotalAmount) As AverageSpending from Orders); 

--Q9)Get customer who placed the highest total orders
Select * from (Select DENSE_RANK() Over(Order By Ord.TotalOrders DESC) As rnk, Customer.Id,Customer.Name, Ord.TotalOrders from Customers Customer Join(Select CustomerId, Count(*) As TotalOrders from Orders Group By CustomerId) Ord On Customer.Id=Ord.CustomerId) T where rnk=1;

--Q10)Get product that generated the highest revenue
Select * from (Select Prod.Id, Prod.Name, DENSE_RANK() OVER (Order By OrdInfo.TotalOrderedQuantity*Prod.Price DESC) As rnk from Products Prod Join (Select ProductId,Sum(Quantity) As TotalOrderedQuantity from OrderItems OrdItem Group By ProductId) OrdInfo On Prod.Id = OrdInfo.ProductId) T where rnk=1;

--Q11)Get customers who ordered all products
Select Customer.Id, Customer.Name from Customers Customer Join Orders Ord On Customer.Id = Ord.CustomerId Join OrderItems OrdItem ON Ord.Id = OrdItem.OrderId group By Customer.Id,Customer.Name HAVING COUNT(DISTINCT OrdItem.ProductId) = (select Count(ID) from Products);



