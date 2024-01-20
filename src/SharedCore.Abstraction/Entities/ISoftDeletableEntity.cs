namespace SharedCore.Abstraction.Entities;

public interface ISoftDeletableEntity
{
    DateTimeOffset? DeleteDate { get; set; }
}