using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication3.Services;

namespace WebApplication3.ArgumentHandlers
{
	internal class PdfTextHandler
	{
		private readonly TextractTextDetectionService textractTextService;
		public PdfTextHandler(TextractTextDetectionService textractTextService)
		{
			this.textractTextService = textractTextService;
		}
		internal List<dynamic> Handle(string bucketName, string pdfFileName)
		{
			var task = textractTextService.StartDocumentTextDetection(bucketName, pdfFileName);
			var jobId = task.Result;
			textractTextService.WaitForJobCompletion(jobId);
			return textractTextService.Print(textractTextService.GetJobResults(jobId));
		}
	}
}
