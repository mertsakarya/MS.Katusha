namespace MS.Katusha.Infrastructure.Exceptions.Services
{
    public class KatushaTicketException : KatushaException
    {
        public KatushaTicketException(string cipherText)
            : base("TicketError", "Unparsable ticket.", null)
        {
            Ticket = cipherText;
        }

        public string Ticket { get; private set; }
    }
}
