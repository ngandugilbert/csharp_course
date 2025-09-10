# Module 6 — Exception Handling & Debugging (Writing Robust Code)

## Overview

Exception handling and debugging are crucial skills for writing reliable C# applications. Exceptions occur when unexpected situations arise during program execution, and proper handling ensures your application can gracefully recover or fail safely. Debugging helps you identify and fix issues in your code. This module covers exception types, handling strategies, debugging techniques, and best practices for creating robust applications.

## Topics Covered

- Exception fundamentals and types
- Try-catch-finally blocks
- Custom exceptions
- Exception propagation and re-throwing
- Debugging with Visual Studio
- Breakpoints and stepping through code
- Watch windows and immediate window
- Call stack and locals
- Logging and tracing
- Unit testing basics
- Defensive programming techniques
- Error handling best practices

## Exception Fundamentals

### What are Exceptions?

Exceptions are runtime errors that occur when something unexpected happens during program execution. They differ from syntax errors (caught at compile-time) and logical errors (bugs in program logic).

```csharp
// This will compile fine but throw an exception at runtime
int[] numbers = { 1, 2, 3 };
int value = numbers[10]; // IndexOutOfRangeException
```

### Exception Hierarchy

All exceptions inherit from the `System.Exception` class:

```
System.Exception
├── System.SystemException
│   ├── System.ArgumentException
│   │   ├── System.ArgumentNullException
│   │   ├── System.ArgumentOutOfRangeException
│   │   └── System.ArgumentException
│   ├── System.ArithmeticException
│   │   ├── System.DivideByZeroException
│   │   └── System.OverflowException
│   ├── System.FormatException
│   ├── System.IndexOutOfRangeException
│   ├── System.InvalidCastException
│   ├── System.IO.IOException
│   │   ├── System.IO.FileNotFoundException
│   │   ├── System.IO.DirectoryNotFoundException
│   │   └── System.IO.PathTooLongException
│   ├── System.NullReferenceException
│   ├── System.OutOfMemoryException
│   └── System.StackOverflowException
└── System.ApplicationException (user-defined exceptions)
```

## Try-Catch-Finally Blocks

### Basic Exception Handling

```csharp
try
{
    // Code that might throw an exception
    int result = DivideNumbers(10, 0);
    Console.WriteLine($"Result: {result}");
}
catch (DivideByZeroException ex)
{
    // Handle specific exception type
    Console.WriteLine($"Division by zero: {ex.Message}");
}
catch (Exception ex)
{
    // Handle any other exception (catch-all)
    Console.WriteLine($"An error occurred: {ex.Message}");
}
finally
{
    // Always executes, regardless of whether an exception occurred
    Console.WriteLine("Cleanup code here");
}
```

### Multiple Catch Blocks

```csharp
public void ProcessFile(string filePath)
{
    try
    {
        string content = File.ReadAllText(filePath);
        ProcessContent(content);
    }
    catch (FileNotFoundException ex)
    {
        Console.WriteLine($"File not found: {ex.FileName}");
        // Could prompt user for different file
    }
    catch (UnauthorizedAccessException ex)
    {
        Console.WriteLine($"Access denied: {ex.Message}");
        // Could request elevated permissions
    }
    catch (IOException ex)
    {
        Console.WriteLine($"IO error: {ex.Message}");
        // Could retry operation or log error
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Unexpected error: {ex.Message}");
        // Log for debugging
        throw; // Re-throw the exception
    }
}
```

### Exception Filters (C# 6.0+)

```csharp
try
{
    ProcessData(data);
}
catch (ArgumentException ex) when (ex.ParamName == "input")
{
    Console.WriteLine("Invalid input parameter");
}
catch (ArgumentException ex) when (ex.ParamName == "output")
{
    Console.WriteLine("Invalid output parameter");
}
catch (InvalidOperationException ex) when (ex.Message.Contains("timeout"))
{
    Console.WriteLine("Operation timed out, retrying...");
    RetryOperation();
}
```

## Custom Exceptions

### Creating Custom Exception Classes

```csharp
// Custom exception for business logic errors
public class InsufficientFundsException : Exception
{
    public decimal CurrentBalance { get; }
    public decimal RequiredAmount { get; }

    public InsufficientFundsException(decimal currentBalance, decimal requiredAmount)
        : base($"Insufficient funds. Current balance: {currentBalance:C}, Required: {requiredAmount:C}")
    {
        CurrentBalance = currentBalance;
        RequiredAmount = requiredAmount;
    }

    public InsufficientFundsException(decimal currentBalance, decimal requiredAmount, Exception innerException)
        : base($"Insufficient funds. Current balance: {currentBalance:C}, Required: {requiredAmount:C}", innerException)
    {
        CurrentBalance = currentBalance;
        RequiredAmount = requiredAmount;
    }
}

// Custom exception with additional data
public class ValidationException : Exception
{
    public string PropertyName { get; }
    public object InvalidValue { get; }
    public List<string> ValidationErrors { get; } = new List<string>();

    public ValidationException(string propertyName, object invalidValue, string message)
        : base(message)
    {
        PropertyName = propertyName;
        InvalidValue = invalidValue;
    }

    public ValidationException(List<string> validationErrors)
        : base("Validation failed")
    {
        ValidationErrors = validationErrors ?? new List<string>();
    }
}
```

### Using Custom Exceptions

```csharp
public class BankAccount
{
    public decimal Balance { get; private set; }
    public string AccountNumber { get; }

    public BankAccount(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Withdrawal amount must be positive", nameof(amount));
        }

        if (amount > Balance)
        {
            throw new InsufficientFundsException(Balance, amount);
        }

        Balance -= amount;
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Deposit amount must be positive", nameof(amount));
        }

        Balance += amount;
    }
}
```

## Exception Propagation and Re-throwing

### Re-throwing Exceptions

```csharp
public void ProcessData(string data)
{
    try
    {
        ValidateData(data);
        SaveData(data);
    }
    catch (ValidationException ex)
    {
        // Log the error with additional context
        Logger.LogError($"Data validation failed for data: {data}", ex);

        // Re-throw to let caller handle it
        throw;
    }
    catch (Exception ex)
    {
        // Wrap the exception with more context
        throw new DataProcessingException("Failed to process data", ex);
    }
}
```

### Exception Chaining

```csharp
try
{
    // Some operation that might fail
    ExternalService.CallService();
}
catch (WebException ex)
{
    // Chain exceptions to preserve original error
    throw new ApplicationException("Service call failed", ex);
}
```

## Debugging Techniques

### Using Breakpoints

```csharp
public int CalculateSum(int[] numbers)
{
    int sum = 0;

    // Set breakpoint here to inspect 'numbers' array
    foreach (int number in numbers)
    {
        // Conditional breakpoint: break when number > 100
        sum += number;
    }

    // Set breakpoint here to check final sum
    return sum;
}
```

### Watch Windows and Immediate Window

```csharp
public void DebugExample()
{
    var data = new List<int> { 1, 2, 3, 4, 5 };

    // In Immediate Window, you can:
    // ? data.Count
    // ? data.Where(x => x > 3).ToList()
    // ? string.Join(", ", data)

    foreach (var item in data)
    {
        // Watch window can monitor:
        // - item (current value)
        // - data.IndexOf(item) (current index)
        // - data.Count (total count)
        ProcessItem(item);
    }
}
```

### Call Stack Navigation

```csharp
public void MethodA()
{
    Console.WriteLine("Entering MethodA");
    MethodB();
    Console.WriteLine("Exiting MethodA");
}

public void MethodB()
{
    Console.WriteLine("Entering MethodB");
    MethodC();
    Console.WriteLine("Exiting MethodB");
}

public void MethodC()
{
    Console.WriteLine("Entering MethodC");
    // Set breakpoint here to see call stack
    // Call stack will show: MethodC -> MethodB -> MethodA -> Main
    Console.WriteLine("Exiting MethodC");
}
```

## Logging and Tracing

### Basic Logging

```csharp
using System.Diagnostics;

public class Logger
{
    private static readonly TraceSource traceSource = new TraceSource("MyApplication");

    static Logger()
    {
        // Configure trace listeners
        traceSource.Listeners.Add(new ConsoleTraceListener());
        traceSource.Listeners.Add(new TextWriterTraceListener("app.log"));
        traceSource.Switch.Level = SourceLevels.All;
    }

    public static void LogInformation(string message)
    {
        traceSource.TraceEvent(TraceEventType.Information, 0, message);
    }

    public static void LogWarning(string message)
    {
        traceSource.TraceEvent(TraceEventType.Warning, 0, message);
    }

    public static void LogError(string message, Exception ex = null)
    {
        if (ex != null)
        {
            traceSource.TraceEvent(TraceEventType.Error, 0, $"{message}: {ex.Message}");
            traceSource.TraceData(TraceEventType.Error, 0, ex);
        }
        else
        {
            traceSource.TraceEvent(TraceEventType.Error, 0, message);
        }
    }

    public static void LogMethodEntry(string methodName)
    {
        traceSource.TraceEvent(TraceEventType.Start, 0, $"Entering {methodName}");
    }

    public static void LogMethodExit(string methodName)
    {
        traceSource.TraceEvent(TraceEventType.Stop, 0, $"Exiting {methodName}");
    }
}
```

### Structured Logging with Serilog

```csharp
using Serilog;

public class StructuredLogger
{
    static StructuredLogger()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("logs/app-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }

    public static void LogUserAction(string userId, string action, object data = null)
    {
        Log.Information("User {UserId} performed {Action} with data {@Data}",
                       userId, action, data);
    }

    public static void LogError(Exception ex, string operation, object context = null)
    {
        Log.Error(ex, "Error during {Operation} with context {@Context}",
                 operation, context);
    }

    public static void LogPerformance(string operation, TimeSpan duration)
    {
        Log.Information("Operation {Operation} completed in {Duration}ms",
                       operation, duration.TotalMilliseconds);
    }
}
```

## Unit Testing Basics

### Creating Unit Tests with MSTest

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class CalculatorTests
{
    private Calculator calculator;

    [TestInitialize]
    public void Setup()
    {
        calculator = new Calculator();
    }

    [TestMethod]
    public void Add_PositiveNumbers_ReturnsCorrectSum()
    {
        // Arrange
        int a = 5;
        int b = 3;
        int expected = 8;

        // Act
        int result = calculator.Add(a, b);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void Divide_ByZero_ThrowsDivideByZeroException()
    {
        // Arrange
        int dividend = 10;
        int divisor = 0;

        // Act & Assert
        Assert.ThrowsException<DivideByZeroException>(() =>
        {
            calculator.Divide(dividend, divisor);
        });
    }

    [TestMethod]
    [DataRow(1, 2, 3)]
    [DataRow(-1, 1, 0)]
    [DataRow(0, 0, 0)]
    public void Add_VariousInputs_ReturnsCorrectSum(int a, int b, int expected)
    {
        // Act
        int result = calculator.Add(a, b);

        // Assert
        Assert.AreEqual(expected, result);
    }
}
```

### Testing Exception Scenarios

```csharp
[TestMethod]
public void Withdraw_InsufficientFunds_ThrowsCustomException()
{
    // Arrange
    var account = new BankAccount("12345", 100);
    decimal withdrawalAmount = 150;

    // Act & Assert
    var exception = Assert.ThrowsException<InsufficientFundsException>(() =>
    {
        account.Withdraw(withdrawalAmount);
    });

    // Verify exception details
    Assert.AreEqual(100, exception.CurrentBalance);
    Assert.AreEqual(150, exception.RequiredAmount);
}
```

## Defensive Programming Techniques

### Input Validation

```csharp
public class UserService
{
    public User CreateUser(string username, string email, string password)
    {
        // Guard clauses for input validation
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty", nameof(username));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        if (!IsValidEmail(email))
            throw new ArgumentException("Invalid email format", nameof(email));

        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            throw new ArgumentException("Password must be at least 8 characters", nameof(password));

        // Proceed with user creation
        return new User
        {
            Username = username.Trim(),
            Email = email.ToLower().Trim(),
            PasswordHash = HashPassword(password)
        };
    }

    private bool IsValidEmail(string email)
    {
        // Simple email validation (in real app, use a proper validator)
        return email.Contains("@") && email.Contains(".");
    }

    private string HashPassword(string password)
    {
        // In real app, use proper password hashing
        return password.GetHashCode().ToString();
    }
}
```

### Null Checks and Null-Conditional Operators

```csharp
public class OrderProcessor
{
    public decimal CalculateTotal(Order order)
    {
        // Traditional null checking
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        if (order.Items == null)
            return 0;

        decimal total = 0;
        foreach (var item in order.Items)
        {
            if (item != null && item.Product != null)
            {
                total += item.Quantity * item.Product.Price;
            }
        }

        return total;
    }

    // Using null-conditional operators (C# 6.0+)
    public decimal CalculateTotalModern(Order order)
    {
        if (order?.Items == null)
            return 0;

        return order.Items
            .Where(item => item?.Product != null)
            .Sum(item => item.Quantity * item.Product.Price);
    }

    // Using null-coalescing operators
    public string GetDisplayName(User user)
    {
        return user?.Name ?? user?.Username ?? "Anonymous";
    }
}
```

### Resource Management with Using Statement

```csharp
public void ProcessFile(string filePath)
{
    // Automatic disposal with using statement
    using (var reader = new StreamReader(filePath))
    {
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            ProcessLine(line);
        }
    }
    // reader.Dispose() is called automatically here
}

// Multiple resources
public void CopyFile(string sourcePath, string destinationPath)
{
    using (var sourceStream = File.OpenRead(sourcePath))
    using (var destinationStream = File.Create(destinationPath))
    {
        sourceStream.CopyTo(destinationStream);
    }
    // Both streams are disposed automatically
}
```

## Error Handling Best Practices

### 1. Catch Specific Exceptions

```csharp
// Bad: Catching all exceptions
try
{
    DoSomething();
}
catch (Exception ex)
{
    Console.WriteLine("Something went wrong");
}

// Good: Catch specific exceptions
try
{
    DoSomething();
}
catch (FileNotFoundException ex)
{
    Console.WriteLine($"File not found: {ex.FileName}");
}
catch (UnauthorizedAccessException ex)
{
    Console.WriteLine("Access denied");
}
catch (IOException ex)
{
    Console.WriteLine($"IO error: {ex.Message}");
}
```

### 2. Don't Catch Exceptions You Can't Handle

```csharp
public void SaveData(Data data)
{
    try
    {
        // Attempt to save
        SaveToDatabase(data);
    }
    catch (SqlException ex)
    {
        // Log the error
        Logger.LogError("Failed to save data", ex);

        // Re-throw - we can't recover from this
        throw new DataSaveException("Failed to save data to database", ex);
    }
}
```

### 3. Use Finally for Cleanup

```csharp
public void ProcessTransaction(Transaction transaction)
{
    DatabaseConnection connection = null;
    try
    {
        connection = Database.OpenConnection();
        connection.BeginTransaction();

        // Process transaction steps
        ValidateTransaction(transaction);
        UpdateBalances(transaction);
        RecordTransaction(transaction);

        connection.CommitTransaction();
    }
    catch (Exception ex)
    {
        // Rollback on error
        connection?.RollbackTransaction();
        throw;
    }
    finally
    {
        // Always close connection
        connection?.Close();
    }
}
```

### 4. Create Meaningful Error Messages

```csharp
// Bad: Generic error message
throw new Exception("Error occurred");

// Good: Specific, actionable error message
throw new ValidationException("Customer age must be between 18 and 120",
                            nameof(customer.Age), customer.Age);
```

## Comprehensive Examples

### Example 1: Robust File Processor

```csharp
using System;
using System.IO;
using System.Collections.Generic;

public class FileProcessor
{
    private readonly ILogger logger;

    public FileProcessor(ILogger logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public List<string> ProcessFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        logger.LogInformation($"Processing file: {filePath}");

        try
        {
            // Validate file exists and is accessible
            var fileInfo = new FileInfo(filePath);

            if (!fileInfo.Exists)
                throw new FileNotFoundException("File does not exist", filePath);

            if (fileInfo.Length == 0)
            {
                logger.LogWarning("File is empty");
                return new List<string>();
            }

            // Process the file
            using (var reader = new StreamReader(filePath))
            {
                var lines = new List<string>();
                string line;
                int lineNumber = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;

                    try
                    {
                        string processedLine = ProcessLine(line, lineNumber);
                        if (!string.IsNullOrEmpty(processedLine))
                        {
                            lines.Add(processedLine);
                        }
                    }
                    catch (LineProcessingException ex)
                    {
                        logger.LogError($"Error processing line {lineNumber}: {ex.Message}", ex);
                        // Continue processing other lines
                    }
                }

                logger.LogInformation($"Successfully processed {lines.Count} lines from {lineNumber} total lines");
                return lines;
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogError($"Access denied to file: {filePath}", ex);
            throw new FileProcessingException($"Cannot access file: {filePath}", ex);
        }
        catch (IOException ex)
        {
            logger.LogError($"IO error processing file: {filePath}", ex);
            throw new FileProcessingException($"IO error processing file: {filePath}", ex);
        }
        catch (Exception ex)
        {
            logger.LogError($"Unexpected error processing file: {filePath}", ex);
            throw new FileProcessingException($"Unexpected error processing file: {filePath}", ex);
        }
    }

    private string ProcessLine(string line, int lineNumber)
    {
        if (string.IsNullOrWhiteSpace(line))
            return null;

        // Simulate some processing that might fail
        if (line.Contains("ERROR"))
        {
            throw new LineProcessingException($"Invalid data in line {lineNumber}: {line}");
        }

        // Process the line (e.g., trim, validate, transform)
        return line.Trim().ToUpper();
    }
}

public class LineProcessingException : Exception
{
    public LineProcessingException(string message) : base(message) { }
    public LineProcessingException(string message, Exception innerException)
        : base(message, innerException) { }
}

public class FileProcessingException : Exception
{
    public FileProcessingException(string message) : base(message) { }
    public FileProcessingException(string message, Exception innerException)
        : base(message, innerException) { }
}

public interface ILogger
{
    void LogInformation(string message);
    void LogWarning(string message);
    void LogError(string message, Exception ex = null);
}

public class ConsoleLogger : ILogger
{
    public void LogInformation(string message) => Console.WriteLine($"[INFO] {message}");
    public void LogWarning(string message) => Console.WriteLine($"[WARN] {message}");
    public void LogError(string message, Exception ex = null)
    {
        Console.WriteLine($"[ERROR] {message}");
        if (ex != null)
        {
            Console.WriteLine($"[ERROR] {ex.StackTrace}");
        }
    }
}
```

### Example 2: Retry Mechanism with Exponential Backoff

```csharp
using System;
using System.Threading;

public class RetryHelper
{
    private readonly ILogger logger;

    public RetryHelper(ILogger logger)
    {
        this.logger = logger;
    }

    public T ExecuteWithRetry<T>(Func<T> operation, int maxRetries = 3, int initialDelayMs = 1000)
    {
        if (operation == null)
            throw new ArgumentNullException(nameof(operation));

        if (maxRetries < 0)
            throw new ArgumentException("Max retries cannot be negative", nameof(maxRetries));

        Exception lastException = null;

        for (int attempt = 0; attempt <= maxRetries; attempt++)
        {
            try
            {
                if (attempt > 0)
                {
                    logger.LogInformation($"Retry attempt {attempt}/{maxRetries}");
                }

                return operation();
            }
            catch (Exception ex) when (IsRetryableException(ex))
            {
                lastException = ex;

                if (attempt == maxRetries)
                {
                    logger.LogError($"Operation failed after {maxRetries + 1} attempts", ex);
                    throw new RetryExhaustedException($"Operation failed after {maxRetries + 1} attempts", ex);
                }

                // Exponential backoff with jitter
                int delayMs = CalculateDelay(attempt, initialDelayMs);
                logger.LogWarning($"Operation failed (attempt {attempt + 1}), retrying in {delayMs}ms: {ex.Message}");

                Thread.Sleep(delayMs);
            }
            catch (Exception ex)
            {
                // Non-retryable exception
                logger.LogError("Non-retryable exception occurred", ex);
                throw;
            }
        }

        // This should never be reached, but just in case
        throw lastException ?? new InvalidOperationException("Unexpected error in retry logic");
    }

    public void ExecuteWithRetry(Action operation, int maxRetries = 3, int initialDelayMs = 1000)
    {
        ExecuteWithRetry(() => { operation(); return true; }, maxRetries, initialDelayMs);
    }

    private bool IsRetryableException(Exception ex)
    {
        // Define which exceptions are retryable
        return ex is TimeoutException ||
               ex is IOException ||
               (ex is WebException webEx && webEx.Status == WebExceptionStatus.Timeout) ||
               (ex is SqlException sqlEx && IsRetryableSqlError(sqlEx));
    }

    private bool IsRetryableSqlError(SqlException ex)
    {
        // SQL Server error codes that are typically retryable
        var retryableErrors = new[] { -2, 53, 701, 802, 945, 1205, 1222, 8645 };
        return Array.Exists(retryableErrors, error => ex.Number == error);
    }

    private int CalculateDelay(int attempt, int initialDelayMs)
    {
        // Exponential backoff: delay = initialDelay * 2^attempt
        int delay = initialDelayMs * (int)Math.Pow(2, attempt);

        // Add jitter to prevent thundering herd
        var random = new Random();
        int jitter = random.Next(-delay / 4, delay / 4);
        delay += jitter;

        // Cap maximum delay at 30 seconds
        return Math.Min(delay, 30000);
    }
}

public class RetryExhaustedException : Exception
{
    public RetryExhaustedException(string message) : base(message) { }
    public RetryExhaustedException(string message, Exception innerException)
        : base(message, innerException) { }
}

// Usage example
public class DataService
{
    private readonly RetryHelper retryHelper;

    public DataService(ILogger logger)
    {
        retryHelper = new RetryHelper(logger);
    }

    public string FetchDataFromExternalService(string url)
    {
        return retryHelper.ExecuteWithRetry(() =>
        {
            // Simulate external service call that might fail
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsStringAsync().Result;
            }
        }, maxRetries: 3, initialDelayMs: 1000);
    }

    public void SaveDataToDatabase(DataRecord record)
    {
        retryHelper.ExecuteWithRetry(() =>
        {
            // Simulate database operation that might fail
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // ... database operations ...
            }
        });
    }
}
```

## Exercises

1. **Custom Exception Classes**
   - Create custom exceptions for different business scenarios
   - Implement exception chaining and custom properties
   - Create exception filters for different error conditions

2. **Robust Calculator Application**
   - Implement a calculator with comprehensive error handling
   - Add input validation and meaningful error messages
   - Include logging and recovery mechanisms

3. **File Processing with Error Recovery**
   - Create a file processor that handles various file errors
   - Implement retry logic for transient failures
   - Add progress tracking and error reporting

4. **Database Operation Wrapper**
   - Create a wrapper for database operations with error handling
   - Implement connection pooling and timeout handling
   - Add transaction management with rollback capabilities

5. **Web Service Client with Resilience**
   - Build a web service client with retry and circuit breaker patterns
   - Implement exponential backoff and jitter
   - Add comprehensive logging and monitoring

6. **Unit Testing Exception Scenarios**
   - Write unit tests for exception handling code
   - Test custom exceptions and error propagation
   - Create tests for retry mechanisms and error recovery

Exception handling and debugging are essential skills for writing production-ready C# applications. By implementing proper error handling, logging, and debugging techniques, you can create applications that are more reliable, maintainable, and easier to troubleshoot. Remember that good error handling is proactive - anticipate potential issues and handle them gracefully rather than letting them crash your application!
