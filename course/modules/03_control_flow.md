# Module 3 — Control Flow (Conditional Statements, Loops, and Pattern Matching)

## Overview

Control flow determines the order in which statements are executed in a program. This module explores the various control structures in C# that allow you to make decisions, repeat actions, and handle different scenarios. Understanding control flow is essential for writing programs that can adapt to different inputs and conditions.

## Topics Covered

- Conditional statements (if, else, switch)
- Loops (for, while, do-while, foreach)
- Jump statements (break, continue, return, goto)
- Pattern matching (C# 7.0+)
- Switch expressions (C# 8.0+)
- Best practices and common patterns
- Nested control structures
- Control flow analysis

## Conditional Statements

### The `if` Statement

The `if` statement executes code based on a boolean condition.

```csharp
// Basic if statement
if (condition)
{
    // Code to execute if condition is true
}

// if-else statement
if (temperature > 30)
{
    Console.WriteLine("It's hot outside!");
}
else
{
    Console.WriteLine("It's not too hot.");
}

// if-else if-else chain
if (score >= 90)
{
    Console.WriteLine("Grade: A");
}
else if (score >= 80)
{
    Console.WriteLine("Grade: B");
}
else if (score >= 70)
{
    Console.WriteLine("Grade: C");
}
else if (score >= 60)
{
    Console.WriteLine("Grade: D");
}
else
{
    Console.WriteLine("Grade: F");
}
```

### Ternary Operator

A concise way to write simple if-else statements:

```csharp
// Traditional if-else
string result;
if (age >= 18)
{
    result = "Adult";
}
else
{
    result = "Minor";
}

// Ternary operator
string result = age >= 18 ? "Adult" : "Minor";

// Nested ternary (use sparingly for readability)
string category = age >= 65 ? "Senior" : age >= 18 ? "Adult" : "Minor";
```

## Switch Statements

### Traditional Switch Statement

```csharp
int dayOfWeek = 3;
string dayName;

switch (dayOfWeek)
{
    case 1:
        dayName = "Monday";
        break;
    case 2:
        dayName = "Tuesday";
        break;
    case 3:
        dayName = "Wednesday";
        break;
    case 4:
        dayName = "Thursday";
        break;
    case 5:
        dayName = "Friday";
        break;
    case 6:
        dayName = "Saturday";
        break;
    case 7:
        dayName = "Sunday";
        break;
    default:
        dayName = "Invalid day";
        break;
}

Console.WriteLine($"Day {dayOfWeek} is {dayName}");
```

### Switch Statement with Patterns (C# 7.0+)

```csharp
object obj = "Hello";

switch (obj)
{
    case string s when s.Length > 5:
        Console.WriteLine($"Long string: {s}");
        break;
    case string s:
        Console.WriteLine($"Short string: {s}");
        break;
    case int i:
        Console.WriteLine($"Integer: {i}");
        break;
    case null:
        Console.WriteLine("Null value");
        break;
    default:
        Console.WriteLine("Unknown type");
        break;
}
```

### Switch Expressions (C# 8.0+)

Switch expressions provide a more concise syntax:

```csharp
// Traditional switch statement
string GetDayName(int day)
{
    switch (day)
    {
        case 1: return "Monday";
        case 2: return "Tuesday";
        case 3: return "Wednesday";
        case 4: return "Thursday";
        case 5: return "Friday";
        case 6: return "Saturday";
        case 7: return "Sunday";
        default: return "Invalid";
    }
}

// Switch expression
string GetDayName(int day) => day switch
{
    1 => "Monday",
    2 => "Tuesday",
    3 => "Wednesday",
    4 => "Thursday",
    5 => "Friday",
    6 => "Saturday",
    7 => "Sunday",
    _ => "Invalid"
};

// With pattern matching
string DescribeNumber(object number) => number switch
{
    int i when i < 0 => "Negative integer",
    int i when i == 0 => "Zero",
    int i when i > 0 => "Positive integer",
    double d => $"Double: {d}",
    string s => $"String: {s}",
    _ => "Unknown type"
};
```

## Loops

### The `for` Loop

Best for when you know the number of iterations:

```csharp
// Basic for loop
for (int i = 0; i < 5; i++)
{
    Console.WriteLine($"Iteration {i}");
}

// Multiple variables
for (int i = 0, j = 10; i < j; i++, j--)
{
    Console.WriteLine($"i: {i}, j: {j}");
}

// Infinite loop (use with caution)
for (;;)
{
    // This will run forever unless broken
    if (someCondition)
        break;
}
```

### The `while` Loop

Executes while a condition is true. Condition is checked before each iteration:

```csharp
int counter = 0;

// Basic while loop
while (counter < 5)
{
    Console.WriteLine($"Counter: {counter}");
    counter++;
}

// While loop with user input
string input;
while ((input = Console.ReadLine()) != "quit")
{
    Console.WriteLine($"You entered: {input}");
}
```

### The `do-while` Loop

Similar to while, but condition is checked after each iteration (guarantees at least one execution):

```csharp
int number;

// Do-while ensures the loop body executes at least once
do
{
    Console.Write("Enter a positive number: ");
    number = int.Parse(Console.ReadLine());
} while (number <= 0);

Console.WriteLine($"You entered: {number}");
```

### The `foreach` Loop

Designed for iterating over collections:

```csharp
// Array iteration
int[] numbers = { 1, 2, 3, 4, 5 };
foreach (int number in numbers)
{
    Console.WriteLine(number);
}

// List iteration
List<string> names = new List<string> { "Alice", "Bob", "Charlie" };
foreach (string name in names)
{
    Console.WriteLine($"Hello, {name}!");
}

// Dictionary iteration
Dictionary<string, int> ages = new Dictionary<string, int>
{
    ["Alice"] = 25,
    ["Bob"] = 30,
    ["Charlie"] = 35
};

foreach (var pair in ages)
{
    Console.WriteLine($"{pair.Key} is {pair.Value} years old");
}
```

## Jump Statements

### `break` Statement

Exits the current loop or switch statement:

```csharp
// Break in for loop
for (int i = 0; i < 100; i++)
{
    if (i == 5)
    {
        Console.WriteLine("Found 5, breaking...");
        break;
    }
    Console.WriteLine(i);
}

// Break in nested loops
for (int i = 0; i < 3; i++)
{
    for (int j = 0; j < 3; j++)
    {
        if (i == 1 && j == 1)
        {
            Console.WriteLine("Breaking out of nested loops");
            break; // Only breaks inner loop
        }
        Console.Write($"({i},{j}) ");
    }
    Console.WriteLine();
}
```

### `continue` Statement

Skips the rest of the current iteration and moves to the next:

```csharp
// Skip even numbers
for (int i = 0; i < 10; i++)
{
    if (i % 2 == 0)
    {
        continue; // Skip even numbers
    }
    Console.WriteLine($"Odd number: {i}");
}

// Skip processing null items
List<string> items = new List<string> { "apple", null, "banana", null, "cherry" };
foreach (string item in items)
{
    if (item == null)
    {
        continue;
    }
    Console.WriteLine($"Processing: {item}");
}
```

### `return` Statement

Exits the current method and optionally returns a value:

```csharp
int FindFirstEven(int[] numbers)
{
    foreach (int number in numbers)
    {
        if (number % 2 == 0)
        {
            return number; // Exit method and return value
        }
    }
    return -1; // Return default value if no even number found
}
```

### `goto` Statement

Jumps to a labeled statement (use sparingly):

```csharp
int i = 0;
start:
if (i < 5)
{
    Console.WriteLine(i);
    i++;
    goto start; // Jump back to start label
}
```

## Advanced Pattern Matching

### Type Patterns

```csharp
object ProcessValue(object value)
{
    switch (value)
    {
        case int i:
            return i * 2;
        case string s when s.Length > 5:
            return s.ToUpper();
        case string s:
            return s.ToLower();
        case double d:
            return Math.Round(d, 2);
        case null:
            return "Null value";
        default:
            return "Unknown type";
    }
}
```

### Property Patterns (C# 8.0+)

```csharp
public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
}

string GetDescription(Person person) => person switch
{
    { Age: < 13 } => "Child",
    { Age: >= 13 and < 20 } => "Teenager",
    { Age: >= 20 and < 65 } => "Adult",
    { Age: >= 65 } => "Senior",
    _ => "Unknown"
};
```

### Tuple Patterns

```csharp
(string, int) GetResult(bool success, int value) => (success, value) switch
{
    (true, > 0) => ("Success", value),
    (true, 0) => ("Success", 0),
    (false, _) => ("Failure", -1),
    _ => ("Unknown", 0)
};
```

### Positional Patterns

```csharp
public class Point
{
    public int X { get; }
    public int Y { get; }

    public Point(int x, int y) => (X, Y) = (x, y);

    public void Deconstruct(out int x, out int y) => (x, y) = (X, Y);
}

string GetQuadrant(Point point) => point switch
{
    (0, 0) => "Origin",
    ( > 0, > 0) => "First quadrant",
    ( < 0, > 0) => "Second quadrant",
    ( < 0, < 0) => "Third quadrant",
    ( > 0, < 0) => "Fourth quadrant",
    _ => "On axis"
};
```

## Control Flow Best Practices

### 1. Avoid Deep Nesting

```csharp
// Avoid this (deep nesting)
if (user != null)
{
    if (user.IsActive)
    {
        if (user.HasPermission)
        {
            // Do something
        }
    }
}

// Prefer this (early returns)
if (user == null) return;
if (!user.IsActive) return;
if (!user.HasPermission) return;
// Do something
```

### 2. Use Descriptive Conditions

```csharp
// Avoid magic numbers
if (age >= 18) { /* ... */ }

// Better with named constants
const int AdultAge = 18;
if (age >= AdultAge) { /* ... */ }
```

### 3. Prefer `foreach` for Collections

```csharp
// Avoid index-based iteration when possible
List<string> names = GetNames();
for (int i = 0; i < names.Count; i++)
{
    Console.WriteLine(names[i]);
}

// Prefer foreach
foreach (string name in names)
{
    Console.WriteLine(name);
}
```

### 4. Use Switch Expressions for Multiple Conditions

```csharp
// Traditional approach
string GetSeason(int month)
{
    switch (month)
    {
        case 12:
        case 1:
        case 2:
            return "Winter";
        case 3:
        case 4:
        case 5:
            return "Spring";
        case 6:
        case 7:
        case 8:
            return "Summer";
        case 9:
        case 10:
        case 11:
            return "Fall";
        default:
            return "Invalid";
    }
}

// Modern approach with patterns
string GetSeason(int month) => month switch
{
    12 or 1 or 2 => "Winter",
    3 or 4 or 5 => "Spring",
    6 or 7 or 8 => "Summer",
    9 or 10 or 11 => "Fall",
    _ => "Invalid"
};
```

## Comprehensive Examples

### Example 1: Number Guessing Game

```csharp
using System;

class GuessingGame
{
    static void Main()
    {
        Random random = new Random();
        int targetNumber = random.Next(1, 101);
        int attempts = 0;
        bool guessedCorrectly = false;

        Console.WriteLine("Welcome to the Number Guessing Game!");
        Console.WriteLine("I'm thinking of a number between 1 and 100.");

        while (!guessedCorrectly)
        {
            Console.Write("Enter your guess: ");
            string input = Console.ReadLine();

            if (!int.TryParse(input, out int guess))
            {
                Console.WriteLine("Please enter a valid number.");
                continue;
            }

            attempts++;

            if (guess < targetNumber)
            {
                Console.WriteLine("Too low! Try again.");
            }
            else if (guess > targetNumber)
            {
                Console.WriteLine("Too high! Try again.");
            }
            else
            {
                guessedCorrectly = true;
                Console.WriteLine($"Congratulations! You guessed it in {attempts} attempts.");
            }
        }
    }
}
```

### Example 2: Menu-Driven Calculator

```csharp
using System;

class Calculator
{
    static void Main()
    {
        bool running = true;

        while (running)
        {
            DisplayMenu();
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    PerformCalculation("addition");
                    break;
                case "2":
                    PerformCalculation("subtraction");
                    break;
                case "3":
                    PerformCalculation("multiplication");
                    break;
                case "4":
                    PerformCalculation("division");
                    break;
                case "5":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }

            if (running)
            {
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }

    static void DisplayMenu()
    {
        Console.WriteLine("=== Calculator Menu ===");
        Console.WriteLine("1. Addition");
        Console.WriteLine("2. Subtraction");
        Console.WriteLine("3. Multiplication");
        Console.WriteLine("4. Division");
        Console.WriteLine("5. Exit");
        Console.Write("Choose an operation: ");
    }

    static void PerformCalculation(string operation)
    {
        Console.Write("Enter first number: ");
        if (!double.TryParse(Console.ReadLine(), out double num1))
        {
            Console.WriteLine("Invalid number.");
            return;
        }

        Console.Write("Enter second number: ");
        if (!double.TryParse(Console.ReadLine(), out double num2))
        {
            Console.WriteLine("Invalid number.");
            return;
        }

        double result = operation switch
        {
            "addition" => num1 + num2,
            "subtraction" => num1 - num2,
            "multiplication" => num1 * num2,
            "division" when num2 != 0 => num1 / num2,
            "division" => throw new DivideByZeroException(),
            _ => 0
        };

        Console.WriteLine($"Result: {num1} {GetOperatorSymbol(operation)} {num2} = {result}");
    }

    static string GetOperatorSymbol(string operation) => operation switch
    {
        "addition" => "+",
        "subtraction" => "-",
        "multiplication" => "*",
        "division" => "/",
        _ => "?"
    };
}
```

### Example 3: Student Grade Analyzer

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

class StudentGradeAnalyzer
{
    static void Main()
    {
        List<Student> students = new List<Student>
        {
            new Student("Alice", 85),
            new Student("Bob", 92),
            new Student("Charlie", 78),
            new Student("Diana", 96),
            new Student("Eve", 88)
        };

        bool running = true;

        while (running)
        {
            Console.Clear();
            Console.WriteLine("=== Student Grade Analyzer ===");
            Console.WriteLine("1. View all students and grades");
            Console.WriteLine("2. Calculate class average");
            Console.WriteLine("3. Find highest and lowest grades");
            Console.WriteLine("4. Get grade distribution");
            Console.WriteLine("5. Search for student");
            Console.WriteLine("6. Exit");
            Console.Write("Choose an option: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    DisplayAllStudents(students);
                    break;
                case "2":
                    CalculateAverage(students);
                    break;
                case "3":
                    FindExtremes(students);
                    break;
                case "4":
                    ShowDistribution(students);
                    break;
                case "5":
                    SearchStudent(students);
                    break;
                case "6":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }

            if (running)
            {
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }
    }

    static void DisplayAllStudents(List<Student> students)
    {
        Console.WriteLine("\n=== All Students ===");
        foreach (var student in students.OrderBy(s => s.Grade))
        {
            string letterGrade = GetLetterGrade(student.Grade);
            Console.WriteLine($"{student.Name}: {student.Grade} ({letterGrade})");
        }
    }

    static void CalculateAverage(List<Student> students)
    {
        double average = students.Average(s => s.Grade);
        Console.WriteLine($"\nClass Average: {average:F2}");
    }

    static void FindExtremes(List<Student> students)
    {
        var highest = students.OrderByDescending(s => s.Grade).First();
        var lowest = students.OrderBy(s => s.Grade).First();

        Console.WriteLine($"\nHighest Grade: {highest.Name} ({highest.Grade})");
        Console.WriteLine($"Lowest Grade: {lowest.Name} ({lowest.Grade})");
    }

    static void ShowDistribution(List<Student> students)
    {
        Console.WriteLine("\n=== Grade Distribution ===");
        var distribution = students.GroupBy(s => GetLetterGrade(s.Grade))
                                  .OrderBy(g => g.Key);

        foreach (var group in distribution)
        {
            Console.WriteLine($"{group.Key}: {group.Count()} students");
        }
    }

    static void SearchStudent(List<Student> students)
    {
        Console.Write("Enter student name: ");
        string searchName = Console.ReadLine();

        var foundStudents = students.Where(s =>
            s.Name.Contains(searchName, StringComparison.OrdinalIgnoreCase));

        if (foundStudents.Any())
        {
            Console.WriteLine("\n=== Search Results ===");
            foreach (var student in foundStudents)
            {
                string letterGrade = GetLetterGrade(student.Grade);
                Console.WriteLine($"{student.Name}: {student.Grade} ({letterGrade})");
            }
        }
        else
        {
            Console.WriteLine("No students found with that name.");
        }
    }

    static string GetLetterGrade(int grade) => grade switch
    {
        >= 90 => "A",
        >= 80 => "B",
        >= 70 => "C",
        >= 60 => "D",
        _ => "F"
    };
}

class Student
{
    public string Name { get; set; }
    public int Grade { get; set; }

    public Student(string name, int grade)
    {
        Name = name;
        Grade = grade;
    }
}
```

## Common Pitfalls and Solutions

### 1. Off-by-One Errors

```csharp
// Incorrect (off-by-one)
for (int i = 0; i <= array.Length; i++)
{
    // This will cause IndexOutOfRangeException
}

// Correct
for (int i = 0; i < array.Length; i++)
{
    // Safe iteration
}
```

### 2. Infinite Loops

```csharp
// Potential infinite loop
while (true)
{
    // Make sure there's a way to exit!
    if (someCondition)
        break;
}
```

### 3. Modifying Collections During Iteration

```csharp
// Dangerous - modifying collection during foreach
foreach (var item in collection)
{
    if (someCondition)
        collection.Remove(item); // Can cause InvalidOperationException
}

// Safe approach
var itemsToRemove = new List<Item>();
foreach (var item in collection)
{
    if (someCondition)
        itemsToRemove.Add(item);
}

foreach (var item in itemsToRemove)
{
    collection.Remove(item);
}
```

### 4. Switch Case Fall-Through

```csharp
// In C#, fall-through is not allowed without break
switch (value)
{
    case 1:
        Console.WriteLine("One");
        break; // Required
    case 2:
        Console.WriteLine("Two");
        break;
    default:
        Console.WriteLine("Other");
        break;
}
```

## Exercises

1. **FizzBuzz Implementation**
   - Write a program that prints numbers from 1 to 100
   - For multiples of 3, print "Fizz" instead of the number
   - For multiples of 5, print "Buzz" instead of the number
   - For multiples of both 3 and 5, print "FizzBuzz"

2. **Simple ATM Simulator**
   - Create a menu-driven program that simulates ATM operations
   - Include options for balance inquiry, deposit, withdrawal
   - Use appropriate validation and error handling

3. **Pattern Printer**
   - Write a program that prints various patterns (triangle, diamond, etc.)
   - Allow user to choose pattern type and size
   - Use nested loops effectively

4. **Grade Calculator**
   - Create a program that takes student scores and calculates grades
   - Support multiple grading scales (letter grades, GPA, etc.)
   - Use switch expressions for grade calculation

5. **Number System Converter**
   - Convert between decimal, binary, hexadecimal, and octal
   - Use pattern matching for different conversion types
   - Include input validation

6. **Text-Based Adventure Game**
   - Create a simple text-based adventure with multiple choices
   - Use switch statements for handling user decisions
   - Include inventory management and scoring

Control flow is the backbone of programming logic. Mastering these concepts will enable you to write programs that can handle complex scenarios and user interactions effectively. Practice combining different control structures to solve various problems!
