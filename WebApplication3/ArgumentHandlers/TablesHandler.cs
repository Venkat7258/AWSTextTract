using System;
using System.Collections.Generic;
using Amazon.Textract.Model;
using WebApplication3.Model;
using WebApplication3.Services;

namespace WebApplication3.ArgHandlers
{
	internal class TablesHandler
	{
		private readonly TextractTextAnalysisService textractAnalysisService;

		public TablesHandler(TextractTextAnalysisService textractAnalysisService)
		{
			this.textractAnalysisService = textractAnalysisService;
		}

		internal List<dynamic> Handle(string bucketName, string formFile)
		{
			var task = textractAnalysisService.StartDocumentAnalysis(bucketName, formFile, "TABLES");
			var jobId = task.Result;
			textractAnalysisService.WaitForJobCompletion(jobId);
			var results = textractAnalysisService.GetJobResults(jobId);
			var document = new TextractDocument(results);
			List<dynamic> objlist = new List<dynamic>();
			document.Pages.ForEach(page => {
				page.Tables.ForEach(table => {
					var r = 0;
					table.Rows.ForEach(row => {
						r++;
						var c = 0;
						row.Cells.ForEach(cell => {
							c++;
							objlist.Add(new {Row =r,Column=c,CellValue= cell.Text }); 
						});
					});
				});
			});
			return objlist;
		}
	}
}