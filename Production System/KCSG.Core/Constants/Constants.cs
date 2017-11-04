using System.ComponentModel;

namespace KCSG.Core.Constants
{
    public static partial class Constants
    {
        public enum CerFlag
        {
            [Description("Yet")] Yet = 0,
            [Description("OK")] CertOK = 1,
            [Description("NG")] CertNG = 2,
            [Description("StrgOK")] StrgOK = 3,
            [Description("StrNG")] StrgNG = 4
        }

        public enum CodeLabel
        {
            [Description("No")] No,
            [Description("ATM")] Atm
        }

        #region Material

        public enum CommWthMeasureSys
        {
            None = 0,
            Conventional = 1,
            Megabit = 2,
            Both = 3
        }

        #endregion

        public enum Crushing
        {
            [Description("Required")] Required = 1,
            [Description("Not Required")] NotRequired = 0
        }

        public enum Default
        {
            value = 0
        }


        public enum EnvActionBuffer
        {
            INSERT_DB = 2,
            UPDATE_DB = 1,
            DELETE_DB = 3,
            UPDATE_DB_REC = 4
        }

        public enum EnvDMent
        {
            RAWDT = 1,
            MODDT = 0
        }

        public enum EnvFieldNo
        {
            FLD_DATA = 1,
            FLD_MEAN = 2,
            FLD_UPPER = 3,
            FLD_LOWER = 4
        }

        public enum EnvironmentStatus
        {
            [Description("Rotate")] Rotate = 'O',
            [Description("Empty Load")] Empty = 'A',
            [Description("Stop")] Stop = 'S',
            [Description("Notaval")] Notaval = 'E',
            [Description("Esterik")] Esterik = '*'
        }

        public enum EnvLine
        {
            [Description("Conventional Line")] Line_Conv = 0,
            [Description("Megabit Line")] Line_Mega
        }

        public enum EnvMode
        {
            [Description("Spec Line")] SpecLine = 0,
            [Description("Control Line")] ControlLine = 1
        }

        public enum EnvType
        {
            TYPE_RM = 1,
            TYPE_MC = 2,
            TYPE_RS = 3,
            TYPE_LT = 4
        }

        public enum ExternalLabel
        {
            [Description("No")] No = 1,
            [Description("Piece")] Piece = 2,
            [Description("ATM")] Atm = 3,
            [Description("955Buzen")] Buzen = 4,
            [Description("STLGG")] Stlgg = 5,
            [Description("Chippac")] Chippac = 6,
            [Description("STS")] Sts = 7,
            [Description("Renesas")] Renesas = 8,
            [Description("GTBF")] GTBF = 9
        }


        public enum F05_StrRtrSts
        {
            [Description("Not Use")] NotUse = 0,
            [Description("Storage")] Storage,
            [Description("Retrieval")] Retrieval,
            [Description("Move")] Move,
            [Description("Error")] Error = 9
        }

        public enum F14_DeviceStatus
        {
            Online = 0,
            Offline = 1,
            Error = 2
        }

        public enum F14_UsePermitClass
        {
            Available = 0,
            Prohibited = 1
        }

        public enum F31_LiquidClass
        {
            NonLiquid = 0,
            Liquid = 1
        }

        public enum F31_ShelfStatus
        {
            EmptyShelf = 0,
            WarehousePallet = 1,
            SupplierPallet = 2,
            Material = 3,
            ReservedForStorage = 4,
            ReservedForRetrieval = 5,
            Prohibited = 6,
            PhysicallyProhibited = 7
        }

        // TX34_MtrWhsCmd
        // F34_CommandNo
        public enum F34_CommandNo
        {
            Storage = 1000,
            Retrieval = 2000,
            Move = 3000,
            StockTakingOff = 7000,
            StockTakingIn = 6000,
            TwoTimesIn = 1001,
            ReRetrieve = 2001
        }

        public enum F37_lowtmpcls
        {
            None = 0,
            One = 1
        }

        public enum F37_ShelfStatus
        {
            EmptyShelf = 0, //Empty Shelf
            EmptyContainer = 1, //Empty Container
            Stock = 3, //Stock
            ReservedForStorage = 4, //Reserved for Storage
            ReservedForRetrieval = 5, // Reserved for Retrieval
            Prohibited = 6, // Prohibited
            PhysicallyProhibited = 7 // Physically  Prohibited
        }

        public enum F37_StockTakingFlag
        {
            InventoryNoChecked = 0, // Inventory not Checked
            InventoryChecked = 1 // Inventory Checked
        }

        public enum F39_Status
        {
            NotCommanded = 0, // Not Commanded
            Commanded = 1, // Commanded
            Kneading = 2, // Kneading
            Completed = 3, // Completed
            ForcedCompletion = 4, // Forced Completion
            Termination = 5, // Termination
            Terminating = 6
        }

        //public enum F40_StockFlag
        //{
        //    TX40_StkFlg_NotStk = 0, // Not in Stock
        //    TX40_StkFlg_Str = 1, // Storing
        //    TX40_StkFlg_Rtr = 2, // Retrieving
        //    TX40_StkFlg_Stk = 3, // Stocked
        //    TX40_StkFlg_NotStkShip = 4, // Not in Stock because of shipping
        //    TX40_StkFlg_Shipping = 5 // Shipping
        //}

        public enum F40_CertificationFlag
        {
            [Description("Yet")] // not Certificaiton
            TX40_CrtfctnFlg_Non = 0,

            [Description("CertOK")] // Certificaiton OK
            TX40_CrtfctnFlg_Ok = 1,

            [Description("CertNG")] // Certificaiton NG
            TX40_CrtfctnFlg_NG = 2,

            [Description("StrgOK")] // Certificaiton Normal
            TX40_CrtfctnFlg_Normal = 3,

            [Description("StrgNG")] // Certificaiton Out of Sepc
            TX40_CrtfctnFlg_OutofSpec = 4
        }

        public enum F42_MtrRtrFlg
        {
            NoTretrieval = 0, // not retrieval
            Retrievaled = 1 //retrievaled
        }

        public enum F42_OutSideClass
        {
            PreProduct = 0,
            ExternalPreProduct,
            OutsideProduct = 1
        }

        public enum F47_CommandNo
        {
            Storage = 1000, //Storage
            Retrieval = 2000, //Retrieval
            Move = 3000, //Move
            StockTakingOff = 7000, //Stock talking off
            StockTakingIn = 6000, //Stock taking in
            TwoTimes = 1001 //Two times in
        }

        public enum F47_Status
        {
            AnInstruction = 0,
            InstructionComplete = 6,
            InstructionCancel = 7,
            DoubleInstallationError = 8,
            AnInstructionCompleteConfirmation = 'C',
            AnInstructionCancelConfirmation = 'D',
            ADoubleInstallationConfirmation = 'E',
            AnEmptyRetrievalConfirm = 'F'
        }

        public enum F47_StrRtrType
        {
            [Description("Product")] Product = 0,
            [Description("Pallet For Warehouse")] PalletForWarehouse = 1,
            [Description("External Pre-Product")] ExternalPreProduct = 2,
            [Description("Out Of Spec Pre Product")] OutOfSpecPreProduct = 3,
            [Description("Product Restorage")] ProductRestorage = 4
        }

        public enum F50_CommandNo
        {
            CmdNoStrg = 1000, // Storage
            CmdNoRtr = 2000, //Retrieval and Middle Command
            CmdNoMove = 4000, // Move Forward Changed
            CmdNoStkRtr = 7000, // Stock taking off
            CmdNoStkStr = 6000, // Stock Taking in
            CmdNoTwoTimes = 1001 // Two times in
        }

        public enum F50_CommandType
        {
            CmdType_0 = 0000,
            CmdType_1 = 0001,
            CmdType_2 = 0100
        }

        public enum F50_StrRtrType
        {
            [Description("Pre-product")] StrRtrType_PrePdt = 0, //Pre-product
            [Description("Container")] StrRtrType_Container = 1 //Container
        }

        public enum F51_ShelfType
        {
            Normal = 0, //Normal Shelf
            BadUse = 1 //Shelf Used for bad  product
        }

        public enum F55_Status
        {
            SentFromAToC = 0,
            KneadOver
        }

        public enum F58_Status
        {
            StorageComplete = 3,
            StorageOver = 4
        }

        public enum F67_SampleFlag
        {
            Sample = 0, //Sample
            Product = 1, //Product
            OutOfPlan = 2 //Out Of Plan
        }

        public enum GetColumnInNoManager
        {
            Pdtwhscmdno = 1,
            MtrWhsCmdNo,
            PrePdtWhsCmdNo,
            KndCmdBookNo
        }

        public enum Inout
        {
            Retrieval = 1
        }


        public enum InternalLabel
        {
            [Description("No")] No = 1,
            [Description("1 Piece")] Piece = 2,
            [Description("2 Pieces")] Pieces = 3,
            [Description("ATM 1P")] Atm1P = 4,
            [Description("ATM 2P")] Atm2P = 5,
            [Description("955 Buzen 1P")] Buzen1P = 6,
            [Description("955 Buzen 2P")] Buzen2P = 7,
            [Description("STLGG 1P")] Stlgg1P = 8,
            [Description("STLGG 2P")] Stlgg2P = 9,
            [Description("STS 1P")] Sts1P = 10,
            [Description("STS 2P")] Sts2P = 11,
            [Description("Renesas")] Renesas = 12,
            [Description("Renesas 2P")] Renesas2P = 13
        }

        public enum Line
        {
            All = 0,
            General,
            Megabit
        }

        public enum LockStatus
        {
            [Description("Lock")] Lock = 0,
            [Description("UnLock")] UnLock = 1
        }


        public enum MessageId
        {
            TC_MID_Cmd = 1,
            TC_MID_ReCmd = 2,
            TC_MID_CmdCancel = 3,
            TC_MID_CmdEnd = 4,
            TC_MID_ReStoraged = 41,
            TC_MID_EmptyRetrieve = 42
        }

        public enum PrintProductCertificationStatus
        {
            [Description("Certificated")] Certificated = 0,
            [Description("Out-of-Spec")] OutOfSpec = 1,
            [Description("Uncertificated")] Uncertificated = 2
        }

        //Environment
        public enum ProcType
        {
            CALC_TE81_TEMP = 0,
            CALC_TETMP_TEMP90 = 1,
            CALC_TE83 = 2,
            CALC_TETMP_RLSPD = 3
        }

        public enum ProductClassification
        {
            [Description("Normal")] Normal = 0,
            [Description("Out of plan")] OutOfPlan = 1
        }

        public enum RollMachine
        {
            [Description("18 Inch Mixing Roll Machine")] Roll_MC_18 = 0,
            [Description("12 Inch Mixing Roll Machine")] Roll_MC_12 = 1
        }

        public enum SelectData
        {
            [Description("Data on Queue")] DataOnQueue = 0,
            [Description("Sent Data")] SentData = 1,
            [Description("All Data")] AllData = 2
        }

        public enum ShelfStatus
        {
            [Description("Empty")] Empty = 0,
            [Description("Pallet")] Pallet = 1,
            [Description("Out/Spec")] OutSpec = 3,
            [Description("Stor")] Stor = 4,
            [Description("Retr")] Retr = 5,
            [Description("Forbid")] Forbid = 6
        }

        public enum ShelfStatusSearch
        {
            [Description("Empty Pallet")] EmptyPallet = 0,
            [Description("Stock")] Stock
        }

        public enum SqlDataType
        {
            DateTime,
            Numeric,
            Unicode,
            Text
        }

        public enum Status
        {
            Yet = 0,
            Command = 1,
            Produce = 2,
            End = 3,
            FEnd = 4,
            Stopped = 5,
            Stopping = 6
        }

        public enum StatusRequest
        {
            [Description("Time off")] Timeoff,
            [Description("Time on")] Timeon
        }

        public enum StorageOfProductStatus
        {
            [Description("Normal")] Normal = 0,
            [Description("Out of Plan")] OutOfPlan = 1,
            [Description("Sample")] Sample = 2
        }

        public enum SupplierName
        {
            [Description("KCSP")] Kcsp,

            [Description("KAP")] Kap
        }

        public enum TM01_Material_EntrustedClass
        {
            Normal = 0,
            Bailment = 1
        }

        public enum TX30_Reception
        {
            [Description("Pending")] NonAccept = 0, //TX30_AcpCls_NonAcp
            [Description("Accepted")] Accepted = 1, //TX30_AcpCls_Acp
            [Description("Rejected")] Rejected = 2, //TX30_AcpCls_Rjt
            [Description("Pending")] Pending = 3
        }


        public enum TX31_MtrShfSts_LiquidFlag
        {
            [Description("Liquid")] Liquid = 0,
            [Description("Non-Liquid")] NonLiquid = 1
        }

        public enum TX31_StkTkgFlg
        {
            [Description("Inventory Not Checked")] InvNotChk = 0,
            [Description("Inventory Checked")] InvChk = 1
        }

        public enum TX32_CSndEndFlg
        {
            [Description("Conventional Not Send")] NotSend = 0,
            [Description("Conventional Send Over")] SendOver = 1
        }

        public enum TX32_MSndEndFlg
        {
            [Description("Megabit Not Send")] NotSend = 0,
            [Description("Megabit Send Over")] SendOver = 1
        }

        public enum TX33_MtrShfStk
        {
            NotInStock = 0, //TX33_StkFlg_NotStk
            Storing, //TX33_StkFlg_Str
            Retrieving = 2, //TX33_StkFlg_Rtr
            Stocked //TX33_StkFlg_Stk
        }

        public enum TX33_StkFlg
        {
            [Description("Not in Stock")] NotStoked = 0,
            [Description("Storing")] Store = 1,
            [Description("Retrieving")] Retrieve = 2,
            [Description("Stocked")] Stocked = 3
        }

        // F34_StrRtrType

        public enum TX34_Status
        {
            Status0 = 0
        }

        public enum TX34_StrRtrType
        {
            [Description("Material")] Material = 0,
            [Description("Warehouse Pallet")] WarehousePallet = 1,
            [Description("Supplier Pallet")] SupplierPallet = 2,
            [Description("Supplier Pallet Side In")] SupplierPalletSideIn = 3,
            [Description("Material Restorage")] MaterialReStorage = 4
        }

        public enum TX41_Status
        {
            [Description("Not Tablet")] NotTablet = 3,
            [Description("Pre-product Retrieving")] PreproductRetrieving = 4,
            [Description("Retrievaling Stoped")] RetrievalingStoped = 5,
            [Description("Retrieval Over")] RetrievalOver = 6,

            [Description("Container Set Error")] ContainerSetError = 0,
            [Description("Container Set")] ContainerSet = 1,
            [Description("Container Set Wait")] ContainerSetWait = 2,
            [Description("Tableted")] Tableted = 7,
            [Description("Stored")] Stored = 8,
            [Description("Default")] Default = 9
        }

        public enum TX56_Status
        {
            [Description("Yet")] Yet = 0,
            [Description("Tableting")] Tableting = 1,
            [Description("Change")] Change = 2,
            [Description("Tableting Over")] TabletingOver = 3,
            [Description("Storage Over")] StorageOver = 4
        }

        public enum TypeOfTable
        {
            CALC_TE81_TEMP = 1,
            CALC_TE81_HUMID = 2,
            CALC_TETMP_TEMP = 3,
            CALC_TETMP_HUMID = 4,
            CALC_TE81_TEMP90 = 5,
            CALC_TE81_HUMID90 = 6,
            CALC_TETMP_TEMP90 = 7,
            CALC_TETMP_HUMID90 = 8,
            CALC_MGMT_LIMIT = 9,
            CALC_MGMT_LIMIT_TEMP = 10,
            CALC_MGMT_LIMIT_HUMID = 11,
            CALC_BUFFER = 12,
            CALC_TE83 = 20,
            CALC_TETMP_RLSPD = 30,
            CALC_TE83_90 = 40,
            CALC_TETMP_RLSPD90 = 50,
            CALC_LOT_AVG = 61,
            CALC_LOT_RANGE = 62
        }

        public enum ViewSelected
        {
            [Description("Monitor")] Monitor,
            [Description("Command Queue")] CommandQueue,
            [Description("Command History")] CommandHistory
        }

        public enum ViewSelectedC4
        {
            [Description("Material Master")] MasterialMaster,
            [Description("Kneading Command")] KneadingCommand,
            [Description("Pre-product Master")] PreproductMaster,

            [Description("Kneading Results")] KneadingResutls,
            [Description("Retrieval")] Retrieval,
            [Description("Monitor")] Monitor
        }

        public const string SystemUser = "System";

        //
        public struct F34_Status
        {
            public const string status0 = "0";
            public const string status1 = "1";
            public const string status2 = "2";
            public const string status3 = "3";
            public const string status4 = "4";
            public const string status5 = "5";
            public const string status6 = "6";
            public const string status7 = "7";
            public const string status8 = "8";
            public const string status9 = "9";

            public const string statusA = "A";
            public const string statusB = "B";
            public const string statusC = "C";
            public const string statusD = "D";
            public const string statusE = "E";
            public const string statusF = "F";
        }

        // TM07_Calendar
// F07_HolidayFlag
        public struct F07_HolidayFlag
        {
            public const string TM07_HldyFlg_Non = "0"; // non holiday
            public const string TM07_HldyFlg_Hldy = "1"; // holiday
// F07_SunSatDayFlag
            public const string TM07_SunSatDayFlag_Non = "0"; // non 
            public const string TM07_SunSatDayFlag_Yes = "1"; // Sun or sat
        }

        public struct StringFormat
        {
            public const string ShortDate = "{0:dd/MM/yyyy}";
            public const string ExportFormat = "{0}_{1:dd-MM-yyyy}.{2}";
        }

        public struct ExportType
        {
            public const string PDF = "pdf";
            public const string Word = "docx";
            public const string Excel = "xlsx";
        }

        public struct RegularExpression
        {
            public const string Email = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
        }

        public struct TC_CMDSTS
        {
            public const string TC_CMDSTS_0 = "0";
            public const string TC_CMDSTS_1 = "1";
            public const string TC_CMDSTS_2 = "2";
            public const string TC_CMDSTS_3 = "3";
            public const string TC_CMDSTS_4 = "4";
            public const string TC_CMDSTS_5 = "5";
            public const string TC_CMDSTS_6 = "6";
            public const string TC_CMDSTS_7 = "7";
            public const string TC_CMDSTS_8 = "8";
            public const string TC_CMDSTS_9 = "9";
            public const string TC_CMDSTS_A = "A";
            public const string TC_CMDSTS_B = "B";
            public const string TC_CMDSTS_C = "C";
            public const string TC_CMDSTS_D = "D";
            public const string TC_CMDSTS_E = "E";
            public const string TC_CMDSTS_F = "F";
        }

        public struct MixTime
        {
            public const string MixTime1 = "Mix time1";
            public const string MixTime2 = "Mix time2";
            public const string MixTime3 = "Mix time3";
        }

        public class F44_Status
        {
            /// <summary>
            ///     Not Shipping
            /// </summary>
            public static string F44_Sts_NotShip = "0";

            /// <summary>
            ///     Shipping
            /// </summary>
            public static string F44_Sts_Shipping = "1";

            /// <summary>
            ///     Shipping over.
            /// </summary>
            public static string F44_Sts_ShipOver = "2";
        }

        // F34_CommandType
        //public enum TX34_CmdType
        //{
        //    CmdType = 0000
        //}

        public struct CmdType
        {
            public const string cmdType = "0000";
        }

        public struct TerminalNo
        {
            public const string A001 = "A001";
            public const string A002 = "A002";
            public const string A003 = "A003";
            public const string A004 = "A004";
            public static string A005 = "A005";
            public static string A009 = "A009";
            public const string A014 = "A014";
            public const string A015 = "A015";
            public const string A018 = "A018";
            public const string A017 = "A017";
            public const string A016 = "A016";
            public const string A019 = "A019";

            public const string A020 = "A020";
            public const string A021 = "A021";
            public const string A022 = "A022";
            public const string A023 = "A023";
            public const string A024 = "A024";
            public const string A025 = "A025";
            public const string C004 = "C004";

            /// <summary>
            ///     This terminal represents third communication terminal.
            /// </summary>
            public const string ThirdCommunication = "COM3";
        }

        public struct PictureNo
        {
            public const string TCPS041F = "TCPS041F";
            public const string TCRM051F = "TCRM051F";
            public const string TCRM082F = "TCRM082F";
            public const string TCRM101F = "TCRM101F";
            public const string TCRM091F = "TCRM091F";
            public const string TCRM111F = "TCRM111F";
            public const string TCRM122F = "TCRM122F";


            public const string TCRM071F = "TCRM071F";

            public const string TCRM131F = "TCRM131F";

            public const string TCRM081F = "TCRM081F";

            public const string TCRM151F = "TCRM151F";
            public const string TCRM031F = "TCRM031F";
            public const string TCRM121F = "TCRM121F";
            public const string TCPP022F = "TCPP022F";
            public const string TCIP011F = "TCIP011F";
            public const string TCIP031F = "TCIP031F";
            public const string TCIP042F = "TCIP042F";
            public const string TCIP061F = "TCIP061F";

            public const string TCRM011F = "TCRM011F";
            public const string TCPR051F = "TCPR051F";
            public const string TCPR081F = "TCPR081F";

            public const string TCPR141F = "TCPR141F";

            public const string TCPR091F = "TCPR091F";
            public const string TCPR071F = "TCPR071F";
            public const string TCPR121F = "TCPR121F";
            public const string TCPR131F = "TCPR131F";
            public const string TCPR011F = "TCPR011F";

            public static string TCPR111F = "TCPR111F";

            public static string TCPR041F = "TCPR041F";
            public static string TCIP043F = "TCIP043F";

            public static string TCPR061F = "TCPR061F";

            public static string TCPR101F = "TCPR101F";

            public static string TCIP023F = "TCIP023F";
            public static string TCIP041F = "TCIP041F";

            public static string TCTR011F = "TCTR011F";

            public static string TCIP022F = "TCIP022F";
            public static string TCIP021F = "TCIP021F";
            public static string TCRM123F = "TCRM123F";
        }

        public struct DeviceCode
        {
            public const string ATW001 = "ATW001";

            public const string ProductDevice = "00000";
        }

        public class F42_Status
        {
            public const string TX42_Sts_NotKnd = "0"; // Not Kneaded
            public const string TX42_Sts_Knd = "1"; // Kneading
            public const string TX42_Sts_Cmp = "2"; // Completed
            public const string TX42_Sts_FrcCmp = "3"; // Forced Completed
            public const string TX42_Sts_Stored = "4"; // Stored
            public const string TX42_Sts_Tbtd = "5"; // Tabletised
            public const string TX42_Sts_Kndcmd = "6"; // Knead Command
            public const string TX42_Sts_Tbtcmd = "7"; // Tablet Command
            public const string TX42_Sts_FrcRtr = "8"; // Forced Retrieving
            public const string TX42_Sts_StoRtrd = "9"; // Forced Retrieved
        }

        public class F33_StockFlag
        {
            public const string TX33_StkFlg_NotStk = "0"; //Not in Stock
            public const string TX33_StkFlg_Str = "1"; //Storing
            public const string TX33_StkFlg_Rtr = "2"; // Retrieving
            public const string TX33_StkFlg_Stk = "3"; // Stocked
        }

        public class F49_ShelfStatus
        {
            public const string TX49_StkFlg_NotStk = "0"; // Not in Stock
            public const string TX49_StkFlg_Str = "1"; // Storing
            public const string TX49_StkFlg_Rtr = "2"; // Retrieving
            public const string TX49_StkFlg_Stk = "3"; // Stocked
        }

        public class F40_StockFlag
        {
            public const string TX40_StkFlg_NotStk = "0"; // Not in Stock
            public const string TX40_StkFlg_Str = "1"; // Storing
            public const string TX40_StkFlg_Rtr = "2"; // Retrieving
            public const string TX40_StkFlg_Stk = "3"; // Stocked
            public const string TX40_StkFlg_NotStkShip = "4"; // Not in Stock because of shipping
            public const string TX40_StkFlg_Shipping = "5"; // Shipping
        }

        public class F53_StokcFlag
        {
            public const string TX53_StkFlg_NotStk = "0"; // Not in Stock
            public const string TX53_StkFlg_Str = "1"; // Storing
            public const string TX53_StkFlg_Rtr = "2"; // Retrieving
            public const string TX53_StkFlg_Stk = "3"; // Stocked
        }

        public class F51_ShelfStatus
        {
            /// <summary>
            ///     Empty shelf
            /// </summary>
            public static string TX51_ShfSts_Epy = "0";

            /// <summary>
            ///     Empty pallet
            /// </summary>
            public static string TX51_ShfSts_WhsPlt = "1";

            /// <summary>
            ///     Product stock
            /// </summary>
            public static string TX51_ShfSts_Pdt = "2";

            /// <summary>
            ///     Out of Sign Pre-Product Stock
            /// </summary>
            public static string TX51_ShfSts_BadPrePdt = "3";

            /// <summary>
            ///     Reserved for Storage
            /// </summary>
            public static string TX51_ShfSts_RsvStr = "4";

            /// <summary>
            ///     Reserved for Retrieval
            /// </summary>
            public static string TX51_ShfSts_RsvRtr = "5";

            /// <summary>
            ///     Prohibited
            /// </summary>
            public static string TX51_ShfSts_Pbt = "6";

            /// <summary>
            ///     Physically  Prohibited
            /// </summary>
            public static string TX51_ShfSts_PhyPbt = "7";

            /// <summary>
            ///     External Pre-product
            /// </summary>
            public static string TX51_ShfSts_ExtPrePdt = "8";
        }

        public class F51_StockTakingFlag
        {
            /// <summary>
            ///     Inventory not Checked
            /// </summary>
            public const string TX51_StkTkgFlg_InvNotChk = "0";

            /// <summary>
            ///     Inventory Checked
            /// </summary>
            public const string TX51_StkTkgFlg_InvChk = "1";
        }

        public struct F52_CommandType
        {
            public const string TX52_CmdType_MtrAdd = "04"; // Add New Material
            public const string TX52_CmdType_MtrUpd = "14"; // Update or Delete Material
            public const string TX52_CmdType_PrePdtAdd = "03"; // Add New Material
            public const string TX52_CmdType_PrePdtUpd = "13"; // Update or Delete Preproduct
        }

        public struct F41_Status
        {
            // F41_Status
            public const string NotTablet = "3";
            public const string PreproductRetrieving = "4"; // Pre-product retrieving
            public const string RetrievalingStoped = "5"; // Retrievaling stoped
            public const string RetrievalOver = "6"; // Retrieval Over
            public const string ContainerSetError = "0"; // Container set error
            public const string ContainerSet = "1"; // Container set 
            public const string ContainerSetWait = "2"; // Container set wait
            public const string Tableted = "7"; // Tableted
            public const string Stored = "8"; // Stored
            public const string Default = "9"; //Default
        }

        public struct F56_Status
        {
            public const string NotTablet = "0"; // Not Tablet
            public const string Tableting = "1"; // Tableting
            public const string Change = "2"; // Container Change
            public const string TabletingOver = "3"; // Tableting Over
            public const string StorageOver = "4"; // Storage Over
        }

        public struct CommandType
        {
            public const string CmdType_0 = "0000";
            public const string CmdType_1 = "0001";
            public const string CmdType_2 = "0100";
        }

        /// <summary>
        ///     List of template which is used for exporting data.
        /// </summary>
        public class ExportTemplate
        {
            /// <summary>
            ///     Template file which is used for exporting master list pre-product.
            /// </summary>
            public const string MasterListPreProduct = "App_Data/ExportTemplates/MasterList-Export-PreProduct.html";

            /// <summary>
            ///     Template file which is used for exporting master list product.
            /// </summary>
            public const string MasterListProduct = "App_Data/ExportTemplates/MasterList-Export-Product.html";

            /// <summary>
            ///     Template file which is used for exporting master list material.
            /// </summary>
            public const string MasterListMaterial = "App_Data/ExportTemplates/MasterList-Export-Material.html";

            /// <summary>
            ///     Template file which is used for exporting input kneading command.
            /// </summary>
            public const string InputKneadingCommands = "App_Data/ExportTemplates/Export-InputKneadingCommand.html";

            public const string MaterialRequirementList = "App_Data/ExportTemplates/MaterialRequirementList-Export.html";
            public const string PreProductPlan = "App_Data/ExportTemplates/PreProductPlan.html";


            /// <summary>
            ///     Template file which is used for exporting inquiry by material name.
            /// </summary>
            public const string InquiryByMaterialNameNormal =
                "App_Data/ExportTemplates/Export-InquiryByMaterialName.html";

            public const string ReportByMaterialNameNormal =
                "App_Data/ExportTemplates/Export-ReportByMaterialName.html";

            public const string MaterialMovementHistory = "App_Data/ExportTemplates/Export-MaterialMovementHistory.html";
            public const string ConsumerMaterial = "App_Data/ExportTemplates/Export-ConsumerMaterialList.html";
            public const string ConsumerPreProduct = "App_Data/ExportTemplates/Export-ConsumerPreProductList.html";
            public const string ConsumerProduct = "App_Data/ExportTemplates/Export-ConsumerProductList.html";

            public const string SupplementaryMaterialNameNormal =
                "App_Data/ExportTemplates/Export-SupplementaryMaterialName.html";

            public const string MaterialSheif = "App_Data/ExportTemplates/Export-MaterialSheif.html";
            public const string MaterialPalletList = "App_Data/ExportTemplates/Export-MaterialPalletList.html";

            public const string InquiryByMaterialNameBailment =
                "App_Data/ExportTemplates/Export-InquiryByMaterialNameBailment.html";

            public const string ReportByMaterialNameBailment =
                "App_Data/ExportTemplates/Export-ReportByMaterialNameBailment.html";

            public const string InquiryByMaterialNameAll =
                "App_Data/ExportTemplates/Export-InquiryByMaterialNameAll.html";

            public const string ReportByMaterialNameAll =
                "App_Data/ExportTemplates/Export-ReportByMaterialNameAll.html";

            public const string MaterialMovementRecord = "App_Data/ExportTemplates/Export-MaterialMovementRecord.html";
            public const string MaterialRetrievalRecord = "App_Data/ExportTemplates/Export-MaterialRetrievalRecord.html";

            public const string MaterialRetrievalRecordEmpty =
                "App_Data/ExportTemplates/Export-MaterialRetrievalRecordEmpty.html";


            /// <summary>
            ///     Template file which is used for exporting inquiry by preproduct name.
            /// </summary>
            public const string InquiryByPreProductNameNormal =
                "App_Data/ExportTemplates/Export-InquiryByPreProductName.html";

            public const string InquiryByPreProductNameExternal =
                "App_Data/ExportTemplates/Export-InquiryByPreProductNameExternal.html";

            public const string PreProductMovementHistory =
                "App_Data/ExportTemplates/Export-PreproductMovementHistory.html";

            public const string PreProductShelfList = "App_Data/ExportTemplates/Export-PreProductShelf.html";

            public const string ExternalPreProductShelfList =
                "App_Data/ExportTemplates/Export-ExternalPreProductShelf.html";

            public const string InquiryByPreProductNameAll =
                "App_Data/ExportTemplates/Export-InquiryByPreProductNameAll.html";

            public const string PreProductRetrievalRecord =
                "App_Data/ExportTemplates/Export-PreProductlRetrievalRecord.html";

            public const string PreProductRetrievalRecordEmpty =
                "App_Data/ExportTemplates/Export-PreProductlRetrievalRecordEmpty.html";


            /// <summary>
            ///     Template file which is used for exporting inquiry by Ext preproduct name.
            /// </summary>
            public const string InquiryByExtPreProductNameNormal =
                "App_Data/ExportTemplates/Export-InquiryByExtPreProductName.html";

            public const string InquiryByExtPreProductNameExternal =
                "App_Data/ExportTemplates/Export-InquiryByExtPreProductNameExternal.html";

            public const string InquiryByExtPreProductNameAll =
                "App_Data/ExportTemplates/Export-InquiryByExtPreProductNameAll.html";

            public const string PreProductMovementRecord =
                "App_Data/ExportTemplates/Export-PreProductMovementRecord.html";


            /// <summary>
            ///     Template file which is used for exporting inquiry by Supplier name.
            /// </summary>
            public const string InquiryBySupplierNameNormal =
                "App_Data/ExportTemplates/Export-InquiryBySupplierName.html";

            public const string InquiryBySupplierNameBailment =
                "App_Data/ExportTemplates/Export-InquiryBySupplierNameBailment.html";

            public const string InquiryBySupplierNameAll =
                "App_Data/ExportTemplates/Export-InquiryBySupplierNameAll.html";


            /// <summary>
            ///     Template file which is used for exporting inquiry by Product name.
            /// </summary>
            public const string InquiryByProductNameCertified =
                "App_Data/ExportTemplates/Export-InquiryByProductName.html";

            public const string InquiryByProductNameCertifiedReport =
                "App_Data/ExportTemplates/Export-InquiryByProductNameReport.html";

            /// </summary>
            public const string ReportByProductNameCertified =
                "App_Data/ExportTemplates/Export-ReportByProductName.html";

            public const string ReportByProductNameCertifiedReport =
                "App_Data/ExportTemplates/Export-InquiryByProductNameNotCertifiedReport.html";

            public const string InquiryByProductNameNotCertified =
                "App_Data/ExportTemplates/Export-InquiryByProductNameNotCertified.html";

            public const string ReportByProductNameNotCertified =
                "App_Data/ExportTemplates/Export-ReportByProductNameNotCertified.html";

            public const string InquiryByProductNameAll = "App_Data/ExportTemplates/Export-InquiryByProductNameAll.html";
            public const string ExternalProductShelfList = "App_Data/ExportTemplates/Export-ExternalProductShelf.html";
            public const string PreProductContainerList = "App_Data/ExportTemplates/Export-PreProductContainerList.html";
            public const string ProductPalletList = "App_Data/ExportTemplates/Export-ProductPalletList.html";
            public const string ProductMovementHistory = "App_Data/ExportTemplates/Export-ProductMovementHistory.html";
            public const string ProductMovementRecord = "App_Data/ExportTemplates/Export-ProductMovementRecord.html";
            public const string ProductShippingRecord = "App_Data/ExportTemplates/Export-ProductShippingRecord.html";

            public const string ProductCertificationRecord =
                "App_Data/ExportTemplates/Export-ProductCertificationRecord.html";


            public const string StockCertified = "App_Data/ExportTemplates/Stocklist-Certificated-Item.html";

            public const string StockListOutOfSpec =
                "App_Data/ExportTemplates/Stocklist-Certificated-Item-OutOfSpec.html";

            public const string StockListUncertificated =
                "App_Data/ExportTemplates/Stocklist-Certificated-Item-Uncertificated.html";

            public const string ProductShippingCommand = "App_Data/ExportTemplates/ProductShippingCommand.html";

            public const string CleanlinessDataInput =
                "App_Data/ExportTemplates/ExportCleanlinessDataInput.html";
        }

        public class F14Maxload
        {
            public const int MaxLoad = 50;
        }

        #region Common

        public const string DummyDate = "1980-01-01";
        public const string LastDummyDate = "1980-01-31";
        public const int LastDayOfMonth = 31;

        public enum KndLine
        {
            Conventional = 1,
            Megabit = 0
        }

        public enum StatusEnd
        {
            [Description("Normal End")] NormalEnd = 1,
            [Description("Forced End")] ForcedEnd = 2
        }

        public enum F56_CertificationFlag
        {
            [Description("Non")] Non = 0, // not Certificaiton
            [Description("OK")] OK = 1, // Certificaiton OK
            [Description("NG")] NG = 2, // Certificaiton OK
            [Description("Normal")] Normal = 3, // Normal
            [Description("Out of Spec")] OutofSpec = 4
        }

        public enum StatusStart
        {
            [Description("Normal Start")] NormalStart = 0,
            [Description("Re-Start")] ReStart = 1
        }

        public enum Bailment
        {
            [Description("Norm")] Normal,
            [Description("Bail")] Bailment
        }

        public enum Liquid
        {
            [Description("Pow")] Powder,
            [Description(" Liq")] Liquid
        }

        public enum RetrievalLocation
        {
            Both = 0,
            [Description("3rd Floor Only")] ThreerdFloorOnly,
            [Description("4th Floor Only")] FourthFloorOnly
        }

        public enum F52_MsrMacCls
        {
            Conv = 1,
            Mega = 2,
            Both = 3
        }

        public enum Temperature
        {
            Low = 0,
            Normal = 1
        }

        public enum Choice
        {
            Yes = 1,
            No = 0
        }

        public enum ColorClass
        {
            Black = 0,
            Color = 1
        }

        public enum SimulationType
        {
            Material,
            Command
        }


        public enum InsideLabelClass
        {
            [Description("None")] None = 0,
            [Description("1 Piece")] OnePiece,
            [Description("2 Piece")] TwoPiece
        }

        public enum F39_KneadingLine
        {
            Megabit = 0,
            [Description("Conventional (C)")] ConventionalC = 1,
            [Description("Conventional (B)")] ConventionalB = 2
        }

        public enum ExecutingClassification
        {
            Search = 0,
            Update = 1
        }

        #endregion

        #region Inquiry

        public enum InquirySearchConditionShelfStatus
        {
            [Description("First Row")] FirstRow = 0,
            [Description("Second Row")] SecondRow = 1
        }

        public enum InquirySearchConditionWarehouseLocation
        {
            [Description("Material")] Material = 0,
            [Description("Pre-product")] PreProduct = 1,
            [Description("Product")] Product = 2
        }

        public enum InquiryPrintOptionWarehouseLocation
        {
            [Description("Normal")] Normal = 0,
            [Description("Baliment")] OutOfPlan = 1,
            [Description("All")] Sample = 2
        }

        #endregion

        #region PreProduct

        public enum WeighingMethod
        {
            Manual = 0,
            Automatic = 1
        }

        public enum Additive
        {
            Normal = 0,
            Additive = 1
        }

        public enum GroupName
        {
            NotCommand = 0,
            Tabletised = 1
        }

        public enum ContainerMode
        {
            ContainerEnd,
            LotEnd
        }

        #endregion

        #region PdtPln

        #endregion

        #region Material Shelf Status

        public enum TX31OlDSheflStatus
        {
            [Description("Empty")] TX31_MtrShfSts_Epy = 0,
            [Description("Pallet")] TX31_MtrShfSts_WhsPlt = 1,
            [Description("Suppl")] TX31_MtrShfSts_SplPlt = 2,
            [Description("Mat")] TX31_MtrShfSts_Mtr = 3,
            [Description("Store")] TX31_MtrShfSts_RsvStg = 4,
            [Description("Retr")] TX31_MtrShfSts_RsvRtr = 5,
            [Description("Forbid")] TX31_MtrShfSts_Pbt = 6,
            [Description("Physic")] TX31_MtrShfSts_PhyPbt = 7
        }

        public enum TX31SheflStatusFor022F
        {
            [Description("1. Empty")] TX31_MtrShfSts_Epy = 0,
            [Description("2. Pallet")] TX31_MtrShfSts_WhsPlt = 1,
            [Description("3. Suppl")] TX31_MtrShfSts_SplPlt = 2,
            [Description("4. Mat")] TX31_MtrShfSts_Mtr = 3,
            [Description("5. Store")] TX31_MtrShfSts_RsvStg = 4,
            [Description("6. Retr")] TX31_MtrShfSts_RsvRtr = 5,
            [Description("7. Forbid")] TX31_MtrShfSts_Pbt = 6
        }

        public enum TX31SheflStatus
        {
            [Description("1. Empty")] TX31_MtrShfSts_Epy = 0,
            [Description("2. Pallet")] TX31_MtrShfSts_WhsPlt = 1,
            [Description("3. Suppl")] TX31_MtrShfSts_SplPlt = 2,
            [Description("4. Mat")] TX31_MtrShfSts_Mtr = 3,
            [Description("5. Store")] TX31_MtrShfSts_RsvStg = 4,
            [Description("6. Retr")] TX31_MtrShfSts_RsvRtr = 5,
            [Description("7. Forbid")] TX31_MtrShfSts_Pbt = 6,
            [Description("8. Physic")] TX31_MtrShfSts_PhyPbt = 7
        }

        public enum T31LiquidClass
        {
            [Description("non-liquid")] TX31_LqdCls_NonLqd = 0,
            [Description("Liquid")] TX31_LqdCls_Lqd = 1
        }

        public enum T31StockTakingFlag
        {
            [Description("Inventory not Checked")] TX31_StkTkgFlg_InvNotChk = 0,
            [Description("Inventory Checked")] TX31_StkTkgFlg_InvChk = 1
        }

        public enum TX37OldSheflStatus
        {
            [Description("Empty")] TX37_ShfSts_Epy = 0,
            [Description("Empty C")] TX37_ShfSts_EpyCtn = 1,
            [Description("Stock")] TX37_ShfSts_Stk = 3,
            [Description("Store")] TX37_ShfSts_RsvStg = 4,
            [Description("Retr")] TX37_ShfSts_RsvRtr = 5,
            [Description("Forbid")] TX37_ShfSts_Pbt = 6,
            [Description("Physic")] TX37_ShfSts_PhyPbt = 7
        }

        public enum TX37SheflStatusFor022F
        {
            [Description("1. Empty")] TX37_ShfSts_Epy = 0,
            [Description("2. Empty C")] TX37_ShfSts_EpyCtn = 1,
            [Description("3. Stock")] TX37_ShfSts_Stk = 3,
            [Description("4. Store")] TX37_ShfSts_RsvStg = 4,
            [Description("5. Retr")] TX37_ShfSts_RsvRtr = 5,
            [Description("6. Forbid")] TX37_ShfSts_Pbt = 6
        }

        // TX37_PrePdtShfSts
        // F37_ShelfStatus
        public enum TX37SheflStatus
        {
            [Description("1.Empty")] TX37_ShfSts_Epy = 0,
            [Description("2.Empty C")] TX37_ShfSts_EpyCtn = 1,
            [Description("3.Stock")] TX37_ShfSts_Stk = 3,
            [Description("4.Store")] TX37_ShfSts_RsvStg = 4,
            [Description("5.Retr")] TX37_ShfSts_RsvRtr = 5,
            [Description("6.Forbid")] TX37_ShfSts_Pbt = 6,
            [Description("7.Physic")] TX37_ShfSts_PhyPbt = 7
        }

        //F37_StockTakingFlag

        public enum TX37StockTakingFlag
        {
            [Description("Inventory not Checked")] TX37_StkTkgFlg_InvNotChk = 0,
            [Description("Inventory Checked")] TX37_StkTkgFlg_InvChk = 1
        }

        public enum TX37LowTmpCls
        {
            [Description("Low Temperation")] TX37_LowTmpCls_Low = 0,
            [Description("Normal Temperation")] TX37_LowTmpCls_Nml = 1
        }


        // F49_ShelfStatus
        public enum TX49SheflStatus
        {
            [Description("Not in Stock")] TX49_StkFlg_NotStk = 0,
            [Description("Storing")] TX49_StkFlg_Str = 1,
            [Description("Retrieving")] TX49_StkFlg_Rtr = 2,
            [Description("Stocked")] TX49_StkFlg_Stk = 3
        }

        //TX51SheflStatus
        public enum TX51SheflStatus
        {
            [Description("Empty")] TX51_ShfSts_Epy = 0,
            [Description("Pallet")] TX51_ShfSts_WhsPlt = 1,
            [Description("Product")] TX51_ShfSts_Pdt = 2,
            [Description("Out/Spec")] TX51_ShfSts_BadPrePdt = 3,
            [Description("Store")] TX51_ShfSts_RsvStr = 4,
            [Description("Retr")] TX51_ShfSts_RsvRtr = 5,
            [Description("Forbid")] TX51_ShfSts_Pbt = 6,
            [Description("Physic")] TX51_ShfSts_PhyPbt = 7,
            [Description("External")] TX51_ShfSts_ExtPrePdt = 8 // 
        }

        public enum TX51SheflStatusShelfTypeNormal //ShelfType =0
        {
            [Description("1.Empty")] TX51_ShfSts_Epy = 0,
            [Description("2.Pallet")] TX51_ShfSts_WhsPlt = 1,
            [Description("3.Product")] TX51_ShfSts_Pdt = 2,
            [Description("4.External")] TX51_ShfSts_ExtPrePdt = 8,
            [Description("5.Store")] TX51_ShfSts_RsvStr = 4,
            [Description("6.Retr")] TX51_ShfSts_RsvRtr = 5,
            [Description("7.Forbid")] TX51_ShfSts_Pbt = 6
        }

        public enum TX51SheflStatusShelfTypeBadFor039 //ShelfType =1
        {
            [Description("Empty")] TX51_ShfSts_Epy = 0,
            [Description("Pallet")] TX51_ShfSts_WhsPlt = 1,
            [Description("Out/Spec")] TX51_ShfSts_BadPrePdt = 3,
            [Description("Store")] TX51_ShfSts_RsvStr = 4,
            [Description("Retr")] TX51_ShfSts_RsvRtr = 5,
            [Description("Forbid")] TX51_ShfSts_Pbt = 6
        }

        public enum TX51SheflStatusShelfTypeBad //ShelfType =1
        {
            [Description("1.Empty")] TX51_ShfSts_Epy = 0,
            [Description("2.Pallet")] TX51_ShfSts_WhsPlt = 1,
            [Description("3.Out/Spec")] TX51_ShfSts_BadPrePdt = 3,
            [Description("4.Store")] TX51_ShfSts_RsvStr = 4,
            [Description("5.Retr")] TX51_ShfSts_RsvRtr = 5,
            [Description("6.Forbid")] TX51_ShfSts_Pbt = 6
        }

        //F51_ShelfType
        public enum TX51ShelfType
        {
            [Description("Normal Shelf")] TX51_ShelfType_Nml = 0,
            [Description("Shelf Used for bad  product")] TX51_ShelfType_BadUse = 1
        }

        // F51_StockTakingFlag
        public enum TX51StockTakingFlag
        {
            [Description("Inventory not Checked")] TX51_StkTkgFlg_InvNotChk = 0,
            [Description("Inventory Checked")] TX51_StkTkgFlg_InvChk = 1
        }

        // F51_BadSign
        public enum F51BadSign
        {
            [Description("Normal")] Normal = 0,
            [Description("Baliment")] Baliment = 1,
            [Description("All")] All = 2
        }

        public enum PrintOptionsPreProduct
        {
            [Description("Normal")] Normal = 0,
            [Description("External")] External = 1,
            [Description("All")] All = 2
        }

        public enum PrintOptions
        {
            [Description("Bailment")] Bailment = 1,
            [Description("Normal")] Normal = 0,
            [Description("All")] All = 2
        }

        public enum PrintOptionsProduct
        {
            [Description("Certified")] Certified = 0,
            [Description("Not Certified")] NotCertified = 1,
            [Description("All")] All = 2
        }

        public enum F57_OutFlg
        {
            Plan = 0, // plan
            OutOfPlan = 1 // Out of plan
        }

        public enum F30_AcceptClass
        {
            [Description("Non Accpet")] TX30_AcpCls_NonAcp = 0,
            [Description("Accept")] TX30_AcpCls_Acp = 1,
            [Description("Reject")] TX30_AcpCls_Rjt = 2
        }

        #endregion
    }
}