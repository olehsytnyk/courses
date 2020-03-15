using STP.Markets.Domain.Entities;
using STP.Markets.MarketManagerService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using STP.Common.Exceptions;
using STP.Interfaces.Enums;
using STP.Markets.Application.Enums;

namespace STP.Markets.Application {
    public class WatchlistPutDto {
        public PutAction Action { get; set; }
        public int Id { get; set; }
    }
}
