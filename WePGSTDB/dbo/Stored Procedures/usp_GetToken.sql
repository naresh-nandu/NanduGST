
/*
Change History
Date 		Who			Description
8/15/2017	Seshadri	Intial Version
*/


Create Procedure [usp_GetToken]
	@SourceString varchar(2000) Output,
	@Token varchar(2000) Output,
	@Separators varchar(10),
	@MaxTokenLength SmallInt,
	@ReturnSpaceifNull tinyint=0
--mssx with encryption 
as
If @SourceString is not null and @SourceString like char(13)+Char(10)+'%'
	Select @SourceString=SubString(@SourceString, 3, 2000)
else if @SourceString is not null and @SourceString like char(10)+Char(13)+'%'
	Select @SourceString=SubString(@SourceString, 3, 2000)
else if @SourceString is not null and @SourceString like Char(13)+'%'
	Select @SourceString=SubString(@SourceString, 2, 2000)
else if @SourceString is not null and @SourceString like Char(10)+'%'
	Select @SourceString=SubString(@SourceString, 2, 2000)

if @SourceString is Null 
begin
	Select @Token= space(@ReturnSpaceifNull)
	return 0
end
declare @EndofToken SmallInt,@EndofToken1 SmallInt,@EndofToken2 SmallInt

/*new line character seperators come in different ways from different systems. they may be char(13) + char (10) OR char(10) + char(13) or char(10) or char(13)
in such cases when the line seperator passed is char(13) + char(10) we should identify the other possible patterns also and treat any one of them as line feed.
in the below section we change the line seperator itself in accordence with the line seperator in the passed string. if line seperator in passed string, for example, 
is char(10) then we change the seperator that is passed from char(13) + char(10) to char(10)
*/
if @Separators = char(13)+char(10)
begin
	Select @EndofToken1=isnull(CharIndex(char(13), @SourceString),0)
	Select @EndofToken2=isnull(CharIndex(char(10), @SourceString),0)

	if @EndofToken1 > 0 or @EndofToken2 > 0
		if @EndofToken1 = @EndofToken2 - 1	and @EndofToken1 > 0 -- case when seperator is char(13) + char(10) 
			select @EndofToken = @EndofToken1, @Separators = char(13)+char(10)
		else if @EndofToken2 = @EndofToken1 - 1 and @EndofToken2 > 0--seperatpr = Char(10) + char(13)
			select @EndofToken = @EndofToken2, @Separators = char(10) + char(13)
		else if (@EndofToken1 < @EndofToken2 - 1 and @EndofToken1 > 0 and @EndofToken2 > 0) or (@EndofToken1 > 0 and @EndofToken2 = 0)--seperator is char(13). if an another char(10) is found then thats not following char(13) immediately.
				select @EndofToken = @EndofToken1, @Separators = char(13)
		else if (@EndofToken2 < @EndofToken1 - 1 and @EndofToken2 > 0 and @EndofToken1 > 0) or (@EndofToken2 > 0 and @EndofToken1 = 0)--seperator = char(10)
				select @EndofToken = @EndofToken2, @Separators = char(10)
	else
		select @EndofToken = 0
end
else
	Select @EndofToken=isnull(CharIndex(@Separators, @SourceString),0)


if @EndofToken > 0 and @EndofToken<= @MaxTokenLength
	Select @Token= IsNull(Substring(@SourceString, 1, @EndofToken-1),space(@ReturnSpaceifNull)),
		@SourceString=Substring(@SourceString, @EndofToken+Datalength(@Separators), 2000)
else
	Select @Token=Substring(@SourceString,1, @MaxTokenLength),
		@SourceString=SubString(@SourceString, @MaxTokenLength+1, 2000)
return 0