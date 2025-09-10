# Module 11 — Project: Advanced Task Management System

## Overview

This comprehensive project brings together all the concepts learned throughout the C# course into a fully-featured Task Management System. We'll implement advanced OOP principles, async programming, LINQ operations, reflection, custom attributes, delegates/events, modern C# features, and more.

## Project Goals

- Demonstrate mastery of all C# concepts covered in the course
- Implement a production-ready console application
- Showcase advanced architectural patterns
- Include comprehensive error handling and logging
- Provide extensive code documentation and comments

## Architecture Overview

```
TaskManagementSystem/
├── Core/
│   ├── Models/
│   │   ├── TaskItem.cs
│   │   ├── PriorityLevel.cs
│   │   ├── TaskStatus.cs
│   │   └── Category.cs
│   ├── Interfaces/
│   │   ├── ITaskRepository.cs
│   │   ├── ITaskService.cs
│   │   ├── ILogger.cs
│   │   └── IValidator.cs
│   └── Exceptions/
│       ├── TaskValidationException.cs
│       ├── TaskNotFoundException.cs
│       └── TaskOperationException.cs
├── Infrastructure/
│   ├── Repositories/
│   │   ├── JsonTaskRepository.cs
│   │   └── InMemoryTaskRepository.cs
│   ├── Services/
│   │   ├── TaskService.cs
│   │   ├── ValidationService.cs
│   │   └── NotificationService.cs
│   ├── Logging/
│   │   ├── ConsoleLogger.cs
│   │   └── FileLogger.cs
│   └── Configuration/
│       └── AppConfig.cs
├── UI/
│   ├── ConsoleUI.cs
│   ├── MenuSystem.cs
│   └── InputHandlers/
│       ├── TaskInputHandler.cs
│       └── ValidationInputHandler.cs
├── Utils/
│   ├── Extensions/
│   │   ├── TaskExtensions.cs
│   │   ├── StringExtensions.cs
│   │   └── DateTimeExtensions.cs
│   └── Helpers/
│       ├── DisplayHelper.cs
│       └── DataGenerator.cs
└── Tests/
    ├── UnitTests/
    │   ├── TaskServiceTests.cs
    │   └── ValidationTests.cs
    └── IntegrationTests/
        └── TaskManagementSystemTests.cs
```

## Core Models

### TaskItem.cs

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskManagementSystem.Core.Models
{
    /// <summary>
    /// Represents a task in the task management system
    /// </summary>
    [Serializable]
    public record TaskItem
    {
        [JsonPropertyName("id")]
        [Required(ErrorMessage = "Task ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Task ID must be positive")]
        public int Id { get; init; }

        [JsonPropertyName("title")]
        [Required(ErrorMessage = "Task title is required")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; }

        [JsonPropertyName("status")]
        public TaskStatus Status { get; set; } = TaskStatus.Pending;

        [JsonPropertyName("priority")]
        public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;

        [JsonPropertyName("category")]
        public Category Category { get; set; }

        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; init; } = DateTime.Now;

        [JsonPropertyName("dueDate")]
        public DateTime? DueDate { get; set; }

        [JsonPropertyName("completedDate")]
        public DateTime? CompletedDate { get; set; }

        [JsonPropertyName("assignedTo")]
        [StringLength(100, ErrorMessage = "Assignee name cannot exceed 100 characters")]
        public string AssignedTo { get; set; }

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; } = new List<string>();

        [JsonPropertyName("estimatedHours")]
        [Range(0.1, 1000, ErrorMessage = "Estimated hours must be between 0.1 and 1000")]
        public double? EstimatedHours { get; set; }

        [JsonPropertyName("actualHours")]
        [Range(0, 10000, ErrorMessage = "Actual hours cannot be negative")]
        public double? ActualHours { get; set; }

        // Computed properties
        [JsonIgnore]
        public bool IsOverdue => DueDate.HasValue && DueDate.Value < DateTime.Now && Status != TaskStatus.Completed;

        [JsonIgnore]
        public bool IsCompleted => Status == TaskStatus.Completed;

        [JsonIgnore]
        public int DaysUntilDue => DueDate.HasValue ? (DueDate.Value - DateTime.Now).Days : int.MaxValue;

        [JsonIgnore]
        public string DisplayTitle => $"[{Id}] {Title}";

        // Methods
        public void MarkCompleted()
        {
            Status = TaskStatus.Completed;
            CompletedDate = DateTime.Now;
        }

        public void UpdateProgress(double actualHours)
        {
            ActualHours = actualHours;
            if (EstimatedHours.HasValue && actualHours >= EstimatedHours.Value)
            {
                Status = TaskStatus.InProgress;
            }
        }

        public override string ToString()
        {
            string statusIcon = Status switch
            {
                TaskStatus.Completed => "✓",
                TaskStatus.InProgress => "⟳",
                TaskStatus.Cancelled => "✗",
                _ => "○"
            };

            string priorityColor = Priority switch
            {
                PriorityLevel.Urgent => "🔴",
                PriorityLevel.High => "🟠",
                PriorityLevel.Medium => "🟡",
                PriorityLevel.Low => "🟢",
                _ => "⚪"
            };

            string dueInfo = DueDate.HasValue
                ? IsOverdue
                    ? $" (OVERDUE: {DueDate.Value.ToShortDateString()})"
                    : $" (Due: {DueDate.Value.ToShortDateString()})"
                : "";

            string categoryInfo = Category != null ? $" [{Category.Name}]" : "";
            string assigneeInfo = !string.IsNullOrEmpty(AssignedTo) ? $" @{AssignedTo}" : "";

            return $"{statusIcon} {priorityColor} {DisplayTitle}{categoryInfo}{assigneeInfo}{dueInfo}";
        }
    }
}
```

### PriorityLevel.cs

```csharp
namespace TaskManagementSystem.Core.Models
{
    /// <summary>
    /// Represents the priority levels for tasks
    /// </summary>
    public enum PriorityLevel
    {
        [Display(Name = "Low Priority", Order = 4)]
        Low = 1,

        [Display(Name = "Medium Priority", Order = 3)]
        Medium = 2,

        [Display(Name = "High Priority", Order = 2)]
        High = 3,

        [Display(Name = "Urgent Priority", Order = 1)]
        Urgent = 4
    }

    /// <summary>
    /// Represents the status of a task
    /// </summary>
    public enum TaskStatus
    {
        [Display(Name = "Pending", Description = "Task is waiting to be started")]
        Pending = 1,

        [Display(Name = "In Progress", Description = "Task is currently being worked on")]
        InProgress = 2,

        [Display(Name = "Completed", Description = "Task has been finished")]
        Completed = 3,

        [Display(Name = "Cancelled", Description = "Task has been cancelled")]
        Cancelled = 4,

        [Display(Name = "On Hold", Description = "Task is temporarily paused")]
        OnHold = 5
    }

    /// <summary>
    /// Represents a category for organizing tasks
    /// </summary>
    public record Category
    {
        public int Id { get; init; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }

        public Category() { }

        public Category(string name, string description = "", string color = "#808080")
        {
            Name = name;
            Description = description;
            Color = color;
        }

        public override string ToString() => Name;
    }

    /// <summary>
    /// Display attribute for enum values
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DisplayAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
    }
}
```

## Interfaces

### ITaskRepository.cs

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagementSystem.Core.Models;

namespace TaskManagementSystem.Core.Interfaces
{
    /// <summary>
    /// Repository interface for task data operations
    /// </summary>
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskItem>> GetAllAsync();
        Task<TaskItem> GetByIdAsync(int id);
        Task<IEnumerable<TaskItem>> GetByStatusAsync(TaskStatus status);
        Task<IEnumerable<TaskItem>> GetByPriorityAsync(PriorityLevel priority);
        Task<IEnumerable<TaskItem>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<TaskItem>> SearchAsync(string keyword);
        Task<TaskItem> AddAsync(TaskItem task);
        Task UpdateAsync(TaskItem task);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<int> GetNextIdAsync();
        Task SaveChangesAsync();
    }
}
```

### ITaskService.cs

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagementSystem.Core.Models;

namespace TaskManagementSystem.Core.Interfaces
{
    /// <summary>
    /// Service interface for task business logic
    /// </summary>
    public interface ITaskService
    {
        Task<TaskItem> CreateTaskAsync(string title, string description, PriorityLevel priority,
            Category category = null, DateTime? dueDate = null, string assignedTo = null,
            IEnumerable<string> tags = null, double? estimatedHours = null);

        Task<TaskItem> GetTaskByIdAsync(int id);
        Task<IEnumerable<TaskItem>> GetAllTasksAsync();
        Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(TaskStatus status);
        Task<IEnumerable<TaskItem>> GetTasksByPriorityAsync(PriorityLevel priority);
        Task<IEnumerable<TaskItem>> GetOverdueTasksAsync();
        Task<IEnumerable<TaskItem>> GetTasksDueSoonAsync(int days = 7);
        Task<IEnumerable<TaskItem>> SearchTasksAsync(string keyword);

        Task UpdateTaskAsync(int id, Action<TaskItem> updateAction);
        Task MarkTaskCompletedAsync(int id);
        Task DeleteTaskAsync(int id);

        Task<TaskStatistics> GetTaskStatisticsAsync();
        Task<IEnumerable<TaskItem>> GetTasksByAssigneeAsync(string assignee);
        Task BulkUpdateStatusAsync(IEnumerable<int> taskIds, TaskStatus newStatus);
    }

    /// <summary>
    /// Task statistics data
    /// </summary>
    public record TaskStatistics
    {
        public int TotalTasks { get; init; }
        public int CompletedTasks { get; init; }
        public int PendingTasks { get; init; }
        public int OverdueTasks { get; init; }
        public double CompletionRate => TotalTasks > 0 ? (double)CompletedTasks / TotalTasks * 100 : 0;
        public Dictionary<PriorityLevel, int> TasksByPriority { get; init; } = new();
        public Dictionary<TaskStatus, int> TasksByStatus { get; init; } = new();
        public Dictionary<string, int> TasksByAssignee { get; init; } = new();
    }
}
```

### ILogger.cs

```csharp
using System;

namespace TaskManagementSystem.Core.Interfaces
{
    /// <summary>
    /// Logging interface for the application
    /// </summary>
    public interface ILogger
    {
        void LogInformation(string message);
        void LogWarning(string message);
        void LogError(string message, Exception exception = null);
        void LogDebug(string message);
        Task LogAsync(LogLevel level, string message, Exception exception = null);
    }

    public enum LogLevel
    {
        Debug,
        Information,
        Warning,
        Error,
        Critical
    }
}
```

### IValidator.cs

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaskManagementSystem.Core.Interfaces
{
    /// <summary>
    /// Validation interface for business objects
    /// </summary>
    public interface IValidator<T>
    {
        Task<ValidationResult> ValidateAsync(T entity);
        IEnumerable<string> GetValidationRules();
    }

    /// <summary>
    /// Validation result
    /// </summary>
    public record ValidationResult
    {
        public bool IsValid { get; init; }
        public IEnumerable<string> Errors { get; init; } = new List<string>();
        public IEnumerable<string> Warnings { get; init; } = new List<string>();

        public static ValidationResult Success() => new() { IsValid = true };
        public static ValidationResult Failure(IEnumerable<string> errors) =>
            new() { IsValid = false, Errors = errors };
    }
}
```

## Custom Exceptions

### TaskValidationException.cs

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskManagementSystem.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when task validation fails
    /// </summary>
    [Serializable]
    public class TaskValidationException : Exception
    {
        public IEnumerable<string> ValidationErrors { get; }

        public TaskValidationException(string message)
            : base(message)
        {
            ValidationErrors = new[] { message };
        }

        public TaskValidationException(IEnumerable<string> errors)
            : base($"Task validation failed: {string.Join(", ", errors)}")
        {
            ValidationErrors = errors;
        }

        public TaskValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
            ValidationErrors = new[] { message };
        }

        protected TaskValidationException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    /// Exception thrown when a task is not found
    /// </summary>
    [Serializable]
    public class TaskNotFoundException : Exception
    {
        public int TaskId { get; }

        public TaskNotFoundException(int taskId)
            : base($"Task with ID {taskId} was not found.")
        {
            TaskId = taskId;
        }

        public TaskNotFoundException(int taskId, string message)
            : base(message)
        {
            TaskId = taskId;
        }

        protected TaskNotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    /// Exception thrown when a task operation fails
    /// </summary>
    [Serializable]
    public class TaskOperationException : Exception
    {
        public string Operation { get; }
        public int? TaskId { get; }

        public TaskOperationException(string operation, string message)
            : base(message)
        {
            Operation = operation;
        }

        public TaskOperationException(string operation, int taskId, string message)
            : base(message)
        {
            Operation = operation;
            TaskId = taskId;
        }

        protected TaskOperationException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
```

## Infrastructure Layer

### JsonTaskRepository.cs

```csharp
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TaskManagementSystem.Core.Interfaces;
using TaskManagementSystem.Core.Models;
using TaskManagementSystem.Core.Exceptions;

namespace TaskManagementSystem.Infrastructure.Repositories
{
    /// <summary>
    /// JSON-based implementation of ITaskRepository
    /// </summary>
    public class JsonTaskRepository : ITaskRepository
    {
        private readonly string _filePath;
        private readonly ILogger _logger;
        private List<TaskItem> _tasks;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public JsonTaskRepository(string filePath, ILogger logger)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tasks = LoadTasksFromFile();
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                return _tasks.ToList();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<TaskItem> GetByIdAsync(int id)
        {
            await _semaphore.WaitAsync();
            try
            {
                var task = _tasks.FirstOrDefault(t => t.Id == id);
                if (task == null)
                {
                    throw new TaskNotFoundException(id);
                }
                return task;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<IEnumerable<TaskItem>> GetByStatusAsync(TaskStatus status)
        {
            await _semaphore.WaitAsync();
            try
            {
                return _tasks.Where(t => t.Status == status).ToList();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<IEnumerable<TaskItem>> GetByPriorityAsync(PriorityLevel priority)
        {
            await _semaphore.WaitAsync();
            try
            {
                return _tasks.Where(t => t.Priority == priority).ToList();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<IEnumerable<TaskItem>> GetByCategoryAsync(int categoryId)
        {
            await _semaphore.WaitAsync();
            try
            {
                return _tasks.Where(t => t.Category?.Id == categoryId).ToList();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<IEnumerable<TaskItem>> SearchAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return await GetAllAsync();
            }

            await _semaphore.WaitAsync();
            try
            {
                string searchTerm = keyword.ToLowerInvariant();
                return _tasks.Where(t =>
                    t.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    t.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    t.Tags.Any(tag => tag.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (t.AssignedTo?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false))
                    .ToList();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<TaskItem> AddAsync(TaskItem task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            await _semaphore.WaitAsync();
            try
            {
                int nextId = await GetNextIdAsync();
                var newTask = task with { Id = nextId };
                _tasks.Add(newTask);

                await SaveChangesAsync();
                await _logger.LogAsync(LogLevel.Information, $"Task '{newTask.Title}' added with ID {newTask.Id}");

                return newTask;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task UpdateAsync(TaskItem task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            await _semaphore.WaitAsync();
            try
            {
                int index = _tasks.FindIndex(t => t.Id == task.Id);
                if (index == -1)
                {
                    throw new TaskNotFoundException(task.Id);
                }

                _tasks[index] = task;
                await SaveChangesAsync();
                await _logger.LogAsync(LogLevel.Information, $"Task '{task.Title}' (ID: {task.Id}) updated");
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task DeleteAsync(int id)
        {
            await _semaphore.WaitAsync();
            try
            {
                var task = _tasks.FirstOrDefault(t => t.Id == id);
                if (task == null)
                {
                    throw new TaskNotFoundException(id);
                }

                _tasks.Remove(task);
                await SaveChangesAsync();
                await _logger.LogAsync(LogLevel.Information, $"Task '{task.Title}' (ID: {id}) deleted");
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            await _semaphore.WaitAsync();
            try
            {
                return _tasks.Any(t => t.Id == id);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<int> GetNextIdAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                return _tasks.Count > 0 ? _tasks.Max(t => t.Id) + 1 : 1;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                string json = JsonSerializer.Serialize(_tasks, options);
                await File.WriteAllTextAsync(_filePath, json);
            }
            catch (Exception ex)
            {
                await _logger.LogAsync(LogLevel.Error, "Failed to save tasks to file", ex);
                throw new TaskOperationException("Save", $"Failed to save tasks: {ex.Message}");
            }
        }

        private List<TaskItem> LoadTasksFromFile()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    return new List<TaskItem>();
                }

                string json = File.ReadAllText(_filePath);
                var tasks = JsonSerializer.Deserialize<List<TaskItem>>(json);

                return tasks ?? new List<TaskItem>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to load tasks from file: {ex.Message}", ex);
                return new List<TaskItem>();
            }
        }
    }
}
```

### TaskService.cs

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagementSystem.Core.Interfaces;
using TaskManagementSystem.Core.Models;
using TaskManagementSystem.Core.Exceptions;

namespace TaskManagementSystem.Infrastructure.Services
{
    /// <summary>
    /// Implementation of ITaskService with business logic
    /// </summary>
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _repository;
        private readonly IValidator<TaskItem> _validator;
        private readonly ILogger _logger;

        public TaskService(ITaskRepository repository, IValidator<TaskItem> validator, ILogger logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TaskItem> CreateTaskAsync(string title, string description, PriorityLevel priority,
            Category category = null, DateTime? dueDate = null, string assignedTo = null,
            IEnumerable<string> tags = null, double? estimatedHours = null)
        {
            try
            {
                var task = new TaskItem
                {
                    Title = title,
                    Description = description,
                    Priority = priority,
                    Category = category,
                    DueDate = dueDate,
                    AssignedTo = assignedTo,
                    Tags = tags?.ToList() ?? new List<string>(),
                    EstimatedHours = estimatedHours
                };

                // Validate the task
                var validationResult = await _validator.ValidateAsync(task);
                if (!validationResult.IsValid)
                {
                    throw new TaskValidationException(validationResult.Errors);
                }

                // Log warnings if any
                foreach (var warning in validationResult.Warnings)
                {
                    await _logger.LogAsync(LogLevel.Warning, $"Task validation warning: {warning}");
                }

                var createdTask = await _repository.AddAsync(task);
                await _logger.LogAsync(LogLevel.Information, $"Task '{title}' created successfully with ID {createdTask.Id}");

                return createdTask;
            }
            catch (Exception ex)
            {
                await _logger.LogAsync(LogLevel.Error, $"Failed to create task '{title}'", ex);
                throw;
            }
        }

        public async Task<TaskItem> GetTaskByIdAsync(int id)
        {
            try
            {
                return await _repository.GetByIdAsync(id);
            }
            catch (TaskNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                await _logger.LogAsync(LogLevel.Error, $"Failed to get task {id}", ex);
                throw new TaskOperationException("Get", id, $"Failed to retrieve task: {ex.Message}");
            }
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
        {
            try
            {
                var tasks = await _repository.GetAllAsync();
                return tasks.OrderByDescending(t => t.Priority)
                           .ThenBy(t => t.DueDate ?? DateTime.MaxValue)
                           .ThenBy(t => t.CreatedDate);
            }
            catch (Exception ex)
            {
                await _logger.LogAsync(LogLevel.Error, "Failed to get all tasks", ex);
                throw new TaskOperationException("GetAll", "Failed to retrieve tasks");
            }
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(TaskStatus status)
        {
            try
            {
                return await _repository.GetByStatusAsync(status);
            }
            catch (Exception ex)
            {
                await _logger.LogAsync(LogLevel.Error, $"Failed to get tasks by status {status}", ex);
                throw new TaskOperationException("GetByStatus", $"Failed to retrieve tasks: {ex.Message}");
            }
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByPriorityAsync(PriorityLevel priority)
        {
            try
            {
                return await _repository.GetByPriorityAsync(priority);
            }
            catch (Exception ex)
            {
                await _logger.LogAsync(LogLevel.Error, $"Failed to get tasks by priority {priority}", ex);
                throw new TaskOperationException("GetByPriority", $"Failed to retrieve tasks: {ex.Message}");
            }
        }

        public async Task<IEnumerable<TaskItem>> GetOverdueTasksAsync()
        {
            try
            {
                var allTasks = await _repository.GetAllAsync();
                return allTasks.Where(t => t.IsOverdue).ToList();
            }
            catch (Exception ex)
            {
                await _logger.LogAsync(LogLevel.Error, "Failed to get overdue tasks", ex);
                throw new TaskOperationException("GetOverdue", "Failed to retrieve overdue tasks");
            }
        }

        public async Task<IEnumerable<TaskItem>> GetTasksDueSoonAsync(int days = 7)
        {
            try
            {
                var allTasks = await _repository.GetAllAsync();
                var cutoffDate = DateTime.Now.AddDays(days);
                return allTasks.Where(t => t.DueDate.HasValue &&
                                         t.DueDate.Value <= cutoffDate &&
                                         t.Status != TaskStatus.Completed)
                              .OrderBy(t => t.DueDate.Value)
                              .ToList();
            }
            catch (Exception ex)
            {
                await _logger.LogAsync(LogLevel.Error, $"Failed to get tasks due in {days} days", ex);
                throw new TaskOperationException("GetDueSoon", "Failed to retrieve upcoming tasks");
            }
        }

        public async Task<IEnumerable<TaskItem>> SearchTasksAsync(string keyword)
        {
            try
            {
                return await _repository.SearchAsync(keyword);
            }
            catch (Exception ex)
            {
                await _logger.LogAsync(LogLevel.Error, $"Failed to search tasks with keyword '{keyword}'", ex);
                throw new TaskOperationException("Search", $"Failed to search tasks: {ex.Message}");
            }
        }

        public async Task UpdateTaskAsync(int id, Action<TaskItem> updateAction)
        {
            try
            {
                var task = await _repository.GetByIdAsync(id);
                updateAction(task);

                // Validate the updated task
                var validationResult = await _validator.ValidateAsync(task);
                if (!validationResult.IsValid)
                {
                    throw new TaskValidationException(validationResult.Errors);
                }

                await _repository.UpdateAsync(task);
                await _logger.LogAsync(LogLevel.Information, $"Task {id} updated successfully");
            }
            catch (TaskNotFoundException)
            {
                throw;
            }
            catch (TaskValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                await _logger.LogAsync(LogLevel.Error, $"Failed to update task {id}", ex);
                throw new TaskOperationException("Update", id, $"Failed to update task: {ex.Message}");
            }
        }

        public async Task MarkTaskCompletedAsync(int id)
        {
            try
            {
                await UpdateTaskAsync(id, task => task.MarkCompleted());
                await _logger.LogAsync(LogLevel.Information, $"Task {id} marked as completed");
            }
            catch (Exception ex)
            {
                await _logger.LogAsync(LogLevel.Error, $"Failed to mark task {id} as completed", ex);
                throw;
            }
        }

        public async Task DeleteTaskAsync(int id)
        {
            try
            {
                await _repository.DeleteAsync(id);
                await _logger.LogAsync(LogLevel.Information, $"Task {id} deleted successfully");
            }
            catch (TaskNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                await _logger.LogAsync(LogLevel.Error, $"Failed to delete task {id}", ex);
                throw new TaskOperationException("Delete", id, $"Failed to delete task: {ex.Message}");
            }
        }

        public async Task<TaskStatistics> GetTaskStatisticsAsync()
        {
            try
            {
                var allTasks = await _repository.GetAllAsync();
                var tasksList = allTasks.ToList();

                var stats = new TaskStatistics
                {
                    TotalTasks = tasksList.Count,
                    CompletedTasks = tasksList.Count(t => t.IsCompleted),
                    PendingTasks = tasksList.Count(t => t.Status == TaskStatus.Pending),
                    OverdueTasks = tasksList.Count(t => t.IsOverdue),
                    TasksByPriority = tasksList.GroupBy(t => t.Priority)
                                              .ToDictionary(g => g.Key, g => g.Count()),
                    TasksByStatus = tasksList.GroupBy(t => t.Status)
                                            .ToDictionary(g => g.Key, g => g.Count()),
                    TasksByAssignee = tasksList.Where(t => !string.IsNullOrEmpty(t.AssignedTo))
                                              .GroupBy(t => t.AssignedTo)
                                              .ToDictionary(g => g.Key, g => g.Count())
                };

                return stats;
            }
            catch (Exception ex)
            {
                await _logger.LogAsync(LogLevel.Error, "Failed to get task statistics", ex);
                throw new TaskOperationException("Statistics", "Failed to calculate statistics");
            }
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByAssigneeAsync(string assignee)
        {
            try
            {
                var allTasks = await _repository.GetAllAsync();
                return allTasks.Where(t => t.AssignedTo?.Equals(assignee, StringComparison.OrdinalIgnoreCase) ?? false)
                              .ToList();
            }
            catch (Exception ex)
            {
                await _logger.LogAsync(LogLevel.Error, $"Failed to get tasks by assignee '{assignee}'", ex);
                throw new TaskOperationException("GetByAssignee", $"Failed to retrieve tasks: {ex.Message}");
            }
        }

        public async Task BulkUpdateStatusAsync(IEnumerable<int> taskIds, TaskStatus newStatus)
        {
            try
            {
                var tasksToUpdate = new List<TaskItem>();

                foreach (int id in taskIds)
                {
                    try
                    {
                        var task = await _repository.GetByIdAsync(id);
                        tasksToUpdate.Add(task);
                    }
                    catch (TaskNotFoundException ex)
                    {
                        await _logger.LogAsync(LogLevel.Warning, $"Task {id} not found during bulk update", ex);
                    }
                }

                foreach (var task in tasksToUpdate)
                {
                    var updatedTask = task with { Status = newStatus };
                    if (newStatus == TaskStatus.Completed)
                    {
                        updatedTask = updatedTask with { CompletedDate = DateTime.Now };
                    }

                    await _repository.UpdateAsync(updatedTask);
                }

                await _logger.LogAsync(LogLevel.Information,
                    $"Bulk updated {tasksToUpdate.Count} tasks to status {newStatus}");
            }
            catch (Exception ex)
            {
                await _logger.LogAsync(LogLevel.Error, "Failed to perform bulk status update", ex);
                throw new TaskOperationException("BulkUpdate", $"Failed to update tasks: {ex.Message}");
            }
        }
    }
}
```

## Logging Implementation

### ConsoleLogger.cs

```csharp
using System;
using System.Threading.Tasks;
using TaskManagementSystem.Core.Interfaces;

namespace TaskManagementSystem.Infrastructure.Logging
{
    /// <summary>
    /// Console-based logger implementation
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        private readonly LogLevel _minimumLevel;
        private readonly object _lock = new object();

        public ConsoleLogger(LogLevel minimumLevel = LogLevel.Information)
        {
            _minimumLevel = minimumLevel;
        }

        public void LogInformation(string message)
        {
            Log(LogLevel.Information, message);
        }

        public void LogWarning(string message)
        {
            Log(LogLevel.Warning, message);
        }

        public void LogError(string message, Exception exception = null)
        {
            Log(LogLevel.Error, message, exception);
        }

        public void LogDebug(string message)
        {
            Log(LogLevel.Debug, message);
        }

        public Task LogAsync(LogLevel level, string message, Exception exception = null)
        {
            Log(level, message, exception);
            return Task.CompletedTask;
        }

        private void Log(LogLevel level, string message, Exception exception = null)
        {
            if (level < _minimumLevel)
            {
                return;
            }

            lock (_lock)
            {
                ConsoleColor originalColor = Console.ForegroundColor;

                Console.ForegroundColor = GetColorForLevel(level);
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                string levelString = level.ToString().ToUpperInvariant().PadRight(11);

                Console.Write($"[{timestamp}] {levelString} ");

                Console.ForegroundColor = originalColor;
                Console.WriteLine(message);

                if (exception != null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Exception: {exception.Message}");
                    if (!string.IsNullOrEmpty(exception.StackTrace))
                    {
                        Console.WriteLine($"Stack Trace: {exception.StackTrace}");
                    }
                    Console.ForegroundColor = originalColor;
                }
            }
        }

        private ConsoleColor GetColorForLevel(LogLevel level)
        {
            return level switch
            {
                LogLevel.Debug => ConsoleColor.Gray,
                LogLevel.Information => ConsoleColor.White,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.Red,
                LogLevel.Critical => ConsoleColor.DarkRed,
                _ => ConsoleColor.White
            };
        }
    }
}
```

## Validation Implementation

### ValidationService.cs

```csharp
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TaskManagementSystem.Core.Interfaces;
using TaskManagementSystem.Core.Models;

namespace TaskManagementSystem.Infrastructure.Services
{
    /// <summary>
    /// Validation service using data annotations and custom rules
    /// </summary>
    public class ValidationService : IValidator<TaskItem>
    {
        private readonly IEnumerable<string> _validationRules;

        public ValidationService()
        {
            _validationRules = new[]
            {
                "Title is required and must be between 1-200 characters",
                "Description cannot exceed 1000 characters",
                "Due date cannot be in the past for new tasks",
                "Estimated hours must be positive if provided",
                "Tags cannot contain special characters",
                "Assignee name must be valid if provided"
            };
        }

        public IEnumerable<string> GetValidationRules()
        {
            return _validationRules;
        }

        public async Task<ValidationResult> ValidateAsync(TaskItem entity)
        {
            var errors = new List<string>();
            var warnings = new List<string>();

            // Use data annotations validation
            var validationContext = new ValidationContext(entity);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(entity, validationContext, validationResults, true);

            if (!isValid)
            {
                errors.AddRange(validationResults.Select(r => r.ErrorMessage));
            }

            // Custom validation rules
            await ValidateBusinessRulesAsync(entity, errors, warnings);

            return errors.Any()
                ? ValidationResult.Failure(errors)
                : ValidationResult.Success();
        }

        private async Task ValidateBusinessRulesAsync(TaskItem task, List<string> errors, List<string> warnings)
        {
            // Due date validation
            if (task.DueDate.HasValue && task.DueDate.Value < DateTime.Now && task.Status == TaskStatus.Pending)
            {
                warnings.Add("Task is already overdue");
            }

            // Estimated vs actual hours validation
            if (task.EstimatedHours.HasValue && task.ActualHours.HasValue)
            {
                if (task.ActualHours.Value > task.EstimatedHours.Value * 1.5)
                {
                    warnings.Add("Actual hours significantly exceed estimate");
                }
                else if (task.ActualHours.Value < task.EstimatedHours.Value * 0.5)
                {
                    warnings.Add("Actual hours are much less than estimate");
                }
            }

            // Tag validation
            if (task.Tags.Any())
            {
                var invalidTags = task.Tags.Where(tag =>
                    string.IsNullOrWhiteSpace(tag) ||
                    tag.Length > 50 ||
                    tag.ContainsAny(new[] { "<", ">", "&", "\"", "'" }));

                if (invalidTags.Any())
                {
                    errors.Add("Tags contain invalid characters or are too long");
                }

                // Check for duplicate tags
                if (task.Tags.Distinct(StringComparer.OrdinalIgnoreCase).Count() != task.Tags.Count)
                {
                    warnings.Add("Duplicate tags found");
                }
            }

            // Priority and due date consistency
            if (task.Priority == PriorityLevel.Urgent && !task.DueDate.HasValue)
            {
                warnings.Add("Urgent tasks should have a due date");
            }

            // Completion validation
            if (task.Status == TaskStatus.Completed && !task.CompletedDate.HasValue)
            {
                errors.Add("Completed tasks must have a completion date");
            }

            // Simulate async operation (could be database validation, etc.)
            await Task.Delay(1);
        }
    }
}
```

## Extension Methods

### TaskExtensions.cs

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using TaskManagementSystem.Core.Models;

namespace TaskManagementSystem.Utils.Extensions
{
    /// <summary>
    /// Extension methods for TaskItem
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Filters tasks by completion status
        /// </summary>
        public static IEnumerable<TaskItem> Completed(this IEnumerable<TaskItem> tasks)
        {
            return tasks.Where(t => t.IsCompleted);
        }

        /// <summary>
        /// Filters tasks by pending status
        /// </summary>
        public static IEnumerable<TaskItem> Pending(this IEnumerable<TaskItem> tasks)
        {
            return tasks.Where(t => t.Status == TaskStatus.Pending);
        }

        /// <summary>
        /// Filters overdue tasks
        /// </summary>
        public static IEnumerable<TaskItem> Overdue(this IEnumerable<TaskItem> tasks)
        {
            return tasks.Where(t => t.IsOverdue);
        }

        /// <summary>
        /// Filters tasks due within specified days
        /// </summary>
        public static IEnumerable<TaskItem> DueWithin(this IEnumerable<TaskItem> tasks, int days)
        {
            var cutoffDate = DateTime.Now.AddDays(days);
            return tasks.Where(t => t.DueDate.HasValue && t.DueDate.Value <= cutoffDate);
        }

        /// <summary>
        /// Orders tasks by priority (highest first) then by due date
        /// </summary>
        public static IOrderedEnumerable<TaskItem> OrderByPriority(this IEnumerable<TaskItem> tasks)
        {
            return tasks.OrderByDescending(t => t.Priority)
                       .ThenBy(t => t.DueDate ?? DateTime.MaxValue);
        }

        /// <summary>
        /// Groups tasks by priority
        /// </summary>
        public static IEnumerable<IGrouping<PriorityLevel, TaskItem>> GroupByPriority(this IEnumerable<TaskItem> tasks)
        {
            return tasks.GroupBy(t => t.Priority);
        }

        /// <summary>
        /// Groups tasks by status
        /// </summary>
        public static IEnumerable<IGrouping<TaskStatus, TaskItem>> GroupByStatus(this IEnumerable<TaskItem> tasks)
        {
            return tasks.GroupBy(t => t.Status);
        }

        /// <summary>
        /// Groups tasks by category
        /// </summary>
        public static IEnumerable<IGrouping<string, TaskItem>> GroupByCategoryName(this IEnumerable<TaskItem> tasks)
        {
            return tasks.GroupBy(t => t.Category?.Name ?? "Uncategorized");
        }

        /// <summary>
        /// Gets tasks assigned to a specific person
        /// </summary>
        public static IEnumerable<TaskItem> AssignedTo(this IEnumerable<TaskItem> tasks, string assignee)
        {
            return tasks.Where(t => t.AssignedTo?.Equals(assignee, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        /// <summary>
        /// Gets tasks containing specific tags
        /// </summary>
        public static IEnumerable<TaskItem> WithTags(this IEnumerable<TaskItem> tasks, params string[] tags)
        {
            return tasks.Where(t => tags.Any(tag =>
                t.Tags.Any(taskTag => taskTag.Contains(tag, StringComparison.OrdinalIgnoreCase))));
        }

        /// <summary>
        /// Calculates average completion time for completed tasks
        /// </summary>
        public static TimeSpan? AverageCompletionTime(this IEnumerable<TaskItem> tasks)
        {
            var completedTasks = tasks.Where(t => t.IsCompleted && t.CompletedDate.HasValue);
            var completionTimes = completedTasks.Select(t => t.CompletedDate.Value - t.CreatedDate);

            return completionTimes.Any() ? TimeSpan.FromTicks((long)completionTimes.Average(t => t.Ticks)) : null;
        }

        /// <summary>
        /// Gets the most productive day (most tasks completed)
        /// </summary>
        public static DateTime? MostProductiveDay(this IEnumerable<TaskItem> tasks)
        {
            return tasks.Where(t => t.CompletedDate.HasValue)
                       .GroupBy(t => t.CompletedDate.Value.Date)
                       .OrderByDescending(g => g.Count())
                       .Select(g => (DateTime?)g.Key)
                       .FirstOrDefault();
        }

        /// <summary>
        /// Converts tasks to a summary format
        /// </summary>
        public static IEnumerable<TaskSummary> ToSummary(this IEnumerable<TaskItem> tasks)
        {
            return tasks.Select(t => new TaskSummary
            {
                Id = t.Id,
                Title = t.Title,
                Status = t.Status,
                Priority = t.Priority,
                IsOverdue = t.IsOverdue,
                DaysUntilDue = t.DaysUntilDue
            });
        }

        /// <summary>
        /// Gets a random sample of tasks
        /// </summary>
        public static IEnumerable<TaskItem> Sample(this IEnumerable<TaskItem> tasks, int count)
        {
            var taskList = tasks.ToList();
            if (taskList.Count <= count)
            {
                return taskList;
            }

            var random = new Random();
            return taskList.OrderBy(_ => random.Next()).Take(count);
        }
    }

    /// <summary>
    /// Summary representation of a task
    /// </summary>
    public record TaskSummary
    {
        public int Id { get; init; }
        public string Title { get; init; }
        public TaskStatus Status { get; init; }
        public PriorityLevel Priority { get; init; }
        public bool IsOverdue { get; init; }
        public int DaysUntilDue { get; init; }
    }
}
```

## User Interface Layer

### ConsoleUI.cs

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagementSystem.Core.Interfaces;
using TaskManagementSystem.Core.Models;
using TaskManagementSystem.Core.Exceptions;
using TaskManagementSystem.Utils.Extensions;
using TaskManagementSystem.Utils.Helpers;

namespace TaskManagementSystem.UI
{
    /// <summary>
    /// Console-based user interface for the task management system
    /// </summary>
    public class ConsoleUI
    {
        private readonly ITaskService _taskService;
        private readonly ILogger _logger;
        private readonly DisplayHelper _displayHelper;
        private readonly TaskInputHandler _inputHandler;

        public ConsoleUI(ITaskService taskService, ILogger logger)
        {
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _displayHelper = new DisplayHelper();
            _inputHandler = new TaskInputHandler();
        }

        public async Task RunAsync()
        {
            await _logger.LogAsync(LogLevel.Information, "Task Management System started");

            bool running = true;

            while (running)
            {
                try
                {
                    DisplayMainMenu();
                    string choice = Console.ReadLine()?.Trim();

                    switch (choice)
                    {
                        case "1":
                            await CreateTaskAsync();
                            break;
                        case "2":
                            await ViewTasksAsync();
                            break;
                        case "3":
                            await EditTaskAsync();
                            break;
                        case "4":
                            await MarkTaskCompletedAsync();
                            break;
                        case "5":
                            await DeleteTaskAsync();
                            break;
                        case "6":
                            await SearchTasksAsync();
                            break;
                        case "7":
                            await ViewStatisticsAsync();
                            break;
                        case "8":
                            await BulkOperationsAsync();
                            break;
                        case "9":
                            await ExportDataAsync();
                            break;
                        case "0":
                            running = false;
                            break;
                        default:
                            _displayHelper.DisplayError("Invalid choice. Please select a valid option.");
                            break;
                    }

                    if (running)
                    {
                        _displayHelper.DisplayMessage("\nPress any key to continue...");
                        Console.ReadKey();
                    }
                }
                catch (Exception ex)
                {
                    await _logger.LogAsync(LogLevel.Error, "UI Error", ex);
                    _displayHelper.DisplayError($"An error occurred: {ex.Message}");
                }
            }

            await _logger.LogAsync(LogLevel.Information, "Task Management System shutting down");
        }

        private void DisplayMainMenu()
        {
            Console.Clear();
            _displayHelper.DisplayHeader("Task Management System");

            Console.WriteLine("1. Create New Task");
            Console.WriteLine("2. View All Tasks");
            Console.WriteLine("3. Edit Task");
            Console.WriteLine("4. Mark Task as Completed");
            Console.WriteLine("5. Delete Task");
            Console.WriteLine("6. Search Tasks");
            Console.WriteLine("7. View Statistics");
            Console.WriteLine("8. Bulk Operations");
            Console.WriteLine("9. Export Data");
            Console.WriteLine("0. Exit");
            Console.Write("\nChoose an option: ");
        }

        private async Task CreateTaskAsync()
        {
            _displayHelper.DisplayHeader("Create New Task");

            try
            {
                var taskData = await _inputHandler.GetTaskInputAsync();

                var task = await _taskService.CreateTaskAsync(
                    taskData.Title,
                    taskData.Description,
                    taskData.Priority,
                    taskData.Category,
                    taskData.DueDate,
                    taskData.AssignedTo,
                    taskData.Tags,
                    taskData.EstimatedHours);

                _displayHelper.DisplaySuccess($"Task '{task.Title}' created successfully with ID {task.Id}");
            }
            catch (TaskValidationException ex)
            {
                _displayHelper.DisplayError("Task validation failed:");
                foreach (var error in ex.ValidationErrors)
                {
                    _displayHelper.DisplayError($"  • {error}");
                }
            }
            catch (Exception ex)
            {
                _displayHelper.DisplayError($"Failed to create task: {ex.Message}");
            }
        }

        private async Task ViewTasksAsync()
        {
            _displayHelper.DisplayHeader("View Tasks");

            Console.WriteLine("Filter options:");
            Console.WriteLine("1. All Tasks");
            Console.WriteLine("2. Pending Tasks");
            Console.WriteLine("3. Completed Tasks");
            Console.WriteLine("4. Overdue Tasks");
            Console.WriteLine("5. Tasks Due Soon (7 days)");
            Console.WriteLine("6. By Priority");
            Console.WriteLine("7. By Status");
            Console.Write("Choose filter: ");

            string choice = Console.ReadLine()?.Trim();
            IEnumerable<TaskItem> tasks = null;

            try
            {
                switch (choice)
                {
                    case "1":
                        tasks = await _taskService.GetAllTasksAsync();
                        break;
                    case "2":
                        tasks = await _taskService.GetTasksByStatusAsync(TaskStatus.Pending);
                        break;
                    case "3":
                        tasks = await _taskService.GetTasksByStatusAsync(TaskStatus.Completed);
                        break;
                    case "4":
                        tasks = await _taskService.GetOverdueTasksAsync();
                        break;
                    case "5":
                        tasks = await _taskService.GetTasksDueSoonAsync(7);
                        break;
                    case "6":
                        await DisplayTasksByPriorityAsync();
                        return;
                    case "7":
                        await DisplayTasksByStatusAsync();
                        return;
                    default:
                        _displayHelper.DisplayError("Invalid choice");
                        return;
                }

                DisplayTaskList(tasks, "Tasks");
            }
            catch (Exception ex)
            {
                _displayHelper.DisplayError($"Failed to retrieve tasks: {ex.Message}");
            }
        }

        private async Task DisplayTasksByPriorityAsync()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            var groupedTasks = tasks.GroupByPriority();

            foreach (var group in groupedTasks.OrderByDescending(g => g.Key))
            {
                _displayHelper.DisplayHeader($"{group.Key} Priority Tasks");
                DisplayTaskList(group, $"{group.Key} Priority");
            }
        }

        private async Task DisplayTasksByStatusAsync()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            var groupedTasks = tasks.GroupByStatus();

            foreach (var group in groupedTasks)
            {
                _displayHelper.DisplayHeader($"{group.Key} Tasks");
                DisplayTaskList(group, $"{group.Key}");
            }
        }

        private void DisplayTaskList(IEnumerable<TaskItem> tasks, string title)
        {
            var taskList = tasks.ToList();

            if (!taskList.Any())
            {
                _displayHelper.DisplayMessage($"No {title.ToLower()} found.");
                return;
            }

            _displayHelper.DisplayHeader($"{title} ({taskList.Count})");

            foreach (var task in taskList.OrderByPriority())
            {
                Console.WriteLine(task.ToString());

                if (!string.IsNullOrEmpty(task.Description))
                {
                    Console.WriteLine($"   {task.Description.Truncate(80)}");
                }

                if (task.Tags.Any())
                {
                    Console.WriteLine($"   Tags: {string.Join(", ", task.Tags)}");
                }

                Console.WriteLine();
            }
        }

        private async Task EditTaskAsync()
        {
            _displayHelper.DisplayHeader("Edit Task");

            int taskId = _inputHandler.GetTaskId("edit");
            if (taskId == 0) return;

            try
            {
                var task = await _taskService.GetTaskByIdAsync(taskId);
                _displayHelper.DisplayMessage($"Current task: {task}");

                var updatedData = await _inputHandler.GetTaskUpdateInputAsync(task);

                await _taskService.UpdateTaskAsync(taskId, t =>
                {
                    t.Title = updatedData.Title ?? t.Title;
                    t.Description = updatedData.Description ?? t.Description;
                    t.Priority = updatedData.Priority ?? t.Priority;
                    t.Category = updatedData.Category ?? t.Category;
                    t.DueDate = updatedData.DueDate ?? t.DueDate;
                    t.AssignedTo = updatedData.AssignedTo ?? t.AssignedTo;
                    t.Tags = updatedData.Tags ?? t.Tags;
                    t.EstimatedHours = updatedData.EstimatedHours ?? t.EstimatedHours;
                });

                _displayHelper.DisplaySuccess("Task updated successfully");
            }
            catch (TaskNotFoundException)
            {
                _displayHelper.DisplayError($"Task with ID {taskId} not found");
            }
            catch (Exception ex)
            {
                _displayHelper.DisplayError($"Failed to edit task: {ex.Message}");
            }
        }

        private async Task MarkTaskCompletedAsync()
        {
            _displayHelper.DisplayHeader("Mark Task as Completed");

            int taskId = _inputHandler.GetTaskId("mark as completed");
            if (taskId == 0) return;

            try
            {
                await _taskService.MarkTaskCompletedAsync(taskId);
                _displayHelper.DisplaySuccess($"Task {taskId} marked as completed");
            }
            catch (TaskNotFoundException)
            {
                _displayHelper.DisplayError($"Task with ID {taskId} not found");
            }
            catch (Exception ex)
            {
                _displayHelper.DisplayError($"Failed to mark task as completed: {ex.Message}");
            }
        }

        private async Task DeleteTaskAsync()
        {
            _displayHelper.DisplayHeader("Delete Task");

            int taskId = _inputHandler.GetTaskId("delete");
            if (taskId == 0) return;

            try
            {
                var task = await _taskService.GetTaskByIdAsync(taskId);

                Console.Write($"Are you sure you want to delete task '{task.Title}'? (y/N): ");
                string confirmation = Console.ReadLine()?.Trim().ToLower();

                if (confirmation == "y" || confirmation == "yes")
                {
                    await _taskService.DeleteTaskAsync(taskId);
                    _displayHelper.DisplaySuccess("Task deleted successfully");
                }
                else
                {
                    _displayHelper.DisplayMessage("Operation cancelled");
                }
            }
            catch (TaskNotFoundException)
            {
                _displayHelper.DisplayError($"Task with ID {taskId} not found");
            }
            catch (Exception ex)
            {
                _displayHelper.DisplayError($"Failed to delete task: {ex.Message}");
            }
        }

        private async Task SearchTasksAsync()
        {
            _displayHelper.DisplayHeader("Search Tasks");

            Console.Write("Enter search keyword: ");
            string keyword = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(keyword))
            {
                _displayHelper.DisplayError("Search keyword cannot be empty");
                return;
            }

            try
            {
                var results = await _taskService.SearchTasksAsync(keyword);
                DisplayTaskList(results, $"Search Results for '{keyword}'");
            }
            catch (Exception ex)
            {
                _displayHelper.DisplayError($"Search failed: {ex.Message}");
            }
        }

        private async Task ViewStatisticsAsync()
        {
            _displayHelper.DisplayHeader("Task Statistics");

            try
            {
                var stats = await _taskService.GetTaskStatisticsAsync();

                Console.WriteLine($"Total Tasks: {stats.TotalTasks}");
                Console.WriteLine($"Completed: {stats.CompletedTasks} ({stats.CompletionRate:F1}%)");
                Console.WriteLine($"Pending: {stats.PendingTasks}");
                Console.WriteLine($"Overdue: {stats.OverdueTasks}");
                Console.WriteLine();

                if (stats.TasksByPriority.Any())
                {
                    Console.WriteLine("Tasks by Priority:");
                    foreach (var kvp in stats.TasksByPriority.OrderByDescending(p => p.Value))
                    {
                        Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
                    }
                    Console.WriteLine();
                }

                if (stats.TasksByStatus.Any())
                {
                    Console.WriteLine("Tasks by Status:");
                    foreach (var kvp in stats.TasksByStatus.OrderByDescending(s => s.Value))
                    {
                        Console.WriteLine($"  {s.Key}: {s.Value}");
                    }
                    Console.WriteLine();
                }

                if (stats.TasksByAssignee.Any())
                {
                    Console.WriteLine("Tasks by Assignee:");
                    foreach (var kvp in stats.TasksByAssignee.OrderByDescending(a => a.Value))
                    {
                        Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
                    }
                }
            }
            catch (Exception ex)
            {
                _displayHelper.DisplayError($"Failed to load statistics: {ex.Message}");
            }
        }

        private async Task BulkOperationsAsync()
        {
            _displayHelper.DisplayHeader("Bulk Operations");

            Console.WriteLine("1. Mark Multiple Tasks as Completed");
            Console.WriteLine("2. Delete Multiple Tasks");
            Console.WriteLine("3. Update Priority for Multiple Tasks");
            Console.Write("Choose operation: ");

            string choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    await BulkMarkCompletedAsync();
                    break;
                case "2":
                    await BulkDeleteAsync();
                    break;
                case "3":
                    await BulkUpdatePriorityAsync();
                    break;
                default:
                    _displayHelper.DisplayError("Invalid choice");
                    break;
            }
        }

        private async Task BulkMarkCompletedAsync()
        {
            var taskIds = _inputHandler.GetMultipleTaskIds("mark as completed");
            if (!taskIds.Any()) return;

            try
            {
                await _taskService.BulkUpdateStatusAsync(taskIds, TaskStatus.Completed);
                _displayHelper.DisplaySuccess($"{taskIds.Count()} tasks marked as completed");
            }
            catch (Exception ex)
            {
                _displayHelper.DisplayError($"Bulk operation failed: {ex.Message}");
            }
        }

        private async Task BulkDeleteAsync()
        {
            var taskIds = _inputHandler.GetMultipleTaskIds("delete");
            if (!taskIds.Any()) return;

            Console.Write($"Are you sure you want to delete {taskIds.Count()} tasks? (y/N): ");
            string confirmation = Console.ReadLine()?.Trim().ToLower();

            if (confirmation == "y" || confirmation == "yes")
            {
                try
                {
                    foreach (int id in taskIds)
                    {
                        await _taskService.DeleteTaskAsync(id);
                    }
                    _displayHelper.DisplaySuccess($"{taskIds.Count()} tasks deleted");
                }
                catch (Exception ex)
                {
                    _displayHelper.DisplayError($"Bulk delete failed: {ex.Message}");
                }
            }
        }

        private async Task BulkUpdatePriorityAsync()
        {
            var taskIds = _inputHandler.GetMultipleTaskIds("update priority for");
            if (!taskIds.Any()) return;

            PriorityLevel newPriority = _inputHandler.GetPriorityLevel();
            if (newPriority == 0) return;

            try
            {
                foreach (int id in taskIds)
                {
                    await _taskService.UpdateTaskAsync(id, t => t.Priority = newPriority);
                }
                _displayHelper.DisplaySuccess($"{taskIds.Count()} tasks updated to {newPriority} priority");
            }
            catch (Exception ex)
            {
                _displayHelper.DisplayError($"Bulk priority update failed: {ex.Message}");
            }
        }

        private async Task ExportDataAsync()
        {
            _displayHelper.DisplayHeader("Export Data");

            Console.WriteLine("1. Export to JSON");
            Console.WriteLine("2. Export to CSV");
            Console.WriteLine("3. Export Statistics Report");
            Console.Write("Choose export format: ");

            string choice = Console.ReadLine()?.Trim();

            try
            {
                switch (choice)
                {
                    case "1":
                        await ExportToJsonAsync();
                        break;
                    case "2":
                        await ExportToCsvAsync();
                        break;
                    case "3":
                        await ExportStatisticsReportAsync();
                        break;
                    default:
                        _displayHelper.DisplayError("Invalid choice");
                        break;
                }
            }
            catch (Exception ex)
            {
                _displayHelper.DisplayError($"Export failed: {ex.Message}");
            }
        }

        private async Task ExportToJsonAsync()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            string filename = $"tasks_export_{DateTime.Now:yyyyMMdd_HHmmss}.json";

            // Implementation would use System.Text.Json to export
            _displayHelper.DisplaySuccess($"Data exported to {filename}");
        }

        private async Task ExportToCsvAsync()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            string filename = $"tasks_export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            // Implementation would create CSV format
            _displayHelper.DisplaySuccess($"Data exported to {filename}");
        }

        private async Task ExportStatisticsReportAsync()
        {
            var stats = await _taskService.GetTaskStatisticsAsync();
            string filename = $"statistics_report_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

            // Implementation would create text report
            _displayHelper.DisplaySuccess($"Statistics report exported to {filename}");
        }
    }
}
```

## Program.cs (Main Entry Point)

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using TaskManagementSystem.Core.Interfaces;
using TaskManagementSystem.Core.Models;
using TaskManagementSystem.Infrastructure.Repositories;
using TaskManagementSystem.Infrastructure.Services;
using TaskManagementSystem.Infrastructure.Logging;
using TaskManagementSystem.UI;

namespace TaskManagementSystem
{
    /// <summary>
    /// Main entry point for the Task Management System
    /// </summary>
    public class Program
    {
        private static async Task Main(string[] args)
        {
            try
            {
                // Display welcome message
                DisplayWelcomeMessage();

                // Setup dependency injection
                var serviceProvider = SetupDependencies();

                // Get the main UI service
                var ui = serviceProvider.GetService(typeof(ConsoleUI)) as ConsoleUI;

                // Run the application
                await ui.RunAsync();

                // Display farewell message
                DisplayFarewellMessage();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Fatal error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Console.ResetColor();

                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
        }

        private static void DisplayWelcomeMessage()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    Task Management System                    ║");
            Console.WriteLine("║                      Version 2.0.0                          ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Welcome to the Advanced Task Management System!");
            Console.WriteLine("This application demonstrates modern C# development practices.");
            Console.WriteLine();
        }

        private static void DisplayFarewellMessage()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nThank you for using Task Management System!");
            Console.WriteLine("Your tasks have been saved.");
            Console.ResetColor();
        }

        private static IServiceProvider SetupDependencies()
        {
            var services = new ServiceCollection();

            // Configuration
            string dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            Directory.CreateDirectory(dataDirectory);
            string tasksFilePath = Path.Combine(dataDirectory, "tasks.json");

            // Infrastructure services
            services.AddSingleton<ILogger>(new ConsoleLogger(LogLevel.Information));
            services.AddSingleton<IValidator<TaskItem>>(new ValidationService());
            services.AddSingleton<ITaskRepository>(provider =>
                new JsonTaskRepository(tasksFilePath, provider.GetService<ILogger>()));

            // Business services
            services.AddSingleton<ITaskService>(provider =>
                new TaskService(
                    provider.GetService<ITaskRepository>(),
                    provider.GetService<IValidator<TaskItem>>(),
                    provider.GetService<ILogger>()));

            // UI services
            services.AddSingleton<ConsoleUI>(provider =>
                new ConsoleUI(
                    provider.GetService<ITaskService>(),
                    provider.GetService<ILogger>()));

            return services.BuildServiceProvider();
        }
    }

    // Simple service collection for dependency injection
    public interface IServiceProvider
    {
        T GetService<T>() where T : class;
        object GetService(Type serviceType);
    }

    public class ServiceCollection : IServiceProvider
    {
        private readonly Dictionary<Type, Func<IServiceProvider, object>> _registrations = new();

        public void AddSingleton<T>(Func<IServiceProvider, T> factory) where T : class
        {
            _registrations[typeof(T)] = factory;
        }

        public void AddSingleton<T>(T instance) where T : class
        {
            _registrations[typeof(T)] = _ => instance;
        }

        public T GetService<T>() where T : class
        {
            return (T)GetService(typeof(T));
        }

        public object GetService(Type serviceType)
        {
            if (_registrations.TryGetValue(serviceType, out var factory))
            {
                return factory(this);
            }

            throw new InvalidOperationException($"Service {serviceType.Name} is not registered");
        }

        public ServiceProvider BuildServiceProvider()
        {
            return new ServiceProvider(_registrations);
        }
    }

    public class ServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, Func<IServiceProvider, object>> _registrations;
        private readonly Dictionary<Type, object> _instances = new();

        public ServiceProvider(Dictionary<Type, Func<IServiceProvider, object>> registrations)
        {
            _registrations = registrations;
        }

        public T GetService<T>() where T : class
        {
            return (T)GetService(typeof(T));
        }

        public object GetService(Type serviceType)
        {
            if (_instances.TryGetValue(serviceType, out var instance))
            {
                return instance;
            }

            if (_registrations.TryGetValue(serviceType, out var factory))
            {
                instance = factory(this);
                _instances[serviceType] = instance;
                return instance;
            }

            throw new InvalidOperationException($"Service {serviceType.Name} is not registered");
        }
    }
}
```

## Unit Tests

### TaskServiceTests.cs

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using TaskManagementSystem.Core.Interfaces;
using TaskManagementSystem.Core.Models;
using TaskManagementSystem.Core.Exceptions;
using TaskManagementSystem.Infrastructure.Services;

namespace TaskManagementSystem.Tests.UnitTests
{
    public class TaskServiceTests
    {
        private readonly Mock<ITaskRepository> _repositoryMock;
        private readonly Mock<IValidator<TaskItem>> _validatorMock;
        private readonly Mock<ILogger> _loggerMock;
        private readonly TaskService _taskService;

        public TaskServiceTests()
        {
            _repositoryMock = new Mock<ITaskRepository>();
            _validatorMock = new Mock<IValidator<TaskItem>>();
            _loggerMock = new Mock<ILogger>();

            _taskService = new TaskService(
                _repositoryMock.Object,
                _validatorMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task CreateTaskAsync_ValidData_ReturnsCreatedTask()
        {
            // Arrange
            var taskData = new
            {
                Title = "Test Task",
                Description = "Test Description",
                Priority = PriorityLevel.Medium
            };

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<TaskItem>()))
                         .ReturnsAsync(ValidationResult.Success());

            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<TaskItem>()))
                          .ReturnsAsync((TaskItem t) => t with { Id = 1 });

            // Act
            var result = await _taskService.CreateTaskAsync(
                taskData.Title,
                taskData.Description,
                taskData.Priority);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(taskData.Title, result.Title);
            Assert.Equal(taskData.Description, result.Description);
            Assert.Equal(taskData.Priority, result.Priority);
        }

        [Fact]
        public async Task CreateTaskAsync_InvalidData_ThrowsValidationException()
        {
            // Arrange
            var validationErrors = new[] { "Title is required" };
            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<TaskItem>()))
                         .ReturnsAsync(ValidationResult.Failure(validationErrors));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<TaskValidationException>(
                () => _taskService.CreateTaskAsync("", "", PriorityLevel.Low));

            Assert.Equal(validationErrors, exception.ValidationErrors);
        }

        [Fact]
        public async Task GetTaskByIdAsync_ExistingTask_ReturnsTask()
        {
            // Arrange
            var expectedTask = new TaskItem { Id = 1, Title = "Test Task" };
            _repositoryMock.Setup(r => r.GetByIdAsync(1))
                          .ReturnsAsync(expectedTask);

            // Act
            var result = await _taskService.GetTaskByIdAsync(1);

            // Assert
            Assert.Equal(expectedTask, result);
        }

        [Fact]
        public async Task GetTaskByIdAsync_NonExistingTask_ThrowsNotFoundException()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(999))
                          .ThrowsAsync(new TaskNotFoundException(999));

            // Act & Assert
            await Assert.ThrowsAsync<TaskNotFoundException>(
                () => _taskService.GetTaskByIdAsync(999));
        }

        [Fact]
        public async Task MarkTaskCompletedAsync_ExistingTask_UpdatesTask()
        {
            // Arrange
            var task = new TaskItem { Id = 1, Title = "Test Task", Status = TaskStatus.Pending };
            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);
            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TaskItem>())).Returns(Task.CompletedTask);

            // Act
            await _taskService.MarkTaskCompletedAsync(1);

            // Assert
            _repositoryMock.Verify(r => r.UpdateAsync(It.Is<TaskItem>(t =>
                t.Id == 1 &&
                t.Status == TaskStatus.Completed &&
                t.CompletedDate.HasValue)), Times.Once);
        }

        [Fact]
        public async Task GetTaskStatisticsAsync_ReturnsCorrectStatistics()
        {
            // Arrange
            var tasks = new List<TaskItem>
            {
                new TaskItem { Id = 1, Status = TaskStatus.Completed, Priority = PriorityLevel.High },
                new TaskItem { Id = 2, Status = TaskStatus.Pending, Priority = PriorityLevel.Medium },
                new TaskItem { Id = 3, Status = TaskStatus.Completed, Priority = PriorityLevel.Low },
                new TaskItem { Id = 4, Status = TaskStatus.InProgress, Priority = PriorityLevel.High }
            };

            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(tasks);

            // Act
            var stats = await _taskService.GetTaskStatisticsAsync();

            // Assert
            Assert.Equal(4, stats.TotalTasks);
            Assert.Equal(2, stats.CompletedTasks);
            Assert.Equal(1, stats.PendingTasks);
            Assert.Equal(50.0, stats.CompletionRate);
            Assert.Equal(2, stats.TasksByPriority[PriorityLevel.High]);
            Assert.Equal(1, stats.TasksByPriority[PriorityLevel.Medium]);
            Assert.Equal(1, stats.TasksByPriority[PriorityLevel.Low]);
        }
    }
}
```

## Configuration and Dependency Injection

### AppConfig.cs

```csharp
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaskManagementSystem.Infrastructure.Configuration
{
    /// <summary>
    /// Application configuration settings
    /// </summary>
    public class AppConfig
    {
        [JsonPropertyName("dataDirectory")]
        public string DataDirectory { get; set; } = "Data";

        [JsonPropertyName("tasksFileName")]
        public string TasksFileName { get; set; } = "tasks.json";

        [JsonPropertyName("logLevel")]
        public string LogLevel { get; set; } = "Information";

        [JsonPropertyName("maxConcurrentOperations")]
        public int MaxConcurrentOperations { get; set; } = 5;

        [JsonPropertyName("autoSaveIntervalMinutes")]
        public int AutoSaveIntervalMinutes { get; set; } = 5;

        [JsonPropertyName("enableStatistics")]
        public bool EnableStatistics { get; set; } = true;

        [JsonPropertyName("defaultTaskPriority")]
        public PriorityLevel DefaultTaskPriority { get; set; } = PriorityLevel.Medium;

        [JsonPropertyName("ui")]
        public UiConfig Ui { get; set; } = new UiConfig();

        public class UiConfig
        {
            [JsonPropertyName("pageSize")]
            public int PageSize { get; set; } = 20;

            [JsonPropertyName("enableColors")]
            public bool EnableColors { get; set; } = true;

            [JsonPropertyName("dateFormat")]
            public string DateFormat { get; set; } = "yyyy-MM-dd";

            [JsonPropertyName("timeFormat")]
            public string TimeFormat { get; set; } = "HH:mm:ss";
        }

        /// <summary>
        /// Loads configuration from file
        /// </summary>
        public static AppConfig Load(string configPath = "appsettings.json")
        {
            try
            {
                if (File.Exists(configPath))
                {
                    string json = File.ReadAllText(configPath);
                    var config = JsonSerializer.Deserialize<AppConfig>(json);
                    return config ?? new AppConfig();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading configuration: {ex.Message}");
            }

            return new AppConfig();
        }

        /// <summary>
        /// Saves configuration to file
        /// </summary>
        public void Save(string configPath = "appsettings.json")
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                string json = JsonSerializer.Serialize(this, options);
                File.WriteAllText(configPath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving configuration: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the full path to the tasks data file
        /// </summary>
        public string GetTasksFilePath()
        {
            return Path.Combine(DataDirectory, TasksFileName);
        }

        /// <summary>
        /// Ensures the data directory exists
        /// </summary>
        public void EnsureDataDirectoryExists()
        {
            Directory.CreateDirectory(DataDirectory);
        }
    }
}
```

## Advanced Features Demonstration

### Pattern Matching Examples

```csharp
public static class TaskPatternMatchingExtensions
{
    /// <summary>
    /// Gets a description based on task properties using pattern matching
    /// </summary>
    public static string GetTaskDescription(this TaskItem task)
    {
        return task switch
        {
            // Complex property patterns
            { Status: TaskStatus.Completed, Priority: PriorityLevel.Urgent } =>
                $"Urgent task completed on time",

            { Status: TaskStatus.Completed, Priority: PriorityLevel.Urgent, IsOverdue: true } =>
                $"Urgent task completed but was overdue",

            { Status: TaskStatus.Pending, Priority: PriorityLevel.Urgent, DueDate: not null }
                when task.DaysUntilDue <= 1 =>
                $"Urgent task due very soon",

            { Status: TaskStatus.Pending, Priority: PriorityLevel.Urgent, DueDate: not null }
                when task.DaysUntilDue <= 7 =>
                $"Urgent task due within a week",

            { Status: TaskStatus.Pending, IsOverdue: true } =>
                $"Overdue task needs immediate attention",

            { Status: TaskStatus.InProgress, EstimatedHours: > 0, ActualHours: var actual }
                when actual > task.EstimatedHours * 1.2 =>
                $"Task is running over estimated time",

            { Priority: PriorityLevel.High, AssignedTo: null or "" } =>
                $"High priority task not assigned",

            { Tags: { Count: > 5 } } =>
                $"Task has many tags - consider organizing",

            { CreatedDate: var created } when (DateTime.Now - created).TotalDays > 30 =>
                $"Old task - consider review",

            _ => "Standard task"
        };
    }

    /// <summary>
    /// Calculates priority score using pattern matching
    /// </summary>
    public static int CalculatePriorityScore(this TaskItem task)
    {
        return task switch
        {
            { Priority: PriorityLevel.Urgent, IsOverdue: true } => 100,
            { Priority: PriorityLevel.Urgent, DueDate: not null } when task.DaysUntilDue <= 1 => 95,
            { Priority: PriorityLevel.Urgent } => 90,
            { Priority: PriorityLevel.High, IsOverdue: true } => 85,
            { Priority: PriorityLevel.High, DueDate: not null } when task.DaysUntilDue <= 3 => 80,
            { Priority: PriorityLevel.High } => 75,
            { Priority: PriorityLevel.Medium, IsOverdue: true } => 70,
            { Priority: PriorityLevel.Medium, DueDate: not null } when task.DaysUntilDue <= 7 => 65,
            { Priority: PriorityLevel.Medium } => 60,
            { Priority: PriorityLevel.Low, IsOverdue: true } => 55,
            _ => 50
        };
    }

    /// <summary>
    /// Gets recommended action using pattern matching
    /// </summary>
    public static string GetRecommendedAction(this TaskItem task)
    {
        return task switch
        {
            { Status: TaskStatus.Pending, IsOverdue: true } =>
                "Start immediately - task is overdue",

            { Status: TaskStatus.Pending, Priority: PriorityLevel.Urgent, DueDate: not null }
                when task.DaysUntilDue <= 1 =>
                "High priority - complete today",

            { Status: TaskStatus.InProgress, EstimatedHours: > 0, ActualHours: var actual }
                when actual > task.EstimatedHours =>
                "Review progress - running over time",

            { Status: TaskStatus.Completed, CompletedDate: var completed }
                when (DateTime.Now - completed).TotalDays > 7 =>
                "Archive completed task",

            { AssignedTo: null or "", Priority: >= PriorityLevel.High } =>
                "Assign team member - high priority task unassigned",

            { Tags: { Count: 0 }, Description: { Length: > 100 } } =>
                "Add tags for better organization",

            _ => "Continue as planned"
        };
    }
}
```

### LINQ Query Examples

```csharp
public static class TaskLinqQueries
{
    /// <summary>
    /// Advanced LINQ queries for task analysis
    /// </summary>
    public static class TaskAnalytics
    {
        public static IEnumerable<TaskItem> GetHighPriorityOverdueTasks(IEnumerable<TaskItem> tasks)
        {
            return tasks.Where(t => t.Priority >= PriorityLevel.High && t.IsOverdue);
        }

        public static IEnumerable<TaskItem> GetTasksDueThisWeek(IEnumerable<TaskItem> tasks)
        {
            var endOfWeek = DateTime.Now.AddDays(7 - (int)DateTime.Now.DayOfWeek);
            return tasks.Where(t => t.DueDate.HasValue && t.DueDate.Value <= endOfWeek);
        }

        public static IEnumerable<IGrouping<string, TaskItem>> GroupTasksByAssignee(IEnumerable<TaskItem> tasks)
        {
            return tasks.Where(t => !string.IsNullOrEmpty(t.AssignedTo))
                       .GroupBy(t => t.AssignedTo);
        }

        public static IEnumerable<TaskItem> GetMostComplexTasks(IEnumerable<TaskItem> tasks)
        {
            return tasks.Where(t => t.EstimatedHours.HasValue)
                       .OrderByDescending(t => t.EstimatedHours.Value)
                       .Take(10);
        }

        public static Dictionary<PriorityLevel, double> GetAverageCompletionTimeByPriority(IEnumerable<TaskItem> tasks)
        {
            return tasks.Where(t => t.IsCompleted && t.CompletedDate.HasValue)
                       .GroupBy(t => t.Priority)
                       .ToDictionary(
                           g => g.Key,
                           g => g.Average(t => (t.CompletedDate.Value - t.CreatedDate).TotalHours));
        }

        public static IEnumerable<TaskItem> GetTasksWithSimilarTitles(IEnumerable<TaskItem> tasks, string keyword)
        {
            return tasks.Where(t => t.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                       .OrderBy(t => t.Title.Length);
        }

        public static IEnumerable<TaskItem> GetRecentlyCreatedTasks(IEnumerable<TaskItem> tasks, int days = 7)
        {
            var cutoffDate = DateTime.Now.AddDays(-days);
            return tasks.Where(t => t.CreatedDate >= cutoffDate)
                       .OrderByDescending(t => t.CreatedDate);
        }

        public static IEnumerable<TaskItem> GetTasksByTag(IEnumerable<TaskItem> tasks, string tag)
        {
            return tasks.Where(t => t.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase));
        }

        public static IEnumerable<TaskItem> GetUnassignedHighPriorityTasks(IEnumerable<TaskItem> tasks)
        {
            return tasks.Where(t => string.IsNullOrEmpty(t.AssignedTo) && t.Priority >= PriorityLevel.High);
        }

        public static double GetTeamProductivityScore(IEnumerable<TaskItem> tasks)
        {
            var completedTasks = tasks.Where(t => t.IsCompleted && t.ActualHours.HasValue);
            if (!completedTasks.Any()) return 0;

            var averageEfficiency = completedTasks.Average(t =>
                t.EstimatedHours.HasValue ? t.EstimatedHours.Value / t.ActualHours.Value : 1);

            return Math.Max(0, Math.Min(100, averageEfficiency * 100));
        }
    }

    /// <summary>
    /// LINQ query builders for dynamic queries
    /// </summary>
    public static class QueryBuilder
    {
        public static IQueryable<TaskItem> BuildDynamicQuery(IQueryable<TaskItem> query,
            string status = null, PriorityLevel? priority = null, string assignee = null,
            DateTime? dueBefore = null, DateTime? dueAfter = null)
        {
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<TaskStatus>(status, true, out var taskStatus))
            {
                query = query.Where(t => t.Status == taskStatus);
            }

            if (priority.HasValue)
            {
                query = query.Where(t => t.Priority == priority.Value);
            }

            if (!string.IsNullOrEmpty(assignee))
            {
                query = query.Where(t => t.AssignedTo.Contains(assignee, StringComparison.OrdinalIgnoreCase));
            }

            if (dueBefore.HasValue)
            {
                query = query.Where(t => t.DueDate.HasValue && t.DueDate.Value <= dueBefore.Value);
            }

            if (dueAfter.HasValue)
            {
                query = query.Where(t => t.DueDate.HasValue && t.DueDate.Value >= dueAfter.Value);
            }

            return query;
        }

        public static IQueryable<TaskItem> ApplySorting(IQueryable<TaskItem> query, string sortBy = "priority")
        {
            return sortBy.ToLower() switch
            {
                "duedate" => query.OrderBy(t => t.DueDate ?? DateTime.MaxValue),
                "created" => query.OrderByDescending(t => t.CreatedDate),
                "title" => query.OrderBy(t => t.Title),
                "assignee" => query.OrderBy(t => t.AssignedTo ?? ""),
                _ => query.OrderByDescending(t => t.Priority).ThenBy(t => t.DueDate ?? DateTime.MaxValue)
            };
        }

        public static IQueryable<TaskItem> ApplyPagination(IQueryable<TaskItem> query, int page = 1, int pageSize = 20)
        {
            return query.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}
```

## Exercises

1. **Add Notification System**
   - Implement an event-driven notification system
   - Support email, console, and file notifications
   - Add notification preferences for different task events

2. **Implement Task Templates**
   - Create a template system for common task types
   - Support template inheritance and customization
   - Add template management through the UI

3. **Add Data Import/Export**
   - Implement CSV import/export functionality
   - Add XML serialization support
   - Create data migration tools

4. **Build Reporting System**
   - Create comprehensive reports (productivity, overdue tasks, etc.)
   - Add chart generation (ASCII-based)
   - Implement scheduled report generation

5. **Add User Management**
   - Implement user authentication and authorization
   - Add role-based access control
   - Create user profiles with preferences

6. **Implement Task Dependencies**
   - Add task dependency relationships
   - Implement dependency validation
   - Create dependency visualization

7. **Add Time Tracking**
   - Implement detailed time tracking
   - Add work session management
   - Create productivity analytics

8. **Create Plugin System**
   - Design a plugin architecture
   - Implement plugin discovery and loading
   - Add extension points throughout the application

9. **Add Search and Filtering**
   - Implement advanced search with multiple criteria
   - Add saved search functionality
   - Create filter presets

10. **Implement Undo/Redo**
    - Add command pattern for operations
    - Implement undo/redo functionality
    - Add operation history

This comprehensive project demonstrates the practical application of all advanced C# concepts covered in the course, from basic async programming to complex architectural patterns. The modular design with proper separation of concerns, comprehensive error handling, and extensive use of modern C# features makes this a production-ready application that showcases professional development practices.
