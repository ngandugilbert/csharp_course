# Module 8 — Classes & Objects (OOP Fundamentals)

## Overview

Classes and objects are the foundation of Object-Oriented Programming (OOP) in C#. A class is a blueprint or template that defines the structure and behavior of objects, while objects are instances of classes that contain actual data and can perform actions. This module covers the essential concepts of creating and working with classes and objects, including properties, methods, constructors, and access modifiers.

## Topics Covered

- Understanding classes and objects
- Creating classes and instantiating objects
- Properties and fields
- Methods and constructors
- Access modifiers (public, private, protected, internal)
- Static members vs instance members
- Object initialization syntax
- Object lifetime and disposal
- Nested classes
- Partial classes
- Class inheritance basics
- Best practices for class design

## Understanding Classes and Objects

### What is a Class?

A class is a user-defined data type that encapsulates data (fields/properties) and behavior (methods) into a single unit. It serves as a blueprint for creating objects.

```csharp
// Class definition
public class Car
{
    // Fields store data
    private string make;
    private string model;
    private int year;
    private decimal price;

    // Properties provide controlled access to fields
    public string Make
    {
        get { return make; }
        set { make = value; }
    }

    // Methods define behavior
    public void StartEngine()
    {
        Console.WriteLine("Engine started");
    }

    public void Drive(int miles)
    {
        Console.WriteLine($"Driving {miles} miles");
    }
}
```

### What is an Object?

An object is an instance of a class. When you create an object, you're creating a specific realization of the class blueprint with its own data.

```csharp
public class ClassObjectDemo
{
    public void DemonstrateObjects()
    {
        // Creating objects (instances) of the Car class
        Car car1 = new Car();  // First object
        Car car2 = new Car();  // Second object

        // Each object has its own data
        car1.Make = "Toyota";
        car2.Make = "Honda";

        // Each object can perform actions
        car1.StartEngine();  // Output: Engine started
        car2.StartEngine();  // Output: Engine started

        // Objects are independent
        car1.Drive(100);     // Drives car1 100 miles
        car2.Drive(50);      // Drives car2 50 miles
    }
}
```

## Properties and Fields

### Fields vs Properties

Fields are private variables that store data, while properties provide controlled access to that data.

```csharp
public class Employee
{
    // Fields (private data storage)
    private string name;
    private decimal salary;
    private readonly DateTime hireDate;

    // Properties (public interface to fields)
    public string Name
    {
        get { return name; }
        set
        {
            if (!string.IsNullOrEmpty(value))
                name = value;
        }
    }

    public decimal Salary
    {
        get { return salary; }
        set
        {
            if (value >= 0)
                salary = value;
        }
    }

    // Read-only property
    public DateTime HireDate
    {
        get { return hireDate; }
    }

    // Auto-implemented property (compiler generates backing field)
    public string Department { get; set; }

    // Constructor
    public Employee(string name, decimal salary)
    {
        this.name = name;
        this.salary = salary;
        this.hireDate = DateTime.Now;
        this.Department = "General";
    }
}
```

### Property Types

```csharp
public class PropertyExamples
{
    // Read-write property
    public string Title { get; set; }

    // Read-only property
    public DateTime CreatedDate { get; } = DateTime.Now;

    // Write-only property (rarely used)
    private string password;
    public string Password
    {
        set { password = HashPassword(value); }
    }

    // Computed property
    private decimal price;
    private decimal taxRate = 0.08m;

    public decimal Price
    {
        get { return price; }
        set { price = value; }
    }

    public decimal Tax
    {
        get { return price * taxRate; }
    }

    public decimal TotalPrice
    {
        get { return price + Tax; }
    }

    private string HashPassword(string input)
    {
        // Simple hash for demonstration (don't use in production)
        return input.GetHashCode().ToString();
    }
}
```

## Methods

### Instance Methods vs Static Methods

```csharp
public class Calculator
{
    // Instance field
    public string Name { get; set; }

    // Instance method (operates on instance data)
    public int Add(int a, int b)
    {
        Console.WriteLine($"{Name} is calculating {a} + {b}");
        return a + b;
    }

    // Static method (doesn't operate on instance data)
    public static int Multiply(int a, int b)
    {
        return a * b;
    }

    // Static method with no parameters
    public static void DisplayInfo()
    {
        Console.WriteLine("This is a Calculator class");
    }
}

public class MethodDemo
{
    public void DemonstrateMethods()
    {
        // Create instance
        Calculator calc = new Calculator { Name = "MyCalc" };

        // Call instance method
        int result1 = calc.Add(5, 3);  // Output: MyCalc is calculating 5 + 3

        // Call static method (no instance needed)
        int result2 = Calculator.Multiply(4, 6);
        Calculator.DisplayInfo();
    }
}
```

### Method Overloading

```csharp
public class MathOperations
{
    // Multiple methods with same name but different parameters
    public int Add(int a, int b)
    {
        return a + b;
    }

    public double Add(double a, double b)
    {
        return a + b;
    }

    public int Add(int a, int b, int c)
    {
        return a + b + c;
    }

    public int Add(params int[] numbers)
    {
        int sum = 0;
        foreach (int num in numbers)
        {
            sum += num;
        }
        return sum;
    }
}

public class OverloadDemo
{
    public void DemonstrateOverloading()
    {
        MathOperations math = new MathOperations();

        Console.WriteLine(math.Add(1, 2));           // 3
        Console.WriteLine(math.Add(1.5, 2.5));       // 4.0
        Console.WriteLine(math.Add(1, 2, 3));        // 6
        Console.WriteLine(math.Add(1, 2, 3, 4, 5)); // 15
    }
}
```

## Constructors

### Constructor Types

```csharp
public class Person
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public string Email { get; set; }

    // Default constructor
    public Person()
    {
        Console.WriteLine("Default constructor called");
    }

    // Parameterized constructor
    public Person(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        Console.WriteLine("Parameterized constructor called");
    }

    // Constructor with all parameters
    public Person(string firstName, string lastName, int age, string email)
        : this(firstName, lastName)  // Constructor chaining
    {
        Age = age;
        Email = email;
        Console.WriteLine("Full constructor called");
    }

    // Static constructor (called once per class)
    static Person()
    {
        Console.WriteLine("Static constructor called");
    }
}

public class ConstructorDemo
{
    public void DemonstrateConstructors()
    {
        // Static constructor called first (only once)
        Person person1 = new Person();  // Default constructor

        Person person2 = new Person("John", "Doe");  // Parameterized constructor

        Person person3 = new Person("Jane", "Smith", 25, "jane@example.com");  // Full constructor
    }
}
```

### Object Initializer Syntax

```csharp
public class Product
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }
    public bool IsAvailable { get; set; }
}

public class ObjectInitializerDemo
{
    public void DemonstrateObjectInitializers()
    {
        // Traditional way
        Product product1 = new Product();
        product1.Name = "Laptop";
        product1.Price = 999.99m;
        product1.Category = "Electronics";
        product1.IsAvailable = true;

        // Object initializer syntax (more concise)
        Product product2 = new Product
        {
            Name = "Mouse",
            Price = 29.99m,
            Category = "Electronics",
            IsAvailable = true
        };

        // Can combine with constructor
        Product product3 = new Product
        {
            Name = "Keyboard",
            Price = 79.99m
        };
        product3.Category = "Electronics";
        product3.IsAvailable = false;
    }
}
```

## Access Modifiers

### Access Modifier Examples

```csharp
public class AccessModifierDemo
{
    // Public: accessible from anywhere
    public string PublicField;

    // Private: accessible only within this class
    private string privateField;

    // Protected: accessible within this class and derived classes
    protected string protectedField;

    // Internal: accessible within the same assembly
    internal string internalField;

    // Protected Internal: accessible within assembly or derived classes
    protected internal string protectedInternalField;

    // Private Protected: accessible within class and derived classes in same assembly
    private protected string privateProtectedField;

    public void DemonstrateAccess()
    {
        // All fields are accessible within the class
        PublicField = "public";
        privateField = "private";
        protectedField = "protected";
        internalField = "internal";
        protectedInternalField = "protected internal";
        privateProtectedField = "private protected";
    }
}

public class DerivedClass : AccessModifierDemo
{
    public void TestAccess()
    {
        // Can access public, protected, protected internal, private protected
        PublicField = "accessible";
        protectedField = "accessible";
        protectedInternalField = "accessible";
        privateProtectedField = "accessible";

        // Cannot access private or internal (if in different assembly)
        // privateField = "not accessible";  // Compilation error
        // internalField = "not accessible"; // Compilation error if different assembly
    }
}
```

## Static Members

### Static Classes and Members

```csharp
public static class MathUtilities
{
    // Static field (shared across all uses)
    public static int OperationCount { get; private set; }

    // Static method
    public static double CalculateCircleArea(double radius)
    {
        OperationCount++;
        return Math.PI * radius * radius;
    }

    // Static method
    public static double CalculateCircleCircumference(double radius)
    {
        OperationCount++;
        return 2 * Math.PI * radius;
    }
}

public class StaticDemo
{
    public void DemonstrateStaticMembers()
    {
        // Call static methods without creating instance
        double area = MathUtilities.CalculateCircleArea(5.0);
        double circumference = MathUtilities.CalculateCircleCircumference(5.0);

        Console.WriteLine($"Area: {area:F2}");
        Console.WriteLine($"Circumference: {circumference:F2}");
        Console.WriteLine($"Operations performed: {MathUtilities.OperationCount}");
    }
}
```

### Instance vs Static Members

```csharp
public class Counter
{
    // Instance field (each object has its own copy)
    public int InstanceCount { get; private set; }

    // Static field (shared by all instances)
    public static int TotalCount { get; private set; }

    public void Increment()
    {
        InstanceCount++;
        TotalCount++;
    }

    public static void ResetTotal()
    {
        TotalCount = 0;
    }
}

public class InstanceVsStaticDemo
{
    public void DemonstrateInstanceVsStatic()
    {
        Counter counter1 = new Counter();
        Counter counter2 = new Counter();

        counter1.Increment();  // InstanceCount: 1, TotalCount: 1
        counter1.Increment();  // InstanceCount: 2, TotalCount: 2
        counter2.Increment();  // InstanceCount: 1, TotalCount: 3

        Console.WriteLine($"Counter1.InstanceCount: {counter1.InstanceCount}");  // 2
        Console.WriteLine($"Counter2.InstanceCount: {counter2.InstanceCount}");  // 1
        Console.WriteLine($"Counter.TotalCount: {Counter.TotalCount}");        // 3

        Counter.ResetTotal();
        Console.WriteLine($"After reset - Counter.TotalCount: {Counter.TotalCount}");  // 0
    }
}
```

## Advanced Class Concepts

### Nested Classes

```csharp
public class OuterClass
{
    private int outerField = 10;

    // Nested class
    public class NestedClass
    {
        public void AccessOuter()
        {
            // Cannot access outerField directly
            // Need reference to outer instance
        }
    }

    // Inner class (can access private members of outer class)
    private class InnerClass
    {
        public void AccessOuter(OuterClass outer)
        {
            Console.WriteLine($"Outer field: {outer.outerField}");
        }
    }

    public void UseInnerClass()
    {
        InnerClass inner = new InnerClass();
        inner.AccessOuter(this);
    }
}
```

### Partial Classes

```csharp
// File 1: Person.Part1.cs
public partial class Person
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public partial void Validate();  // Declaration only
}

// File 2: Person.Part2.cs
public partial class Person
{
    public int Age { get; set; }
    public string Email { get; set; }

    public partial void Validate()  // Implementation
    {
        if (Age < 0)
            throw new ArgumentException("Age cannot be negative");
    }

    public string GetFullName()
    {
        return $"{FirstName} {LastName}";
    }
}
```

## Object Lifetime and Disposal

### IDisposable Pattern

```csharp
public class ResourceManager : IDisposable
{
    private bool disposed = false;
    private FileStream fileStream;
    private StreamWriter writer;

    public ResourceManager(string filePath)
    {
        fileStream = new FileStream(filePath, FileMode.Create);
        writer = new StreamWriter(fileStream);
    }

    public void WriteData(string data)
    {
        if (disposed)
            throw new ObjectDisposedException(nameof(ResourceManager));

        writer.WriteLine(data);
    }

    // Implement IDisposable
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
            return;

        if (disposing)
        {
            // Dispose managed resources
            writer?.Dispose();
            fileStream?.Dispose();
        }

        // Dispose unmanaged resources here if any

        disposed = true;
    }

    // Destructor (finalizer) - called by GC if Dispose not called
    ~ResourceManager()
    {
        Dispose(false);
    }
}

public class DisposalDemo
{
    public void DemonstrateDisposal()
    {
        // Recommended: using statement
        using (ResourceManager manager = new ResourceManager("data.txt"))
        {
            manager.WriteData("Some data");
        }  // Dispose() called automatically

        // Manual disposal
        ResourceManager manager2 = new ResourceManager("data2.txt");
        try
        {
            manager2.WriteData("More data");
        }
        finally
        {
            manager2.Dispose();
        }
    }
}
```

## Best Practices

### 1. Class Design Principles

```csharp
// Good: Single Responsibility Principle
public class EmailService
{
    public void SendEmail(string to, string subject, string body)
    {
        // Email sending logic only
    }
}

// Bad: Multiple responsibilities
public class BadEmailService
{
    public void SendEmail(string to, string subject, string body)
    {
        // Email logic
    }

    public void SaveToDatabase(string data)
    {
        // Database logic - should be separate
    }

    public void LogMessage(string message)
    {
        // Logging logic - should be separate
    }
}
```

### 2. Property Design

```csharp
public class GoodPropertyDesign
{
    // Good: Encapsulation with validation
    private decimal balance;
    public decimal Balance
    {
        get { return balance; }
        private set
        {
            if (value < 0)
                throw new ArgumentException("Balance cannot be negative");
            balance = value;
        }
    }

    // Good: Read-only computed property
    public decimal BalanceInCents
    {
        get { return balance * 100; }
    }

    // Good: Auto-implemented for simple cases
    public string AccountNumber { get; private set; }

    public void Deposit(decimal amount)
    {
        Balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (amount > Balance)
            throw new InvalidOperationException("Insufficient funds");
        Balance -= amount;
    }
}
```

### 3. Constructor Best Practices

```csharp
public class BestPracticeConstructor
{
    public string Name { get; }
    public int Age { get; }
    public string Email { get; private set; }

    // Good: Use readonly properties for immutable data
    public BestPracticeConstructor(string name, int age)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Name is required");

        if (age < 0 || age > 150)
            throw new ArgumentOutOfRangeException("Age must be between 0 and 150");

        Name = name;
        Age = age;
    }

    // Good: Method for optional initialization
    public void SetEmail(string email)
    {
        if (!IsValidEmail(email))
            throw new ArgumentException("Invalid email format");

        Email = email;
    }

    private bool IsValidEmail(string email)
    {
        // Simple email validation
        return email.Contains("@") && email.Contains(".");
    }
}
```

## Comprehensive Examples

### Example 1: Bank Account System

```csharp
public class BankAccount
{
    public string AccountNumber { get; }
    public string OwnerName { get; }
    public DateTime CreatedDate { get; } = DateTime.Now;

    private decimal balance;
    private List<Transaction> transactionHistory = new List<Transaction>();

    public decimal Balance
    {
        get { return balance; }
        private set { balance = value; }
    }

    public BankAccount(string accountNumber, string ownerName)
    {
        AccountNumber = accountNumber ?? throw new ArgumentNullException(nameof(accountNumber));
        OwnerName = ownerName ?? throw new ArgumentNullException(nameof(ownerName));
    }

    public BankAccount(string accountNumber, string ownerName, decimal initialDeposit)
        : this(accountNumber, ownerName)
    {
        if (initialDeposit < 0)
            throw new ArgumentException("Initial deposit cannot be negative");

        Deposit(initialDeposit, "Initial deposit");
    }

    public void Deposit(decimal amount, string description = "Deposit")
    {
        if (amount <= 0)
            throw new ArgumentException("Deposit amount must be positive");

        Balance += amount;
        transactionHistory.Add(new Transaction(DateTime.Now, amount, description, TransactionType.Deposit));
    }

    public void Withdraw(decimal amount, string description = "Withdrawal")
    {
        if (amount <= 0)
            throw new ArgumentException("Withdrawal amount must be positive");

        if (amount > Balance)
            throw new InvalidOperationException("Insufficient funds");

        Balance -= amount;
        transactionHistory.Add(new Transaction(DateTime.Now, -amount, description, TransactionType.Withdrawal));
    }

    public void Transfer(BankAccount targetAccount, decimal amount, string description = "Transfer")
    {
        if (targetAccount == null)
            throw new ArgumentNullException(nameof(targetAccount));

        Withdraw(amount, $"Transfer to {targetAccount.AccountNumber}: {description}");
        targetAccount.Deposit(amount, $"Transfer from {AccountNumber}: {description}");
    }

    public IEnumerable<Transaction> GetTransactionHistory()
    {
        return transactionHistory.AsReadOnly();
    }

    public void PrintStatement()
    {
        Console.WriteLine($"Account Statement for {OwnerName}");
        Console.WriteLine($"Account Number: {AccountNumber}");
        Console.WriteLine($"Current Balance: ${Balance:F2}");
        Console.WriteLine($"Created: {CreatedDate:d}");
        Console.WriteLine();

        Console.WriteLine("Transaction History:");
        Console.WriteLine("Date\t\tAmount\t\tDescription");
        Console.WriteLine("----\t\t------\t\t-----------");

        foreach (Transaction transaction in transactionHistory)
        {
            Console.WriteLine($"{transaction.Date:d}\t${transaction.Amount:F2}\t{transaction.Description}");
        }
    }
}

public class Transaction
{
    public DateTime Date { get; }
    public decimal Amount { get; }
    public string Description { get; }
    public TransactionType Type { get; }

    public Transaction(DateTime date, decimal amount, string description, TransactionType type)
    {
        Date = date;
        Amount = amount;
        Description = description;
        Type = type;
    }
}

public enum TransactionType
{
    Deposit,
    Withdrawal,
    Transfer
}

class Program
{
    static void Main()
    {
        // Create accounts
        BankAccount account1 = new BankAccount("12345", "Alice Johnson", 1000);
        BankAccount account2 = new BankAccount("67890", "Bob Smith", 500);

        // Perform transactions
        account1.Deposit(200, "Salary deposit");
        account1.Withdraw(50, "ATM withdrawal");
        account1.Transfer(account2, 100, "Gift");

        // Print statements
        account1.PrintStatement();
        Console.WriteLine();
        account2.PrintStatement();
    }
}
```

### Example 2: Library Management System

```csharp
public class Book
{
    public string ISBN { get; }
    public string Title { get; }
    public string Author { get; }
    public int PublicationYear { get; }
    public bool IsAvailable { get; private set; } = true;

    public Book(string isbn, string title, string author, int publicationYear)
    {
        ISBN = isbn ?? throw new ArgumentNullException(nameof(isbn));
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Author = author ?? throw new ArgumentNullException(nameof(author));

        if (publicationYear < 1000 || publicationYear > DateTime.Now.Year + 1)
            throw new ArgumentOutOfRangeException(nameof(publicationYear));

        PublicationYear = publicationYear;
    }

    public void Borrow()
    {
        if (!IsAvailable)
            throw new InvalidOperationException("Book is already borrowed");

        IsAvailable = false;
    }

    public void Return()
    {
        if (IsAvailable)
            throw new InvalidOperationException("Book is not borrowed");

        IsAvailable = true;
    }

    public override string ToString()
    {
        return $"{Title} by {Author} ({PublicationYear}) - {(IsAvailable ? "Available" : "Borrowed")}";
    }
}

public class LibraryMember
{
    public string MemberId { get; }
    public string Name { get; }
    public string Email { get; private set; }
    public DateTime MembershipDate { get; } = DateTime.Now;
    public List<Book> BorrowedBooks { get; } = new List<Book>();

    public LibraryMember(string memberId, string name)
    {
        MemberId = memberId ?? throw new ArgumentNullException(nameof(memberId));
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public void UpdateEmail(string email)
    {
        if (string.IsNullOrEmpty(email) || !email.Contains("@"))
            throw new ArgumentException("Invalid email address");

        Email = email;
    }

    public void BorrowBook(Book book)
    {
        if (book == null)
            throw new ArgumentNullException(nameof(book));

        if (BorrowedBooks.Count >= 5)
            throw new InvalidOperationException("Member cannot borrow more than 5 books");

        if (!book.IsAvailable)
            throw new InvalidOperationException("Book is not available");

        book.Borrow();
        BorrowedBooks.Add(book);
    }

    public void ReturnBook(Book book)
    {
        if (book == null)
            throw new ArgumentNullException(nameof(book));

        if (!BorrowedBooks.Contains(book))
            throw new InvalidOperationException("Member did not borrow this book");

        book.Return();
        BorrowedBooks.Remove(book);
    }

    public void DisplayBorrowedBooks()
    {
        Console.WriteLine($"Books borrowed by {Name}:");
        if (BorrowedBooks.Count == 0)
        {
            Console.WriteLine("No books currently borrowed");
        }
        else
        {
            foreach (Book book in BorrowedBooks)
            {
                Console.WriteLine($"- {book.Title} by {book.Author}");
            }
        }
    }
}

public class Library
{
    private List<Book> books = new List<Book>();
    private List<LibraryMember> members = new List<LibraryMember>();

    public void AddBook(Book book)
    {
        if (book == null)
            throw new ArgumentNullException(nameof(book));

        if (books.Any(b => b.ISBN == book.ISBN))
            throw new InvalidOperationException("Book with this ISBN already exists");

        books.Add(book);
    }

    public void AddMember(LibraryMember member)
    {
        if (member == null)
            throw new ArgumentNullException(nameof(member));

        if (members.Any(m => m.MemberId == member.MemberId))
            throw new InvalidOperationException("Member with this ID already exists");

        members.Add(member);
    }

    public List<Book> SearchBooks(string searchTerm)
    {
        return books.Where(b =>
            b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            b.Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public LibraryMember FindMember(string memberId)
    {
        return members.FirstOrDefault(m => m.MemberId == memberId);
    }

    public void DisplayAvailableBooks()
    {
        Console.WriteLine("Available Books:");
        foreach (Book book in books.Where(b => b.IsAvailable))
        {
            Console.WriteLine($"- {book}");
        }
    }

    public void DisplayAllMembers()
    {
        Console.WriteLine("Library Members:");
        foreach (LibraryMember member in members)
        {
            Console.WriteLine($"- {member.Name} (ID: {member.MemberId}) - {member.BorrowedBooks.Count} books borrowed");
        }
    }
}

class Program
{
    static void Main()
    {
        Library library = new Library();

        // Add books
        library.AddBook(new Book("978-0-123456-78-9", "C# Programming", "John Doe", 2020));
        library.AddBook(new Book("978-0-987654-32-1", "Object-Oriented Design", "Jane Smith", 2019));
        library.AddBook(new Book("978-0-111111-11-1", "Data Structures", "Bob Johnson", 2021));

        // Add members
        LibraryMember alice = new LibraryMember("M001", "Alice Wilson");
        LibraryMember bob = new LibraryMember("M002", "Bob Brown");

        library.AddMember(alice);
        library.AddMember(bob);

        // Borrow books
        List<Book> searchResults = library.SearchBooks("C#");
        if (searchResults.Count > 0)
        {
            alice.BorrowBook(searchResults[0]);
        }

        searchResults = library.SearchBooks("Data");
        if (searchResults.Count > 0)
        {
            bob.BorrowBook(searchResults[0]);
        }

        // Display status
        library.DisplayAvailableBooks();
        Console.WriteLine();
        library.DisplayAllMembers();
        Console.WriteLine();
        alice.DisplayBorrowedBooks();
    }
}
```

## Exercises

1. **Student Grade Management System**
   - Create a `Student` class with properties for name, ID, and a list of grades
   - Add methods to add grades, calculate GPA, and display student information
   - Create a `Course` class that contains multiple students
   - Implement methods to calculate class average and find top performers

2. **Inventory Management System**
   - Create a `Product` class with properties for name, price, quantity, and category
   - Implement methods for updating inventory, calculating total value, and checking stock levels
   - Create an `Inventory` class that manages multiple products
   - Add functionality for searching products and generating inventory reports

3. **Task Management Application**
   - Create a `Task` class with properties for title, description, due date, and status
   - Implement methods for marking tasks complete, updating due dates, and checking overdue tasks
   - Create a `TaskManager` class that manages a collection of tasks
   - Add features for filtering tasks by status, priority, and due date

4. **Shape Calculator with Inheritance**
   - Create a base `Shape` class with abstract methods for calculating area and perimeter
   - Create derived classes for `Circle`, `Rectangle`, and `Triangle`
   - Implement the abstract methods in each derived class
   - Create a `ShapeCalculator` class that can work with any shape type

5. **Employee Management System**
   - Create a base `Employee` class with properties for name, ID, and salary
   - Create derived classes for `Manager`, `Developer`, and `Intern` with specific properties
   - Implement methods for calculating bonuses, displaying employee information, and managing subordinates
   - Create a `Company` class that manages all employees

6. **File System Simulator**
   - Create a `FileSystemItem` base class with properties for name and size
   - Create derived classes for `File` and `Directory`
   - Implement methods for calculating total size, displaying contents, and navigating the file system
   - Add functionality for creating, deleting, and moving items

Classes and objects are the building blocks of C# applications. By mastering these concepts, you can create well-structured, maintainable, and extensible code. Remember to follow good design principles like encapsulation, single responsibility, and proper use of access modifiers to create robust object-oriented applications!
