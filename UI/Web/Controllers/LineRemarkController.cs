using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Entities.Domain;
using Service.Lines;
using Utils;
using Web.Extend;
using Web.Models.LineRemark;

namespace Web.Controllers
{
    public class LineRemarkController : Controller
    {
        private readonly ILineRemarkService _lineRemarkService;
        private readonly IWorkContext _workContext;

        public LineRemarkController(ILineRemarkService lineRemarkService, 
            IWorkContext workContext)
        {
            _lineRemarkService = lineRemarkService;
            _workContext = workContext;
        }
      
        [HttpPost]
        public async Task<ActionResult> Update(LineRemarkModel model)
        {
            var lineRemark = await _lineRemarkService.GetLineByDateAndLineCode(model.CreateDate, model.LineCode, model.TypeCode);
            
            if (lineRemark == null)
            {
                var newLineRemark = new LineRemark()
                {
                    CreateDate = model.CreateDate,
                    LineCode = model.LineCode,
                    Remark = model.Remark,
                    CreateUserId = _workContext.CurrentUser.Id,
                    UpdateDate = DateTime.Now.Date,
                    LineRemarkTypeId = model.TypeCode,
                };
                var checkDate = new DateTime();
                if (model.CreateDate == checkDate)
                    newLineRemark.CreateDate = DateTime.Now.Date;
                await _lineRemarkService.InsertAsync(newLineRemark);
            }
            else
            {
                lineRemark.UpdateDate = DateTime.Now.Date;
                lineRemark.Remark = model.Remark;
                lineRemark.UpdateUserId = _workContext.CurrentUser.Id;
            }
            return new NullJsonResult();
        }
      
        public async Task<string> GetRemarkByDateAndLineCode(string lineCode, int typeCode, DateTime date )
        {
            var lineRemark = await _lineRemarkService.GetLineByDateAndLineCode(date, lineCode, typeCode);
            if (lineRemark != null)
            {
                return lineRemark.Remark;
            }
            return "";
        }
    }
}