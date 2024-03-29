﻿using DbController;

namespace BlazorForms.Core.Models.FormElements
{
    public abstract class FormTextElementBase : FormElement
    {
        [CompareField("regex_pattern")]
        public string RegexPattern { get; set; } = string.Empty;
        [CompareField("regex_validation_message")]
        public string RegexValidationMessage { get; set; } = string.Empty;
        [CompareField("min_length")]
        public int MinLength { get; set; }
        [CompareField("max_length")]
        public int MaxLength { get; set; }
        [CompareField("value_string")]
        public string Value { get; set; } = string.Empty;
        public override Dictionary<string, object?> GetParameters()
        {
            var parameters = base.GetParameters();
            parameters.Add("REGEX_PATTERN", RegexPattern);
            parameters.Add("REGEX_VALIDATION_MESSAGE", RegexValidationMessage);
            parameters.Add("MIN_LENGTH", MinLength);
            parameters.Add("MAX_LENGTH", MaxLength);

            parameters["VALUE_STRING"] = Value;
            return parameters;
        }

        public override void SetValue(FormEntryElement element)
        {
            Value = element.ValueString;
        }

        public override void Reset()
        {
            Value = string.Empty;
        }
    }
}
