# Module 9 — OOP Pillars: Encapsulation, Inheritance, Polymorphism, Abstraction

## Overview

The four pillars of Object-Oriented Programming (OOP) form the foundation of well-designed, maintainable, and extensible software. This module explores each pillar in depth with comprehensive examples, best practices, and real-world applications. Understanding these principles is crucial for creating robust C# applications that can evolve and adapt to changing requirements.

## Topics Covered

- Encapsulation: Data hiding, access control, and information hiding
- Inheritance: Class hierarchies, base/derived relationships, and code reuse
- Polymorphism: Method overriding, virtual methods, and dynamic binding
- Abstraction: Abstract classes, interfaces, and contract-based design
- Method overriding vs method overloading
- Constructor chaining and initialization
- Interface segregation and dependency inversion
- Design patterns and OOP best practices
- SOLID principles application

## Encapsulation

### Understanding Encapsulation

Encapsulation is the practice of hiding internal implementation details and exposing only necessary interfaces. It protects data integrity and reduces coupling between components.

```csharp
public class BankAccount
{
    // Private fields - encapsulated data
    private string accountNumber;
    private decimal balance;
    private List<Transaction> transactionHistory;
    private readonly DateTime createdDate;

    // Public properties with controlled access
    public string AccountNumber
    {
        get { return accountNumber; }
        private set { accountNumber = value; }
    }

    public string AccountHolder { get; private set; }

    public decimal Balance
    {
        get { return balance; }
        private set { balance = value; }
    }

    public DateTime CreatedDate
    {
        get { return createdDate; }
    }

    // Constructor with validation
    public BankAccount(string accountNumber, string accountHolder, decimal initialDeposit = 0)
    {
        if (string.IsNullOrEmpty(accountNumber))
            throw new ArgumentException("Account number is required");

        if (string.IsNullOrEmpty(accountHolder))
            throw new ArgumentException("Account holder is required");

        if (initialDeposit < 0)
            throw new ArgumentException("Initial deposit cannot be negative");

        this.accountNumber = accountNumber;
        AccountHolder = accountHolder;
        balance = initialDeposit;
        transactionHistory = new List<Transaction>();
        createdDate = DateTime.Now;

        if (initialDeposit > 0)
        {
            transactionHistory.Add(new Transaction(DateTime.Now, initialDeposit, "Initial deposit"));
        }
    }

    // Public methods - encapsulated behavior
    public void Deposit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Deposit amount must be positive");

        Balance += amount;
        transactionHistory.Add(new Transaction(DateTime.Now, amount, "Deposit"));
    }

    public bool Withdraw(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Withdrawal amount must be positive");

        if (amount > Balance)
            return false; // Insufficient funds

        Balance -= amount;
        transactionHistory.Add(new Transaction(DateTime.Now, -amount, "Withdrawal"));
        return true;
    }

    public void Transfer(BankAccount targetAccount, decimal amount)
    {
        if (targetAccount == null)
            throw new ArgumentNullException(nameof(targetAccount));

        if (Withdraw(amount))
        {
            targetAccount.Deposit(amount);
            transactionHistory.Add(new Transaction(DateTime.Now, -amount,
                $"Transfer to {targetAccount.AccountNumber}"));
            targetAccount.transactionHistory.Add(new Transaction(DateTime.Now, amount,
                $"Transfer from {AccountNumber}"));
        }
    }

    public IEnumerable<Transaction> GetTransactionHistory()
    {
        return transactionHistory.AsReadOnly();
    }

    // Private helper method
    private bool IsValidTransaction(decimal amount)
    {
        return amount > 0 && amount <= 10000; // Max transaction limit
    }
}

public class Transaction
{
    public DateTime Timestamp { get; }
    public decimal Amount { get; }
    public string Description { get; }

    public Transaction(DateTime timestamp, decimal amount, string description)
    {
        Timestamp = timestamp;
        Amount = amount;
        Description = description;
    }
}
```

### Advanced Encapsulation Techniques

```csharp
public class SecureContainer<T>
{
    private T data;
    private string password;
    private bool isLocked = true;

    public SecureContainer(T data, string password)
    {
        this.data = data;
        this.password = HashPassword(password);
    }

    public bool Unlock(string password)
    {
        if (VerifyPassword(password))
        {
            isLocked = false;
            return true;
        }
        return false;
    }

    public void Lock()
    {
        isLocked = true;
    }

    public T GetData()
    {
        if (isLocked)
            throw new InvalidOperationException("Container is locked");

        return data;
    }

    public void SetData(T newData)
    {
        if (isLocked)
            throw new InvalidOperationException("Container is locked");

        data = newData;
    }

    public bool IsLocked => isLocked;

    private string HashPassword(string password)
    {
        // Simple hash for demonstration (use proper hashing in production)
        return password.GetHashCode().ToString();
    }

    private bool VerifyPassword(string password)
    {
        return HashPassword(password) == this.password;
    }
}
```

## Inheritance

### Basic Inheritance

```csharp
// Base class (parent/superclass)
public class Vehicle
{
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public decimal Price { get; set; }

    public Vehicle(string make, string model, int year, decimal price)
    {
        Make = make;
        Model = model;
        Year = year;
        Price = price;
    }

    public virtual void Start()
    {
        Console.WriteLine($"{Year} {Make} {Model} is starting...");
    }

    public virtual void Stop()
    {
        Console.WriteLine($"{Year} {Make} {Model} is stopping...");
    }

    public virtual string GetDescription()
    {
        return $"{Year} {Make} {Model}";
    }

    public void DisplayInfo()
    {
        Console.WriteLine($"Vehicle: {GetDescription()}");
        Console.WriteLine($"Price: ${Price:N2}");
    }
}

// Derived class (child/subclass)
public class Car : Vehicle
{
    public int NumberOfDoors { get; set; }
    public string FuelType { get; set; }

    public Car(string make, string model, int year, decimal price,
               int numberOfDoors, string fuelType)
        : base(make, model, year, price)
    {
        NumberOfDoors = numberOfDoors;
        FuelType = fuelType;
    }

    public override void Start()
    {
        base.Start(); // Call base class method
        Console.WriteLine("Car engine is running smoothly.");
    }

    public override string GetDescription()
    {
        return base.GetDescription() + $" ({NumberOfDoors}-door {FuelType})";
    }

    public void OpenTrunk()
    {
        Console.WriteLine("Trunk is open.");
    }
}

// Another derived class
public class Motorcycle : Vehicle
{
    public bool HasSidecar { get; set; }
    public string EngineType { get; set; }

    public Motorcycle(string make, string model, int year, decimal price,
                      bool hasSidecar, string engineType)
        : base(make, model, year, price)
    {
        HasSidecar = hasSidecar;
        EngineType = engineType;
    }

    public override void Start()
    {
        Console.WriteLine($"{Year} {Make} {Model} motorcycle is revving up!");
    }

    public override string GetDescription()
    {
        string sidecarText = HasSidecar ? "with sidecar" : "solo";
        return $"{base.GetDescription()} ({EngineType} {sidecarText})";
    }

    public void Wheelie()
    {
        Console.WriteLine("Doing a wheelie!");
    }
}
```

### Constructor Chaining and Base Class Initialization

```csharp
public class Employee
{
    public string EmployeeId { get; }
    public string Name { get; set; }
    public DateTime HireDate { get; }
    public decimal BaseSalary { get; set; }

    public Employee(string employeeId, string name, decimal baseSalary)
    {
        EmployeeId = employeeId ?? throw new ArgumentNullException(nameof(employeeId));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        HireDate = DateTime.Now;
        BaseSalary = baseSalary > 0 ? baseSalary : throw new ArgumentException("Salary must be positive");
    }

    public Employee(string employeeId, string name)
        : this(employeeId, name, 30000) // Chain to parameterized constructor
    {
    }

    public virtual decimal CalculateSalary()
    {
        return BaseSalary;
    }

    public virtual void DisplayInfo()
    {
        Console.WriteLine($"Employee: {Name} (ID: {EmployeeId})");
        Console.WriteLine($"Hire Date: {HireDate:d}");
        Console.WriteLine($"Salary: ${CalculateSalary():N2}");
    }
}

public class Manager : Employee
{
    public decimal Bonus { get; set; }
    public int TeamSize { get; set; }

    // Constructor chaining with base class
    public Manager(string employeeId, string name, decimal baseSalary,
                   decimal bonus, int teamSize)
        : base(employeeId, name, baseSalary)
    {
        Bonus = bonus;
        TeamSize = teamSize;
    }

    public Manager(string employeeId, string name, int teamSize)
        : this(employeeId, name, 50000, 10000, teamSize)
    {
    }

    public override decimal CalculateSalary()
    {
        return BaseSalary + Bonus;
    }

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"Bonus: ${Bonus:N2}");
        Console.WriteLine($"Team Size: {TeamSize}");
    }

    public void ConductMeeting()
    {
        Console.WriteLine($"{Name} is conducting a team meeting.");
    }
}

public class Developer : Employee
{
    public string ProgrammingLanguage { get; set; }
    public int YearsOfExperience { get; set; }

    public Developer(string employeeId, string name, decimal baseSalary,
                     string programmingLanguage, int yearsOfExperience)
        : base(employeeId, name, baseSalary)
    {
        ProgrammingLanguage = programmingLanguage;
        YearsOfExperience = yearsOfExperience;
    }

    public override decimal CalculateSalary()
    {
        // Experience-based salary calculation
        decimal experienceBonus = YearsOfExperience * 2000;
        return BaseSalary + experienceBonus;
    }

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"Programming Language: {ProgrammingLanguage}");
        Console.WriteLine($"Years of Experience: {YearsOfExperience}");
    }

    public void WriteCode()
    {
        Console.WriteLine($"{Name} is writing {ProgrammingLanguage} code.");
    }
}
```

### Sealed Classes and Methods

```csharp
// Sealed class cannot be inherited from
public sealed class SecuritySystem
{
    public void AuthenticateUser(string username, string password)
    {
        Console.WriteLine("Authenticating user...");
    }

    public void LogSecurityEvent(string eventDescription)
    {
        Console.WriteLine($"Security Event: {eventDescription}");
    }
}

// Class with sealed method
public class Document
{
    public virtual void Print()
    {
        Console.WriteLine("Printing document...");
    }

    public sealed override string ToString()
    {
        return "Document base class";
    }
}

public class Report : Document
{
    public override void Print()
    {
        Console.WriteLine("Printing report with formatting...");
    }

    // Cannot override ToString() because it's sealed in base class
    // public override string ToString() { ... } // Compilation error
}
```

## Polymorphism

### Method Overriding and Virtual Methods

```csharp
public abstract class PaymentProcessor
{
    public string ProcessorName { get; protected set; }

    public PaymentProcessor(string processorName)
    {
        ProcessorName = processorName;
    }

    // Abstract method - must be implemented by derived classes
    public abstract bool ProcessPayment(decimal amount, string cardNumber, string expiryDate);

    // Virtual method - can be overridden by derived classes
    public virtual void LogTransaction(decimal amount, bool success)
    {
        Console.WriteLine($"{ProcessorName}: Transaction of ${amount} was {(success ? "successful" : "failed")}");
    }

    // Non-virtual method - cannot be overridden
    public void DisplayProcessorInfo()
    {
        Console.WriteLine($"Payment Processor: {ProcessorName}");
    }
}

public class CreditCardProcessor : PaymentProcessor
{
    public CreditCardProcessor() : base("Credit Card Processor") { }

    public override bool ProcessPayment(decimal amount, string cardNumber, string expiryDate)
    {
        // Simulate credit card processing
        Console.WriteLine($"Processing ${amount} credit card payment...");
        Console.WriteLine($"Card: ****-****-****-{cardNumber.Substring(cardNumber.Length - 4)}");

        // Simple validation
        bool isValid = cardNumber.Length >= 13 && !string.IsNullOrEmpty(expiryDate);
        LogTransaction(amount, isValid);
        return isValid;
    }

    public override void LogTransaction(decimal amount, bool success)
    {
        Console.WriteLine($"[CREDIT CARD] {ProcessorName}: ${amount} - {(success ? "APPROVED" : "DECLINED")}");
    }
}

public class PayPalProcessor : PaymentProcessor
{
    public PayPalProcessor() : base("PayPal Processor") { }

    public override bool ProcessPayment(decimal amount, string cardNumber, string expiryDate)
    {
        // Simulate PayPal processing
        Console.WriteLine($"Processing ${amount} PayPal payment...");
        Console.WriteLine($"PayPal Account: {cardNumber}");

        // PayPal-specific validation
        bool isValid = cardNumber.Contains("@") && amount <= 10000;
        LogTransaction(amount, isValid);
        return isValid;
    }

    public override void LogTransaction(decimal amount, bool success)
    {
        Console.WriteLine($"[PAYPAL] {ProcessorName}: ${amount} - {(success ? "COMPLETED" : "FAILED")}");
    }
}

public class BankTransferProcessor : PaymentProcessor
{
    public BankTransferProcessor() : base("Bank Transfer Processor") { }

    public override bool ProcessPayment(decimal amount, string cardNumber, string expiryDate)
    {
        // Simulate bank transfer processing
        Console.WriteLine($"Processing ${amount} bank transfer...");
        Console.WriteLine($"Account: {cardNumber}");

        // Bank transfer validation
        bool isValid = cardNumber.Length >= 8 && amount >= 1;
        LogTransaction(amount, isValid);
        return isValid;
    }
}
```

### Polymorphism in Action

```csharp
public class PaymentService
{
    private List<PaymentProcessor> processors;

    public PaymentService()
    {
        processors = new List<PaymentProcessor>
        {
            new CreditCardProcessor(),
            new PayPalProcessor(),
            new BankTransferProcessor()
        };
    }

    public bool ProcessPayment(string processorType, decimal amount,
                              string paymentInfo, string expiryDate = null)
    {
        PaymentProcessor processor = FindProcessor(processorType);
        if (processor == null)
        {
            Console.WriteLine("Unknown payment processor");
            return false;
        }

        return processor.ProcessPayment(amount, paymentInfo, expiryDate);
    }

    public void DisplayAllProcessors()
    {
        foreach (PaymentProcessor processor in processors)
        {
            processor.DisplayProcessorInfo();
        }
    }

    private PaymentProcessor FindProcessor(string processorType)
    {
        return processors.Find(p =>
            p.ProcessorName.Contains(processorType, StringComparison.OrdinalIgnoreCase));
    }
}

class Program
{
    static void Main()
    {
        PaymentService paymentService = new PaymentService();

        // Demonstrate polymorphism
        paymentService.ProcessPayment("Credit", 100.50m, "4111111111111111", "12/25");
        paymentService.ProcessPayment("PayPal", 75.25m, "user@example.com");
        paymentService.ProcessPayment("Bank", 200.00m, "12345678");

        Console.WriteLine("\nAvailable processors:");
        paymentService.DisplayAllProcessors();
    }
}
```

## Abstraction

### Abstract Classes

```csharp
public abstract class DatabaseConnection
{
    public string ConnectionString { get; protected set; }
    public bool IsConnected { get; protected set; }

    protected DatabaseConnection(string connectionString)
    {
        ConnectionString = connectionString;
        IsConnected = false;
    }

    // Abstract methods - must be implemented by derived classes
    public abstract void Open();
    public abstract void Close();
    public abstract bool ExecuteQuery(string query);
    public abstract List<Dictionary<string, object>> GetResults();

    // Virtual method with default implementation
    public virtual void TestConnection()
    {
        try
        {
            Open();
            Console.WriteLine("Connection test successful");
            Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Connection test failed: {ex.Message}");
        }
    }

    // Concrete method
    public void DisplayConnectionInfo()
    {
        Console.WriteLine($"Connection String: {ConnectionString}");
        Console.WriteLine($"Status: {(IsConnected ? "Connected" : "Disconnected")}");
    }
}

public class SqlServerConnection : DatabaseConnection
{
    public SqlServerConnection(string connectionString) : base(connectionString) { }

    public override void Open()
    {
        // Simulate SQL Server connection
        Console.WriteLine("Opening SQL Server connection...");
        IsConnected = true;
    }

    public override void Close()
    {
        Console.WriteLine("Closing SQL Server connection...");
        IsConnected = false;
    }

    public override bool ExecuteQuery(string query)
    {
        if (!IsConnected) return false;

        Console.WriteLine($"Executing SQL Server query: {query}");
        // Simulate query execution
        return query.ToUpper().Contains("SELECT");
    }

    public override List<Dictionary<string, object>> GetResults()
    {
        // Simulate getting results
        return new List<Dictionary<string, object>>
        {
            new Dictionary<string, object> { ["Id"] = 1, ["Name"] = "John Doe" },
            new Dictionary<string, object> { ["Id"] = 2, ["Name"] = "Jane Smith" }
        };
    }
}

public class MySqlConnection : DatabaseConnection
{
    public MySqlConnection(string connectionString) : base(connectionString) { }

    public override void Open()
    {
        Console.WriteLine("Opening MySQL connection...");
        IsConnected = true;
    }

    public override void Close()
    {
        Console.WriteLine("Closing MySQL connection...");
        IsConnected = false;
    }

    public override bool ExecuteQuery(string query)
    {
        if (!IsConnected) return false;

        Console.WriteLine($"Executing MySQL query: {query}");
        return query.ToUpper().Contains("SELECT");
    }

    public override List<Dictionary<string, object>> GetResults()
    {
        return new List<Dictionary<string, object>>
        {
            new Dictionary<string, object> { ["id"] = 1, ["name"] = "John Doe" },
            new Dictionary<string, object> { ["id"] = 2, ["name"] = "Jane Smith" }
        };
    }
}
```

### Interfaces

```csharp
// Interface for logging
public interface ILogger
{
    void LogInfo(string message);
    void LogWarning(string message);
    void LogError(string message, Exception exception = null);
    bool IsEnabled(LogLevel level);
}

public enum LogLevel
{
    Info,
    Warning,
    Error
}

// Interface for data persistence
public interface IRepository<T>
{
    void Add(T item);
    T GetById(int id);
    IEnumerable<T> GetAll();
    void Update(T item);
    void Delete(int id);
    bool Exists(int id);
}

// Interface for notification service
public interface INotificationService
{
    void SendNotification(string recipient, string message);
    bool IsAvailable(string recipient);
}

// Concrete implementation combining multiple interfaces
public class EmailNotificationService : INotificationService, ILogger
{
    private List<string> sentNotifications = new List<string>();

    public void SendNotification(string recipient, string message)
    {
        if (!IsAvailable(recipient))
        {
            LogWarning($"Cannot send notification to {recipient}");
            return;
        }

        // Simulate sending email
        string notification = $"Email to {recipient}: {message}";
        sentNotifications.Add(notification);
        LogInfo($"Notification sent: {notification}");
    }

    public bool IsAvailable(string recipient)
    {
        // Simple email validation
        return recipient.Contains("@") && recipient.Contains(".");
    }

    public void LogInfo(string message)
    {
        Console.WriteLine($"[INFO] {DateTime.Now}: {message}");
    }

    public void LogWarning(string message)
    {
        Console.WriteLine($"[WARNING] {DateTime.Now}: {message}");
    }

    public void LogError(string message, Exception exception = null)
    {
        Console.WriteLine($"[ERROR] {DateTime.Now}: {message}");
        if (exception != null)
        {
            Console.WriteLine($"Exception: {exception.Message}");
        }
    }

    public bool IsEnabled(LogLevel level)
    {
        return true; // All levels enabled for this implementation
    }
}

// Generic repository implementation
public class InMemoryRepository<T> : IRepository<T> where T : class
{
    private List<T> items = new List<T>();
    private int nextId = 1;

    public void Add(T item)
    {
        // Assume T has an Id property (simplified example)
        items.Add(item);
    }

    public T GetById(int id)
    {
        return items.FirstOrDefault(); // Simplified
    }

    public IEnumerable<T> GetAll()
    {
        return items.AsReadOnly();
    }

    public void Update(T item)
    {
        // Implementation would update existing item
    }

    public void Delete(int id)
    {
        // Implementation would remove item
    }

    public bool Exists(int id)
    {
        return GetById(id) != null;
    }
}
```

### Interface Segregation Principle

```csharp
// Bad example: Fat interface (violates ISP)
public interface IWorker
{
    void Work();
    void Eat();
    void Sleep();
    void Code();
    void Test();
    void Design();
}

// Good example: Segregated interfaces
public interface IWorkable
{
    void Work();
}

public interface IFeedable
{
    void Eat();
}

public interface IRestable
{
    void Sleep();
}

public interface IDevelopable
{
    void Code();
    void Test();
}

public interface IDesignable
{
    void Design();
}

// Implementations can pick only what they need
public class HumanWorker : IWorkable, IFeedable, IRestable, IDevelopable
{
    public void Work() => Console.WriteLine("Human is working");
    public void Eat() => Console.WriteLine("Human is eating");
    public void Sleep() => Console.WriteLine("Human is sleeping");
    public void Code() => Console.WriteLine("Human is coding");
    public void Test() => Console.WriteLine("Human is testing");
}

public class RobotWorker : IWorkable, IDevelopable
{
    public void Work() => Console.WriteLine("Robot is working");
    public void Code() => Console.WriteLine("Robot is coding");
    public void Test() => Console.WriteLine("Robot is testing");
}

public class FreelanceDesigner : IWorkable, IFeedable, IRestable, IDesignable
{
    public void Work() => Console.WriteLine("Designer is working");
    public void Eat() => Console.WriteLine("Designer is eating");
    public void Sleep() => Console.WriteLine("Designer is sleeping");
    public void Design() => Console.WriteLine("Designer is designing");
}
```

## Advanced OOP Concepts

### Multiple Inheritance with Interfaces

```csharp
public interface IPrintable
{
    void Print();
    string GetPrintData();
}

public interface ISerializable
{
    string Serialize();
    void Deserialize(string data);
}

public interface ICloneable
{
    object Clone();
}

public class Document : IPrintable, ISerializable, ICloneable
{
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedDate { get; set; }

    public Document()
    {
        CreatedDate = DateTime.Now;
    }

    // IPrintable implementation
    public void Print()
    {
        Console.WriteLine("=== DOCUMENT PRINT ===");
        Console.WriteLine($"Title: {Title}");
        Console.WriteLine($"Content: {Content}");
        Console.WriteLine($"Created: {CreatedDate}");
    }

    public string GetPrintData()
    {
        return $"{Title}\n\n{Content}\n\nCreated: {CreatedDate}";
    }

    // ISerializable implementation
    public string Serialize()
    {
        return $"{Title}|{Content}|{CreatedDate.Ticks}";
    }

    public void Deserialize(string data)
    {
        string[] parts = data.Split('|');
        if (parts.Length == 3)
        {
            Title = parts[0];
            Content = parts[1];
            CreatedDate = new DateTime(long.Parse(parts[2]));
        }
    }

    // ICloneable implementation
    public object Clone()
    {
        return new Document
        {
            Title = this.Title,
            Content = this.Content,
            CreatedDate = this.CreatedDate
        };
    }
}
```

### Composition over Inheritance

```csharp
// Engine behavior
public interface IEngine
{
    void Start();
    void Stop();
    string GetEngineType();
}

public class GasolineEngine : IEngine
{
    public void Start() => Console.WriteLine("Gasoline engine started");
    public void Stop() => Console.WriteLine("Gasoline engine stopped");
    public string GetEngineType() => "Gasoline";
}

public class ElectricEngine : IEngine
{
    public void Start() => Console.WriteLine("Electric engine started");
    public void Stop() => Console.WriteLine("Electric engine stopped");
    public string GetEngineType() => "Electric";
}

// Transmission behavior
public interface ITransmission
{
    void ShiftGear(int gear);
    string GetTransmissionType();
}

public class ManualTransmission : ITransmission
{
    public void ShiftGear(int gear) => Console.WriteLine($"Manual transmission shifted to gear {gear}");
    public string GetTransmissionType() => "Manual";
}

public class AutomaticTransmission : ITransmission
{
    public void ShiftGear(int gear) => Console.WriteLine($"Automatic transmission shifted to gear {gear}");
    public string GetTransmissionType() => "Automatic";
}

// Car using composition
public class Car
{
    private IEngine engine;
    private ITransmission transmission;
    private string make;
    private string model;

    public Car(string make, string model, IEngine engine, ITransmission transmission)
    {
        this.make = make;
        this.model = model;
        this.engine = engine;
        this.transmission = transmission;
    }

    public void Start()
    {
        Console.WriteLine($"{make} {model} starting...");
        engine.Start();
    }

    public void Drive(int gear)
    {
        transmission.ShiftGear(gear);
        Console.WriteLine($"{make} {model} is driving with {engine.GetEngineType()} engine");
    }

    public void Stop()
    {
        engine.Stop();
        Console.WriteLine($"{make} {model} stopped");
    }

    public void DisplaySpecs()
    {
        Console.WriteLine($"Make: {make}");
        Console.WriteLine($"Model: {model}");
        Console.WriteLine($"Engine: {engine.GetEngineType()}");
        Console.WriteLine($"Transmission: {transmission.GetTransmissionType()}");
    }
}

class Program
{
    static void Main()
    {
        // Create different car configurations using composition
        Car sportsCar = new Car("Ferrari", "488", new GasolineEngine(), new ManualTransmission());
        Car electricCar = new Car("Tesla", "Model 3", new ElectricEngine(), new AutomaticTransmission());

        sportsCar.DisplaySpecs();
        Console.WriteLine();
        electricCar.DisplaySpecs();

        Console.WriteLine("\nDriving sports car:");
        sportsCar.Start();
        sportsCar.Drive(2);

        Console.WriteLine("\nDriving electric car:");
        electricCar.Start();
        electricCar.Drive(1);
    }
}
```

## SOLID Principles

### Single Responsibility Principle (SRP)

```csharp
// Bad: Multiple responsibilities
public class BadEmployee
{
    public string Name { get; set; }
    public decimal Salary { get; set; }

    // Employee data management
    public void Save() { /* Database logic */ }
    public void Load() { /* Database logic */ }

    // Salary calculation
    public decimal CalculateSalary() { /* Calculation logic */ }

    // Email sending
    public void SendEmail(string message) { /* Email logic */ }

    // Report generation
    public string GenerateReport() { /* Report logic */ }
}

// Good: Single responsibility per class
public class Employee
{
    public string Name { get; set; }
    public decimal Salary { get; set; }
}

public class EmployeeRepository
{
    public void Save(Employee employee) { /* Database logic */ }
    public Employee Load(int id) { /* Database logic */ }
}

public class SalaryCalculator
{
    public decimal CalculateSalary(Employee employee) { /* Calculation logic */ }
}

public class EmailService
{
    public void SendEmail(string to, string message) { /* Email logic */ }
}

public class ReportGenerator
{
    public string GenerateReport(Employee employee) { /* Report logic */ }
}
```

### Open/Closed Principle (OCP)

```csharp
// Bad: Modifying existing code for new functionality
public class BadShapeCalculator
{
    public double CalculateArea(object shape)
    {
        if (shape is Rectangle rect)
        {
            return rect.Width * rect.Height;
        }
        else if (shape is Circle circle)
        {
            return Math.PI * circle.Radius * circle.Radius;
        }
        // Adding Triangle requires modifying this method
        else if (shape is Triangle triangle)
        {
            return triangle.Base * triangle.Height / 2;
        }
        throw new ArgumentException("Unknown shape");
    }
}

// Good: Open for extension, closed for modification
public abstract class Shape
{
    public abstract double CalculateArea();
}

public class Rectangle : Shape
{
    public double Width { get; set; }
    public double Height { get; set; }

    public override double CalculateArea()
    {
        return Width * Height;
    }
}

public class Circle : Shape
{
    public double Radius { get; set; }

    public override double CalculateArea()
    {
        return Math.PI * Radius * Radius;
    }
}

public class Triangle : Shape
{
    public double Base { get; set; }
    public double Height { get; set; }

    public override double CalculateArea()
    {
        return Base * Height / 2;
    }
}

public class ShapeCalculator
{
    public double CalculateArea(Shape shape)
    {
        return shape.CalculateArea();
    }

    // Can calculate area for any shape without modification
    public double CalculateTotalArea(IEnumerable<Shape> shapes)
    {
        return shapes.Sum(shape => shape.CalculateArea());
    }
}
```

## Comprehensive Examples

### Example 1: University Management System

```csharp
// Base Person class
public abstract class Person
{
    public string Id { get; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }

    protected Person(string id, string firstName, string lastName, DateTime dateOfBirth)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        DateOfBirth = dateOfBirth;
    }

    public virtual string GetFullName()
    {
        return $"{FirstName} {LastName}";
    }

    public abstract void DisplayInfo();
}

// Student class
public class Student : Person
{
    public string StudentId { get; }
    public List<Course> EnrolledCourses { get; } = new List<Course>();
    public Dictionary<Course, Grade> Grades { get; } = new Dictionary<Course, Grade>();

    public Student(string id, string firstName, string lastName, DateTime dateOfBirth, string studentId)
        : base(id, firstName, lastName, dateOfBirth)
    {
        StudentId = studentId ?? throw new ArgumentNullException(nameof(studentId));
    }

    public void EnrollInCourse(Course course)
    {
        if (course == null) throw new ArgumentNullException(nameof(course));
        if (!EnrolledCourses.Contains(course))
        {
            EnrolledCourses.Add(course);
            course.EnrollStudent(this);
        }
    }

    public void AssignGrade(Course course, Grade grade)
    {
        if (course == null) throw new ArgumentNullException(nameof(course));
        Grades[course] = grade;
    }

    public double CalculateGPA()
    {
        if (Grades.Count == 0) return 0.0;

        double totalPoints = Grades.Values.Sum(grade => grade.PointValue);
        return totalPoints / Grades.Count;
    }

    public override void DisplayInfo()
    {
        Console.WriteLine($"Student: {GetFullName()}");
        Console.WriteLine($"Student ID: {StudentId}");
        Console.WriteLine($"Date of Birth: {DateOfBirth:d}");
        Console.WriteLine($"Enrolled Courses: {EnrolledCourses.Count}");
        Console.WriteLine($"GPA: {CalculateGPA():F2}");
    }
}

// Instructor class
public class Instructor : Person
{
    public string EmployeeId { get; }
    public string Department { get; set; }
    public List<Course> TeachingCourses { get; } = new List<Course>();

    public Instructor(string id, string firstName, string lastName, DateTime dateOfBirth,
                      string employeeId, string department)
        : base(id, firstName, lastName, dateOfBirth)
    {
        EmployeeId = employeeId ?? throw new ArgumentNullException(nameof(employeeId));
        Department = department ?? throw new ArgumentNullException(nameof(department));
    }

    public void AssignToCourse(Course course)
    {
        if (course == null) throw new ArgumentNullException(nameof(course));
        if (!TeachingCourses.Contains(course))
        {
            TeachingCourses.Add(course);
            course.AssignInstructor(this);
        }
    }

    public override void DisplayInfo()
    {
        Console.WriteLine($"Instructor: {GetFullName()}");
        Console.WriteLine($"Employee ID: {EmployeeId}");
        Console.WriteLine($"Department: {Department}");
        Console.WriteLine($"Teaching Courses: {TeachingCourses.Count}");
    }
}

// Course class
public class Course
{
    public string CourseCode { get; }
    public string CourseName { get; }
    public int Credits { get; }
    public Instructor Instructor { get; private set; }
    public List<Student> EnrolledStudents { get; } = new List<Student>();

    public Course(string courseCode, string courseName, int credits)
    {
        CourseCode = courseCode ?? throw new ArgumentNullException(nameof(courseCode));
        CourseName = courseName ?? throw new ArgumentNullException(nameof(courseName));
        Credits = credits > 0 ? credits : throw new ArgumentException("Credits must be positive");
    }

    public void AssignInstructor(Instructor instructor)
    {
        Instructor = instructor ?? throw new ArgumentNullException(nameof(instructor));
    }

    public void EnrollStudent(Student student)
    {
        if (student == null) throw new ArgumentNullException(nameof(student));
        if (!EnrolledStudents.Contains(student))
        {
            EnrolledStudents.Add(student);
        }
    }

    public void DisplayInfo()
    {
        Console.WriteLine($"Course: {CourseName} ({CourseCode})");
        Console.WriteLine($"Credits: {Credits}");
        Console.WriteLine($"Instructor: {Instructor?.GetFullName() ?? "Not assigned"}");
        Console.WriteLine($"Enrolled Students: {EnrolledStudents.Count}");
    }
}

// Grade enumeration
public enum Grade
{
    A = 4,
    B = 3,
    C = 2,
    D = 1,
    F = 0
}

public static class GradeExtensions
{
    public static double PointValue(this Grade grade)
    {
        return (int)grade;
    }

    public static string Description(this Grade grade)
    {
        return grade switch
        {
            Grade.A => "Excellent",
            Grade.B => "Good",
            Grade.C => "Satisfactory",
            Grade.D => "Poor",
            Grade.F => "Fail",
            _ => "Unknown"
        };
    }
}

class Program
{
    static void Main()
    {
        // Create instructor
        Instructor instructor = new Instructor("I001", "Dr.", "Smith", new DateTime(1975, 3, 15),
                                              "EMP001", "Computer Science");

        // Create students
        Student student1 = new Student("S001", "Alice", "Johnson", new DateTime(2000, 5, 20), "STU001");
        Student student2 = new Student("S002", "Bob", "Wilson", new DateTime(1999, 8, 10), "STU002");

        // Create course
        Course course = new Course("CS101", "Introduction to Programming", 3);

        // Assign instructor and enroll students
        instructor.AssignToCourse(course);
        student1.EnrollInCourse(course);
        student2.EnrollInCourse(course);

        // Assign grades
        student1.AssignGrade(course, Grade.A);
        student2.AssignGrade(course, Grade.B);

        // Display information
        Console.WriteLine("=== UNIVERSITY MANAGEMENT SYSTEM ===");
        instructor.DisplayInfo();
        Console.WriteLine();
        student1.DisplayInfo();
        Console.WriteLine();
        student2.DisplayInfo();
        Console.WriteLine();
        course.DisplayInfo();
    }
}
```

## Exercises

1. **E-Commerce System with OOP**
   - Create a base `Product` class with derived classes for `PhysicalProduct`, `DigitalProduct`, and `ServiceProduct`
   - Implement different pricing strategies using polymorphism
   - Create a shopping cart system that can handle different product types
   - Implement discount calculation with strategy pattern

2. **Library Management System**
   - Create abstract `LibraryItem` class with derived classes for `Book`, `Magazine`, `DVD`, and `Journal`
   - Implement borrowing and returning functionality with polymorphism
   - Add search and filtering capabilities
   - Implement overdue fine calculation

3. **Banking System with Multiple Account Types**
   - Create base `Account` class with derived classes for `SavingsAccount`, `CheckingAccount`, and `CreditAccount`
   - Implement different interest calculation methods
   - Add transaction history and statement generation
   - Implement transfer functionality between accounts

4. **Game Character System**
   - Create abstract `Character` class with derived classes for different character types
   - Implement attack and defense behaviors using polymorphism
   - Add inventory management with composition
   - Implement character leveling and skill systems

5. **Restaurant Management System**
   - Create base `MenuItem` class with derived classes for different food categories
   - Implement pricing and inventory management
   - Add order processing with polymorphism
   - Implement customer loyalty program

6. **File Processing Framework**
   - Create abstract `FileProcessor` class with derived classes for different file formats (CSV, XML, JSON)
   - Implement parsing and validation using polymorphism
   - Add progress reporting and error handling
   - Create a batch processing system

The four pillars of OOP—encapsulation, inheritance, polymorphism, and abstraction—are fundamental to creating maintainable, extensible, and robust software. By mastering these principles and applying SOLID design principles, you can create flexible systems that are easy to understand, test, and modify. Remember that good OOP design often favors composition over inheritance and depends on abstractions rather than concrete implementations.
