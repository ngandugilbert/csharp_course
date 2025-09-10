# Module 2 — Basic C# Syntax & Data Types

## Overview

This module introduces the fundamental building blocks of C# programming: variables, data types, operators, and basic syntax rules. Understanding these concepts is crucial as they form the foundation for everything you'll build in C#. We'll explore how to declare and use variables, work with different data types, and perform operations on data.

## Topics Covered

- Variables and constants
- Primitive data types (integers, floating-point, boolean, character)
- Reference types (strings, objects)
- Type inference with `var`
- Operators (arithmetic, comparison, logical, assignment)
- Type conversion and casting
- Nullable types
- Basic input/output operations

## Variables and Constants

### Variable Declaration and Initialization

Variables are named storage locations in memory that hold data. In C#, you must declare a variable before using it.

```csharp
// Variable declaration
int age;
string name;

// Variable initialization
age = 25;
name = "Alice";

// Declaration with initialization
int year = 2024;
string greeting = "Hello, World!";
```

### Constants

Constants are immutable values that cannot be changed after declaration. Use the `const` keyword:

```csharp
const double PI = 3.14159;
const string COMPANY_NAME = "Tech Corp";
const int MAX_USERS = 1000;
```

### Variable Naming Rules

- Must start with a letter or underscore
- Can contain letters, digits, and underscores
- Cannot be C# keywords
- Case-sensitive
- Use camelCase for variables, PascalCase for constants

```csharp
// Valid variable names
int userAge;
string firstName;
double _temperature;
int numberOfItems;

// Invalid variable names (would cause compilation errors)
// int 123invalid;     // Starts with digit
// string class;       // Reserved keyword
// int user-age;       // Contains hyphen
```

## Primitive Data Types

C# provides several built-in primitive data types for different kinds of data.

### Integer Types

```csharp
// Signed integers
sbyte smallNumber = -128;      // -128 to 127 (8 bits)
short shortNumber = -32768;    // -32,768 to 32,767 (16 bits)
int regularNumber = -2147483648; // -2.1B to 2.1B (32 bits)
long bigNumber = -9223372036854775808; // Very large range (64 bits)

// Unsigned integers
byte tinyPositive = 255;       // 0 to 255 (8 bits)
ushort shortPositive = 65535;  // 0 to 65,535 (16 bits)
uint regularPositive = 4294967295; // 0 to 4.2B (32 bits)
ulong bigPositive = 18446744073709551615; // 0 to very large (64 bits)
```

### Floating-Point Types

```csharp
float singlePrecision = 3.14159f;     // ~7 decimal digits precision
double doublePrecision = 3.141592653589793; // ~15-16 decimal digits
decimal highPrecision = 123.45678901234567890123456789m; // 28-29 decimal digits
```

**Key Differences:**
- `float`: Fastest, least precise, ends with `f`
- `double`: Balanced performance and precision (default for decimals)
- `decimal`: Slowest, most precise, ends with `m`, ideal for financial calculations

### Boolean Type

```csharp
bool isActive = true;
bool isCompleted = false;
bool hasPermission = user.Role == "Admin";
```

### Character Type

```csharp
char letter = 'A';
char digit = '5';
char symbol = '@';
char unicodeChar = '\u0041';  // Unicode for 'A'
char newline = '\n';          // Escape sequences
```

## Reference Types

### Strings

Strings are reference types that represent sequences of characters.

```csharp
// String literals
string greeting = "Hello, World!";
string emptyString = "";
string nullString = null;

// String interpolation (C# 6.0+)
string name = "Alice";
int age = 30;
string message = $"Hello, {name}! You are {age} years old.";

// Verbatim strings (preserves formatting)
string multiline = @"This is a
multiline string
that preserves whitespace";

// String concatenation
string fullName = "John" + " " + "Doe";

// String methods
string text = "Hello, World!";
int length = text.Length;              // 13
string upper = text.ToUpper();         // "HELLO, WORLD!"
string lower = text.ToLower();         // "hello, world!"
bool contains = text.Contains("World"); // true
string substring = text.Substring(7, 5); // "World"
```

### Objects

All non-primitive types inherit from `object`, the root of C#'s type hierarchy.

```csharp
object anything = "This can be anything";
anything = 42;        // Now it's an integer
anything = true;      // Now it's a boolean
anything = new int[] {1, 2, 3}; // Now it's an array
```

## Type Inference with `var`

C# 3.0 introduced the `var` keyword for implicit typing:

```csharp
// Explicit typing
int number = 42;
string text = "Hello";

// Implicit typing with var
var number = 42;          // Inferred as int
var text = "Hello";       // Inferred as string
var list = new List<int>(); // Inferred as List<int>
var dictionary = new Dictionary<string, int>(); // Inferred as Dictionary<string, int>
```

**When to use `var`:**
- When the type is obvious from the right side
- With LINQ queries
- When working with anonymous types
- To reduce verbosity

**When to avoid `var`:**
- When the type isn't clear from context
- In public APIs where type clarity is important
- When you need to be explicit about the type

## Operators

### Arithmetic Operators

```csharp
int a = 10;
int b = 3;

int sum = a + b;        // 13
int difference = a - b; // 7
int product = a * b;    // 30
int quotient = a / b;   // 3 (integer division)
int remainder = a % b;  // 1 (modulo)

double precise = 10.0 / 3.0; // 3.333...

// Increment and decrement
int counter = 5;
counter++;              // counter is now 6
counter--;              // counter is now 5

// Compound assignment
int value = 10;
value += 5;             // value = value + 5 (15)
value -= 3;             // value = value - 3 (12)
value *= 2;             // value = value * 2 (24)
value /= 4;             // value = value * 4 (6)
```

### Comparison Operators

```csharp
int x = 5;
int y = 10;

bool equal = x == y;           // false
bool notEqual = x != y;        // true
bool greater = x > y;          // false
bool less = x < y;             // true
bool greaterOrEqual = x >= y;  // false
bool lessOrEqual = x <= y;     // true
```

### Logical Operators

```csharp
bool a = true;
bool b = false;

bool andResult = a && b;       // false (AND)
bool orResult = a || b;        // true (OR)
bool notResult = !a;           // false (NOT)

// Short-circuit evaluation
bool result = (x > 0) && (y / x > 1); // Won't divide by zero if x <= 0
```

### Bitwise Operators

```csharp
int flags = 0b1010;    // Binary: 10 in decimal

// Bitwise AND, OR, XOR
int and = flags & 0b1100;   // 0b1000 (8)
int or = flags | 0b1100;    // 0b1110 (14)
int xor = flags ^ 0b1100;   // 0b0110 (6)

// Bit shifting
int leftShift = flags << 2;  // 0b101000 (40)
int rightShift = flags >> 1; // 0b0101 (5)

// Bitwise NOT
int not = ~flags;           // Inverts all bits
```

## Type Conversion

### Implicit Conversion

```csharp
// Safe conversions that don't lose data
int smallNumber = 42;
long bigNumber = smallNumber;     // int to long

float single = 3.14f;
double precise = single;          // float to double

// Smaller to larger integer types
short shortNum = 100;
int intNum = shortNum;
```

### Explicit Conversion (Casting)

```csharp
// Potentially unsafe conversions
double precise = 3.14159;
int whole = (int)precise;          // 3 (truncates decimal part)

long bigNum = 1000000L;
int smallNum = (int)bigNum;        // May lose data if too large

// Using Convert class
string numberString = "42";
int converted = Convert.ToInt32(numberString);

// Using Parse methods
double parsed = double.Parse("3.14");
bool boolean = bool.Parse("true");
```

### Using `as` and `is` Operators

```csharp
object obj = "Hello, World!";

// Safe casting with 'as'
string str = obj as string;
if (str != null)
{
    Console.WriteLine($"String length: {str.Length}");
}

// Type checking with 'is'
if (obj is string text)
{
    Console.WriteLine($"String length: {text.Length}");
}
```

## Nullable Types

Nullable types can represent the normal range of values plus `null`.

```csharp
// Nullable value types
int? nullableInt = null;
double? nullableDouble = 3.14;
bool? nullableBool = null;

// Nullable reference types (C# 8.0+)
string? nullableString = null;

// Checking for null
if (nullableInt.HasValue)
{
    int value = nullableInt.Value;
    Console.WriteLine($"Value: {value}");
}

// Null coalescing operator
int defaultValue = nullableInt ?? 42; // 42 if null

// Null conditional operator
int? length = nullableString?.Length; // null if string is null
```

## Basic Input/Output Operations

### Console Output

```csharp
// Simple output
Console.WriteLine("Hello, World!");
Console.Write("No newline");

// Formatted output
int age = 25;
Console.WriteLine("Age: {0}", age);
Console.WriteLine($"Age: {age}"); // String interpolation

// Multiple values
string name = "Alice";
Console.WriteLine("Name: {0}, Age: {1}", name, age);
```

### Console Input

```csharp
// Reading strings
Console.Write("Enter your name: ");
string name = Console.ReadLine();

// Reading numbers (with validation)
Console.Write("Enter your age: ");
string ageInput = Console.ReadLine();

if (int.TryParse(ageInput, out int age))
{
    Console.WriteLine($"Hello, {name}! You are {age} years old.");
}
else
{
    Console.WriteLine("Invalid age entered.");
}
```

## Comprehensive Examples

### Example 1: Temperature Converter

```csharp
using System;

class TemperatureConverter
{
    static void Main()
    {
        Console.Write("Enter temperature in Celsius: ");
        string input = Console.ReadLine();

        if (double.TryParse(input, out double celsius))
        {
            double fahrenheit = (celsius * 9 / 5) + 32;
            Console.WriteLine($"{celsius}°C = {fahrenheit:F2}°F");
        }
        else
        {
            Console.WriteLine("Invalid temperature entered.");
        }
    }
}
```

### Example 2: Simple Calculator

```csharp
using System;

class Calculator
{
    static void Main()
    {
        Console.Write("Enter first number: ");
        double num1 = double.Parse(Console.ReadLine());

        Console.Write("Enter operator (+, -, *, /): ");
        char operation = char.Parse(Console.ReadLine());

        Console.Write("Enter second number: ");
        double num2 = double.Parse(Console.ReadLine());

        double result = 0;
        bool validOperation = true;

        switch (operation)
        {
            case '+':
                result = num1 + num2;
                break;
            case '-':
                result = num1 - num2;
                break;
            case '*':
                result = num1 * num2;
                break;
            case '/':
                if (num2 != 0)
                {
                    result = num1 / num2;
                }
                else
                {
                    Console.WriteLine("Cannot divide by zero!");
                    validOperation = false;
                }
                break;
            default:
                Console.WriteLine("Invalid operation!");
                validOperation = false;
                break;
        }

        if (validOperation)
        {
            Console.WriteLine($"{num1} {operation} {num2} = {result}");
        }
    }
}
```

### Example 3: Data Type Explorer

```csharp
using System;

class DataTypeExplorer
{
    static void Main()
    {
        // Integer types
        Console.WriteLine("=== Integer Types ===");
        sbyte sb = -100;
        byte b = 255;
        short s = -32768;
        ushort us = 65535;
        int i = -2147483648;
        uint ui = 4294967295U;
        long l = -9223372036854775808L;
        ulong ul = 18446744073709551615UL;

        Console.WriteLine($"sbyte: {sb} (Range: {sbyte.MinValue} to {sbyte.MaxValue})");
        Console.WriteLine($"byte: {b} (Range: {byte.MinValue} to {byte.MaxValue})");
        Console.WriteLine($"short: {s} (Range: {short.MinValue} to {short.MaxValue})");
        Console.WriteLine($"ushort: {us} (Range: {ushort.MinValue} to {ushort.MaxValue})");
        Console.WriteLine($"int: {i} (Range: {int.MinValue} to {int.MaxValue})");
        Console.WriteLine($"uint: {ui} (Range: {uint.MinValue} to {uint.MaxValue})");
        Console.WriteLine($"long: {l} (Range: {long.MinValue} to {long.MaxValue})");
        Console.WriteLine($"ulong: {ul} (Range: {ulong.MinValue} to {ulong.MaxValue})");

        // Floating-point types
        Console.WriteLine("\n=== Floating-Point Types ===");
        float f = 3.14159f;
        double d = 3.141592653589793;
        decimal m = 3.1415926535897932384626433833m;

        Console.WriteLine($"float: {f} (Precision: ~7 digits)");
        Console.WriteLine($"double: {d} (Precision: ~15-16 digits)");
        Console.WriteLine($"decimal: {m} (Precision: 28-29 digits)");

        // Boolean and character
        Console.WriteLine("\n=== Boolean and Character ===");
        bool boolean = true;
        char character = 'A';

        Console.WriteLine($"bool: {boolean}");
        Console.WriteLine($"char: {character} (Unicode: {(int)character})");

        // String operations
        Console.WriteLine("\n=== String Operations ===");
        string text = "Hello, C# World!";
        Console.WriteLine($"Original: {text}");
        Console.WriteLine($"Length: {text.Length}");
        Console.WriteLine($"Uppercase: {text.ToUpper()}");
        Console.WriteLine($"Lowercase: {text.ToLower()}");
        Console.WriteLine($"Contains 'C#': {text.Contains("C#")}");
        Console.WriteLine($"Substring: {text.Substring(7, 2)}");
    }
}
```

## Best Practices

1. **Choose Appropriate Data Types**
   - Use `int` for most integer calculations
   - Use `double` for general floating-point math
   - Use `decimal` for financial calculations
   - Use `string` for text, `char` for single characters

2. **Validate Input**
   - Always validate user input before conversion
   - Use `TryParse` methods instead of `Parse` to avoid exceptions
   - Provide meaningful error messages

3. **Use Meaningful Variable Names**
   - `userAge` instead of `x`
   - `totalPrice` instead of `tp`
   - `isValidEmail` instead of `valid`

4. **Initialize Variables**
   - Initialize variables when declared
   - Use meaningful default values
   - Consider using nullable types when appropriate

5. **Be Careful with Type Conversions**
   - Avoid unnecessary casting
   - Use explicit casting only when necessary
   - Be aware of potential data loss

## Common Pitfalls

1. **Integer Division**
   ```csharp
   int result = 5 / 2; // Result is 2, not 2.5
   double correct = 5.0 / 2; // Result is 2.5
   ```

2. **Floating-Point Precision**
   ```csharp
   double sum = 0.1 + 0.2; // May not be exactly 0.3 due to floating-point representation
   Console.WriteLine(sum); // Outputs: 0.30000000000000004
   ```

3. **String Concatenation in Loops**
   ```csharp
   // Inefficient - creates new string each iteration
   string result = "";
   for (int i = 0; i < 1000; i++)
   {
       result += i.ToString();
   }

   // Better - use StringBuilder
   var builder = new StringBuilder();
   for (int i = 0; i < 1000; i++)
   {
       builder.Append(i);
   }
   string efficient = builder.ToString();
   ```

## Exercises

1. **Variable Declaration Practice**
   - Declare variables of all primitive types
   - Initialize them with appropriate values
   - Print their values and ranges

2. **Type Conversion Exercises**
   - Convert between different numeric types
   - Handle potential overflow scenarios
   - Practice safe parsing of user input

3. **Calculator Enhancement**
   - Extend the calculator to support more operations
   - Add input validation
   - Handle division by zero gracefully

4. **String Manipulation**
   - Create a program that analyzes text input
   - Count words, characters, and sentences
   - Find and replace text patterns

5. **Data Type Quiz**
   - Create a quiz program that tests knowledge of data types
   - Ask questions about ranges and precision
   - Provide feedback on answers

6. **Temperature Conversion Program**
   - Support conversion between Celsius, Fahrenheit, and Kelvin
   - Allow batch conversions
   - Format output nicely

Remember, mastering these fundamental concepts will make learning more advanced topics much easier. Practice regularly and experiment with different scenarios to build your understanding!
