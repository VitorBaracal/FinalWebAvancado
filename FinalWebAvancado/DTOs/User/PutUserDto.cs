using System.ComponentModel.DataAnnotations;
using Api.Enums;

namespace Api.Dto;

public class PutUserDto
{
    public string? Name { get; set; }

    public string? Login { get; set; }

    [RegularExpression(EnumRegex.Password, ErrorMessage = EnumMessageAnottation.PasswordRules)]
    public string? Password { get; set; }

    [RegularExpression(EnumRegex.Password, ErrorMessage = EnumMessageAnottation.PasswordRules)]
    public string? ConfirmPassword { get; set; }
}
