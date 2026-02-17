using OpenFga.Sdk.Client;
using OpenFga.Sdk.Configuration;
using OpenFga.Sdk.Client.Model;

public class Auth0FgaService
{
    private readonly OpenFgaClient fgaClient;
    public Auth0FgaService(IConfiguration configuration)
    {
        var config = new ClientConfiguration()
        {
            ApiUrl = configuration["Auth0Fga:ApiUrl"],
            StoreId = configuration["Auth0Fga:StoreId"],
            Credentials = new Credentials()
            {
                Method = CredentialsMethod.ClientCredentials,
                Config = new CredentialsConfig()
                {
                    ApiTokenIssuer = configuration["Auth0Fga:ApiTokenIssuer"],
                    ApiAudience = configuration["Auth0Fga:ApiAudience"],
                    ClientId = configuration["Auth0Fga:ClientId"],
                    ClientSecret = configuration["Auth0Fga:ClientSecret"],
                }
            }
        };

        fgaClient = new OpenFgaClient(config);
    }

    public async Task<IEnumerable<string>> GetAuthorizedDocumentsAsync(string userId)
    {
        var response = await fgaClient.Read(new ClientReadRequest
        {
            User = $"user:{userId}",
            Relation = "can_read",
            Object = "document:"
        });

        return response.Tuples
            .Select(t => t.Key.Object.Replace("document:", ""))
            .ToList();
    }
}