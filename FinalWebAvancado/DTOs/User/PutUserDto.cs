namespace Api.Dto;

public class PutUserDto
{
    public string? Name { get; set; }

    public string? Login { get; set; }

    public string? Password { get; set; }

    public string? ConfirmPassword { get; set; }
}
