namespace KCSG.Domain.Models.ProductManagement
{
    public class AssignProductShippingCommandResult
    {
        public bool ib_instk { get; set; }

        public bool ib_outstk { get; set; }

        public string ls_shelfno { get; set; }

        public string ls_palletno { get; set; }

        public double lc_assign { get; set; }
        public double packageAmount { get; set; }

        public string row { get; set; }

        public string bay { get; set; }

        public string level { get; set; }
    }
}