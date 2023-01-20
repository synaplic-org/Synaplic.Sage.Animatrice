using Uni.Scan.Application.Interfaces.Services;
using System;

namespace Uni.Scan.Infrastructure.Services.Date
{
    public class SystemDateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;
    }
}