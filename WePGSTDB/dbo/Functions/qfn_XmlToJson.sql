CREATE FUNCTION [qfn_XmlToJson]
(
	@XmlData xml
)
RETURNS nvarchar(max)
AS
BEGIN
	declare @m nvarchar(max)
	SELECT @m='['+Stuff(
		(
		
		SELECT theline from																				
	(SELECT ','+' {'+																							
				Stuff(
					(SELECT ',"'+coalesce(b.c.value('local-name(.)', 'NVARCHAR(255)'),'')+'":'+			
						case when b.c.value('count(*)','int')=0 then 
                         dbo.qfn_JsonEscape(b.c.value('text()[1]','NVARCHAR(MAX)')) 
						else dbo.qfn_XmlToJson(b.c.query('*'))
						end
						
					from x.a.nodes('*') b(c)																
					for xml path(''),TYPE).value('(./text())[1]','NVARCHAR(MAX)')
				,1,1,'')+'}'																				
				
			from @XmlData.nodes('/*') x(a)																
			) JSON(theLine)																					
			
	
			
			for xml path(''),TYPE).value('.','NVARCHAR(MAX)' )
			,1,1,'')+']'																						--remove the first leading comma
	return @m
END