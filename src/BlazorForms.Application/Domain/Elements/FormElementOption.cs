using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Application.Domain.Elements;
public sealed class FormElementOption
{
    public int ElementOptionId { get; set; }
    public int ElementId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsDefaultValue { get; set; }
}
