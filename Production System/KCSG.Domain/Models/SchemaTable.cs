namespace KCSG.Domain.Models
{
    public class SchemaTable
    {
        /// <summary>
        /// Database which table belongs to.
        /// </summary>
        public string Catalog { get; set; }

        /// <summary>
        /// Schema of table.
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// Name of table.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type of table.
        /// </summary>
        public string Type { get; set; }
    }
}