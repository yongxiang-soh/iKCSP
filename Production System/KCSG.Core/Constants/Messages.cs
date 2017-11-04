using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Helper;

namespace KCSG.Core.Constants
{
    public static partial class Constants
    {
        public struct Messages
        {
            public const string Material_MSG001 = "Item does not exist";
            public const string Material_MSG002 = "Data invalid";
            public const string Material_MSG003 = "Do you want to delete the selected item?";
            public const string Material_MSG004 = "Item must be unique";
            public const string Material_MSG025 = "Fraction cannot be more than the packing unit!";
            public const string Material_MSG026 = "Value is out of range!";
            public const string Material_MSG39 = "Requested Retrieval Quantity must be greater than 0!";
            public const string Material_MSG40 = "The retrieval quantity cannot be more than the possible retrieval quantity!";
            public const string Material_MSG03 = "The same record exists in database.";

            #region PreProduct_ScreenTCPP021F

            public const string PreProduct_TCPP022F01 =
                "There is the same Pre-product Code name, please input another!!";

            public const string PreProduct_TCPP022F02 = "The Pre-product Code field must be inputted!!";
            public const string PreProduct_TCPP022F03 = "Yieldrate must be inputted!!";
            public const string PreProduct_TCPP022F04 = "Container type must be inputted!!";
            public const string PreProduct_TCPP022F05 = "Batch/Lot must be inputted!!";
            public const string PreProduct_TCPP022F06 = "Are you sure to add this Preproduct?";
            public const string PreProduct_TCPP022F07 = "Are you sure to update this Preproduct?";
            public const string PreProduct_TCPP022F08 = "The length of Message is incorrect (not equal 34)!!";

            public const string PreProduct_TCPP022F09 =
                "The Material is being used, and the record can't be updated or deleted!!!";

            public const string PreProduct_TCPP022F10 = "Incorrect kneading line!!";
            public const string PreProduct_TCPP022F11 = "You must select at least one material!!";

            public const string PreProduct_TCPP023F01 =
                "There is no the kind of Material, please input the correct material code!!";

            public const string PreProduct_TCPP023F02 = "There is the same material, please input another!!";
            public const string PreProduct_TCPP023F03 = "Material code must be inputted!!";
            public const string PreProduct_TCPP023F04 = "Charged Order must be inputted!!";

            #endregion

            #region PdtPln_ScreenTCPP041F

            #endregion

            #region Storage of Material TCRM031F

            public const string StoreMaterial_MSG01 = "You can’t leave this field blank.";
            public const string StoreMaterial_MSG07 = "P. O. No. or Partial Delivery or material code is not corresponding!";
            public const string StoreMaterial_MSG08 = "Material lot-no cannot be empty!";
            public const string StoreMaterial_MSG09 = "Pack quantity must be more than zero!";
            public const string StoreMaterial_MSG10 = "This pallet is being used!";
            public const string StoreMaterial_MSG11 = "The grand total must be less than expected quantity!";
            public const string StoreMaterial_MSG12 = "At least one line must be valid, please input data!";
            public const string StoreMaterial_MSG13 = "This lot-no is been used!";
            public const string StoreMaterial_MSG14 = "Fraction cannot be more than the packing unit!";
            public const string StoreMaterial_MSG15 = "Conveyor status error!";
            public const string StoreMaterial_MSG16 = "Warehouse status error!";
            public const string StoreMaterial_MSG17 = "Liquid Class error!";
            public const string StoreMaterial_MSG18 = "There is no empty location available in the warehouse now!";

            #endregion

            #region Inter-floor Movement of Material
            public const string MovementOfMaterial_MSG01 = "The corresponding conveyor does not exist!";
            public const string MovementOfMaterial_MSG02 = "Conveyor status error!";
            public const string MovementOfMaterial_MSG03 = "Warehouse status error!";
            #endregion

            #region 
            public const string Conveyor_MSG01 = "Conveyor status error!";
            public const string Device_MSG01 = "Warehouse status error!";
            public const string StorageOfWarehouse_MSG03 = "There is no empty location available in the warehouse now!";
            #endregion

            #region System management

            public const string SystemManagement_MSG6 = "Unused information of pre-product warehouse deleted!";
            public const string SystemManagement_MSG1 = "You can’t leave this field blank.";
            public const string SystemManagement_MSG22 = "Ready to add?"; 
            public const string SystemManagement_MSG28 = "Ready to update?";
            #endregion

            #region Tabletising

            public const string Tabletising_MSG32 =
                "The charging container number is more than retrieved container amount";
            #endregion
            #region Out Of Plan's Product
            public const string OutOfPlanProduct_MSG41 = "The input product code and pre-product lot-no is duplicated by other product!";
            #endregion

            #region ProductSippingPlan_ScreenTCPR032F
            public const string ProductShipping_MSG21 = "The same record exists in database!";
            #endregion

            #region Product Management

            public const string ProductManagement_MSG40 =
                "The retrieval quantity cannot be more than the possible retrieval quantity!";
            public const string ProductManagement_MSG14 =
                "Pack quantity must be more than zero!";

            #endregion

            #region Inquiry
            public const string SystemManagement_TCSS000041 = "Cannot find details with this shelf no !";
            public const string SystemManagement_TCFC021F03 = @"Ready to change status to Empty ?";
            public const string SystemManagement_TCFC021F04 = @"Ready to change status to Empty Pallet ?";
            #endregion
        }

       
    }
}
