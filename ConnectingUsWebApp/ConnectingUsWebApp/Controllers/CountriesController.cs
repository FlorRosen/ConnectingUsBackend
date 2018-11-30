using System.Collections.Generic;
using System.Web.Http;
using ConnectingUsWebApp.Models;
using ConnectingUsWebApp.Repositories;

namespace ConnectingUsWebApp.Controllers
{
    [Authorize]
    public class CountriesController: ApiController
    {
        static readonly CountriesRepository countriesRepo = new CountriesRepository();

        //Public Methods
        [HttpGet]
        public IEnumerable<Country> GetCountries()
        {
            List<Country> countries = countriesRepo.GetCountries();
            return countries;
        }


        //Public Methods
        //Returns the countries where servieces are offer
        [Route("api/countries/Map")]
        [HttpGet]
        public IEnumerable<Country> GetCountriesOfServices()
        {
            List<Country> countries = countriesRepo.GetCountriesOfServices();
            return countries;
        }

        [HttpGet]
        public IHttpActionResult GetCountry(int id)
        {
            var country = countriesRepo.GetCountry(id);

            return country == null ? NotFound() : (IHttpActionResult)Ok(country);
        }



    }
}