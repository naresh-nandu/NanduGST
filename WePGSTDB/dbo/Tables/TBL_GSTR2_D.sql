CREATE TABLE [dbo].[TBL_GSTR2_D] (
    [gstr2did]        INT           IDENTITY (1, 1) NOT NULL,
    [gstin]           NVARCHAR (15) NULL,
    [gstinid]         INT           NOT NULL,
    [ret_period]      VARCHAR (6)   NULL,
    [action_required] VARCHAR (1)   NULL,
    CONSTRAINT [PK_TBL_GSTR2_D] PRIMARY KEY CLUSTERED ([gstr2did] ASC),
    CONSTRAINT [FK_TBL_GSTR2_D_TBL_Cust_GSTIN] FOREIGN KEY ([gstinid]) REFERENCES [dbo].[TBL_Cust_GSTIN] ([GSTINId])
);

