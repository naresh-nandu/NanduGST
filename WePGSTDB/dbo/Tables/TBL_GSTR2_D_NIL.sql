CREATE TABLE [dbo].[TBL_GSTR2_D_NIL] (
    [nilid]    INT          IDENTITY (1, 1) NOT NULL,
    [gstr2did] INT          NOT NULL,
    [sply_ty]  VARCHAR (50) NULL,
    [gstinid]  INT          NULL,
    CONSTRAINT [PK_TBL_GSTR2_D_NIL] PRIMARY KEY CLUSTERED ([nilid] ASC),
    CONSTRAINT [FK_TBL_GSTR2_D_NIL_TBL_GSTR2_D] FOREIGN KEY ([gstr2did]) REFERENCES [dbo].[TBL_GSTR2_D] ([gstr2did])
);

