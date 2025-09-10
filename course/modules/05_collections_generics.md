# Module 5 — Collections & Generics (Managing Data Efficiently)

## Overview

Collections are fundamental data structures that allow you to store and manipulate groups of related objects. Generics provide type safety and performance benefits by allowing you to create reusable code that works with different types. This module explores arrays, lists, dictionaries, and other collection types, along with LINQ (Language Integrated Query) for powerful data manipulation.

## Topics Covered

- Arrays (fixed-size collections)
- List<T> and other dynamic collections
- Dictionary<TKey, TValue> and hash tables
- HashSet<T> and set operations
- Stack<T> and Queue<T>
- Generic constraints and type parameters
- LINQ query syntax and method syntax
- Lambda expressions with collections
- Custom generic classes and methods
- Performance considerations
- Collection best practices

## Arrays

### Declaration and Initialization

```csharp
// Single-dimensional arrays
int[] numbers = new int[5];           // Size known at runtime
int[] primes = { 2, 3, 5, 7, 11 };    // Initialized with values
string[] names = new string[] { "Alice", "Bob", "Charlie" };

// Multi-dimensional arrays
int[,] matrix = new int[3, 4];        // 3x4 matrix
int[,,] cube = new int[2, 3, 4];      // 3D array

// Jagged arrays (arrays of arrays)
int[][] jaggedArray = new int[3][];
jaggedArray[0] = new int[] { 1, 2 };
jaggedArray[1] = new int[] { 3, 4, 5 };
jaggedArray[2] = new int[] { 6 };
```

### Array Operations

```csharp
int[] numbers = { 10, 20, 30, 40, 50 };

// Accessing elements
int first = numbers[0];        // 10
int last = numbers[^1];        // 50 (C# 8.0+ index from end)

// Iterating through arrays
foreach (int number in numbers)
{
    Console.WriteLine(number);
}

// Finding elements
int index = Array.IndexOf(numbers, 30);    // 2
bool contains = Array.Exists(numbers, n => n > 25); // true

// Sorting and reversing
Array.Sort(numbers);              // {10, 20, 30, 40, 50}
Array.Reverse(numbers);           // {50, 40, 30, 20, 10}

// Copying arrays
int[] copy = new int[numbers.Length];
Array.Copy(numbers, copy, numbers.Length);

// Converting to other collections
List<int> list = new List<int>(numbers);
```

## List<T> - Dynamic Arrays

### Basic Operations

```csharp
// Creating lists
List<int> numbers = new List<int>();
List<string> names = new List<string> { "Alice", "Bob", "Charlie" };

// Adding elements
numbers.Add(10);
numbers.AddRange(new[] { 20, 30, 40 });
numbers.Insert(1, 15);           // Insert at index 1

// Accessing elements
int first = numbers[0];
int last = numbers[^1];           // C# 8.0+

// Removing elements
numbers.Remove(20);               // Remove first occurrence of 20
numbers.RemoveAt(0);              // Remove element at index 0
numbers.RemoveAll(n => n > 25);   // Remove all elements > 25

// Checking contents
bool contains = numbers.Contains(30);
bool isEmpty = numbers.Count == 0;

// Capacity management
numbers.TrimExcess();             // Reduce capacity to actual count
numbers.Clear();                  // Remove all elements
```

### Advanced List Operations

```csharp
List<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

// Finding elements
int firstEven = numbers.Find(n => n % 2 == 0);           // 2
List<int> allEven = numbers.FindAll(n => n % 2 == 0);     // {2, 4, 6, 8, 10}
int evenIndex = numbers.FindIndex(n => n % 2 == 0);       // 1

// Transforming elements
List<int> doubled = numbers.Select(n => n * 2).ToList();   // {2, 4, 6, 8, 10, 12, 14, 16, 18, 20}
List<string> strings = numbers.Select(n => $"Number: {n}").ToList();

// Sorting
numbers.Sort();                    // Ascending: {1, 2, 3, 4, 5, 6, 7, 8, 9, 10}
numbers.Sort((a, b) => b.CompareTo(a)); // Descending: {10, 9, 8, 7, 6, 5, 4, 3, 2, 1}

// Converting
int[] array = numbers.ToArray();
HashSet<int> set = new HashSet<int>(numbers);
```

## Dictionary<TKey, TValue> - Key-Value Pairs

### Basic Operations

```csharp
// Creating dictionaries
Dictionary<string, int> ages = new Dictionary<string, int>();
Dictionary<int, string> errorCodes = new Dictionary<int, string>
{
    { 404, "Not Found" },
    { 500, "Internal Server Error" },
    { 200, "OK" }
};

// Adding and updating
ages["Alice"] = 25;
ages["Bob"] = 30;
ages.Add("Charlie", 35);

// Safe adding (won't throw if key exists)
if (!ages.ContainsKey("Alice"))
{
    ages.Add("Alice", 26);
}

// TryAdd (C# 8.0+)
ages.TryAdd("David", 28); // Returns true if added, false if key exists

// Accessing values
int aliceAge = ages["Alice"];
bool hasBob = ages.TryGetValue("Bob", out int bobAge);

// Iterating
foreach (var pair in ages)
{
    Console.WriteLine($"{pair.Key}: {pair.Value}");
}

// Keys and values separately
ICollection<string> keys = ages.Keys;
ICollection<int> values = ages.Values;
```

### Advanced Dictionary Operations

```csharp
Dictionary<string, List<string>> phoneBook = new Dictionary<string, List<string>>();

// Adding multiple phone numbers per person
phoneBook["Alice"] = new List<string> { "555-0101", "555-0102" };
phoneBook["Bob"] = new List<string> { "555-0201" };

// Safe access with null coalescing
List<string> aliceNumbers = phoneBook.GetValueOrDefault("Alice") ?? new List<string>();
List<string> charlieNumbers = phoneBook.GetValueOrDefault("Charlie", new List<string>());

// LINQ operations on dictionaries
var adults = ages.Where(pair => pair.Value >= 18)
                 .ToDictionary(pair => pair.Key, pair => pair.Value);

var averageAge = ages.Values.Average();
var oldestPerson = ages.OrderByDescending(pair => pair.Value).First();

// Grouping
var ageGroups = ages.GroupBy(pair => pair.Value / 10 * 10)
                    .ToDictionary(g => g.Key, g => g.ToList());
```

## HashSet<T> - Unique Collections

### Basic Operations

```csharp
// Creating sets
HashSet<int> numbers = new HashSet<int> { 1, 2, 3, 4, 5 };
HashSet<string> fruits = new HashSet<string>();

// Adding elements
fruits.Add("Apple");
fruits.Add("Banana");
fruits.Add("Apple"); // Duplicate, won't be added

// Set operations
HashSet<int> evenNumbers = new HashSet<int> { 2, 4, 6, 8 };
HashSet<int> multiplesOf3 = new HashSet<int> { 3, 6, 9 };

// Union
HashSet<int> union = new HashSet<int>(numbers);
union.UnionWith(evenNumbers); // {1, 2, 3, 4, 5, 6, 8}

// Intersection
HashSet<int> intersection = new HashSet<int>(evenNumbers);
intersection.IntersectWith(multiplesOf3); // {6}

// Difference
HashSet<int> difference = new HashSet<int>(numbers);
difference.ExceptWith(evenNumbers); // {1, 3, 5}

// Symmetric difference
HashSet<int> symmetricDiff = new HashSet<int>(numbers);
symmetricDiff.SymmetricExceptWith(evenNumbers); // {1, 3, 5, 8}
```

## Stack<T> and Queue<T>

### Stack Operations (LIFO - Last In, First Out)

```csharp
Stack<string> browserHistory = new Stack<string>();

// Navigating pages
browserHistory.Push("Home");
browserHistory.Push("Products");
browserHistory.Push("Contact");

// Going back
string currentPage = browserHistory.Pop();     // "Contact"
currentPage = browserHistory.Pop();            // "Products"

// Peeking without removing
string nextPage = browserHistory.Peek();       // "Home"
bool isEmpty = browserHistory.Count == 0;      // false

// Checking contents
bool hasHome = browserHistory.Contains("Home"); // true
```

### Queue Operations (FIFO - First In, First Out)

```csharp
Queue<string> printQueue = new Queue<string>();

// Adding print jobs
printQueue.Enqueue("Document1.pdf");
printQueue.Enqueue("Document2.docx");
printQueue.Enqueue("Image1.jpg");

// Processing jobs
string nextJob = printQueue.Dequeue();         // "Document1.pdf"
nextJob = printQueue.Dequeue();                // "Document2.docx"

// Peeking without removing
string upcomingJob = printQueue.Peek();        // "Image1.jpg"

// Queue management
int jobCount = printQueue.Count;
printQueue.Clear();                            // Remove all jobs
```

## Generics Fundamentals

### Generic Classes

```csharp
public class Pair<T1, T2>
{
    public T1 First { get; set; }
    public T2 Second { get; set; }

    public Pair(T1 first, T2 second)
    {
        First = first;
        Second = second;
    }

    public void Swap()
    {
        (First, Second) = (Second, First);
    }
}

// Usage
var intStringPair = new Pair<int, string>(42, "Answer");
var stringBoolPair = new Pair<string, bool>("IsReady", true);
```

### Generic Methods

```csharp
public static class CollectionUtils
{
    // Generic method to find maximum
    public static T FindMax<T>(IEnumerable<T> collection) where T : IComparable<T>
    {
        if (collection == null || !collection.Any())
            throw new ArgumentException("Collection cannot be null or empty");

        T max = collection.First();
        foreach (T item in collection)
        {
            if (item.CompareTo(max) > 0)
                max = item;
        }
        return max;
    }

    // Generic method to swap elements
    public static void Swap<T>(ref T a, ref T b)
    {
        T temp = a;
        a = b;
        b = temp;
    }

    // Generic method with multiple constraints
    public static T CreateInstance<T>() where T : class, new()
    {
        return new T();
    }
}
```

### Generic Constraints

```csharp
// Interface constraint
public class Repository<T> where T : IEntity
{
    // T must implement IEntity
}

// Base class constraint
public class AnimalProcessor<T> where T : Animal
{
    // T must inherit from Animal
}

// Value type constraint
public class NumberProcessor<T> where T : struct
{
    // T must be a value type
}

// Reference type constraint
public class ObjectProcessor<T> where T : class
{
    // T must be a reference type
}

// Constructor constraint
public class Factory<T> where T : new()
{
    // T must have a parameterless constructor
}

// Multiple constraints
public class AdvancedProcessor<T> where T : class, IComparable<T>, new()
{
    // T must be reference type, comparable, and have parameterless constructor
}
```

## LINQ (Language Integrated Query)

### LINQ Method Syntax

```csharp
List<Student> students = GetStudents();

var excellentStudents = students
    .Where(s => s.Grade >= 90)
    .OrderByDescending(s => s.Grade)
    .ThenBy(s => s.Name)
    .Select(s => new { s.Name, s.Grade })
    .ToList();

var averageGrade = students.Average(s => s.Grade);
var gradeGroups = students.GroupBy(s => s.Grade / 10 * 10);
var topStudent = students.OrderByDescending(s => s.Grade).FirstOrDefault();
```

### LINQ Query Syntax

```csharp
// Basic query
var highPerformers = from student in students
                    where student.Grade >= 85
                    orderby student.Grade descending, student.Name
                    select student;

// Grouping
var gradeDistribution = from student in students
                       group student by student.Grade / 10 * 10 into gradeGroup
                       orderby gradeGroup.Key
                       select new
                       {
                           GradeRange = gradeGroup.Key,
                           Count = gradeGroup.Count(),
                           Students = gradeGroup.ToList()
                       };

// Joining
var enrollments = from student in students
                 join course in courses on student.Id equals course.StudentId
                 select new
                 {
                     StudentName = student.Name,
                     CourseName = course.Name,
                     Grade = course.Grade
                 };
```

### LINQ Operators

```csharp
List<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

// Filtering
var evenNumbers = numbers.Where(n => n % 2 == 0);
var positiveNumbers = numbers.Where(n => n > 0);

// Projection
var squares = numbers.Select(n => n * n);
var numberStrings = numbers.Select(n => $"Number: {n}");

// Ordering
var ascending = numbers.OrderBy(n => n);
var descending = numbers.OrderByDescending(n => n);

// Aggregation
int sum = numbers.Sum();
double average = numbers.Average();
int max = numbers.Max();
int min = numbers.Min();
int count = numbers.Count();

// Set operations
List<int> otherNumbers = new List<int> { 5, 6, 7, 8, 9, 10, 11, 12 };
var union = numbers.Union(otherNumbers);
var intersection = numbers.Intersect(otherNumbers);
var difference = numbers.Except(otherNumbers);

// Partitioning
var first3 = numbers.Take(3);
var skipFirst3 = numbers.Skip(3);
var page2 = numbers.Skip(5).Take(5);

// Element operations
int first = numbers.First();
int firstEven = numbers.First(n => n % 2 == 0);
int firstOrDefault = numbers.FirstOrDefault(n => n > 100); // 0
bool anyEven = numbers.Any(n => n % 2 == 0); // true
bool allEven = numbers.All(n => n % 2 == 0); // false
```

## Custom Generic Collections

### Generic Stack Implementation

```csharp
public class CustomStack<T>
{
    private List<T> items = new List<T>();

    public void Push(T item)
    {
        items.Add(item);
    }

    public T Pop()
    {
        if (items.Count == 0)
            throw new InvalidOperationException("Stack is empty");

        T item = items[^1];
        items.RemoveAt(items.Count - 1);
        return item;
    }

    public T Peek()
    {
        if (items.Count == 0)
            throw new InvalidOperationException("Stack is empty");

        return items[^1];
    }

    public bool IsEmpty => items.Count == 0;
    public int Count => items.Count;

    public void Clear()
    {
        items.Clear();
    }
}
```

### Generic Repository Pattern

```csharp
public interface IRepository<T> where T : class
{
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    T GetById(int id);
    IEnumerable<T> GetAll();
    IEnumerable<T> Find(Func<T, bool> predicate);
}

public class InMemoryRepository<T> : IRepository<T> where T : class
{
    private readonly List<T> _entities = new List<T>();
    private int _nextId = 1;

    public void Add(T entity)
    {
        // Set ID if entity has Id property
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty != null && idProperty.PropertyType == typeof(int))
        {
            idProperty.SetValue(entity, _nextId++);
        }

        _entities.Add(entity);
    }

    public void Update(T entity)
    {
        // Implementation would depend on how to identify entities
    }

    public void Delete(T entity)
    {
        _entities.Remove(entity);
    }

    public T GetById(int id)
    {
        return _entities.FirstOrDefault(e =>
        {
            var idProperty = e.GetType().GetProperty("Id");
            return idProperty != null && (int)idProperty.GetValue(e) == id;
        });
    }

    public IEnumerable<T> GetAll()
    {
        return _entities;
    }

    public IEnumerable<T> Find(Func<T, bool> predicate)
    {
        return _entities.Where(predicate);
    }
}
```

## Performance Considerations

### Choosing the Right Collection

```csharp
// Fast lookups by key - use Dictionary<TKey, TValue>
Dictionary<string, User> usersByEmail = new Dictionary<string, User>();

// Unique items only - use HashSet<T>
HashSet<string> uniqueNames = new HashSet<string>();

// Ordered collection with fast access by index - use List<T>
List<Order> orders = new List<Order>();

// First-in-first-out processing - use Queue<T>
Queue<PrintJob> printQueue = new Queue<PrintJob>();

// Last-in-first-out processing - use Stack<T>
Stack<UndoAction> undoStack = new Stack<UndoAction>();

// Fixed-size collection - use array
int[] lookupTable = new int[1000];
```

### Capacity Planning

```csharp
// Pre-allocate capacity for better performance
List<int> largeList = new List<int>(10000); // Avoids reallocations

// Dictionary with initial capacity
Dictionary<string, object> cache = new Dictionary<string, object>(1000);

// Trim excess capacity when done adding
largeList.TrimExcess();
```

## Best Practices

### 1. Choose Appropriate Collection Types

```csharp
// Bad: Using List<T> when you need fast lookups
List<User> users = GetUsers();
User user = users.FirstOrDefault(u => u.Email == "test@example.com"); // O(n)

// Good: Use Dictionary for fast lookups
Dictionary<string, User> usersByEmail = GetUsers().ToDictionary(u => u.Email);
User user = usersByEmail["test@example.com"]; // O(1)
```

### 2. Use Read-Only Collections When Appropriate

```csharp
public class DataProcessor
{
    private readonly List<string> _data;

    // Expose read-only view
    public IReadOnlyList<string> Data => _data;

    public DataProcessor()
    {
        _data = new List<string>();
    }

    // Internal modifications allowed
    public void AddData(string item)
    {
        _data.Add(item);
    }
}
```

### 3. Implement IEnumerable<T> for Custom Collections

```csharp
public class CustomCollection<T> : IEnumerable<T>
{
    private readonly List<T> _items = new List<T>();

    public void Add(T item) => _items.Add(item);

    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
```

## Comprehensive Examples

### Example 1: Student Grade Management System

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

public class Student
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Dictionary<string, double> Grades { get; set; } = new Dictionary<string, double>();

    public double GPA => Grades.Values.Average();
}

public class GradeManager
{
    private readonly Dictionary<int, Student> _students = new Dictionary<int, Student>();
    private readonly Dictionary<string, List<double>> _courseGrades = new Dictionary<string, List<double>>();
    private int _nextStudentId = 1;

    public int AddStudent(string name)
    {
        int id = _nextStudentId++;
        _students[id] = new Student { Id = id, Name = name };
        return id;
    }

    public void AddGrade(int studentId, string course, double grade)
    {
        if (!_students.ContainsKey(studentId))
            throw new ArgumentException("Student not found");

        _students[studentId].Grades[course] = grade;

        // Update course statistics
        if (!_courseGrades.ContainsKey(course))
            _courseGrades[course] = new List<double>();

        _courseGrades[course].Add(grade);
    }

    public Student GetStudent(int id) => _students[id];

    public IEnumerable<Student> GetAllStudents() => _students.Values;

    public IEnumerable<Student> GetTopPerformers(int count = 5)
    {
        return _students.Values
            .OrderByDescending(s => s.GPA)
            .Take(count);
    }

    public Dictionary<string, double> GetCourseAverages()
    {
        return _courseGrades.ToDictionary(
            pair => pair.Key,
            pair => pair.Value.Average()
        );
    }

    public IEnumerable<string> GetFailingStudents(string course, double passingGrade = 60)
    {
        return _students.Values
            .Where(s => s.Grades.ContainsKey(course) && s.Grades[course] < passingGrade)
            .Select(s => s.Name);
    }

    public void GenerateReport()
    {
        Console.WriteLine("=== Student Grade Report ===");

        foreach (var student in _students.Values.OrderBy(s => s.Name))
        {
            Console.WriteLine($"{student.Name} (ID: {student.Id}) - GPA: {student.GPA:F2}");

            foreach (var grade in student.Grades)
            {
                Console.WriteLine($"  {grade.Key}: {grade.Value:F1}");
            }
            Console.WriteLine();
        }

        Console.WriteLine("=== Course Averages ===");
        foreach (var courseAvg in GetCourseAverages())
        {
            Console.WriteLine($"{courseAvg.Key}: {courseAvg.Value:F2}");
        }
    }
}

class Program
{
    static void Main()
    {
        var manager = new GradeManager();

        // Add students
        int aliceId = manager.AddStudent("Alice");
        int bobId = manager.AddStudent("Bob");
        int charlieId = manager.AddStudent("Charlie");

        // Add grades
        manager.AddGrade(aliceId, "Math", 95);
        manager.AddGrade(aliceId, "Science", 87);
        manager.AddGrade(aliceId, "English", 92);

        manager.AddGrade(bobId, "Math", 78);
        manager.AddGrade(bobId, "Science", 85);
        manager.AddGrade(bobId, "English", 88);

        manager.AddGrade(charlieId, "Math", 92);
        manager.AddGrade(charlieId, "Science", 79);
        manager.AddGrade(charlieId, "English", 85);

        // Generate report
        manager.GenerateReport();

        // Show top performers
        Console.WriteLine("\n=== Top Performers ===");
        foreach (var student in manager.GetTopPerformers(2))
        {
            Console.WriteLine($"{student.Name}: {student.GPA:F2}");
        }

        // Show failing students in Math
        var failingMath = manager.GetFailingStudents("Math", 80).ToList();
        if (failingMath.Any())
        {
            Console.WriteLine($"\nStudents failing Math: {string.Join(", ", failingMath)}");
        }
    }
}
```

### Example 2: Generic Cache Implementation

```csharp
using System;
using System.Collections.Generic;

public interface ICache<T>
{
    void Set(string key, T value);
    T Get(string key);
    bool TryGet(string key, out T value);
    void Remove(string key);
    void Clear();
    bool ContainsKey(string key);
}

public class MemoryCache<T> : ICache<T>
{
    private readonly Dictionary<string, CacheItem<T>> _cache = new Dictionary<string, CacheItem<T>>();
    private readonly object _lock = new object();

    public void Set(string key, T value)
    {
        lock (_lock)
        {
            _cache[key] = new CacheItem<T>(value);
        }
    }

    public T Get(string key)
    {
        lock (_lock)
        {
            if (_cache.TryGetValue(key, out var item))
            {
                item.LastAccessed = DateTime.Now;
                return item.Value;
            }
            throw new KeyNotFoundException($"Key '{key}' not found in cache");
        }
    }

    public bool TryGet(string key, out T value)
    {
        lock (_lock)
        {
            if (_cache.TryGetValue(key, out var item))
            {
                item.LastAccessed = DateTime.Now;
                value = item.Value;
                return true;
            }
            value = default;
            return false;
        }
    }

    public void Remove(string key)
    {
        lock (_lock)
        {
            _cache.Remove(key);
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _cache.Clear();
        }
    }

    public bool ContainsKey(string key)
    {
        lock (_lock)
        {
            return _cache.ContainsKey(key);
        }
    }

    // Additional methods for cache management
    public IEnumerable<string> GetAllKeys()
    {
        lock (_lock)
        {
            return new List<string>(_cache.Keys);
        }
    }

    public int Count => _cache.Count;

    public void RemoveExpiredItems(TimeSpan maxAge)
    {
        lock (_lock)
        {
            var expiredKeys = new List<string>();

            foreach (var pair in _cache)
            {
                if (DateTime.Now - pair.Value.Created > maxAge)
                {
                    expiredKeys.Add(pair.Key);
                }
            }

            foreach (var key in expiredKeys)
            {
                _cache.Remove(key);
            }
        }
    }

    public Dictionary<string, DateTime> GetAccessStatistics()
    {
        lock (_lock)
        {
            return _cache.ToDictionary(
                pair => pair.Key,
                pair => pair.Value.LastAccessed
            );
        }
    }
}

public class CacheItem<T>
{
    public T Value { get; }
    public DateTime Created { get; } = DateTime.Now;
    public DateTime LastAccessed { get; set; } = DateTime.Now;

    public CacheItem(T value)
    {
        Value = value;
    }
}

class Program
{
    static void Main()
    {
        var cache = new MemoryCache<string>();

        // Add items to cache
        cache.Set("user:1", "Alice");
        cache.Set("user:2", "Bob");
        cache.Set("settings:theme", "dark");

        // Retrieve items
        try
        {
            string user1 = cache.Get("user:1");
            Console.WriteLine($"User 1: {user1}");
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine(ex.Message);
        }

        // Safe retrieval
        if (cache.TryGet("user:2", out string user2))
        {
            Console.WriteLine($"User 2: {user2}");
        }

        // Check existence
        Console.WriteLine($"Cache contains 'user:3': {cache.ContainsKey("user:3")}");

        // Get all keys
        Console.WriteLine("All keys: " + string.Join(", ", cache.GetAllKeys()));

        // Cache statistics
        var stats = cache.GetAccessStatistics();
        Console.WriteLine("Access statistics:");
        foreach (var stat in stats)
        {
            Console.WriteLine($"{stat.Key}: {stat.Value}");
        }

        // Cleanup
        cache.RemoveExpiredItems(TimeSpan.FromMinutes(1));
        Console.WriteLine($"Items in cache after cleanup: {cache.Count}");
    }
}
```

## Exercises

1. **Custom Collection Implementation**
   - Implement a generic `PriorityQueue<T>` class
   - Create a `CircularBuffer<T>` with fixed capacity
   - Build a `Trie<T>` for efficient string operations

2. **LINQ Query Practice**
   - Create complex LINQ queries for data analysis
   - Implement custom LINQ extension methods
   - Practice query syntax vs method syntax conversions

3. **Performance Optimization**
   - Compare performance of different collection types
   - Implement caching mechanisms
   - Optimize memory usage in large collections

4. **Generic Utility Library**
   - Create generic methods for common operations
   - Implement generic comparers and equality checkers
   - Build generic validation and conversion utilities

5. **Data Processing Pipeline**
   - Create a LINQ-based data processing pipeline
   - Implement filtering, transformation, and aggregation
   - Add error handling and logging

6. **Collection Benchmarks**
   - Write performance benchmarks for different collections
   - Compare memory usage and operation speeds
   - Analyze big O complexity of operations

Collections and generics are essential tools for managing data efficiently in C#. By choosing the right collection type and leveraging LINQ effectively, you can write clean, performant, and maintainable code. Practice working with different collection types and LINQ operations to become proficient in data manipulation!
