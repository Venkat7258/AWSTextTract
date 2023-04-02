using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication3.Services;

namespace WebApplication3.ArgumentHandlers
{
	internal class DetectTextS3Handler
	{
		private readonly TextractTextDetectionService textractTextService;

		public DetectTextS3Handler(TextractTextDetectionService textractTextService)
		{
			this.textractTextService = textractTextService;
		}
        internal List<dynamic> Handle(string bucketName, string fileName)
        {
            var s3Task = textractTextService.DetectTextS3(bucketName, fileName);
            s3Task.Wait();
            return textractTextService.Print(s3Task.Result);
        }
    }
}
