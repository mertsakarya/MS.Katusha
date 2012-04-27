namespace MS.Katusha.Interfaces.Services
{
    public interface IUtilityService
    {
        void ClearDatabase(string photosFolder);
        void RegisterRaven();
    }
}
