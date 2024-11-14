using BlazorForms.Core.Models;
using BlazorForms.Core.Models.FormElements;
using BlazorForms.Core.Pdf.Models;
using DbController;
using System.Text;

namespace BlazorForms.Core.Pdf
{
    public class ReportFormEntry : ReportBase
    {
        private readonly FormEntry _entry;

        private ReportFormEntry(FormEntry entry)
        {
            _entry = entry;
            _layoutFile = "FormEntry.html";
            _template = string.Empty;
        }

        public static async Task<ReportFormEntry> CreateAsync(FormEntry entry)
        {
            var report = new ReportFormEntry(entry);
            await report.InitializeAsync();
            return report;
        }

        protected override async Task InitializeAsync()
        {
            string layoutPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LAYOUT_PATH, _layoutFile);
            _template = await File.ReadAllTextAsync(layoutPath);
            SetRules();
            _template = await SetTemplateVariables();
        }
        protected override Task InitializeAsync(IDbController dbController) => throw new NotImplementedException();
        protected override Task<string> SetTemplateVariables()
        {

            string logo = _entry.Form.Logo.Any() ? $"<img src='data:image/png;base64, {Convert.ToBase64String(_entry.Form.Logo)}' />" : string.Empty;

            StringBuilder sb = new StringBuilder();

            foreach (var row in _entry.Form.Rows)
            {
                if (row.IsVisible())
                {
                    sb.AppendLine("<table class=\"table table-borderless table-formentry\">");
                    sb.AppendLine("\t<tbody>");
                    sb.AppendLine($"\t\t<tr>");

                    string columnSize = row.Columns.Count switch
                    {
                        1 => "100%",
                        2 => "50%",
                        3 => "33.33%",
                        4 => "25%",
                        _ => "100%"
                    };


                    foreach (var column in row.Columns)
                    {
                        sb.AppendLine($"\t\t\t<td width=\"{columnSize}\">");
                        if (column.IsVisible())
                        {
                            foreach (var element in column.Elements)
                            {
                                if (!element.IsVisible())
                                {
                                    continue;
                                }

                                if (element is FormTextElementBase textElement)
                                {
                                    sb.AppendLine($"\t\t\t\t<label class=\"input-label\">{element}</label>");
                                    sb.AppendLine($"\t\t\t\t\t<div class='element'>");
                                    sb.AppendLine($"\t\t\t\t\t\t{textElement.Value}");
                                    sb.AppendLine("\t\t\t\t\t</div>");
                                }
                                else if (element is FormLabelElement labelElement)
                                {
                                    if (!labelElement.ShowOnPdf)
                                    {
                                        continue;
                                    }

                                    string description = labelElement.Description
                                        .Replace("<b>", "<span class=\"fw-bold\">")
                                        .Replace("<strong>", "<span class=\"fw-bold\">")
                                        .Replace("</b>", "</span>")
                                        .Replace("</strong>", "</span>");

                                    sb.AppendLine($"\t\t\t\t<p>{description}</p>");
                                }
                                else if (element is FormNumberElement numberElement)
                                {
                                    sb.AppendLine($"\t\t\t\t<label class=\"input-label\">{element}</label>");
                                    sb.AppendLine($"\t\t\t\t<div class='element'>");
                                    sb.AppendLine($"\t\t\t\t\t{numberElement.Value}");
                                    sb.AppendLine("\t\t\t\t</div>");
                                }
                                else if (element is FormSelectElement selectElement)
                                {
                                    sb.AppendLine($"\t\t\t\t<label class=\"input-label\">{element}</label>");
                                    sb.AppendLine($"\t\t\t\t<div class='element'>");
                                    sb.AppendLine($"\t\t\t\t\t{selectElement.Value}");
                                    sb.AppendLine("\t\t\t\t</div>");
                                }
                                else if (element is FormDateElement dateElement)
                                {
                                    sb.AppendLine($"\t\t\t\t<label class=\"input-label\">{element}</label>");
                                    sb.AppendLine($"\t\t\t\t<div class='element'>");
                                    sb.AppendLine($"\t\t\t\t\t{dateElement.Value.ToShortDateString()}");
                                    sb.AppendLine("\t\t\t\t</div>");
                                }
                                else if (element is FormRadioElement radioElement)
                                {

                                    sb.AppendLine($"\t\t\t\t<fieldset>");
                                    sb.AppendLine($"\t\t\t\t\t<legend>{element}</legend>");
                                    foreach (var item in radioElement.Options)
                                    {
                                        bool isChecked = item.Name == radioElement.Value;
                                        sb.AppendLine($"\t\t\t\t\t<div class=\"radio-wrapper\">");
                                        sb.AppendLine($"\t\t\t\t\t\t<input type='radio' value='{item.Name}' name='{item.GetHashCode()}' {(isChecked ? "checked" : string.Empty)}>");
                                        sb.AppendLine($"\t\t\t\t\t\t<label>{item.Name}</label>");
                                        sb.AppendLine($"\t\t\t\t\t</div>");
                                    }
                                    sb.AppendLine($"\t\t\t\t</fieldset>");
                                }
                                else if (element is FormCheckboxElement checkboxElement)
                                {
                                    bool isChecked = checkboxElement.Value;
                                    sb.AppendLine($"\t\t\t\t<div>");
                                    sb.AppendLine($"\t\t\t\t\t<input type='checkbox' name='{checkboxElement.GetHashCode()}' {(isChecked ? "checked" : string.Empty)} />");
                                    sb.AppendLine($"\t\t\t\t\t<label>{checkboxElement.Name}</label>");
                                    sb.AppendLine($"\t\t\t\t</div>");
                                }
                                else if (element is FormTableElement tableElement)
                                {
                                    sb.AppendLine($"\t\t\t\t<table class='table table-xs form-table'>");
                                    sb.AppendLine($"\t\t\t\t\t<thead>");
                                    sb.AppendLine($"\t\t\t\t\t\t<tr>");
                                    sb.AppendLine($"\t\t\t\t\t\t\t<th class=\"text-center\" colspan=\"{tableElement.Elements.Count}\">{element.Name}</th>");
                                    sb.AppendLine($"\t\t\t\t\t\t</tr>");
                                    sb.AppendLine($"\t\t\t\t\t\t<tr>");
                                    foreach (var header in tableElement.Elements)
                                    {
                                        sb.AppendLine($"\t\t\t\t\t\t\t<th class=\"nowrap\">");
                                        sb.AppendLine($"\t\t\t\t\t\t\t\t{header.Name}");
                                        sb.AppendLine($"\t\t\t\t\t\t\t</th>");
                                    }
                                    sb.AppendLine($"\t\t\t\t\t\t</tr>");
                                    sb.AppendLine($"\t\t\t\t\t</thead>");
                                    sb.AppendLine($"\t\t\t\t\t<tbody>");

                                    var totals = new Dictionary<int, TableSum>();

                                    foreach (var tableRow in tableElement.ElementValues)
                                    {
                                        sb.AppendLine($"\t\t\t\t\t\t<tr>");
                                        foreach (var table_element in tableRow)
                                        {
                                            sb.AppendLine($"\t\t\t\t\t\t\t<td class='element'>");
                                            if (table_element is FormCheckboxElement tableCheckboxElement)
                                            {
                                                bool isChecked = tableCheckboxElement.Value;
                                                sb.AppendLine($"\t\t\t\t\t\t\t\t<input type='checkbox' name='{tableCheckboxElement.GetHashCode()}' {(isChecked ? "checked" : string.Empty)} />");
                                            }
                                            else if (table_element is FormDateElement tableDateElement)
                                            {
                                                sb.AppendLine($"\t\t\t\t\t\t\t\t{tableDateElement.Value.ToShortDateString()}");
                                            }
                                            else if (table_element is FormNumberElement tableNumberElement)
                                            {
                                                string numberFormat = "0.";
                                                for (int i = 0; i < tableNumberElement.DecimalPlaces; i++)
                                                {
                                                    numberFormat += "0";
                                                }

                                                if (tableNumberElement.IsSummable)
                                                {
                                                    if (totals.ContainsKey(tableNumberElement.SortOrder))
                                                    {
                                                        totals[tableNumberElement.SortOrder].Value += tableNumberElement.Value;
                                                    }
                                                    else
                                                    {
                                                        totals.Add(tableNumberElement.SortOrder, new TableSum(numberFormat, tableNumberElement.Value));
                                                    }
                                                }

                                                sb.AppendLine($"\t\t\t\t\t\t\t\t{tableNumberElement.Value.ToString(numberFormat)}");
                                            }
                                            else if (table_element is FormSelectElement tableSelectElement)
                                            {
                                                sb.AppendLine($"\t\t\t\t\t\t\t\t{tableSelectElement.Value}");
                                            }
                                            else if (table_element is FormTextElementBase tableTextElement)
                                            {
                                                sb.AppendLine($"\t\t\t\t\t\t\t\t{tableTextElement.Value}");
                                            }
                                            sb.AppendLine($"\t\t\t\t\t\t\t</td>");
                                        }
                                        sb.AppendLine($"\t\t\t\t\t\t</tr>");

                                    }
                                    if (totals.Any())
                                    {
                                        sb.AppendLine($"\t\t\t\t\t\t<tr>");
                                        sb.AppendLine($"\t\t\t\t\t\t\t<th class=\"text-center\" colspan=\"{tableElement.Elements.Count}\">Summen</th>");
                                        sb.AppendLine($"\t\t\t\t\t\t</tr>");
                                        sb.AppendLine("\t\t\t\t\t\t<tr>");
                                        for (int i = 1; i <= tableElement.Elements.Count; i++)
                                        {
                                            if (totals.TryGetValue(i, out TableSum? total))
                                            {
                                                sb.AppendLine($"\t\t\t\t\t\t\t<td class='element'>");
                                                sb.AppendLine($"\t\t\t\t\t\t\t\t{total.Value.ToString(total.NumberFormat)}");
                                                sb.AppendLine($"\t\t\t\t\t\t\t</td>");
                                            }
                                            else
                                            {
                                                sb.AppendLine("\t\t\t\t\t\t\t<td></td>");
                                            }
                                        }
                                        sb.AppendLine("\t\t\t\t\t\t</tr>");
                                    }
                                    sb.AppendLine($"\t\t\t\t\t</tbody>");
                                    sb.AppendLine($"\t\t\t\t</table>");
                                }

                            }
                        }
                        sb.AppendLine($"\t\t\t</td>");
                    }
                    sb.AppendLine($"\t\t</tr>");
                }
                sb.AppendLine("\t</tbody>");
                sb.AppendLine("</table>");
            }



            _template = _template
               .Replace("{{ DISPLAY_CSS }}", GenerateDisplayCss())
               .Replace("{{ CONTENT }}", sb.ToString())
               .Replace("{{ LOGO }}", logo)
               .Replace("{{ TITLE }}", _entry.Form.Name)
               .Replace("{{ CURRENT_DATE }}", DateTime.Now.ToLongDateString())
               ;

            return Task.FromResult(_template);
        }
    }
}
