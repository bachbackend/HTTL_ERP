using HTTL_ERP.DataAccess;
using HTTL_ERP.DTO;
using HTTL_ERP.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HTTL_ERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogisticController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly HttlerpContext _context;
        private readonly MailService _mailService;
        private readonly PaginationSettings _paginationSettings;

        public LogisticController(IConfiguration configuration, HttlerpContext context, MailService mailService, IOptions<PaginationSettings> paginationSettings)
        {
            _configuration = configuration;
            _context = context;
            _mailService = mailService;
            _paginationSettings = paginationSettings.Value;
        }

        [HttpGet("GetAllLogistic")]
        public async Task<IActionResult> GetAllLogistic(
            int pageNumber = 1,
            int? pageSize = null
            )
        {
            int actualPageSize = pageSize ?? _paginationSettings.DefaultPageSize;
            var logistic = _context.Logistics
                .AsQueryable();

            int totalLogisticCount = await logistic.CountAsync();


            int totalPageCount = (int)Math.Ceiling(totalLogisticCount / (double)actualPageSize);
            int nextPage = pageNumber + 1 > totalPageCount ? pageNumber : pageNumber + 1;
            int previousPage = pageNumber - 1 < 1 ? pageNumber : pageNumber - 1;

            var pagingResult = new PagingReturn
            {
                TotalPageCount = totalPageCount,
                CurrentPage = pageNumber,
                NextPage = nextPage,
                PreviousPage = previousPage
            };

            List<LogisticReturnDTO> logisticWithPaging = await logistic
                .Skip((pageNumber - 1) * actualPageSize)
                .Take(actualPageSize)
                .Select(p => new LogisticReturnDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Phonenumber = p.Phonenumber,
                    Status = p.Status,
                    Comment = p.Comment
                })
            .ToListAsync();

            var result = new
            {
                Logistic = logisticWithPaging,
                Paging = pagingResult
            };

            return Ok(result);
        }

        [HttpGet("GetLogisticById/{id}")]
        public async Task<IActionResult> GetLogisticById(int id)
        {
            var logistic = await _context.Logistics
                .Where(p => p.Id == id)
                .Select(p => new LogisticReturnDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Phonenumber = p.Phonenumber,
                    Status = p.Status,
                    Comment = p.Comment
                })
                .FirstOrDefaultAsync();

            // Nếu không tìm thấy vận chuyển, trả về NotFound
            if (logistic == null)
            {
                return NotFound(new { Message = $"Không tìm thấy đơn vị vận chuyển với Id = {id}" });
            }

            // Trả về bài viết
            return Ok(logistic);
        }

        [HttpPost("addLogistic")]
        public async Task<IActionResult> AddLogistic([FromForm] LogisticRequest model)
        {
            // Tạo một sản phẩm mới và lưu vào database
            var logistic = new Logistic
            {
                Name = model.Name,
                Phonenumber = model.Phonenumber,
                Status = model.Status,
                Comment = model.Comment
            };

            // Lưu sản phẩm vào database
            _context.Logistics.Add(logistic);
            await _context.SaveChangesAsync();

            return Ok(new { logisticId = logistic.Id });
        }

        [HttpPut("updateLogictis/{id}")]
        public async Task<IActionResult> UpdateLogictis(int id, [FromForm] LogictisRequestUpdate model)
        {
            var logistic = await _context.Logistics.FirstOrDefaultAsync(p => p.Id == id);
            if (logistic == null)
            {
                return NotFound(new { message = "Logistic not found." });
            }
            logistic.Name = model.Name;
            logistic.Phonenumber = model.Phonenumber;
            logistic.Status = model.Status;
            logistic.Comment = model.Comment;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Logistic updated successfully." });
        }

        [HttpDelete("deleteLogistic/{id}")]
        public async Task<IActionResult> DeleteAgency(int id)
        {
            var logistic = await _context.Logistics.FindAsync(id);

            if (logistic == null)
            {
                return NotFound(new { message = "Logistic not found." });
            }

            // Xóa bài viết khỏi database
            _context.Logistics.Remove(logistic);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Logistic deleted successfully." });
        }


    }
}
