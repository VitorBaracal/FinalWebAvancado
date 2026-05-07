using System.ComponentModel.DataAnnotations;
using Api.Enums;

namespace Api.Dto;

public class UserDto
{
    public int Id { get; set; }
    [Required(ErrorMessage = EnumTypeAnottation.RequiredName)]
    public string Nome { get; set; } = string.Empty;
    
    [Required(ErrorMessage = EnumTypeAnottation.RequiredLogin)]
    public string Login { get; set; } = string.Empty;
}