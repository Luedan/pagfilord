using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagFilOrd.bd;
using PagFilOrd.bd.models;

namespace PagFilOrd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private  ApplicationContext _context;

        public CategoryController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> Get()
        {
            return await _context.Categories.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category?>> Get(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }
            return category;
        }

        [HttpPost]
        public async Task<ActionResult<Category>> Post([FromBody] Category body)
        {
            var newCategory = new Category()
            {
                Name = body.Name,
                BrandId = body.BrandId
            };
            
            await _context.Categories.AddAsync(newCategory);
            
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(Get), new { id = newCategory.Id }, newCategory);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Category>> Put(int id, [FromBody] Category body)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            category.Name = body.Name;
            category.BrandId = body.BrandId;

            await _context.SaveChangesAsync();

            return category;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Category>> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            
            if (category == null)
            {
                return NotFound();
            }
            
            _context.Categories.Remove(category);
            
            await _context.SaveChangesAsync();
            
            return category;
        }
    }
}