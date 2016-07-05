using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindWifesPhoneDNX.Library
{
    public interface IFindPhoneService
    {
        Task<bool> FindPhone(string deviceName);
    }
}
