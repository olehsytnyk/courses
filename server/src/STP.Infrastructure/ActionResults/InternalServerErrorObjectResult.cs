using Microsoft.AspNetCore.Mvc;
using STP.Common.Models;
using System.Net;
using System.Threading.Tasks;

namespace STP.Infrastructure.ActionResults
{
    public class InternalServerErrorObjectResult : ObjectResult
    {
        public InternalServerErrorObjectResult(object error) : base(error)
        {
            StatusCode = (int)HttpStatusCode.InternalServerError;
        }
    }
}
