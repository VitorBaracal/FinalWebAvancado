using System.ComponentModel.DataAnnotations;
using Api.Enums;

namespace Api.Dto;

public class PutCategoryDto
{
    public string? Name { get; set; }

    [RegularExpression(
        "^#([0-9A-Fa-f]{3}|[0-9A-Fa-f]{6}|[0-9A-Fa-f]{8})$",
        ErrorMessage = EnumMessageAnottation.InvalidColorHex)]
    public string? ColorHex { get; set; }
}
