using SmartTime.Factory.Enums;

namespace SmartTime.Factory.Interfaces;

public interface IExportStrategyFactory
{
    IExportStrategy GetStrategy(ExportFormat format);
}