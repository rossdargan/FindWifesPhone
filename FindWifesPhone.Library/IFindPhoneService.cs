﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindWifesPhone.Library
{
    public interface IFindPhoneService
    {
        Task<bool> FindPhone(string username, string password, string deviceName);
    }
}
