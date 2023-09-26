using Microsoft.AspNetCore.SignalR;
using Npgsql;
using System.Diagnostics;

namespace PromoLimit.Services
{
    public class PostgresNotificationService
    {
        private readonly NpgsqlConnectionStringBuilder _csb;

        //private readonly IHubContext<ChatHub> _chatHub;


        public PostgresNotificationService(IServiceProvider provider, IConfiguration configuration)
        {
            //_chatHub = chatHubContext;
            _csb = new NpgsqlConnectionStringBuilder
            {
                Database = "meliEspelho",
                Port = 5351,
                Username = "meliDBA",
                Password = configuration["SuperSecretSettings:NpgPassword"],
                Host = "192.168.10.215"
            };
        }

        public event EventHandler<NpgsqlNotificationEventArgs> NotificationReceived;

        public async Task MonitorForNotification()
        {
            await using var conn = new NpgsqlConnection(_csb.ConnectionString);
            try
            {
                await conn.OpenAsync();
            }
            catch (Exception e)
            {
                Debugger.Break();
            }

            //e.Payload is string representation of JSON we constructed in NotifyOnDataChange() function
            conn.Notification += async (_, e) =>
            {
                //await _chatHub.Clients.All.SendAsync("PostNotification", "Server", e.Payload);
                 NotificationReceived?.Invoke(this, e);
            };

            await using (var cmd = new NpgsqlCommand("LISTEN dbnotification;", conn))
                cmd.ExecuteNonQuery();

            while (true)
                await conn.WaitAsync(); // wait for events

            Debug.WriteLine("Ended");
        }
    }
}
