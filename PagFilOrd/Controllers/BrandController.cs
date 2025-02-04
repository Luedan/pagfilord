using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagFilOrd.bd;
using PagFilOrd.bd.models;

namespace PagFilOrd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private  ApplicationContext _context;

        public BrandController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Brand>>> Get()
        {
            return await _context.Brands.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Brand?>> Get(int id)
        {
            var brand = await _context.Brands.FindAsync(id);

            if (brand == null)
            {
                return NotFound();
            }
            return brand;
        }

        [HttpPost]
        public async Task<ActionResult<Brand>> Post([FromBody] Brand body)
        {
            var newBrand = new Brand()
            {
                Name = body.Name
            };
            
            await _context.Brands.AddAsync(newBrand);
            
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(Get), new { id = newBrand.Id }, newBrand);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Brand>> Put(int id, [FromBody] Brand body)
        {
            var brand = await _context.Brands.FindAsync(id);

            if (brand == null)
            {
                return NotFound();
            }

            brand.Name = body.Name;

            await _context.SaveChangesAsync();

            return brand;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Brand>> Delete(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            
            if (brand == null)
            {
                return NotFound();
            }
            
            _context.Brands.Remove(brand);
            
            await _context.SaveChangesAsync();
            
            return brand;
        }
    }
}