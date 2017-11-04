namespace KCSG.jsGrid.MVC.Enums
{
    public enum SortOrder
    {
        Asc,
        Desc
    }

    public enum ColumnType
    {
        Text,
        Number,
        Checkbox,
        Select,
        TextArea,
        Control,
        Radio,
        SingleSuggestion,
        MultiSuggestion
    }

    public enum GridMode
    {
        Listing,
        Editing
    }

    public enum Align
    {
        Center,
        Left,
        Right,
    }

    public struct BuiltInSortingStrategy
    {
        /// <summary>
        /// string sorter
        /// </summary>
        public const string String = "string";
        /// <summary>
        /// number sorter
        /// </summary>
        public const string Number = "number";
        /// <summary>
        /// date sorter
        /// </summary>
        public const string Date = "date";
        /// <summary>
        /// numbers are parsed before comparison
        /// </summary>
        public const string NumberAsString = "numberAsString";
    }

    public struct BuiltInValidator
    {
        /// <summary>
        /// the field value is required
        /// </summary>
        public const string Required = "required";
        /// <summary>
        /// the length of the field value is limited by range (the range should be provided as an array in param field of validation config)
        /// </summary>
        public const string RangeLength = "rangeLength";
        /// <summary>
        /// the minimum length of the field value is limited (the minimum value should be provided in param field of validation config)
        /// </summary>
        public const string MinLength = "minLength";
        /// <summary>
        /// the maximum length of the field value is limited (the maximum value should be provided in param field of validation config)
        /// </summary>
        public const string MaxLength = "maxLength";
        /// <summary>
        /// the field value should match the defined pattern (the pattern should be provided as a string regexp in param field of validation config)
        /// </summary>
        public const string Pattern = "pattern";
        /// <summary>
        /// the value of the number field is limited by range (the range should be provided as an array in param field of validation config)
        /// </summary>
        public const string Range = "range";
        /// <summary>
        /// the minimum value of the number field is limited (the minimum should be provided in param field of validation config)
        /// </summary>
        public const string Min = "min";
        /// <summary>
        /// the maximum value of the number field is limited (the maximum should be provided in param field of validation config)
        /// </summary>
        public const string Max = "max";
    }
}
