using Entities.Domain.Categories;
using Nois.Web.Framework.Kendoui;
using Service.Categories;
using Service.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Utils;
using Web.Models.Category;

namespace Web.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly ICategoryService _categoryService;


        public CategoryController(
            IPermissionService permissionService,
            IWorkContext workContext,
            ICategoryService categoryService
            )
        {
            _permissionService = permissionService;
            _workContext = workContext;
            _categoryService = categoryService;
        }

        public ActionResult Index()
        {
            if (!_permissionService.Authorize(PermissionProvider.CategoryManagement))
                return AccessDeniedView();

            return View("List");
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command, SearchCategoryModel model)
        {
            if (!_permissionService.Authorize(PermissionProvider.CategoryManagement))
                return AccessDeniedView();

            var categorys = _categoryService.GetAllCategoryAsync(searchByCategoryName: model.CategoryName, pageIndex: command.Page - 1, pageSize: command.PageSize).Result;

            var gridModel = new DataSourceResult
            {
                Data = categorys.Select(s => new CategoryModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Note = s.Note,
                    DisplayOrder = s.DisplayOrder
                }),
                Total = categorys.TotalCount
            };

            // Return the result as JSON
            return Json(gridModel);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CategoryModel model)
        {
            if (!_permissionService.Authorize(PermissionProvider.CategoryManagement))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                if (_categoryService.CheckNameHasExisted(model.Name).Result)
                    return Content("Category Name has existed.");

                var category = new Category()
                {
                    Name = model.Name,
                    Note = model.Note,
                    DisplayOrder = model.DisplayOrder
                };

                try
                {
                    await _categoryService.InsertAsync(category);
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }

                return Json(new { status = "success" });
            }

            // If we got this far, something failed, redisplay form
            return Content(GetErrorMessageFromModelState());
        }

        [HttpPost]
        public async Task<ActionResult> Update(CategoryModel model)
        {
            if (!_permissionService.Authorize(PermissionProvider.CategoryManagement))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                if (model.Id <= 0)
                    return Content("Identity of Category is invalid.");

                var category = await _categoryService.GetByIdAsync(model.Id);
                if (category == null)
                    return Content("Can not found Category.");

                var exiestedCategory = await _categoryService.GetCategoryByNameAsync(model.Name);
                if(exiestedCategory != null && exiestedCategory.Id != category.Id)
                    return Content("Category Name has existed.");

                category.Name = model.Name;
                category.Note = model.Note;
                category.DisplayOrder = model.DisplayOrder;

                try
                {
                    await _categoryService.UpdateAsync(category);
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }

                return Json(new { status = "success" });
            }

            // If we got this far, something failed, redisplay form
            return Content(GetErrorMessageFromModelState());
        }

        [HttpPost]
        public async Task<ActionResult> DeleteCategories(List<int> listId)
        {
            if (!_permissionService.Authorize(PermissionProvider.CategoryManagement))
                return AccessDeniedView();

            try
            {
                await _categoryService.DeleteCategoriesAsync(listId);
            }
            catch (Exception ex)
            {
                return Json(new { status = "false", Content = ex.Message });
            }

            return Json(new
            {
                status = "success",
            });
        }
    }
}
