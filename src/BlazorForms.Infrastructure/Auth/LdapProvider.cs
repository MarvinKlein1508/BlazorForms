using System.DirectoryServices.Protocols;
using System.Net;

namespace BlazorForms.Infrastructure.Auth;

public static class LdapProvider
{
    public static LdapResult? Authenticate(string username, string password, LdapSettings ldapSettings)
    {
        try
        {
            using var connection = new LdapConnection(ldapSettings.DomainServer);

            var networkCredential = new NetworkCredential(username, password, ldapSettings.DomainServer);
            connection.SessionOptions.SecureSocketLayer = false;
            connection.AuthType = AuthType.Negotiate;
            connection.Bind(networkCredential);

            var searchRequest = new SearchRequest
            (
                distinguishedName: ldapSettings.DistinguishedName,
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
            LdapResult ldapResult = new();

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
                        ldapResult.Groups.Add(item);
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

