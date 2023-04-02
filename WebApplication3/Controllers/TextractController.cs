using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Textract;
using Amazon.Textract.Model;
using Amazon.S3.Model;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using WebApplication3.ArgumentHandlers;
using WebApplication3.Services;
using System.IO;
using WebApplication3.ArgHandlers;

namespace WebApplication3.Controllers
{
    public class TextractController : ControllerBase
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IAmazonTextract _textract;
        private IWebHostEnvironment _hostEnvironment;
        public TextractTextDetectionService _textractTextDetectionService;
        public TextractTextAnalysisService _textractTextAnalysisService;
        public TextractController(IAmazonS3 s3Client, IAmazonTextract textract, IWebHostEnvironment hostEnvironment)
        {
            _s3Client = s3Client;
            _textract = textract;
            _hostEnvironment = hostEnvironment;
            _textractTextDetectionService = new TextractTextDetectionService(textract);
            _textractTextAnalysisService = new TextractTextAnalysisService(textract);
        }

        [HttpGet("GetAmazonTextract")]
        public async Task<IActionResult> GetAmazonTextract(string textractType, string bucketName, string fileName)
        {

            switch (textractType)
            {
                case "dtl":
                    return Ok(new DetectTextHandler(_textractTextDetectionService).Handle(Path.Combine(_hostEnvironment.ContentRootPath, "Files\\" + fileName)));
                case "dts3":
                    return Ok(new DetectTextS3Handler(_textractTextDetectionService).Handle(bucketName, fileName));
                case "pdft":
                 return Ok(new PdfTextHandler(_textractTextDetectionService).Handle(bucketName, fileName));
                case "forms":
                 return Ok(new FormsHandler(_textractTextAnalysisService).Handle(bucketName, fileName));
                case "tables":
                    return Ok(new TablesHandler(_textractTextAnalysisService).Handle(bucketName, fileName));

            }
            return Ok();
        }


        [HttpGet("getTextract")]
        public async Task<IActionResult> GetAllAmazonTextract(string bucketName, string key)
        {
            //var bucketExists = await _s3Client.DoesS3BucketExistAsync(bucketName);
            //if (!bucketExists) return NotFound($"Bucket {bucketName} does not exist.");
            //var s3Object = await _s3Client.GetObjectAsync(bucketName, key);
            var startResponse = await _textract.StartDocumentTextDetectionAsync(new StartDocumentTextDetectionRequest
            {
                DocumentLocation = new DocumentLocation
                {
                    S3Object = new Amazon.Textract.Model.S3Object
                    {
                        Bucket = bucketName,
                        Name = key
                    }
                }
            });
            var getDetectionRequest = new GetDocumentTextDetectionRequest
            {
                JobId = startResponse.JobId
            };

            Console.WriteLine("Poll for detect job to complete");
            // Poll till job is no longer in progress.
            GetDocumentTextDetectionResponse getDetectionResponse = null;
            do
            {
                Thread.Sleep(1000);
                getDetectionResponse = await _textract.GetDocumentTextDetectionAsync(getDetectionRequest);
            } while (getDetectionResponse.JobStatus == JobStatus.IN_PROGRESS);

            Console.WriteLine("Print out results if the job was successful.");
            // If the job was successful loop through the pages of results and print the detected text
            if (getDetectionResponse.JobStatus == JobStatus.SUCCEEDED)
            {
                do
                {
                    foreach (var block in getDetectionResponse.Blocks)
                    {
                        Console.WriteLine($"Type {block.BlockType}, Text: {block.Text}");
                    }

                    // Check to see if there are no more pages of data. If no then break.
                    if (string.IsNullOrEmpty(getDetectionResponse.NextToken))
                    {
                        break;
                    }

                    getDetectionRequest.NextToken = getDetectionResponse.NextToken;
                    getDetectionResponse = await _textract.GetDocumentTextDetectionAsync(getDetectionRequest);

                } while (!string.IsNullOrEmpty(getDetectionResponse.NextToken));
            }
            else
            {
                Console.WriteLine($"Job failed with message: {getDetectionResponse.StatusMessage}");
            }
            return Ok(getDetectionResponse.Blocks);
        }

    }
}
