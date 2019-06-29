using Microsoft.EntityFrameworkCore;
using OnIndustri.Data.Model.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnIndustri.Data.Database
{
    public class PartnerContext : ApplicationContext
    {
        public PartnerContext()
        {
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
    }
}
