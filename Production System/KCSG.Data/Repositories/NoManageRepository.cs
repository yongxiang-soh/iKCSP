using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Data.DatabaseContext;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;

namespace KCSG.Data.Repositories
{
    public class NoManageRepository : RepositoryBase<TX48_NoManage>
    {
        public NoManageRepository(IKCSGDbContext context)
            : base(context)
        {
        }
        public int CreateOrUpdateTX48(ref bool isNoManage,Constants.GetColumnInNoManager getValue, int mtrWhsCmdNo = 0, int prePdtWhsCmdNo=0, 
            int kndCmdBookNo=0, int outKndCmdNo=0, int pdtWhsCmdNo=0)
        {
           
            var noManage = GetMany(i => i.F48_SystemId.Trim().Equals("00000")).FirstOrDefault();// Get(n => n.F48_SystemId.Equals("00000"));
            if (noManage != null)
            {
                if (Constants.GetColumnInNoManager.Pdtwhscmdno.Equals(getValue))
                {
                    noManage.F48_PdtWhsCmdNo += 1;
                    if (noManage.F48_PdtWhsCmdNo > 9999)
                        noManage.F48_PdtWhsCmdNo = 1;
                }
                else if (getValue.Equals(Constants.GetColumnInNoManager.MtrWhsCmdNo))
                {
                    noManage.F48_MtrWhsCmdNo += 1;
                    if (noManage.F48_MtrWhsCmdNo > 9999)
                        noManage.F48_MtrWhsCmdNo = 1;
                }
                else if (getValue.Equals(Constants.GetColumnInNoManager.PrePdtWhsCmdNo))
                {
                    noManage.F48_PrePdtWhsCmdNo += 1;
                    if (noManage.F48_PrePdtWhsCmdNo > 9999)
                        noManage.F48_PrePdtWhsCmdNo = 1;
                }
                else if (getValue.Equals(Constants.GetColumnInNoManager.KndCmdBookNo))
                {
                    noManage.F48_KndCmdBookNo += 1;
                    if (noManage.F48_KndCmdBookNo > 9999)
                        noManage.F48_KndCmdBookNo = 1;
                }
                else
                {
                    noManage.F48_OutKndCmdNo += 1;
                    if (noManage.F48_OutKndCmdNo > 9999)
                        noManage.F48_OutKndCmdNo = 1;
                }

                ////	Increase [f48_mtrwhscmdno] by 1.
                //noManage.F48_MtrWhsCmdNo += 1;
                ////	If [f48_mtrwhscmdno] is greater than 9999, the system will:
                //if (noManage.F48_MtrWhsCmdNo > 9999)
                //{
                //    //	Update [f48_mtrwhscmdno] as 1.
                //    noManage.F48_MtrWhsCmdNo = 1;
                //}
                ////	Update [f48_updatedate] as current date and time.
                noManage.F48_UpdateDate = DateTime.Now;
                Update(noManage);
                isNoManage = true;
            }
            else
            {
               using (var a = new KCSGConnectionString())
            {
                noManage = new TX48_NoManage();
                noManage.F48_SystemId = "00000";
                noManage.F48_MegaKndCmdNo = 0;
                noManage.F48_GnrKndCmdNo = 0;
                noManage.F48_MtrWhsCmdNo = mtrWhsCmdNo;
                noManage.F48_PrePdtWhsCmdNo = 0;
                noManage.F48_PrePdtWhsCmdNo = prePdtWhsCmdNo;
                noManage.F48_PdtWhsCmdNo = pdtWhsCmdNo;
                noManage.F48_KndCmdBookNo = kndCmdBookNo;
                noManage.F48_AddDate = DateTime.Now;
                noManage.F48_UpdateDate = DateTime.Now;
                noManage.F48_KneadSheefNo = 0;
                noManage.F48_OutKndCmdNo = outKndCmdNo;
                noManage.F48_CnrKndCmdNo = 0;
                a.TX48_NoManage.Add(noManage);
                a.SaveChanges();
               }
                //insert tx48
                
                isNoManage = false;
            }
            switch (getValue)
            {
                case Constants.GetColumnInNoManager.Pdtwhscmdno:
                    return noManage.F48_PdtWhsCmdNo;
                    
                case Constants.GetColumnInNoManager.MtrWhsCmdNo:
                    return noManage.F48_MtrWhsCmdNo;
                    
                case Constants.GetColumnInNoManager. PrePdtWhsCmdNo:
                    return noManage.F48_PrePdtWhsCmdNo;
                case Constants.GetColumnInNoManager.KndCmdBookNo:
                    return noManage.F48_KndCmdBookNo;
                    
                default:
                    return noManage.F48_OutKndCmdNo;
            }
            
        }

        public void InsertNomanage(int mtrWhsCmdNo , int prePdtWhsCmdNo, int kndCmdBookNo, int outKndCmdNo, int pdtWhsCmdNo)
        {
            //insert tx48
            var noManage = new TX48_NoManage();
            noManage.F48_SystemId = "00000";
            noManage.F48_KndCmdBookNo = 0;
            noManage.F48_MegaKndCmdNo = 0;
            noManage.F48_GnrKndCmdNo = 0;
            noManage.F48_MtrWhsCmdNo = mtrWhsCmdNo;
            noManage.F48_PrePdtWhsCmdNo = prePdtWhsCmdNo;
            noManage.F48_PdtWhsCmdNo = pdtWhsCmdNo;
            noManage.F48_KndCmdBookNo = kndCmdBookNo;
            noManage.F48_AddDate = DateTime.Now;
            noManage.F48_UpdateDate = DateTime.Now;
            noManage.F48_KneadSheefNo = 0;
            noManage.F48_OutKndCmdNo = outKndCmdNo;
            noManage.F48_CnrKndCmdNo = 0;
            Add(noManage);
        }

        public void  UpdateNomanage(TX48_NoManage nomanage)
        {
            if (nomanage != null)
            {
                nomanage.F48_OutKndCmdNo = nomanage.F48_OutKndCmdNo > 999 ? 1 : nomanage.F48_OutKndCmdNo + 1;
                nomanage.F48_UpdateDate = DateTime.Now;

                Update(nomanage);
            }

        }

        public bool GetkndNo(string ach_lineno, ref string as_kndcmdno, ref int ai_kndcmdbookno)
        {

            int ll_kndcmdno, ll_kndcmdbookno;
            int ll_no1, ll_no2, ll_no3, ll_bookno, ll_cnt;
            string ls_sysid;

            ls_sysid = "00000";
            var tx48 = GetById(ls_sysid);
            if (tx48 == null)
            {
                switch (ach_lineno)
                {
                    case "0":

                        ll_no1 = 1;
                        ll_no2 = 0;
                        ll_no3 = 0;
                        break;
                    case "1":
                        ll_no1 = 0;
                        ll_no2 = 1;
                        ll_no3 = 0;
                        break;
                    default:
                        ll_no1 = 0;
                        ll_no2 = 0;
                        ll_no3 = 1;
                        break;
                }

                tx48 = new TX48_NoManage()
                {
                    F48_SystemId = "00000",
                    F48_KndCmdBookNo = 0,
                    F48_MegaKndCmdNo = ll_no1,
                    F48_GnrKndCmdNo = ll_no2,
                    F48_OutKndCmdNo = ll_no3,
                    F48_KneadSheefNo = 0,
                    F48_CnrKndCmdNo = 0,
                    F48_MtrWhsCmdNo = 0,
                    F48_PrePdtWhsCmdNo = 0,
                    F48_PdtWhsCmdNo = 0,
                    F48_AddDate = DateTime.Now,
                    F48_UpdateDate = DateTime.Now,
                };
                Add(tx48);
                as_kndcmdno = "001";
                ai_kndcmdbookno = 1;
                return true;
            }
            else
            {


                switch (ach_lineno)
                {
                    case "0":

                        ll_kndcmdno = tx48.F48_MegaKndCmdNo;
                        ll_kndcmdbookno = tx48.F48_KndCmdBookNo;
                        break;
                    case "1":

                        ll_kndcmdno = tx48.F48_GnrKndCmdNo;
                        ll_kndcmdbookno = tx48.F48_KndCmdBookNo;
                        break;
                    default:

                        ll_kndcmdno = tx48.F48_OutKndCmdNo;
                        ll_kndcmdbookno = tx48.F48_KndCmdBookNo;
                        break;
                }

                ll_kndcmdno = ll_kndcmdno + 1;
                if (ll_kndcmdno > 999)
                {
                    ll_kndcmdno = 1;
                }
                if (ai_kndcmdbookno < 0)
                {
                    ll_bookno = ll_kndcmdbookno + 1;
                }
                else
                {
                    ll_bookno = ll_kndcmdbookno;
                }

                switch (ach_lineno)
                {
                    case "0":
                        tx48.F48_MegaKndCmdNo = ll_kndcmdno;
                        tx48.F48_KndCmdBookNo = ll_bookno;
                        break;
                    case "1":

                        tx48.F48_GnrKndCmdNo = ll_kndcmdno;
                        tx48.F48_KndCmdBookNo = ll_bookno;
                        break;
                    default:

                        tx48.F48_OutKndCmdNo = ll_kndcmdno;
                        tx48.F48_KndCmdBookNo = ll_bookno;
                        break;
                }
                Update(tx48);
                as_kndcmdno = ll_kndcmdno.ToString("D3");
                ai_kndcmdbookno = ll_kndcmdbookno;
                return true;

            }
        }
    }
}
