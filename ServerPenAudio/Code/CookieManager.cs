using Microsoft.AspNetCore.Http;

namespace ServerPenAudio.Code
{
    public static class CookieManager
    {
        public const string AudioKey = "AudioId";

        public static void AddCookie(HttpResponse response, string cookieValue)
        {
            response.Cookies.Append(
                AudioKey,
                cookieValue,
                new CookieOptions()
                {
                    Path = "/",
                });
        }
    }
}