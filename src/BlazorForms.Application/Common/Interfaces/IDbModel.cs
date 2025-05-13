namespace BlazorForms.Application.Common.Interfaces;

public interface IDbModel<TIdentifier>
{
    TIdentifier GetIdentifier();
}