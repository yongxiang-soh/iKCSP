namespace KCSG.AutomatedWarehouse.ViewModel
{
    public class ConverterViewModel
    {
        /// <summary>
        /// Convert text to int.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="defaultReturn"></param>
        /// <returns></returns>
        public int TextToInt(string text, int? defaultReturn)
        {
            try
            {
                return int.Parse(text);
            }
            catch
            {
                if (defaultReturn != null)
                    return defaultReturn.Value;
                throw;
            }
        }
    }
}