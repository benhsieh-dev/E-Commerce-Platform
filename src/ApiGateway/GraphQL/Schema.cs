using GraphQL.Types;

namespace ApiGateway.GraphQL
{
    public class ApiGatewaySchema : GraphQL.Types.Schema
    {
        public ApiGatewaySchema(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Query = serviceProvider.GetRequiredService<Query>();
            Mutation = serviceProvider.GetRequiredService<Mutation>();
        }
    }
}