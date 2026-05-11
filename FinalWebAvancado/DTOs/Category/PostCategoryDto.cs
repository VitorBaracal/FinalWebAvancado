using System.ComponentModel.DataAnnotations;
using Api.Enums;

namespace Api.Dto;

public class PostCategoryDto
{
    [Range(1, int.MaxValue, ErrorMessage = EnumMessageAnottation.RequiredUserId)]
    public int UserId { get; set; }

    [Required(ErrorMessage = EnumMessageAnottation.RequiredName)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = EnumMessageAnottation.RequiredColorHex)]
    [RegularExpression(EnumRegex.ColorHex, ErrorMessage = EnumMessageAnottation.InvalidColorHex)]
    public string ColorHex { get; set; } = string.Empty;
}