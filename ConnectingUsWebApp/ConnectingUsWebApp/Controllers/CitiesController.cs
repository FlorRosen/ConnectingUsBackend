using System.Collections.Generic;
using System.Web.Http;
using ConnectingUsWebApp.Models;
using ConnectingUsWebApp.Repositories;

namespace ConnectingUsWebApp.Controllers
{
    public class CitiesController : ApiController
    {
        static readonly CitiesRepository citiesRepo = new CitiesRepository();

        //Public Methods
        public IEnumerable<City> GetCities(int idCountry)
        {
            List<City> cities = citiesRepo.GetCities(idCountry);
            return cities;
        }


        public IHttpActionResult GetCity(int cityId, int countryId)
        {
            var country = citiesRepo.GetCity(cityId, countryId);

            return country == null ? NotFound() : (IHttpActionResult)Ok(country);
        }
    }
}