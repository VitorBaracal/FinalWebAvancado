using Api.Enums;

namespace Api.Dto;

public class TaskDto
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public EnumTaskLevel Level { get; set; }

    public EnumTaskStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
