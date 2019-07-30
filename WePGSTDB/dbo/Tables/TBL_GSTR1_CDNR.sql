CREATE TABLE [dbo].[TBL_GSTR1_CDNR] (
    [cdnrid]  INT          IDENTITY (1, 1) NOT NULL,
    [gstr1id] INT          NOT NULL,
    [ctin]    VARCHAR (15) NULL,
    [cfs]     VARCHAR (1)  NULL,
    [gstinid] INT          NULL,
    PRIMARY KEY CLUSTERED ([cdnrid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_CDNR_TBL_GSTR1] FOREIGN KEY ([gstr1id]) REFERENCES [dbo].[TBL_GSTR1] ([gstr1id])
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_GSTR1_CDNR_5D141D7DB66ADF7C34B8EE952C4689A3]
    ON [dbo].[TBL_GSTR1_CDNR]([gstr1id] ASC, [ctin] ASC);

