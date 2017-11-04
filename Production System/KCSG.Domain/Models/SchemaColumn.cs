namespace KCSG.Domain.Models
{
    public class SchemaColumn
    {
        /// <summary>
        /// Name of column.
        /// </summary>
        public string Column { get; set; }

        /// <summary>
        /// Position of column in row.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Whether column can be null or not (YES|NO)
        /// </summary>
        public string IsNullable { get; set; }

        /// <summary>
        /// Maximum data length.
        /// </summary>
        public int? MaxLength { get; set; }

        /// <summary>
        /// Type of column (nvarchar|tinyint|...)
        /// </summary>
        public string Type { get; set; }


    }
}