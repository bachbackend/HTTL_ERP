namespace HTTL_ERP.DTO
{
    public class BrickFactoryReturnDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Address { get; set; } = null!;

        public string Location { get; set; } = null!;

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public string Phonenumber { get; set; } = null!;

        public string? Comment { get; set; }

        public short? Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public short? TakeSample { get; set; }

        public int DistrictId { get; set; }

        public string DistrictName { get; set; } = null!;

        public int CityId { get; set; }

        public string CityName { get; set; } = null!;
    }
}
