using HTTL_ERP.DataAccess;
using HTTL_ERP.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HTTL_ERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistrictController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly HttlerpContext _context;
        private readonly MailService _mailService;
        private readonly PaginationSettings _paginationSettings;

        public DistrictController(IConfiguration configuration, HttlerpContext context, MailService mailService, IOptions<PaginationSettings> paginationSettings)
        {
            _configuration = configuration;
            _context = context;
            _mailService = mailService;
            _paginationSettings = paginationSettings.Value;
        }

        [HttpGet("GetProvinces")]
        public async Task<IActionResult> GetProvinces()
        {
            var citis = await _context.Cities
                .Select(p => new { p.Id, p.Name })
                .ToListAsync();

            return Ok(citis);
        }

        [HttpGet("GetDistricts/{cityId}")]
        public async Task<IActionResult> GetDistricts(int cityId)
        {
            var districts = await _context.Districts
                .Where(d => d.CityId == cityId)
                .Select(d => new { d.Id, d.Name })
                .ToListAsync();

            if (districts == null || !districts.Any())
            {
                return NotFound("Không có quận/huyện nào.");
            }

            return Ok(districts);
        }
    }
}
