using EfCoreSample.Doman.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EfCoreSample.Contracts.V1.Requests
{
    public class CreatePostRequest
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Discription { get; set; }

        public StatusType Status { get; set; }
    }
}
