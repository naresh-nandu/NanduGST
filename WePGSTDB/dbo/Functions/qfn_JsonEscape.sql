
CREATE FUNCTION [qfn_JsonEscape](@value nvarchar(max) )
returns nvarchar(max)
as begin
 
 if (@value is null) return 'null'
 if (TRY_PARSE( @value as float) is not null) return @value

 set @value=replace(@value,CHAR(92),CHAR(92)+CHAR(92))
 set @value=replace(@value,CHAR(34),CHAR(92)+CHAR(34))

 set @value=replace(@value,'/',CHAR(92)+'/')
 set @value=replace(@value,CHAR(10),CHAR(92)+'n')
 set @value=replace(@value,CHAR(13),CHAR(92)+'r')
 set @value=replace(@value,CHAR(19),CHAR(92)+'t')


 return CHAR(34)+@value+CHAR(34)

end