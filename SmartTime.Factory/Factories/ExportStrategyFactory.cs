using SmartTime.Factory.Enums;
using SmartTime.Factory.Interfaces;

namespace SmartTime.Factory.Factories;

public class ExportStrategyFactory : IExportStrategyFactory
{
    private readonly IEnumerable<IExportStrategy> _strategies;

    // All registered IExportStrategy implementations are injected here by DI;
    // the factory just picks the one matching the requested format.
    public ExportStrategyFactory(IEnumerable<IExportStrategy> strategies)
    {
        _strategies = strategies;
    }

    public IExportStrategy GetStrategy(ExportFormat format)
    {
        var strategy = _strategies.FirstOrDefault(s => s.Format == format);
        if (strategy is null)
        {
            throw new NotSupportedException($"No export strategy registered for format '{format}'.");
        }

        return strategy;
    }
}