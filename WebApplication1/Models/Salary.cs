using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace WebApplication1.Models
{
    public class Salary
    {
        [Key]
        public Int64 SalaryID  { get; set; }
        public Int64 EmployeeID { get; set; }
        public int MonthNumber { get; set; }
        public Int64 SalaryAmount { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
