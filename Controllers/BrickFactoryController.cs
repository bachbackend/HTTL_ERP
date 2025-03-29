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
    public class BrickFactoryController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly HttlerpContext _context;
        private readonly MailService _mailService;
        private readonly PaginationSettings _paginationSettings;

        public BrickFactoryController(IConfiguration configuration, HttlerpContext context, MailService mailService, IOptions<PaginationSettings> paginationSettings)
        {
            _configuration = configuration;
            _context = context;
            _mailService = mailService;
            _paginationSettings = paginationSettings.Value;
        }

        [HttpGet("GetAllBrickFactory")]
        public async Task<IActionResult> GetAllAgency(
            int pageNumber = 1,
            int? pageSize = null
            )
        {
            int actualPageSize = pageSize ?? _paginationSettings.DefaultPageSize;
            var bf = _context.BrickFactories
                .Include(p => p.District)
                    .ThenInclude(p => p.City)
                .AsQueryable();

            int totalBrickFactoryCount = await bf.CountAsync();


            int totalPageCount = (int)Math.Ceiling(totalBrickFactoryCount / (double)actualPageSize);
            int nextPage = pageNumber + 1 > totalPageCount ? pageNumber : pageNumber + 1;
            int previousPage = pageNumber - 1 < 1 ? pageNumber : pageNumber - 1;

            var pagingResult = new PagingReturn
            {
                TotalPageCount = totalPageCount,
                CurrentPage = pageNumber,
                NextPage = nextPage,
                PreviousPage = previousPage
            };

            List<BrickFactoryReturnDTO> brickFactoryWithPaging = await bf
                .Skip((pageNumber - 1) * actualPageSize)
                .Take(actualPageSize)
                .Select(p => new BrickFactoryReturnDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Address = p.Address,
                    Location = p.Location,
                    Latitude = p.Latitude,
                    Longitude = p.Longitude,
                    Phonenumber = p.Phonenumber,
                    Comment = p.Comment,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt,
                    TakeSample = p.TakeSample,
                    DistrictId  = p.DistrictId,
                    DistrictName = p.District.Name,
                    CityId = p.District.City.Id,
                    CityName = p.District.City.Name
                })
            .ToListAsync();

            var result = new
            {
                BrickFactory = brickFactoryWithPaging,
                Paging = pagingResult
            };

            return Ok(result);
        }

        [HttpGet("GetBrickFactoryById/{id}")]
        public async Task<IActionResult> GetBrickFactoryById(int id)
        {
            // Tìm bài viết theo Id, bao gồm thông tin danh mục và người dùng
            var bf = await _context.BrickFactories
                .Include(p => p.District)
                    .ThenInclude(p => p.City)
                .Where(p => p.Id == id)
                .Select(p => new BrickFactoryReturnDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Address = p.Address,
                    Location = p.Location,
                    Latitude = p.Latitude,
                    Longitude = p.Longitude,
                    Phonenumber = p.Phonenumber,
                    Comment = p.Comment,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt,
                    TakeSample = p.TakeSample,
                    DistrictId = p.DistrictId,
                    DistrictName = p.District.Name,
                    CityId = p.District.City.Id,
                    CityName = p.District.City.Name
                })
                .FirstOrDefaultAsync();

            // Nếu không tìm thấy bài viết, trả về NotFound
            if (bf == null)
            {
                return NotFound(new { Message = $"Không tìm thấy nhà máy gạch với Id = {id}" });
            }

            // Trả về bài viết
            return Ok(bf);
        }

        [HttpPost("addBrickFactory")]
        public async Task<IActionResult> BrickFactory([FromForm] BrickFactoryRequest model)
        {
            // Tạo một sản phẩm mới và lưu vào database
            var brickFactory = new BrickFactory
            {
                Name = model.Name,
                Address = model.Address,
                Location = model.Location,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Phonenumber = model.Phonenumber,
                Comment = model.Comment,
                Status = model.Status,
                CreatedAt = model.CreatedAt,
                TakeSample = model.TakeSample,
                DistrictId = model.DistrictId
            };

            // Lưu sản phẩm vào database
            _context.BrickFactories.Add(brickFactory);
            await _context.SaveChangesAsync();

            return Ok(new { brickFactoryId = brickFactory.Id });
        }

        [HttpPut("updateBrickFactory/{id}")]
        public async Task<IActionResult> UpdateBrickFactory(int id, [FromForm] BrickFactoryRequestUpdate model)
        {
            var bf = await _context.BrickFactories.FirstOrDefaultAsync(p => p.Id == id);
            if (bf == null)
            {
                return NotFound(new { message = "Brick factory not found." });
            }

            
            bf.Name = model.Name;
            bf.Address = model.Address;
            bf.Location = model.Location;
            bf.Latitude = model.Latitude;
            bf.Longitude = model.Longitude;
            bf.Phonenumber = model.Phonenumber;
            bf.Comment  = model.Comment;
            bf.Status = model.Status;
            bf.TakeSample = model.TakeSample;
            bf.DistrictId = model.DistrictId;


            await _context.SaveChangesAsync();

            return Ok(new { message = "Brick factory updated successfully." });
        }

        [HttpDelete("deleteBrickFactory/{id}")]
        public async Task<IActionResult> DeleteBrickFactory(int id)
        {
            //tìm nhà máy gạch theo id
            var bf = await _context.BrickFactories.FindAsync(id);

            if (bf == null)
            {
                return NotFound(new { message = "Brick factory not found." });
            }

            // Xóa nhà máy gạch khỏi database
            _context.BrickFactories.Remove(bf);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Brick factory deleted successfully." });
        }

    }
}
