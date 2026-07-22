using System.Reflection;
using Domain.Users;
using Infrastructure.Database;
using SharedKernel.Abstractions.Messaging;

namespace ArchitectureTests;

public abstract class BaseTest
{
    protected static readonly Assembly DomainAssembly = typeof(User).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(ICommand).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(ApplicationDbContext).Assembly;
    protected static readonly Assembly DataProcessorPresentationAssembly = typeof(DataProcessor.Api.DependencyInjection).Assembly;
    protected static readonly Assembly PresentationAssembly = typeof(Web.Api.DependencyInjection).Assembly;
}
