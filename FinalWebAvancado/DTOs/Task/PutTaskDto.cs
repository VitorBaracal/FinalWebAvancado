using System.ComponentModel.DataAnnotations;
using Api.Enums;

namespace Api.Dto;

public class PutTaskDto
{
    [Range(1, int.MaxValue, ErrorMessage = EnumMessageAnottation.RequiredUserId)]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    [StringLength(4000)]
    public string? Description { get; set; }

    [EnumDataType(typeof(EnumTaskLevel), ErrorMessage = EnumMessageAnottation.RequiredLevel)]
    public EnumTaskLevel Level { get; set; }

    [EnumDataType(typeof(EnumTaskStatus), ErrorMessage = EnumMessageAnottation.RequiredStatus)]
    public EnumTaskStatus Status { get; set; }
}

 
