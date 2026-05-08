namespace Api.Enums;

public static class EnumMessageReponse {
    public const string InvalidLogin = "Invalid user or password";

    public const string DistinctPasswords = "The passwords entered do not match.";

    public const string UsedLogin = "This login already in use";

    public const string InvalidUserReference = "The specified user does not exist.";

    public const string InvalidTaskReference =
        "The specified task does not exist or does not belong to the user.";

    public const string InvalidCategoryReference =
        "The specified category does not exist or does not belong to the user.";

    public const string TaskCategoryAlreadyLinked =
        "This task is already linked to this category.";
}