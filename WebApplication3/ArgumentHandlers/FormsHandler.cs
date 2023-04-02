using System;
using System.Collections.Generic;
using Amazon.Textract.Model;
using WebApplication3.Model;
using WebApplication3.Services;

namespace WebApplication3.ArgHandlers
{
	internal class FormsHandler
	{
		private readonly TextractTextAnalysisService textractAnalysisService;
		public FormsHandler(TextractTextAnalysisService textractAnalysisService)
		{
			this.textractAnalysisService = textractAnalysisService;
		}

		internal List<dynamic> Handle(string bucketName, string formFile)
		{
			var task = textractAnalysisService.StartDocumentAnalysis(bucketName, formFile, "FORMS");
			var jobId = task.Result;
			textractAnalysisService.WaitForJobCompletion(jobId);
			var results = textractAnalysisService.GetJobResults(jobId);
			var document = new TextractDocument(results);
			List<dynamic> objlist = new List<dynamic>();
			document.Pages.ForEach(page => {
				page.Form.Fields.ForEach(f => {
					objlist.Add(new { FieldKey = f.Key.Text, FieldValue = f.Value.Text });
					Console.WriteLine("Key: {0}, Value {1}", f.Key, f.Value);
				});
				Console.WriteLine("Get Field by Key:");
				var key = "Phone Number:";
				var field = page.Form.GetFieldByKey(key);
				if (field != null)
				{
					//objlist.Add(new { FieldKey = field.Key.Text, FieldValue = field.Value.Text });
				}
			});
			return objlist;
		}
	}
}