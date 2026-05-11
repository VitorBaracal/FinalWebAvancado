using System.ComponentModel.DataAnnotations;
using Api.Enums;

namespace Api.Dto;

public class PutCategoryDto
{
    public string? Name { get; set; }

    [RegularExpression(EnumRegex.ColorHex, ErrorMessage = EnumMessageAnottation.InvalidColorHex)]
    public string? ColorHex { get; set; }
}
