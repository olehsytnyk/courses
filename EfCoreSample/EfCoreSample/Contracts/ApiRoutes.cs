using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EfCoreSample.Contracts
{
    public static class ApiRoutes
    {
        public const string Root = "api";

        public const string Version = "v1";

        public const string Base = Root + "/" + Version;

        public static class Posts
        {
            public const string GetAll = Base + "/Viev";

            public const string Update = Base + "/Update/{ById}";

            public const string Delete = Base + "/Delete/{ById}";

            public const string Get = Base + "/Get/{ById}";

            public const string Create = Base + "/Create";
        }

    }
}
