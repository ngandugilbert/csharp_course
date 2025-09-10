# Module 1 — Introduction & Setup

## Overview

Welcome to your journey into C# programming! This module provides a comprehensive introduction to C# and the .NET ecosystem, guiding you through the installation process and helping you set up your development environment. By the end of this module, you'll have a solid understanding of what C# is, how it fits into the .NET platform, and you'll be ready to write and run your first programs.

## Topics Covered

- What is C# and its history
- Understanding the .NET ecosystem
- .NET runtimes and frameworks
- Installing the .NET SDK
- Setting up your development environment
- Creating your first C# program
- Understanding project structure
- Basic dotnet CLI commands

## What is C#?

C# (pronounced "C Sharp") is a modern, object-oriented programming language developed by Microsoft as part of the .NET initiative. It's designed to be simple, powerful, and type-safe, making it an excellent choice for building a wide variety of applications.

### Key Characteristics of C#

- **Object-Oriented**: Everything in C# is an object, supporting encapsulation, inheritance, and polymorphism
- **Type-Safe**: Strong typing helps catch errors at compile-time rather than runtime
- **Modern Syntax**: Clean, readable syntax with features like properties, events, and LINQ
- **Cross-Platform**: Runs on Windows, macOS, Linux, and even in browsers via WebAssembly
- **Versatile**: Used for web applications, desktop apps, mobile apps, games, and more

### Brief History

- **2000**: C# 1.0 released with .NET Framework 1.0
- **2005**: C# 2.0 introduced generics and anonymous methods
- **2007**: C# 3.0 brought LINQ, lambda expressions, and extension methods
- **2010**: C# 4.0 added dynamic typing and named parameters
- **2012**: C# 5.0 introduced async/await for asynchronous programming
- **2015**: C# 6.0 focused on productivity improvements
- **2017**: C# 7.0 added pattern matching and tuples
- **2019**: C# 8.0 introduced nullable reference types and async streams
- **2020**: C# 9.0 brought records and top-level statements
- **2021**: C# 10.0 enhanced struct functionality and global usings
- **2022**: C# 11.0 introduced raw string literals and generic attributes
- **2023**: C# 12.0 added primary constructors and collection expressions

## The .NET Ecosystem

.NET is a free, open-source, cross-platform framework for building modern applications. It consists of several key components:

### .NET SDK (Software Development Kit)

The .NET SDK includes everything you need to develop .NET applications:
- .NET CLI (Command Line Interface)
- .NET runtime
- Libraries and frameworks
- Language compilers (C#, F#, VB.NET)

### .NET Runtime

The runtime executes your compiled .NET code. There are different runtimes for different scenarios:

- **.NET Runtime**: For running framework-dependent applications
- **ASP.NET Core Runtime**: For running web applications
- **.NET Desktop Runtime**: For running desktop applications (Windows only)

### Target Frameworks

When creating a .NET project, you specify a target framework:

- **.NET 8.0**: Latest LTS (Long Term Support) version, recommended for new projects
- **.NET 7.0**: Previous version, still supported
- **.NET 6.0**: LTS version, widely used
- **.NET Core 3.1**: Legacy LTS version
- **.NET Framework 4.8**: Windows-only legacy framework

### Key Frameworks and Libraries

- **ASP.NET Core**: For building web applications and APIs
- **Entity Framework Core**: Object-relational mapping (ORM)
- **Blazor**: For building web UIs with C#
- **MAUI**: For building cross-platform mobile and desktop apps
- **SignalR**: Real-time web functionality
- **System.Text.Json**: High-performance JSON processing

## Installing the .NET SDK

### Windows Installation

1. **Download the Installer**
   - Visit https://dotnet.microsoft.com/download
   - Download the latest .NET SDK for Windows
   - Choose the x64 version for most modern systems

2. **Run the Installer**
   - Execute the downloaded .exe file
   - Follow the installation wizard
   - The installer will add .NET to your PATH automatically

3. **Verify Installation**
   ```bash
   dotnet --version
   dotnet --info
   ```

### macOS Installation 
Since you are using Rider, you can skip this part. That is if you managed to set it up.

1. **Using Homebrew (Recommended)**
   ```bash
   brew install --cask dotnet-sdk
   ```

2. **Using Installer**
   - Download from https://dotnet.microsoft.com/download
   - Run the .pkg installer
   - Follow the installation prompts

3. **Verify Installation**
   ```bash
   dotnet --version
   dotnet --info
   ```

### Linux Installation

1. **Ubuntu/Debian**
   ```bash
   # Add Microsoft package repository
   wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
   sudo dpkg -i packages-microsoft-prod.deb
   rm packages-microsoft-prod.deb

   # Install SDK
   sudo apt-get update
   sudo apt-get install -y dotnet-sdk-8.0
   ```

2. **CentOS/RHEL/Fedora**
   ```bash
   # Add Microsoft package repository
   sudo rpm -Uvh https://packages.microsoft.com/config/rhel/8/packages-microsoft-prod.rpm

   # Install SDK
   sudo dnf install dotnet-sdk-8.0
   ```

3. **Verify Installation**
   ```bash
   dotnet --version
   dotnet --info
   ```

## Setting Up Your Development Environment

### Choosing an IDE

1. **Visual Studio Code (Recommended for beginners)**
   - Free, lightweight, cross-platform
   - Excellent C# support with extensions
   - Great for learning and small projects

2. **Visual Studio 2022 (Professional)**
   - Full-featured IDE from Microsoft
   - Best for large projects and enterprise development
   - Windows-only (with some cross-platform capabilities)

3. **JetBrains Rider**
   - Cross-platform .NET IDE
   - Excellent code analysis and refactoring tools
   - Commercial product with free educational licenses

### VS Code Setup for C#

1. **Install VS Code**
   - Download from https://code.visualstudio.com/
   - Install the appropriate version for your OS

2. **Install C# Extension**
   - Open VS Code
   - Go to Extensions (Ctrl+Shift+X)
   - Search for "C#" and install the official Microsoft C# extension

3. **Install Additional Extensions**
   - C# Dev Kit (provides additional C# development tools)
   - .NET Install Tool (for managing .NET versions)
   - IntelliCode for C# (AI-assisted code completion)

## Your First C# Program

Let's create and run your first C# program using the .NET CLI.

### Creating a New Console Application

1. **Open Terminal/Command Prompt**
   - Navigate to your desired project directory

2. **Create New Project**
   ```bash
   dotnet new console -n HelloWorld
   cd HelloWorld
   ```

3. **Examine Project Structure**
   ```
   HelloWorld/
   ├── HelloWorld.csproj    # Project file
   ├── Program.cs          # Main source file
   └── obj/                # Build artifacts
   ```

4. **Run the Application**
   ```bash
   dotnet run
   ```

### Understanding the Default Program

The default `Program.cs` file contains:

```csharp
// Program.cs
Console.WriteLine("Hello, World!");
```

This simple program demonstrates:
- **Top-level statements**: C# 9.0+ feature allowing code outside of a class/method
- **Console.WriteLine()**: Method to output text to the console
- **String literals**: Text enclosed in double quotes

### Enhanced Hello World Example

Let's create a more comprehensive first program:

```csharp
using System;

// Our first C# program with multiple concepts
class Program
{
    static void Main(string[] args)
    {
        // Variables and data types
        string name = "Developer";
        int year = DateTime.Now.Year;
        double version = 8.0;

        // String interpolation
        Console.WriteLine($"Hello, {name}!");
        Console.WriteLine($"Welcome to C# {version} in {year}");

        // Basic arithmetic
        int a = 10;
        int b = 20;
        int sum = a + b;

        Console.WriteLine($"{a} + {b} = {sum}");

        // Conditional logic
        if (year >= 2023)
        {
            Console.WriteLine("You're learning a modern programming language!");
        }

        // Arrays and loops
        string[] features = { "Object-Oriented", "Type-Safe", "Cross-Platform", "Modern" };

        Console.WriteLine("\nC# Features:");
        for (int i = 0; i < features.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {features[i]}");
        }

        // Wait for user input before closing
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
```

### Understanding the Enhanced Example

This program demonstrates several fundamental C# concepts:

1. **Using Directives**: `using System;` imports namespaces
2. **Class Declaration**: `class Program` defines a class
3. **Main Method**: Entry point of the application
4. **Variables**: Declaration and initialization
5. **String Interpolation**: `$"..."` syntax for embedding expressions
6. **DateTime**: Working with dates and times
7. **Arithmetic Operations**: Basic math
8. **Conditional Statements**: `if` statements
9. **Arrays**: Fixed-size collections
10. **For Loops**: Iterating through collections
11. **Console Input/Output**: Reading from and writing to console

## .NET CLI Basics

The .NET CLI is your primary tool for creating, building, and running .NET applications.

### Essential Commands

1. **Create New Projects**
   ```bash
   # Console application
   dotnet new console

   # Class library
   dotnet new classlib

   # ASP.NET Core web app
   dotnet new web

   # Blazor WebAssembly app
   dotnet new blazorwasm
   ```

2. **Build Projects**
   ```bash
   # Build in debug mode
   dotnet build

   # Build in release mode
   dotnet build --configuration Release
   ```

3. **Run Applications**
   ```bash
   # Run the application
   dotnet run

   # Run with specific configuration
   dotnet run --configuration Release
   ```

4. **Manage Dependencies**
   ```bash
   # Add a NuGet package
   dotnet add package Newtonsoft.Json

   # Remove a package
   dotnet remove package Newtonsoft.Json

   # Restore packages
   dotnet restore
   ```

5. **Project Management**
   ```bash
   # Clean build artifacts
   dotnet clean

   # Publish application
   dotnet publish

   # Create solution file
   dotnet new sln

   # Add project to solution
   dotnet sln add MyProject.csproj
   ```

### Understanding Project Files

The `.csproj` file defines your project configuration:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Some.Package" Version="1.0.0" />
  </ItemGroup>

</Project>
```

Key elements:
- **TargetFramework**: Specifies the .NET version
- **OutputType**: Exe for executables, Library for libraries
- **PackageReference**: NuGet package dependencies

## Understanding Compilation and Execution

### The Build Process

1. **Source Code** (.cs files) → **Compiler** → **IL Code** (.dll/.exe)
2. **IL Code** + **CLR** → **Native Code** → **Execution**

### Key Concepts

- **IL (Intermediate Language)**: Platform-independent bytecode
- **CLR (Common Language Runtime)**: Virtual machine that executes IL
- **JIT (Just-In-Time) Compiler**: Converts IL to native code at runtime
- **Assemblies**: .dll or .exe files containing compiled IL code

## Best Practices for Getting Started

1. **Start Small**: Begin with simple console applications
2. **Experiment**: Try modifying the examples and see what happens
3. **Read Error Messages**: They often contain helpful information
4. **Use Documentation**: Microsoft's documentation is excellent
5. **Practice Regularly**: Consistent practice is key to learning

## Common Issues and Solutions

### "dotnet command not found"
- Ensure .NET SDK is installed
- Restart your terminal/command prompt
- Check that .NET is in your PATH

### "The framework 'Microsoft.NETCore.App' was not found"
- Install the correct .NET runtime/SDK version
- Check `dotnet --info` for installed versions
- Update your project to target an installed framework

### Build Errors
- Check for syntax errors (missing semicolons, brackets)
- Ensure all required packages are installed
- Verify target framework compatibility

## Next Steps

Now that you have C# and .NET set up, you're ready to dive deeper into the language. In the next module, we'll explore basic syntax and data types, building on the foundation you've established here.

## Exercises

1. **Environment Setup Verification**
   - Verify your .NET installation with `dotnet --info`
   - Create and run the enhanced Hello World program
   - Experiment with different `dotnet new` templates

2. **Project Exploration**
   - Create a new console project and examine all generated files
   - Modify the default program and observe the changes
   - Try building in both Debug and Release configurations

3. **CLI Mastery**
   - Practice creating different types of projects
   - Learn to add and remove NuGet packages
   - Experiment with various dotnet CLI options

4. **Documentation Dive**
   - Visit https://learn.microsoft.com/dotnet/
   - Read about .NET fundamentals
   - Explore the C# language reference

5. **Personal Project**
   - Create a program that displays information about yourself
   - Include your name, favorite programming languages, and current year
   - Use variables, string interpolation, and basic arithmetic

Remember, the key to learning programming is consistent practice and experimentation. Don't be afraid to break things and learn from the errors!
