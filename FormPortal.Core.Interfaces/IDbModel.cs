namespace FormPortal.Core.Interfaces
{
    public interface IDbModel
    {
        int Id { get; }
        Dictionary<string, object?> GetParameters();
    }
}
