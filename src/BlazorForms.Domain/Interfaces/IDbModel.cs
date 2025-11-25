namespace BlazorForms.Domain.Interfaces;

public interface IDbModel<TIdentifier>
{
    TIdentifier GetIdentifier();
}
