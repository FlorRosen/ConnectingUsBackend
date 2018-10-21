using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using ConnectingUsWebApp.Models;
using System.Data.SqlClient;
using System.Text;
using System.Web.Http;
using ConnectingUsWebApp.Repositories;
using ConnectingUsWebApp.Models.ViewModels;

namespace ConnectingUsWebApp.Controllers
{
    public class ServicesController : ApiController
    {
        private static readonly ServicesRepository servicesRepo = new ServicesRepository();
        //[Route("api/services/{idService:int?}")]

        //Public Methods
        //returns all services with optional parameters
        [HttpGet]
        public IEnumerable<Service> GetServices(int? idService=null, int? idUser = null, int? idCategory = null)
        {
            List<Service> services = servicesRepo.GetServices(idService, idUser,idCategory);
            return services;
        }

        //returns all services with the filters selected 
        //[Route("api/services/Search")]
        //[HttpGet]
        //public IEnumerable<Service> Search(List<string> idCategories, string textForSearch="", int? idCountry = null, int? idCity = null, int? idUser = null)
        //{
        //    List<Service> services = servicesRepo.Search(idCategories, textForSearch, idCountry, idCity, idUser);
        //    return services;
        //}

        [Route("api/services/Search")]
        [HttpPost]
        public IEnumerable<Service> Post([FromBody] SearchViewModel searchViewModel)
        {
            List<Service> services = servicesRepo.Search(searchViewModel.IdCategories, searchViewModel.TextForSearch, searchViewModel.IdCountry, searchViewModel.IdCity, searchViewModel.IdUser);
            return services;
        }

        //POST api/services
        //add service into DB
        [HttpPost]
        public IHttpActionResult Post([FromBody] Service service)
        {
            var ok = servicesRepo.AddService(service);

            return ok ? (IHttpActionResult)Ok() : Content(HttpStatusCode.BadRequest, "Fail to create service");
        }

        //PUT api/services
        [HttpPut]
        public IHttpActionResult Put([FromBody] Service service)
        {
            var ok = servicesRepo.UpdateService(service);

            return ok ? (IHttpActionResult)Ok() : Content(HttpStatusCode.BadRequest, "Fail to update service");
        }

    }
}
