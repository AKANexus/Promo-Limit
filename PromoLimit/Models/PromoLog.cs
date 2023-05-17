namespace PromoLimit.Models
{
	public class PromoLog : EntityBase
	{
		public PromoLog(LogSeverity logSeverity, string module, string message)
		{
			LogSeverity = logSeverity;
			Module = module;
			Message = message;
		}

		public LogSeverity LogSeverity { get; set; }
		public string Module { get; set; }
		public string Message { get; set; }
		public DateTime DateTime { get; set; } = DateTime.UtcNow;

		
	}

	public enum LogSeverity
	{
		Debug,
		Info,
		Warning,
		Error,
		Critical
	}
}
