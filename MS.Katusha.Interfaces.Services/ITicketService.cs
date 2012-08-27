using MS.Katusha.Domain.Service;

namespace MS.Katusha.Interfaces.Services
{
    public interface ITicketService
    {
        string EncryptTicket(TicketData ticketData);
        TicketData DecryptTicket(string cipherText);
    }
}
