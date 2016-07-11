using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Portal11.Logic;

namespace Portal11.Models
{
    public class PortalDatabaseInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {

            // These initializations are temporary to make testing easier. Sprinkle in a few rows for a bogus franchise to make sure they are excluded
            // from searching.

            // Initialize rows of the Person table to get it created.

            Person pers = new Person();
            pers.FranchiseKey = Franchise.LocalFranchiseKey;
            pers.Inactive = false;
            pers.Name = "Doe, Jane";
            pers.Address = "123 Anywhere St.\r\nPhiladelphia, PA 19000";
            pers.Phone = "215-456-7890";
            pers.Email = "jane.doe@def.org";
            pers.CreatedTime = System.DateTime.Now;
            context.Persons.Add(pers);
            context.SaveChanges();

            pers.Name = "Smith, John";
            pers.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            pers.Phone = "215-456-7890";
            pers.Email = "john.smith@def.org";
            pers.CreatedTime = System.DateTime.Now;
            context.Persons.Add(pers);
            context.SaveChanges();

            pers.Name = "Aaaaaa, John";
            pers.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            pers.Phone = "215-456-7890";
            pers.Email = "john.smith@def.org";
            pers.CreatedTime = System.DateTime.Now;
            context.Persons.Add(pers);
            context.SaveChanges();

            pers.Name = "Bbbbb, John";
            pers.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            pers.Phone = "215-456-7890";
            pers.Email = "john.smith@def.org";
            pers.CreatedTime = System.DateTime.Now;
            context.Persons.Add(pers);
            context.SaveChanges();

            pers.Name = "Ccccc, Hohn";
            pers.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            pers.Phone = "215-456-7890";
            pers.Email = "john.smith@def.org";
            pers.CreatedTime = System.DateTime.Now;
            context.Persons.Add(pers);
            context.SaveChanges();

            pers.Name = "Dddddd, John";
            pers.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            pers.Phone = "215-456-7890";
            pers.Email = "john.smith@def.org";
            pers.CreatedTime = System.DateTime.Now;
            context.Persons.Add(pers);
            context.SaveChanges();

            pers.Name = "Eeeeee, John";
            pers.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            pers.Phone = "215-456-7890";
            pers.Email = "john.smith@def.org";
            pers.CreatedTime = System.DateTime.Now;
            context.Persons.Add(pers);
            context.SaveChanges();

            pers.Name = "Fffff, John";
            pers.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            pers.Phone = "215-456-7890";
            pers.Email = "john.smith@def.org";
            pers.CreatedTime = System.DateTime.Now;
            context.Persons.Add(pers);
            context.SaveChanges();

            pers.Name = "Ggggggg, John";
            pers.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            pers.Phone = "215-456-7890";
            pers.Email = "john.smith@def.org";
            pers.CreatedTime = System.DateTime.Now;
            context.Persons.Add(pers);
            context.SaveChanges();

            pers.Name = "Hhhhhhh, John";
            pers.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            pers.Phone = "215-456-7890";
            pers.Email = "john.smith@def.org";
            pers.CreatedTime = System.DateTime.Now;
            context.Persons.Add(pers);
            context.SaveChanges();

            pers.FranchiseKey = "wrong";
            pers.Inactive = false;
            pers.Name = "Wrong Franchise";
            pers.Address = "123 Anywhere St.\r\nPhiladelphia, PA 19000";
            pers.Phone = "215-456-7890";
            pers.Email = "jane.doe@def.org";
            pers.CreatedTime = System.DateTime.Now;
            context.Persons.Add(pers);
            context.SaveChanges();

            // Initialize rows of the Entity table to get it created.

            Entity ent = new Entity();
            ent.FranchiseKey = Franchise.LocalFranchiseKey;
            ent.Inactive = false;
            ent.EntityType = EntityType.Corporation;
            ent.Name = "Acme Corporation";
            ent.Address = "123 Anywhere St.\r\nPhiladelphia, PA 19000";
            ent.Phone = "215-456-7890";
            ent.Email = "jane.doe@def.org";
            ent.CreatedTime = System.DateTime.Now;
            context.Entitys.Add(ent);
            context.SaveChanges();

            ent.Name = "Baker Hughes";
            ent.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            ent.Phone = "215-456-7890";
            ent.Email = "john.smith@def.org";
            ent.CreatedTime = System.DateTime.Now;
            context.Entitys.Add(ent);
            context.SaveChanges();

            ent.Name = "Caterpiller Corporation";
            ent.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            ent.Phone = "215-456-7890";
            ent.Email = "john.smith@def.org";
            ent.CreatedTime = System.DateTime.Now;
            context.Entitys.Add(ent);
            context.SaveChanges();

            ent.Name = "Deere Equipment Corp";
            ent.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            ent.Phone = "215-456-7890";
            ent.Email = "john.smith@def.org";
            ent.CreatedTime = System.DateTime.Now;
            context.Entitys.Add(ent);
            context.SaveChanges();

            ent.Name = "Eaton Corporation";
            ent.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            ent.Phone = "215-456-7890";
            ent.Email = "john.smith@def.org";
            ent.CreatedTime = System.DateTime.Now;
            context.Entitys.Add(ent);
            context.SaveChanges();

            ent.Name = "Fluor Construction";
            ent.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            ent.Phone = "215-456-7890";
            ent.Email = "john.smith@def.org";
            ent.CreatedTime = System.DateTime.Now;
            context.Entitys.Add(ent);
            context.SaveChanges();

            ent.Name = "General Mills";
            ent.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            ent.Phone = "215-456-7890";
            ent.Email = "john.smith@def.org";
            ent.CreatedTime = System.DateTime.Now;
            context.Entitys.Add(ent);
            context.SaveChanges();

            ent.Name = "Harris Corporation";
            ent.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            ent.Phone = "215-456-7890";
            ent.Email = "john.smith@def.org";
            ent.CreatedTime = System.DateTime.Now;
            context.Entitys.Add(ent);
            context.SaveChanges();

            ent.Name = "IBM Corporation";
            ent.Address = "123 Anywhere St\r\n Philadelphia, PA 19000";
            ent.Phone = "215-456-7890";
            ent.Email = "john.smith@def.org";
            ent.CreatedTime = System.DateTime.Now;
            context.Entitys.Add(ent);
            context.SaveChanges();

            ent.FranchiseKey = "wrong";
            ent.Inactive = false;
            ent.Name = "Wrong Franchise";
            ent.Address = "123 Anywhere St.\r\nPhiladelphia, PA 19000";
            ent.Phone = "215-456-7890";
            ent.Email = "jane.doe@def.org";
            ent.CreatedTime = System.DateTime.Now;
            context.Entitys.Add(ent);
            context.SaveChanges();

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
            gl.Code = "42100 - Foundation"; gl.ExpCode = true; gl.DepCode = false;
            gl.CreatedTime = System.DateTime.Now;
            context.GLCodes.Add(gl);
            context.SaveChanges();

            gl.Code = "42300 - Individual Donation"; gl.ExpCode = true; gl.DepCode = false;
            gl.CreatedTime = System.DateTime.Now;
            context.GLCodes.Add(gl);
            context.SaveChanges();

            gl.Code = "44015 - Space/Equipment Rental"; gl.ExpCode = true; gl.DepCode = false;
            gl.CreatedTime = System.DateTime.Now;
            context.GLCodes.Add(gl);
            context.SaveChanges();

            gl.Code = "45011 - Donated Materials"; gl.ExpCode = true; gl.DepCode = false;
            gl.CreatedTime = System.DateTime.Now;
            context.GLCodes.Add(gl);
            context.SaveChanges();

            gl.Code = "62010 - Artistic & Curatorial Fees"; gl.ExpCode = true; gl.DepCode = false;
            gl.CreatedTime = System.DateTime.Now;
            context.GLCodes.Add(gl);
            context.SaveChanges();

            gl.Code = "62021 - Teaching"; gl.ExpCode = true; gl.DepCode = false;
            gl.CreatedTime = System.DateTime.Now;
            context.GLCodes.Add(gl);
            context.SaveChanges();

            gl.Code = "63010 - CW Coworking Membership"; gl.ExpCode = true; gl.DepCode = false;
            gl.CreatedTime = System.DateTime.Now;
            context.GLCodes.Add(gl);
            context.SaveChanges();

            gl.Code = "63020 - Office Supplies & Expendables"; gl.ExpCode = true; gl.DepCode = false;
            gl.CreatedTime = System.DateTime.Now;
            context.GLCodes.Add(gl);
            context.SaveChanges();

            gl.Code = "12345 - Deposit Code 1"; gl.ExpCode = false; gl.DepCode = true;
            gl.CreatedTime = System.DateTime.Now;
            context.GLCodes.Add(gl);
            context.SaveChanges();

            gl.Code = "54321 - Deposit Code 2"; gl.ExpCode = false; gl.DepCode = true;
            gl.CreatedTime = System.DateTime.Now;
            context.GLCodes.Add(gl);
            context.SaveChanges();

            gl.FranchiseKey = "Wrong";
            gl.Inactive = false;
            gl.Code = "Wrong Franchise"; gl.ExpCode = true; gl.DepCode = false;
            gl.CreatedTime = System.DateTime.Now;
            context.GLCodes.Add(gl);
            context.SaveChanges();

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

            Project proj = new Project();
            proj.FranchiseKey = Franchise.LocalFranchiseKey;
            proj.Inactive = false;
            proj.Name = "Manhattan Project";
            proj.Description = "A sample project for testing purposes. It is really a really, really important, but not well respected, project.";
            proj.CreatedTime = System.DateTime.Now;  proj.BalanceDate = System.DateTime.Now; proj.CurrentFunds = 123.45M; // proj.RestrictedGrants = 543.21M;
            context.Projects.Add(proj);
            context.SaveChanges();
            ProjectClassActions.AddMasterProjectClasses(proj.ProjectID);        // Create Project Class rows from Master Project Class table

            proj.Name = "AAA Project";
            proj.Description = "A sample project for testing purposes. It is really a really important, but not well respected, project.";
            proj.CreatedTime = System.DateTime.Now; proj.BalanceDate = System.DateTime.Now; proj.CurrentFunds = 123.45M; // proj.RestrictedGrants = 543.21M;
            context.Projects.Add(proj);
            context.SaveChanges();
            ProjectClassActions.AddMasterProjectClasses(proj.ProjectID);        // Create Project Class rows from Master Project Class table

            proj.Name = "The Head & the Hand Workshop";
            proj.Description = "A sample project for testing purposes. It is really an important, and the epitome of well respected, project.";
            proj.CreatedTime = System.DateTime.Now; proj.BalanceDate = System.DateTime.Now; proj.CurrentFunds = 123.45M; // proj.RestrictedGrants = 543.21M;
            context.Projects.Add(proj);
            context.SaveChanges();
            ProjectClassActions.AddMasterProjectClasses(proj.ProjectID);        // Create Project Class rows from Master Project Class table

            proj.Name = "Philadelphia Choral Collective";
            proj.Description = "A sample project for testing purposes. It is really an important, and the epitome of well respected, project.";
            proj.CreatedTime = System.DateTime.Now; proj.BalanceDate = System.DateTime.Now; proj.CurrentFunds = 123.45M; // proj.RestrictedGrants = 543.21M;
            context.Projects.Add(proj);
            context.SaveChanges();
            ProjectClassActions.AddMasterProjectClasses(proj.ProjectID);        // Create Project Class rows from Master Project Class table
            
            proj.Name = "Calm Clarity";
            proj.Description = "A sample project for testing purposes. It is really an important, and the epitome of well respected, project.";
            proj.CreatedTime = System.DateTime.Now; proj.BalanceDate = System.DateTime.Now; proj.CurrentFunds = 123.45M; // proj.RestrictedGrants = 543.21M;
            context.Projects.Add(proj);
            context.SaveChanges();
            ProjectClassActions.AddMasterProjectClasses(proj.ProjectID);        // Create Project Class rows from Master Project Class table
            
            proj.Name = "215 Festival";
            proj.Description = "A sample project for testing purposes. It is really an important, and the epitome of well respected, project.";
            proj.CreatedTime = System.DateTime.Now; proj.BalanceDate = System.DateTime.Now; proj.CurrentFunds = 123.45M; // proj.RestrictedGrants = 543.21M;
            context.Projects.Add(proj);
            context.SaveChanges();
            ProjectClassActions.AddMasterProjectClasses(proj.ProjectID);        // Create Project Class rows from Master Project Class table
            
            proj.Name = "III Project";
            proj.Description = "A sample project for testing purposes. It is just a project.";
            proj.CreatedTime = System.DateTime.Now; proj.BalanceDate = System.DateTime.Now; proj.CurrentFunds = 123.45M; // proj.RestrictedGrants = 543.21M;
            context.Projects.Add(proj);
            context.SaveChanges();
            ProjectClassActions.AddMasterProjectClasses(proj.ProjectID);        // Create Project Class rows from Master Project Class table

            proj.FranchiseKey = "Wrong";
            proj.Inactive = false;
            proj.Name = "Wrong Franchise";
            proj.Description = "Should never appear";
            proj.CreatedTime = System.DateTime.Now; proj.BalanceDate = System.DateTime.Now; proj.CurrentFunds = 123.45M; // proj.RestrictedGrants = 543.21M;
            context.Projects.Add(proj);
            context.SaveChanges();
            ProjectClassActions.AddMasterProjectClasses(proj.ProjectID);        // Create Project Class rows from Master Project Class table

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