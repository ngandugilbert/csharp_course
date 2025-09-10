# Module 10 — Advanced Topics (Modern C# Features)

## Overview

This module explores advanced C# features that enable sophisticated programming techniques. You'll learn about asynchronous programming, reflection, attributes, LINQ, delegates, events, and modern language features that make C# powerful and expressive. These concepts are essential for building scalable, maintainable applications and working with complex scenarios.

## Topics Covered

- Asynchronous programming with async/await
- LINQ (Language Integrated Query) in depth
- Delegates and events
- Reflection and metadata
- Attributes and custom attributes
- Extension methods
- Anonymous types and dynamic typing
- Pattern matching (advanced)
- Records and immutable types
- Nullable reference types
- Tuples and deconstruction
- Local functions
- Expression trees

## Asynchronous Programming

### Async/Await Fundamentals

```csharp
public class AsyncExamples
{
    // Basic async method
    public async Task<string> DownloadContentAsync(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            // Non-blocking call
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                // Non-blocking read
                string content = await response.Content.ReadAsStringAsync();
                return content;
            }

            throw new HttpRequestException($"Failed to download: {response.StatusCode}");
        }
    }

    // Async method with progress reporting
    public async Task DownloadFileWithProgressAsync(string url, string destinationPath,
                                                   IProgress<double> progress = null)
    {
        using (HttpClient client = new HttpClient())
        using (HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
        using (Stream contentStream = await response.Content.ReadAsStreamAsync())
        using (FileStream fileStream = File.Create(destinationPath))
        {
            long totalBytes = response.Content.Headers.ContentLength ?? -1;
            long downloadedBytes = 0;
            byte[] buffer = new byte[8192];
            int bytesRead;

            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, bytesRead);
                downloadedBytes += bytesRead;

                if (totalBytes > 0 && progress != null)
                {
                    double percentage = (double)downloadedBytes / totalBytes * 100;
                    progress.Report(percentage);
                }
            }
        }
    }

    // Parallel async operations
    public async Task<IEnumerable<string>> DownloadMultipleUrlsAsync(IEnumerable<string> urls)
    {
        // Create tasks for all downloads
        IEnumerable<Task<string>> downloadTasks = urls.Select(url => DownloadContentAsync(url));

        // Wait for all to complete
        string[] results = await Task.WhenAll(downloadTasks);

        return results;
    }

    // Async with cancellation
    public async Task ProcessDataWithCancellationAsync(IEnumerable<int> data,
                                                       CancellationToken cancellationToken = default)
    {
        foreach (int item in data)
        {
            // Check for cancellation
            cancellationToken.ThrowIfCancellationRequested();

            // Simulate processing
            await Task.Delay(100, cancellationToken);

            Console.WriteLine($"Processed: {item}");

            // Report progress if needed
            // progress?.Report((double)processedCount / totalCount * 100);
        }
    }
}

public class AsyncUsageExample
{
    public async Task DemonstrateAsync()
    {
        AsyncExamples asyncExamples = new AsyncExamples();

        // Progress reporting
        Progress<double> progress = new Progress<double>(percentage =>
        {
            Console.WriteLine($"Download progress: {percentage:F1}%");
        });

        try
        {
            // Download with progress
            await asyncExamples.DownloadFileWithProgressAsync(
                "https://example.com/file.zip",
                "downloaded_file.zip",
                progress);

            Console.WriteLine("Download completed successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Download failed: {ex.Message}");
        }

        // Multiple downloads
        string[] urls = {
            "https://api.example.com/data1",
            "https://api.example.com/data2",
            "https://api.example.com/data3"
        };

        IEnumerable<string> results = await asyncExamples.DownloadMultipleUrlsAsync(urls);
        Console.WriteLine($"Downloaded {results.Count()} pages");

        // Cancellation example
        CancellationTokenSource cts = new CancellationTokenSource();
        cts.CancelAfter(5000); // Cancel after 5 seconds

        try
        {
            await asyncExamples.ProcessDataWithCancellationAsync(
                Enumerable.Range(1, 100), cts.Token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation was cancelled");
        }
    }
}
```

### Task-Based Asynchronous Patterns

```csharp
public class TaskBasedPatterns
{
    // Fire and forget (be careful with exception handling)
    public void FireAndForget()
    {
        Task.Run(async () =>
        {
            try
            {
                await DoSomeWorkAsync();
            }
            catch (Exception ex)
            {
                // Handle exceptions in fire-and-forget scenarios
                Console.WriteLine($"Background task failed: {ex.Message}");
            }
        });
    }

    // Producer-consumer pattern
    public class AsyncQueue<T>
    {
        private readonly Queue<T> queue = new Queue<T>();
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(0);
        private bool isCompleted = false;

        public void Enqueue(T item)
        {
            lock (queue)
            {
                queue.Enqueue(item);
            }
            semaphore.Release();
        }

        public void Complete()
        {
            isCompleted = true;
            semaphore.Release();
        }

        public async Task<T> DequeueAsync(CancellationToken cancellationToken = default)
        {
            await semaphore.WaitAsync(cancellationToken);

            lock (queue)
            {
                if (queue.Count > 0)
                {
                    return queue.Dequeue();
                }
            }

            if (isCompleted)
            {
                throw new InvalidOperationException("Queue is completed");
            }

            // This shouldn't happen, but handle gracefully
            throw new InvalidOperationException("Unexpected queue state");
        }
    }

    // Retry pattern
    public async Task<T> RetryAsync<T>(Func<Task<T>> operation, int maxRetries = 3,
                                       TimeSpan? delay = null)
    {
        delay ??= TimeSpan.FromSeconds(1);

        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex) when (i < maxRetries - 1)
            {
                Console.WriteLine($"Attempt {i + 1} failed: {ex.Message}. Retrying in {delay.Value.TotalSeconds}s...");
                await Task.Delay(delay.Value);
                delay = delay.Value * 2; // Exponential backoff
            }
        }

        throw new Exception($"Operation failed after {maxRetries} attempts");
    }

    private async Task DoSomeWorkAsync()
    {
        await Task.Delay(1000);
        Console.WriteLine("Work completed");
    }
}
```

## LINQ (Language Integrated Query)

### Advanced LINQ Operations

```csharp
public class LinqAdvancedExamples
{
    public void DemonstrateAdvancedLinq()
    {
        // Sample data
        List<Employee> employees = new List<Employee>
        {
            new Employee { Id = 1, Name = "Alice", Department = "Engineering", Salary = 75000, HireDate = new DateTime(2020, 1, 15) },
            new Employee { Id = 2, Name = "Bob", Department = "Engineering", Salary = 80000, HireDate = new DateTime(2019, 6, 10) },
            new Employee { Id = 3, Name = "Charlie", Department = "Sales", Salary = 65000, HireDate = new DateTime(2021, 3, 20) },
            new Employee { Id = 4, Name = "Diana", Department = "Engineering", Salary = 90000, HireDate = new DateTime(2018, 11, 5) },
            new Employee { Id = 5, Name = "Eve", Department = "Sales", Salary = 70000, HireDate = new DateTime(2020, 8, 12) },
            new Employee { Id = 6, Name = "Frank", Department = "HR", Salary = 55000, HireDate = new DateTime(2022, 1, 8) }
        };

        // Complex query with multiple operations
        var engineeringReport = employees
            .Where(e => e.Department == "Engineering")
            .OrderByDescending(e => e.Salary)
            .ThenBy(e => e.HireDate)
            .Select(e => new
            {
                e.Name,
                e.Salary,
                Tenure = (DateTime.Now - e.HireDate).Days / 365.0,
                SalaryRank = 0 // Will be calculated below
            })
            .ToList();

        // Add ranking
        for (int i = 0; i < engineeringReport.Count; i++)
        {
            engineeringReport[i] = engineeringReport[i] with { SalaryRank = i + 1 };
        }

        Console.WriteLine("Engineering Department Report:");
        foreach (var emp in engineeringReport)
        {
            Console.WriteLine($"{emp.SalaryRank}. {emp.Name}: ${emp.Salary:N0}, {emp.Tenure:F1} years");
        }

        // Grouping and aggregation
        var departmentStats = employees
            .GroupBy(e => e.Department)
            .Select(g => new
            {
                Department = g.Key,
                EmployeeCount = g.Count(),
                AverageSalary = g.Average(e => e.Salary),
                MaxSalary = g.Max(e => e.Salary),
                MinSalary = g.Min(e => e.Salary),
                TotalSalary = g.Sum(e => e.Salary)
            })
            .OrderByDescending(d => d.AverageSalary);

        Console.WriteLine("\nDepartment Statistics:");
        foreach (var dept in departmentStats)
        {
            Console.WriteLine($"{dept.Department}:");
            Console.WriteLine($"  Employees: {dept.EmployeeCount}");
            Console.WriteLine($"  Avg Salary: ${dept.AverageSalary:N0}");
            Console.WriteLine($"  Salary Range: ${dept.MinSalary:N0} - ${dept.MaxSalary:N0}");
            Console.WriteLine($"  Total Payroll: ${dept.TotalSalary:N0}");
        }

        // Joining data
        List<Department> departments = new List<Department>
        {
            new Department { Id = 1, Name = "Engineering", Budget = 500000 },
            new Department { Id = 2, Name = "Sales", Budget = 300000 },
            new Department { Id = 3, Name = "HR", Budget = 150000 }
        };

        var employeeDeptJoin = employees
            .Join(departments,
                  e => e.Department,
                  d => d.Name,
                  (e, d) => new { Employee = e, Department = d })
            .Select(j => new
            {
                j.Employee.Name,
                j.Employee.Salary,
                DepartmentName = j.Department.Name,
                BudgetUtilization = (double)j.Employee.Salary / j.Department.Budget * 100
            });

        Console.WriteLine("\nEmployee Department Join:");
        foreach (var item in employeeDeptJoin)
        {
            Console.WriteLine($"{item.Name} ({item.DepartmentName}): ${item.Salary:N0} ({item.BudgetUtilization:F1}% of budget)");
        }
    }

    // Custom LINQ extension methods
    public static class LinqExtensions
    {
        public static IEnumerable<T> WhereNot<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return source.Where(item => !predicate(item));
        }

        public static IEnumerable<T> TakeRandom<T>(this IEnumerable<T> source, int count)
        {
            Random random = new Random();
            List<T> list = source.ToList();
            return list.OrderBy(x => random.Next()).Take(count);
        }

        public static IEnumerable<TResult> SelectWithIndex<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, int, TResult> selector)
        {
            int index = 0;
            foreach (TSource item in source)
            {
                yield return selector(item, index);
                index++;
            }
        }

        public static T RandomElement<T>(this IEnumerable<T> source)
        {
            List<T> list = source.ToList();
            if (list.Count == 0)
                throw new InvalidOperationException("Sequence contains no elements");

            return list[new Random().Next(list.Count)];
        }
    }
}

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Department { get; set; }
    public decimal Salary { get; set; }
    public DateTime HireDate { get; set; }
}

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Budget { get; set; }
}
```

### LINQ to Different Data Sources

```csharp
public class LinqToDifferentSources
{
    public void DemonstrateLinqToObjects()
    {
        int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        var query = from n in numbers
                   where n % 2 == 0
                   select n * n;

        Console.WriteLine("Even squares: " + string.Join(", ", query));
    }

    public void DemonstrateLinqToXml()
    {
        string xmlData = @"
        <books>
            <book id='1'>
                <title>C# Programming</title>
                <author>John Doe</author>
                <price>29.99</price>
            </book>
            <book id='2'>
                <title>LINQ Mastery</title>
                <author>Jane Smith</author>
                <price>34.99</price>
            </book>
        </books>";

        XDocument doc = XDocument.Parse(xmlData);

        var books = from book in doc.Descendants("book")
                   select new
                   {
                       Id = (int)book.Attribute("id"),
                       Title = (string)book.Element("title"),
                       Author = (string)book.Element("author"),
                       Price = (decimal)book.Element("price")
                   };

        foreach (var book in books)
        {
            Console.WriteLine($"{book.Title} by {book.Author}: ${book.Price}");
        }
    }

    public async Task DemonstrateLinqToEntities()
    {
        // This would typically connect to a database
        // For demonstration, we'll use an in-memory collection

        List<Product> products = await GetProductsFromDatabaseAsync();

        var expensiveProducts = from p in products
                              where p.Price > 100
                              orderby p.Price descending
                              select new
                              {
                                  p.Name,
                                  p.Price,
                                  CategoryName = p.Category?.Name ?? "Uncategorized"
                              };

        foreach (var product in expensiveProducts)
        {
            Console.WriteLine($"{product.Name}: ${product.Price} ({product.CategoryName})");
        }
    }

    private async Task<List<Product>> GetProductsFromDatabaseAsync()
    {
        // Simulate database call
        await Task.Delay(100);

        return new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Price = 999.99m, Category = new Category { Name = "Electronics" } },
            new Product { Id = 2, Name = "Book", Price = 19.99m, Category = new Category { Name = "Education" } },
            new Product { Id = 3, Name = "Monitor", Price = 299.99m, Category = new Category { Name = "Electronics" } }
        };
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public Category Category { get; set; }
}

public class Category
{
    public string Name { get; set; }
}
```

## Delegates and Events

### Advanced Delegate Patterns

```csharp
// Generic delegate types
public delegate TResult Func<in T, out TResult>(T arg);
public delegate void Action<in T>(T arg);
public delegate void Action<in T1, in T2>(T1 arg1, T2 arg2);

// Custom delegate
public delegate void PriceChangedHandler(object sender, PriceChangedEventArgs e);

public class PriceChangedEventArgs : EventArgs
{
    public decimal OldPrice { get; }
    public decimal NewPrice { get; }
    public DateTime Timestamp { get; }

    public PriceChangedEventArgs(decimal oldPrice, decimal newPrice)
    {
        OldPrice = oldPrice;
        NewPrice = newPrice;
        Timestamp = DateTime.Now;
    }
}

public class Stock
{
    private decimal price;
    private readonly List<PriceChangedHandler> priceChangedHandlers = new List<PriceChangedHandler>();

    public string Symbol { get; }
    public decimal Price
    {
        get => price;
        set
        {
            if (value != price)
            {
                decimal oldPrice = price;
                price = value;
                OnPriceChanged(oldPrice, price);
            }
        }
    }

    public Stock(string symbol, decimal initialPrice)
    {
        Symbol = symbol;
        price = initialPrice;
    }

    // Event using custom delegate
    public event PriceChangedHandler PriceChanged
    {
        add
        {
            if (value != null)
            {
                priceChangedHandlers.Add(value);
                Console.WriteLine($"Handler added for {Symbol}");
            }
        }
        remove
        {
            if (value != null)
            {
                priceChangedHandlers.Remove(value);
                Console.WriteLine($"Handler removed for {Symbol}");
            }
        }
    }

    protected virtual void OnPriceChanged(decimal oldPrice, decimal newPrice)
    {
        PriceChangedEventArgs args = new PriceChangedEventArgs(oldPrice, newPrice);

        // Invoke all handlers
        foreach (PriceChangedHandler handler in priceChangedHandlers)
        {
            try
            {
                handler(this, args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in price change handler: {ex.Message}");
            }
        }
    }

    // Method to simulate price changes
    public async Task SimulatePriceChangesAsync()
    {
        Random random = new Random();

        for (int i = 0; i < 10; i++)
        {
            decimal change = (decimal)(random.NextDouble() * 20 - 10); // -10 to +10
            Price = Math.Max(0, Price + change);

            await Task.Delay(500);
        }
    }
}

public class StockMonitor
{
    private readonly Dictionary<string, Stock> watchedStocks = new Dictionary<string, Stock>();

    public void WatchStock(Stock stock)
    {
        if (!watchedStocks.ContainsKey(stock.Symbol))
        {
            stock.PriceChanged += OnStockPriceChanged;
            watchedStocks[stock.Symbol] = stock;
            Console.WriteLine($"Now watching {stock.Symbol}");
        }
    }

    public void UnwatchStock(string symbol)
    {
        if (watchedStocks.TryGetValue(symbol, out Stock stock))
        {
            stock.PriceChanged -= OnStockPriceChanged;
            watchedStocks.Remove(symbol);
            Console.WriteLine($"Stopped watching {symbol}");
        }
    }

    private void OnStockPriceChanged(object sender, PriceChangedEventArgs e)
    {
        Stock stock = (Stock)sender;
        decimal change = e.NewPrice - e.OldPrice;
        string direction = change > 0 ? "UP" : "DOWN";

        Console.WriteLine($"[{e.Timestamp:HH:mm:ss}] {stock.Symbol}: ${e.OldPrice:N2} -> ${e.NewPrice:N2} ({direction} ${Math.Abs(change):N2})");
    }
}

public class DelegateExamples
{
    public async Task DemonstrateDelegatesAndEvents()
    {
        // Create stock and monitor
        Stock appleStock = new Stock("AAPL", 150.00m);
        StockMonitor monitor = new StockMonitor();

        // Subscribe to price changes
        monitor.WatchStock(appleStock);

        // Simulate price changes
        await appleStock.SimulatePriceChangesAsync();

        // Unsubscribe
        monitor.UnwatchStock("AAPL");
    }

    // Using built-in delegates
    public void DemonstrateBuiltInDelegates()
    {
        List<string> names = new List<string> { "Alice", "Bob", "Charlie", "Diana" };

        // Action delegate (no return value)
        Action<string> printName = name => Console.WriteLine($"Hello, {name}!");
        names.ForEach(printName);

        // Func delegate (with return value)
        Func<string, int> getLength = name => name.Length;
        var nameLengths = names.Select(getLength);

        Console.WriteLine("Name lengths: " + string.Join(", ", nameLengths));

        // Predicate delegate (returns bool)
        Predicate<string> isLongName = name => name.Length > 4;
        var longNames = names.FindAll(isLongName);

        Console.WriteLine("Long names: " + string.Join(", ", longNames));
    }
}
```

## Reflection and Attributes

### Reflection Basics

```csharp
public class ReflectionExamples
{
    public void DemonstrateReflection()
    {
        Type type = typeof(Person);

        Console.WriteLine($"Type: {type.Name}");
        Console.WriteLine($"Namespace: {type.Namespace}");
        Console.WriteLine($"Assembly: {type.Assembly.FullName}");

        // Get properties
        PropertyInfo[] properties = type.GetProperties();
        Console.WriteLine("\nProperties:");
        foreach (PropertyInfo prop in properties)
        {
            Console.WriteLine($"  {prop.Name}: {prop.PropertyType.Name}");
        }

        // Get methods
        MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
        Console.WriteLine("\nPublic Methods:");
        foreach (MethodInfo method in methods)
        {
            Console.WriteLine($"  {method.Name}");
        }

        // Create instance using reflection
        Person person = (Person)Activator.CreateInstance(type);
        person.Name = "John Doe";
        person.Age = 30;

        Console.WriteLine($"\nCreated person: {person.Name}, Age: {person.Age}");
    }

    public void DemonstrateDynamicInvocation()
    {
        Person person = new Person { Name = "Alice", Age = 25 };

        Type type = person.GetType();
        MethodInfo greetMethod = type.GetMethod("Greet");

        // Invoke method dynamically
        string greeting = (string)greetMethod.Invoke(person, new object[] { "Bob" });
        Console.WriteLine(greeting);

        // Set property dynamically
        PropertyInfo ageProperty = type.GetProperty("Age");
        ageProperty.SetValue(person, 26);

        Console.WriteLine($"Updated age: {person.Age}");
    }

    public void DemonstrateGenericTypeInspection()
    {
        Type listType = typeof(List<string>);
        Type dictType = typeof(Dictionary<int, string>);

        Console.WriteLine($"List<string> is generic: {listType.IsGenericType}");
        Console.WriteLine($"Generic type definition: {listType.GetGenericTypeDefinition()}");

        Type[] typeArgs = listType.GetGenericArguments();
        Console.WriteLine($"Type arguments: {string.Join(", ", typeArgs.Select(t => t.Name))}");

        Console.WriteLine($"\nDictionary<int, string> is generic: {dictType.IsGenericType}");
        Type[] dictTypeArgs = dictType.GetGenericArguments();
        Console.WriteLine($"Type arguments: {string.Join(", ", dictTypeArgs.Select(t => t.Name))}");
    }
}

public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }

    public string Greet(string otherName)
    {
        return $"{Name} says hello to {otherName}!";
    }

    public override string ToString()
    {
        return $"{Name} ({Age} years old)";
    }
}
```

### Custom Attributes

```csharp
// Custom attribute definition
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property,
                AllowMultiple = true, Inherited = true)]
public class DocumentationAttribute : Attribute
{
    public string Description { get; }
    public string Author { get; set; }
    public string Version { get; set; }
    public DateTime LastModified { get; set; }

    public DocumentationAttribute(string description)
    {
        Description = description;
        LastModified = DateTime.Now;
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class ValidationAttribute : Attribute
{
    public int MinLength { get; set; }
    public int MaxLength { get; set; }
    public bool Required { get; set; }
    public string Pattern { get; set; }
}

[AttributeUsage(AttributeTargets.Method)]
public class PerformanceMonitorAttribute : Attribute
{
    public string OperationName { get; set; }
}

// Applying custom attributes
[Documentation("Represents a customer in the system", Author = "Dev Team", Version = "1.0")]
public class Customer
{
    [Validation(Required = true, MinLength = 2, MaxLength = 50)]
    public string FirstName { get; set; }

    [Validation(Required = true, MinLength = 2, MaxLength = 50)]
    public string LastName { get; set; }

    [Validation(Pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    public string Email { get; set; }

    [Documentation("Calculates the customer's full name")]
    [PerformanceMonitor(OperationName = "NameConcatenation")]
    public string GetFullName()
    {
        return $"{FirstName} {LastName}";
    }

    [Documentation("Validates customer data")]
    public bool Validate()
    {
        // Basic validation logic
        return !string.IsNullOrEmpty(FirstName) &&
               !string.IsNullOrEmpty(LastName) &&
               !string.IsNullOrEmpty(Email) &&
               Email.Contains("@");
    }
}

public class AttributeProcessor
{
    public void ProcessTypeAttributes<T>()
    {
        Type type = typeof(T);

        // Get class-level attributes
        DocumentationAttribute[] classDocs = (DocumentationAttribute[])type.GetCustomAttributes(typeof(DocumentationAttribute), true);

        Console.WriteLine($"Class: {type.Name}");
        foreach (DocumentationAttribute doc in classDocs)
        {
            Console.WriteLine($"  Description: {doc.Description}");
            Console.WriteLine($"  Author: {doc.Author}");
            Console.WriteLine($"  Version: {doc.Version}");
            Console.WriteLine($"  Last Modified: {doc.LastModified}");
        }

        // Process property attributes
        PropertyInfo[] properties = type.GetProperties();
        foreach (PropertyInfo prop in properties)
        {
            ValidationAttribute[] validations = (ValidationAttribute[])prop.GetCustomAttributes(typeof(ValidationAttribute), true);

            if (validations.Length > 0)
            {
                Console.WriteLine($"\nProperty: {prop.Name}");
                foreach (ValidationAttribute validation in validations)
                {
                    Console.WriteLine($"  Required: {validation.Required}");
                    Console.WriteLine($"  Min Length: {validation.MinLength}");
                    Console.WriteLine($"  Max Length: {validation.MaxLength}");
                    Console.WriteLine($"  Pattern: {validation.Pattern}");
                }
            }
        }

        // Process method attributes
        MethodInfo[] methods = type.GetMethods();
        foreach (MethodInfo method in methods)
        {
            PerformanceMonitorAttribute[] monitors = (PerformanceMonitorAttribute[])method.GetCustomAttributes(typeof(PerformanceMonitorAttribute), true);

            if (monitors.Length > 0)
            {
                Console.WriteLine($"\nMethod: {method.Name}");
                foreach (PerformanceMonitorAttribute monitor in monitors)
                {
                    Console.WriteLine($"  Operation: {monitor.OperationName}");
                }
            }
        }
    }

    public void DemonstrateAttributeProcessing()
    {
        AttributeProcessor processor = new AttributeProcessor();
        processor.ProcessTypeAttributes<Customer>();
    }
}
```

## Extension Methods

### Creating and Using Extension Methods

```csharp
public static class StringExtensions
{
    // Basic string extensions
    public static bool IsNullOrEmpty(this string str)
    {
        return string.IsNullOrEmpty(str);
    }

    public static bool IsNullOrWhiteSpace(this string str)
    {
        return string.IsNullOrWhiteSpace(str);
    }

    public static string Truncate(this string str, int maxLength)
    {
        if (str == null) return null;
        return str.Length <= maxLength ? str : str.Substring(0, maxLength) + "...";
    }

    public static string ToTitleCase(this string str)
    {
        if (str.IsNullOrEmpty()) return str;

        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
    }

    // Validation extensions
    public static bool IsEmail(this string str)
    {
        if (str.IsNullOrEmpty()) return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(str);
            return addr.Address == str;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsPhoneNumber(this string str)
    {
        if (str.IsNullOrEmpty()) return false;

        // Simple phone number validation (US format)
        return System.Text.RegularExpressions.Regex.IsMatch(str, @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$");
    }

    public static bool IsCreditCardNumber(this string str)
    {
        if (str.IsNullOrEmpty()) return false;

        // Remove spaces and dashes
        str = str.Replace(" ", "").Replace("-", "");

        // Check if all digits
        if (!str.All(char.IsDigit)) return false;

        // Basic length check (13-19 digits for most cards)
        if (str.Length < 13 || str.Length > 19) return false;

        // Luhn algorithm for validation
        return IsValidLuhn(str);
    }

    private static bool IsValidLuhn(string cardNumber)
    {
        int sum = 0;
        bool alternate = false;

        for (int i = cardNumber.Length - 1; i >= 0; i--)
        {
            int digit = cardNumber[i] - '0';

            if (alternate)
            {
                digit *= 2;
                if (digit > 9)
                    digit -= 9;
            }

            sum += digit;
            alternate = !alternate;
        }

        return sum % 10 == 0;
    }
}

public static class CollectionExtensions
{
    // Generic collection extensions
    public static T RandomElement<T>(this IEnumerable<T> source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        List<T> list = source.ToList();
        if (list.Count == 0)
            throw new InvalidOperationException("Sequence contains no elements");

        return list[new Random().Next(list.Count)];
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        List<T> list = source.ToList();
        Random rng = new Random();

        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }

        return list;
    }

    public static IEnumerable<T> TakeRandom<T>(this IEnumerable<T> source, int count)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (count <= 0) return Enumerable.Empty<T>();

        return source.Shuffle().Take(count);
    }

    public static bool IsEmpty<T>(this IEnumerable<T> source)
    {
        return !source.Any();
    }

    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (action == null)
            throw new ArgumentNullException(nameof(action));

        foreach (T item in source)
        {
            action(item);
        }
    }
}

public class ExtensionMethodDemo
{
    public void DemonstrateExtensions()
    {
        // String extensions
        string email = "user@example.com";
        string phone = "555-123-4567";
        string cardNumber = "4111-1111-1111-1111";
        string longText = "This is a very long text that needs to be truncated";

        Console.WriteLine($"Email valid: {email.IsEmail()}");
        Console.WriteLine($"Phone valid: {phone.IsPhoneNumber()}");
        Console.WriteLine($"Card valid: {cardNumber.IsCreditCardNumber()}");
        Console.WriteLine($"Truncated: {longText.Truncate(20)}");
        Console.WriteLine($"Title case: {"hello world".ToTitleCase()}");

        // Collection extensions
        List<int> numbers = Enumerable.Range(1, 10).ToList();

        Console.WriteLine($"Random number: {numbers.RandomElement()}");
        Console.WriteLine($"Random selection: {string.Join(", ", numbers.TakeRandom(3))}");
        Console.WriteLine($"Is empty: {numbers.IsEmpty()}");

        // Custom ForEach
        numbers.ForEach(n => Console.Write($"{n} "));
        Console.WriteLine();
    }
}
```

## Records and Modern C# Features

### Records (Immutable Reference Types)

```csharp
// Simple record
public record PersonRecord(string FirstName, string LastName, int Age);

// Record with additional members
public record EmployeeRecord(string FirstName, string LastName, int Age, string Department)
{
    // Additional properties
    public string FullName => $"{FirstName} {LastName}";
    public bool IsAdult => Age >= 18;

    // Custom constructor
    public EmployeeRecord(string firstName, string lastName, int age)
        : this(firstName, lastName, age, "General")
    {
    }

    // Method
    public string GetDisplayInfo()
    {
        return $"{FullName}, Age: {Age}, Department: {Department}";
    }
}

// Record with mutable properties
public record MutablePerson
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}

public class RecordExamples
{
    public void DemonstrateRecords()
    {
        // Creating records
        PersonRecord person1 = new PersonRecord("John", "Doe", 30);
        PersonRecord person2 = new PersonRecord("John", "Doe", 30);

        Console.WriteLine($"Person 1: {person1}");
        Console.WriteLine($"Person 2: {person2}");
        Console.WriteLine($"Are equal: {person1 == person2}"); // True (structural equality)

        // With-expression for creating modified copies
        PersonRecord olderPerson = person1 with { Age = 31 };
        Console.WriteLine($"Older person: {olderPerson}");

        // Employee record
        EmployeeRecord emp1 = new EmployeeRecord("Alice", "Smith", 28, "Engineering");
        EmployeeRecord emp2 = new EmployeeRecord("Bob", "Johnson", 35);

        Console.WriteLine(emp1.GetDisplayInfo());
        Console.WriteLine(emp2.GetDisplayInfo());

        // Deconstruction
        var (firstName, lastName, age, dept) = emp1;
        Console.WriteLine($"Deconstructed: {firstName} {lastName}, {age}, {dept}");

        // Mutable record
        MutablePerson mutablePerson = new MutablePerson
        {
            FirstName = "Charlie",
            LastName = "Brown",
            Age = 25
        };

        Console.WriteLine($"Mutable: {mutablePerson.FullName}");
        mutablePerson.Age = 26;
        Console.WriteLine($"After change: {mutablePerson.FullName}, Age: {mutablePerson.Age}");
    }
}
```

### Pattern Matching (Advanced)

```csharp
public class PatternMatchingExamples
{
    public void DemonstratePatternMatching()
    {
        object[] objects = {
            "Hello World",
            42,
            new PersonRecord("Alice", "Smith", 30),
            new List<int> { 1, 2, 3 },
            null,
            3.14
        };

        foreach (object obj in objects)
        {
            string description = GetObjectDescription(obj);
            Console.WriteLine(description);
        }
    }

    private string GetObjectDescription(object obj)
    {
        return obj switch
        {
            // Type pattern
            string s => $"String: '{s}' (Length: {s.Length})",

            // Constant pattern
            int n when n < 0 => $"Negative integer: {n}",
            int n => $"Positive integer: {n}",

            // Property pattern
            PersonRecord { Age: >= 18 } p => $"Adult: {p.FirstName} {p.LastName}, Age: {p.Age}",

            // List pattern
            List<int> { Count: 0 } => "Empty integer list",
            List<int> { Count: 1 } list => $"Single item list: [{list[0]}]",
            List<int> list => $"Integer list with {list.Count} items",

            // Relational pattern
            double d when d > 0 => $"Positive double: {d}",
            double d => $"Double: {d}",

            // Null pattern
            null => "Null value",

            // Discard pattern (catch-all)
            _ => $"Unknown type: {obj.GetType().Name}"
        };
    }

    // Pattern matching in methods
    public decimal CalculateDiscount(object customer)
    {
        return customer switch
        {
            // Complex property patterns
            PersonRecord { Age: < 18 } => 0.0m,  // No discount for minors
            PersonRecord { Age: >= 65 } => 0.2m, // 20% for seniors
            PersonRecord { Age: >= 18 } => 0.1m, // 10% for adults

            // Type patterns with when clauses
            string vip when vip.Contains("VIP") => 0.3m, // 30% for VIPs

            // Default
            _ => 0.0m
        };
    }

    // Pattern matching with tuples
    public string ClassifyPoint((int x, int y) point)
    {
        return point switch
        {
            (0, 0) => "Origin",
            (var x, 0) => $"On X-axis at {x}",
            (0, var y) => $"On Y-axis at {y}",
            (var x, var y) when x > 0 && y > 0 => "First quadrant",
            (var x, var y) when x < 0 && y > 0 => "Second quadrant",
            (var x, var y) when x < 0 && y < 0 => "Third quadrant",
            (var x, var y) when x > 0 && y < 0 => "Fourth quadrant",
            _ => "Unknown"
        };
    }
}
```

### Tuples and Deconstruction

```csharp
public class TupleExamples
{
    public void DemonstrateTuples()
    {
        // Named tuples
        (string name, int age, string city) person = ("Alice", 30, "New York");
        Console.WriteLine($"{person.name} is {person.age} years old and lives in {person.city}");

        // Tuple as return type
        var result = CalculateStatistics(new[] { 1, 2, 3, 4, 5 });
        Console.WriteLine($"Sum: {result.sum}, Average: {result.average:F2}, Count: {result.count}");

        // Deconstruction
        var (min, max) = FindMinMax(new[] { 3, 1, 4, 1, 5, 9, 2, 6 });
        Console.WriteLine($"Min: {min}, Max: {max}");

        // Discarding values
        var (_, _, third) = GetTopThreeScores();
        Console.WriteLine($"Third highest score: {third}");
    }

    private (int sum, double average, int count) CalculateStatistics(int[] numbers)
    {
        int sum = numbers.Sum();
        double average = numbers.Average();
        int count = numbers.Length;

        return (sum, average, count);
    }

    private (int min, int max) FindMinMax(int[] numbers)
    {
        return (numbers.Min(), numbers.Max());
    }

    private (int first, int second, int third) GetTopThreeScores()
    {
        return (95, 87, 82);
    }

    // Tuples in LINQ
    public void DemonstrateTupleLinq()
    {
        var people = new[]
        {
            ("Alice", 30, "Engineer"),
            ("Bob", 25, "Designer"),
            ("Charlie", 35, "Manager")
        };

        var adults = people
            .Where(p => p.Item2 >= 18)
            .Select(p => (p.Item1, p.Item2, p.Item3))
            .OrderByDescending(p => p.Item2);

        foreach (var (name, age, profession) in adults)
        {
            Console.WriteLine($"{name}: {age} years old, {profession}");
        }
    }
}
```

### Local Functions

```csharp
public class LocalFunctionExamples
{
    public void DemonstrateLocalFunctions()
    {
        // Local function for validation
        bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // Local function with closure
        int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        IEnumerable<int> FilterAndTransform(Func<int, bool> predicate, Func<int, int> transformer)
        {
            foreach (int num in numbers)
            {
                if (predicate(num))
                {
                    yield return transformer(num);
                }
            }
        }

        // Using local functions
        var evenSquares = FilterAndTransform(n => n % 2 == 0, n => n * n);
        Console.WriteLine("Even squares: " + string.Join(", ", evenSquares));

        var oddCubes = FilterAndTransform(n => n % 2 != 0, n => n * n * n);
        Console.WriteLine("Odd cubes: " + string.Join(", ", oddCubes));

        // Recursive local function
        long Factorial(int n)
        {
            if (n <= 1) return 1;
            return n * Factorial(n - 1);
        }

        Console.WriteLine($"Factorial of 5: {Factorial(5)}");

        // Test email validation
        string[] emails = { "valid@example.com", "invalid-email", "", "another@valid.com" };
        foreach (string email in emails)
        {
            Console.WriteLine($"{email}: {IsValidEmail(email)}");
        }
    }

    // Iterator with local function
    public IEnumerable<int> GenerateFibonacci(int count)
    {
        if (count <= 0) yield break;

        yield return 0;
        if (count == 1) yield break;

        yield return 1;
        if (count == 2) yield break;

        int a = 0, b = 1;
        for (int i = 3; i <= count; i++)
        {
            int c = a + b;
            yield return c;
            a = b;
            b = c;
        }
    }
}
```

## Comprehensive Examples

### Example 1: Async Data Processor

```csharp
public class AsyncDataProcessor
{
    private readonly HttpClient httpClient;
    private readonly SemaphoreSlim semaphore;

    public AsyncDataProcessor()
    {
        httpClient = new HttpClient();
        semaphore = new SemaphoreSlim(5); // Limit concurrent requests
    }

    public async Task ProcessMultipleDataSourcesAsync(IEnumerable<string> urls)
    {
        var tasks = urls.Select(url => ProcessDataSourceAsync(url));
        var results = await Task.WhenAll(tasks);

        Console.WriteLine($"Processed {results.Length} data sources");
        Console.WriteLine($"Successful: {results.Count(r => r.Success)}");
        Console.WriteLine($"Failed: {results.Count(r => !r.Success)}");
    }

    private async Task<ProcessingResult> ProcessDataSourceAsync(string url)
    {
        await semaphore.WaitAsync();

        try
        {
            using (var response = await httpClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    int wordCount = CountWords(content);

                    return new ProcessingResult
                    {
                        Url = url,
                        Success = true,
                        WordCount = wordCount,
                        ProcessingTime = DateTime.Now
                    };
                }
                else
                {
                    return new ProcessingResult
                    {
                        Url = url,
                        Success = false,
                        ErrorMessage = $"HTTP {response.StatusCode}"
                    };
                }
            }
        }
        catch (Exception ex)
        {
            return new ProcessingResult
            {
                Url = url,
                Success = false,
                ErrorMessage = ex.Message
            };
        }
        finally
        {
            semaphore.Release();
        }
    }

    private int CountWords(string text)
    {
        if (string.IsNullOrEmpty(text)) return 0;

        // Simple word counting (split by whitespace and punctuation)
        var words = text.Split(new[] { ' ', '\t', '\n', '\r', '.', ',', '!', '?' },
                              StringSplitOptions.RemoveEmptyEntries);
        return words.Length;
    }

    public void Dispose()
    {
        httpClient?.Dispose();
        semaphore?.Dispose();
    }
}

public class ProcessingResult
{
    public string Url { get; set; }
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public int WordCount { get; set; }
    public DateTime ProcessingTime { get; set; }
}

public class DataProcessingDemo
{
    public async Task DemonstrateAsyncProcessing()
    {
        using (var processor = new AsyncDataProcessor())
        {
            string[] urls = {
                "https://www.example.com",
                "https://httpbin.org/html",
                "https://jsonplaceholder.typicode.com/posts/1"
            };

            await processor.ProcessMultipleDataSourcesAsync(urls);
        }
    }
}
```

### Example 2: Dynamic Configuration System

```csharp
public class ConfigurationManager
{
    private readonly Dictionary<string, object> settings = new Dictionary<string, object>();
    private readonly string configFilePath;

    public ConfigurationManager(string configFilePath = "config.json")
    {
        this.configFilePath = configFilePath;
        LoadConfiguration();
    }

    public T GetSetting<T>(string key, T defaultValue = default)
    {
        if (settings.TryGetValue(key, out object value))
        {
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }
        return defaultValue;
    }

    public void SetSetting<T>(string key, T value)
    {
        settings[key] = value;
        SaveConfiguration();
    }

    public bool HasSetting(string key)
    {
        return settings.ContainsKey(key);
    }

    public IEnumerable<string> GetAllKeys()
    {
        return settings.Keys;
    }

    public void RemoveSetting(string key)
    {
        if (settings.Remove(key))
        {
            SaveConfiguration();
        }
    }

    public void ClearAllSettings()
    {
        settings.Clear();
        SaveConfiguration();
    }

    private void LoadConfiguration()
    {
        try
        {
            if (File.Exists(configFilePath))
            {
                string jsonContent = File.ReadAllText(configFilePath);
                var loadedSettings = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonContent);

                if (loadedSettings != null)
                {
                    foreach (var kvp in loadedSettings)
                    {
                        settings[kvp.Key] = kvp.Value;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading configuration: {ex.Message}");
        }
    }

    private void SaveConfiguration()
    {
        try
        {
            string jsonContent = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(configFilePath, jsonContent);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving configuration: {ex.Message}");
        }
    }

    // Dynamic property access using reflection
    public object GetValue(string propertyName)
    {
        return settings.TryGetValue(propertyName, out object value) ? value : null;
    }

    public void SetValue(string propertyName, object value)
    {
        settings[propertyName] = value;
        SaveConfiguration();
    }

    // LINQ-based queries
    public IEnumerable<KeyValuePair<string, object>> FindSettings(Func<KeyValuePair<string, object>, bool> predicate)
    {
        return settings.Where(predicate);
    }

    public IEnumerable<string> GetKeysByType(Type type)
    {
        return settings.Where(kvp => kvp.Value?.GetType() == type).Select(kvp => kvp.Key);
    }

    public void PrintConfiguration()
    {
        Console.WriteLine("Current Configuration:");
        Console.WriteLine("======================");

        foreach (var setting in settings.OrderBy(s => s.Key))
        {
            Console.WriteLine($"{setting.Key}: {setting.Value} ({setting.Value?.GetType().Name ?? "null"})");
        }

        Console.WriteLine($"\nTotal settings: {settings.Count}");
    }
}

public class ConfigurationDemo
{
    public void DemonstrateConfiguration()
    {
        var config = new ConfigurationManager();

        // Set various types of settings
        config.SetSetting("AppName", "MyApplication");
        config.SetSetting("Version", "1.2.3");
        config.SetSetting("MaxConnections", 100);
        config.SetSetting("DebugMode", true);
        config.SetSetting("Timeout", 30.5);

        // Retrieve settings with type safety
        string appName = config.GetSetting<string>("AppName", "DefaultApp");
        int maxConnections = config.GetSetting<int>("MaxConnections", 50);
        bool debugMode = config.GetSetting<bool>("DebugMode", false);

        Console.WriteLine($"Application: {appName}");
        Console.WriteLine($"Max Connections: {maxConnections}");
        Console.WriteLine($"Debug Mode: {debugMode}");

        // Dynamic access
        config.SetValue("DynamicSetting", "DynamicValue");
        object dynamicValue = config.GetValue("DynamicSetting");
        Console.WriteLine($"Dynamic value: {dynamicValue}");

        // LINQ queries
        var stringSettings = config.FindSettings(kvp => kvp.Value is string);
        Console.WriteLine("\nString settings:");
        foreach (var setting in stringSettings)
        {
            Console.WriteLine($"  {setting.Key}: {setting.Value}");
        }

        var numericKeys = config.GetKeysByType(typeof(int));
        Console.WriteLine($"\nNumeric setting keys: {string.Join(", ", numericKeys)}");

        config.PrintConfiguration();
    }
}
```

## Exercises

1. **Async Web Scraper**
   - Create an async web scraper that downloads multiple web pages concurrently
   - Implement retry logic for failed requests
   - Add progress reporting and cancellation support
   - Parse HTML content and extract specific data

2. **LINQ Data Analyzer**
   - Build a data analysis tool using LINQ
   - Support multiple data sources (CSV, JSON, XML)
   - Implement complex queries with grouping and aggregation
   - Add export functionality for results

3. **Plugin System with Reflection**
   - Create a plugin system using reflection
   - Implement plugin discovery and loading
   - Add custom attributes for plugin metadata
   - Support dependency injection for plugins

4. **Event-Driven Notification System**
   - Build an event-driven notification system
   - Support multiple notification channels (email, SMS, push)
   - Implement event filtering and routing
   - Add retry logic and dead letter queues

5. **Dynamic Object Mapper**
   - Create a dynamic object mapper using reflection
   - Support mapping between different object types
   - Implement custom mapping rules with attributes
   - Add validation and transformation capabilities

6. **Advanced LINQ Extensions**
   - Create a library of useful LINQ extension methods
   - Implement batching, pagination, and caching
   - Add statistical and mathematical operations
   - Support async LINQ operations

7. **Configuration Builder with Fluent API**
   - Build a fluent configuration API
   - Support validation and type conversion
   - Implement hierarchical configurations
   - Add environment-specific overrides

8. **Expression Tree Builder**
   - Create a tool for building dynamic LINQ expressions
   - Support complex query building at runtime
   - Implement query optimization
   - Add support for custom operators

Modern C# features like async/await, LINQ, reflection, and attributes provide powerful capabilities for building sophisticated applications. Mastering these concepts enables you to write more expressive, maintainable, and performant code. The key is understanding when and how to apply each feature appropriately in your applications.
