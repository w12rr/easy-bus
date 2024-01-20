namespace SharedCore.Abstraction.Entities;

public interface IIdentifiableEntity<TId>
    where TId : struct
{
    TId Id { get; set; }
}