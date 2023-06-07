using Nop.Web.Framework.Models;

namespace Bau.Plugin.Optimization.Gurobi.Domain;

public record ClassroomDto : BaseNopEntityModel
{
    public int Size { get; set; }

    public string Name { get; set; }
}