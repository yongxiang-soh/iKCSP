namespace KCSG.AutomatedWarehouse.Enumeration
{
    public enum MaterialCommand
    {
        Storage = 1000,
        Retrieval = 2000,
        Move = 3000,
        StockTakingOff = 7000,
        StockTakingIn = 6000,
        TwoTimesIn = 1001,
        ReRetrieve = 2001
    }
}