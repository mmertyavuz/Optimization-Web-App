using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain;
using Nop.Data.Mapping.Builders;

namespace Nop.Data.OptimizationApp.Mapping.Builders;

public partial class EducatorBuilder : NopEntityBuilder<Educator>
{
    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(Educator.Name)).AsString(400).NotNullable()
            .WithColumn(nameof(Educator.Surname)).AsString(400).NotNullable()
            .WithColumn(nameof(Educator.Email)).AsString(1000).NotNullable();
    }
}