using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Core.Interfaces;

/// <summary>
/// Defines a database model.
/// </summary>
public interface IDbModel<TIdentifier> : IDbParameterizable
{
    /// <summary>
    /// Gets the unique database identifier for the object.
    /// </summary>
    TIdentifier GetIdentifier();
}

/// <summary>
/// Extends the <see cref="IDbModel{TIdentifier}"/> interface with a method for a name.
/// <para>
/// This interface should only be used when you don't plan on using localization for your database objects.
/// </para>
/// </summary>
public interface IDbModelWithName<TIdentifier> : IDbModel<TIdentifier>
{
    /// <summary>
    /// Gets the display name for the object
    /// </summary>
    /// <returns></returns>
    public string GetName();
}

/// <summary>
/// Defines a database model which supports one or more localizable properties.
/// </summary>
public interface ILocalizedDbModel<TIdentifier> : IDbModel<TIdentifier>
{
    /// <summary>
    /// Returns an <see cref="Dictionary{TKey, TValue}"/> of parameters for each available localization.
    /// </summary>
    /// <returns></returns>
    IEnumerable<Dictionary<string, object?>> GetLocalizedParameters();
}

/// <summary>
/// Provides methods to generate parameters for Dapper queries.
/// </summary>
public interface IDbParameterizable
{
    /// <summary>
    /// Gets a dictionary of parameters for the object which can be used in Dapper-Queries.
    /// </summary>
    /// <returns></returns>
    Dictionary<string, object?> GetParameters();
}