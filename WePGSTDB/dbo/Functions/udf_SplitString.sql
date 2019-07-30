﻿CREATE FUNCTION [udf_SplitString]
(    
      @Input NVARCHAR(MAX),
      @Character CHAR(1)
)
RETURNS @Output TABLE (
		Col_1 varchar(50),
		Col_2 varchar(50),
		Col_3 varchar(50),
		Col_4 varchar(50),
		Col_5 varchar(50),
		Col_6 varchar(50),
		Col_7 varchar(50),
		Col_8 varchar(50),
		Col_9 varchar(50),
		Col_10 varchar(50),
		Col_11 varchar(50),
		Col_12 varchar(50),
		Col_13 varchar(50),
		Col_14 varchar(50),
		Col_15 varchar(50),
		Col_16 varchar(50),
		Col_17 varchar(50),
		Col_18 varchar(50),
		Col_19 varchar(50),
		Col_20 varchar(50),
		Col_21 varchar(50),
		Col_22 varchar(50),
		Col_23 varchar(50),
		Col_24 varchar(50),
		Col_25 varchar(50),
		Col_26 varchar(50),
		Col_27 varchar(50),
		Col_28 varchar(50),
		Col_29 varchar(50),
		Col_30 varchar(50),
		Col_31 varchar(50),
		Col_32 varchar(50),
		Col_33 varchar(50),
		Col_34 varchar(50),
		Col_35 varchar(50),
		Col_36 varchar(50),
		Col_37 varchar(50),
		Col_38 varchar(50),
	--	Col_39 varchar(50),
		Col_40 varchar(50),
		Col_41 varchar(50),
	--	Col_42 varchar(50),
		Col_43 varchar(50),
		Col_44 varchar(50),
	--	Col_45 varchar(50),
		Col_46 varchar(50),
		Col_47 varchar(50),
		Col_48 varchar(50),
		Col_49 varchar(50),
	--	Col_50 varchar(50),
		Col_51 varchar(50),
		Col_52 varchar(50),
		Col_53 varchar(50),
		Col_54 varchar(50),
		Col_55 varchar(50),
		Col_56 varchar(50)
)
AS
BEGIN

	DECLARE @TBL_Values TABLE 
	(
		RowNum Int,
		Value varchar(255)
	)
	
	Insert Into @TBL_Values
	Select	Row_Number() over (Order by (SELECT NULL)) as RowNum, 
			Value 
	From string_split( @Input, @Character) 

	Insert Into @Output
	(	Col_1,Col_2,Col_3,Col_4,Col_5,Col_6,Col_7,
		Col_8,Col_9,Col_10,Col_11,Col_12,Col_13,Col_14,
		Col_15,Col_16,Col_17,Col_18,Col_19,Col_20,Col_21,
		Col_22,Col_23,Col_24,Col_25,Col_26,Col_27,Col_28,
		Col_29,Col_30,Col_31,Col_32,Col_33,Col_34,Col_35,
		Col_36,Col_37,Col_38,
		--Col_39,
		Col_40,Col_41,
		--Col_42,
		Col_43,Col_44,
		--Col_45,
		Col_46,Col_47,Col_48,Col_49,
		--Col_50,
		Col_51,Col_52,Col_53,Col_54,Col_55,Col_56

	)
	Values
	(
		(Select Value From  @TBL_Values Where RowNum = 1),
		(Select Value From  @TBL_Values Where RowNum = 2),
		(Select Value From  @TBL_Values Where RowNum = 3),
		(Select Value From  @TBL_Values Where RowNum = 4),
		(Select Value From  @TBL_Values Where RowNum = 5),
		(Select Value From  @TBL_Values Where RowNum = 6),
		(Select Value From  @TBL_Values Where RowNum = 7),
		(Select Value From  @TBL_Values Where RowNum = 8),
		(Select Value From  @TBL_Values Where RowNum = 9),
		(Select Value From  @TBL_Values Where RowNum = 10),
		(Select Value From  @TBL_Values Where RowNum = 11),
		(Select Value From  @TBL_Values Where RowNum = 12),
		(Select Value From  @TBL_Values Where RowNum = 13),
		(Select Value From  @TBL_Values Where RowNum = 14),
		(Select Value From  @TBL_Values Where RowNum = 15),
		(Select Value From  @TBL_Values Where RowNum = 16),
		(Select Value From  @TBL_Values Where RowNum = 17),
		(Select Value From  @TBL_Values Where RowNum = 18),
		(Select Value From  @TBL_Values Where RowNum = 19),
		(Select Value From  @TBL_Values Where RowNum = 20),
		(Select Value From  @TBL_Values Where RowNum = 21),
		(Select Value From  @TBL_Values Where RowNum = 22),
		(Select Value From  @TBL_Values Where RowNum = 23),
		(Select Value From  @TBL_Values Where RowNum = 24),
		(Select Value From  @TBL_Values Where RowNum = 25),
		(Select Value From  @TBL_Values Where RowNum = 26),
		(Select Value From  @TBL_Values Where RowNum = 27),
		(Select Value From  @TBL_Values Where RowNum = 28),
		(Select Value From  @TBL_Values Where RowNum = 29),
		(Select Value From  @TBL_Values Where RowNum = 30),
		(Select Value From  @TBL_Values Where RowNum = 31),
		(Select Value From  @TBL_Values Where RowNum = 32),
		(Select Value From  @TBL_Values Where RowNum = 33),
		(Select Value From  @TBL_Values Where RowNum = 34),
		(Select Value From  @TBL_Values Where RowNum = 35),
		(Select Value From  @TBL_Values Where RowNum = 36),
		(Select Value From  @TBL_Values Where RowNum = 37),
		(Select Value From  @TBL_Values Where RowNum = 38),
		--(Select Value From  @TBL_Values Where RowNum = 39),
		(Select Value From  @TBL_Values Where RowNum = 40),
		(Select Value From  @TBL_Values Where RowNum = 41),
		--(Select Value From  @TBL_Values Where RowNum = 42),
		(Select Value From  @TBL_Values Where RowNum = 43),
		(Select Value From  @TBL_Values Where RowNum = 44),
		--(Select Value From  @TBL_Values Where RowNum = 45),
		(Select Value From  @TBL_Values Where RowNum = 46),
		(Select Value From  @TBL_Values Where RowNum = 47),
		(Select Value From  @TBL_Values Where RowNum = 48),
		(Select Value From  @TBL_Values Where RowNum = 49),
		--(Select Value From  @TBL_Values Where RowNum = 50),
		(Select Value From  @TBL_Values Where RowNum = 51),
		(Select Value From  @TBL_Values Where RowNum = 52),
		(Select Value From  @TBL_Values Where RowNum = 53),
		(Select Value From  @TBL_Values Where RowNum = 54),
		(Select Value From  @TBL_Values Where RowNum = 55),
		(Select Value From  @TBL_Values Where RowNum = 56)
	)		

     RETURN
END