using KCSG.Domain.Models.MaterialManagement;

namespace KCSG.Domain.Interfaces.MaterialManagement
{
    public interface IMaterialShelfStatusDomain
    {
        MaterialShelfStatusItem GetByShelfBayLevel(string shelfRow, string shelfBay, string shelfLevel);
    }
}
