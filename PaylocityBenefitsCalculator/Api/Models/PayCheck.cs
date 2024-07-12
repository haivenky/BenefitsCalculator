namespace Api.Models
{
    public class PayCheck
    {
        public int EmployeeId { get; set; }
        public decimal GrossPay { get; set; }
        public decimal Deductions { get; set; }
        public decimal NetPay { get; set; }
    }
}
