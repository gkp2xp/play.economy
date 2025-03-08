using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Amazon.Runtime.Internal.Util;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Play.Catalog.Contracts;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Common;
using Play.Common.Settings;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("Upload")]
    public class UploadController : ControllerBase
    {
        private readonly IRepository<FileUpload> fileUploadRepository;
        private readonly ILogger logger;
        private readonly ServiceSettings settings;

        public UploadController(IOptions<ServiceSettings> settings, IRepository<FileUpload> fileUploadRepository, ILogger<UploadController> logger)
        {
            this.settings = settings.Value;
            this.fileUploadRepository = fileUploadRepository;
            this.logger = logger;
        }


        [HttpPost]
        [Authorize(Policies.Write)]
        public async Task<IActionResult> PostAsync(List<IFormFile> files)
        {
            logger.Log(LogLevel.Information, "Upload is called");
            List<FileUpload> uploads = new();

            foreach (var formFile in Request.Form.Files)
            {
                if (formFile.Length > 0)
                {
                    var filePath = Path.Combine(settings.StoredFilesPath, Path.GetRandomFileName().Replace(".", ""));
                    filePath = Path.ChangeExtension(filePath, Path.GetExtension(formFile.FileName));
                    var uri = Path.Combine(settings.PublicPath, Path.GetFileName(filePath)).Replace(Path.DirectorySeparatorChar, '/').Replace(" ", "%20");

                    FileUpload fileUpload = new() { Id = Guid.NewGuid(), Uri = uri, Path = filePath, CreatedDate = DateTime.UtcNow };
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                        await fileUploadRepository.CreateAsync(fileUpload);
                        uploads.Add(fileUpload);
                    }
                }
            }

            return Ok(new
            {
                uploads = uploads.Select(upload => new { fileId = upload.Id, fileUri = upload.Uri })
            });
        }
    }
}