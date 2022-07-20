using Infrastructure;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace RestService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryContext _dbContext;
        public CategoryController(CategoryContext catalogContext)
        {
            _dbContext = catalogContext;
        }
        [HttpPost]
        public async Task<ActionResult<Category>> Create(Category category)
        {
            category.Id = _dbContext.Categories.Any() ?
                     _dbContext.Categories.Max(p => p.Id) + 1 : 1;
            _dbContext.Add(category);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }
        [HttpPost("{categoryId}")]
        public async Task<ActionResult<Category>> CreateItem(int categoryId, Item item)
        {
            var category = _dbContext.Categories.FirstOrDefault(c => c.Id == categoryId);

            if(category == null)
            {
                return NotFound();
            }
            category.Items.Add(item);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetById(int id)
        {
            var category = _dbContext.Categories.FirstOrDefault(c => c.Id == id);

            return Ok(category);
        }
        [HttpGet]
        public async Task<ActionResult<Category>> Get()
        {
            var categories = _dbContext.Categories.ToList();

            return Ok(categories);
        }
        [HttpGet("{categoryId}")]
        public async Task<ActionResult<Category>> GetItems(int categoryId, [FromQuery] PaginationFilter filter)
        {
            var categories = await _dbContext.Items.Where(i => i.CategoryId == categoryId)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return Ok(categories);
        }
        [HttpPut]
        public async Task<ActionResult> Update(Category category)
        {
            _dbContext.Attach(category);
            _dbContext.Entry(category).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
        [HttpPut("items")]
        public async Task<ActionResult> UpdateItem(Item item)
        {
            _dbContext.Attach(item);
            _dbContext.Entry(item).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("items/{id}")]
        public async Task<ActionResult> DeleteItem(int id)
        {
            var dbItem = _dbContext.Items.FirstOrDefault(i => i.Id == id);
            if(dbItem != null)
            {
                _dbContext.Remove(dbItem);
                await _dbContext.SaveChangesAsync();
            }
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var dbCategory = _dbContext.Categories.FirstOrDefault(i => i.Id == id);
            if (dbCategory != null)
            {
                _dbContext.Remove(dbCategory);
                await _dbContext.SaveChangesAsync();
            }
            return NoContent();
        }
    }
}
