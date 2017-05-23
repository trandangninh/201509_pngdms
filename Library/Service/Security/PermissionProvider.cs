using System.Collections.Generic;
using Entities.Domain.Security;
using Entities.Domain.Users;
using Service.Interface;

namespace Service.Security
{

    /// <summary>
    /// Standard permission provider
    /// </summary>
    public partial class PermissionProvider : IPermissionProvider
    {
        //admin area permissions
        public static readonly PermissionRecord Management = new PermissionRecord { Name = "Admin area. Management Panel", SystemName = "Management", Category = "Admin" };
        public static readonly PermissionRecord ManageUser = new PermissionRecord { Name = "Admin area. Manage User", SystemName = "ManageUser", Category = "Manager" };
        public static readonly PermissionRecord ManageDepartment = new PermissionRecord { Name = "Admin area. Manage Department", SystemName = "ManageDepartment", Category = "Department" };
        public static readonly PermissionRecord ManageLine = new PermissionRecord { Name = "Admin area. Manage Line", SystemName = "ManageLine", Category = "Line" };
        public static readonly PermissionRecord ManageDms = new PermissionRecord { Name = "Admin area. Manage DMS", SystemName = "ManageDMS", Category = "Dms" };
        public static readonly PermissionRecord ManageMeasure = new PermissionRecord { Name = "Admin area. Manage Measure", SystemName = "ManageMeasure", Category = "Measure" };


        public static readonly PermissionRecord WriteAttendance = new PermissionRecord { Name = "Write Attendance ", SystemName = "WriteAttendance", Category = "Department" };
        public static readonly PermissionRecord ViewAttendance = new PermissionRecord { Name = "View Attendance ", SystemName = "ViewAttendance", Category = "Department" };
        public static readonly PermissionRecord WriteIssues = new PermissionRecord { Name = "Write Issues", SystemName = "WriteIssues", Category = "Department" };
        public static readonly PermissionRecord ViewIssues = new PermissionRecord { Name = "View Issues ", SystemName = "ViewIssues", Category = "Department" };
        public static readonly PermissionRecord WriteTracking = new PermissionRecord { Name = "Write Tracking", SystemName = "WriteTracking", Category = "Department" };
        public static readonly PermissionRecord ViewTracking = new PermissionRecord { Name = "View Tracking ", SystemName = "ViewTracking", Category = "Department" };

        public static readonly PermissionRecord QualityAlert = new PermissionRecord { Name = "Quality Alert ", SystemName = "QualityAlert", Category = "QualityAlert" };
        public static readonly PermissionRecord SupplierManagement = new PermissionRecord { Name = "Supplier Management ", SystemName = "SupplierManagement", Category = "QualityAlert" };
        public static readonly PermissionRecord CategoryManagement = new PermissionRecord { Name = "Category Management ", SystemName = "CategoryManagement", Category = "QualityAlert" };
        public static readonly PermissionRecord ScoreCardMeasureManagement = new PermissionRecord { Name = "Score Card Measure Management ", SystemName = "ScoreCardMeasureManagement", Category = "QualityAlert" };
        public static readonly PermissionRecord ClassificationDefectManagement = new PermissionRecord { Name = "Classification Defect Management ", SystemName = "ClassificationDefectManagement", Category = "QualityAlert" };
        public static readonly PermissionRecord ScoreCard = new PermissionRecord { Name = "Score Card ", SystemName = "ScoreCard", Category = "QualityAlert" };
        public static readonly PermissionRecord QAReport = new PermissionRecord { Name = "QA Report ", SystemName = "QAReport", Category = "QualityAlert" };
        public static readonly PermissionRecord QAGuest = new PermissionRecord { Name = "QA Guest ", SystemName = "QAGuest", Category = "QualityAlert" };
        public static readonly PermissionRecord QAMPD = new PermissionRecord { Name = "QA MPD ", SystemName = "QA/MPD", Category = "QualityAlert" };
        public static readonly PermissionRecord QAEditStatus = new PermissionRecord { Name = "QA Edit Status ", SystemName = "QAEditStatus", Category = "QualityAlert" };
        /// <summary>
        /// Just view supply chain DDS
        /// </summary>

        public virtual IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                ManageUser,
                ManageLine,
                ManageDms,
                ManageMeasure,
                WriteAttendance,
                ViewAttendance,
                WriteIssues,
                ViewIssues,
                WriteTracking,
                ViewTracking,
                QualityAlert,
                SupplierManagement,
                CategoryManagement,
                ScoreCardMeasureManagement,
                ClassificationDefectManagement,
                ScoreCard,
                QAReport,
                QAGuest,
                QAMPD
                //To-Do need to add more permission
            };
        }

        public virtual IEnumerable<DefaultPermissionRecord> GetDefaultPermissions()
        {
            return new[]
            {
                new DefaultPermissionRecord
                {
                    CustomerRoleSystemName = SystemUserRoleNames.Administrators,
                    PermissionRecords = new[]
                    {
                        ManageUser,
                        ManageDms,
                        ManageLine,
                        ManageMeasure,

                        WriteAttendance,
                        ViewAttendance,
                        WriteIssues,
                        ViewIssues,
                        WriteTracking,
                        ViewTracking,

                        QualityAlert,
                        SupplierManagement,
                        CategoryManagement,
                        ScoreCardMeasureManagement,
                        ClassificationDefectManagement,
                        ScoreCard,
                        QAReport,
                        QAGuest,
                        QAMPD
                    }
                },

                new DefaultPermissionRecord
                {
                    CustomerRoleSystemName = SystemUserRoleNames.Employees,
                    PermissionRecords = new[]
                    {
                        WriteAttendance,
                        ViewAttendance,
                        WriteIssues,
                        ViewIssues,
                        WriteTracking,
                        ViewTracking,
                    }
                }
            };
        }
    }
}
