﻿using FormularPortal.Core.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormularPortal.Core.Services
{
    public static class AppdatenService
    {
        private static IConfiguration? _configuration;

        public static async Task InitAsync(IConfiguration configuration)
        {
            _configuration = configuration;

            // TODO: Load anything required
        }
        public static List<FormElement> Elements { get; } = new List<FormElement>
        {
            new FormTextElement { Name = "Text" },
            new FormDateElement { Name = "Date" },
            new FormSelectElement { Name = "Select" },
            new FormCheckboxElement { Name = "Checkbox"},
            new FormTextareaElement { Name = "Textarea" },
            new FormFileElement { Name = "File"},
            new FormRadioElement { Name = "Radio"},
            new FormNumberElement { Name ="Number"},
            new FormTableElement { Name = "Table"},
            new FormLabelElement { Name = "Label"}
        };
    }
}
