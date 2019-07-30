CREATE TABLE [dbo].[TBL_GSTR1_D_CDNR_NT_ITMS] (
    [itmsid]   INT IDENTITY (1, 1) NOT NULL,
    [ntid]     INT NOT NULL,
    [num]      INT NULL,
    [gstinid]  INT NULL,
    [gstr1did] INT NULL,
    PRIMARY KEY CLUSTERED ([itmsid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_D_CDNR_NT_ITMS_TBL_GSTR1_D_CDNR_NT] FOREIGN KEY ([ntid]) REFERENCES [dbo].[TBL_GSTR1_D_CDNR_NT] ([ntid])
);

