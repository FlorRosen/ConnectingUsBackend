using System.Collections.Generic;
using System.Web.Http;
using ConnectingUsWebApp.Models;
using ConnectingUsWebApp.Repositories;

namespace ConnectingUsWebApp.Controllers
{
    public class CountriesController: ApiController
    {
        static readonly CountriesRepository countriesRepo = new CountriesRepository();

        //Public Methods
        public IEnumerable<Country> GetCountries()
        {
            List<Country> countries = countriesRepo.GetCountries();
            return countries;
        }

        public IHttpActionResult GetCountry(int id)
        {
            var country = countriesRepo.GetCountry(id);

            return country == null ? NotFound() : (IHttpActionResult)Ok(country);
        }
    }
}