using System.ComponentModel.DataAnnotations;
using Api.Enums;

namespace Api.Dto;

public class UserCreateDto
{
    [Required(ErrorMessage = EnumTypeAnottation.RequiredName)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = EnumTypeAnottation.RequiredLogin)]
    public string Login { get; set; } = string.Empty;


    [Required(ErrorMessage = EnumTypeAnottation.RequiredPassword)]
    public string Password { get; set; } = string.Empty;
    

    [Required(ErrorMessage = EnumTypeAnottation.RequiredConfirmPassword)]
    public string ConfirmPassword { get; set; } = string.Empty;
}