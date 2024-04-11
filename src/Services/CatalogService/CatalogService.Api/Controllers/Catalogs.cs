using CatalogService.Api.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CatalogService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Catalogs : ControllerBase
    {
        private readonly ICatalogBrandRepository _catalogBrandRepository;

        public Catalogs(ICatalogBrandRepository catalogBrandRepository)
        {
            _catalogBrandRepository = catalogBrandRepository;
        }

        [HttpGet("catalogBrands")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetCatalogBrands()
        {
            var result = await _catalogBrandRepository.GetListAsync();
            return Ok(result);
        }
    }
}
