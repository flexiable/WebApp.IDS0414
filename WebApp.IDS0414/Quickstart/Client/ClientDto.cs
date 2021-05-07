namespace WebApp.IDS0414.Controllers.Client
{
    public class ClientDto
    {
        public int id { get; set; }
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientSecrets { get; set; }
        public string Description { get; set; }
        public string AllowedGrantTypes { get; set; }
        public string AllowedScopes { get; set; }
        public string AllowedCorsOrigins { get; set; }
        public string RedirectUris { get; set; }
        public string PostLogoutRedirectUris { get; set; }
        public bool Enabled
        { get; set; }


        public int? AccessTokenLifetime { get; set; }
    }
}