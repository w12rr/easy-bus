using FluentValidation;
using SharedCore.Features.FluentValidation;

namespace SharedCore.Messaging.AzureServiceBus.Options;

public sealed class AzureServiceBusOptionsValidator : OptionsValidator<AzureServiceBusOptions>
{
    public AzureServiceBusOptionsValidator()
    {
        RuleFor(x => x.Connections).NotEmpty().ForEach(e =>
        {
            e.ChildRules(cr =>
            {
                cr.RuleFor(x => x.Key).NotEmpty();
                cr.RuleFor(x => x.Value).NotNull().ChildRules(crv =>
                {
                    crv.RuleFor(x => x.Prefix).NotEmpty();
                    crv.RuleFor(x => x.ConnectionString).NotEmpty();
                });
            });
        });
    }
}