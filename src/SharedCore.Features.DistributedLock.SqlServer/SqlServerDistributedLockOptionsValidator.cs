using FluentValidation;
using SharedCore.Features.FluentValidation;

namespace SharedCore.Features.DistributedLock.SqlServer;

public sealed class SqlServerDistributedLockOptionsValidator : OptionsValidator<SqlServerDistributedLockOptions>
{
    public SqlServerDistributedLockOptionsValidator()
    {
        RuleFor(x => x.ConnectionString).NotEmpty();
    }
}