#if DEBUG
#define CREATE_HTML
#endif
using iText.Html2pdf;
using System.Text;

namespace BlazorForms.Core.Pdf
{
    /// <summary>
    /// Represents the basis of an implementation for a new print template.
    /// </summary>
    public abstract class ReportBase
    {
        /// <summary>
        /// Gets the base path for all template files.
        /// <para>
        /// This value should not be changed!
        /// </para>
        /// </summary>
        public const string LAYOUT_PATH = "Pdf\\Templates";

        /// <summary>
        /// Gets the filename of the selected layout.
        /// <para>
        /// Example: Offer.html
        /// </para>
        /// </summary>
        protected string _layoutFile = String.Empty;
        /// <summary>
        /// Gets the HTML content of the print layout.
        /// <para>
        /// Will be filled in <see cref="InitializeAsync" /> with the data of the <see cref="_layoutFile"/>. All placeholders will be replaced in <see cref="SetTemplateVariables" />.
        /// </para>
        /// </summary>
        protected string _template = String.Empty;

        protected ReportBase()
        {

        }

        /// <summary>
        /// Generates the document as a byte array
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(_template);
            MemoryStream stream = new MemoryStream(byteArray);
            ConverterProperties converterProperties = new ConverterProperties()
                .SetBaseUri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LAYOUT_PATH));

            MemoryStream output = new MemoryStream();

            HtmlConverter.ConvertToPdf(stream, output, converterProperties);

            return output.ToArray();
        }

        /// <summary>
        /// Generates the document as a PDF in the specified location.
        /// </summary>
        /// <param name="path">Der Pfad in dem das Dokument gespeichert werden soll, ohne Dateinamen.</param>
        /// <returns>Returns the full file name of the generated PDF.</returns>
        public virtual void Print(string filename)
        {
#if CREATE_HTML
            File.WriteAllText($"{filename}.html", _template);
#endif
            byte[] byteArray = Encoding.UTF8.GetBytes(_template);
            MemoryStream stream = new MemoryStream(byteArray);
            ConverterProperties converterProperties = new ConverterProperties()
                .SetBaseUri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LAYOUT_PATH));

            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            HtmlConverter.ConvertToPdf(stream, new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None), converterProperties);
        }
        /// <summary>
        /// Sets display rules for generating the PDF.
        /// </summary>
        protected virtual void SetRules()
        {
            return;
        }
        /// <summary>
        /// Overwrites the layout's placeholder variables with the corresponding content.
        /// </summary>
        /// <returns></returns>
        protected abstract Task<string> SetTemplateVariables();
        /// <summary>
        /// Initializes the print template
        /// </summary>
        /// <returns></returns>
        protected abstract Task InitializeAsync();
        /// <summary>
        /// Initializes the print template
        /// </summary>
        /// <returns></returns>
        protected abstract Task InitializeAsync(IDbController dbController);
        /// <summary>
        /// Generates the style section that hides all classes that have been set by <see cref="SetRules"/>.
        /// </summary>
        /// <returns></returns>
        protected virtual string GenerateDisplayCss()
        {
            return string.Empty;
        }
    }
}
