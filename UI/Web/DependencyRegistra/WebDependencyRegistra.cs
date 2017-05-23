using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using Entities;
using RepositoryPattern.DataContext;
using RepositoryPattern.Infrastructure;
using RepositoryPattern.Infrastructure.DependencyManagement;
using RepositoryPattern.Repositories;
using RepositoryPatternEF6;
using Service;
using Service.Authentication;
using Service.Common;
using Service.Implement;
using Service.Interface;
using Service.Lines;
using Service.Messages;
using Service.Security;
//using Service.SupplyChain;
using Service.SupplyChain;
using Service.Users;
using Utils;
using Utils.Caching;
using Service.QualityAlerts;
using Service.Departments;
using Service.Meetings;
using Service.Dds;
using Service.Suppliers;
using Service.Categories;
using Service.ClassificationDefects;
using Service.ScMeasures;
using Service.ScoreCards;
using Service.FoundByFunctions;
using Service.Frequencys;
using Service.Tasks;

namespace Web.DependencyRegistra
{
    public class WebDependencyRegistra : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            //HTTP context and other related stuff
            builder.Register(c =>
                //register FakeHttpContext when HttpContext is not available
                HttpContext.Current != null ?
                (new HttpContextWrapper(HttpContext.Current) as HttpContextBase) :
                (new FakeHttpContext("~/") as HttpContextBase))
                .As<HttpContextBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Request)
                .As<HttpRequestBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Response)
                .As<HttpResponseBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Server)
                .As<HttpServerUtilityBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Session)
                .As<HttpSessionStateBase>()
                .InstancePerLifetimeScope();

            //controllers
            builder.RegisterControllers(typeFinder.GetAssemblies().ToArray());
            //Context
            var connectionFactory = new SqlConnectionFactory();
            Database.DefaultConnectionFactory = connectionFactory;

            var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            builder.Register<IDbContextAsync>(c => new NoisObjectContext(connectionString)).InstancePerLifetimeScope();

            //cache
            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().Named<ICacheManager>("nois_cache_static").SingleInstance();
            builder.RegisterType<PerRequestCacheManager>().As<ICacheManager>().Named<ICacheManager>("nois_cache_per_request").InstancePerLifetimeScope();
            
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepositoryAsync<>)).InstancePerLifetimeScope();
            builder.RegisterType<FormsAuthenticationService>().As<IAuthenticationService>().InstancePerLifetimeScope();
            builder.RegisterType<WebWorkContext>().As<IWorkContext>().InstancePerLifetimeScope();


            builder.RegisterType<SupplyChainProductionPlanningService>().As<ISupplyChainProductionPlanningService>().InstancePerRequest();
            builder.RegisterType<SupplyChainServiceService>().As<ISupplyChainServiceService>().InstancePerRequest();
            builder.RegisterType<SupplyChainFPQService>().As<ISupplyChainFPQService>().InstancePerRequest();
            builder.RegisterType<SupplyChainDDSService>().As<ISupplyChainDDSService>().InstancePerRequest();
            builder.RegisterType<SupplyChainHSEService>().As<ISupplyChainHSEService>().InstancePerRequest();
            builder.RegisterType<SupplyChainMPSAService>().As<ISupplyChainMPSAService>().InstancePerRequest();
            builder.RegisterType<MeasureSupplyChainService>().As<IMeasureSupplyChainService>().InstancePerRequest();
            builder.RegisterType<ReportService>().As<IReportService>().InstancePerRequest();
            //User in DMS 
            builder.RegisterType<UserAllowInSupplyChainService>().As<IUserAllowInSupplyChainService>().InstancePerRequest();

            //email send
            builder.RegisterType<QueuedEmailService>().As<IQueuedEmailService>().InstancePerRequest();
            builder.RegisterType<ScheduleTaskService>().As<IScheduleTaskService>().InstancePerRequest();
            builder.RegisterType<StringBuilderWorkFlowMessageService>().As<IWorkFlowMessageService>().InstancePerRequest();
            builder.RegisterType<SendMailByTaskService>().As<ISendMailService>().InstancePerRequest();
        
            //xml 
            builder.RegisterType<XmlService>().As<IXmlService>().InstancePerRequest();

            //excell service
            builder.RegisterType<ExcellService>().As<IExcellService>().InstancePerRequest();
            //product planning
            builder.RegisterType<ProductPlanningService>().As<IProductPlanningService>().InstancePerRequest();

            builder.RegisterType<UserService>().As<IUserService>().InstancePerRequest();
            builder.RegisterType<TrackingService>().As<ITrackingService>().InstancePerRequest();
            builder.RegisterType<LineService>().As<ILineService>().InstancePerRequest();
            builder.RegisterType<LineRemarkService>().As<ILineRemarkService>().InstancePerRequest();
            builder.RegisterType<DmsService>().As<IDmsService>().InstancePerRequest();
            builder.RegisterType<MeasureService>().As<IMeasureService>().InstancePerRequest();
            //builder.RegisterType<NoisMainMeasureService>().As<INoisMainMeasureService>().InstancePerRequest();
            //builder.RegisterType<NoisMainMeasureConfigService>().As<INoisMainMeasureConfigService>().InstancePerRequest();
            //builder.RegisterType<AttendanceService>().As<IAttendanceService>().InstancePerRequest();

            builder.RegisterType<IssueService>().As<IIssueService>().InstancePerRequest();
            builder.RegisterType<ShutdownRequestService>().As<IShutdownRequestService>().InstancePerRequest();
            builder.RegisterType<ProductPlanningService>().As<IProductPlanningService>().InstancePerRequest();
            //builder.RegisterType<UserAllowInMeetingService>().As<IUserAllowInMeetingService>().InstancePerRequest();

            builder.RegisterType<PermissionService>().As<IPermissionService>().InstancePerRequest();
            builder.RegisterType<PermissionProvider>().As<IPermissionProvider>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nois_cache_static"))
                .InstancePerLifetimeScope();
            //builder.RegisterType<RoleService>().As<IRoleService>().InstancePerRequest();
            builder.RegisterType<ClassificationService>().As<IClassificationService>().InstancePerRequest();
            builder.RegisterType<QualityAlertService>().As<IQualityAlertService>().InstancePerRequest();
            builder.RegisterType<UserRegistrationService>().As<IUserRegistrationService>().InstancePerRequest();
            builder.RegisterType<EncryptionService>().As<IEncryptionService>().InstancePerRequest();
            builder.RegisterType<DepartmentService>().As<IDepartmentService>().InstancePerRequest();
            builder.RegisterType<MeetingService>().As<IMeetingService>().InstancePerRequest();
            builder.RegisterType<UserInMeetingService>().As<IUserInMeetingService>().InstancePerRequest();
            builder.RegisterType<DdsConfigService>().As<IDdsConfigService>().InstancePerRequest();
            builder.RegisterType<DdsMeetingService>().As<IDdsMeetingService>().InstancePerRequest();
            builder.RegisterType<DdsMeetingResultService>().As<IDdsMeetingResultService>().InstancePerRequest();
            builder.RegisterType<DdsMeetingDetailService>().As<IDdsMeetingDetailService>().InstancePerRequest();
            builder.RegisterType<DdsMeetingPrDetailService>().As<IDdsMeetingPrDetailService>().InstancePerRequest();
            builder.RegisterType<UserRoleService>().As<IUserRoleService>().InstancePerRequest();
            builder.RegisterType<SupplierService>().As<ISupplierService>().InstancePerRequest();
            builder.RegisterType<CategoryService>().As<ICategoryService>().InstancePerRequest();
            builder.RegisterType<ClassificationDefectService>().As<IClassificationDefectService>().InstancePerRequest();
            builder.RegisterType<ScMeasureService>().As<IScMeasureService>().InstancePerRequest();
            builder.RegisterType<MaterialService>().As<IMaterialService>().InstancePerRequest();
            builder.RegisterType<ScoreCardService>().As<IScoreCardService>().InstancePerRequest();
            builder.RegisterType<MqsMeasureService>().As<IMqsMeasureService>().InstancePerRequest();
            builder.RegisterType<ScMeasureTargetService>().As<IScMeasureTargetService>().InstancePerRequest();
            builder.RegisterType<SubColumnFormulaService>().As<ISubColumnFormulaService>().InstancePerRequest();
            builder.RegisterType<FoundByFunctionService>().As<IFoundByFunctionService>().InstancePerRequest();

            builder.RegisterType<FrequencyService>().As<IFrequencyService>().InstancePerRequest();
            builder.RegisterType<SendMailTask>().As<ITask>().InstancePerRequest();

            //builder.RegisterType<SupplyChainService>().As<ISupplyChainService>().InstancePerRequest();
            //user store
            //builder.RegisterType<NoisUserStore>().As<IUserStore<User>>().InstancePerRequest();
            //UserManager
            //builder.RegisterType<NoisUserManager>().As<UserManager<User>>().InstancePerRequest();
            //role store
            //Khang comment builder.RegisterType<NoisRoleStore>().As<IRoleStore<Role>>().InstancePerRequest();
            //RoleManager
            //Khang comment builder.RegisterType<NoisRoleManager>().As<RoleManager<Role>>().InstancePerRequest();
        }

        public int Order { get { return 0; } }
    }
}