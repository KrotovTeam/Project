using DataAccessLayer.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DataAccessLayer
{
    public class DatabaseContext : IdentityDbContext<UserModel>
    {
        public DatabaseContext() : base("DatabaseConntection")
        {
        }
    }
}