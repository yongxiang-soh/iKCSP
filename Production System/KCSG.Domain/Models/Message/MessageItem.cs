namespace KCSG.Domain.Models
{
    public class MessageItem
    {
        /// <summary>
        /// Id of message.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Terminal which sent the message.
        /// </summary>
        public string TerminalNo { get; set; }
        public string PictureNo { get; set; }
        public int Size { get; set; }

        public object WarehouseItem { get; set; }
    }
}