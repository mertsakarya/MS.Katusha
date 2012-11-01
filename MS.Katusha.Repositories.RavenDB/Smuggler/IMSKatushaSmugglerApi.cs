namespace MS.Katusha.Repositories.RavenDB.Smuggler
{
    public interface IMSKatushaSmugglerApi
    {
        void ExportData(MSKatushaSmugglerOptions options, bool incremental);
        void ImportData(MSKatushaSmugglerOptions options, bool incremental);
    }
}
