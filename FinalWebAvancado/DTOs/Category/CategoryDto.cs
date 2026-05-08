using System.ComponentModel.DataAnnotations;
using Api.Enums;

namespace Api.Dto;

public class CategoryDto
{
    [Range(1, int.MaxValue, ErrorMessage = EnumMessageAnottation.RequiredId)]
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = EnumMessageAnottation.RequiredUserId)]
    public int UserId { get; set; }

    [Required(ErrorMessage = EnumMessageAnottation.RequiredName)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = EnumMessageAnottation.RequiredColorHex)]
    [RegularExpression(
        "^#([0-9A-Fa-f]{3}|[0-9A-Fa-f]{6}|[0-9A-Fa-f]{8})$",
        ErrorMessage = EnumMessageAnottation.InvalidColorHex)]
    public string ColorHex { get; set; } = string.Empty;
}
