using System;
using System.Collections.Generic;

namespace iCarRentalSystem
{

    public class Renter : User
    {
        private string driversLicenseNo;
        private List<Booking> bookList;

        public Renter(string driversLicenseNo, int userId, string name, int contact, DateTime dob, string address)
            : base(userId, name, contact, dob, address)
        {
            this.driversLicenseNo = driversLicenseNo;
            this.bookList = new List<Booking>();
        }

        public string DriversLicenseNo
        {
            get { return driversLicenseNo; }
            set { driversLicenseNo = value; }
        }

        public List<Booking> BookList
        {
            get { return bookList; }
            set { bookList = value; }
        }

        public override string ToString()
        {
            return $"{base.ToString()}, Driver's License No: {driversLicenseNo}";
        }
    }
}
