namespace E_commerce.Core.Tests.Common;

internal static class MapperFactory
{
    private static readonly ILoggerFactory LoggerFactory = NullLoggerFactory.Instance;

    private static readonly Lazy<IMapper> Mapper = new(() =>
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<MappingConfiguration>(), LoggerFactory);
        return configuration.CreateMapper();
    });

    public static IMapper Create() => Mapper.Value;
}
