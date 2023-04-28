using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain;
using Nop.Data.Mapping.Builders;

namespace Nop.Data.OptimizationApp.Mapping.Builders;

public partial class ClassroomBuilder : NopEntityBuilder<Classroom>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(Classroom.Name)).AsString(400).NotNullable()
            .WithColumn(nameof(Classroom.Description)).AsString(1000).Nullable();
    }

    #endregion
}