namespace Api.Enums;

public static class EnumRegex
{
    public const string Password = @"^(?=.*[A-Z])(?=.*[^a-zA-Z0-9]).{8,}$";

    public const string ColorHex = "^#([0-9A-Fa-f]{3}|[0-9A-Fa-f]{6}|[0-9A-Fa-f]{8})$";
}
