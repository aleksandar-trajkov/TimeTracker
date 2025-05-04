namespace TimeTracker.WebApi.Interfaces;

public interface IMinimalApiEndpointDefinition
{
    IEndpointConventionBuilder Map(IEndpointRouteBuilder app);
}
