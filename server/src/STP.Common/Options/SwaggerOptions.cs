using System.Collections.Generic;

namespace STP.Common.Options
{
    public class SwaggerOptions
    {
        public string Version { get; set; }

        public string Description { get; set; }

        public string UiEndpoint { get; set; }

        public Dictionary<string, string> SwaggerScopes { get; set; }
    }
}
