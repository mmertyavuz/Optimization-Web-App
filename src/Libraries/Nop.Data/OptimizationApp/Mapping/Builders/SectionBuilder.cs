using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain;
using Nop.Data.Mapping.Builders;

namespace Nop.Data.OptimizationApp.Mapping.Builders;

public class SectionBuilder : NopEntityBuilder<Section>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(Section.SectionNumber)).AsString(50).NotNullable();
    }
}