CREATE TABLE [dbo].[TBL_MAS_EWAYBILL] (
    [ewbTypeId]    INT           IDENTITY (1, 1) NOT NULL,
    [TypeName]     NVARCHAR (50) NULL,
    [MasterType]   NVARCHAR (50) NULL,
    [TypeAbbr]     NVARCHAR (10) NULL,
    [subewbTypeId] INT           NULL
);



