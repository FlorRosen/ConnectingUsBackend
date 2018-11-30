using System.Collections.Generic;
using System.Web.Http;
using ConnectingUsWebApp.Models;
using ConnectingUsWebApp.Repositories;

namespace ConnectingUsWebApp.Controllers
{
    [Authorize]
    public class CategoriesController : ApiController
    {
        static readonly CategoriesRepository categoriesRepo = new CategoriesRepository();

        //Public Methods
        [HttpGet]
        public IEnumerable<Category> GetCategories()
        {
            List<Category> categories = categoriesRepo.GetCategories();
            return categories;
        }

    }
}