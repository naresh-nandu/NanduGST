CREATE VIEW [dbo].[View_Role_Resource_Menus]
AS
SELECT        dbo.MAS_Roles.Role_ID, dbo.MAS_Roles.Role_Name, dbo.MAS_Resources.Resource_ID, dbo.MAS_Resources.Resource_Name, dbo.MAS_Resources.Resource_Display_Name, 
                         dbo.MAS_Resources.Resource_Page_Name, dbo.MAS_Resources.Resource_MenuItem_ID, dbo.MAS_Resources.FK_Parent_Resource_ID, dbo.MAS_Roles_Resources.Role_Resource_ID, 
                         dbo.MAS_Roles_Resources.FK_Role_Resource_Role_ID, dbo.MAS_Roles_Resources.FK_Role_Resource_Resource_ID, dbo.MAS_Roles_Resources.Role_Resource_IsAssigned, 
                         dbo.MAS_Roles.CustomerID
FROM            dbo.MAS_Roles_Resources INNER JOIN
                         dbo.MAS_Roles ON dbo.MAS_Roles_Resources.FK_Role_Resource_Role_ID = dbo.MAS_Roles.Role_ID INNER JOIN
                         dbo.MAS_Resources ON dbo.MAS_Roles_Resources.FK_Role_Resource_Resource_ID = dbo.MAS_Resources.Resource_ID