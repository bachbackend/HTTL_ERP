using HTTL_ERP.DataAccess;
using HTTL_ERP.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HTTL_ERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistanceController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly HttlerpContext _context;
        private readonly MailService _mailService;
        private readonly PaginationSettings _paginationSettings;

        public DistanceController(IConfiguration configuration, HttlerpContext context, MailService mailService, IOptions<PaginationSettings> paginationSettings)
        {
            _configuration = configuration;
            _context = context;
            _mailService = mailService;
            _paginationSettings = paginationSettings.Value;
        }

        [HttpGet("nearby-factories")]
        public IActionResult GetNearbyFactories([FromQuery] decimal latitude, [FromQuery] decimal longitude, [FromQuery] decimal radius = 50)
        {
            // Lấy danh sách tất cả nhà máy gạch
            var allFactories = _context.BrickFactories.ToList();

            // Danh sách nhà máy trong bán kính
            var nearbyFactories = allFactories.Where(factory =>
            {
                decimal distance = HaversineDistance(latitude, longitude, factory.Latitude, factory.Longitude);
                return distance <= radius; // Chỉ lấy những nhà máy trong bán kính
            }).ToList();

            return Ok(nearbyFactories);
        }

        private decimal HaversineDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            const decimal R = 6371m; // Bán kính Trái Đất (km)
            decimal dLat = (lat2 - lat1) * (decimal)Math.PI / 180;
            decimal dLon = (lon2 - lon1) * (decimal)Math.PI / 180;

            decimal a = (decimal)Math.Sin((double)(dLat / 2)) * (decimal)Math.Sin((double)(dLat / 2)) +
                        (decimal)Math.Cos((double)(lat1 * (decimal)Math.PI / 180)) * (decimal)Math.Cos((double)(lat2 * (decimal)Math.PI / 180)) *
                        (decimal)Math.Sin((double)(dLon / 2)) * (decimal)Math.Sin((double)(dLon / 2));

            decimal c = 2 * (decimal)Math.Atan2(Math.Sqrt((double)a), Math.Sqrt(1 - (double)a));
            return R * c; // Khoảng cách tính bằng km
        }

    }
}
