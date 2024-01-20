namespace SharedCore.Features.FluentValidation;

public static class Consts
{
    public static class ErrorCodes
    {
        public const string OptionCannotBeNull = "OptionCannotBeNull";
        public const string UriIsNotWellFormatted = "UriIsNotWellFormatted";
        public const string UriCannotEndWithSlash = "UriCannotEndWithSlash";
        public const string MustBeDomainName = "MustBeDomainName";
    }
}