using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Amazon.S3;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Okta.Blog.FileStreams.Controllers
{
    public class FileController : ControllerBase
    {
        IAmazonS3 _s3Client;
        public FileController(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        [Authorize]
        public async Task Index()
        {
            //Our domain specific security logic would go here
            var emailClaim = User.Claims.FirstOrDefault(x => x.Type == "email");
            var email = emailClaim.Value;

            if (email == "{some email}")
            {
                using (var fileStream = await _s3Client.GetObjectStreamAsync("{your bucket}", "{your file}", null))
                {
                    this.Response.ContentType = "application/json";
                    this.Response.Headers.Add("Content-Disposition", "attachment; filename=\"{your file}\"");
                    this.Response.StatusCode = 200;
                    await fileStream.CopyToAsync(this.Response.Body);
                }
            }
        }
    }
}