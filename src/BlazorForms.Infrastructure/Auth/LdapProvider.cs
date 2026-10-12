using System.DirectoryServices.Protocols;
using System.Net;
using BlazorForms.Infrastructure.Auth;
using Microsoft.Extensions.Options;

namespace BlazorForms.Infrastructure.Auth;

/// <summary>
/// Provides LDAP authentication and user lookup using <see cref="System.DirectoryServices.Protocols.LdapConnection"/>.
/// </summary>
public class LdapProvider
{
    /// <summary>
    /// The LDAP configuration options supplied via DI.
    /// </summary>
    private readonly LdapOptions _ldapOptions;

    /// <summary>
    /// Creates a new instance of <see cref="LdapProvider"/>.
    /// </summary>
    /// <param name="options">The LDAP options wrapper (populated from configuration).</param>
    public LdapProvider(IOptions<LdapOptions> options)
    {
        _ldapOptions = options.Value;
    }

    /// <summary>
    /// Attempts to authenticate the given user against the configured LDAP/Active Directory server.
    /// </summary>
    /// <param name="username">The user login name (SAMAccountName) to authenticate and search for.</param>
    /// <param name="password">The user's password used to bind to the LDAP server.</param>
    /// <returns>
    /// A populated <see cref="LdapResult"/> when authentication and lookup succeed; otherwise <c>null</c>.
    /// Returns <c>null</c> if LDAP is disabled via configuration, if the required GUID attribute is missing,
    /// or if an <see cref="LdapException"/> occurs during bind or search.
    /// </returns>
    public LdapResult? Authenticate(string username, string password)
    {
        if (!_ldapOptions.IsEnabled)
        {
            return null;
        }

        try
        {
            using var connection = new LdapConnection(_ldapOptions.DOMAIN_SERVER);

            var networkCredential = new NetworkCredential(username, password, _ldapOptions.DOMAIN_SERVER);
            connection.SessionOptions.SecureSocketLayer = false;
            connection.AuthType = AuthType.Negotiate;
            connection.Bind(networkCredential);

            var searchRequest = new SearchRequest
            (
                distinguishedName: _ldapOptions.DistinguishedName,
                ldapFilter: $"(SAMAccountName={username})",
                searchScope: SearchScope.Subtree,
                attributeList:
                [
                    "cn",
                    "mail",
                    "displayName",
                    "givenName",
                    "sn",
                    "objectGUID",
                    "memberOf"
                ]
            );

            SearchResponse directoryResponse = (SearchResponse)connection.SendRequest(searchRequest);
            SearchResultEntry searchResultEntry = directoryResponse.Entries[0];
            LdapResult ldapResult = new()
            {
                ShouldCreate = _ldapOptions.ShouldCreateUsers
            };

            foreach (DirectoryAttribute userReturnAttribute in searchResultEntry.Attributes.Values)
            {
                if (userReturnAttribute.Name == "objectGUID")
                {
                    byte[] guidByteArray = (byte[])userReturnAttribute.GetValues(typeof(byte[]))[0];
                    ldapResult.Guid = new Guid(guidByteArray);
                }
                else if (userReturnAttribute.Name == "memberOf")
                {
                    foreach (string item in userReturnAttribute.GetValues(typeof(string)).Cast<string>())
                    {
                        if (item.Contains(_ldapOptions.GroupBaseOU))
                        {
                            string groupName = item.Replace($",{_ldapOptions.GroupBaseOU}", string.Empty).Replace("CN=", string.Empty);
                            ldapResult.Groups.Add(groupName);
                        }
                    }
                }
                else
                {
                    ldapResult.Attributes.Add(userReturnAttribute.Name, (string)userReturnAttribute.GetValues(typeof(string))[0]);
                }
            }

            ldapResult.Attributes.TryAdd("mail", string.Empty);
            ldapResult.Attributes.TryAdd("sn", string.Empty);
            ldapResult.Attributes.TryAdd("givenName", string.Empty);
            ldapResult.Attributes.TryAdd("displayName", string.Empty);

            if (ldapResult.Guid is null)
            {
                return null;
            }

            return ldapResult;
        }
        catch (LdapException)
        {
            return null;
        }
    }
}
