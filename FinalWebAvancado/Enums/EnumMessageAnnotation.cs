namespace Api.Enums;

public static class EnumMessageAnottation {
    public const string RequiredName = "The field 'NAME' is required";
    public const string RequiredLogin = "The field 'LOGIN' is required";
    public const string RequiredPassword = "The field 'PASSWORD' is required";
    public const string RequiredConfirmPassword = "The field 'CONFIRMPASSWORD' is required";

    public const string RequiredId = "The field 'ID' is required";
    public const string RequiredUserId = "The field 'USER_ID' is required";
    public const string RequiredLevel = "The field 'LEVEL' is required";
    public const string RequiredStatus = "The field 'STATUS' is required";
    public const string RequiredCreatedAt = "The field 'CREATED_AT' is required";
    public const string RequiredUpdatedAt = "The field 'UPDATED_AT' is required";

    public const string RequiredColorHex = "The field 'COLOR_HEX' is required";
    public const string InvalidColorHex = "The field 'COLOR_HEX' must be a valid hex color (e.g. #RRGGBB)";

    public const string RequiredTaskId = "The field 'TASK_ID' is required";
    public const string RequiredCategoryId = "The field 'CATEGORY_ID' is required";
}