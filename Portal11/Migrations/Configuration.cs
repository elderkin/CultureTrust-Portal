namespace Portal11.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Portal11.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;                   // A little risky, but we only turn this on rarely
            ContextKey = "Portal11.Models.ApplicationDbContext";
        }

        protected override void Seed(Portal11.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //context.Persons.AddOrUpdate(
            //    p => p.Name,
            //    new Models.Person { Name = "Doe, Jane", Address = "123 Anywhere St.\r\nPhiladelphia, PA 19000", Phone = "215-123-4567", Email = "Jane.doe@def.org", CreatedTime = System.DateTime.Now }
            //    );

            //                pers.FranchiseKey = Franchise.LocalFranchiseKey;
            //pers.Inactive = false;
            //pers.Name = "Doe, Jane";
            //pers.Address = "123 Anywhere St.\r\nPhiladelphia, PA 19000";
            //pers.Phone = "215-456-7890";
            //pers.Email = "jane.doe@def.org";
            //pers.CreatedTime = System.DateTime.Now;

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            ; //TODO More here - sense empty database and fill initial tables
        }
    }
}
