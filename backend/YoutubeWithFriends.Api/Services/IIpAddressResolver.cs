using Microsoft.AspNetCore.Http;

namespace YoutubeWithFriends.Api.Services {
    public interface IIpAddressResolver {
        string GetIpAddress(HttpRequest httpRequest);
    }
}