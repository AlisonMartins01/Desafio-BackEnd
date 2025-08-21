using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Rentals.Application.Abstractions.Messaging
{
    public sealed class LoggingBehavior<TReq, TRes> : IPipelineBehavior<TReq, TRes>
    {
        private readonly ILogger<LoggingBehavior<TReq, TRes>> _log;
        public LoggingBehavior(ILogger<LoggingBehavior<TReq, TRes>> log) => _log = log;

        public async Task<TRes> Handle(TReq request, RequestHandlerDelegate<TRes> next, CancellationToken ct)
        {
            var sw = Stopwatch.StartNew();
            _log.LogInformation("Handling {RequestName} {@Request}", typeof(TReq).Name, request);
            try
            {
                var response = await next();
                sw.Stop();
                _log.LogInformation("Handled {RequestName} in {Elapsed} ms", typeof(TReq).Name, sw.ElapsedMilliseconds);
                return response;
            }
            catch (Exception ex)
            {
                sw.Stop();
                _log.LogError(ex, "Error on {RequestName} after {Elapsed} ms", typeof(TReq).Name, sw.ElapsedMilliseconds);
                throw;
            }
        }
    }
}
