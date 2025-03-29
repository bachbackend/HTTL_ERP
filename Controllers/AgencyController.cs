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
    public class AgencyController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly HttlerpContext _context;
        private readonly MailService _mailService;
        private readonly PaginationSettings _paginationSettings;

        public AgencyController(IConfiguration configuration, HttlerpContext context, MailService mailService, IOptions<PaginationSettings> paginationSettings)
        {
            _configuration = configuration;
            _context = context;
            _mailService = mailService;
            _paginationSettings = paginationSettings.Value;
        }

        [HttpGet("GetAllAgency")]
        public async Task<IActionResult> GetAllAgency(
            int pageNumber = 1,
            int? pageSize = null
            )
        {
            int actualPageSize = pageSize ?? _paginationSettings.DefaultPageSize;
            var agency = _context.Agencies
                .Include(p => p.District)
                    .ThenInclude(p => p.City)
                .AsQueryable();

            int totalAgencyCount = await agency.CountAsync();


            int totalPageCount = (int)Math.Ceiling(totalAgencyCount / (double)actualPageSize);
            int nextPage = pageNumber + 1 > totalPageCount ? pageNumber : pageNumber + 1;
            int previousPage = pageNumber - 1 < 1 ? pageNumber : pageNumber - 1;

            var pagingResult = new PagingReturn
            {
                TotalPageCount = totalPageCount,
                CurrentPage = pageNumber,
                NextPage = nextPage,
                PreviousPage = previousPage
            };

            List<AgencyReturnDTO> agencyWithPaging = await agency
                .Skip((pageNumber - 1) * actualPageSize)
                .Take(actualPageSize)
                .Select(p => new AgencyReturnDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Address = p.Address,
                    Location = p.Location,
                    Latitude = p.Latitude,
                    Longitude = p.Longitude,
                    Phonenumber = p.Phonenumber,
                    Comment = p.Comment,
                    SampleDelivery = p.SampleDelivery,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt,
                    Type = p.Type,
                    DistrictId = p.DistrictId,
                    DistrictName = p.District.Name,
                    CityId = p.District.CityId,
                    CityName = p.District.City.Name
                })
            .ToListAsync();

            var result = new
            {
                Agency = agencyWithPaging,
                Paging = pagingResult
            };

            return Ok(result);
        }

        [HttpGet("GetAgencyById/{id}")]
        public async Task<IActionResult> GetAgencyById(int id)
        {
            // Tìm bài viết theo Id, bao gồm thông tin danh mục và người dùng
            var agency = await _context.Agencies
                .Include(p => p.District)
                    .ThenInclude(p => p.City)
                .Where(p => p.Id == id)
                .Select(p => new AgencyReturnDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Address = p.Address,
                    Location = p.Location,
                    Latitude = p.Latitude,
                    Longitude = p.Longitude,
                    Phonenumber = p.Phonenumber,
                    Comment = p.Comment,
                    SampleDelivery = p.SampleDelivery,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt,
                    Type = p.Type,
                    DistrictId = p.DistrictId,
                    DistrictName = p.District.Name,
                    CityId = p.District.CityId,
                    CityName = p.District.City.Name
                })
                .FirstOrDefaultAsync();

            // Nếu không tìm thấy bài viết, trả về NotFound
            if (agency == null)
            {
                return NotFound(new { Message = $"Không tìm thấy bài viết với Id = {id}" });
            }

            // Trả về bài viết
            return Ok(agency);
        }

        [HttpPost("addAgency")]
        public async Task<IActionResult> AddAgency([FromForm] AgencyRequest model)
        {
            // Tạo một sản phẩm mới và lưu vào database
            var agency = new Agency
            {
                Name = model.Name,
                Address = model.Address,
                Location = model.Location,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Phonenumber = model.Phonenumber,
                Comment = model.Comment,
                SampleDelivery = model.SampleDelivery,
                Status = model.Status,
                CreatedAt = model.CreatedAt,
                Type = model.Type,
                DistrictId = model.DistrictId,
            };

            // Lưu sản phẩm vào database
            _context.Agencies.Add(agency);
            await _context.SaveChangesAsync();

            return Ok(new { agencyId = agency.Id });
        }

        [HttpPut("updateAgency/{id}")]
        public async Task<IActionResult> UpdateAgency(int id, [FromForm] AgencyRequestUpdate model)
        {
            var agency = await _context.Agencies.FirstOrDefaultAsync(p => p.Id == id);
            if (agency == null)
            {
                return NotFound(new { message = "Agency not found." });
            }

            agency.Name = model.Name;
            agency.Address = model.Address;
            agency.Location = model.Location;
            agency.Latitude = model.Latitude;
            agency.Longitude = model.Longitude;
            agency.Phonenumber = model.Phonenumber;
            agency.Comment = model.Comment;
            agency.SampleDelivery = model.SampleDelivery;
            agency.Status = model.Status;
            agency.Type = model.Type;
            agency.DistrictId = model.DistrictId;


            await _context.SaveChangesAsync();

            return Ok(new { message = "Agency updated successfully." });
        }

        [HttpDelete("deleteAgency/{id}")]
        public async Task<IActionResult> DeleteAgency(int id)
        {
            // Tìm bài viết theo ID
            var agency = await _context.Agencies.FindAsync(id);

            if (agency == null)
            {
                return NotFound(new { message = "Agency not found." });
            }

            // Xóa bài viết khỏi database
            _context.Agencies.Remove(agency);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Agency deleted successfully." });
        }



    }
}
