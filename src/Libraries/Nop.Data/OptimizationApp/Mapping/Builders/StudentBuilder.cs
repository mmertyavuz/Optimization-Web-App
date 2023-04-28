using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain;
using Nop.Data.Mapping.Builders;

namespace Nop.Data.OptimizationApp.Mapping.Builders;

public partial class StudentBuilder : NopEntityBuilder<Student>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(Student.StudentNumber)).AsString(50).NotNullable()
            .WithColumn(nameof(Student.FirstName)).AsString(400).NotNullable()
            .WithColumn(nameof(Student.LastName)).AsString(400).NotNullable()
            .WithColumn(nameof(Student.Email)).AsString(1000).NotNullable();
    }

    #endregion
}