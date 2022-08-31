using Microsoft.AspNetCore.Mvc;
using SampleApi.WebApi.Models;
using SampleApi.WebApi.Services;

namespace SampleApi.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        readonly IService<Product> _service;
        public ProductsController(IService<Product> service)
        {
            _service = service;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(CancellationToken cancellationToken = default(CancellationToken))
        {
            var items = await _service.List(cancellationToken);
            if (items != null)
            {
                return Ok(items);
            }
            else
            {
                return Accepted();
            }
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var item = await _service.GetById(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> PutProduct(long id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }
            return await _service.Update(product);
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var item = await _service.Add(product);
                return Ok(item);
            }
            catch (ArgumentException) 
            {
                return BadRequest();
            }
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var item = await Task.Run(() => _service.GetById(id));
            if (item == null)
            {
                return NotFound();
            }
            await _service.Delete(id);
            return Ok(item);
        }
    }
}
