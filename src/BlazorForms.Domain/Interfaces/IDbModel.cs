namespace BlazorForms.Domain;

public interface IDbModel<TIdentifier>
{
    TIdentifier GetIdentifier();
}
