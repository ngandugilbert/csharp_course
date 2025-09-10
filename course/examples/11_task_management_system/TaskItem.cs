using System;

public class TaskItem {
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
    public PriorityLevel Priority { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? DueDate { get; set; }

    public TaskItem() {
        CreatedDate = DateTime.Now;
        IsCompleted = false;
    }

    public override string ToString() {
        string status = IsCompleted ? "[✓]" : "[ ]";
        string dueInfo = DueDate.HasValue ? $" (Due: {DueDate.Value.ToShortDateString()})" : "";
        return $"{status} {Id}. {Title} - Priority: {Priority}{dueInfo}";
    }
}

public enum PriorityLevel {
    Low,
    Medium,
    High,
    Urgent
}
