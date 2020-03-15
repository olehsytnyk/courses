using FluentValidation;
using Microsoft.AspNetCore.Http;
using STP.Common.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace STP.Infrastructure.Validation
{

    public class UploadFileValidator : AbstractValidator<UploadFileDTO>
    {
        public UploadFileValidator()
        {
            RuleFor(p => p.FormFile)
                .NotEmpty()
                .Must(ValidateFileSize)
                .WithMessage("File cannot be bigger than 5 Mb.");

            RuleFor(p => Path.GetExtension(p.FormFile.FileName))
                .NotEmpty()
                .Matches(@"(.jpeg|.JPEG|.jpg|.JPG|.png|.PNG|.bmp|.BMP|.gif|.GIF|.ico|.ICO)$")
                .WithMessage("Bad file format.");
        }

        private bool ValidateFileSize(IFormFile file)
        {
            return (file.Length <= 5120000);
        }
    }
}