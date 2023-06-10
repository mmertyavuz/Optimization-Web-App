using System;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Reports;
using Nop.Web.Areas.Admin.OptimizationApp.Models;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Home
{
    /// <summary>
    /// Represents a dashboard model
    /// </summary>
    public partial record DashboardModel : BaseNopModel
    {
        #region Ctor

        public DashboardModel()
        {
            PopularSearchTerms = new PopularSearchTermSearchModel();
            BestsellersByAmount = new BestsellerBriefSearchModel();
            BestsellersByQuantity = new BestsellerBriefSearchModel();
            OptimizationOverviewModel = new OptimizationOverviewModel();
            StudentCountByDayModel = new List<StudentCountByDayModel>();
            StudentCountByTimeModel = new List<StudentCountByTimeModel>();
            StudentCountByClassroomModel = new List<StudentCountByClassroomModel>();
            BestClassroomsByDepartmentModel = new List<BestClassroomsByDepartmentModel>();
            
        }

        #endregion

        #region Properties

        public bool IsLoggedInAsVendor { get; set; }

        public PopularSearchTermSearchModel PopularSearchTerms { get; set; }

        public BestsellerBriefSearchModel BestsellersByAmount { get; set; }

        public BestsellerBriefSearchModel BestsellersByQuantity { get; set; }

        public OptimizationOverviewModel OptimizationOverviewModel { get; set; }

        public List<StudentCountByDayModel> StudentCountByDayModel { get; set; }

        public List<StudentCountByTimeModel> StudentCountByTimeModel { get; set; }

        public List<StudentCountByClassroomModel> StudentCountByClassroomModel { get; set; }

        public List<BestClassroomsByDepartmentModel> BestClassroomsByDepartmentModel { get; set; }

        #endregion
    }
    
    public record StudentCountByDayModel : BaseNopModel
    {
        public DayOfWeek Day { get; set; }
        public int Count { get; set; }
    }
    
    public record StudentCountByTimeModel : BaseNopModel
    {
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public int Count { get; set; }
    }
    
    public record StudentCountByClassroomModel : BaseNopModel
    {
        public string Classroom { get; set; }
        public int Count { get; set; }
    }
    
    public record BestClassroomsByDepartmentModel : BaseNopModel
    {
        public string Department { get; set; }
        public string Classroom { get; set; }
        public int Count { get; set; }
    }
}