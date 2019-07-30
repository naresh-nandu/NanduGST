 CREATE PROC [usp_GetTotalGSTR1DataByGstnId]   
 @TRPId INT=1   
 AS    
 BEGIN    
  --Inserting CustomerId into @CustomerId table variable  
  DECLARE @CustomerIds Table  
  (  
   Custid int  
  )  
  INSERT INTO @CustomerIds  
  SELECT  
       custid   
  FROM TBL_Customer  
  WHERE   
  TRPId = @TRPId   
  AND  
  rowstatus =1   
    
  
 --Inserting the GSTIN details into @GSTIN table variable  
  DECLARE @GSTINID TABLE  
  (  
  GSTINId INT  
  )  
  INSERT INTO @GSTINID  
  SELECT   
    --    CG.GSTINNo  
    --,CG.CustId  
    --,  
    CG.GSTINId  
    --,CG.PANNo  
    FROM     
     TBL_Cust_GSTIN CG   
  INNER JOIN    
        UserAccess_GSTIN UG  ON CG.GSTINId = UG .GSTINId    
    WHERE  
     UG .Rowstatus=1     
        AND  
  CG.Rowstatus =1     
        AND   
  UG.custid in (Select custid from @CustomerIds)    
     
   -- Adding B2b, B2Cl,B2Cs,EXP for output tax liability.   
   DECLARE @SUM_OF_GSTR1_ATTRIBUTE_ONE TABLE  
   (  
    gstinid INT,  
 OIGST DECIMAL(18,2),  
 OCGST DECIMAL(18,2),  
 OSGST DECIMAL(18,2),  
 OCESS DECIMAL(18,2)  
   )   
  
   INSERT INTO @SUM_OF_GSTR1_ATTRIBUTE_ONE  
   SELECT     
     ONE.gstinid    
    ,sum(OIGST)as OIGST    
    ,sum(OCGST)as OCGST    
    ,sum(OSGST) as OSGST    
    ,sum(OCESS)as OCESS  
 FROM  
 (  
  --B2B     
   SELECT     
     t3.gstinid    
    ,sum(iamt)as OIGST    
    ,sum(camt)as OCGST    
    ,sum(samt) as OSGST    
    ,sum(csamt)as OCESS        
   FROM TBL_GSTR1 t1      
       Inner Join TBL_GSTR1_B2B t2 On t2.gstr1id = t1.gstr1id      
       Inner Join TBL_GSTR1_B2B_INV t3 On t3.b2bid = t2.b2bid      
       Inner Join TBL_GSTR1_B2B_INV_ITMS t4 On t4.invid = t3.invid      
       Inner Join TBL_GSTR1_B2B_INV_ITMS_DET t5 On t5.itmsid = t4.itmsid     
   WHERE  
       t3.gstinid IN (SELECT GSTINId FROM @GSTINID)   
   GROUP BY t3.gstinid     
       UNION ALL  
   --B2CL    
   SELECT     
        t3.gstinid    
       ,sum(iamt)as OIGST    
       ,0 as OCGST    
       ,0 as OSGST    
       ,sum(csamt)as OCESS        
   FROM TBL_GSTR1 t1      
           Inner Join TBL_GSTR1_B2CL t2 On t2.gstr1id = t1.gstr1id      
           Inner Join TBL_GSTR1_B2CL_INV t3 On t3.b2clid = t2.b2clid      
           Inner Join TBL_GSTR1_B2CL_INV_ITMS t4 On t4.invid = t3.invid      
           Inner Join TBL_GSTR1_B2CL_INV_ITMS_DET t5 On t5.itmsid = t4.itmsid    
   WHERE  
           t3.gstinid IN (SELECT GSTINId FROM @GSTINID)       
   GROUP BY t3.gstinid     
        UNION ALL  
  --B2CS    
  SELECT     
        t2.gstinid    
       ,sum(iamt)as OIGST    
       ,sum(camt)as OCGST    
       ,sum(samt) as OSGST    
       ,sum(csamt)as OCESS        
   FROM TBL_GSTR1 t1      
           Inner Join     
           TBL_GSTR1_B2CS t2 On t2.gstr1id = t1.gstr1id      
 WHERE  
            t2.gstinid IN (SELECT GSTINId FROM @GSTINID)   
    GROUP BY t2.gstinid      
        UNION ALL  
 --EXP    
  SELECT    
       t3.gstinid    
      ,sum(iamt)as OIGST    
      ,0 as OCGST    
      ,0 as OSGST    
      ,0 as OCESS        
   FROM TBL_GSTR1 t1      
           Inner Join   
     TBL_GSTR1_EXP t2 On t2.gstr1id = t1.gstr1id      
           Inner Join TBL_GSTR1_EXP_INV t3 On t3.expid = t2.expid      
           Inner Join TBL_GSTR1_EXP_INV_ITMS t4 On t4.invid = t3.invid      
 WHERE  
       t3.gstinid IN (SELECT GSTINId FROM @GSTINID)   
    GROUP BY t3.gstinid     
 ) ONE  
 GROUP BY ONE.gstinid  
  
   
  
 DECLARE @SUM_OF_GSTR1_ATTRIBUTE_TWO TABLE  
   (  
    gstinid INT,  
 ntty CHAR,  
 OIGST DECIMAL(18,2),  
 OCGST DECIMAL(18,2),  
 OSGST DECIMAL(18,2),  
 OCESS DECIMAL(18,2)  
   )    
   INSERT INTO @SUM_OF_GSTR1_ATTRIBUTE_TWO  
  --CDNR  
  SELECT   
          A.gstinid  
   ,A.ntty  
   ,sum(OIGST)as OIGST  
   ,sum(OCGST)as OCGST  
   ,sum(OSGST) as OSGST  
   ,sum(OCESS)as OCESS   
   FROM  
   (  
   SELECT   
          t3.gstinid  
   ,t3.ntty  
   ,sum(iamt)as OIGST  
   ,sum(camt)as OCGST  
   ,sum(samt) as OSGST  
   ,sum(csamt)as OCESS      
    FROM   
        TBL_GSTR1 t1    
        Inner Join TBL_GSTR1_CDNR t2 On t2.gstr1id = t1.gstr1id    
        Inner Join TBL_GSTR1_CDNR_NT t3 On t3.cdnrid = t2.cdnrid    
        Inner Join TBL_GSTR1_CDNR_NT_ITMS t4 On t4.ntid = t3.ntid    
        Inner Join TBL_GSTR1_CDNR_NT_ITMS_DET t5 On t5.itmsid = t4.itmsid    
   WHERE t3.gstinid in (SELECT GSTINId FROM @GSTINID)    
   GROUP BY t3.gstinid,t3.ntty   
       UNION ALL  
   SELECT  
         t3.gstinid  
  ,t2.ntty  
  ,sum(iamt)as OIGST  
  ,sum(camt)as OCGST  
  ,sum(samt) as OSGST  
  ,sum(csamt)as OCESS      
        from TBL_GSTR1 t1    
        Inner Join TBL_GSTR1_CDNUR t2 On t2.gstr1id = t1.gstr1id    
        Inner Join TBL_GSTR1_CDNUR_ITMS t3 On t3.cdnurid = t2.cdnurid    
        Inner Join TBL_GSTR1_CDNUR_ITMS_DET t4 On t4.itmsid = t3.itmsid    
    WHERE   
      t3.gstinid in (SELECT GSTINId FROM @GSTINID)    
 GROUP BY  
      t3.gstinid,t2.ntty  
    )A  
 GROUP BY  
 A.gstinid  
 , A.ntty  
  
 --SELECT   
 --*  
 --FROM  
 --@SUM_OF_GSTR1_ATTRIBUTE_ONE   
   
  
 Update  
      @SUM_OF_GSTR1_ATTRIBUTE_ONE     
    SET   
      t1.OIGST =  t1.OIGST - t2.OIGST,     
         t1.OCGST =  t1.OCGST - t2.OCGST,    
         t1.OSGST =  t1.OSGST - t2.OSGST,    
         t1.OCESS =  t1.OCESS - t2.OCESS    
    FROM  
      @SUM_OF_GSTR1_ATTRIBUTE_ONE t1    
   INNER JOIN   
         @SUM_OF_GSTR1_ATTRIBUTE_TWO t2 ON t1.gstinid = t2.gstinid AND t2.ntty='C'   
    WHERE   
          t2.ntty = 'C'  
     
 Update  
      @SUM_OF_GSTR1_ATTRIBUTE_ONE     
    SET   
      t1.OIGST =  t1.OIGST + t2.OIGST,     
         t1.OCGST =  t1.OCGST + t2.OCGST,    
         t1.OSGST =  t1.OSGST + t2.OSGST,    
         t1.OCESS =  t1.OCESS + t2.OCESS    
    FROM  
      @SUM_OF_GSTR1_ATTRIBUTE_ONE t1    
   INNER JOIN   
         @SUM_OF_GSTR1_ATTRIBUTE_TWO t2 ON t1.gstinid = t2.gstinid AND t2.ntty='D'   
    WHERE   
          t2.ntty = 'D'  
     
 SELECT gstinid   
     ,OIGST   
     ,OCGST   
     ,OSGST   
     ,OCESS   
  FROM @SUM_OF_GSTR1_ATTRIBUTE_ONE  
  
   --SELECT   
   --*   
   --FROM  
   --@SUM_OF_GSTR1_ATTRIBUTE_TWO   
  
    --select * from TBL_Customer where rowstatus=1    
    --select * from TBL_Cust_GSTIN    
  
  
     
  
 END