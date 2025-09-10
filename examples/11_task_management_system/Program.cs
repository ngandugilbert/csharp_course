using System;

class Program {
    static void Main(string[] args) {
        Console.WriteLine("Welcome to Task Management System!");

        var taskService = new TaskService();
        var ui = new ConsoleUI(taskService);

        ui.Run();

        Console.WriteLine("Thank you for using Task Management System!");
    }
}
