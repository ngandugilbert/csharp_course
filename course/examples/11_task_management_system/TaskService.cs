using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.IO;

public class TaskService {
    private List<TaskItem> tasks;
    private int nextId;
    private const string DataFile = "tasks.json";

    public TaskService() {
        tasks = LoadTasks();
        nextId = tasks.Count > 0 ? tasks.Max(t => t.Id) + 1 : 1;
    }

    public void AddTask(string title, string description, PriorityLevel priority, DateTime? dueDate = null) {
        var task = new TaskItem {
            Id = nextId++,
            Title = title,
            Description = description,
            Priority = priority,
            DueDate = dueDate
        };

        tasks.Add(task);
        SaveTasks();
        Console.WriteLine($"Task '{title}' added successfully!");
    }

    public void MarkTaskCompleted(int id) {
        var task = tasks.FirstOrDefault(t => t.Id == id);
        if (task != null) {
            task.IsCompleted = true;
            SaveTasks();
            Console.WriteLine($"Task '{task.Title}' marked as completed!");
        } else {
            Console.WriteLine("Task not found.");
        }
    }

    public void DeleteTask(int id) {
        var task = tasks.FirstOrDefault(t => t.Id == id);
        if (task != null) {
            tasks.Remove(task);
            SaveTasks();
            Console.WriteLine($"Task '{task.Title}' deleted!");
        } else {
            Console.WriteLine("Task not found.");
        }
    }

    public List<TaskItem> GetAllTasks() {
        return tasks.OrderByDescending(t => t.Priority).ThenBy(t => t.CreatedDate).ToList();
    }

    public List<TaskItem> GetPendingTasks() {
        return tasks.Where(t => !t.IsCompleted).ToList();
    }

    public List<TaskItem> SearchTasks(string keyword) {
        return tasks.Where(t =>
            t.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            t.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private void SaveTasks() {
        try {
            string jsonString = JsonSerializer.Serialize(tasks, new JsonSerializerOptions {
                WriteIndented = true
            });
            File.WriteAllText(DataFile, jsonString);
        } catch (Exception ex) {
            Console.WriteLine($"Error saving tasks: {ex.Message}");
        }
    }

    private List<TaskItem> LoadTasks() {
        try {
            if (File.Exists(DataFile)) {
                string jsonString = File.ReadAllText(DataFile);
                return JsonSerializer.Deserialize<List<TaskItem>>(jsonString) ?? new List<TaskItem>();
            }
        } catch (Exception ex) {
            Console.WriteLine($"Error loading tasks: {ex.Message}");
        }

        return new List<TaskItem>();
    }
}
