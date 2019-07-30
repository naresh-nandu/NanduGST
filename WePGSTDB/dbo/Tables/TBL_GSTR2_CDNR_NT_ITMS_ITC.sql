CREATE TABLE [dbo].[TBL_GSTR2_CDNR_NT_ITMS_ITC] (
    [itmitcid] INT             IDENTITY (1, 1) NOT NULL,
    [itmsid]   INT             NOT NULL,
    [tx_i]     DECIMAL (18, 2) NULL,
    [tx_c]     DECIMAL (18, 2) NULL,
    [tx_s]     DECIMAL (18, 2) NULL,
    [tx_cs]    DECIMAL (18, 2) NULL,
    [elg]      VARCHAR (2)     NULL,
    [gstinid]  INT             NULL,
    [gstr2id]  INT             NULL,
    CONSTRAINT [PK_TBL_GSTR2_CDNR_NT_ITMS_ITC] PRIMARY KEY CLUSTERED ([itmitcid] ASC),
    CONSTRAINT [FK_TBL_GSTR2_CDNR_NT_ITMS_ITC_TBL_GSTR2_CDNR_NT_ITMS] FOREIGN KEY ([itmsid]) REFERENCES [dbo].[TBL_GSTR2_CDNR_NT_ITMS] ([itmsid])
);

