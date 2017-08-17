using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Bsp.Core.Domain.Cats;
using Bsp.Services.Cats;
using Bsp.Models;

namespace WebApplicationBasic.Controllers {
    [Route("api/[controller]")]
    public class CatsController : Controller { 
        private readonly ICatService _catService;

        public CatsController(ICatService catService) {
            this._catService = catService;
        }

        [HttpPost]
        public virtual IActionResult Post([FromBody] Cat cat) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            _catService.InsertCat(cat);
            return Created($"/api/cats/{cat.Id}", cat);
        }
    }
}
