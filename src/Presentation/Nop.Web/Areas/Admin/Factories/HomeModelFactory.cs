using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain;
using Nop.Core.Domain.Common;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.OptimizationApp;
using Nop.Web.Areas.Admin.Infrastructure.Cache;
using Nop.Web.Areas.Admin.Models.Home;
using Nop.Web.Areas.Admin.OptimizationApp.Models;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the home models factory implementation
    /// </summary>
    public partial class HomeModelFactory : IHomeModelFactory
    {
        #region Fields

        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly ICommonModelFactory _commonModelFactory;
        private readonly ILogger _logger;
        private readonly IOrderModelFactory _orderModelFactory;
        private readonly ISettingService _settingService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IWorkContext _workContext;
        private readonly NopHttpClient _nopHttpClient;
        private readonly IOptimizationModelFactory _optimizationModelFactory;
        private readonly ISectionService _sectionService;
        private readonly ICorporationService _corporationService;
        private readonly IOptimizationProcessingService _optimizationProcessingService;
        private readonly IOptimizationResultService _optimizationResultService;

        #endregion

        #region Ctor

        public HomeModelFactory(AdminAreaSettings adminAreaSettings,
            ICommonModelFactory commonModelFactory,
            ILogger logger,
            IOrderModelFactory orderModelFactory,
            ISettingService settingService,
            IStaticCacheManager staticCacheManager,
            IWorkContext workContext,
            NopHttpClient nopHttpClient, IOptimizationModelFactory optimizationModelFactory, ISectionService sectionService, ICorporationService corporationService, IOptimizationProcessingService optimizationProcessingService, IOptimizationResultService optimizationResultService)
        {
            _adminAreaSettings = adminAreaSettings;
            _commonModelFactory = commonModelFactory;
            _logger = logger;
            _orderModelFactory = orderModelFactory;
            _settingService = settingService;
            _staticCacheManager = staticCacheManager;
            _workContext = workContext;
            _nopHttpClient = nopHttpClient;
            _optimizationModelFactory = optimizationModelFactory;
            _sectionService = sectionService;
            _corporationService = corporationService;
            _optimizationProcessingService = optimizationProcessingService;
            _optimizationResultService = optimizationResultService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare dashboard model
        /// </summary>
        /// <param name="model">Dashboard model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the dashboard model
        /// </returns>
        public virtual async Task<DashboardModel> PrepareDashboardModelAsync(DashboardModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;

            //prepare nested search models
            var sections = await _sectionService.GetAllSectionsAsync();
        
            var classrooms = await _corporationService.GetAllClassroomsAsync();

            model.OptimizationOverviewModel = new OptimizationOverviewModel
            {
                SectionCount = sections.Count,
                ClassroomCount = classrooms.Count,
                IsReadyForOptimization = sections.Count > 0 && classrooms.Count > 0,
                IsOptimized = _optimizationProcessingService.IsOptimized(),
            };

            #region Graphs

            model.StudentCountByDayModel = await PrepareStudentCountByDayModelAsync();
            model.StudentCountByClassroomModel = await PrepareStudentCountByClassroomModelAsync();

            #endregion

            return model;
        }

        /// <summary>
        /// Prepare nopCommerce news model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the nopCommerce news model
        /// </returns>
        public virtual async Task<NopCommerceNewsModel> PrepareNopCommerceNewsModelAsync()
        {
            var model = new NopCommerceNewsModel
            {
                HideAdvertisements = _adminAreaSettings.HideAdvertisementsOnAdminArea
            };

            try
            {
                //try to get news RSS feed
                var rssData = await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.OfficialNewsModelKey), async () =>
                {
                    try
                    {
                        return await _nopHttpClient.GetNewsRssAsync();
                    }
                    catch (AggregateException exception)
                    {
                        //rethrow actual excepion
                        throw exception.InnerException;
                    }
                });

                for (var i = 0; i < rssData.Items.Count; i++)
                {
                    var item = rssData.Items.ElementAt(i);
                    var newsItem = new NopCommerceNewsDetailsModel
                    {
                        Title = item.TitleText,
                        Summary = XmlHelper.XmlDecode(item.Content?.Value ?? string.Empty),
                        Url = item.Url.OriginalString,
                        PublishDate = item.PublishDate
                    };
                    model.Items.Add(newsItem);

                    //has new items?
                    if (i != 0)
                        continue;

                    var firstRequest = string.IsNullOrEmpty(_adminAreaSettings.LastNewsTitleAdminArea);
                    if (_adminAreaSettings.LastNewsTitleAdminArea == newsItem.Title)
                        continue;

                    _adminAreaSettings.LastNewsTitleAdminArea = newsItem.Title;
                    await _settingService.SaveSettingAsync(_adminAreaSettings);

                    //new item
                    if (!firstRequest)
                        model.HasNewItems = true;
                }
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync("No access to the news. Website www.nopcommerce.com is not available.", ex);
            }

            return model;
        }

        private async Task<List<StudentCountByDayModel>> PrepareStudentCountByDayModelAsync()
        {
            var list = new List<StudentCountByDayModel>
            {
                new StudentCountByDayModel
                {
                    Day = DayOfWeek.Monday
                },
                new StudentCountByDayModel
                {
                    Day = DayOfWeek.Tuesday
                },
                new StudentCountByDayModel
                {
                    Day = DayOfWeek.Wednesday
                },
                new StudentCountByDayModel
                {
                    Day = DayOfWeek.Thursday
                },
                new StudentCountByDayModel
                {
                    Day = DayOfWeek.Friday
                },
                new StudentCountByDayModel
                {
                    Day = DayOfWeek.Saturday
                }
            };

            foreach (var l in list)
            {
                var sections = await _optimizationResultService.GetOptimizedSectionsAsync(DayId: (int) l.Day);

                foreach (var section in sections)
                {
                    l.Count += section.StudentCount;
                }
            }

            return list;
        }
        
        private async Task<List<StudentCountByTimeModel>> PrepareStudentCountByTimeModelAsync()
        {
            var list = new List<StudentCountByTimeModel>()
            {
                new StudentCountByTimeModel
                {
                    Start = new TimeSpan(8, 00, 0),
                    End = new TimeSpan(10, 00, 0)
                },
                new StudentCountByTimeModel
                {
                    Start = new TimeSpan(10, 00, 0),
                    End = new TimeSpan(12, 00, 0)
                },
                new StudentCountByTimeModel
                {
                    Start = new TimeSpan(12, 00, 0),
                    End = new TimeSpan(14, 00, 0)
                },
                new StudentCountByTimeModel
                {
                    Start = new TimeSpan(14, 00, 0),
                    End = new TimeSpan(16, 00, 0)
                },
                new StudentCountByTimeModel
                {
                    Start = new TimeSpan(16, 00, 0),
                    End = new TimeSpan(18, 00, 0)
                },
                new StudentCountByTimeModel
                {
                    Start = new TimeSpan(18, 00, 0),
                    End = new TimeSpan(20, 00, 0)
                },
                
            };

            foreach (var studentCountByTimeModel in list)
            {
                var sections = await _optimizationResultService.GetOptimizedSectionsAsync(StartDate: studentCountByTimeModel.Start, EndDate: studentCountByTimeModel.End);

                foreach (var section in sections)
                {
                    studentCountByTimeModel.Count += section.StudentCount;
                }
            }

            return list;
        }
        
        private async Task<List<StudentCountByClassroomModel>> PrepareStudentCountByClassroomModelAsync()
        {
            var classrooms = await _corporationService.GetAllClassroomsAsync();

            var list = new List<StudentCountByClassroomModel>();
            
            foreach (var classroom in classrooms)
            {
                var sections = await _optimizationResultService.GetOptimizedSectionsAsync(ClassroomId: classroom.Id);
                
                var model = new StudentCountByClassroomModel
                {
                    Classroom = classroom.Name,
                    Count = 0
                };
                
                foreach (var section in sections)
                {
                    model.Count += section.StudentCount;
                }
                
                list.Add(model);
            }

            list = list.Where(x => x.Count > 0).OrderBy(x => x.Count).ToList();
            
            
            return list;
        }
        
        #endregion
    }
}