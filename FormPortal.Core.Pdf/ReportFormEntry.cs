using DatabaseControllerProvider;
using FormPortal.Core.Models;
using FormPortal.Core.Models.FormElements;
using System.Text;

namespace FormPortal.Core.Pdf
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
                                    sb.AppendLine($"<label class=\"input-label\">{element}</label>");
                                    sb.AppendLine($"<div class='element'>");
                                    sb.AppendLine($"{textElement.Value}");
                                    sb.AppendLine("</div>");
                                }
                                else if (element is FormLabelElement labelElement)
                                {
                                    string description = labelElement.Description
                                        .Replace("<b>", "<span class=\"fw-bold\">")
                                        .Replace("<strong>", "<span class=\"fw-bold\">")
                                        .Replace("</b>", "</span>")
                                        .Replace("</strong>", "</span>");

                                    sb.AppendLine($"<p>{description}</p>");
                                }
                                else if (element is FormNumberElement numberElement)
                                {
                                    sb.AppendLine($"<label class=\"input-label\">{element}</label>");
                                    sb.AppendLine($"<div class='element'>");
                                    sb.AppendLine($"{numberElement.Value}");
                                    sb.AppendLine("</div>");
                                }
                                else if (element is FormSelectElement selectElement)
                                {
                                    sb.AppendLine($"<label class=\"input-label\">{element}</label>");
                                    sb.AppendLine($"<div class='element'>");
                                    sb.AppendLine($"{selectElement.Value}");
                                    sb.AppendLine("</div>");
                                }
                                else if (element is FormDateElement dateElement)
                                {
                                    sb.AppendLine($"<label class=\"input-label\">{element}</label>");
                                    sb.AppendLine($"<div class='element'>");
                                    sb.AppendLine($"{dateElement.Value.ToShortDateString()}");
                                    sb.AppendLine("</div>");
                                }
                                else if (element is FormRadioElement radioElement)
                                {

                                    sb.AppendLine($"<fieldset>");
                                    sb.AppendLine($"<legend>{element}</legend>");
                                    foreach (var item in radioElement.Options)
                                    {
                                        bool isChecked = item.Name == radioElement.Value;
                                        sb.AppendLine($"<div class=\"radio-wrapper\">");
                                        sb.AppendLine($"<input type='radio' value='{item.Name}' name='{item.GetHashCode()}' {(isChecked ? "checked" : string.Empty)}>");
                                        sb.AppendLine($"<label>{item.Name}</label>");
                                        sb.AppendLine($"</div>");
                                    }
                                    sb.AppendLine($"</fieldset>");
                                }
                                else if (element is FormCheckboxElement checkboxElement)
                                {
                                    bool isChecked = checkboxElement.Value;
                                    sb.AppendLine($"<div>");
                                    sb.AppendLine($"<input type='checkbox' name='{checkboxElement.GetHashCode()}' {(isChecked ? "checked" : string.Empty)} />");
                                    sb.AppendLine($"<label>{checkboxElement.Name}</label>");
                                    sb.AppendLine($"</div>");
                                }
                                else if (element is FormTableElement tableElement)
                                {
                                    sb.AppendLine($"<table class='table table-xs'>");
                                    sb.AppendLine($"<thead>");
                                    sb.AppendLine($"<tr>");
                                    sb.AppendLine($"<th class=\"text-center\" colspan=\"{tableElement.Elements.Count}\">{element.Name}</th>");
                                    sb.AppendLine($"</tr>");
                                    sb.AppendLine($"<tr>");
                                    foreach (var header in tableElement.Elements)
                                    {
                                        sb.AppendLine($"<th class=\"nowrap\">");
                                        sb.AppendLine($"{header.Name}");
                                        sb.AppendLine($"</th>");
                                    }
                                    sb.AppendLine($"</tr>");
                                    sb.AppendLine($"</thead>");
                                    sb.AppendLine($"<tbody>");
                                    foreach (var tableRow in tableElement.ElementValues)
                                    {
                                        sb.AppendLine($"<tr>");
                                        foreach (var table_element in tableRow)
                                        {
                                            sb.AppendLine($"<td>");
                                            if (table_element is FormCheckboxElement tableCheckboxElement)
                                            {
                                                bool isChecked = tableCheckboxElement.Value;
                                                sb.AppendLine($"<div>");
                                                sb.AppendLine($"<input type='checkbox' name='{tableCheckboxElement.GetHashCode()}' {(isChecked ? "checked" : string.Empty)} />");
                                                sb.AppendLine($"</div>");
                                            }
                                            else if (table_element is FormDateElement tableDateElement)
                                            {
                                                sb.AppendLine($"<div class='element'>");
                                                sb.AppendLine($"{tableDateElement.Value.ToShortDateString()}");
                                                sb.AppendLine("</div>");
                                            }
                                            else if (table_element is FormNumberElement tableNumberElement)
                                            {
                                                sb.AppendLine($"<div class='element'>");
                                                sb.AppendLine($"{tableNumberElement.Value}");
                                                sb.AppendLine("</div>");
                                            }
                                            else if (table_element is FormSelectElement tableSelectElement)
                                            {
                                                sb.AppendLine($"<div class='element'>");
                                                sb.AppendLine($"{tableSelectElement.Value}");
                                                sb.AppendLine("</div>");
                                            }
                                            else if (table_element is FormTextElementBase tableTextElement)
                                            {
                                                sb.AppendLine($"<div class='element'>");
                                                sb.AppendLine($"{tableTextElement.Value}");
                                                sb.AppendLine("</div>");
                                            }
                                            sb.AppendLine($"</td>");
                                        }
                                        sb.AppendLine($"</tr>");
                                    }
                                    sb.AppendLine($"</tbody>");
                                    sb.AppendLine($"</table>");
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
