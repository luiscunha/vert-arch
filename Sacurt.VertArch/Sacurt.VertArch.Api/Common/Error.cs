namespace Sacurt.VertArch.Api.Common
{
    internal record Error(ErrorType Type, string Code, string Message)
    {
        public static readonly Error None = new(ErrorType.None, string.Empty, string.Empty);
         
        public static readonly Error NullValue = new(ErrorType.InternalServerError, "Error.NullValue", "The specified result value is null.");

        public static readonly Error ConditionNotMet = new(ErrorType.Validation, "Error.ConditionNotMet", "The specified condition was not met.");
    }
}
