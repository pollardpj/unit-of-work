namespace Shared.Repository;

public interface IEntity<TId>
{
    TId Id { get; init; }
    byte[] RowVersion { get; set; }
}
