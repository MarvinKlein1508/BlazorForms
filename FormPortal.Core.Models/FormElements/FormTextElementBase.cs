﻿using DatabaseControllerProvider;

namespace FormPortal.Core.Models.FormElements
{
    public abstract class FormTextElementBase : FormElement
    {
        [CompareField("regex_pattern")]
        public string RegexPattern { get; set; } = string.Empty;
        [CompareField("min_length")]
        public int MinLength { get; set; }
        [CompareField("max_length")]
        public int MaxLength { get; set; }

        public string Value { get; set; } = string.Empty;
        public override Dictionary<string, object?> GetParameters()
        {
            var parameters = base.GetParameters();
            parameters.Add("REGEX_PATTERN", RegexPattern);
            parameters.Add("MIN_LENGTH", MinLength);
            parameters.Add("MAX_LENGTH", MaxLength);
            return parameters;
        }
    }
}