CREATE TABLE [dbo].[TBL_UQC_MASTER] (
    [uqcid]       INT           IDENTITY (1, 1) NOT NULL,
    [uqc]         NVARCHAR (25) NULL,
    [uqcDesc]     NVARCHAR (50) NULL,
    [rowstatus]   BIT           CONSTRAINT [DF_TBL_UQC_MASTER_rowstatus] DEFAULT ((1)) NULL,
    [createdby]   INT           NULL,
    [createddate] DATETIME      NULL,
    CONSTRAINT [PK_TBL_UQC_MASTER] PRIMARY KEY CLUSTERED ([uqcid] ASC)
);

