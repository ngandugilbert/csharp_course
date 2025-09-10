# Task Management System

A complete console-based task management application demonstrating C# concepts from the course.

## Features

- ✅ Add tasks with title, description, priority, and due date
- ✅ View all tasks sorted by priority
- ✅ Mark tasks as completed
- ✅ Delete tasks
- ✅ Search tasks by keyword
- ✅ JSON file persistence (data saved automatically)
- ✅ Input validation and error handling

## How to Run

1. Navigate to the project directory:
   ```bash
   cd course/examples/11_task_management_system
   ```

2. Build the project:
   ```bash
   dotnet build
   ```

3. Run the application:
   ```bash
   dotnet run
   ```

## Usage

The application provides a simple menu-driven interface:

1. **Add Task** - Create a new task with title, description, priority, and optional due date
2. **View All Tasks** - Display all tasks sorted by priority
3. **Mark Task as Completed** - Mark a task as done using its ID
4. **Delete Task** - Remove a task using its ID
5. **Search Tasks** - Find tasks containing a specific keyword
6. **Exit** - Close the application

## Data Persistence

Tasks are automatically saved to `tasks.json` in the same directory as the executable. The data persists between application runs.

## Concepts Demonstrated

This project showcases:

- **Classes and Objects** - TaskItem class with properties and methods
- **OOP Principles** - Encapsulation, inheritance (enum), polymorphism
- **Collections & Generics** - List<TaskItem> for data storage
- **File I/O** - JSON serialization for data persistence
- **LINQ** - Querying and sorting tasks
- **Exception Handling** - Try-catch blocks for file operations
- **User Interface** - Console-based menu system
- **Input Validation** - Checking user input for correctness

## Project Structure

- `Program.cs` - Application entry point
- `TaskItem.cs` - Data model for tasks
- `TaskService.cs` - Business logic and data management
- `ConsoleUI.cs` - User interface and input handling
- `TaskManagementSystem.csproj` - Project configuration
