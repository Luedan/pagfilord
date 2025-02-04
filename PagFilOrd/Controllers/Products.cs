using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagFilOrd.bd;
using PagFilOrd.bd.models;

namespace PagFilOrd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private  ApplicationContext _context;

        public ProductController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> Get()
        {
            return await _context.Products.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product?>> Get(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }
            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Post([FromBody] Product body)
        {
            var newProduct = new Product()
            {
                Name = body.Name,
                CategoryId = body.CategoryId,
                Description = body.Description,
                Image = body.Image,
                Price = body.Price
            };
            
            await _context.Products.AddAsync(newProduct);
            
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(Get), new { id = newProduct.Id }, newProduct);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> Put(int id, [FromBody] Product body)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            product.Name = body.Name;
            product.CategoryId = body.CategoryId;
            product.Description = body.Description;
            product.Image = body.Image;
            product.Price = body.Price;

            await _context.SaveChangesAsync();

            return product;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            
            if (product == null)
            {
                return NotFound();
            }
            
            _context.Products.Remove(product);
            
            await _context.SaveChangesAsync();
            
            return product;
        }
    }
}