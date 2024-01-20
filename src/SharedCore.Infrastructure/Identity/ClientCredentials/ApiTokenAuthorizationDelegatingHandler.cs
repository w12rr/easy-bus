using System.Net.Http.Headers;
using IdentityModel.Client;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedCore.Abstraction.Services;

namespace SharedCore.Infrastructure.Identity.ClientCredentials;

public class ApiTokenAuthorizationDelegatingHandler : DelegatingHandler
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<ApiTokenAuthorizationDelegatingHandler> _logger;
    private readonly IdentityProviderOptions _isOptions;

    public ApiTokenAuthorizationDelegatingHandler(IHttpClientFactory clientFactory,
        IOptions<IdentityProviderOptions> options,
        IMemoryCache memoryCache,
        ILogger<ApiTokenAuthorizationDelegatingHandler> logger)
    {
        _clientFactory = clientFactory;
        _memoryCache = memoryCache;
        _logger = logger;
        _isOptions = options.Value;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await GetTokenAsync();
        request.Headers.Authorization = new AuthenticationHeaderValue(IdentityConsts.Schema.Bearer, token);

        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<string?> GetTokenAsync()
    {
        if (_memoryCache.TryGetValue(IdentityConsts.ClientCredentials.ApiTokenCacheKey, out string? result))
            return result.AssertNullOrWhiteSpace();

        var tokenData = await QueryApiToken();
        if (tokenData is null) return default;

        using var entry = _memoryCache.CreateEntry(IdentityConsts.ClientCredentials.ApiTokenCacheKey);
        entry.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(tokenData.ExpiresIn).AddSeconds(-2);
        entry.Value = tokenData.AccessToken;

        return tokenData.AccessToken;
    }

    private async Task<TokenResponse?> QueryApiToken()
    {
        var client = _clientFactory.CreateClient();
        var disco = await client.GetDiscoveryDocumentAsync(_isOptions.Authority);
        if (disco.IsError)
        {
            _logger.LogCritical("Got error during getting discovery document: {Error} {ErrorType} {HttpErrorReason}",
                disco.Error, disco.ErrorType, disco.HttpErrorReason);
            return default;
        }

        var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = _isOptions.ClientId,
            ClientSecret = _isOptions.ClientSecret,
            Scope = _isOptions.ClientScope
        });

        if (!tokenResponse.IsError) return tokenResponse;
        
        _logger.LogCritical("Cannot get m2m token: {Error} {ErrorType} {ErrorDescription} {HttpErrorReason}",
            tokenResponse.Error, tokenResponse.ErrorType, tokenResponse.ErrorDescription,
            tokenResponse.HttpErrorReason);
        return default;
    }
}