
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 07/19/2016 11:25:59
-- Generated from EDMX file: D:\KCSG\Production System\KCSG.Data\DataModel\KCSGDataModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [KCSG.Database];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[sysdiagrams]', 'U') IS NOT NULL
    DROP TABLE [dbo].[sysdiagrams];
GO
IF OBJECT_ID(N'[dbo].[TM01_Material]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TM01_Material];
GO
IF OBJECT_ID(N'[dbo].[TM02_PrePdtMkp]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TM02_PrePdtMkp];
GO
IF OBJECT_ID(N'[dbo].[TM03_PreProduct]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TM03_PreProduct];
GO
IF OBJECT_ID(N'[dbo].[TM09_Product]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TM09_Product];
GO
IF OBJECT_ID(N'[dbo].[TM11_PckMtr]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TM11_PckMtr];
GO
IF OBJECT_ID(N'[dbo].[TM15_SubMaterial]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TM15_SubMaterial];
GO
IF OBJECT_ID(N'[dbo].[TX39_PdtPln]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TX39_PdtPln];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[Te80_Env_Mesp]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[Te80_Env_Mesp];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[Te81_Env_Temp]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[Te81_Env_Temp];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[Te82_Env_Aval]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[Te82_Env_Aval];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[Te83_Env_Else]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[Te83_Env_Else];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[Te84_Env_Lot]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[Te84_Env_Lot];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[Te85_Env_Prod]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[Te85_Env_Prod];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TH60_MtrWhsCmdHst]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TH60_MtrWhsCmdHst];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TH61_MtrStgRtrHst]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TH61_MtrStgRtrHst];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TH62_KndCmdMsrSndHst]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TH62_KndCmdMsrSndHst];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TH63_PrePdtWhsCmdHst]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TH63_PrePdtWhsCmdHst];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TH64_PrePdtStgRtrHst]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TH64_PrePdtStgRtrHst];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TH65_PdtStgRtrHst]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TH65_PdtStgRtrHst];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TH66_PdtWhsCmdHst]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TH66_PdtWhsCmdHst];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TH67_CrfHst]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TH67_CrfHst];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TH68_MtrMsrSndCmdHst]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TH68_MtrMsrSndCmdHst];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TH69_MtrRtrMsrSndCmdHst]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TH69_MtrRtrMsrSndCmdHst];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TH70_PdtShipHst]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TH70_PdtShipHst];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TM04_Supplier]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TM04_Supplier];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TM05_Conveyor]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TM05_Conveyor];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TM06_Terminal]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TM06_Terminal];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TM07_Calendar]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TM07_Calendar];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TM08_Container]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TM08_Container];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TM10_EndUsr]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TM10_EndUsr];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TM12_TrmPicMgn]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TM12_TrmPicMgn];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TM13_Function]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TM13_Function];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TM14_Device]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TM14_Device];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TM16_Password]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TM16_Password];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TM17_TermStatus]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TM17_TermStatus];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TM18_Access]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TM18_Access];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TX30_Reception]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TX30_Reception];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TX31_MtrShfSts]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TX31_MtrShfSts];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TX32_MtrShf]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TX32_MtrShf];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TX33_MtrShfStk]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TX33_MtrShfStk];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TX34_MtrWhsCmd]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TX34_MtrWhsCmd];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TX37_PrePdtShfSts]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TX37_PrePdtShfSts];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TX40_PdtShfStk]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TX40_PdtShfStk];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TX41_TbtCmd]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TX41_TbtCmd];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TX42_KndCmd]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TX42_KndCmd];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TX43_KndRcd]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TX43_KndRcd];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TX44_ShippingPlan]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TX44_ShippingPlan];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TX45_ShipCommand]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TX45_ShipCommand];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TX46_SupMtrStk]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TX46_SupMtrStk];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TX47_PdtWhsCmd]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TX47_PdtWhsCmd];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TX48_NoManage]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TX48_NoManage];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TX49_PrePdtShfStk]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TX49_PrePdtShfStk];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TX50_PrePdtWhsCmd]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TX50_PrePdtWhsCmd];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TX51_PdtShfSts]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TX51_PdtShfSts];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TX52_MtrMsrSndCmd]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TX52_MtrMsrSndCmd];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TX53_OutSidePrePdtStk]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TX53_OutSidePrePdtStk];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TX54_MtrRtrMsrSndCmd]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TX54_MtrRtrMsrSndCmd];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TX55_KndCmdMsrSnd]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TX55_KndCmdMsrSnd];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TX56_TbtPdt]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TX56_TbtPdt];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TX57_PdtShf]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TX57_PdtShf];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TX58_OutPlanPdt]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TX58_OutPlanPdt];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TX91_MaterialTotal]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TX91_MaterialTotal];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TX92_PrepdtTotal]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TX92_PrepdtTotal];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TX93_ProductTotal]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TX93_ProductTotal];
GO
IF OBJECT_ID(N'[KCSGDataModelStoreContainer].[TX94_Prepdtplan]', 'U') IS NOT NULL
    DROP TABLE [KCSGDataModelStoreContainer].[TX94_Prepdtplan];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'sysdiagrams'
CREATE TABLE [dbo].[sysdiagrams] (
    [name] nvarchar(128)  NOT NULL,
    [principal_id] int  NOT NULL,
    [diagram_id] int IDENTITY(1,1) NOT NULL,
    [version] int  NULL,
    [definition] varbinary(max)  NULL
);
GO

-- Creating table 'TM01_Material'
CREATE TABLE [dbo].[TM01_Material] (
    [F01_MaterialCode] char(12)  NOT NULL,
    [F01_SupplierCode] char(5)  NULL,
    [F01_MaterialDsp] char(16)  NULL,
    [F01_EMP] float  NULL,
    [F01_ModifyClass] char(1)  NULL,
    [F01_Point] char(3)  NULL,
    [F01_Unit] char(1)  NOT NULL,
    [F01_EntrustedClass] char(1)  NOT NULL,
    [F01_PackingUnit] float  NOT NULL,
    [F01_Price] float  NULL,
    [F01_Department] char(4)  NULL,
    [F01_LiquidClass] char(1)  NOT NULL,
    [F01_FactoryClass] char(1)  NULL,
    [F01_RtrPosCls] char(1)  NOT NULL,
    [F01_MsrMacSndFlg] char(1)  NOT NULL,
    [F01_State] char(1)  NULL,
    [F01_Color] char(1)  NULL,
    [F01_AddDate] datetime  NOT NULL,
    [F01_UpdateDate] datetime  NOT NULL,
    [F01_UpdateCount] int  NULL
);
GO

-- Creating table 'TM02_PrePdtMkp'
CREATE TABLE [dbo].[TM02_PrePdtMkp] (
    [F02_PreProductCode] char(12)  NOT NULL,
    [F02_MaterialCode] char(12)  NOT NULL,
    [F02_ThrawSeqNo] char(2)  NOT NULL,
    [F02_3FLayinAmount] float  NOT NULL,
    [F02_4FLayinAmount] float  NOT NULL,
    [F02_LayinPriority] int  NOT NULL,
    [F02_WeighingMethod] int  NOT NULL,
    [F02_MilingFlag1] int  NOT NULL,
    [F02_MilingFlag2] int  NOT NULL,
    [F02_LoadPosition] char(4)  NULL,
    [F02_PotSeqNo] char(1)  NULL,
    [F02_MsrSeqNo] char(1)  NULL,
    [F02_Addtive] char(1)  NULL,
    [F02_AddDate] datetime  NOT NULL,
    [F02_UpdateDate] datetime  NOT NULL,
    [F02_UpdateCount] int  NULL
);
GO

-- Creating table 'TM03_PreProduct'
CREATE TABLE [dbo].[TM03_PreProduct] (
    [F03_PreProductCode] char(12)  NOT NULL,
    [F03_PreProductName] char(15)  NULL,
    [F03_BatchLot] int  NOT NULL,
    [F03_KneadingLine] char(1)  NOT NULL,
    [F03_YieldRate] float  NOT NULL,
    [F03_AllMtrAmtPerBth] float  NOT NULL,
    [F03_Point] char(3)  NULL,
    [F03_MixDate1] datetime  NULL,
    [F03_MixDate2] datetime  NULL,
    [F03_MixDate3] datetime  NULL,
    [F03_TmpRetTime] datetime  NULL,
    [F03_ContainerType] char(2)  NOT NULL,
    [F03_LowTmpClass] char(1)  NOT NULL,
    [F03_ColorClass] char(1)  NOT NULL,
    [F03_LotNoEnd] int  NOT NULL,
    [F03_MixMode] char(1)  NULL,
    [F03_AddDate] datetime  NOT NULL,
    [F03_UpdateDate] datetime  NOT NULL,
    [F03_UpdateCount] int  NULL
);
GO

-- Creating table 'TM09_Product'
CREATE TABLE [dbo].[TM09_Product] (
    [F09_ProductCode] char(12)  NOT NULL,
    [F09_ProductDesp] char(15)  NULL,
    [F09_TabletType] char(15)  NULL,
    [F09_TabletSize] char(4)  NULL,
    [F09_TabletSize2] char(5)  NULL,
    [F09_NeedTime] int  NOT NULL,
    [F09_TabletAmount] float  NOT NULL,
    [F09_KneadingTime] int  NOT NULL,
    [F09_EndUserCode] char(5)  NULL,
    [F09_YieldRate] float  NOT NULL,
    [F09_Factor] char(3)  NULL,
    [F09_StdStkMtn] float  NOT NULL,
    [F09_PackingUnit] float  NOT NULL,
    [F09_PreProductCode] char(12)  NOT NULL,
    [F09_Unit] char(1)  NOT NULL,
    [F09_ValidPeriod] char(2)  NULL,
    [F09_InsideLabelClass] char(1)  NULL,
    [F09_LowTmpCls] char(1)  NOT NULL,
    [F09_AddDate] datetime  NOT NULL,
    [F09_UpdateDate] datetime  NOT NULL,
    [F09_UpdateCount] int  NULL,
    [F09_Label] char(20)  NULL
);
GO

-- Creating table 'TM11_PckMtr'
CREATE TABLE [dbo].[TM11_PckMtr] (
    [F11_ProductCode] char(12)  NOT NULL,
    [F11_SubMaterialCode] char(12)  NOT NULL,
    [F11_Amount] float  NOT NULL,
    [F11_Unit] char(1)  NOT NULL,
    [F11_AddDate] datetime  NOT NULL,
    [F11_UpdateDate] datetime  NOT NULL,
    [F11_UpdateCount] int  NULL
);
GO

-- Creating table 'TM15_SubMaterial'
CREATE TABLE [dbo].[TM15_SubMaterial] (
    [F15_SubMaterialCode] char(12)  NOT NULL,
    [F15_SupplierCode] char(5)  NULL,
    [F15_MaterialDsp] char(16)  NULL,
    [F15_EMP] float  NULL,
    [F15_ModifyClass] char(1)  NULL,
    [F15_Point] char(3)  NULL,
    [F15_Unit] char(1)  NOT NULL,
    [F15_EntrustedClass] char(1)  NOT NULL,
    [F15_PackingUnit] float  NOT NULL,
    [F15_Price] float  NULL,
    [F15_Department] char(4)  NULL,
    [F15_State] char(1)  NULL,
    [F15_Color] char(1)  NULL,
    [F15_AddDate] datetime  NOT NULL,
    [F15_UpdateDate] datetime  NOT NULL,
    [F15_UpdateCount] int  NULL
);
GO

-- Creating table 'TX39_PdtPln'
CREATE TABLE [dbo].[TX39_PdtPln] (
    [F39_PreProductCode] char(12)  NOT NULL,
    [F39_KndEptBgnDate] datetime  NOT NULL,
    [F39_KneadingLine] char(1)  NOT NULL,
    [F39_KndCmdNo] char(6)  NOT NULL,
    [F39_PrePdtLotAmt] int  NOT NULL,
    [F39_Status] char(1)  NOT NULL,
    [F39_ColorClass] char(1)  NULL,
    [F39_StartLotNo] char(10)  NOT NULL,
    [F39_EndLotAmont] int  NOT NULL,
    [F39_AddDate] datetime  NOT NULL,
    [F39_UpdateDate] datetime  NOT NULL,
    [F39_UpdateCount] int  NULL
);
GO

-- Creating table 'Te80_Env_Mesp'
CREATE TABLE [dbo].[Te80_Env_Mesp] (
    [F80_Type] char(1)  NOT NULL,
    [F80_Id] int  NOT NULL,
    [F80_Name] char(40)  NOT NULL,
    [F80_Humidity] char(1)  NULL,
    [F80_D_From] datetime  NULL,
    [F80_D_To] datetime  NULL,
    [F80_T_Usl] float  NULL,
    [F80_T_Lsl] float  NULL,
    [F80_T_Ucl] float  NULL,
    [F80_T_Lcl] float  NULL,
    [F80_T_Mean] float  NULL,
    [F80_T_Sigma] float  NULL,
    [F80_T_Cp] float  NULL,
    [F80_T_Cpk] float  NULL,
    [F80_T_Range] float  NULL,
    [F80_H_Usl] float  NULL,
    [F80_H_Lsl] float  NULL,
    [F80_H_Ucl] float  NULL,
    [F80_H_Lcl] float  NULL,
    [F80_H_Mean] float  NULL,
    [F80_H_Sigma] float  NULL,
    [F80_H_Cp] float  NULL,
    [F80_H_Cpk] float  NULL,
    [F80_H_Range] float  NULL,
    [F80_T_Dis_Up] float  NULL,
    [F80_T_Dis_Lo] float  NULL,
    [F80_T_Scale] float  NULL,
    [F80_T_Cal_Up] float  NULL,
    [F80_T_Cal_Lo] float  NULL,
    [F80_H_Dis_Up] float  NULL,
    [F80_H_Dis_Lo] float  NULL,
    [F80_H_Scale] float  NULL,
    [F80_H_Cal_Up] float  NULL,
    [F80_H_Cal_Lo] float  NULL
);
GO

-- Creating table 'Te81_Env_Temp'
CREATE TABLE [dbo].[Te81_Env_Temp] (
    [F81_Env_Time] datetime  NOT NULL,
    [F81_Id] int  NOT NULL,
    [F81_Temp] float  NULL,
    [F81_Humidity] float  NULL
);
GO

-- Creating table 'Te82_Env_Aval'
CREATE TABLE [dbo].[Te82_Env_Aval] (
    [F82_Env_Time] datetime  NOT NULL,
    [F82_Id] int  NOT NULL,
    [F82_Status] char(1)  NULL
);
GO

-- Creating table 'Te83_Env_Else'
CREATE TABLE [dbo].[Te83_Env_Else] (
    [F83_Env_Time] datetime  NOT NULL,
    [F83_Id] int  NOT NULL,
    [F83_Value] float  NULL
);
GO

-- Creating table 'Te84_Env_Lot'
CREATE TABLE [dbo].[Te84_Env_Lot] (
    [F84_ProductCode] char(12)  NOT NULL,
    [F84_S_Time] datetime  NOT NULL,
    [F84_ProductLotNo] char(10)  NOT NULL,
    [F84_Temp] float  NULL
);
GO

-- Creating table 'Te85_Env_Prod'
CREATE TABLE [dbo].[Te85_Env_Prod] (
    [F85_Code] char(12)  NOT NULL,
    [F85_Type] char(1)  NULL,
    [F85_Id] int  NULL,
    [F85_From] datetime  NULL,
    [F85_To] datetime  NULL,
    [F85_M_Usl] float  NULL,
    [F85_M_Lsl] float  NULL,
    [F85_M_Ucl] float  NULL,
    [F85_M_Lcl] float  NULL,
    [F85_R_Usl] float  NULL,
    [F85_R_Lsl] float  NULL,
    [F85_T_High] float  NULL,
    [F85_T_Low] float  NULL,
    [F85_T_Ucl] float  NULL,
    [F85_T_Lcl] float  NULL,
    [F85_T_Cpk] float  NULL,
    [F85_T_Cp] float  NULL,
    [F85_T_Range] float  NULL,
    [F85_T_Mean] float  NULL,
    [F85_T_Sigma] float  NULL,
    [F85_R_High] float  NULL,
    [F85_R_Low] float  NULL,
    [F85_R_Ucl] float  NULL,
    [F85_R_Lcl] float  NULL,
    [F85_R_Cpk] float  NULL,
    [F85_R_Cp] float  NULL,
    [F85_R_Range] float  NULL,
    [F85_R_Mean] float  NULL,
    [F85_R_Sigma] float  NULL,
    [F85_No_Lot] int  NULL
);
GO

-- Creating table 'TH60_MtrWhsCmdHst'
CREATE TABLE [dbo].[TH60_MtrWhsCmdHst] (
    [F60_CommandNo] char(4)  NOT NULL,
    [F60_CmdSeqNo] char(4)  NOT NULL,
    [F60_CommandType] char(4)  NOT NULL,
    [F60_StrRtrType] char(1)  NOT NULL,
    [F60_Status] char(1)  NOT NULL,
    [F60_Priority] int  NOT NULL,
    [F60_PalletNo] char(3)  NULL,
    [F60_From] char(6)  NOT NULL,
    [F60_To] char(6)  NOT NULL,
    [F60_CommandSendDate] datetime  NULL,
    [F60_CommandEndDate] datetime  NULL,
    [F60_TerminalNo] char(4)  NOT NULL,
    [F60_PictureNo] char(8)  NOT NULL,
    [F60_AbnormalCode] char(4)  NULL,
    [F60_RetryCount] int  NULL,
    [F60_AddDate] datetime  NOT NULL,
    [F60_UpdateDate] datetime  NOT NULL,
    [F60_UpdateCount] int  NULL
);
GO

-- Creating table 'TH61_MtrStgRtrHst'
CREATE TABLE [dbo].[TH61_MtrStgRtrHst] (
    [F61_MaterialCode] char(12)  NOT NULL,
    [F61_MaterialLotNo] char(16)  NOT NULL,
    [F61_PalletNo] char(3)  NOT NULL,
    [F61_StgRtrDate] datetime  NOT NULL,
    [F61_PrcPdtNo] char(7)  NULL,
    [F61_PrtDvrNo] char(2)  NULL,
    [F61_From] char(6)  NOT NULL,
    [F61_To] char(6)  NOT NULL,
    [F61_StgRtrCls] char(1)  NOT NULL,
    [F61_TerminalNo] char(4)  NOT NULL,
    [F61_Amount] float  NOT NULL,
    [F61_AddDate] datetime  NOT NULL,
    [F61_UpdateDate] datetime  NOT NULL
);
GO

-- Creating table 'TH62_KndCmdMsrSndHst'
CREATE TABLE [dbo].[TH62_KndCmdMsrSndHst] (
    [F62_KndCmdNo] char(6)  NOT NULL,
    [F62_PrePdtLotNo] char(10)  NOT NULL,
    [F62_PrePdtCode] char(12)  NOT NULL,
    [F62_Status] char(1)  NOT NULL,
    [F62_Priority] char(2)  NOT NULL,
    [F62_MsrSndCls] char(1)  NOT NULL,
    [F62_TerminalNo] char(4)  NOT NULL,
    [F62_PictureNo] char(8)  NOT NULL,
    [F62_AbnormalCode] char(2)  NOT NULL
);
GO

-- Creating table 'TH63_PrePdtWhsCmdHst'
CREATE TABLE [dbo].[TH63_PrePdtWhsCmdHst] (
    [F63_CommandNo] char(4)  NOT NULL,
    [F63_CmdSeqNo] char(4)  NOT NULL,
    [F63_CommandType] char(4)  NOT NULL,
    [F63_StrRtrType] char(1)  NOT NULL,
    [F63_Status] char(1)  NOT NULL,
    [F63_ContainerNo] char(3)  NULL,
    [F63_ContainerCode] char(12)  NOT NULL,
    [F63_Priority] int  NOT NULL,
    [F63_From] char(6)  NOT NULL,
    [F63_To] char(6)  NOT NULL,
    [F63_CommandSendDate] datetime  NULL,
    [F63_CommandEndDate] datetime  NULL,
    [F63_TerminalNo] char(4)  NOT NULL,
    [F63_PictureNo] char(8)  NOT NULL,
    [F63_AbnormalCode] char(4)  NULL,
    [F63_RetryCount] int  NULL,
    [F63_AddDate] datetime  NOT NULL,
    [F63_UpdateDate] datetime  NOT NULL,
    [F63_UpdateCount] int  NULL
);
GO

-- Creating table 'TH64_PrePdtStgRtrHst'
CREATE TABLE [dbo].[TH64_PrePdtStgRtrHst] (
    [F64_PreProductCode] char(12)  NOT NULL,
    [F64_PreProductLotNo] char(10)  NOT NULL,
    [F64_ContainerCode] char(12)  NOT NULL,
    [F64_StgRtrDate] datetime  NOT NULL,
    [F64_ContainerNo] char(3)  NULL,
    [F64_ContainerSeqNo] int  NOT NULL,
    [F64_From] char(6)  NOT NULL,
    [F64_To] char(6)  NOT NULL,
    [F64_StgRtrCls] char(1)  NOT NULL,
    [F64_TerminalNo] char(4)  NOT NULL,
    [F64_Amount] float  NOT NULL,
    [F64_AddDate] datetime  NOT NULL,
    [F64_UpdateDate] datetime  NOT NULL
);
GO

-- Creating table 'TH65_PdtStgRtrHst'
CREATE TABLE [dbo].[TH65_PdtStgRtrHst] (
    [F65_ProductCode] char(12)  NOT NULL,
    [F65_PrePdtLotNo] char(10)  NOT NULL,
    [F65_PalletNo] char(3)  NOT NULL,
    [F65_StgRtrDate] datetime  NOT NULL,
    [F65_ProductLotNo] char(10)  NOT NULL,
    [F65_From] char(6)  NOT NULL,
    [F65_To] char(6)  NOT NULL,
    [F65_StgRtrCls] char(1)  NOT NULL,
    [F65_TerminalNo] char(4)  NOT NULL,
    [F65_Amount] float  NOT NULL,
    [F65_AddDate] datetime  NOT NULL,
    [F65_UpdateDate] datetime  NOT NULL
);
GO

-- Creating table 'TH66_PdtWhsCmdHst'
CREATE TABLE [dbo].[TH66_PdtWhsCmdHst] (
    [F66_CommandNo] char(4)  NOT NULL,
    [F66_CmdSeqNo] char(4)  NOT NULL,
    [F66_CommandType] char(4)  NOT NULL,
    [F66_StrRtrType] char(1)  NOT NULL,
    [F66_Status] char(1)  NOT NULL,
    [F66_Priority] int  NOT NULL,
    [F66_PalletNo] char(3)  NULL,
    [F66_From] char(6)  NOT NULL,
    [F66_To] char(6)  NOT NULL,
    [F66_CommandSendDate] datetime  NULL,
    [F66_CommandEndDate] datetime  NULL,
    [F66_TerminalNo] char(4)  NOT NULL,
    [F66_PictureNo] char(8)  NOT NULL,
    [F66_AbnormalCode] char(4)  NULL,
    [F66_RetryCount] int  NULL,
    [F66_AddDate] datetime  NOT NULL,
    [F66_UpdateDate] datetime  NOT NULL,
    [F66_UpdateCount] int  NULL
);
GO

-- Creating table 'TH67_CrfHst'
CREATE TABLE [dbo].[TH67_CrfHst] (
    [F67_ProductCode] char(12)  NOT NULL,
    [F67_PrePdtLotNo] char(10)  NOT NULL,
    [F67_ProductFlg] char(1)  NOT NULL,
    [F67_Amount] float  NOT NULL,
    [F67_ProductLotNo] char(10)  NOT NULL,
    [F67_CertificationFlag] char(1)  NOT NULL,
    [F67_CertificationDate] datetime  NOT NULL,
    [F67_AddDate] datetime  NOT NULL,
    [F67_UpdateDate] datetime  NOT NULL,
    [F67_UpdateCount] int  NULL
);
GO

-- Creating table 'TH68_MtrMsrSndCmdHst'
CREATE TABLE [dbo].[TH68_MtrMsrSndCmdHst] (
    [F68_TerminalNo] char(4)  NOT NULL,
    [F68_AddDate] datetime  NOT NULL,
    [F68_MsrMacCls] char(1)  NOT NULL,
    [F68_CommandType] char(2)  NOT NULL,
    [F68_Status] char(1)  NOT NULL,
    [F68_Priority] char(1)  NOT NULL,
    [F68_MasterCode] char(12)  NOT NULL,
    [F68_PictureNo] char(8)  NOT NULL,
    [F68_AbnormalCode] char(2)  NULL
);
GO

-- Creating table 'TH69_MtrRtrMsrSndCmdHst'
CREATE TABLE [dbo].[TH69_MtrRtrMsrSndCmdHst] (
    [F69_MtrRtrDate] datetime  NOT NULL,
    [F69_PalletNo] char(3)  NOT NULL,
    [F69_MaterialCode] char(12)  NOT NULL,
    [F69_MsrMacClass] char(1)  NOT NULL,
    [F69_TerminalNo] char(4)  NOT NULL,
    [F69_PictureNo] char(8)  NOT NULL,
    [F69_Status] char(1)  NOT NULL,
    [F69_Priority] char(2)  NOT NULL,
    [F69_MtrLotNo1] char(16)  NULL,
    [F69_MtrLotNo2] char(16)  NULL,
    [F69_MtrLotNo3] char(16)  NULL,
    [F69_MtrLotNo4] char(16)  NULL,
    [F69_MtrLotNo5] char(16)  NULL,
    [F69_Amount1] float  NULL,
    [F69_Amount2] float  NULL,
    [F69_Amount3] float  NULL,
    [F69_Amount4] float  NULL,
    [F69_Amount5] float  NULL,
    [F69_AbnormalCode] char(2)  NULL
);
GO

-- Creating table 'TH70_PdtShipHst'
CREATE TABLE [dbo].[TH70_PdtShipHst] (
    [F70_ShipCommandNo] char(8)  NOT NULL,
    [F70_PalletNo] char(3)  NOT NULL,
    [F70_PrePdtLotNo] char(10)  NOT NULL,
    [F70_ProductCode] char(12)  NOT NULL,
    [F70_ShelfNo] char(6)  NULL,
    [F70_ProductLotNo] char(10)  NOT NULL,
    [F70_ShippedAmount] float  NOT NULL,
    [F70_AddDate] datetime  NOT NULL,
    [F70_UpdateDate] datetime  NOT NULL,
    [F70_UpdateCount] int  NULL
);
GO

-- Creating table 'TM04_Supplier'
CREATE TABLE [dbo].[TM04_Supplier] (
    [F04_SupplierCode] char(5)  NOT NULL,
    [F04_SupplierName] char(50)  NULL,
    [F04_Address1] char(80)  NULL,
    [F04_Address2] char(80)  NULL,
    [F04_Address3] char(80)  NULL,
    [F04_Tel1] char(20)  NULL,
    [F04_Tel2] char(20)  NULL,
    [F04_Tel3] char(20)  NULL,
    [F04_Fax1] char(20)  NULL,
    [F04_Fax2] char(20)  NULL,
    [F04_Fax3] char(20)  NULL,
    [F04_MaxLoadAmount] int  NOT NULL,
    [F04_AddDate] datetime  NOT NULL,
    [F04_UpdateDate] datetime  NOT NULL,
    [F04_UpdateCount] int  NULL
);
GO

-- Creating table 'TM05_Conveyor'
CREATE TABLE [dbo].[TM05_Conveyor] (
    [F05_ConveyorCode] char(6)  NOT NULL,
    [F05_TerminalNo] char(4)  NULL,
    [F05_MaxBuffer] int  NOT NULL,
    [F05_StrRtrSts] char(1)  NOT NULL,
    [F05_BufferUsing] int  NOT NULL,
    [F05_UsingTerm] char(4)  NULL,
    [F05_LineNo] char(1)  NULL,
    [F05_ColorClass] char(1)  NULL,
    [F05_AddDate] datetime  NOT NULL,
    [F05_UpdateDate] datetime  NOT NULL,
    [F05_UpdateCount] int  NULL
);
GO

-- Creating table 'TM06_Terminal'
CREATE TABLE [dbo].[TM06_Terminal] (
    [F06_TerminalNo] char(4)  NOT NULL,
    [F06_TerminalName] char(20)  NULL,
    [F06_TermPos] char(1)  NOT NULL,
    [F06_TabletLine] char(6)  NULL,
    [F06_ContinueFlag] char(1)  NOT NULL,
    [F06_OtherFlag] char(1)  NOT NULL,
    [F06_IPAddress1] int  NOT NULL,
    [F06_IPAddress2] int  NOT NULL,
    [F06_IPAddress3] int  NOT NULL,
    [F06_IPAddress4] int  NOT NULL,
    [F06_AddDate] datetime  NOT NULL,
    [F06_UpdateDate] datetime  NOT NULL,
    [F06_UpdateCount] int  NULL
);
GO

-- Creating table 'TM07_Calendar'
CREATE TABLE [dbo].[TM07_Calendar] (
    [F07_Date] datetime  NOT NULL,
    [F07_HolidayFlag] char(1)  NOT NULL,
    [F07_SunSatDayFlag] char(1)  NOT NULL,
    [F07_Description] char(20)  NULL,
    [F07_AddDate] datetime  NOT NULL,
    [F07_UpdateDate] datetime  NOT NULL,
    [F07_UpdateCount] int  NULL
);
GO

-- Creating table 'TM08_Container'
CREATE TABLE [dbo].[TM08_Container] (
    [F08_ContainerType] char(2)  NOT NULL,
    [F08_ContainerName] char(30)  NULL,
    [F08_AddDate] datetime  NOT NULL,
    [F08_UpdateDate] datetime  NOT NULL,
    [F08_UpdateCount] int  NULL
);
GO

-- Creating table 'TM10_EndUsr'
CREATE TABLE [dbo].[TM10_EndUsr] (
    [F10_EndUserCode] char(5)  NOT NULL,
    [F10_EndUserName] char(50)  NULL,
    [F10_Address1] char(80)  NULL,
    [F10_Address2] char(80)  NULL,
    [F10_Address3] char(80)  NULL,
    [F10_Tel1] char(20)  NULL,
    [F10_Tel2] char(20)  NULL,
    [F10_Tel3] char(20)  NULL,
    [F10_Fax1] char(20)  NULL,
    [F10_Fax2] char(20)  NULL,
    [F10_Fax3] char(20)  NULL,
    [F10_AddDate] datetime  NOT NULL,
    [F10_UpdateDate] datetime  NOT NULL,
    [F10_UpdateCount] int  NULL
);
GO

-- Creating table 'TM12_TrmPicMgn'
CREATE TABLE [dbo].[TM12_TrmPicMgn] (
    [F12_TerminalNo] char(4)  NOT NULL,
    [F12_PictureNo] char(8)  NOT NULL
);
GO

-- Creating table 'TM13_Function'
CREATE TABLE [dbo].[TM13_Function] (
    [F13_PictureNo] char(8)  NOT NULL,
    [F13_PswdLevel] char(1)  NOT NULL,
    [F13_AccessCtrlFlag] char(1)  NOT NULL,
    [F13_UserNumCtrlFlag] char(1)  NOT NULL,
    [F13_MaxUser] int  NOT NULL,
    [F13_UsingCount] int  NOT NULL
);
GO

-- Creating table 'TM14_Device'
CREATE TABLE [dbo].[TM14_Device] (
    [F14_DeviceCode] char(6)  NOT NULL,
    [F14_DeviceName] char(20)  NULL,
    [F14_UsePermitClass] char(1)  NOT NULL,
    [F14_DeviceStatus] char(1)  NULL,
    [F14_IPAddress1] int  NULL,
    [F14_IPAddress2] int  NULL,
    [F14_IPAddress3] int  NULL,
    [F14_IPAddress4] int  NULL,
    [F14_DeviceMode] char(1)  NULL,
    [F14_CSNo] char(1)  NULL,
    [F14_AddDate] datetime  NOT NULL,
    [F14_UpdateDate] datetime  NOT NULL,
    [F14_UpdateCount] int  NULL,
    [F14_CSNumber] int  NULL
);
GO

-- Creating table 'TM16_Password'
CREATE TABLE [dbo].[TM16_Password] (
    [F16_PswdLevel] char(1)  NOT NULL,
    [F16_Password] char(10)  NOT NULL
);
GO

-- Creating table 'TM17_TermStatus'
CREATE TABLE [dbo].[TM17_TermStatus] (
    [F17_TermNo] char(4)  NOT NULL,
    [F17_InUsePictureNo] char(8)  NULL
);
GO

-- Creating table 'TM18_Access'
CREATE TABLE [dbo].[TM18_Access] (
    [F18_LockPicture] char(8)  NOT NULL,
    [F18_LockedPicture] char(8)  NOT NULL
);
GO

-- Creating table 'TX30_Reception'
CREATE TABLE [dbo].[TX30_Reception] (
    [F30_PrcOrdNo] char(7)  NOT NULL,
    [F30_PrtDvrNo] char(2)  NOT NULL,
    [F30_MaterialCode] char(12)  NOT NULL,
    [F30_ExpectAmount] float  NOT NULL,
    [F30_ExpectDate] datetime  NULL,
    [F30_StoragedAmount] float  NOT NULL,
    [F30_AcceptClass] char(1)  NOT NULL,
    [F30_AddDate] datetime  NOT NULL,
    [F30_UpdateDate] datetime  NOT NULL,
    [F30_UpdateCount] int  NULL
);
GO

-- Creating table 'TX31_MtrShfSts'
CREATE TABLE [dbo].[TX31_MtrShfSts] (
    [F31_ShelfRow] char(2)  NOT NULL,
    [F31_ShelfBay] char(2)  NOT NULL,
    [F31_ShelfLevel] char(2)  NOT NULL,
    [F31_ShelfStatus] char(1)  NOT NULL,
    [F31_ShelfType] char(1)  NULL,
    [F31_LiquidClass] char(1)  NOT NULL,
    [F31_CmnShfAgnOrd] int  NULL,
    [F31_LqdShfAgnOrd] int  NULL,
    [F31_StockTakingFlag] char(1)  NOT NULL,
    [F31_TerminalNo] char(4)  NULL,
    [F31_PalletNo] char(3)  NULL,
    [F31_SupplierCode] char(5)  NULL,
    [F31_LoadAmount] int  NULL,
    [F31_Amount] float  NULL,
    [F31_StorageDate] datetime  NULL,
    [F31_RetrievalDate] datetime  NULL,
    [F31_AddDate] datetime  NOT NULL,
    [F31_UpdateDate] datetime  NOT NULL,
    [F31_UpdateCount] int  NULL
);
GO

-- Creating table 'TX32_MtrShf'
CREATE TABLE [dbo].[TX32_MtrShf] (
    [F32_PalletNo] char(3)  NOT NULL,
    [F32_PrcOrdNo] char(7)  NULL,
    [F32_PrtDvrNo] char(2)  NULL,
    [F32_MegaMsrMacSndEndFlg] char(1)  NOT NULL,
    [F32_GnrlMsrMacSndEndFlg] char(1)  NOT NULL,
    [F32_StorageDate] datetime  NULL,
    [F32_ReStorageDate] datetime  NULL,
    [F32_RetrievalDate] datetime  NULL,
    [F32_AddDate] datetime  NOT NULL,
    [F32_UpdateDate] datetime  NOT NULL,
    [F32_UpdateCount] int  NULL
);
GO

-- Creating table 'TX33_MtrShfStk'
CREATE TABLE [dbo].[TX33_MtrShfStk] (
    [F33_PalletNo] char(3)  NOT NULL,
    [F33_MaterialCode] char(12)  NOT NULL,
    [F33_MaterialLotNo] char(16)  NOT NULL,
    [F33_Amount] float  NOT NULL,
    [F33_StockFlag] char(1)  NOT NULL,
    [F33_AddDate] datetime  NOT NULL,
    [F33_UpdateDate] datetime  NOT NULL
);
GO

-- Creating table 'TX34_MtrWhsCmd'
CREATE TABLE [dbo].[TX34_MtrWhsCmd] (
    [F34_CommandNo] char(4)  NOT NULL,
    [F34_CmdSeqNo] char(4)  NOT NULL,
    [F34_CommandType] char(4)  NOT NULL,
    [F34_StrRtrType] char(1)  NOT NULL,
    [F34_Status] char(1)  NOT NULL,
    [F34_Priority] int  NOT NULL,
    [F34_PalletNo] char(3)  NULL,
    [F34_From] char(6)  NOT NULL,
    [F34_To] char(6)  NOT NULL,
    [F34_CommandSendDate] datetime  NULL,
    [F34_CommandEndDate] datetime  NULL,
    [F34_TerminalNo] char(4)  NOT NULL,
    [F34_PictureNo] char(8)  NOT NULL,
    [F34_AbnormalCode] char(4)  NULL,
    [F34_RetryCount] int  NULL,
    [F34_AddDate] datetime  NOT NULL,
    [F34_UpdateDate] datetime  NOT NULL,
    [F34_UpdateCount] int  NULL
);
GO

-- Creating table 'TX37_PrePdtShfSts'
CREATE TABLE [dbo].[TX37_PrePdtShfSts] (
    [F37_ShelfRow] char(2)  NOT NULL,
    [F37_ShelfBay] char(2)  NOT NULL,
    [F37_ShelfLevel] char(2)  NOT NULL,
    [F37_ContainerCode] char(12)  NULL,
    [F37_ContainerNo] char(3)  NULL,
    [F37_ShelfStatus] char(1)  NOT NULL,
    [F37_ShelfType] char(1)  NULL,
    [F37_LowTmpCls] char(1)  NOT NULL,
    [F37_CmnShfAgnOrd] int  NULL,
    [F37_LowTmpShfAgnOrd] int  NULL,
    [F37_TerminalNo] char(4)  NULL,
    [F37_ContainerType] char(2)  NULL,
    [F37_StockTakingFlag] char(1)  NOT NULL,
    [F37_StorageDate] datetime  NULL,
    [F37_RetrievalDate] datetime  NULL,
    [F37_AddDate] datetime  NOT NULL,
    [F37_UpdateDate] datetime  NOT NULL,
    [F37_UpdateCount] int  NULL
);
GO

-- Creating table 'TX40_PdtShfStk'
CREATE TABLE [dbo].[TX40_PdtShfStk] (
    [F40_PalletNo] char(3)  NOT NULL,
    [F40_PrePdtLotNo] char(10)  NOT NULL,
    [F40_ProductCode] char(12)  NOT NULL,
    [F40_ProductLotNo] char(10)  NOT NULL,
    [F40_StockFlag] char(1)  NOT NULL,
    [F40_PackageAmount] int  NOT NULL,
    [F40_Fraction] float  NOT NULL,
    [F40_Amount] float  NOT NULL,
    [F40_TabletingEndDate] datetime  NOT NULL,
    [F40_ShippedAmount] float  NOT NULL,
    [F40_ShipCommandNo] char(8)  NULL,
    [F40_AssignAmount] float  NULL,
    [F40_CertificationFlg] char(1)  NOT NULL,
    [F40_CertificationDate] datetime  NULL,
    [F40_AddDate] datetime  NOT NULL,
    [F40_UpdateDate] datetime  NOT NULL,
    [F40_UpdateCount] int  NULL
);
GO

-- Creating table 'TX41_TbtCmd'
CREATE TABLE [dbo].[TX41_TbtCmd] (
    [F41_KndCmdNo] char(6)  NOT NULL,
    [F41_PrePdtLotNo] char(10)  NOT NULL,
    [F41_TabletLine] char(6)  NULL,
    [F41_PreproductCode] char(12)  NOT NULL,
    [F41_Status] char(1)  NOT NULL,
    [F41_TblCntAmt] int  NOT NULL,
    [F41_RtrEndCntAmt] int  NOT NULL,
    [F41_ChgCntAmt] int  NOT NULL,
    [F41_TbtBgnDate] datetime  NULL,
    [F41_TbtEndDate] datetime  NULL,
    [F41_AddDate] datetime  NOT NULL,
    [F41_UpdateDate] datetime  NOT NULL,
    [F41_UpdateCount] int  NULL
);
GO

-- Creating table 'TX42_KndCmd'
CREATE TABLE [dbo].[TX42_KndCmd] (
    [F42_KndCmdNo] char(6)  NOT NULL,
    [F42_PrePdtLotNo] char(10)  NOT NULL,
    [F42_PreProductCode] char(12)  NOT NULL,
    [F42_KndEptBgnDate] datetime  NOT NULL,
    [F42_OutSideClass] char(1)  NOT NULL,
    [F42_Status] char(1)  NOT NULL,
    [F42_ThrowAmount] float  NOT NULL,
    [F42_NeedAmount] float  NOT NULL,
    [F42_TrwBgnDate] datetime  NULL,
    [F42_TrwEndDate] datetime  NULL,
    [F42_KndBgnDate] datetime  NULL,
    [F42_KndEndDate] datetime  NULL,
    [F42_StgCtnAmt] int  NOT NULL,
    [F42_BatchEndAmount] int  NOT NULL,
    [F42_KndCmdBookNo] int  NOT NULL,
    [F42_LotSeqNo] int  NOT NULL,
    [F42_CommandSeqNo] int  NOT NULL,
    [F42_MtrRtrFlg] char(1)  NOT NULL,
    [F42_AddDate] datetime  NOT NULL,
    [F42_UpdateDate] datetime  NOT NULL,
    [F42_UpdateCount] int  NULL
);
GO

-- Creating table 'TX43_KndRcd'
CREATE TABLE [dbo].[TX43_KndRcd] (
    [F43_KndCmdNo] char(6)  NOT NULL,
    [F43_PrePdtLotNo] char(10)  NOT NULL,
    [F43_BatchSeqNo] int  NOT NULL,
    [F43_MaterialCode] char(12)  NOT NULL,
    [F43_MaterialLotNo] char(16)  NOT NULL,
    [F43_LayinginAmount] float  NOT NULL,
    [F43_MsrMacSndDate] datetime  NULL,
    [F43_BthBgnDate] datetime  NULL,
    [F43_BthEndDate] datetime  NULL,
    [F43_AddDate] datetime  NOT NULL,
    [F43_UpdateDate] datetime  NOT NULL,
    [F43_UpdateCount] int  NULL
);
GO

-- Creating table 'TX44_ShippingPlan'
CREATE TABLE [dbo].[TX44_ShippingPlan] (
    [F44_ShipCommandNo] char(8)  NOT NULL,
    [F44_ProductLotNo] char(10)  NULL,
    [F44_ProductCode] char(12)  NOT NULL,
    [F44_ShpRqtAmt] float  NOT NULL,
    [F44_ShippedAmount] float  NOT NULL,
    [F44_DeliveryDate] datetime  NULL,
    [F44_EndUserCode] char(5)  NULL,
    [F44_Status] char(1)  NOT NULL,
    [F44_AddDate] datetime  NOT NULL,
    [F44_UpdateDate] datetime  NOT NULL,
    [F44_UpdateCount] int  NULL
);
GO

-- Creating table 'TX45_ShipCommand'
CREATE TABLE [dbo].[TX45_ShipCommand] (
    [F45_ShipCommandNo] char(8)  NOT NULL,
    [F45_ShipDate] datetime  NULL,
    [F45_ShipAmount] float  NOT NULL,
    [F45_ShippedAmount] float  NOT NULL,
    [F45_AddDate] datetime  NOT NULL,
    [F45_UpdateDate] datetime  NOT NULL,
    [F45_UpdateCount] int  NULL
);
GO

-- Creating table 'TX46_SupMtrStk'
CREATE TABLE [dbo].[TX46_SupMtrStk] (
    [F46_SubMaterialCode] char(12)  NOT NULL,
    [F46_Comment] char(40)  NULL,
    [F46_StorageDate] datetime  NOT NULL,
    [F46_Amount] float  NOT NULL,
    [F46_AddDate] datetime  NOT NULL,
    [F46_UpdateDate] datetime  NOT NULL,
    [F46_UpdateCount] int  NULL
);
GO

-- Creating table 'TX47_PdtWhsCmd'
CREATE TABLE [dbo].[TX47_PdtWhsCmd] (
    [F47_CommandNo] char(4)  NOT NULL,
    [F47_CmdSeqNo] char(4)  NOT NULL,
    [F47_CommandType] char(4)  NOT NULL,
    [F47_StrRtrType] char(1)  NOT NULL,
    [F47_Status] char(1)  NOT NULL,
    [F47_Priority] int  NOT NULL,
    [F47_PalletNo] char(3)  NULL,
    [F47_From] char(6)  NOT NULL,
    [F47_To] char(6)  NOT NULL,
    [F47_CommandSendDate] datetime  NULL,
    [F47_CommandEndDate] datetime  NULL,
    [F47_TerminalNo] char(4)  NOT NULL,
    [F47_PictureNo] char(8)  NOT NULL,
    [F47_AbnormalCode] char(4)  NULL,
    [F47_RetryCount] int  NULL,
    [F47_AddDate] datetime  NOT NULL,
    [F47_UpdateDate] datetime  NOT NULL,
    [F47_UpdateCount] int  NULL
);
GO

-- Creating table 'TX48_NoManage'
CREATE TABLE [dbo].[TX48_NoManage] (
    [F48_SystemId] char(5)  NOT NULL,
    [F48_KndCmdBookNo] int  NOT NULL,
    [F48_MegaKndCmdNo] int  NOT NULL,
    [F48_GnrKndCmdNo] int  NOT NULL,
    [F48_OutKndCmdNo] int  NOT NULL,
    [F48_KneadSheefNo] int  NOT NULL,
    [F48_CnrKndCmdNo] int  NOT NULL,
    [F48_MtrWhsCmdNo] int  NOT NULL,
    [F48_PrePdtWhsCmdNo] int  NOT NULL,
    [F48_PdtWhsCmdNo] int  NOT NULL,
    [F48_OutPlanPdtCmdNo] int  NOT NULL,
    [F48_AddDate] datetime  NOT NULL,
    [F48_UpdateDate] datetime  NOT NULL,
    [F48_UpdateCount] int  NULL
);
GO

-- Creating table 'TX49_PrePdtShfStk'
CREATE TABLE [dbo].[TX49_PrePdtShfStk] (
    [F49_ContainerCode] char(12)  NOT NULL,
    [F49_KndCmdNo] char(6)  NOT NULL,
    [F49_ContainerNo] char(3)  NULL,
    [F49_PreProductCode] char(12)  NOT NULL,
    [F49_PreProductLotNo] char(10)  NOT NULL,
    [F49_Amount] float  NOT NULL,
    [F49_ShelfStatus] char(1)  NOT NULL,
    [F49_StorageDate] datetime  NULL,
    [F49_RetrievalDate] datetime  NULL,
    [F49_ContainerSeqNo] int  NOT NULL,
    [F49_AddDate] datetime  NOT NULL,
    [F49_UpdateDate] datetime  NOT NULL,
    [F49_UpdateCount] int  NULL
);
GO

-- Creating table 'TX50_PrePdtWhsCmd'
CREATE TABLE [dbo].[TX50_PrePdtWhsCmd] (
    [F50_CommandNo] char(4)  NOT NULL,
    [F50_CmdSeqNo] char(4)  NOT NULL,
    [F50_CommandType] char(4)  NOT NULL,
    [F50_StrRtrType] char(1)  NOT NULL,
    [F50_Status] char(1)  NOT NULL,
    [F50_ContainerNo] char(3)  NULL,
    [F50_ContainerCode] char(12)  NOT NULL,
    [F50_Priority] int  NOT NULL,
    [F50_From] char(6)  NOT NULL,
    [F50_To] char(6)  NOT NULL,
    [F50_CommandSendDate] datetime  NULL,
    [F50_CommandEndDate] datetime  NULL,
    [F50_TerminalNo] char(4)  NOT NULL,
    [F50_PictureNo] char(8)  NOT NULL,
    [F50_AbnormalCode] char(4)  NULL,
    [F50_RetryCount] int  NULL,
    [F50_LotEndFlg] char(1)  NULL,
    [F50_AddDate] datetime  NOT NULL,
    [F50_UpdateDate] datetime  NOT NULL,
    [F50_UpdateCount] int  NULL
);
GO

-- Creating table 'TX51_PdtShfSts'
CREATE TABLE [dbo].[TX51_PdtShfSts] (
    [F51_ShelfRow] char(2)  NOT NULL,
    [F51_ShelfBay] char(2)  NOT NULL,
    [F51_ShelfLevel] char(2)  NOT NULL,
    [F51_ShelfStatus] char(1)  NULL,
    [F51_ShelfType] char(1)  NOT NULL,
    [F51_CmdShfAgnOrd] int  NULL,
    [F51_LowTmpShfAgnOrd] int  NULL,
    [F51_LoadAmount] int  NOT NULL,
    [F51_StockTakingFlag] char(1)  NOT NULL,
    [F51_PalletNo] char(3)  NULL,
    [F51_TerminalNo] char(4)  NULL,
    [F51_StorageDate] datetime  NULL,
    [F51_RetrievalDate] datetime  NULL,
    [F51_AddDate] datetime  NOT NULL,
    [F51_UpdateDate] datetime  NOT NULL,
    [F51_UpdateCount] int  NULL
);
GO

-- Creating table 'TX52_MtrMsrSndCmd'
CREATE TABLE [dbo].[TX52_MtrMsrSndCmd] (
    [F52_TerminalNo] char(4)  NOT NULL,
    [F52_AddDate] datetime  NOT NULL,
    [F52_MsrMacCls] char(1)  NOT NULL,
    [F52_CommandType] char(2)  NOT NULL,
    [F52_Status] char(1)  NOT NULL,
    [F52_Priority] char(1)  NOT NULL,
    [F52_MasterCode] char(12)  NOT NULL,
    [F52_PictureNo] char(8)  NULL,
    [F52_AbnormalCode] char(2)  NULL,
    [F52_UpdateDate] datetime  NOT NULL,
    [F52_UpdateCount] int  NULL
);
GO

-- Creating table 'TX53_OutSidePrePdtStk'
CREATE TABLE [dbo].[TX53_OutSidePrePdtStk] (
    [F53_PalletNo] char(3)  NOT NULL,
    [F53_OutSidePrePdtCode] char(12)  NOT NULL,
    [F53_OutSidePrePdtLotNo] char(10)  NOT NULL,
    [F53_KndCmdNo] char(6)  NOT NULL,
    [F53_PalletSeqNo] int  NOT NULL,
    [F53_Amount] float  NOT NULL,
    [F53_StockFlag] char(1)  NOT NULL,
    [F53_AddDate] datetime  NOT NULL,
    [F53_UpdateDate] datetime  NOT NULL,
    [F53_UpdateCount] int  NULL
);
GO

-- Creating table 'TX54_MtrRtrMsrSndCmd'
CREATE TABLE [dbo].[TX54_MtrRtrMsrSndCmd] (
    [F54_MtrRtrDate] datetime  NOT NULL,
    [F54_PalletNo] char(3)  NOT NULL,
    [F54_MaterialCode] char(12)  NOT NULL,
    [F54_MsrMacClass] char(1)  NOT NULL,
    [F54_TerminalNo] char(4)  NOT NULL,
    [F54_PictureNo] char(8)  NOT NULL,
    [F54_Status] char(1)  NOT NULL,
    [F54_Priority] char(2)  NOT NULL,
    [F54_MtrLotNo1] char(16)  NULL,
    [F54_MtrLotNo2] char(16)  NULL,
    [F54_MtrLotNo3] char(16)  NULL,
    [F54_MtrLotNo4] char(16)  NULL,
    [F54_MtrLotNo5] char(16)  NULL,
    [F54_Amount1] float  NULL,
    [F54_Amount2] float  NULL,
    [F54_Amount3] float  NULL,
    [F54_Amount4] float  NULL,
    [F54_Amount5] float  NULL,
    [F54_AbnormalCode] char(2)  NULL,
    [F54_AddDate] datetime  NOT NULL,
    [F54_UpdateDate] datetime  NOT NULL,
    [F54_UpdateCount] int  NULL
);
GO

-- Creating table 'TX55_KndCmdMsrSnd'
CREATE TABLE [dbo].[TX55_KndCmdMsrSnd] (
    [F55_KndCmdNo] char(6)  NOT NULL,
    [F55_PrePdtLotNo] char(10)  NOT NULL,
    [F55_PrePdtCode] char(12)  NOT NULL,
    [F55_Status] char(1)  NOT NULL,
    [F55_Priority] char(2)  NOT NULL,
    [F55_MsrSndCls] char(1)  NOT NULL,
    [F55_TerminalNo] char(4)  NOT NULL,
    [F55_PictureNo] char(8)  NOT NULL,
    [F55_AbnormalCode] char(2)  NOT NULL,
    [F55_AddDate] datetime  NOT NULL,
    [F55_UpdateDate] datetime  NOT NULL,
    [F55_UpdateCount] int  NULL
);
GO

-- Creating table 'TX56_TbtPdt'
CREATE TABLE [dbo].[TX56_TbtPdt] (
    [F56_KndCmdNo] char(6)  NOT NULL,
    [F56_PrePdtLotNo] char(10)  NOT NULL,
    [F56_ProductCode] char(12)  NOT NULL,
    [F56_ProductLotNo] char(10)  NOT NULL,
    [F56_Status] char(1)  NOT NULL,
    [F56_TbtCmdAmt] float  NOT NULL,
    [F56_TbtCmdEndPackAmt] float  NOT NULL,
    [F56_TbtCmdEndFrtAmt] float  NOT NULL,
    [F56_TbtCmdEndAmt] float  NOT NULL,
    [F56_StorageAmt] float  NOT NULL,
    [F56_TbtBgnDate] datetime  NULL,
    [F56_TbtEndDate] datetime  NULL,
    [F56_CertificationFlag] char(1)  NOT NULL,
    [F56_CertificationDate] datetime  NULL,
    [F56_ShipDate] datetime  NULL,
    [F56_AddDate] datetime  NOT NULL,
    [F56_UpdateDate] datetime  NOT NULL,
    [F56_UpdateCount] int  NULL
);
GO

-- Creating table 'TX57_PdtShf'
CREATE TABLE [dbo].[TX57_PdtShf] (
    [F57_PalletNo] char(3)  NOT NULL,
    [F57_StorageDate] datetime  NULL,
    [F57_ReStorageDate] datetime  NULL,
    [F57_RetievalDate] datetime  NULL,
    [F57_OutFlg] char(1)  NOT NULL,
    [F57_AddDate] datetime  NOT NULL,
    [F57_UpdateDate] datetime  NOT NULL,
    [F57_UpdateCount] int  NULL
);
GO

-- Creating table 'TX58_OutPlanPdt'
CREATE TABLE [dbo].[TX58_OutPlanPdt] (
    [F58_PrePdtLotNo] char(10)  NOT NULL,
    [F58_ProductCode] char(12)  NOT NULL,
    [F58_PdtSeqNo] char(6)  NOT NULL,
    [F58_ProductLotNo] char(10)  NOT NULL,
    [F58_Status] char(1)  NOT NULL,
    [F58_TbtCmdEndPackAmt] float  NOT NULL,
    [F58_TbtCmdEndFrtAmt] float  NOT NULL,
    [F58_TbtCmdEndAmt] float  NOT NULL,
    [F58_StorageAmt] float  NOT NULL,
    [F58_TbtEndDate] datetime  NULL,
    [F58_CertificationFlag] char(1)  NOT NULL,
    [F58_CertificationDate] datetime  NULL,
    [F58_ShipDate] datetime  NULL,
    [F58_AddDate] datetime  NOT NULL,
    [F58_UpdateDate] datetime  NOT NULL,
    [F58_UpdateCount] int  NULL
);
GO

-- Creating table 'TX91_MaterialTotal'
CREATE TABLE [dbo].[TX91_MaterialTotal] (
    [F91_YearMonth] datetime  NOT NULL,
    [F91_MaterialCode] char(12)  NOT NULL,
    [F91_PrevRemainder] float  NOT NULL,
    [F91_Received] float  NOT NULL,
    [F91_Used] float  NOT NULL,
    [F91_AddDate] datetime  NULL,
    [F91_UpdateDate] datetime  NULL
);
GO

-- Creating table 'TX92_PrepdtTotal'
CREATE TABLE [dbo].[TX92_PrepdtTotal] (
    [F92_YearMonth] datetime  NOT NULL,
    [F92_PrepdtCode] char(12)  NOT NULL,
    [F92_PrevRemainder] float  NOT NULL,
    [F92_Received] float  NOT NULL,
    [F92_Used] float  NOT NULL,
    [F92_AddDate] datetime  NULL,
    [F92_UpdateDate] datetime  NULL
);
GO

-- Creating table 'TX93_ProductTotal'
CREATE TABLE [dbo].[TX93_ProductTotal] (
    [F93_YearMonth] datetime  NOT NULL,
    [F93_ProductCode] char(12)  NOT NULL,
    [F93_PrevRemainder] float  NOT NULL,
    [F93_Received] float  NOT NULL,
    [F93_Used] float  NOT NULL,
    [F93_AddDate] datetime  NULL,
    [F93_UpdateDate] datetime  NULL
);
GO

-- Creating table 'TX94_Prepdtplan'
CREATE TABLE [dbo].[TX94_Prepdtplan] (
    [F94_YearMonth] datetime  NOT NULL,
    [F94_PrepdtCode] char(12)  NOT NULL,
    [F94_amount] float  NOT NULL,
    [F94_AddDate] datetime  NULL,
    [F94_UpdateDate] datetime  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [diagram_id] in table 'sysdiagrams'
ALTER TABLE [dbo].[sysdiagrams]
ADD CONSTRAINT [PK_sysdiagrams]
    PRIMARY KEY CLUSTERED ([diagram_id] ASC);
GO

-- Creating primary key on [F01_MaterialCode] in table 'TM01_Material'
ALTER TABLE [dbo].[TM01_Material]
ADD CONSTRAINT [PK_TM01_Material]
    PRIMARY KEY CLUSTERED ([F01_MaterialCode] ASC);
GO

-- Creating primary key on [F02_PreProductCode], [F02_MaterialCode], [F02_ThrawSeqNo] in table 'TM02_PrePdtMkp'
ALTER TABLE [dbo].[TM02_PrePdtMkp]
ADD CONSTRAINT [PK_TM02_PrePdtMkp]
    PRIMARY KEY CLUSTERED ([F02_PreProductCode], [F02_MaterialCode], [F02_ThrawSeqNo] ASC);
GO

-- Creating primary key on [F03_PreProductCode] in table 'TM03_PreProduct'
ALTER TABLE [dbo].[TM03_PreProduct]
ADD CONSTRAINT [PK_TM03_PreProduct]
    PRIMARY KEY CLUSTERED ([F03_PreProductCode] ASC);
GO

-- Creating primary key on [F09_ProductCode] in table 'TM09_Product'
ALTER TABLE [dbo].[TM09_Product]
ADD CONSTRAINT [PK_TM09_Product]
    PRIMARY KEY CLUSTERED ([F09_ProductCode] ASC);
GO

-- Creating primary key on [F11_ProductCode], [F11_SubMaterialCode] in table 'TM11_PckMtr'
ALTER TABLE [dbo].[TM11_PckMtr]
ADD CONSTRAINT [PK_TM11_PckMtr]
    PRIMARY KEY CLUSTERED ([F11_ProductCode], [F11_SubMaterialCode] ASC);
GO

-- Creating primary key on [F15_SubMaterialCode] in table 'TM15_SubMaterial'
ALTER TABLE [dbo].[TM15_SubMaterial]
ADD CONSTRAINT [PK_TM15_SubMaterial]
    PRIMARY KEY CLUSTERED ([F15_SubMaterialCode] ASC);
GO

-- Creating primary key on [F39_PreProductCode], [F39_KndEptBgnDate] in table 'TX39_PdtPln'
ALTER TABLE [dbo].[TX39_PdtPln]
ADD CONSTRAINT [PK_TX39_PdtPln]
    PRIMARY KEY CLUSTERED ([F39_PreProductCode], [F39_KndEptBgnDate] ASC);
GO

-- Creating primary key on [F80_Type], [F80_Id], [F80_Name] in table 'Te80_Env_Mesp'
ALTER TABLE [dbo].[Te80_Env_Mesp]
ADD CONSTRAINT [PK_Te80_Env_Mesp]
    PRIMARY KEY CLUSTERED ([F80_Type], [F80_Id], [F80_Name] ASC);
GO

-- Creating primary key on [F81_Env_Time], [F81_Id] in table 'Te81_Env_Temp'
ALTER TABLE [dbo].[Te81_Env_Temp]
ADD CONSTRAINT [PK_Te81_Env_Temp]
    PRIMARY KEY CLUSTERED ([F81_Env_Time], [F81_Id] ASC);
GO

-- Creating primary key on [F82_Env_Time], [F82_Id] in table 'Te82_Env_Aval'
ALTER TABLE [dbo].[Te82_Env_Aval]
ADD CONSTRAINT [PK_Te82_Env_Aval]
    PRIMARY KEY CLUSTERED ([F82_Env_Time], [F82_Id] ASC);
GO

-- Creating primary key on [F83_Env_Time], [F83_Id] in table 'Te83_Env_Else'
ALTER TABLE [dbo].[Te83_Env_Else]
ADD CONSTRAINT [PK_Te83_Env_Else]
    PRIMARY KEY CLUSTERED ([F83_Env_Time], [F83_Id] ASC);
GO

-- Creating primary key on [F84_ProductCode], [F84_S_Time], [F84_ProductLotNo] in table 'Te84_Env_Lot'
ALTER TABLE [dbo].[Te84_Env_Lot]
ADD CONSTRAINT [PK_Te84_Env_Lot]
    PRIMARY KEY CLUSTERED ([F84_ProductCode], [F84_S_Time], [F84_ProductLotNo] ASC);
GO

-- Creating primary key on [F85_Code] in table 'Te85_Env_Prod'
ALTER TABLE [dbo].[Te85_Env_Prod]
ADD CONSTRAINT [PK_Te85_Env_Prod]
    PRIMARY KEY CLUSTERED ([F85_Code] ASC);
GO

-- Creating primary key on [F60_CommandNo], [F60_CmdSeqNo], [F60_CommandType], [F60_StrRtrType], [F60_Status], [F60_Priority], [F60_From], [F60_To], [F60_TerminalNo], [F60_PictureNo], [F60_AddDate], [F60_UpdateDate] in table 'TH60_MtrWhsCmdHst'
ALTER TABLE [dbo].[TH60_MtrWhsCmdHst]
ADD CONSTRAINT [PK_TH60_MtrWhsCmdHst]
    PRIMARY KEY CLUSTERED ([F60_CommandNo], [F60_CmdSeqNo], [F60_CommandType], [F60_StrRtrType], [F60_Status], [F60_Priority], [F60_From], [F60_To], [F60_TerminalNo], [F60_PictureNo], [F60_AddDate], [F60_UpdateDate] ASC);
GO

-- Creating primary key on [F61_MaterialCode], [F61_MaterialLotNo], [F61_PalletNo], [F61_StgRtrDate], [F61_From], [F61_To], [F61_StgRtrCls], [F61_TerminalNo], [F61_Amount], [F61_AddDate], [F61_UpdateDate] in table 'TH61_MtrStgRtrHst'
ALTER TABLE [dbo].[TH61_MtrStgRtrHst]
ADD CONSTRAINT [PK_TH61_MtrStgRtrHst]
    PRIMARY KEY CLUSTERED ([F61_MaterialCode], [F61_MaterialLotNo], [F61_PalletNo], [F61_StgRtrDate], [F61_From], [F61_To], [F61_StgRtrCls], [F61_TerminalNo], [F61_Amount], [F61_AddDate], [F61_UpdateDate] ASC);
GO

-- Creating primary key on [F62_KndCmdNo], [F62_PrePdtLotNo], [F62_PrePdtCode], [F62_Status], [F62_Priority], [F62_MsrSndCls], [F62_TerminalNo], [F62_PictureNo], [F62_AbnormalCode] in table 'TH62_KndCmdMsrSndHst'
ALTER TABLE [dbo].[TH62_KndCmdMsrSndHst]
ADD CONSTRAINT [PK_TH62_KndCmdMsrSndHst]
    PRIMARY KEY CLUSTERED ([F62_KndCmdNo], [F62_PrePdtLotNo], [F62_PrePdtCode], [F62_Status], [F62_Priority], [F62_MsrSndCls], [F62_TerminalNo], [F62_PictureNo], [F62_AbnormalCode] ASC);
GO

-- Creating primary key on [F63_CommandNo], [F63_CmdSeqNo], [F63_CommandType], [F63_StrRtrType], [F63_Status], [F63_ContainerCode], [F63_Priority], [F63_From], [F63_To], [F63_TerminalNo], [F63_PictureNo], [F63_AddDate], [F63_UpdateDate] in table 'TH63_PrePdtWhsCmdHst'
ALTER TABLE [dbo].[TH63_PrePdtWhsCmdHst]
ADD CONSTRAINT [PK_TH63_PrePdtWhsCmdHst]
    PRIMARY KEY CLUSTERED ([F63_CommandNo], [F63_CmdSeqNo], [F63_CommandType], [F63_StrRtrType], [F63_Status], [F63_ContainerCode], [F63_Priority], [F63_From], [F63_To], [F63_TerminalNo], [F63_PictureNo], [F63_AddDate], [F63_UpdateDate] ASC);
GO

-- Creating primary key on [F64_PreProductCode], [F64_PreProductLotNo], [F64_ContainerCode], [F64_StgRtrDate], [F64_ContainerSeqNo], [F64_From], [F64_To], [F64_StgRtrCls], [F64_TerminalNo], [F64_Amount], [F64_AddDate], [F64_UpdateDate] in table 'TH64_PrePdtStgRtrHst'
ALTER TABLE [dbo].[TH64_PrePdtStgRtrHst]
ADD CONSTRAINT [PK_TH64_PrePdtStgRtrHst]
    PRIMARY KEY CLUSTERED ([F64_PreProductCode], [F64_PreProductLotNo], [F64_ContainerCode], [F64_StgRtrDate], [F64_ContainerSeqNo], [F64_From], [F64_To], [F64_StgRtrCls], [F64_TerminalNo], [F64_Amount], [F64_AddDate], [F64_UpdateDate] ASC);
GO

-- Creating primary key on [F65_ProductCode], [F65_PrePdtLotNo], [F65_PalletNo], [F65_StgRtrDate], [F65_ProductLotNo], [F65_From], [F65_To], [F65_StgRtrCls], [F65_TerminalNo], [F65_Amount], [F65_AddDate], [F65_UpdateDate] in table 'TH65_PdtStgRtrHst'
ALTER TABLE [dbo].[TH65_PdtStgRtrHst]
ADD CONSTRAINT [PK_TH65_PdtStgRtrHst]
    PRIMARY KEY CLUSTERED ([F65_ProductCode], [F65_PrePdtLotNo], [F65_PalletNo], [F65_StgRtrDate], [F65_ProductLotNo], [F65_From], [F65_To], [F65_StgRtrCls], [F65_TerminalNo], [F65_Amount], [F65_AddDate], [F65_UpdateDate] ASC);
GO

-- Creating primary key on [F66_CommandNo], [F66_CmdSeqNo], [F66_CommandType], [F66_StrRtrType], [F66_Status], [F66_Priority], [F66_From], [F66_To], [F66_TerminalNo], [F66_PictureNo], [F66_AddDate], [F66_UpdateDate] in table 'TH66_PdtWhsCmdHst'
ALTER TABLE [dbo].[TH66_PdtWhsCmdHst]
ADD CONSTRAINT [PK_TH66_PdtWhsCmdHst]
    PRIMARY KEY CLUSTERED ([F66_CommandNo], [F66_CmdSeqNo], [F66_CommandType], [F66_StrRtrType], [F66_Status], [F66_Priority], [F66_From], [F66_To], [F66_TerminalNo], [F66_PictureNo], [F66_AddDate], [F66_UpdateDate] ASC);
GO

-- Creating primary key on [F67_ProductCode], [F67_PrePdtLotNo], [F67_ProductFlg], [F67_Amount], [F67_ProductLotNo], [F67_CertificationFlag], [F67_CertificationDate], [F67_AddDate], [F67_UpdateDate] in table 'TH67_CrfHst'
ALTER TABLE [dbo].[TH67_CrfHst]
ADD CONSTRAINT [PK_TH67_CrfHst]
    PRIMARY KEY CLUSTERED ([F67_ProductCode], [F67_PrePdtLotNo], [F67_ProductFlg], [F67_Amount], [F67_ProductLotNo], [F67_CertificationFlag], [F67_CertificationDate], [F67_AddDate], [F67_UpdateDate] ASC);
GO

-- Creating primary key on [F68_TerminalNo], [F68_AddDate], [F68_MsrMacCls], [F68_CommandType], [F68_Status], [F68_Priority], [F68_MasterCode], [F68_PictureNo] in table 'TH68_MtrMsrSndCmdHst'
ALTER TABLE [dbo].[TH68_MtrMsrSndCmdHst]
ADD CONSTRAINT [PK_TH68_MtrMsrSndCmdHst]
    PRIMARY KEY CLUSTERED ([F68_TerminalNo], [F68_AddDate], [F68_MsrMacCls], [F68_CommandType], [F68_Status], [F68_Priority], [F68_MasterCode], [F68_PictureNo] ASC);
GO

-- Creating primary key on [F69_MtrRtrDate], [F69_PalletNo], [F69_MaterialCode], [F69_MsrMacClass], [F69_TerminalNo], [F69_PictureNo], [F69_Status], [F69_Priority] in table 'TH69_MtrRtrMsrSndCmdHst'
ALTER TABLE [dbo].[TH69_MtrRtrMsrSndCmdHst]
ADD CONSTRAINT [PK_TH69_MtrRtrMsrSndCmdHst]
    PRIMARY KEY CLUSTERED ([F69_MtrRtrDate], [F69_PalletNo], [F69_MaterialCode], [F69_MsrMacClass], [F69_TerminalNo], [F69_PictureNo], [F69_Status], [F69_Priority] ASC);
GO

-- Creating primary key on [F70_ShipCommandNo], [F70_PalletNo], [F70_PrePdtLotNo], [F70_ProductCode], [F70_ProductLotNo], [F70_ShippedAmount], [F70_AddDate], [F70_UpdateDate] in table 'TH70_PdtShipHst'
ALTER TABLE [dbo].[TH70_PdtShipHst]
ADD CONSTRAINT [PK_TH70_PdtShipHst]
    PRIMARY KEY CLUSTERED ([F70_ShipCommandNo], [F70_PalletNo], [F70_PrePdtLotNo], [F70_ProductCode], [F70_ProductLotNo], [F70_ShippedAmount], [F70_AddDate], [F70_UpdateDate] ASC);
GO

-- Creating primary key on [F04_SupplierCode], [F04_MaxLoadAmount], [F04_AddDate], [F04_UpdateDate] in table 'TM04_Supplier'
ALTER TABLE [dbo].[TM04_Supplier]
ADD CONSTRAINT [PK_TM04_Supplier]
    PRIMARY KEY CLUSTERED ([F04_SupplierCode], [F04_MaxLoadAmount], [F04_AddDate], [F04_UpdateDate] ASC);
GO

-- Creating primary key on [F05_ConveyorCode], [F05_MaxBuffer], [F05_StrRtrSts], [F05_BufferUsing], [F05_AddDate], [F05_UpdateDate] in table 'TM05_Conveyor'
ALTER TABLE [dbo].[TM05_Conveyor]
ADD CONSTRAINT [PK_TM05_Conveyor]
    PRIMARY KEY CLUSTERED ([F05_ConveyorCode], [F05_MaxBuffer], [F05_StrRtrSts], [F05_BufferUsing], [F05_AddDate], [F05_UpdateDate] ASC);
GO

-- Creating primary key on [F06_TerminalNo], [F06_TermPos], [F06_ContinueFlag], [F06_OtherFlag], [F06_IPAddress1], [F06_IPAddress2], [F06_IPAddress3], [F06_IPAddress4], [F06_AddDate], [F06_UpdateDate] in table 'TM06_Terminal'
ALTER TABLE [dbo].[TM06_Terminal]
ADD CONSTRAINT [PK_TM06_Terminal]
    PRIMARY KEY CLUSTERED ([F06_TerminalNo], [F06_TermPos], [F06_ContinueFlag], [F06_OtherFlag], [F06_IPAddress1], [F06_IPAddress2], [F06_IPAddress3], [F06_IPAddress4], [F06_AddDate], [F06_UpdateDate] ASC);
GO

-- Creating primary key on [F07_Date], [F07_HolidayFlag], [F07_SunSatDayFlag], [F07_AddDate], [F07_UpdateDate] in table 'TM07_Calendar'
ALTER TABLE [dbo].[TM07_Calendar]
ADD CONSTRAINT [PK_TM07_Calendar]
    PRIMARY KEY CLUSTERED ([F07_Date], [F07_HolidayFlag], [F07_SunSatDayFlag], [F07_AddDate], [F07_UpdateDate] ASC);
GO

-- Creating primary key on [F08_ContainerType], [F08_AddDate], [F08_UpdateDate] in table 'TM08_Container'
ALTER TABLE [dbo].[TM08_Container]
ADD CONSTRAINT [PK_TM08_Container]
    PRIMARY KEY CLUSTERED ([F08_ContainerType], [F08_AddDate], [F08_UpdateDate] ASC);
GO

-- Creating primary key on [F10_EndUserCode], [F10_AddDate], [F10_UpdateDate] in table 'TM10_EndUsr'
ALTER TABLE [dbo].[TM10_EndUsr]
ADD CONSTRAINT [PK_TM10_EndUsr]
    PRIMARY KEY CLUSTERED ([F10_EndUserCode], [F10_AddDate], [F10_UpdateDate] ASC);
GO

-- Creating primary key on [F12_TerminalNo], [F12_PictureNo] in table 'TM12_TrmPicMgn'
ALTER TABLE [dbo].[TM12_TrmPicMgn]
ADD CONSTRAINT [PK_TM12_TrmPicMgn]
    PRIMARY KEY CLUSTERED ([F12_TerminalNo], [F12_PictureNo] ASC);
GO

-- Creating primary key on [F13_PictureNo], [F13_PswdLevel], [F13_AccessCtrlFlag], [F13_UserNumCtrlFlag], [F13_MaxUser], [F13_UsingCount] in table 'TM13_Function'
ALTER TABLE [dbo].[TM13_Function]
ADD CONSTRAINT [PK_TM13_Function]
    PRIMARY KEY CLUSTERED ([F13_PictureNo], [F13_PswdLevel], [F13_AccessCtrlFlag], [F13_UserNumCtrlFlag], [F13_MaxUser], [F13_UsingCount] ASC);
GO

-- Creating primary key on [F14_DeviceCode], [F14_UsePermitClass], [F14_AddDate], [F14_UpdateDate] in table 'TM14_Device'
ALTER TABLE [dbo].[TM14_Device]
ADD CONSTRAINT [PK_TM14_Device]
    PRIMARY KEY CLUSTERED ([F14_DeviceCode], [F14_UsePermitClass], [F14_AddDate], [F14_UpdateDate] ASC);
GO

-- Creating primary key on [F16_PswdLevel], [F16_Password] in table 'TM16_Password'
ALTER TABLE [dbo].[TM16_Password]
ADD CONSTRAINT [PK_TM16_Password]
    PRIMARY KEY CLUSTERED ([F16_PswdLevel], [F16_Password] ASC);
GO

-- Creating primary key on [F17_TermNo] in table 'TM17_TermStatus'
ALTER TABLE [dbo].[TM17_TermStatus]
ADD CONSTRAINT [PK_TM17_TermStatus]
    PRIMARY KEY CLUSTERED ([F17_TermNo] ASC);
GO

-- Creating primary key on [F18_LockPicture], [F18_LockedPicture] in table 'TM18_Access'
ALTER TABLE [dbo].[TM18_Access]
ADD CONSTRAINT [PK_TM18_Access]
    PRIMARY KEY CLUSTERED ([F18_LockPicture], [F18_LockedPicture] ASC);
GO

-- Creating primary key on [F30_PrcOrdNo], [F30_PrtDvrNo], [F30_MaterialCode], [F30_ExpectAmount], [F30_StoragedAmount], [F30_AcceptClass], [F30_AddDate], [F30_UpdateDate] in table 'TX30_Reception'
ALTER TABLE [dbo].[TX30_Reception]
ADD CONSTRAINT [PK_TX30_Reception]
    PRIMARY KEY CLUSTERED ([F30_PrcOrdNo], [F30_PrtDvrNo], [F30_MaterialCode], [F30_ExpectAmount], [F30_StoragedAmount], [F30_AcceptClass], [F30_AddDate], [F30_UpdateDate] ASC);
GO

-- Creating primary key on [F31_ShelfRow], [F31_ShelfBay], [F31_ShelfLevel], [F31_ShelfStatus], [F31_LiquidClass], [F31_StockTakingFlag], [F31_AddDate], [F31_UpdateDate] in table 'TX31_MtrShfSts'
ALTER TABLE [dbo].[TX31_MtrShfSts]
ADD CONSTRAINT [PK_TX31_MtrShfSts]
    PRIMARY KEY CLUSTERED ([F31_ShelfRow], [F31_ShelfBay], [F31_ShelfLevel], [F31_ShelfStatus], [F31_LiquidClass], [F31_StockTakingFlag], [F31_AddDate], [F31_UpdateDate] ASC);
GO

-- Creating primary key on [F32_PalletNo], [F32_MegaMsrMacSndEndFlg], [F32_GnrlMsrMacSndEndFlg], [F32_AddDate], [F32_UpdateDate] in table 'TX32_MtrShf'
ALTER TABLE [dbo].[TX32_MtrShf]
ADD CONSTRAINT [PK_TX32_MtrShf]
    PRIMARY KEY CLUSTERED ([F32_PalletNo], [F32_MegaMsrMacSndEndFlg], [F32_GnrlMsrMacSndEndFlg], [F32_AddDate], [F32_UpdateDate] ASC);
GO

-- Creating primary key on [F33_PalletNo], [F33_MaterialCode], [F33_MaterialLotNo], [F33_Amount], [F33_StockFlag], [F33_AddDate], [F33_UpdateDate] in table 'TX33_MtrShfStk'
ALTER TABLE [dbo].[TX33_MtrShfStk]
ADD CONSTRAINT [PK_TX33_MtrShfStk]
    PRIMARY KEY CLUSTERED ([F33_PalletNo], [F33_MaterialCode], [F33_MaterialLotNo], [F33_Amount], [F33_StockFlag], [F33_AddDate], [F33_UpdateDate] ASC);
GO

-- Creating primary key on [F34_CommandNo], [F34_CmdSeqNo], [F34_CommandType], [F34_StrRtrType], [F34_Status], [F34_Priority], [F34_From], [F34_To], [F34_TerminalNo], [F34_PictureNo], [F34_AddDate], [F34_UpdateDate] in table 'TX34_MtrWhsCmd'
ALTER TABLE [dbo].[TX34_MtrWhsCmd]
ADD CONSTRAINT [PK_TX34_MtrWhsCmd]
    PRIMARY KEY CLUSTERED ([F34_CommandNo], [F34_CmdSeqNo], [F34_CommandType], [F34_StrRtrType], [F34_Status], [F34_Priority], [F34_From], [F34_To], [F34_TerminalNo], [F34_PictureNo], [F34_AddDate], [F34_UpdateDate] ASC);
GO

-- Creating primary key on [F37_ShelfRow], [F37_ShelfBay], [F37_ShelfLevel], [F37_ShelfStatus], [F37_LowTmpCls], [F37_StockTakingFlag], [F37_AddDate], [F37_UpdateDate] in table 'TX37_PrePdtShfSts'
ALTER TABLE [dbo].[TX37_PrePdtShfSts]
ADD CONSTRAINT [PK_TX37_PrePdtShfSts]
    PRIMARY KEY CLUSTERED ([F37_ShelfRow], [F37_ShelfBay], [F37_ShelfLevel], [F37_ShelfStatus], [F37_LowTmpCls], [F37_StockTakingFlag], [F37_AddDate], [F37_UpdateDate] ASC);
GO

-- Creating primary key on [F40_PalletNo], [F40_PrePdtLotNo], [F40_ProductCode], [F40_ProductLotNo], [F40_StockFlag], [F40_PackageAmount], [F40_Fraction], [F40_Amount], [F40_TabletingEndDate], [F40_ShippedAmount], [F40_CertificationFlg], [F40_AddDate], [F40_UpdateDate] in table 'TX40_PdtShfStk'
ALTER TABLE [dbo].[TX40_PdtShfStk]
ADD CONSTRAINT [PK_TX40_PdtShfStk]
    PRIMARY KEY CLUSTERED ([F40_PalletNo], [F40_PrePdtLotNo], [F40_ProductCode], [F40_ProductLotNo], [F40_StockFlag], [F40_PackageAmount], [F40_Fraction], [F40_Amount], [F40_TabletingEndDate], [F40_ShippedAmount], [F40_CertificationFlg], [F40_AddDate], [F40_UpdateDate] ASC);
GO

-- Creating primary key on [F41_KndCmdNo], [F41_PrePdtLotNo], [F41_PreproductCode], [F41_Status], [F41_TblCntAmt], [F41_RtrEndCntAmt], [F41_ChgCntAmt], [F41_AddDate], [F41_UpdateDate] in table 'TX41_TbtCmd'
ALTER TABLE [dbo].[TX41_TbtCmd]
ADD CONSTRAINT [PK_TX41_TbtCmd]
    PRIMARY KEY CLUSTERED ([F41_KndCmdNo], [F41_PrePdtLotNo], [F41_PreproductCode], [F41_Status], [F41_TblCntAmt], [F41_RtrEndCntAmt], [F41_ChgCntAmt], [F41_AddDate], [F41_UpdateDate] ASC);
GO

-- Creating primary key on [F42_KndCmdNo], [F42_PrePdtLotNo], [F42_PreProductCode], [F42_KndEptBgnDate], [F42_OutSideClass], [F42_Status], [F42_ThrowAmount], [F42_NeedAmount], [F42_StgCtnAmt], [F42_BatchEndAmount], [F42_KndCmdBookNo], [F42_LotSeqNo], [F42_CommandSeqNo], [F42_MtrRtrFlg], [F42_AddDate], [F42_UpdateDate] in table 'TX42_KndCmd'
ALTER TABLE [dbo].[TX42_KndCmd]
ADD CONSTRAINT [PK_TX42_KndCmd]
    PRIMARY KEY CLUSTERED ([F42_KndCmdNo], [F42_PrePdtLotNo], [F42_PreProductCode], [F42_KndEptBgnDate], [F42_OutSideClass], [F42_Status], [F42_ThrowAmount], [F42_NeedAmount], [F42_StgCtnAmt], [F42_BatchEndAmount], [F42_KndCmdBookNo], [F42_LotSeqNo], [F42_CommandSeqNo], [F42_MtrRtrFlg], [F42_AddDate], [F42_UpdateDate] ASC);
GO

-- Creating primary key on [F43_KndCmdNo], [F43_PrePdtLotNo], [F43_BatchSeqNo], [F43_MaterialCode], [F43_MaterialLotNo], [F43_LayinginAmount], [F43_AddDate], [F43_UpdateDate] in table 'TX43_KndRcd'
ALTER TABLE [dbo].[TX43_KndRcd]
ADD CONSTRAINT [PK_TX43_KndRcd]
    PRIMARY KEY CLUSTERED ([F43_KndCmdNo], [F43_PrePdtLotNo], [F43_BatchSeqNo], [F43_MaterialCode], [F43_MaterialLotNo], [F43_LayinginAmount], [F43_AddDate], [F43_UpdateDate] ASC);
GO

-- Creating primary key on [F44_ShipCommandNo], [F44_ProductCode], [F44_ShpRqtAmt], [F44_ShippedAmount], [F44_Status], [F44_AddDate], [F44_UpdateDate] in table 'TX44_ShippingPlan'
ALTER TABLE [dbo].[TX44_ShippingPlan]
ADD CONSTRAINT [PK_TX44_ShippingPlan]
    PRIMARY KEY CLUSTERED ([F44_ShipCommandNo], [F44_ProductCode], [F44_ShpRqtAmt], [F44_ShippedAmount], [F44_Status], [F44_AddDate], [F44_UpdateDate] ASC);
GO

-- Creating primary key on [F45_ShipCommandNo], [F45_ShipAmount], [F45_ShippedAmount], [F45_AddDate], [F45_UpdateDate] in table 'TX45_ShipCommand'
ALTER TABLE [dbo].[TX45_ShipCommand]
ADD CONSTRAINT [PK_TX45_ShipCommand]
    PRIMARY KEY CLUSTERED ([F45_ShipCommandNo], [F45_ShipAmount], [F45_ShippedAmount], [F45_AddDate], [F45_UpdateDate] ASC);
GO

-- Creating primary key on [F46_SubMaterialCode], [F46_StorageDate], [F46_Amount], [F46_AddDate], [F46_UpdateDate] in table 'TX46_SupMtrStk'
ALTER TABLE [dbo].[TX46_SupMtrStk]
ADD CONSTRAINT [PK_TX46_SupMtrStk]
    PRIMARY KEY CLUSTERED ([F46_SubMaterialCode], [F46_StorageDate], [F46_Amount], [F46_AddDate], [F46_UpdateDate] ASC);
GO

-- Creating primary key on [F47_CommandNo], [F47_CmdSeqNo], [F47_CommandType], [F47_StrRtrType], [F47_Status], [F47_Priority], [F47_From], [F47_To], [F47_TerminalNo], [F47_PictureNo], [F47_AddDate], [F47_UpdateDate] in table 'TX47_PdtWhsCmd'
ALTER TABLE [dbo].[TX47_PdtWhsCmd]
ADD CONSTRAINT [PK_TX47_PdtWhsCmd]
    PRIMARY KEY CLUSTERED ([F47_CommandNo], [F47_CmdSeqNo], [F47_CommandType], [F47_StrRtrType], [F47_Status], [F47_Priority], [F47_From], [F47_To], [F47_TerminalNo], [F47_PictureNo], [F47_AddDate], [F47_UpdateDate] ASC);
GO

-- Creating primary key on [F48_SystemId], [F48_KndCmdBookNo], [F48_MegaKndCmdNo], [F48_GnrKndCmdNo], [F48_OutKndCmdNo], [F48_KneadSheefNo], [F48_CnrKndCmdNo], [F48_MtrWhsCmdNo], [F48_PrePdtWhsCmdNo], [F48_PdtWhsCmdNo], [F48_OutPlanPdtCmdNo], [F48_AddDate], [F48_UpdateDate] in table 'TX48_NoManage'
ALTER TABLE [dbo].[TX48_NoManage]
ADD CONSTRAINT [PK_TX48_NoManage]
    PRIMARY KEY CLUSTERED ([F48_SystemId], [F48_KndCmdBookNo], [F48_MegaKndCmdNo], [F48_GnrKndCmdNo], [F48_OutKndCmdNo], [F48_KneadSheefNo], [F48_CnrKndCmdNo], [F48_MtrWhsCmdNo], [F48_PrePdtWhsCmdNo], [F48_PdtWhsCmdNo], [F48_OutPlanPdtCmdNo], [F48_AddDate], [F48_UpdateDate] ASC);
GO

-- Creating primary key on [F49_ContainerCode], [F49_KndCmdNo], [F49_PreProductCode], [F49_PreProductLotNo], [F49_Amount], [F49_ShelfStatus], [F49_ContainerSeqNo], [F49_AddDate], [F49_UpdateDate] in table 'TX49_PrePdtShfStk'
ALTER TABLE [dbo].[TX49_PrePdtShfStk]
ADD CONSTRAINT [PK_TX49_PrePdtShfStk]
    PRIMARY KEY CLUSTERED ([F49_ContainerCode], [F49_KndCmdNo], [F49_PreProductCode], [F49_PreProductLotNo], [F49_Amount], [F49_ShelfStatus], [F49_ContainerSeqNo], [F49_AddDate], [F49_UpdateDate] ASC);
GO

-- Creating primary key on [F50_CommandNo], [F50_CmdSeqNo], [F50_CommandType], [F50_StrRtrType], [F50_Status], [F50_ContainerCode], [F50_Priority], [F50_From], [F50_To], [F50_TerminalNo], [F50_PictureNo], [F50_AddDate], [F50_UpdateDate] in table 'TX50_PrePdtWhsCmd'
ALTER TABLE [dbo].[TX50_PrePdtWhsCmd]
ADD CONSTRAINT [PK_TX50_PrePdtWhsCmd]
    PRIMARY KEY CLUSTERED ([F50_CommandNo], [F50_CmdSeqNo], [F50_CommandType], [F50_StrRtrType], [F50_Status], [F50_ContainerCode], [F50_Priority], [F50_From], [F50_To], [F50_TerminalNo], [F50_PictureNo], [F50_AddDate], [F50_UpdateDate] ASC);
GO

-- Creating primary key on [F51_ShelfRow], [F51_ShelfBay], [F51_ShelfLevel], [F51_ShelfType], [F51_LoadAmount], [F51_StockTakingFlag], [F51_AddDate], [F51_UpdateDate] in table 'TX51_PdtShfSts'
ALTER TABLE [dbo].[TX51_PdtShfSts]
ADD CONSTRAINT [PK_TX51_PdtShfSts]
    PRIMARY KEY CLUSTERED ([F51_ShelfRow], [F51_ShelfBay], [F51_ShelfLevel], [F51_ShelfType], [F51_LoadAmount], [F51_StockTakingFlag], [F51_AddDate], [F51_UpdateDate] ASC);
GO

-- Creating primary key on [F52_TerminalNo], [F52_AddDate], [F52_MsrMacCls], [F52_CommandType], [F52_Status], [F52_Priority], [F52_MasterCode], [F52_UpdateDate] in table 'TX52_MtrMsrSndCmd'
ALTER TABLE [dbo].[TX52_MtrMsrSndCmd]
ADD CONSTRAINT [PK_TX52_MtrMsrSndCmd]
    PRIMARY KEY CLUSTERED ([F52_TerminalNo], [F52_AddDate], [F52_MsrMacCls], [F52_CommandType], [F52_Status], [F52_Priority], [F52_MasterCode], [F52_UpdateDate] ASC);
GO

-- Creating primary key on [F53_PalletNo], [F53_OutSidePrePdtCode], [F53_OutSidePrePdtLotNo], [F53_KndCmdNo], [F53_PalletSeqNo], [F53_Amount], [F53_StockFlag], [F53_AddDate], [F53_UpdateDate] in table 'TX53_OutSidePrePdtStk'
ALTER TABLE [dbo].[TX53_OutSidePrePdtStk]
ADD CONSTRAINT [PK_TX53_OutSidePrePdtStk]
    PRIMARY KEY CLUSTERED ([F53_PalletNo], [F53_OutSidePrePdtCode], [F53_OutSidePrePdtLotNo], [F53_KndCmdNo], [F53_PalletSeqNo], [F53_Amount], [F53_StockFlag], [F53_AddDate], [F53_UpdateDate] ASC);
GO

-- Creating primary key on [F54_MtrRtrDate], [F54_PalletNo], [F54_MaterialCode], [F54_MsrMacClass], [F54_TerminalNo], [F54_PictureNo], [F54_Status], [F54_Priority], [F54_AddDate], [F54_UpdateDate] in table 'TX54_MtrRtrMsrSndCmd'
ALTER TABLE [dbo].[TX54_MtrRtrMsrSndCmd]
ADD CONSTRAINT [PK_TX54_MtrRtrMsrSndCmd]
    PRIMARY KEY CLUSTERED ([F54_MtrRtrDate], [F54_PalletNo], [F54_MaterialCode], [F54_MsrMacClass], [F54_TerminalNo], [F54_PictureNo], [F54_Status], [F54_Priority], [F54_AddDate], [F54_UpdateDate] ASC);
GO

-- Creating primary key on [F55_KndCmdNo], [F55_PrePdtLotNo], [F55_PrePdtCode], [F55_Status], [F55_Priority], [F55_MsrSndCls], [F55_TerminalNo], [F55_PictureNo], [F55_AbnormalCode], [F55_AddDate], [F55_UpdateDate] in table 'TX55_KndCmdMsrSnd'
ALTER TABLE [dbo].[TX55_KndCmdMsrSnd]
ADD CONSTRAINT [PK_TX55_KndCmdMsrSnd]
    PRIMARY KEY CLUSTERED ([F55_KndCmdNo], [F55_PrePdtLotNo], [F55_PrePdtCode], [F55_Status], [F55_Priority], [F55_MsrSndCls], [F55_TerminalNo], [F55_PictureNo], [F55_AbnormalCode], [F55_AddDate], [F55_UpdateDate] ASC);
GO

-- Creating primary key on [F56_KndCmdNo], [F56_PrePdtLotNo], [F56_ProductCode], [F56_ProductLotNo], [F56_Status], [F56_TbtCmdAmt], [F56_TbtCmdEndPackAmt], [F56_TbtCmdEndFrtAmt], [F56_TbtCmdEndAmt], [F56_StorageAmt], [F56_CertificationFlag], [F56_AddDate], [F56_UpdateDate] in table 'TX56_TbtPdt'
ALTER TABLE [dbo].[TX56_TbtPdt]
ADD CONSTRAINT [PK_TX56_TbtPdt]
    PRIMARY KEY CLUSTERED ([F56_KndCmdNo], [F56_PrePdtLotNo], [F56_ProductCode], [F56_ProductLotNo], [F56_Status], [F56_TbtCmdAmt], [F56_TbtCmdEndPackAmt], [F56_TbtCmdEndFrtAmt], [F56_TbtCmdEndAmt], [F56_StorageAmt], [F56_CertificationFlag], [F56_AddDate], [F56_UpdateDate] ASC);
GO

-- Creating primary key on [F57_PalletNo], [F57_OutFlg], [F57_AddDate], [F57_UpdateDate] in table 'TX57_PdtShf'
ALTER TABLE [dbo].[TX57_PdtShf]
ADD CONSTRAINT [PK_TX57_PdtShf]
    PRIMARY KEY CLUSTERED ([F57_PalletNo], [F57_OutFlg], [F57_AddDate], [F57_UpdateDate] ASC);
GO

-- Creating primary key on [F58_PrePdtLotNo], [F58_ProductCode], [F58_PdtSeqNo], [F58_ProductLotNo], [F58_Status], [F58_TbtCmdEndPackAmt], [F58_TbtCmdEndFrtAmt], [F58_TbtCmdEndAmt], [F58_StorageAmt], [F58_CertificationFlag], [F58_AddDate], [F58_UpdateDate] in table 'TX58_OutPlanPdt'
ALTER TABLE [dbo].[TX58_OutPlanPdt]
ADD CONSTRAINT [PK_TX58_OutPlanPdt]
    PRIMARY KEY CLUSTERED ([F58_PrePdtLotNo], [F58_ProductCode], [F58_PdtSeqNo], [F58_ProductLotNo], [F58_Status], [F58_TbtCmdEndPackAmt], [F58_TbtCmdEndFrtAmt], [F58_TbtCmdEndAmt], [F58_StorageAmt], [F58_CertificationFlag], [F58_AddDate], [F58_UpdateDate] ASC);
GO

-- Creating primary key on [F91_YearMonth], [F91_MaterialCode], [F91_PrevRemainder], [F91_Received], [F91_Used] in table 'TX91_MaterialTotal'
ALTER TABLE [dbo].[TX91_MaterialTotal]
ADD CONSTRAINT [PK_TX91_MaterialTotal]
    PRIMARY KEY CLUSTERED ([F91_YearMonth], [F91_MaterialCode], [F91_PrevRemainder], [F91_Received], [F91_Used] ASC);
GO

-- Creating primary key on [F92_YearMonth], [F92_PrepdtCode], [F92_PrevRemainder], [F92_Received], [F92_Used] in table 'TX92_PrepdtTotal'
ALTER TABLE [dbo].[TX92_PrepdtTotal]
ADD CONSTRAINT [PK_TX92_PrepdtTotal]
    PRIMARY KEY CLUSTERED ([F92_YearMonth], [F92_PrepdtCode], [F92_PrevRemainder], [F92_Received], [F92_Used] ASC);
GO

-- Creating primary key on [F93_YearMonth], [F93_ProductCode], [F93_PrevRemainder], [F93_Received], [F93_Used] in table 'TX93_ProductTotal'
ALTER TABLE [dbo].[TX93_ProductTotal]
ADD CONSTRAINT [PK_TX93_ProductTotal]
    PRIMARY KEY CLUSTERED ([F93_YearMonth], [F93_ProductCode], [F93_PrevRemainder], [F93_Received], [F93_Used] ASC);
GO

-- Creating primary key on [F94_YearMonth], [F94_PrepdtCode], [F94_amount] in table 'TX94_Prepdtplan'
ALTER TABLE [dbo].[TX94_Prepdtplan]
ADD CONSTRAINT [PK_TX94_Prepdtplan]
    PRIMARY KEY CLUSTERED ([F94_YearMonth], [F94_PrepdtCode], [F94_amount] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------