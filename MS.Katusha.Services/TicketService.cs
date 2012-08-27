using System;
using System.Globalization;
using MS.Katusha.Domain.Service;
using MS.Katusha.Infrastructure.Exceptions.Services;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Services.Encryption;

namespace MS.Katusha.Services
{
    public class TicketService : ITicketService
    {
        private readonly IEncryptionService _encryptionService;
        private const char Seperator = '|';

        public TicketService(IEncryptionService encryptionService) {
            _encryptionService = encryptionService;
        }

        public string EncryptTicket(TicketData ticketData) { return _encryptionService.Encrypt(ticketData.LoginDate.ToString(CultureInfo.InvariantCulture) + Seperator + ticketData.Guid + Seperator + ticketData.UserRole); }
        public TicketData DecryptTicket(string cipherText) { 
            var arr = _encryptionService.Decrypt(cipherText).Split(Seperator);
            if (arr.Length != 3)
                throw new KatushaTicketException(cipherText);
            var ticketData = new TicketData {LoginDate = DateTime.Parse(arr[0]), Guid = Guid.Parse(arr[1]), UserRole = long.Parse(arr[2])};
            return ticketData;
        }
    }
}