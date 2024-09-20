using Microsoft.EntityFrameworkCore;
using StripePaymentGateway.WebAPI.Models;

namespace StripePaymentGateway.WebAPI.DatabaseContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<OrderDetails> OrderDetails { get; set; }
    }
}
