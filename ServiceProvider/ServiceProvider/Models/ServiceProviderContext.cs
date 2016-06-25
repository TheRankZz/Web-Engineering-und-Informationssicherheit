using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ServiceProvider.Models
{
    public class ServiceProviderContext : DbContext
    {
        public ServiceProviderContext() : base("ServiceProviderContext")
        {

        }
        public DbSet<Message> Messages { get; set; }
        public DbSet<User> Users { get; set; }
    }
}