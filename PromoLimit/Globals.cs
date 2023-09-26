global using System.Text.Json.Serialization;
global using static PromoLimit.Globals;


namespace PromoLimit
{
    public static class Globals
    {
        public static Task? NotificationTask;
        public static bool Verificando = false;
    }
}