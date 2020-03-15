using Microsoft.AspNetCore.Mvc;
using STP.Common.Models;
using System.Net;
using System.Threading.Tasks;

namespace STP.Infrastructure.ActionResults
{
    public class ForbiddenObjectResult : ObjectResult
    {   
        public ForbiddenObjectResult(object error) : base(error)
        {
            StatusCode = (int)HttpStatusCode.Forbidden;
        }
    }
}
