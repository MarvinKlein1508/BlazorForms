using FormPortal.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormPortal.Core.Models
{
    public abstract class LocalizationModelBase<T> where T : ILocalizationHelper
    {
        public List<T> Description { get; set; } = new();
        public T? GetLocalization(CultureInfo culture)
        {
            var description = Description.FirstOrDefault(x => x.Code.Equals(culture.TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase));
            return description;
        }
    }
}
