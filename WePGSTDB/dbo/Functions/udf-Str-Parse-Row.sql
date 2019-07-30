CREATE FUNCTION [udf-Str-Parse-Row] (@String varchar(max),@Delimeter varchar(10))
--Usage: Select * from [dbo].[udf-Str-Parse-Row]('Dog,Cat,House,Car',',')
--       Select * from [dbo].[udf-Str-Parse-Row]('John Cappelletti',' ')
--       Select * from [dbo].[udf-Str-Parse-Row]('id26,id46|id658,id967','|')

Returns Table 

As

Return (
    SELECT Pos1 = xDim.value('/x[1]','varchar(250)')
          ,Pos2 = xDim.value('/x[2]','varchar(250)')
          ,Pos3 = xDim.value('/x[3]','varchar(250)')
          ,Pos4 = xDim.value('/x[4]','varchar(250)')
          ,Pos5 = xDim.value('/x[5]','varchar(250)')
          ,Pos6 = xDim.value('/x[6]','varchar(250)')
          ,Pos7 = xDim.value('/x[7]','varchar(250)')
          ,Pos8 = xDim.value('/x[8]','varchar(250)')
          ,Pos9 = xDim.value('/x[9]','varchar(250)')
    FROM (Select Cast('<x>' + Replace(@String,@Delimeter,'</x><x>')+'</x>' as XML) as xDim) A)