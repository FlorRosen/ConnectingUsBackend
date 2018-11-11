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
        private static readonly ImagesRepository imagesRepo = new ImagesRepository();
        //[Route("api/services/{idService:int?}")]

        //Public Methods
        //returns all services with optional parameters
        [HttpGet]
        public IEnumerable<Service> GetServices(int? idService = null, int? idUser = null, int? idCategory = null)
        {
            List<Service> services = servicesRepo.GetServices(idService, idUser, idCategory);
            return services;
        }

        //Post and returns all services with the filters selected         
        [Route("api/services/Search")]
        [HttpPost]
        public List<Service> Search([FromBody] SearchViewModel search)
        {
            return servicesRepo.Search(search);
        }
        
        //POST api/services
        //add service into DB
        [HttpPost]
        public IHttpActionResult Post([FromBody] Service service)
        {
            var ok = servicesRepo.AddService(service);

            return ok ? (IHttpActionResult)Ok() : Content(HttpStatusCode.BadRequest, "Fail to create service");
        }

        [Route("api/services/images")]
        [HttpPost]
        public IHttpActionResult SaveImagesPost([FromBody] Image image)
        {
            var ok = imagesRepo.AddImage(image);

            return ok ? (IHttpActionResult)Ok() : Content(HttpStatusCode.InternalServerError, "Fail save image");
        }

        [Route("api/services/images")]
        [HttpGet]
        public List<Image> GetImages(int idService)
        {
           

            return imagesRepo.GetImages(idService);
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
