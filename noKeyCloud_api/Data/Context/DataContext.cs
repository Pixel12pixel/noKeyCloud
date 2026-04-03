using Microsoft.EntityFrameworkCore;

namespace noKeyCloud_api.Data.Context
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
    }
}
