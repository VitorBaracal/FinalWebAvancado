using System.ComponentModel.DataAnnotations;
using Api.Enums;

namespace Api.Dto;

public class PostUserDto
{
    [Required(ErrorMessage = EnumMessageAnottation.RequiredName)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = EnumMessageAnottation.RequiredLogin)]
    public string Login { get; set; } = string.Empty;

    [Required(ErrorMessage = EnumMessageAnottation.RequiredPassword)]
    [RegularExpression(EnumRegex.Password, ErrorMessage = EnumMessageAnottation.PasswordRules)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = EnumMessageAnottation.RequiredConfirmPassword)]
    [RegularExpression(EnumRegex.Password, ErrorMessage = EnumMessageAnottation.PasswordRules)]
    public string ConfirmPassword { get; set; } = string.Empty;
}
