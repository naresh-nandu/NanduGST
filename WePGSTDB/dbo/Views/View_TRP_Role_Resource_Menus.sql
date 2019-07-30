CREATE VIEW [dbo].[View_TRP_Role_Resource_Menus]
AS
SELECT        dbo.TBL_TRP_Roles.Role_ID, dbo.TBL_TRP_Roles.Role_Name, dbo.TBL_TRP_Resources.Resource_ID, dbo.TBL_TRP_Resources.Resource_Name, dbo.TBL_TRP_Resources.Resource_Display_Name, 
                         dbo.TBL_TRP_Resources.Resource_Page_Name, dbo.TBL_TRP_Resources.Resource_MenuItem_ID, dbo.TBL_TRP_Resources.FK_Parent_Resource_ID, dbo.TBL_TRP_Roles_Resources.Role_Resource_ID, 
                         dbo.TBL_TRP_Roles_Resources.FK_Role_Resource_Role_ID, dbo.TBL_TRP_Roles_Resources.FK_Role_Resource_Resource_ID, dbo.TBL_TRP_Roles_Resources.Role_Resource_IsAssigned, 
                         dbo.TBL_TRP_Roles.TRPId
FROM            dbo.TBL_TRP_Roles_Resources INNER JOIN
                         dbo.TBL_TRP_Roles ON dbo.TBL_TRP_Roles_Resources.FK_Role_Resource_Role_ID = dbo.TBL_TRP_Roles.Role_ID INNER JOIN
                         dbo.TBL_TRP_Resources ON dbo.TBL_TRP_Roles_Resources.FK_Role_Resource_Resource_ID = dbo.TBL_TRP_Resources.Resource_ID