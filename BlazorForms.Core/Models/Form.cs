using BlazorForms.Core.Interfaces;
using BlazorForms.Core.Models.FormElements;

namespace BlazorForms.Core.Models;

public class Form : IDbModel<int?>, IHasSortableElement
{
    private bool _isOnlyAvailableForLoggedInUsers;

    public int FormId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsOnlyAvailableForLoggedInUsers { get => _isOnlyAvailableForLoggedInUsers || AllowedUsersForNewEntries.Count != 0; set => _isOnlyAvailableForLoggedInUsers = value; }
    public bool IsActive { get; set; }
    public int DefaultStatusId { get; set; }
    public int LanguageId { get; set; }
    public byte[] Logo { get; set; } = [];
    public byte[] Image { get; set; } = [];
    public int SortOrder { get; set; }

    /// <summary>
    /// Gets or sets a name which will be applied as default name for new form entries.
    /// </summary>
    public string DefaultName { get; set; } = string.Empty;
    public int? GetIdentifier()
    {
        return FormId > 0 ? FormId : null;
    }
    public bool EntryMode { get; set; }
    public List<FormRow> Rows { get; set; } = [];
    /// <summary>
    /// Gets all users which are capable of creating a new <see cref="FormEntry"/> for this form.
    /// </summary>
    public List<User> AllowedUsersForNewEntries { get; set; } = [];
    /// <summary>
    /// Gets all users which are capable of editing and deleting entries for this form.
    /// </summary>
    public List<User> ManagerUsers { get; set; } = [];
    public virtual Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "FORM_ID", FormId },
            { "NAME", Name },
            { "DESCRIPTION", Description },
            { "LOGO", Logo },
            { "IMAGE", Image },
            { "LOGIN_REQUIRED", IsOnlyAvailableForLoggedInUsers },
            { "IS_ACTIVE", IsActive },
            { "DEFAULT_STATUS_ID", DefaultStatusId },
            { "LANGUAGE_ID", LanguageId <= 0 ? null : LanguageId },
            { "DEFAULT_NAME", DefaultName }
        };
    }

    public void RemoveEmptyRows()
    {
        var list = Rows.Where(x => x.Columns.Count == 0).ToList();
        foreach (var item in list)
        {
            Rows.Remove(item);
        }
    }

    /// <summary>
    /// Gets all elements, excluding elements within <see cref="FormTableElement.Elements"/>
    /// </summary>
    /// <returns></returns>
    public IEnumerable<FormElement> GetElements()
    {
        foreach (var row in Rows)
        {
            foreach (var column in row.Columns)
            {
                foreach (var element in column.Elements)
                {
                    yield return element;
                }
            }
        }
    }
    /// <summary>
    /// Gets all elements including elements within <see cref="FormTableElement.Elements"/>
    /// </summary>
    /// <returns></returns>
    public IEnumerable<FormElement> GetAllElements()
    {
        foreach (var element in GetElements())
        {
            yield return element;
            if (element is FormTableElement tableElement)
            {
                foreach (var table_element in tableElement.Elements)
                {
                    yield return table_element;
                }
            }
        }
    }


    public IEnumerable<FormColumn> GetColumns()
    {
        foreach (var row in Rows)
        {
            foreach (var column in row.Columns)
            {
                yield return column;
            }
        }
    }

    public IEnumerable<FormNumberElement> GetCalcRuleSetElements(bool tableElements)
    {
        foreach (var element in GetElements())
        {
            if (element is FormTableElement formTableElement && tableElements)
            {
                foreach (var table_element in formTableElement.Elements)
                {
                    if (table_element is FormNumberElement formNumberElement)
                    {
                        yield return formNumberElement;
                    }
                }
            }
            else if (element is FormNumberElement formNumberElement && !tableElements)
            {
                yield return formNumberElement;
            }
        }
    }
    public void DeleteRulesForElement(params FormElement[] elements)
    {
        foreach (var row in Rows)
        {
            row.Rules.RemoveAll(x => elements.Contains(x.Element));
            foreach (var column in row.Columns)
            {
                column.Rules.RemoveAll(x => elements.Contains(x.Element));
                foreach (var element in column.Elements)
                {
                    element.Rules.RemoveAll(x => elements.Contains(x.Element));

                    if (element is FormTableElement formTableElement)
                    {
                        foreach (var table_element in formTableElement.Elements)
                        {
                            table_element.Rules.RemoveAll(x => elements.Contains(x.Element));
                        }
                    }

                    if (element is FormNumberElement numberElement)
                    {
                        numberElement.CalcRules.RemoveAll(x => elements.Select(x => x.Guid).Contains(x.GuidElement));
                    }
                }
            }
        }
    }

    public void RemoveRow(FormRow row)
    {
        Rows.Remove(row);
        DeleteRulesForElement(row.GetElements().ToArray());
    }

    public void RemoveColumn(FormColumn column)
    {
        foreach (var row in Rows)
        {
            row.Columns.Remove(column);
        }

        DeleteRulesForElement(column.GetElements().ToArray());
    }

    public string GetDefaultName()
    {
        string defaultName = DefaultName
            .Replace("{DATE}", DateTime.Today.ToShortDateString())
            ;

        return defaultName;
    }
}
