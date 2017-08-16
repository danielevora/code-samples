using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CatApplicationBasic.Models;

namespace CatApplicationBasic.Controllers {
    [Route("api/[controller]")]
    public class CatsController : Controller {
        private readonly ICatService _catService;
        public CatsController(ICatService catService) {
            this._catService = catService;
        } 

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> Get(int id) {
            var cat = await _catService.GetCatById(id);
            if (cat == null) {
                return NotFound();
            }
            return Ok(cat);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post([FromBody] CatModel cat) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var newCat = await _catService.InsertCat(cat);
            if (newCat != null) {
                return Created(newCat);
            }

            return Ok(); //TODO: throw error!
        }

        [HttpPut]
        public virtual async Task<IActionResult> Put([FromBody] int id, CatModel cat) {
            if (cat.Id != id || !ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var updatedCat = await _catService.UpdateCat(cat);
            if (updatedCat != null) {
                return NoContent();
            }

            return NotFound();
        }

        [HttpDelete]
        public virtual async Task<IActionResult> Delete(int id) {
            var deleted = await _catService.DeleteCat(id);
            
            if (deleted) {
                return Ok();
            }

            return NotFound();
        }
    }
}
