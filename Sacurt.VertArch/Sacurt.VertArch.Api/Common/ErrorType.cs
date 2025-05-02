namespace Sacurt.VertArch.Api.Common;

internal enum ErrorType
{
    None = 0,
    Validation = 1,
    Problem = 2,
    NotFound = 3,
    Conflict = 4,
    Unauthorized = 5,
    Forbidden = 6,
    BadRequest = 7,
    InternalServerError = 8
}
