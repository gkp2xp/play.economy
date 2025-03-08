using System;
using System.Collections.Generic;
using Duende.IdentityServer.Models;

namespace Play.Identity.Service.Settings
{
    public class IdentityServerSettings
    {
        public IReadOnlyCollection<IdentityResource> IdentityResources =>
            new []
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource("roles", new[]{"role"})
            };
    }
}