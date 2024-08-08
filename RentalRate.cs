using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCarRentalSystem
{
    public class RentalRate
    {
        public double Rate { get; set; }

        public RentalRate(double rate)
        {
            if (ValidateRate(rate))
            {
                Rate = rate;
            }
            else
            {
                throw new ArgumentException("Invalid rate. Rate must be greater than 0.");
            }
        }

        public bool ValidateRate(double rate)
        {
            return rate > 0;
        }

        public void UpdateRate(double rate)
        {
            if (ValidateRate(rate))
            {
                Rate = rate;
            }
            else
            {
                throw new ArgumentException("Invalid rate. Rate must be greater than 0.");
            }
        }
    }
}
