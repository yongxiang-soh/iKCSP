using KCSG.Domain.Models.MaterialManagement;

namespace KCSG.Domain.Interfaces.MaterialManagement
{
    public interface IMaterialWarehouseCommandDomain
    {
        bool CheckExistence(MaterialWarehouseCommandItem commandItem);
        void Create(MaterialWarehouseCommandItem materialWarehouseCommandItem);
        void Delete(string commandNo, string commandSeqNo);
    }
}
