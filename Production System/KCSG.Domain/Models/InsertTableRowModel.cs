namespace KCSG.Domain.Models
{
    public class InsertTableRowModel
    {
        /// <summary>
        /// Name of column which data should be inserted into.
        /// </summary>
        public string Column { get; set; }

        /// <summary>
        /// Data type of column.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Whether column can be nullable or not.
        /// </summary>
        public string IsNullable { get; set; }

        /// <summary>
        /// Data value.
        /// </summary>
        public object Value { get; set; }
    }
}