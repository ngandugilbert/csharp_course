using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace TodoApp {
    public class TodoItem {
        public string Title { get; set; }
        public bool Done { get; set; }
    }

    public class TodoListManager {
        private List<TodoItem> items = new List<TodoItem>();
        private string filePath = "todos.json";

        public void Add(string title) => items.Add(new TodoItem { Title = title, Done = false });
        public IEnumerable<TodoItem> List() => items;
        public void Toggle(int index) { if (index >= 0 && index < items.Count) items[index].Done = !items[index].Done; }
        public void Save() => File.WriteAllText(filePath, JsonSerializer.Serialize(items));
        public void Load() { if (File.Exists(filePath)) items = JsonSerializer.Deserialize<List<TodoItem>>(File.ReadAllText(filePath)) ?? new List<TodoItem>(); }
    }

    class Program {
        static void Main() {
            var mgr = new TodoListManager();
            mgr.Load();
            Console.WriteLine("Simple TODO App\nCommands: add <task>, list, toggle <index>, save, exit");
            while (true) {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = line.Split(' ', 2);
                var cmd = parts[0].ToLowerInvariant();
                if (cmd == "add" && parts.Length == 2) { mgr.Add(parts[1]); Console.WriteLine("Added"); }
                else if (cmd == "list") {
                    var i = 0; foreach(var t in mgr.List()) Console.WriteLine($"[{i++}] {(t.Done ? "x" : " ")} {t.Title}"); }
                else if (cmd == "toggle" && parts.Length == 2 && int.TryParse(parts[1], out var idx)) { mgr.Toggle(idx); }
                else if (cmd == "save") { mgr.Save(); Console.WriteLine("Saved"); }
                else if (cmd == "exit") { mgr.Save(); break; }
                else Console.WriteLine("Unknown command");
            }
        }
    }
}
