namespace Api.Models;

public class TaskCategory
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int TaskId { get; set; }
    public int CategoryId { get; set; }
}
