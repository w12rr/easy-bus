namespace SharedCore.Infrastructure;

public static class Consts
{
    public static class ProcessResults
    {
        public const string NotFoundMessage = "Resource was not found";
        public const string GotValidationErrorMessage = "Got validation error";
        public const string ValidationDetectedInvalidData = "Validation detected invalid data";
        public const string ConflictMessage = "An conflict occurred while processing the request";
        public const string UnprocessableEntity = "An unprocessable entity occurred while processing the request";

        public const string ProblemDetailsReasonKey = "reason";
        public const string ProblemDetailsTraceIdKey = "traceId";
    }
    
    public static class Serilog
    {
        public const string ApplicationProperty = "Application";
        public const string TraceIdProperty = "TraceId";
        public const string RoleProperty = "Role";
        public const string UserNameProperty = "UserName";
        public const string LocalFormatter = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}";
    }
    
    public static class Configuration
    {
        public const string ConfigurationDirectoryPath = "configuration/";
        public const string ConfigurationPlaceholder = "$VAR$";
        public const string EnvDevelopment = "Development";
        public const string Cors = "Cors";
    }
    
    public static class HealthChecks
    {
        public const string LivenessCheckPath = "/health/liveness";
        public const string ReadinessCheckPath = "/health/readiness";
    }
    
    public static class ErrorCodes
    {
        public const string MustEndWithTopLevelDomainSuffix = "MustEndWithTopLevelDomainSuffix";
    }
}