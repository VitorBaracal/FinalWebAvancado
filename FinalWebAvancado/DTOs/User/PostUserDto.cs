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
    public string Password { get; set; } = string.Empty;
    
    [Required(ErrorMessage = EnumMessageAnottation.RequiredConfirmPassword)]
    public string ConfirmPassword { get; set; } = string.Empty;
}
