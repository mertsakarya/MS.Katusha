using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MS.Katusha.Interfaces.Services
{
    public interface IConfigurationService
    {
        string InitializeDB();
        string ResetDatabaseResources();
    }
}
