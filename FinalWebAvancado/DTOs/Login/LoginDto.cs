using System.ComponentModel.DataAnnotations;
using Api.Enums;

namespace Api.Dto;

public class LoginDto
{
    [Required(ErrorMessage = EnumMessageAnottation.RequiredLogin)]
    public string Login { get; set; } = string.Empty;

    [Required(ErrorMessage = EnumMessageAnottation.RequiredPassword)]
    public string Password { get; set; } = string.Empty;
}