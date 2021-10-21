using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Registry;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IM.Common.API.Extensions
{
    public static class ResiliencePolicies
    {

        private static AsyncRetryPolicy _asyncRetryPolicy;

        private static int _numberOfRetries;

        public static IServiceCollection ConfigurePollyPolicies(this IServiceCollection services)
        {
            PolicyRegistry policyRegistry = new()
            {
                { "SQLAsyncResilienceStrategy", GetAsyncRetryPolicy() },
                { "SQLResilienceStrategy", GetRetryPolicy() }
            };
            services.AddSingleton<IReadOnlyPolicyRegistry<string>>(policyRegistry);
            return services;
        }


        private static AsyncRetryPolicy GetAsyncRetryPolicy()
        {
            _numberOfRetries = 3;
            _asyncRetryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(_numberOfRetries, retryAttempts =>
            {
                var timeToWait = TimeSpan.FromSeconds(Math.Pow(_numberOfRetries, retryAttempts));
                Console.WriteLine($"Waiting {timeToWait.TotalSeconds} seconds");
                return timeToWait;
            });

            return _asyncRetryPolicy;
        }


        private static RetryPolicy GetRetryPolicy()
        {
            return Policy.Handle<Exception>().WaitAndRetry(3, retryAttempts =>
            {
                var timeToWait = TimeSpan.FromSeconds(Math.Pow(_numberOfRetries, retryAttempts));
                Console.WriteLine($"Waiting {timeToWait.TotalSeconds} seconds");
                return timeToWait;
            });
        }
    }
}
