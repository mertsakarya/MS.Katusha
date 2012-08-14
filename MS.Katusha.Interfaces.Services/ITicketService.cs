using System;

namespace MS.Katusha.Interfaces.Services
{
    public class TicketData
    {
        public DateTime LoginDate { get; set; }
        public Guid Guid { get; set; }
        public long UserRole { get; set; }
    }

    public interface ITicketService
    {
        string EncryptTicket(TicketData ticketData);
        TicketData DecryptTicket(string cipherText);
    }
}
