namespace Shared.Repository;

public interface IEntity
{
    Guid Id { get; init; }
    byte[] RowVersion { get; set; }
}
