using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace ApiGateway.GraphQL
{
    public class ApiGatewaySchema : Schema
    {
        public ApiGatewaySchema(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Query = serviceProvider.GetRequiredService<Query>();
            Mutation = serviceProvider.GetRequiredService<Mutation>();
        }
    }
}