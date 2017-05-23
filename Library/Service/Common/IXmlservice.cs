using System.Collections.Generic;
using Entities.Domain;
using System;

namespace Service.Common
{
    public interface IXmlService 
    {
        List<ProductionPlanningColor> GetAllProductionPlanningColors();
        List<String> GetAllComplaintTrackingColor();
        List<String> GetAllNumberComplaintTrackingColor();
        int GetQA_MarkLimit();
        int GetQA_SeverityLimit();
        string GetQA_EmailWarning();
    }
    
}
