using APIDemo.ResponseModule;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuggyController : ControllerBase
    {
        private readonly StoreDbContext _context;

        public BuggyController(StoreDbContext context)
        {
            _context = context;
        }

        [HttpGet("TestText")]
        [Authorize]
        public ActionResult<string> GetText()
        {
            return "Text";
        }

        [HttpGet("NotFound")]
        public ActionResult GetNotFoundRequest()
        {
            var anything = _context.Products.Find(1000);

            if (anything is null)
                return NotFound(new ApiResponse(404));

            return Ok();
        }

        [HttpGet("BadRequest")]
        public ActionResult GetBadRequest()
        {
            return BadRequest(new ApiResponse(400));
        }
    }
}
