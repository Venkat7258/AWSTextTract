
using System;
using System.Collections.Generic;
using WebApplication3.Services;

namespace WebApplication3.ArgumentHandlers
{
	internal class DetectTextHandler
	{
		private readonly TextractTextDetectionService textractTextService;

		public DetectTextHandler(TextractTextDetectionService textractTextService)
		{
			this.textractTextService = textractTextService;
		}

		internal List<dynamic> Handle(string localFile)
		{
			var localTask = textractTextService.DetectTextLocal(localFile);
			localTask.Wait();
		return	textractTextService.Print(localTask.Result);
		}
	}
}
