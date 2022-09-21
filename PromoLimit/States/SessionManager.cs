using PromoLimit.Services;

namespace PromoLimit.States
{
	public class SessionManager
	{
        private static readonly JsonWebToken JsonWebToken = new();
        public static bool IsSignedIn(HttpContext context)
        {
            if (context.Request.Cookies["apiKey"] is not null)
                JsonWebToken.TryDecodeToken(context.Request.Cookies["apiKey"]!, out ApiKeyInfo aaa);
            if (context.Session.GetString("uuid") is null)
            {
                var apiKey = context.Request.Cookies["apiKey"];
                if (apiKey is null)
                {
                    return false;
                }

                if (!JsonWebToken.TryDecodeToken(context.Request.Cookies["apiKey"]!, out ApiKeyInfo decodedToken))
                {
                    return false;
                }

                if (context.Session.GetString("uuid") is null)
                {
                    if (decodedToken.displayName is null)
                    {
                        //throw new Exception();
                    }
                    context.Session.SetString("uuid", decodedToken.uuid);
                    //context.Session.SetInt32("gerencia", Convert.ToInt32(decodedToken.gerencia));
                    //context.Session.SetInt32("setorId", decodedToken.setorId);
                    context.Session.SetString("displayName", decodedToken.displayName ?? "Abelardo");
                    return true;
                }

                return true;
            }
            else
            {
                return true;
            }
        }

        public static string GetLoggedUserName(HttpContext context)
        {
            return context.Session.GetString("displayName") ?? "Genovêva";
        }
	}

    public class ApiKeyInfo
    {
        public string uuid { get; set; }
        //public int setorId { get; set; }
        //public bool gerencia { get; set; }
        public string? displayName { get; set; }
    }
}
