namespace TimeTracker.WebApi.Interfaces;

public interface IEndpointDefinition
{
    IEndpointConventionBuilder Map(IEndpointRouteBuilder app);
}
