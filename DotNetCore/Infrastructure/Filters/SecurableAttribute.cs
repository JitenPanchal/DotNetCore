using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore.Infrastructure.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SecurableAttribute : Attribute
    {
        public SecurableAttribute(string securableKey, string securable)
        {
            SecurableKey = securableKey;
            Securable = securable;
        }

        public string SecurableKey { get; private set; }

        // Friendly permission/securable name for use in UI
        public string Securable { get; private set; }
    }
}
