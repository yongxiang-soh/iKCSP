namespace KCSG.Domain.Models.MaterialManagement
{
    public class AssignRejectedMaterialResult
    {
        /// <summary>
        /// Total items which have been assigned.
        /// </summary>
        public int Assigned { get; set; } 

        /// <summary>
        /// Total item quantity which have been assigned.
        /// </summary>
        public double AssignedQuantity { get; set; }
    }
}