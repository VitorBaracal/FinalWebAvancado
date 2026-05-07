using System.ComponentModel.DataAnnotations;
using Api.Enums;

namespace Api.Dto;

public class UserDto
{
    public int Id { get; set; }
    [Required(ErrorMessage = EnumMessageAnottation.RequiredName)]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = EnumMessageAnottation.RequiredLogin)]
    public string Login { get; set; } = string.Empty;
}