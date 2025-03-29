namespace HTTL_ERP.DTO
{
    public class LogisticReturnDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Phonenumber { get; set; } = null!;

        public short? Status { get; set; }

        public string? Comment { get; set; }
    }
}
