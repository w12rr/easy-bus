using FluentValidation;
using SharedCore.Features.FluentValidation;

namespace SharedCore.Features.DistributedLock.MySql;

public sealed class MySqlDistributedLockOptionsValidator : OptionsValidator<MySqlDistributedLockOptions>
{
    public MySqlDistributedLockOptionsValidator()
    {
        RuleFor(x => x.ConnectionString).NotEmpty();
    }
}