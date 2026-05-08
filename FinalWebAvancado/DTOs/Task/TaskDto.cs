using System.ComponentModel.DataAnnotations;
using Api.Enums;

namespace Api.Dto;

public class TaskDto
{
    [Range(1, int.MaxValue, ErrorMessage = EnumMessageAnottation.RequiredId)]
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = EnumMessageAnottation.RequiredUserId)]
    public int UserId { get; set; }

    [Required(ErrorMessage = EnumMessageAnottation.RequiredName)]
    public string Name { get; set; } = string.Empty;

    [StringLength(4000)]
    public string? Description { get; set; }

    [EnumDataType(typeof(EnumTaskLevel), ErrorMessage = EnumMessageAnottation.RequiredLevel)]
    [Required(ErrorMessage = EnumMessageAnottation.RequiredLevel)]
    public EnumTaskLevel Level { get; set; }

    [EnumDataType(typeof(EnumTaskStatus), ErrorMessage = EnumMessageAnottation.RequiredStatus)]
    [Required(ErrorMessage = EnumMessageAnottation.RequiredStatus)]
    public EnumTaskStatus Status { get; set; }

    [Required(ErrorMessage = EnumMessageAnottation.RequiredCreatedAt)]
    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; }

    [Required(ErrorMessage = EnumMessageAnottation.RequiredUpdatedAt)]
    [DataType(DataType.DateTime)]
    public DateTime UpdatedAt { get; set; }
}
