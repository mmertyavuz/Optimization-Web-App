using System;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain;
using Nop.Services.OptimizationApp;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Corporations;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories;

public interface IClassroomModelFactory
{
    ClassroomSearchModel PrepareClassroomSearchModel(ClassroomSearchModel searchModel);
    
    Task<ClassroomListModel> PrepareClassroomListModelAsync(ClassroomSearchModel searchModel);

    Task<ClassroomModel> PrepareClassroomModelAsync(ClassroomModel model, Classroom classroom, bool excludeProperties = false);
}

public class ClassroomModelFactory : IClassroomModelFactory
{
    #region Fields

    private readonly ICorporationService _corporationService;

    #endregion

    #region Ctor

    public ClassroomModelFactory(ICorporationService corporationService)
    {
        _corporationService = corporationService;
    }

    #endregion


    public ClassroomSearchModel PrepareClassroomSearchModel(ClassroomSearchModel searchModel)
    {
        if (searchModel == null)
            throw new ArgumentNullException(nameof(searchModel));
        
        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    public async Task<ClassroomListModel> PrepareClassroomListModelAsync(ClassroomSearchModel searchModel)
    {
        if (searchModel == null)
            throw new ArgumentNullException(nameof(searchModel));
        
        var classRooms = await _corporationService.GetAllClassroomsAsync(
            name: searchModel.Name,
            minCapacity: searchModel.MinCapacity,
            maxCapacity: searchModel.MaxCapacity,
            orderByCapacity: searchModel.orderByCapacity);
        
        var pagedClassRooms = classRooms.ToPagedList(searchModel);
        
        //prepare grid model
        var model = new ClassroomListModel().PrepareToGrid(searchModel, pagedClassRooms, () =>
        {
            return pagedClassRooms.Select(classRoom =>
            {
                //fill in model values from the entity
                var classroomModel = classRoom.ToModel<ClassroomModel>();

                return classroomModel;
            });
        });

        return model;
    }

    public async Task<ClassroomModel> PrepareClassroomModelAsync(ClassroomModel model, Classroom classroom, bool excludeProperties = false)
    {
        if (classroom != null)
        {
            //fill in model values from the entity
            if (model == null)
            {
                model = classroom.ToModel<ClassroomModel>();
            }
        }
        return model;
    }
}