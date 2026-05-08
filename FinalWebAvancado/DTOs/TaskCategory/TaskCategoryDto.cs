using System.ComponentModel.DataAnnotations;
using Api.Enums;

namespace Api.Dto;

public class TaskCategoryDto
{
    [Range(1, int.MaxValue, ErrorMessage = EnumMessageAnottation.RequiredId)]
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = EnumMessageAnottation.RequiredUserId)]
    public int UserId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = EnumMessageAnottation.RequiredTaskId)]
    public int TaskId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = EnumMessageAnottation.RequiredCategoryId)]
    public int CategoryId { get; set; }
}
