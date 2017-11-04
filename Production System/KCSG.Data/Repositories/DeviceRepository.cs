using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Data.DatabaseContext;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;

namespace KCSG.Data.Repositories
{
    public class DeviceRepository : RepositoryBase<TM14_Device>
    {
        public DeviceRepository(IKCSGDbContext context)
            : base(context)
        {
        }

        public void UpdateStatus(string id, int status)
        {
            var device = GetById(id.Trim());
            device.F14_DeviceStatus = status.ToString();
            device.F14_UpdateDate  = DateTime.Now;
            Update(device);
            
        }

        public TM14_Device GetByDeviceCode(string deviceCode)
        {
            return Get(i => i.F14_DeviceCode.Trim().Equals(deviceCode.Trim()));
        }

        public bool CheckDeviceStatus(string deviceCode)
        {
            var device = Get(i => i.F14_DeviceCode.Trim().Equals(deviceCode));
            if (device == null)
            {
                return false;
            }
            var offlineStatus = Constants.F14_DeviceStatus.Offline.ToString("D");
            var errorStatus = Constants.F14_DeviceStatus.Error.ToString("D");
            var usepermitClass = Constants.F14_UsePermitClass.Prohibited.ToString("D");
            if (device.F14_DeviceStatus.Equals(offlineStatus) || device.F14_DeviceStatus.Equals(errorStatus) || device.F14_UsePermitClass.Equals(usepermitClass))
            {
                return false;
            }
            return true;
        }
    }
}
