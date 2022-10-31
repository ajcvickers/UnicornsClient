using System.ComponentModel.DataAnnotations;

namespace UnicornSuppliesX
{
    public class EmailAddress
    {
        public string Address { get; set; }
        public bool Primary { get; set; }
    }
}
