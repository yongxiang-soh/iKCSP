namespace KCSG.jsGrid.MVC
{
    public class FieldEvent
    {
        public string EventName { get; set; }
        public string CallbackFunction { get; set; }

        public override string ToString()
        {
            return string.Format("{{ EventName: \"{0}\", CallbackFunction: {1}}},", EventName, CallbackFunction);
        }
    }
}
