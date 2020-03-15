using STP.Markets.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Markets.Application {
    public class WatchlistDto : WatchlistPostDto {
        public long Id { get; set; }
    }
}
