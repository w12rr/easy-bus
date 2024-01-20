namespace SharedCore.Abstraction.Entities;

public interface IAuditableEntity
{
    DateTimeOffset CreateDate { get; set; }
    DateTimeOffset? ModifyDate { get; set; }
}