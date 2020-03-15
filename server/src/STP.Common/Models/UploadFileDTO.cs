using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace STP.Common.Models
{
    public class UploadFileDTO
    {
        public IFormFile FormFile { get; set; }
    }

}
