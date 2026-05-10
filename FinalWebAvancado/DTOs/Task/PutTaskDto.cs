using System.ComponentModel.DataAnnotations;
using Api.Enums;

namespace Api.Dto;

public class PutTaskDto
{
    public string? Name { get; set; }

    [StringLength(4000)]
    public string? Description { get; set; }

    [EnumDataType(typeof(EnumTaskLevel), ErrorMessage = EnumMessageAnottation.InvalidLevelConstant)]
    public EnumTaskLevel? Level { get; set; }

    [EnumDataType(typeof(EnumTaskStatus), ErrorMessage = EnumMessageAnottation.InvalidStatusConstant)]
    public EnumTaskStatus? Status { get; set; }
}

 
