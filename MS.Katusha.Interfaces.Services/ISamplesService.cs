namespace MS.Katusha.Interfaces.Services
{
    public interface ISamplesService
    {
        void GenerateRandomUserAndProfile(int count, int extra = 0);
        void GenerateRandomConversation(int count, int extra = 0);
        void GenerateRandomVisit(int count, int extra = 0);
    }
}