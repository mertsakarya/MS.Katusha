using System.Collections.Generic;

namespace MS.Katusha.Interfaces.Services
{
    public interface IConfigurationService
    {
        IEnumerable<string> ResetDatabaseResources();
        void GenerateRandomUserAndProfile(int count);
    }
}
