namespace KCSG.AutomatedWarehouse.Enumeration
{
    public class MessageCommand
    {
        public const string Send = "0000";
        public const string Receive = "0010";
        public const string Complete = "0100";
        public const string Cancel = "1000";
    }
}