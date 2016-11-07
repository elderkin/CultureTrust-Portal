using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Portal11.Logic;

namespace Portal11.Models
{
    public class PortalDatabaseInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
//        public class PortalDatabaseInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {

            // These initializations are temporary to make testing easier. Sprinkle in a few rows for a bogus franchise to make sure they are excluded
            // from searching.

            // Initialize rows of the Person table to get it created.

            //Person pers = new Person();
            //pers.FranchiseKey = Franchise.LocalFranchiseKey;
            //pers.Inactive = false;
            //pers.Name = "Doe, Jane";
            //pers.Address = "123 Anywhere St.\r\nPhiladelphia, PA 19000";
            //pers.Phone = "215-456-7890";
            //pers.Email = "jane.doe@def.org";
            //pers.CreatedTime = System.DateTime.Now;
            //context.Persons.Add(pers);
            //context.SaveChanges();

            //pers.Name = "Smith, John";
            //pers.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            //pers.Phone = "215-456-7890";
            //pers.Email = "john.smith@def.org";
            //pers.CreatedTime = System.DateTime.Now;
            //context.Persons.Add(pers);
            //context.SaveChanges();

            //pers.Name = "Aaaaaa, John";
            //pers.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            //pers.Phone = "215-456-7890";
            //pers.Email = "john.smith@def.org";
            //pers.CreatedTime = System.DateTime.Now;
            //context.Persons.Add(pers);
            //context.SaveChanges();

            //pers.Name = "Bbbbb, John";
            //pers.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            //pers.Phone = "215-456-7890";
            //pers.Email = "john.smith@def.org";
            //pers.CreatedTime = System.DateTime.Now;
            //context.Persons.Add(pers);
            //context.SaveChanges();

            //pers.Name = "Ccccc, Hohn";
            //pers.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            //pers.Phone = "215-456-7890";
            //pers.Email = "john.smith@def.org";
            //pers.CreatedTime = System.DateTime.Now;
            //context.Persons.Add(pers);
            //context.SaveChanges();

            //pers.Name = "Dddddd, John";
            //pers.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            //pers.Phone = "215-456-7890";
            //pers.Email = "john.smith@def.org";
            //pers.CreatedTime = System.DateTime.Now;
            //context.Persons.Add(pers);
            //context.SaveChanges();

            //pers.Name = "Eeeeee, John";
            //pers.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            //pers.Phone = "215-456-7890";
            //pers.Email = "john.smith@def.org";
            //pers.CreatedTime = System.DateTime.Now;
            //context.Persons.Add(pers);
            //context.SaveChanges();

            //pers.Name = "Fffff, John";
            //pers.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            //pers.Phone = "215-456-7890";
            //pers.Email = "john.smith@def.org";
            //pers.CreatedTime = System.DateTime.Now;
            //context.Persons.Add(pers);
            //context.SaveChanges();

            //pers.Name = "Ggggggg, John";
            //pers.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            //pers.Phone = "215-456-7890";
            //pers.Email = "john.smith@def.org";
            //pers.CreatedTime = System.DateTime.Now;
            //context.Persons.Add(pers);
            //context.SaveChanges();

            //pers.Name = "Hhhhhhh, John";
            //pers.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            //pers.Phone = "215-456-7890";
            //pers.Email = "john.smith@def.org";
            //pers.CreatedTime = System.DateTime.Now;
            //context.Persons.Add(pers);
            //context.SaveChanges();

            //pers.FranchiseKey = "wrong";
            //pers.Inactive = false;
            //pers.Name = "Wrong Franchise";
            //pers.Address = "123 Anywhere St.\r\nPhiladelphia, PA 19000";
            //pers.Phone = "215-456-7890";
            //pers.Email = "jane.doe@def.org";
            //pers.CreatedTime = System.DateTime.Now;
            //context.Persons.Add(pers);
            //context.SaveChanges();

            //// Initialize rows of the Entity table to get it created.

            //Entity ent = new Entity();
            //ent.FranchiseKey = Franchise.LocalFranchiseKey;
            //ent.Inactive = false;
            //ent.EntityType = EntityType.Corporation;
            //ent.Name = "Acme Corporation";
            //ent.Address = "123 Anywhere St.\r\nPhiladelphia, PA 19000";
            //ent.Phone = "215-456-7890";
            //ent.Email = "jane.doe@def.org";
            //ent.CreatedTime = System.DateTime.Now;
            //context.Entitys.Add(ent);
            //context.SaveChanges();

            //ent.Name = "Baker Hughes";
            //ent.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            //ent.Phone = "215-456-7890";
            //ent.Email = "john.smith@def.org";
            //ent.CreatedTime = System.DateTime.Now;
            //context.Entitys.Add(ent);
            //context.SaveChanges();

            //ent.Name = "Caterpiller Corporation";
            //ent.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            //ent.Phone = "215-456-7890";
            //ent.Email = "john.smith@def.org";
            //ent.CreatedTime = System.DateTime.Now;
            //context.Entitys.Add(ent);
            //context.SaveChanges();

            //ent.Name = "Deere Equipment Corp";
            //ent.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            //ent.Phone = "215-456-7890";
            //ent.Email = "john.smith@def.org";
            //ent.CreatedTime = System.DateTime.Now;
            //context.Entitys.Add(ent);
            //context.SaveChanges();

            //ent.Name = "Eaton Corporation";
            //ent.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            //ent.Phone = "215-456-7890";
            //ent.Email = "john.smith@def.org";
            //ent.CreatedTime = System.DateTime.Now;
            //context.Entitys.Add(ent);
            //context.SaveChanges();

            //ent.Name = "Fluor Construction";
            //ent.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            //ent.Phone = "215-456-7890";
            //ent.Email = "john.smith@def.org";
            //ent.CreatedTime = System.DateTime.Now;
            //context.Entitys.Add(ent);
            //context.SaveChanges();

            //ent.Name = "General Mills";
            //ent.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            //ent.Phone = "215-456-7890";
            //ent.Email = "john.smith@def.org";
            //ent.CreatedTime = System.DateTime.Now;
            //context.Entitys.Add(ent);
            //context.SaveChanges();

            //ent.Name = "Harris Corporation";
            //ent.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            //ent.Phone = "215-456-7890";
            //ent.Email = "john.smith@def.org";
            //ent.CreatedTime = System.DateTime.Now;
            //context.Entitys.Add(ent);
            //context.SaveChanges();

            //ent.Name = "IBM Corporation";
            //ent.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            //ent.Phone = "215-456-7890";
            //ent.Email = "john.smith@def.org";
            //ent.CreatedTime = System.DateTime.Now;
            //context.Entitys.Add(ent);
            //context.SaveChanges();

            //ent.FranchiseKey = "wrong";
            //ent.Inactive = false;
            //ent.Name = "Wrong Franchise";
            //ent.Address = "123 Anywhere St.\r\nPhiladelphia, PA 19000";
            //ent.Phone = "215-456-7890";
            //ent.Email = "jane.doe@def.org";
            //ent.CreatedTime = System.DateTime.Now;
            //context.Entitys.Add(ent);
            //context.SaveChanges();

            // Initialize one row of Franchise table

            Franchise fran = new Franchise();
            fran.FranchiseKey = Franchise.LocalFranchiseKey;
            fran.Inactive = false;
            fran.Name = "CultureWorks Greater Philadelphia";
            fran.Description = "The one and only";
            fran.CreatedTime = System.DateTime.Now;
            fran.LicenseExpiration = System.DateTime.Now.AddYears(100);
            context.Franchises.Add(fran);
            context.SaveChanges();

            fran.FranchiseKey = "Wrong";
            fran.Inactive = false;
            fran.Name = "Wrong Franchise";
            fran.Description = "Should never appear";
            fran.CreatedTime = System.DateTime.Now;
            fran.LicenseExpiration = System.DateTime.Now.AddYears(100);
            context.Franchises.Add(fran);
            context.SaveChanges();

            // Initialize rows of General Ledger table

            GLCode gl = new GLCode();
            gl.FranchiseKey = Franchise.LocalFranchiseKey;
            gl.Inactive = false;
            gl.ExpCode = false; gl.DepCode = true; gl.CreatedTime = System.DateTime.Now;

            gl.Code = "41000 — RESTRICTED REVENUE";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "41100 — Govt / Local";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "41200 — State";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "41300 — Federal";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "42000 — CONTRIBUTED REVENUE";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "42100 — Foundation";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "42200 — Government";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "42210 — Federal";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "42220 — State";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "42230 — Local";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "42300 — Individual";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "42400 — Corporate";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "44000 — EARNED REVENUE";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "44010 — Admissions";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "44012 — Merchandise";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "44013 — Professional Fees";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "44014 — Licensing / Royalty Fees";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "44015 — Space / Equipment Rental";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "44016 — Investments";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "44017 — Advertising";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "44018 — Sponsorship";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "44019 — CoProducing";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "44020 — Submission Fees";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "44021 — Materials";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "44022 — Memberships";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "44023 — Vendor Fees";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "45000 — IN - KIND";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "45010 — Donated Services";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "45011 — Donated Materials";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "45012 — Donated Equipment/ Goods";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "45013 — Donated Space";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "45014 — Volunteer Time";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "46000 — MISCELLANEOUS";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.ExpCode = true; gl.DepCode = false; gl.CreatedTime = System.DateTime.Now;

            gl.Code = "60000 — SALARIES(W - 2 Employee)";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "61000 — FRINGE";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "61010 — Payroll Taxes";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "61012 — Health Benefits";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "61013 — Retirement";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "61014 — Professional Develop/ Edu";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "61015 — Merrit Based Compensation";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "62000 — PROFESSIONAL FEES (1099)";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "62010 — Artistic & Curatorial Fees";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "62011 — Design Fees";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "62012 — Fundraising";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "62013 — Marketing / PR";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "62014 — Legal";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "62015 — Documentation";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "62016 — Production";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "62017 — Research / Evaluation";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "62018 — Accounting / Finance";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "62019 — Fabrication";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "62020 — General Consulting";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "62021 — Teaching";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "62022 — Administration";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "62023 — Development";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "62024 — Prize Money";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "63000 — PROGRAM / GENERAL ADMIN";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "63010 — CW Coworking Membership";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "63011 — Common Pool Management Fees";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "63012 — Licenses, Permits, Visas";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "63014 — Rights & Reproductions";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "63015 — Venue Fees(Rent / Lease)";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "63016 — Project Materials";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "63017 — Merchandise Production";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "63018 — Postage & Shipping";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "63019 — Printing and Publication";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "63000 — PROGRAM / GENERAL ADMIN: 63019 — Printing & Publications";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "63020 — Office Supplies & Expendables";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "63021 — Software Purchase & Licensing";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "63022 — Telephone";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "63023 — Internet & IT";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "63024 — Advertising(ad purchasing)";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "63025 — Dues & Subscriptions";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "63026 — Bank & Merchant Service Fees";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "63027 — Sales Tax";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "63028 — Fees";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "63029 — Compliance";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "63030 — Admission Fees";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "63031 — Special Insurances";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "63032 — Equipment Rental";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "63033 — Professional Development";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "63034 — Contributions";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "63035 — Utilities";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "64000 — TRAVEL & MEETING EXPENSES";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "64010 — Transportation";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "64011 — Accommodations";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "64012 — Meals / Food / Catering";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "64013 — Gift Cards / Cash Box / PEX";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "65000 — OWNED FACILITY COSTS";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "65010 — Mortgage";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "65011 — Utilities";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "65012 — Maintenance";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "65013 — Property Taxes";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "66000 — CAPITAL COSTS";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "66010 — Working Capital";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "66012 — Endowment";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "66013 — Leasehold Improvements";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "66014 — Equipment";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "67000 — IN - KIND(expense)";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "67010 — Donated Services";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "67011 — Donated Materials";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "67012 — Donated Equipment & Goods";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "67013 — Donated Space";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "67014 — Volunteer Time";
            context.GLCodes.Add(gl); context.SaveChanges();

            gl.Code = "68000 — REGRANTING ACTIVITY";
            context.GLCodes.Add(gl); context.SaveChanges();

            // Initialize rows of Grant Maker table

            //GrantMaker gm = new GrantMaker();
            //gm.Name = "Grant Maker A";
            //context.GrantMakers.Add(gm);
            //context.SaveChanges();

            //gm.Name = "Grant Maker BB";
            //context.GrantMakers.Add(gm);
            //context.SaveChanges();

            //gm.Name = "Grant Maker CCC";
            //context.GrantMakers.Add(gm);
            //context.SaveChanges();

            //gm.Name = "Grant Maker DDDD";
            //context.GrantMakers.Add(gm);
            //context.SaveChanges();

            //gm.Name = "Grant Maker EEEEE";
            //context.GrantMakers.Add(gm);
            //context.SaveChanges();

            //gm.Name = "Grant Maker FFFFFF";
            //context.GrantMakers.Add(gm);
            //context.SaveChanges();

            //gm.Name = "Grant Maker GGGGGGG";
            //context.GrantMakers.Add(gm);
            //context.SaveChanges();

            //gm.Name = "Grant Maker HHHHHHHH";
            //context.GrantMakers.Add(gm);
            //context.SaveChanges();

            //gm.Name = "Grant Maker IIIIIIIII";
            //context.GrantMakers.Add(gm);
            //context.SaveChanges();

            //gm.Name = "Grant Maker JJJJJJJJJJ";
            //context.GrantMakers.Add(gm);
            //context.SaveChanges();

            //gm.Name = "Grant Maker KKKKKKKKKKK";
            //context.GrantMakers.Add(gm);
            //context.SaveChanges();

            // Initialize rows of Grant table

            //Grant g = new Grant();
            //g.Name = "Grant Maker A, Grant 1";
            //g.CreatedTime = System.DateTime.Now; g.OriginalFunds = 1000; g.CurrentFunds = 500; g.GrantMakerID = 1;
            //context.Grants.Add(g);
            //context.SaveChanges();

            //g.Name = "Grant Maker A, Grant 1";
            //g.CreatedTime = System.DateTime.Now; g.OriginalFunds = 1000; g.CurrentFunds = 500; g.GrantMakerID = 1;
            //context.Grants.Add(g);
            //context.SaveChanges();

            //g.Name = "Grant Maker A, Grant 1";
            //g.CreatedTime = System.DateTime.Now; g.OriginalFunds = 1000; g.CurrentFunds = 500; g.GrantMakerID = 1;
            //context.Grants.Add(g);
            //context.SaveChanges();

            //g.Name = "Grant Maker A, Grant 2";
            //g.CreatedTime = System.DateTime.Now; g.OriginalFunds = 2000; g.CurrentFunds = 500; g.GrantMakerID = 1;
            //context.Grants.Add(g);
            //context.SaveChanges();

            //g.Name = "Grant Maker A, Grant 3";
            //g.CreatedTime = System.DateTime.Now; g.OriginalFunds = 3000; g.CurrentFunds = 500; g.GrantMakerID = 1;
            //context.Grants.Add(g);
            //context.SaveChanges();

            //g.Name = "Grant Maker B, Grant 1";
            //g.CreatedTime = System.DateTime.Now; g.OriginalFunds = 1100; g.CurrentFunds = 500; g.GrantMakerID = 2;
            //context.Grants.Add(g);
            //context.SaveChanges();

            //g.Name = "Grant Maker B, Grant 2";
            //g.CreatedTime = System.DateTime.Now; g.OriginalFunds = 2100; g.CurrentFunds = 500; g.GrantMakerID = 2;
            //context.Grants.Add(g);
            //context.SaveChanges();

            // Initialize the ProjectClassMaster table

            ProjectClassMaster pcm = new ProjectClassMaster();
            pcm.FranchiseKey = Franchise.LocalFranchiseKey;
            pcm.Inactive = false;
            pcm.Name = "Fundraising";
            pcm.Description = "";
            pcm.CreatedTime = System.DateTime.Now;
            pcm.Unrestricted = false;
            context.ProjectClassMasters.Add(pcm);
            context.SaveChanges();

            pcm.Name = "General Operating";
            pcm.Description = "";
            pcm.CreatedTime = System.DateTime.Now;
            pcm.Unrestricted = true;
            context.ProjectClassMasters.Add(pcm);
            context.SaveChanges();

            pcm.Name = "Marketing";
            pcm.Description = "";
            pcm.CreatedTime = System.DateTime.Now;
            pcm.Unrestricted = false;
            context.ProjectClassMasters.Add(pcm);
            context.SaveChanges();

            pcm.Name = "Program - General";
            pcm.Description = "";
            pcm.CreatedTime = System.DateTime.Now;
            pcm.Unrestricted = false;
            context.ProjectClassMasters.Add(pcm);
            context.SaveChanges();

            pcm.FranchiseKey = "Wrong";
            pcm.Inactive = false;
            pcm.Name = "Wrong Franchise";
            pcm.Description = "";
            pcm.CreatedTime = System.DateTime.Now;
            pcm.Unrestricted = false;
            context.ProjectClassMasters.Add(pcm);
            context.SaveChanges();

            // Initialize one row of the Project table to get it created.

            //Project proj = new Project();
            //proj.FranchiseKey = Franchise.LocalFranchiseKey;
            //proj.Inactive = false;
            //proj.Name = "Manhattan Project";
            //proj.Description = "A sample project for testing purposes. It is really a really, really important, but not well respected, project.";
            //proj.CreatedTime = System.DateTime.Now;  proj.BalanceDate = System.DateTime.Now; proj.CurrentFunds = 123.45M; // proj.RestrictedGrants = 543.21M;
            //context.Projects.Add(proj);
            //context.SaveChanges();
            //ProjectClassActions.AddMasterProjectClasses(proj.ProjectID);        // Create Project Class rows from Master Project Class table

            //proj.Name = "AAA Project";
            //proj.Description = "A sample project for testing purposes. It is really a really important, but not well respected, project.";
            //proj.CreatedTime = System.DateTime.Now; proj.BalanceDate = System.DateTime.Now; proj.CurrentFunds = 123.45M; // proj.RestrictedGrants = 543.21M;
            //context.Projects.Add(proj);
            //context.SaveChanges();
            //ProjectClassActions.AddMasterProjectClasses(proj.ProjectID);        // Create Project Class rows from Master Project Class table

            //proj.Name = "The Head & the Hand Workshop";
            //proj.Description = "A sample project for testing purposes. It is really an important, and the epitome of well respected, project.";
            //proj.CreatedTime = System.DateTime.Now; proj.BalanceDate = System.DateTime.Now; proj.CurrentFunds = 123.45M; // proj.RestrictedGrants = 543.21M;
            //context.Projects.Add(proj);
            //context.SaveChanges();
            //ProjectClassActions.AddMasterProjectClasses(proj.ProjectID);        // Create Project Class rows from Master Project Class table

            //proj.Name = "Philadelphia Choral Collective";
            //proj.Description = "A sample project for testing purposes. It is really an important, and the epitome of well respected, project.";
            //proj.CreatedTime = System.DateTime.Now; proj.BalanceDate = System.DateTime.Now; proj.CurrentFunds = 123.45M; // proj.RestrictedGrants = 543.21M;
            //context.Projects.Add(proj);
            //context.SaveChanges();
            //ProjectClassActions.AddMasterProjectClasses(proj.ProjectID);        // Create Project Class rows from Master Project Class table
            
            //proj.Name = "Calm Clarity";
            //proj.Description = "A sample project for testing purposes. It is really an important, and the epitome of well respected, project.";
            //proj.CreatedTime = System.DateTime.Now; proj.BalanceDate = System.DateTime.Now; proj.CurrentFunds = 123.45M; // proj.RestrictedGrants = 543.21M;
            //context.Projects.Add(proj);
            //context.SaveChanges();
            //ProjectClassActions.AddMasterProjectClasses(proj.ProjectID);        // Create Project Class rows from Master Project Class table
            
            //proj.Name = "215 Festival";
            //proj.Description = "A sample project for testing purposes. It is really an important, and the epitome of well respected, project.";
            //proj.CreatedTime = System.DateTime.Now; proj.BalanceDate = System.DateTime.Now; proj.CurrentFunds = 123.45M; // proj.RestrictedGrants = 543.21M;
            //context.Projects.Add(proj);
            //context.SaveChanges();
            //ProjectClassActions.AddMasterProjectClasses(proj.ProjectID);        // Create Project Class rows from Master Project Class table
            
            //proj.Name = "III Project";
            //proj.Description = "A sample project for testing purposes. It is just a project.";
            //proj.CreatedTime = System.DateTime.Now; proj.BalanceDate = System.DateTime.Now; proj.CurrentFunds = 123.45M; // proj.RestrictedGrants = 543.21M;
            //context.Projects.Add(proj);
            //context.SaveChanges();
            //ProjectClassActions.AddMasterProjectClasses(proj.ProjectID);        // Create Project Class rows from Master Project Class table

            //proj.FranchiseKey = "Wrong";
            //proj.Inactive = false;
            //proj.Name = "Wrong Franchise";
            //proj.Description = "Should never appear";
            //proj.CreatedTime = System.DateTime.Now; proj.BalanceDate = System.DateTime.Now; proj.CurrentFunds = 123.45M; // proj.RestrictedGrants = 543.21M;
            //context.Projects.Add(proj);
            //context.SaveChanges();
            //ProjectClassActions.AddMasterProjectClasses(proj.ProjectID);        // Create Project Class rows from Master Project Class table

            // Initialize a few rows of the Vendor table to get it created.

            //Vendor ven = new Vendor();
            //ven.FranchiseKey = Franchise.LocalFranchiseKey;
            //ven.Inactive = false;
            //ven.Name = "ABC Vending Company";
            //ven.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            //ven.Phone = "215-456-7890";
            //ven.Email = "abc@def.org";
            //ven.CreatedTime = System.DateTime.Now;
            //context.Vendors.Add(ven);
            //context.SaveChanges();

            //ven.Name = "DEF Vending Company";
            //ven.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            //ven.Phone = "215-456-7890";
            //ven.Email = "abc@def.org";
            //context.Vendors.Add(ven);
            //context.SaveChanges();

            //ven.Name = "GHI Vending Company";
            //ven.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            //ven.Phone = "215-456-7890";
            //ven.Email = "abc@def.org";
            //context.Vendors.Add(ven);
            //context.SaveChanges();

            //ven.Name = "JKL Vending Company";
            //ven.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            //ven.Phone = "215-456-7890";
            //ven.Email = "abc@def.org";
            //context.Vendors.Add(ven);
            //context.SaveChanges();

            //ven.Name = "MNO Vending Company";
            //ven.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            //ven.Phone = "215-456-7890";
            //ven.Email = "abc@def.org";
            //context.Vendors.Add(ven);
            //context.SaveChanges();

            //ven.Name = "PQR Vending Company";
            //ven.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            //ven.Phone = "215-456-7890";
            //ven.Email = "abc@def.org";
            //context.Vendors.Add(ven);
            //context.SaveChanges();

            //ven.Name = "STU Vending Company";
            //ven.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            //ven.Phone = "215-456-7890";
            //ven.Email = "abc@def.org";
            //context.Vendors.Add(ven);
            //context.SaveChanges();

            //ven.FranchiseKey = "Wrong";
            //ven.Inactive = false;
            //ven.Name = "Wrong franchise";
            //ven.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            //ven.Phone = "215-456-7890";
            //ven.Email = "abc@def.org";
            //context.Vendors.Add(ven);
            //context.SaveChanges();

            //// Initialize rows of the Deposit table to get it created.

            //Dep dep = new Dep()
            //{
            //    Inactive = false,
            //    DepType = DepType.Cash,
            //    ProjectID = 1,
            //    Description = "First Deposit",
            //    Amount = 1.23M,
            //    CreatedTime = System.DateTime.Now,
            //    CurrentState = DepState.Unsubmitted,
            //    CurrentTime = System.DateTime.Now

            //};
            //context.Deps.Add(dep);
            //context.SaveChanges();
        }
    }
}