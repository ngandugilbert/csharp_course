using System;

public class ConsoleUI {
    private readonly TaskService taskService;

    public ConsoleUI(TaskService taskService) {
        this.taskService = taskService;
    }

    public void Run() {
        bool running = true;

        while (running) {
            DisplayMenu();
            string choice = Console.ReadLine();

            switch (choice) {
                case "1":
                    AddTask();
                    break;
                case "2":
                    ViewTasks();
                    break;
                case "3":
                    MarkTaskCompleted();
                    break;
                case "4":
                    DeleteTask();
                    break;
                case "5":
                    SearchTasks();
                    break;
                case "6":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }

            if (running) {
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }
    }

    private void DisplayMenu() {
        Console.Clear();
        Console.WriteLine("=== Task Management System ===");
        Console.WriteLine("1. Add Task");
        Console.WriteLine("2. View All Tasks");
        Console.WriteLine("3. Mark Task as Completed");
        Console.WriteLine("4. Delete Task");
        Console.WriteLine("5. Search Tasks");
        Console.WriteLine("6. Exit");
        Console.Write("Choose an option: ");
    }

    private void AddTask() {
        Console.Write("Enter task title: ");
        string title = Console.ReadLine();

        Console.Write("Enter task description: ");
        string description = Console.ReadLine();

        Console.Write("Enter priority (Low/Medium/High/Urgent): ");
        string priorityInput = Console.ReadLine();
        PriorityLevel priority;

        if (!Enum.TryParse(priorityInput, true, out priority)) {
            priority = PriorityLevel.Medium;
        }

        Console.Write("Enter due date (yyyy-mm-dd) or leave empty: ");
        string dueDateInput = Console.ReadLine();
        DateTime? dueDate = null;

        if (!string.IsNullOrEmpty(dueDateInput) && DateTime.TryParse(dueDateInput, out DateTime parsedDate)) {
            dueDate = parsedDate;
        }

        taskService.AddTask(title, description, priority, dueDate);
    }

    private void ViewTasks() {
        var tasks = taskService.GetAllTasks();

        if (tasks.Count == 0) {
            Console.WriteLine("No tasks found.");
            return;
        }

        Console.WriteLine("\n=== All Tasks ===");
        foreach (var task in tasks) {
            Console.WriteLine(task);
            if (!string.IsNullOrEmpty(task.Description)) {
                Console.WriteLine($"   {task.Description}");
            }
        }
    }

    private void MarkTaskCompleted() {
        Console.Write("Enter task ID to mark as completed: ");
        if (int.TryParse(Console.ReadLine(), out int id)) {
            taskService.MarkTaskCompleted(id);
        } else {
            Console.WriteLine("Invalid task ID.");
        }
    }

    private void DeleteTask() {
        Console.Write("Enter task ID to delete: ");
        if (int.TryParse(Console.ReadLine(), out int id)) {
            taskService.DeleteTask(id);
        } else {
            Console.WriteLine("Invalid task ID.");
        }
    }

    private void SearchTasks() {
        Console.Write("Enter search keyword: ");
        string keyword = Console.ReadLine();

        var results = taskService.SearchTasks(keyword);

        if (results.Count == 0) {
            Console.WriteLine("No tasks found matching the search criteria.");
            return;
        }

        Console.WriteLine($"\n=== Search Results for '{keyword}' ===");
        foreach (var task in results) {
            Console.WriteLine(task);
        }
    }
}
