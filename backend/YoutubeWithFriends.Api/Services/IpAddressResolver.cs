using System.Linq;

using Microsoft.AspNetCore.Http;

namespace YoutubeWithFriends.Api.Services {
    public class IpAddressResolver : IIpAddressResolver {
        public string GetIpAddress(HttpRequest httpRequest) {
            if (httpRequest.Headers["HTTP_X_FORWARDED_FOR"].FirstOrDefault() is string forwardedAddress) {
                return forwardedAddress;
            }

            if (httpRequest.Headers["X-Real-IP"].FirstOrDefault() is string realIpAddress) {
                return realIpAddress;
            }

            if (httpRequest.Headers["REMOTE_ADDR"].FirstOrDefault() is string remoteAddress) {
                return remoteAddress;
            }

            return httpRequest.HttpContext.Connection.RemoteIpAddress.ToString();
        }
    }
}
