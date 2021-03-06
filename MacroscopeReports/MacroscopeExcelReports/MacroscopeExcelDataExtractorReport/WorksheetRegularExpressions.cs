﻿/*

  This file is part of SEOMacroscope.

  Copyright 2017 Jason Holland.

  The GitHub repository may be found at:

    https://github.com/nazuke/SEOMacroscope

  Foobar is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  Foobar is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with Foobar.  If not, see <http://www.gnu.org/licenses/>.

*/

using System;
using ClosedXML.Excel;
using System.Collections.Generic;
using System.Net;

namespace SEOMacroscope
{

  public partial class MacroscopeExcelDataExtractorReport : MacroscopeExcelReports
  {

    /**************************************************************************/

    private void BuildWorksheetRegularExpressions (
      MacroscopeJobMaster JobMaster,
      XLWorkbook wb,
      string WorksheetLabel
    )
    {

      var ws = wb.Worksheets.Add( WorksheetLabel );

      int iRow = 1;
      int iCol = 1;
      int iColMax = 1;
      
      MacroscopeDocumentCollection DocCollection = JobMaster.GetDocCollection();
      MacroscopeAllowedHosts AllowedHosts = JobMaster.GetAllowedHosts();

      {

        ws.Cell( iRow, iCol ).Value = MacroscopeConstants.Url;
        iCol++;

        ws.Cell( iRow, iCol ).Value = MacroscopeConstants.StatusCode;
        iCol++;

        ws.Cell( iRow, iCol ).Value = MacroscopeConstants.Status;
        iCol++;

        ws.Cell( iRow, iCol ).Value = MacroscopeConstants.ContentType;
        iCol++;
        
        ws.Cell( iRow, iCol ).Value = "Extracted Label";
        iCol++;

        ws.Cell( iRow, iCol ).Value = "Extracted Value";

      }

      iColMax = iCol;

      iRow++;

      foreach( string Url in DocCollection.DocumentKeys() )
      {

        MacroscopeDocument msDoc = DocCollection.GetDocument( Url );
        string DocUrl = msDoc.GetUrl();
        string StatusCode = ( ( int )msDoc.GetStatusCode() ).ToString();
        string Status = msDoc.GetStatusCode().ToString();
        string MimeType = msDoc.GetMimeType();
        
        if( !this.DataExtractorRegexes.CanApplyDataExtractorsToDocument( msDoc: msDoc ) )
        {
          continue;
        }        
                
        foreach( KeyValuePair<string,string> DataExtractedPair in msDoc.IterateDataExtractedRegexes() )
        {

          string ExtractedLabel = DataExtractedPair.Key;
          string ExtractedValue = DataExtractedPair.Value;

          if( 
            string.IsNullOrEmpty( ExtractedLabel )
            || string.IsNullOrEmpty( ExtractedValue ) )
          {
            continue;
          }

          iCol = 1;

          this.InsertAndFormatUrlCell( ws, iRow, iCol, msDoc );

          if( msDoc.GetIsInternal() )
          {
            ws.Cell( iRow, iCol ).Style.Font.SetFontColor( XLColor.Green );
          }
          else
          {
            ws.Cell( iRow, iCol ).Style.Font.SetFontColor( XLColor.Gray );
          }

          iCol++;

          this.InsertAndFormatStatusCodeCell( ws, iRow, iCol, msDoc );

          iCol++;

          this.InsertAndFormatContentCell( ws, iRow, iCol, this.FormatIfMissing( Status ) );

          iCol++;

          this.InsertAndFormatContentCell( ws, iRow, iCol, this.FormatIfMissing( MimeType ) );

          iCol++;
          
          this.InsertAndFormatContentCell( ws, iRow, iCol, this.FormatIfMissing( ExtractedLabel ) );

          iCol++;
        
          this.InsertAndFormatContentCell( ws, iRow, iCol, this.FormatIfMissing( ExtractedValue ) );

          iRow++;
          
        }
       
      }

      {
        var rangeData = ws.Range( 1, 1, iRow - 1, iColMax );
        var excelTable = rangeData.CreateTable();
      }

    }

    /**************************************************************************/

  }

}
