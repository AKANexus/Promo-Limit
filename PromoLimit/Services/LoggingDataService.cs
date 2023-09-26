using PromoLimit.Contexts;
using PromoLimit.Models;

namespace PromoLimit.Services
{
	public class LoggingDataService
	{
		private readonly ILogger<LoggingDataService> _logger;
		private readonly IServiceProvider _provider;

		public LoggingDataService(ILogger<LoggingDataService> logger, IServiceProvider provider)
		{
			_logger = logger;
			_provider = provider;
		}

		public async Task LogTrace(string thisIsATrace, string module)
		{
			using var scope = _provider.CreateScope();
			_logger.LogTrace(thisIsATrace);
			scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>().PromoLogs.Add(new(LogSeverity.Info, module, thisIsATrace));
			await scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>().SaveChangesAsync();
		}

		public async Task LogDebug(string thisIsADebug, string module)
		{
			using var scope = _provider.CreateScope();
			_logger.LogDebug(thisIsADebug);
			scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>().PromoLogs.Add(new(LogSeverity.Debug, module, thisIsADebug));
			await scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>().SaveChangesAsync();
		}

		public async Task LogError(string thisIsAnError, string module)
		{
			using var scope = _provider.CreateScope();
			_logger.LogError(thisIsAnError);
			scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>().PromoLogs.Add(new(LogSeverity.Error, module, thisIsAnError));
			await scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>().SaveChangesAsync();
		}

		public async Task LogInformation(string thisIsAnInformation, string module)
		{
			using var scope = _provider.CreateScope();
			_logger.LogInformation(thisIsAnInformation);
			scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>().PromoLogs.Add(new(LogSeverity.Info, module, thisIsAnInformation));
			await scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>().SaveChangesAsync();
		}

		public async Task LogCritical(string thisWouldBeACritical, string module)
		{
			using var scope = _provider.CreateScope();
			_logger.LogCritical(thisWouldBeACritical);
			scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>().PromoLogs.Add(new(LogSeverity.Critical, module, thisWouldBeACritical));
			await scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>().SaveChangesAsync();
		}

		public async Task LogWarning(string thisIsAWarning, string module)
		{
			using var scope = _provider.CreateScope();
			_logger.LogWarning(thisIsAWarning);
			scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>().PromoLogs.Add(new(LogSeverity.Warning, module, thisIsAWarning));
			await scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>().SaveChangesAsync();
		}
	}
}
