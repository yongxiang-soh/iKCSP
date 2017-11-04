using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.MaterialManagement
{
   public class StockTakingOfMaterialC1ViewModelItem
   {
        public string ShelfNo { get; set; }

        public string PalletNo { get; set; }

        public string MaterialCode { get; set; }
        public string MaterialDsp { get; set; }


        public string MaterialLotNo01 { get; set; }

        public string MaterialLotNo02 { get; set; }


        public string MaterialLotNo03 { get; set; }

        public string MaterialLotNo04 { get; set; }

        public string MaterialLotNo05 { get; set; }

   
        public double PackUnit01 { get; set; }

        public double PackUnit02 { get; set; }

    
        public double PackUnit03 { get; set; }

        public double PackUnit04 { get; set; }

    
        public double PackUnit05 { get; set; }

 
        public int PackQuantity01 { get; set; }

        public int PackQuantity02 { get; set; }

        public int PackQuantity03 { get; set; }

  
        public int PackQuantity04 { get; set; }

    
        public int PackQuantity05 { get; set; }

        public double Fraction01 { get; set; }

   
        public double Fraction02 { get; set; }


        public double Fraction03 { get; set; }


        public double Fraction04 { get; set; }

      
        public double Fraction05 { get; set; }

   
        public double Total01 { get; set; }

        
        public double Total02 { get; set; }

      
        public double Total03 { get; set; }

      
        public double Total04 { get; set; }

       
        public double Total05 { get; set; }

       
        public double GrandTotal { get; set; }

       
        public string UnitFlag { get; set; }
    }
}
