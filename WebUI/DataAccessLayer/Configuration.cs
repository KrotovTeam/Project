using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace DataAccessLayer
{
    public class Configuration : DbMigrationsConfiguration<DatabaseContext>
    {
        protected override void Seed(DatabaseContext context)
        {
        }
    }
}
