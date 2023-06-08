using FluentMigrator;
using Nop.Core.Domain;
using Nop.Core.Domain.Media;
using Nop.Data.Extensions;
using Nop.Data.Mapping;
using Nop.Data.Migrations;

namespace Nop.Data.OptimizationApp.Migrations.v00___Initial
{
    [NopMigration("2023-04-30 00:00:00", "Initial Migration", MigrationProcessType.Update)]
    public class SchemaMigration : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(Classroom))).Exists())
            {
                Create.TableFor<Classroom>();
            }

            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(Course))).Exists())
            {
                Create.TableFor<Course>();
            }
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(EducationalDepartment))).Exists())
            {
                Create.TableFor<EducationalDepartment>();
            }

            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(Educator))).Exists())
            {
                Create.TableFor<Educator>();
            }

            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(Faculty))).Exists())
            {
                Create.TableFor<Faculty>();
            }

            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(Section))).Exists())
            {
                Create.TableFor<Section>();
            }

            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(Student))).Exists())
            {
                Create.TableFor<Student>();
            }

            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(StudentSectionMapping))).Exists())
            {
                Create.TableFor<StudentSectionMapping>();
            }
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(OptimizationResult))).Exists())
            {
                Create.TableFor<OptimizationResult>();
            }

        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
