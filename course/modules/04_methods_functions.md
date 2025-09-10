# Module 4 — Methods & Functions (Building Reusable Code)

## Overview

Methods (also called functions) are the building blocks of C# programs. They allow you to organize code into reusable, logical units that perform specific tasks. This module explores method declaration, parameters, return values, and advanced concepts like recursion, delegates, and lambda expressions. Understanding methods is crucial for writing maintainable and modular code.

## Topics Covered

- Method declaration and syntax
- Parameters and arguments (value, reference, output, optional)
- Return types and values
- Method overloading
- Recursion
- Local functions
- Expression-bodied members
- Delegates and lambda expressions
- Method best practices
- Documentation and comments

## Method Declaration Syntax

### Basic Method Structure

```csharp
// Method declaration
access_modifier return_type MethodName(parameter_list)
{
    // Method body
    // Statements to execute
    return value; // If return type is not void
}

// Example
public int AddNumbers(int a, int b)
{
    int sum = a + b;
    return sum;
}
```

### Method Components

1. **Access Modifier**: `public`, `private`, `protected`, `internal`
2. **Return Type**: The type of value the method returns (`void` for no return)
3. **Method Name**: Identifier following PascalCase convention
4. **Parameter List**: Input values in parentheses
5. **Method Body**: Code block containing the implementation

## Parameters and Arguments

### Value Parameters (Pass by Value)

```csharp
public void ModifyValue(int number)
{
    number = number * 2; // This only affects the local copy
    Console.WriteLine($"Inside method: {number}");
}

// Usage
int original = 5;
ModifyValue(original);
Console.WriteLine($"After method: {original}"); // Still 5
```

### Reference Parameters (`ref`)

```csharp
public void ModifyReference(ref int number)
{
    number = number * 2; // This modifies the original value
    Console.WriteLine($"Inside method: {number}");
}

// Usage
int original = 5;
ModifyReference(ref original);
Console.WriteLine($"After method: {original}"); // Now 10
```

### Output Parameters (`out`)

```csharp
public bool TryDivide(int dividend, int divisor, out int result)
{
    if (divisor == 0)
    {
        result = 0; // Must assign before returning
        return false;
    }

    result = dividend / divisor;
    return true;
}

// Usage
int quotient;
if (TryDivide(10, 2, out quotient))
{
    Console.WriteLine($"Result: {quotient}"); // 5
}
else
{
    Console.WriteLine("Division by zero!");
}
```

### Optional Parameters

```csharp
public void PrintMessage(string message, int repeatCount = 1, ConsoleColor color = ConsoleColor.White)
{
    Console.ForegroundColor = color;
    for (int i = 0; i < repeatCount; i++)
    {
        Console.WriteLine(message);
    }
    Console.ResetColor();
}

// Usage
PrintMessage("Hello");                    // Uses defaults
PrintMessage("Hello", 3);                 // Custom repeat count
PrintMessage("Hello", 2, ConsoleColor.Red); // Custom count and color
```

### Parameter Arrays (`params`)

```csharp
public int Sum(params int[] numbers)
{
    int total = 0;
    foreach (int number in numbers)
    {
        total += number;
    }
    return total;
}

// Usage
int result1 = Sum(1, 2, 3);           // 6
int result2 = Sum(1, 2, 3, 4, 5);     // 15

int[] array = { 1, 2, 3, 4 };
int result3 = Sum(array);             // 10
```

## Return Types and Values

### Void Methods

```csharp
public void DisplayWelcome()
{
    Console.WriteLine("Welcome to our application!");
    Console.WriteLine("Please follow the instructions.");
}
```

### Value-Returning Methods

```csharp
public int CalculateSquare(int number)
{
    return number * number;
}

public string GetFullName(string firstName, string lastName)
{
    return $"{firstName} {lastName}";
}

public bool IsEven(int number)
{
    return number % 2 == 0;
}
```

### Returning Multiple Values

```csharp
// Using tuples (C# 7.0+)
public (int quotient, int remainder) Divide(int dividend, int divisor)
{
    int quotient = dividend / divisor;
    int remainder = dividend % divisor;
    return (quotient, remainder);
}

// Usage
var (quotient, remainder) = Divide(17, 5);
Console.WriteLine($"17 ÷ 5 = {quotient} remainder {remainder}");

// Using out parameters
public void GetMinMax(int[] numbers, out int min, out int max)
{
    min = numbers.Min();
    max = numbers.Max();
}

// Usage
int[] values = { 3, 1, 8, 4, 9 };
GetMinMax(values, out int minimum, out int maximum);
Console.WriteLine($"Min: {minimum}, Max: {maximum}");
```

## Method Overloading

```csharp
public class Calculator
{
    // Overloaded methods with different parameter types
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

    public string Add(string a, string b)
    {
        return a + b;
    }
}

// Usage
Calculator calc = new Calculator();
int sum1 = calc.Add(5, 3);           // 8
double sum2 = calc.Add(5.5, 3.2);    // 8.7
int sum3 = calc.Add(1, 2, 3);        // 6
string concat = calc.Add("Hello", " World"); // "Hello World"
```

## Recursion

### Basic Recursion

```csharp
public int Factorial(int n)
{
    // Base case: stops the recursion
    if (n <= 1)
        return 1;

    // Recursive case: calls itself
    return n * Factorial(n - 1);
}

// Usage
int result = Factorial(5); // 120 (5! = 5 × 4 × 3 × 2 × 1)
```

### Recursion with Memoization

```csharp
private Dictionary<int, long> fibonacciCache = new Dictionary<int, long>();

public long Fibonacci(int n)
{
    // Base cases
    if (n == 0) return 0;
    if (n == 1) return 1;

    // Check cache first
    if (fibonacciCache.ContainsKey(n))
        return fibonacciCache[n];

    // Recursive calculation with caching
    long result = Fibonacci(n - 1) + Fibonacci(n - 2);
    fibonacciCache[n] = result;

    return result;
}
```

### Tail Recursion (Conceptual)

```csharp
// Traditional recursion (not tail-recursive)
public int SumRecursive(int n)
{
    if (n == 0) return 0;
    return n + SumRecursive(n - 1); // Addition happens after recursive call
}

// Tail-recursive version
public int SumTailRecursive(int n, int accumulator = 0)
{
    if (n == 0) return accumulator;
    return SumTailRecursive(n - 1, accumulator + n); // No operation after recursive call
}
```

## Local Functions

```csharp
public int ProcessNumbers(int[] numbers)
{
    // Local function - only accessible within ProcessNumbers
    bool IsValidNumber(int num)
    {
        return num > 0 && num < 100;
    }

    int validCount = 0;
    foreach (int number in numbers)
    {
        if (IsValidNumber(number))
        {
            validCount++;
        }
    }

    return validCount;
}

// Local functions can be recursive
public void PrintNumbers(int start, int end)
{
    void PrintRange(int current)
    {
        if (current > end) return;

        Console.WriteLine(current);
        PrintRange(current + 1);
    }

    PrintRange(start);
}
```

## Expression-Bodied Members

```csharp
public class Rectangle
{
    public double Width { get; set; }
    public double Height { get; set; }

    // Expression-bodied property
    public double Area => Width * Height;

    // Expression-bodied method
    public double Perimeter() => 2 * (Width + Height);

    // Expression-bodied constructor
    public Rectangle(double width, double height) => (Width, Height) = (width, height);
}
```

## Delegates and Lambda Expressions

### Delegates

```csharp
// Delegate declaration
public delegate int MathOperation(int a, int b);

// Methods that match the delegate signature
public int Add(int a, int b) => a + b;
public int Subtract(int a, int b) => a - b;
public int Multiply(int a, int b) => a * b;

// Usage
MathOperation operation = Add;
int result = operation(5, 3); // 8

operation = Multiply;
result = operation(5, 3); // 15
```

### Lambda Expressions

```csharp
// Lambda expression syntax: parameters => expression
Func<int, int, int> add = (a, b) => a + b;
Func<int, bool> isEven = n => n % 2 == 0;
Action<string> greet = name => Console.WriteLine($"Hello, {name}!");

// Usage
int sum = add(3, 5); // 8
bool even = isEven(4); // true
greet("Alice"); // "Hello, Alice!"
```

### LINQ with Lambda Expressions

```csharp
List<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

// Filter with lambda
var evenNumbers = numbers.Where(n => n % 2 == 0);

// Transform with lambda
var squaredNumbers = numbers.Select(n => n * n);

// Complex query
var result = numbers
    .Where(n => n > 3)
    .Select(n => n * n)
    .Where(square => square < 50)
    .Sum();
```

## Advanced Method Concepts

### Generic Methods

```csharp
public T FindMax<T>(T[] array) where T : IComparable<T>
{
    if (array == null || array.Length == 0)
        throw new ArgumentException("Array cannot be null or empty");

    T max = array[0];
    for (int i = 1; i < array.Length; i++)
    {
        if (array[i].CompareTo(max) > 0)
            max = array[i];
    }
    return max;
}

// Usage
int[] intArray = { 3, 1, 8, 4, 9 };
int maxInt = FindMax(intArray); // 9

string[] stringArray = { "apple", "banana", "cherry" };
string maxString = FindMax(stringArray); // "cherry"
```

### Extension Methods

```csharp
public static class StringExtensions
{
    public static bool IsPalindrome(this string str)
    {
        if (string.IsNullOrEmpty(str)) return true;

        string reversed = new string(str.Reverse().ToArray());
        return str.Equals(reversed, StringComparison.OrdinalIgnoreCase);
    }

    public static string ToTitleCase(this string str)
    {
        if (string.IsNullOrEmpty(str)) return str;

        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
    }
}

// Usage
string word = "racecar";
bool isPalindrome = word.IsPalindrome(); // true

string sentence = "hello world";
string titleCase = sentence.ToTitleCase(); // "Hello World"
```

## Method Best Practices

### 1. Single Responsibility Principle

```csharp
// Bad: Method does too many things
public void ProcessOrder(Order order)
{
    // Validate order
    // Calculate total
    // Apply discount
    // Save to database
    // Send confirmation email
}

// Good: Break into focused methods
public bool ValidateOrder(Order order) { /* ... */ }
public decimal CalculateTotal(Order order) { /* ... */ }
public decimal ApplyDiscount(Order order, decimal total) { /* ... */ }
public void SaveOrder(Order order) { /* ... */ }
public void SendConfirmationEmail(Order order) { /* ... */ }

public void ProcessOrder(Order order)
{
    if (!ValidateOrder(order)) return;

    decimal total = CalculateTotal(order);
    total = ApplyDiscount(order, total);

    SaveOrder(order);
    SendConfirmationEmail(order);
}
```

### 2. Meaningful Names

```csharp
// Bad names
public void DoStuff(int x, int y) { /* ... */ }
public int Calc(int a, int b) { /* ... */ }

// Good names
public void ProcessPayment(decimal amount, PaymentMethod method) { /* ... */ }
public int CalculateMonthlyPayment(decimal principal, int termMonths) { /* ... */ }
```

### 3. Proper Parameter Validation

```csharp
public void Withdraw(Account account, decimal amount)
{
    if (account == null)
        throw new ArgumentNullException(nameof(account));

    if (amount <= 0)
        throw new ArgumentException("Amount must be positive", nameof(amount));

    if (amount > account.Balance)
        throw new InvalidOperationException("Insufficient funds");

    // Proceed with withdrawal
    account.Balance -= amount;
}
```

### 4. Documentation

```csharp
/// <summary>
/// Calculates the distance between two points in 2D space.
/// </summary>
/// <param name="x1">X coordinate of first point</param>
/// <param name="y1">Y coordinate of first point</param>
/// <param name="x2">X coordinate of second point</param>
/// <param name="y2">Y coordinate of second point</param>
/// <returns>The Euclidean distance between the points</returns>
public double CalculateDistance(double x1, double y1, double x2, double y2)
{
    double deltaX = x2 - x1;
    double deltaY = y2 - y1;
    return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
}
```

## Comprehensive Examples

### Example 1: Math Library

```csharp
using System;

public static class MathUtils
{
    /// <summary>
    /// Calculates the factorial of a number using iteration
    /// </summary>
    public static long FactorialIterative(int n)
    {
        if (n < 0) throw new ArgumentException("Number must be non-negative");

        long result = 1;
        for (int i = 2; i <= n; i++)
        {
            result *= i;
        }
        return result;
    }

    /// <summary>
    /// Calculates the factorial of a number using recursion
    /// </summary>
    public static long FactorialRecursive(int n)
    {
        if (n < 0) throw new ArgumentException("Number must be non-negative");
        if (n <= 1) return 1;
        return n * FactorialRecursive(n - 1);
    }

    /// <summary>
    /// Checks if a number is prime
    /// </summary>
    public static bool IsPrime(int number)
    {
        if (number <= 1) return false;
        if (number <= 3) return true;
        if (number % 2 == 0 || number % 3 == 0) return false;

        for (int i = 5; i * i <= number; i += 6)
        {
            if (number % i == 0 || number % (i + 2) == 0)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Calculates the greatest common divisor using Euclidean algorithm
    /// </summary>
    public static int Gcd(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    /// <summary>
    /// Calculates the least common multiple
    /// </summary>
    public static int Lcm(int a, int b)
    {
        return Math.Abs(a * b) / Gcd(a, b);
    }

    /// <summary>
    /// Generates Fibonacci sequence up to n terms
    /// </summary>
    public static IEnumerable<long> FibonacciSequence(int terms)
    {
        if (terms <= 0) yield break;

        long a = 0, b = 1;
        for (int i = 0; i < terms; i++)
        {
            yield return a;
            (a, b) = (b, a + b);
        }
    }
}

class Program
{
    static void Main()
    {
        // Test factorial methods
        Console.WriteLine($"5! (iterative) = {MathUtils.FactorialIterative(5)}");
        Console.WriteLine($"5! (recursive) = {MathUtils.FactorialRecursive(5)}");

        // Test prime checking
        Console.WriteLine($"17 is prime: {MathUtils.IsPrime(17)}");
        Console.WriteLine($"18 is prime: {MathUtils.IsPrime(18)}");

        // Test GCD and LCM
        Console.WriteLine($"GCD(48, 18) = {MathUtils.Gcd(48, 18)}");
        Console.WriteLine($"LCM(48, 18) = {MathUtils.Lcm(48, 18)}");

        // Test Fibonacci sequence
        Console.Write("Fibonacci sequence (10 terms): ");
        foreach (long number in MathUtils.FibonacciSequence(10))
        {
            Console.Write($"{number} ");
        }
        Console.WriteLine();
    }
}
```

### Example 2: String Processing Utility

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class StringProcessor
{
    /// <summary>
    /// Counts the frequency of each word in a text
    /// </summary>
    public static Dictionary<string, int> GetWordFrequency(string text)
    {
        return text.Split(new[] { ' ', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
                   .GroupBy(word => word.ToLower())
                   .ToDictionary(group => group.Key, group => group.Count());
    }

    /// <summary>
    /// Reverses the words in a sentence while maintaining word order
    /// </summary>
    public static string ReverseWords(string sentence)
    {
        if (string.IsNullOrEmpty(sentence)) return sentence;

        string[] words = sentence.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        Array.Reverse(words);
        return string.Join(" ", words);
    }

    /// <summary>
    /// Checks if a string is a valid email address (basic validation)
    /// </summary>
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrEmpty(email)) return false;

        int atIndex = email.IndexOf('@');
        int dotIndex = email.LastIndexOf('.');

        return atIndex > 0 &&
               dotIndex > atIndex + 1 &&
               dotIndex < email.Length - 1;
    }

    /// <summary>
    /// Converts a string to Pig Latin
    /// </summary>
    public static string ToPigLatin(string word)
    {
        if (string.IsNullOrEmpty(word)) return word;

        string lowerWord = word.ToLower();
        char firstLetter = lowerWord[0];

        if ("aeiou".Contains(firstLetter))
        {
            return word + "way";
        }
        else
        {
            string restOfWord = lowerWord.Substring(1);
            return restOfWord + firstLetter + "ay";
        }
    }

    /// <summary>
    /// Generates a random password with specified criteria
    /// </summary>
    public static string GeneratePassword(int length = 8, bool includeUppercase = true,
                                        bool includeNumbers = true, bool includeSymbols = false)
    {
        const string lowercase = "abcdefghijklmnopqrstuvwxyz";
        const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string numbers = "0123456789";
        const string symbols = "!@#$%^&*";

        StringBuilder charSet = new StringBuilder(lowercase);

        if (includeUppercase) charSet.Append(uppercase);
        if (includeNumbers) charSet.Append(numbers);
        if (includeSymbols) charSet.Append(symbols);

        Random random = new Random();
        StringBuilder password = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            int index = random.Next(charSet.Length);
            password.Append(charSet[index]);
        }

        return password.ToString();
    }

    /// <summary>
    /// Calculates the Levenshtein distance between two strings
    /// </summary>
    public static int LevenshteinDistance(string s1, string s2)
    {
        if (string.IsNullOrEmpty(s1)) return s2?.Length ?? 0;
        if (string.IsNullOrEmpty(s2)) return s1.Length;

        int[,] matrix = new int[s1.Length + 1, s2.Length + 1];

        // Initialize first row and column
        for (int i = 0; i <= s1.Length; i++) matrix[i, 0] = i;
        for (int j = 0; j <= s2.Length; j++) matrix[0, j] = j;

        // Fill the matrix
        for (int i = 1; i <= s1.Length; i++)
        {
            for (int j = 1; j <= s2.Length; j++)
            {
                int cost = (s2[j - 1] == s1[i - 1]) ? 0 : 1;

                matrix[i, j] = Math.Min(
                    Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                    matrix[i - 1, j - 1] + cost);
            }
        }

        return matrix[s1.Length, s2.Length];
    }
}

class Program
{
    static void Main()
    {
        // Test word frequency
        string text = "The quick brown fox jumps over the lazy dog. The fox is quick.";
        var frequencies = StringProcessor.GetWordFrequency(text);
        Console.WriteLine("Word frequencies:");
        foreach (var pair in frequencies.OrderByDescending(p => p.Value))
        {
            Console.WriteLine($"{pair.Key}: {pair.Value}");
        }

        // Test word reversal
        string sentence = "Hello world this is a test";
        Console.WriteLine($"\nOriginal: {sentence}");
        Console.WriteLine($"Reversed: {StringProcessor.ReverseWords(sentence)}");

        // Test email validation
        string[] emails = { "user@example.com", "invalid-email", "test@", "@domain.com" };
        foreach (string email in emails)
        {
            Console.WriteLine($"{email} is valid: {StringProcessor.IsValidEmail(email)}");
        }

        // Test Pig Latin
        string[] words = { "hello", "apple", "strong", "eat" };
        Console.WriteLine("\nPig Latin:");
        foreach (string word in words)
        {
            Console.WriteLine($"{word} -> {StringProcessor.ToPigLatin(word)}");
        }

        // Test password generation
        Console.WriteLine($"\nGenerated password: {StringProcessor.GeneratePassword(12)}");

        // Test Levenshtein distance
        string word1 = "kitten";
        string word2 = "sitting";
        int distance = StringProcessor.LevenshteinDistance(word1, word2);
        Console.WriteLine($"\nLevenshtein distance between '{word1}' and '{word2}': {distance}");
    }
}
```

## Exercises

1. **Calculator Class**
   - Create a Calculator class with methods for basic arithmetic operations
   - Add method overloading for different data types
   - Include input validation and error handling

2. **String Manipulation Library**
   - Implement methods to reverse strings, check palindromes, count vowels
   - Add methods for case conversion and word counting
   - Create extension methods for the string class

3. **Mathematical Functions**
   - Implement recursive and iterative versions of factorial and Fibonacci
   - Add methods for prime number checking and finding factors
   - Create methods for statistical calculations (mean, median, mode)

4. **Array Processing Methods**
   - Create methods to find min/max values in arrays
   - Implement sorting algorithms (bubble sort, quicksort)
   - Add methods for array searching and filtering

5. **File Processing Utility**
   - Create methods to read and write text files
   - Implement CSV file parsing
   - Add methods for file statistics (line count, word count, etc.)

6. **Validation Library**
   - Create methods to validate emails, phone numbers, and postal codes
   - Implement password strength checking
   - Add methods for input sanitization

Methods are the foundation of well-structured C# programs. By mastering method creation, parameter passing, and return values, you'll be able to write clean, maintainable, and reusable code. Practice creating methods for common tasks and gradually build up your library of useful functions!
