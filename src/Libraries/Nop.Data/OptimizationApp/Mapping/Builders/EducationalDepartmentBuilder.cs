using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain;
using Nop.Data.Mapping.Builders;

namespace Nop.Data.OptimizationApp.Mapping.Builders;

public partial class EducationalDepartmentBuilder : NopEntityBuilder<EducationalDepartment>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(EducationalDepartment.Name)).AsString(400).NotNullable()
            .WithColumn(nameof(EducationalDepartment.Code)).AsString(50).NotNullable()
            .WithColumn(nameof(EducationalDepartment.Description)).AsString(1000).NotNullable();
    }

    #endregion
}