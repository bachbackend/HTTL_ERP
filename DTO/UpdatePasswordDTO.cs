using System.ComponentModel.DataAnnotations;

namespace HTTL_ERP.DTO
{
    public class UpdatePasswordDTO
    {
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
