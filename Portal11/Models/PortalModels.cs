using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace Portal11.Models
{

    // One row of the GridView named AssignProjectView, used by AssignUserToProject

    public class AssignUserToProjectViewRow
    {
        public string RowID { get; set; }
        public ProjectRole ProjectRole { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CurrentPD { get; set; }
        public string UserRole { get; set; }
        public const int DescriptionLength = 40;
    }

    // One row of the GridView named EDHistoryView, used by EditDeposit, EditRequest, ReviewDeposit, and ReviewRequest

    public class EDHistoryViewRow
    {
        public DateTime Date { get; set; }
        public string FormerStatus { get; set; }
        public string EstablishedBy { get; set; }
        public string UpdatedStatus { get; set; }
        public string ReasonForChange { get; set; }
        public string ReturnNote { get; set; }
    }

    // One row of the GridView named AllPortalUsers, used by ListPortalUsers

    public class AllPortalUsersRow
    {
        public string UserID { get; set; }
        public const int RowIDCell = 0;
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserRoleDesc { get; set; }
        public string Administrator { get; set; }
        public int LoginCount { get; set; }
        public DateTime LastLogin { get; set; }
        public string Inactive { get; set; }
        public const int InactiveColumn = 7;
    }

    // One row of the GridView named AllPortalUsers, used by ListPortalUsers

    public class AllProjectsRow
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime BalanceDate { get; set; }
        public string CurrentFunds { get; set; }
        public string ProjectDirector { get; set; }
        public int TotalRequests { get; set; }
        public string Inactive { get; set; }
        public const int InactiveColumn = 8;
    }

    // One row of the GridView named ImportCSV, used by ImportProjectBalance

    public class ImportCSVRow
    {
        public string ProjectCode { get; set; }
        public string BalanceDate { get; set; }
        public string CurrentFunds { get; set; }
        public string Status { get; set; }
        public bool Error { get; set; }
    }

    // One row of the GridView named AllAppView, used by ProjectDashboard

    public class ProjectAppViewRow
    {
        public string RowID { get; set; }
        public const int RowIDCell = 0;
        public DateTime CurrentTime { get; set; }
        public string AppTypeDesc { get; set; }
        public string Description { get; set; }
        public AppState CurrentState { get; set; }
        public const int CurrentStateCell = 4;
        public string CurrentStateDesc { get; set; }
        public const int CurrentStateDescRow = 5;
        public string ReturnNote { get; set; }
        public bool Archived { get; set; }
    }

    // One row of the GridView named AllDepView, used by ProjectDashboard

    public class ProjectDepViewRow
    {
        public string RowID { get; set; }
        public const int RowIDCell = 0;
        public DateTime CurrentTime { get; set; }
        public string DepTypeDesc { get; set; }
        public string Description { get; set; }
        public string SourceOfFunds { get; set; }
        public string Amount { get; set; }
        public DepState CurrentState { get; set; }
        public const int CurrentStateCell = 6;
        public string CurrentStateDesc { get; set; }
        public const int CurrentStateDescRow = 7;
        public string ReturnNote { get; set; }
        public bool Archived { get; set; }
    }

    // One row of the GridView named AllExpView, used by ProjectDashboard

    public class ProjectExpViewRow
    {
        public string RowID { get; set; }
        public DateTime CurrentTime { get; set; }
        public string ExpTypeDesc { get; set; }
        public string Description { get; set; }
        public string Target { get; set; }
        public string Amount { get; set; }
        public ExpState CurrentState { get; set; }
        public const int CurrentStateCell = 6;
        public string CurrentStateDesc { get; set; }
        public const int CurrentStateDescRow = 7;
        public string ReturnNote { get; set; }
        public bool Archived { get; set; }
        public bool Rush { get; set; }
    }

    // One row of the GridViews named AllProjectView, used by SelectProject

    public class SelectProjectAllViewRow
    {
        public string ProjectID { get; set; }
        public string Inactive { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
        public const int InactiveColumn = 4;
    }

    public class SelectProjectUserViewRow
    {
        public string ProjectID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProjectRole { get; set; }
        public int Count { get; set; }
    }

    // One row of the GridView named StaffDepView, used by StaffDashboard

    public class StaffAppViewRow
    {
        public string RowID { get; set; }
        public const int RowIDCell = 0;
        public DateTime CurrentTime { get; set; }
        public string ProjectName { get; set; }
        public string AppTypeDesc { get; set; }
        public string AppReviewType { get; set; }
        public AppState CurrentState { get; set; }
        public const int CurrentStateCell = 5;
        public string CurrentStateDesc { get; set; }
        public string Owner { get; set; }
        public const int OwnerRow = 7;
        public string Description { get; set; }
        public string ReturnNote { get; set; }
        public bool Archived { get; set; }
    }

    public class StaffDepViewRow
    {
        public string RowID { get; set; }
        public const int RowIDCell = 0;
        public DateTime CurrentTime { get; set; }
        public string ProjectName { get; set; }
        public string DepTypeDesc { get; set; }
        public decimal Amount { get; set; }
        public DepState CurrentState { get; set; }
        public const int CurrentStateCell = 5;
        public string CurrentStateDesc { get; set; }
        public string Owner { get; set; }
        public const int OwnerRow = 7;
        public string Description { get; set; }
        public string ReturnNote { get; set; }
        public bool Archived { get; set; }
    }
    // One row of the GridView named StaffExpView, used by StaffDashboard

    public class StaffExpViewRow
    {
        public string RowID { get; set; }
        public const int RowIDCell = 0;
        public DateTime CurrentTime { get; set; }
        public string ProjectName { get; set; }
        public string ExpTypeDesc { get; set; }
        public decimal Amount { get; set; }
        public ExpState CurrentState { get; set; }
        public const int CurrentStateCell = 5;
        public string CurrentStateDesc { get; set; }
        public string Owner { get; set; }
        public const int OwnerRow = 7;
        public string Target { get; set; }
        public string Summary { get; set; }
        public string ReturnNote { get; set; }
        public bool Archived { get; set; }
        public bool Rush { get; set; }
    }

    // One row of the GridView named UserProjectView, used by AssignUserToProject

    public class UserProjectViewRow
    {
        public int ProjectID { get; set; }
        public ProjectRole ProjectRole { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
    }

    // How the Expensed materials get delivered

    public enum DeliveryMode
    {
        [Description("Hold for pickup")]
        Pickup,
        [Description("Mail to payee")]
        MailPayee,
        [Description("Mail to the below address")]
        MailAddress
    }
    public enum PODeliveryMode
    {
        [Description("Have the vendor hold this item. I will pick it up.")]
        Pickup,
        [Description("Have the item delivered to CultureWorks.")]
        DeliverCW,
        [Description("Have the item delivered to the below address.")]
        DeliverAddress
    }

    // The Approval Request - made by a CW Staff on a Project.

    public class App
    {
        public int AppID { get; set; }
        public bool Inactive { get; set; }
        public bool Archived { get; set; }
        public AppType AppType { get; set; }                // Will grow to include many types
        [Required]
        public int? ProjectID { get; set; }
        public virtual Project Project { get; set; }

        public AppReviewType AppReviewType { get; set; }    // Shouldn't grow

        [Required]
        public DateTime CreatedTime { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }                   // Free-form comments on the Request

        [DataType(DataType.MultilineText)]
        public string ReturnNote { get; set; }

        // The CURRENT state of the Request - where it is in the flow right now and who put it in this state
        [Required]
        public AppState CurrentState { get; set; }
        public DateTime CurrentTime { get; set; }
        // The User who took the most recent action on the Deposit
        public string CurrentUserID { get; set; }
        public virtual ApplicationUser CurrentUser { get; set; }
        // The User who originally submitted the Deposit. They get notified if it's returned
        public string SubmitUserID { get; set; }
        public virtual ApplicationUser SubmitUser { get; set; }

    }

    // The states that the Approval has gone through. Who did what when why to the Request

    public class AppHistory
    {
        public int AppHistoryID { get; set; }
        public int AppID { get; set; }
        public AppState PriorAppState { get; set; }
        public AppState NewAppState { get; set; }
        public DateTime HistoryTime { get; set; }
        public string HistoryUserID { get; set; }
        public virtual ApplicationUser HistoryUser { get; set; }
        public string HistoryNote { get; set; }
        public string ReturnNote { get; set; }
    }

    // The processing states that a Approval can go through

    public enum AppState
    {
        [Description("Unsubmitted (IC)")]
        UnsubmittedByInternalCoordinator = 1,
        [Description("Unsubmitted (PS)")]
        UnsubmittedByProjectStaff,
        [Description("Unsubmitted (PD)")]
        UnsubmittedByProjectDirector,
        [Description("Awaiting Project Director")]
        AwaitingProjectDirector,
        [Description("Awaiting Trust Director")]
        AwaitingTrustDirector,
        [Description("Awaiting Finance Director")]
        AwaitingFinanceDirector,
        [Description("Awaiting Trust Executive")]
        AwaitingTrustExecutive,
        [Description("Awaiting Internal Coordinator")]
        AwaitingInternalCoordinator,
        [Description("Approved")]
        Approved,
        [Description("Returned")]
        Returned,
        [Description("Error During Processing")]
        Error,
        [Description("Reserved For Future Use")]
        ReservedForFutureUse
    }

    // Each type of Approval fills different fields in the App object. This mapping is complicated and expressed in code.

    public enum AppReviewType
    {
        [Description("Express - no financial or executive review")]
        Express = 1,
        [Description("Full - include financial and executive review")]
        Full,
        [Description("Reserved For Future Use")]
        ReservedForFutureUse
    }

    // Each type of Approval fills different fields in the App object. This mapping is complicated and expressed in code.

    public enum AppType
    {
        [Description("Contract/Proposal of Work")]
        Contract = 1,
        [Description("Grant Proposal/Fundraising Materials")]
        Grant,
        [Description("Marketing/Fundraising Campaign Information")]
        Campaign,
        [Description("Certificate of Insurance Request")]
        Certificate,
        [Description("Financial Report/Analysis Request")]
        Report,
        [Description("Reserved for Future Use")]
        ReservedForFutureUse
    }

    // The Deposit - made by a CW Staff on a Project.

    public class Dep
    {
        public int DepID { get; set; }
        public bool Inactive { get; set; }
        public bool Archived { get; set; }
        public DepType DepType { get; set; }
        [Required]
        public int? ProjectID { get; set; }
        public virtual Project Project { get; set; }

        public decimal Amount { get; set; }

        [Required]
        public DateTime CreatedTime { get; set; }
        public DateTime DateOfDeposit { get; set; }         // Date on the check/EFT

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public SourceOfExpFunds DestOfFunds { get; set; }    // Where the Request sends its Funds
        public int? ProjectClassID { get; set; }
        public virtual ProjectClass ProjectClass { get; set; }

        public bool EntityNeeded { get; set; }              // Flag to indicate that a new Entity is needed
        public EntityRole EntityRole { get; set; }          // The Entity involved in this Request - so far, just DepositEntity
        public int? EntityID { get; set; }
        public virtual Entity Entity { get; set; }
        public bool EntityIsAnonymous { get; set; }
        public const string EntityAnonymous = "Anonymous";

        public int? GLCodeID { get; set; }
        public virtual GLCode GLCode { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }                   // Free-form comments on the Request


        public bool PledgePayment { get; set; }
        public const string OptionPledgePayment = "PledgePayment";

        [DataType(DataType.MultilineText)]
        public string ReturnNote { get; set; }

        public SourceOfDepFunds SourceOfFunds { get; set; } // Where the Request gets its funds
        public bool PersonNeeded { get; set; }              // Flag to ask CW Staff to create a new Person
        public PersonRole PersonRole { get; set; }          // The Person who will send funds via this Request - Donor/Customer
        public int? PersonID { get; set; }
        public virtual Person Person { get; set; }
        public bool PersonIsAnonymous { get; set; }
        public const string PersonAnonymous = "Anonymous";

        // The CURRENT state of the Deposit - where it is in the flow right now and who put it in this state
        [Required]
        public DepState CurrentState { get; set; }
        public DateTime CurrentTime { get; set; }
        // The User who took the most recent action on the Deposit
        public string CurrentUserID { get; set; }
        public virtual ApplicationUser CurrentUser { get; set; }
        // The User who originally submitted the Deposit. They get notified if it's returned
        public string SubmitUserID { get; set; }
        public virtual ApplicationUser SubmitUser { get; set; }

    }

    // The states that the Deposit has gone through. Who did what when why to the Request

    public class DepHistory
    {
        public int DepHistoryID { get; set; }
        public int DepID { get; set; }
        public DepState PriorDepState { get; set; }
        public DepState NewDepState { get; set; }
        public DateTime HistoryTime { get; set; }
        public string HistoryUserID { get; set; }
        public virtual ApplicationUser HistoryUser { get; set; }
        public string HistoryNote { get; set; }
        public string ReturnNote { get; set; }
    }

    // The processing states that a Deposit can go through

    public enum DepState
    {
        [Description("Unsubmitted (IC)")]
        UnsubmittedByInternalCoordinator = 1,
        [Description("Awaiting Project Director")]
        AwaitingProjectDirector,
        [Description("Awaiting Trust Director")]
        AwaitingTrustDirector,
        [Description("Awaiting Finance Director")]
        AwaitingFinanceDirector,
        [Description("Approved/Ready To Deposit")]
        ApprovedReadyToDeposit,
        [Description("Deposit Complete")]
        DepositComplete,
        [Description("Returned")]
        Returned,
        [Description("Error During Processing")]
        Error,
        [Description("Reserved For Future Use")]
        ReservedForFutureUse
    }

    // Each type of Deposit fills different fields in the Deposit object. This mapping is complicated and expressed in code.
    
    public enum DepType
    {
        [Description("Check")]
        Check = 1,
        [Description("EFT")]
        EFT,
        [Description("Cash")]
        Cash,
        [Description("Pledge/Contract")]
        Pledge,
        [Description("In kind")]
        InKind,
        [Description("Reserved For Future Use")]
        ReservedForFutureUse
    }

    // The Entities that Projects receive Deposits from

    public class Entity
    {
        public int EntityID { get; set; }
        [Required, StringLength(20), Index("FranchiseAndEntity", 1)]
        public string FranchiseKey { get; set; }
        public bool Inactive { get; set; }
        public const int InactiveColumn = 3;                // Column when displayed in GridView
        public EntityType EntityType { get; set; }
        [Required, StringLength(100), Index("FranchiseAndEntity", 2, IsUnique = true)]
        public string Name { get; set; }
        [StringLength(250), DataType(DataType.MultilineText)]
        public string Address { get; set; }
        [StringLength(20)]
        public string Phone { get; set; }
        [StringLength(100)]
        public string Email { get; set; }
        public string URL { get; set; }
//        public bool EINPresent { get; set; }
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }                   // Free-form comments
        public DateTime CreatedTime { get; set; }
    }

    // The Entity can assume many Roles in a Request. We store an EntityID in the Request, then explain which type of Entity it is.

    public enum EntityRole
    {
        [Description("Entity (for Deposit)")]
        DepositEntity = 1,
        [Description("Vendor (for Expense)")]
        ExpenseVendor,
        [Description("Reserved for Future Use")]
        ReservedForFutureUse
    }

    // The types of Entities.

    public enum EntityType
    {
        [Description("Corporation")]
        Corporation = 1,
        [Description("Foundation")]
        Foundation,
        [Description("Government Agency")]
        GovernmentAgency,
        [Description("Reserved For Future Use")]
        ReservedForFutureUse
    }

    // The Expense - made by a User on a Project to CW Staff. This is why we built this system. (Originally called "Rqst".)
    // This record is sufficiently complex that its fields are listed alphabetically, rather than logically. That makes it easier to double-check
    // that every field is being handled as the record gets loaded and unloaded. On screen, the fields appear in an order that's logical to the user.

    public class Exp
    {
        public int ExpID { get; set; }
        public bool Inactive { get; set; }
        public bool Archived { get; set; }

        public decimal Amount { get; set; }                 // Total funds in this Request. For Paycheck, this is Gross Pay

        public DateTime BeginningDate { get; set; }         // Pay Period for Paycheck
        public DateTime EndingDate { get; set; }

        public int CardsQuantity { get; set; }              // Number of PEX Cards and value of each
        public decimal CardsValueEach { get; set; }


        public YesNo ContractOnFile { get; set; }            // Series of questions about Contract
        [DataType(DataType.MultilineText)]
        public string ContractReason { get; set; }
        public YesNo ContractComing { get; set; }

        [Required]
        public DateTime CreatedTime { get; set; }           // When the Expense was first created

        public DateTime DateOfInvoice { get; set; }         // Date Invoice was received

        public DateTime DateNeeded { get; set; }            // Date needed for PEX Cards

        [StringLength(250), DataType(DataType.MultilineText)]
        public string DeliveryAddress { get; set; }         // Delivery Instructions, special for Purchase Order
        public DeliveryMode DeliveryMode { get; set; }
        public PODeliveryMode PODeliveryMode { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }             // A longer, multi-line free text field. Could use as Notes

        public bool EntityNeeded { get; set; }              // Flag to indicate that a new Entity is needed
        public EntityRole EntityRole { get; set; }          // The Entity involved in this Request - so far, just Vendor
        public int? EntityID { get; set; }                  // The Vendor to receive Invoice or Purchase Order
        public virtual Entity Entity { get; set; }

        public ExpType ExpType { get; set; }

        public int? GLCodeID { get; set; }                  // General Ledger Code
        public virtual GLCode GLCode { get; set; }

        public string GoodsDescription { get; set; }        // Several fields to describe Goods
        public string GoodsSKU { get; set; }
        public int GoodsQuantity { get; set; }
        public decimal GoodsCostPerUnit { get; set; }

        public string InvoiceNumber { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }                   // Free-form comments on the Request

        public PaymentMethod PaymentMethod { get; set; }    // How the Expense Request gets paid

        public bool PersonNeeded { get; set; }              // Flag to indicate that a new Person is needed
        public PersonRole PersonRole { get; set; }          // The Person who will receive funds via this Request - Contractor/Employee/Responsible/Recipient
        public int? PersonID { get; set; }
        public virtual Person Person { get; set; }

        public YesNo POVendorMode { get; set; }             // PO Fulfilment Instructions

        [Required]
        public int? ProjectID { get; set; }                 // The Project that owns this Request
        public virtual Project Project { get; set; }

        [DataType(DataType.MultilineText)]
        public string ReturnNote { get; set; }              // When approval is denied, the reason goes here

        public bool Rush { get; set; }                      // Whether the Request has "Rush" status
        public const string DeliveryInstructionsRush = "Rush";

        public SourceOfExpFunds SourceOfFunds { get; set; } // Where the Request gets its Funds
        public int? ProjectClassID { get; set; }
        public virtual ProjectClass ProjectClass { get; set; }

        //TODO: Delete Summary
        [StringLength(30)]
        public string Summary { get; set; }                 // A free text name for the Expense

        //TODO: Delete URL?
        public string URL { get; set; }                     // For the item in a Purchase Order

        // The CURRENT state of the Expense - where it is in the flow right now and who put it in this state. See ExpHistory for how it got here
        [Required]
        public ExpState CurrentState { get; set; }
        public DateTime CurrentTime { get; set; }           // The time of the most recent action
        public string CurrentUserID { get; set; }           // The User who took the most recent action on the Exp
        public virtual ApplicationUser CurrentUser { get; set; }
        public string SubmitUserID { get; set; }            // The User who originally submitted the Exp. They get notified if it's returned
        public virtual ApplicationUser SubmitUser { get; set; }
    }

    // The states that the Expense has gone through. Who did what when why to the Request

    public class ExpHistory
    {
        public int ExpHistoryID { get; set; }
        public int ExpID { get; set; }
        public ExpState PriorExpState { get; set; }
        public ExpState NewExpState { get; set; }
        public DateTime HistoryTime { get; set; }
        public string HistoryUserID { get; set; }
        public virtual ApplicationUser HistoryUser { get; set; }
        public string HistoryNote { get; set; }
        public string ReturnNote { get; set; }
    }

    // The processing states that a Expense can go through

    public enum ExpState
    {
        [Description("Unsubmitted (IC)")]
        UnsubmittedByInternalCoordinator = 1,
        [Description("Unsubmitted (PS)")]
        UnsubmittedByProjectStaff,
        [Description("Unsubmitted (PD)")]
        UnsubmittedByProjectDirector,
        [Description("Awaiting Project Director")]
        AwaitingProjectDirector,
        [Description("Awaiting Trust Director")]
        AwaitingTrustDirector,
        [Description("Awaiting Finance Director")]
        AwaitingFinanceDirector,
        [Description("Awaiting Trust Executive")]
        AwaitingTrustExecutive,
        [Description("Approved/Ready to Pay")]
        Approved,
        [Description("Payment Sent")]
        PaymentSent,
        [Description("Paid and Sent")]
        Paid,
        [Description("Returned")]
        Returned,
        [Description("Error During Processing")]
        Error,
        [Description("Reserved For Future Use")]
        ReservedForFutureUse
    }
    // Each type of Expense fills different fields in the Expense object. This mapping is complicated and expressed in code.
    public enum ExpType
    {
        [Description("Contractor Invoice")]
        ContractorInvoice = 1,
        [Description("PEX Card")]
        PEXCard,
        [Description("Paycheck")]
        Paycheck,
        [Description("Purchase Order")]
        PurchaseOrder,
        [Description("Reimbursement")]
        Reimbursement,
        [Description("Vendor Invoice")]
        VendorInvoice
    }

    // The Franchises that run this product

    public class Franchise
    {
        public int FranchiseID { get; set; }
        public bool Inactive { get; set; }
        [Required]
        public string FranchiseKey { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Required]
        public DateTime CreatedTime { get; set; }
        public DateTime LicenseExpiration { get; set; }

        public const string LocalFranchiseKey = "Philadelphia";
        public int ForceReload { get; set; }        // Recreate database
    }

    // General Ledger accounting codes

    public class GLCode
    {
        public int GLCodeID { get; set; }
        [Required, StringLength(20), Index("FranchiseAndGLCode", 1)]
        public string FranchiseKey { get; set; }
        public bool Inactive { get; set; }
        public const int InactiveColumn = 5;                // Column when displayed in GridView
        public bool ExpCode { get; set; }
        public bool DepCode { get; set; }
        [Required, StringLength(100), Index("FranchiseAndGLCode", 2, IsUnique = true)]
        public string Code { get; set; }
        public string Description { get; set; }
        [Required]
        public DateTime CreatedTime { get; set; }
    }

    // A Grant from a Grant Maker to a Project

    public class Grant
    {
        public int GrantID { get; set; }
        public bool Inactive { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Required]
        public DateTime CreatedTime { get; set; }
        public decimal OriginalFunds { get; set; }
        public decimal CurrentFunds { get; set; }
        public int GrantMakerID { get; set; }
        public virtual GrantMaker GrantMaker { get; set; }
    }

    // The entities that make Grants to Projects

    public class GrantMaker
    {
        public int GrantMakerID { get; set; }
        public bool Inactive { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        [StringLength(250), DataType(DataType.MultilineText)]
        public string Address { get; set; }
        [StringLength(50)]
        public string Contact { get; set; }
        [StringLength(20)]
        public string Phone { get; set; }
        [StringLength(100)]
        public string Email { get; set; }
    }

    // A Person, who can take on the role of Contractor, Employee, Responsible, or Recipient on a project-by-project basis

    public class Person
    {
        public int PersonID { get; set; }
        [Required, StringLength(20), Index("FranchiseAndPerson", 1)]
        public string FranchiseKey { get; set; }
        public bool Inactive { get; set; }
        public const int InactiveColumn = 4;                // Column when displayed in GridView
        [Required, StringLength(100), Index("FranchiseAndPerson", 2, IsUnique = true)]
        public string Name { get; set; }
        [StringLength(250), DataType(DataType.MultilineText)]
        public string Address { get; set; }
        [StringLength(20)]
        public string Phone { get; set; }
        [StringLength(100)]
        public string Email { get; set; }
        public string EINSSN { get; set; }
        public bool W9Present { get; set; }
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }                   // Free-form comments
        public DateTime CreatedTime { get; set; }
        public string TriggerReload { get; set; } // Just a dummy to force database recreate and reload
    }

    // An Expense Request can be paid in a number of ways

    public enum PaymentMethod
    {
        [Description("Check")]
        Check = 1,
        [Description("CultureTrust Credit Card")]
        CreditCard,
        [Description("EFT/Direct Deposit")]
        EFT,
        [Description("Request Invoice")]
        Invoice
    }

    // The Person can assume many Roles in a Request. We store a PersonID in the Request, then explain which type of Person it is.

    public enum PersonRole
    {
        [Description("Contractor")]
        Contractor = 1,
        [Description("Donor")]
        Donor,
        [Description("Employee")]
        Employee,
        [Description("Responsible Person")]
        ResponsiblePerson,
        [Description("Recipient")]
        Recipient
    }

    // The Projects that make the world go 'round

    public class Project
    {
        public int ProjectID { get; set; }
        [Required, StringLength(20), Index("FranchiseAndProject", 1)
        //    , Index("FranchiseAndProjectCode",1)
            ]
        public string FranchiseKey { get; set; }
        public bool Inactive { get; set; }
        [Required, StringLength(100), Index("FranchiseAndProject", 2, IsUnique = true)]
        public string Name { get; set; }
        [StringLength(3)]
        //[Required, StringLength(3), Index("FranchiseAndProjectCode", 2, IsUnique = true)]
        public string Code { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Required]
        public DateTime CreatedTime { get; set; }
        public DateTime BalanceDate { get; set; }
        public decimal CurrentFunds { get; set; }
    }

    // The project classes of a project

    public class ProjectClass
    {
        public int ProjectClassID { get; set; }
        [Required]
        public bool Inactive { get; set; }
        public const int InactiveColumn = 5;                // Column when displayed in GridView
        [Required]
        public int ProjectID { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public DateTime CreatedTime { get; set; }
        public bool CreatedFromMaster { get; set; }
        public bool Default { get; set; }
    }

    // The master project classes of a project - these project classes are copied into every project when it is created

    public class ProjectClassMaster
    {
        public int ProjectClassMasterID { get; set; }
        [Required, StringLength(20), Index("FranchiseAndPCM", 1)]
        public string FranchiseKey { get; set; }
        public bool Inactive { get; set; }
        [Required, StringLength(100), Index("FranchiseAndPCM", 2, IsUnique = true)]
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public DateTime CreatedTime { get; set; }
        public bool Unrestricted { get; set; }
    }

    // Many-to-many connector for Projects and Entities. So far, there's no distinction among Entity types, e.g., Corporation, 
    // Foundation, Government Agency, but just you wait!

    public class ProjectEntity
    {
        public int ProjectEntityID { get; set; }
        public DateTime StartDate { get; set; }
        public int ProjectID { get; set; }
        public int EntityID { get; set; }
        public virtual Entity Entity { get; set; }
        public EntityRole EntityRole { get; set; }
    }

    // Many-to-many connector for Projects and Grants

    public class ProjectGrant
    {
        public int ProjectGrantID { get; set; }
        public DateTime StartDate { get; set; }
        public decimal OriginalAmount { get; set; }     // Available for future use to support partial assignment of grant funds
        public decimal RemainingAmount { get; set; }    // Available for future use
        public int ProjectID { get; set; }
        public int GrantID { get; set; }
        public virtual Grant Grant { get; set; }
    }

    // Many-to-many connector for Projects and Persons. Each Project has its own collection of Contractors, Donors, Employees, Responsibles and Recipients.
    // Each project has a set of ProjectPerson rows, one set for each PersonRole.

    public class ProjectPerson
    {
        public int ProjectPersonID { get; set; }
        public DateTime StartDate { get; set; }
        public int ProjectID { get; set; }
        public int PersonID { get; set; }
        public virtual Person Person { get; set; }
        public PersonRole PersonRole { get; set; }
    }

    // Classes to connect Users to Projects

    public enum ProjectRole
    {
        [Description("Internal Coordinator")]
        InternalCoordinator = 1,
        [Description("Project Director")]
        ProjectDirector,
        [Description("Project Staff")]
        ProjectStaff,
        [Description("No Role")]
        NoRole
    }

    // The value of a Query String, parsed into string and integer forms

    public class QSValue
    {
        public string String { get; set; }
        public int Int { get; set; }
    }

    // The states that the Request has gone through. Who did what when why to the Request.
    // This is an experiment - to see if a single class can serve all three request types

    public class RequestHistory
    {
        public int RequestHistoryID { get; set; }
        public int RequestID { get; set; }
        public RequestType RequestType { get; set; }
        public int PriorState { get; set; }                             // Could be AppState, DepState, ExpState
        public int NewState { get; set; }
        public DateTime HistoryTime { get; set; }
        public string HistoryUserID { get; set; }
        public virtual ApplicationUser HistoryUser { get; set; }
        public string HistoryNote { get; set; }
        public string ReturnNote { get; set; }
    }

    // Types of Requests.
    public enum RequestType
    {
        [Description("Approval Request")]
        Approval = 1,
        [Description("Deposit Request")]
        Deposit,
        [Description("Expense Request")]
        Expense,
        [Description("Entity")]
        Entity,
        [Description("Person")]
        Person,
        [Description("Reserved for Future Use")]
        ReservedForFutureUse
    }

    // Whether an Expense/Deposit has a Source of Funds and uses a Project Class. Careful here. Expenses use this as "Source of Funds" while
    // Deposits use it as "Destination of Funds." To finesse, we call the enum "SourceOfExpFunds." This lets Deposits have their own
    // "Source of Funds" whose Enum is "SourceOfDepFunds."

    public enum SourceOfDepFunds
    {
        [Description("Not Applicable")]
        NA = 1,
        [Description("Entity (EIN)")]
        Entity,
        [Description("Individual (SSN)")]
        Individual
    }
    public enum SourceOfExpFunds
    {
        [Description("Not Applicable")]
        NA = 1,
        [Description("Unrestricted (No Project Class")]
        Unrestricted,
        [Description("Restricted (One Project Class")]
        Restricted
    }

    // Files attached to a Expense or Deposit. This is just a description of the file. The actual file is stored in the
    // /Supporting directory.
    
    public class SupportingDoc
    {
        public int SupportingDocID { get; set; }
        public string FileName { get; set; }
        public string MIME { get; set; }
        public int FileLength { get; set; }
        [Required]
        public DateTime UploadedTime { get; set; }
        public int OwnerID { get; set; }
        public RequestType OwnerType { get; set; }
    }
    
    // Temporary copy of SupportingDoc. This copy lasts from the time the Attachment is uploaded until the Exp/Deposit 
    // (and its attachments) are saved.
    
    public class SupportingDocTemp
    {
        public int SupportingDocTempID { get; set; }
        public string FileName { get; set; }
        public string MIME { get; set; }
        public int FileLength { get; set; }
        [Required]
        public DateTime UploadedTime { get; set; }
        public string UserID { get; set; }
    }

    // Many-to-many connector for Users and Projects

    public class UserProject
    {
        public int UserProjectID { get; set; }
        public ProjectRole ProjectRole { get; set; }
        public int ProjectID { get; set; }
        public virtual Project Project { get; set; }
        public string UserID { get; set; }
        public virtual ApplicationUser User { get; set; }
    }

    // Roles that the User can assume. These values must be in sync with the Register and EditRegistration pages, which contain checkboxes
    // for each role. In those checkboxes, our Description must match the checkbox Text and our Value must match the checkbox Value.

    public enum UserRole
    {
        [Description("System Administrator")]
        Administrator = 1,
        [Description("Auditor")]
        Auditor,
        [Description("Internal Coordinator")]
        InternalCoordinator,
        [Description("Finance Director")]
        FinanceDirector,
        [Description("Project Member")]
        Project,
        //[Description("Project Director")] //TODO: Can we do without this role?
        //ProjectDirector,
        [Description("Trust Director")]
        TrustDirector,
        [Description("Trust Executive")]
        TrustExecutive,
        [Description("Undefined")]
        Undefined
    }

    // Simple yes/no question asked via radio buttons.

    public enum YesNo
    {
        [Description("No")]
        No,
        [Description("Yes")]
        Yes,
        [Description("None")]
        None
    }

    // The vendors that Projects want to pay

    public class Vendor
    {
        public int VendorID { get; set; }
        [Required]
        public string FranchiseKey { get; set; }
        public bool Inactive { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        [StringLength(250), DataType(DataType.MultilineText)]
        public string Address { get; set; }
        [StringLength(20)]
        public string Phone { get; set; }
        [StringLength(100)]
        public string Email { get; set; }
        public string URL { get; set; }
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }                   // Free-form comments
        public DateTime CreatedTime { get; set; }
    }
    public class PortalConstants
    {
        // Values of Query Strings
        public const string
            QSAspxErrorPath = "aspxerrorpath",
            QSCommand = "Command",
            QSCommandAssign = "Assign",
            QSCommandAssignEntitys = "AssignEntitys",
            QSCommandAssignGrant = "AssignGrant",
            QSCommandAssignPersons = "AssignPersons",
            QSCommandChangePassword = "ChangePassword",
            QSCommandCopy = "Copy",
            QSCommandEdit = "Edit",
            QSCommandFirst = "First",
            QSCommandMenu = "Menu",
            QSCommandNew = "New",
            QSCommandReview = "Review",
            QSCommandView = "View",
            QSCommandUserLogin = "UserLogin",
            QSDanger = "Danger",
            QSEmail = "Email",
            QSEntityID = "EntityID",
            QSEntityName = "EntityName",
            QSErrorText = "ErrorText",
            QSFullName = "FullName",
            QSGLCodeID = "GLCodeID",
            QSGrantID = "GrantID",
            QSPageName = "PageName",
            QSPersonID = "PersonID",
            QSProjectID = "ProjectID",
            QSProjectClassID = "ProjectClassID",
            QSProjectName = "ProjectName",
            QSRememberEmail = "RememberEmail",
            QSRequestID = "RequestID",
            QSReturn = "Return",
            QSSeverity = "Severity",
            QSStatus = "Status",
            QSSuccess = "Success",
            QSVendorID = "VendorID",
            QSVendorName = "VendorName",
            QSUserID = "UserID";

        // LoginCookie stores information from the login process, like their email address

        public const string
            CLoginInfo = "RememberInfoCookie",
            CLoginEmail = "Email",
            CLoginRememberEmail = "RememberEmail";

        // UserInfoCookie stores information about the logged in user. Like their role.

        public const string
            CUserInfo = "UserInfoCookie",
            CUserFranchiseKey = "FranchiseKey",
            CUserFullName = "UserFullName",
            CUserID = "UserID",
            CUserTypeAdmin = "Admin",
            CUserTypeInternalCoordinator = "Internal Coordinator",
            CUserTypeProject = "Project",
            CUserTypeStaff = "Staff",
            CUserRole = "Role",
            CUserRoleDescription = "RoleDescription",
            CUserProjectSelector = "ProjectSelector",
            CUserProjectAll = "All",
            CUserProjectUser = "User",
            CUserGridViewRows = "GridViewRows";
        

        // ProjectInfoCookie stores information about the project that the user has selected to work on.

        public const string
            CProjectInfo = "ProjectInfoCookie",
            CProjectID = "ProjectID",
            CProjectName = "ProjectName",
            CProjectRole = "ProjectRole",
            CProjectRoleDescription = "ProjectRoleDescription";

        // This cookie holds the settings of checkboxes on the Project Dashboard page so we can leave the page and return with the same settings

        public const string
            CProjectCheckboxes = "ProjectCheckboxesCookie",

            CProjectCkSearchVisible = "ProjectSearchVisible",
            CProjectCkRAwaitingProjectStaff = "CkRAwaitingProjectStaff",
            CProjectCkRAwaitingCWStaff = "CkRAwaitingCWStaff",
            CProjectCkRApproved = "CkRApproved",
            CProjectCkRReturned = "CkRReturned",
            CProjectCkRActive = "CkRActive",
            CProjectCkRArchived = "CkRArchived",
            CProjectFromDate = "CProjectFromDate",
            CProjectToDate = "CProjectToDate",

            CProjectDdlEntityID = "CProjectDdlEntityID",
            CProjectDdlPersonID = "CProjectDdlPersonID",

            CProjectAppVisible = "ProjectAppVisible",
            
            CProjectDepVisible = "ProjectDepVisible",
            CProjectCkDAwaitingProjectStaff = "CkDAwaitingProjectStaff",
            CProjectCkDAwaitingCWStaff = "CkDAwaitingCWStaff",
            CProjectCkDDepositComplete = "CkDDepositComplete",
            CProjectCkDReturned = "CkDReturned",
            
            CProjectExpVisible = "ProjectExpVisible",
            CProjectCkEAwaitingProjectStaff = "CkEAwaitingProjectStaff",
            CProjectCkEAwaitingCWStaff = "CkEAwaitingCWStaff",
            CProjectCkEPaid = "CkEPaid",
            CProjectCkEReturned = "CkEReturned";


        // This cookie holds the settings of checkboxes on the Staff Dashbord page so we can leave the page and return with the same settings

        public const string
            CStaffCheckboxes = "StaffCheckboxesCookie",

            CStaffCkSearchVisible = "StaffSearchVisible",

            CStaffCkRInternalCoordinator = "CkRInternalCoordinator",
            CStaffCkRFinanceDirector = "CkRFinanceDirector",
            CStaffCkRProjectMember = "CkRProjectMember",
            CStaffCkRTrustDirector = "CkRTrustDirector",
            CStaffCkRTrustExecutive = "CkRTrustExecutive",

            CStaffFromDate = "CStaffFromDate",
            CStaffToDate = "CStaffToDate",

            CStaffDdlEntityID = "CStaffDdlEntityID",
            CStaffDdlGLCodeID = "CStaffDdlGLCodeID",
            CStaffDdlPersonID = "CStaffDdlPersonID",
            CStaffDdlProjectID = "CStaffDdlProjectID",

            CStaffCkRActive = "CStaffCkRActive",
            CStaffCkRArchived = "CStaffCkRArchived",

            CStaffApprovalsVisible = "StaffApprovalsVisible",
            CStaffCkAExpress = "CkAExpress",
            CStaffCkAFull = "CkAFull",

            CStaffDepositsVisible = "StaffDepositsVisible",
            CStaffCkDCheck = "CkDCheck",
            CStaffCkDEFT = "CkDEFT",
            CStaffCkDCash = "CkDCash",
            CStaffCkDInKind = "CkDInKind",
            CStaffCkDPledge = "CkDPledge",
            CStaffCkDFinanceDirector = "CkDFinanceDirector",
            CStaffCkDProjectMember = "CkDProjectMember",
            CStaffCkDTrustDirector = "CkDTrustDirector",

            CStaffExpVisible = "StaffExpVisible",
            CStaffCkEContractorInvoice = "CkEContractorInvoice",
            CStaffCkEPEXCard = "CkEPEXCard",
            CStaffCkEPaycheck = "CkEPaycheck",
            CStaffCkEPurchaseOrder = "CkEPurchaseOrder",
            CStaffCkEReimbursement = "CkEReimbursement",
            CStaffCkEVendorInvoice = "CkEVendorInvoice",
            CStaffCkEFinanceDirector = "CkEFinanceDirector",
            CStaffCkEProjectMember = "CkEProjectMember",
            CStaffCkETrustDirector = "CkETrustDirector",
            CStaffCkETrustExecutive = "CkETrustExecutive"
            ;

        // Random constants

        public const string
            AssemblyName = "Portal11",
            DdlID = "ID",
            DdlName = "Name",
            DdlNeededSignal = "-1",
            EventArgument = "__EVENTARGUMENT",
            EventSupporting = "Supporting",
            ForeColor = "#18bc9c",
            ReadmeDir = "~/App_Readme/",
            SupportingDir = "~/Supporting/",
            SupportingTempFlag = "T",
            WhatsNewName = "WhatsNew.txt";
        public const int
            AllPersonViewW9Column = 3,
            MaxSupportingFileSize = 1048576,
            ProjectPersonViewW9Column = 3;

        // URLs of key pages. Expressed here so if their names change, it only has to be changed here

        public const string
            URLAdminMain = "~/Admin/Main",
            URLAssignEntitysToProject = "~/Admin/AssignEntitysToProject",
            URLAssignPersonsToProject = "~/Admin/AssignPersonsToProject",
            URLAssignUserToProject = "~/Admin/AssignUserToProject",
            URLChangeUserPassword = "~/Account/ChangeUserPassword",
            URLDefault = "~/Default",
            URLEditApproval = "~/Rqsts/EditApproval",
            URLEditDeposit = "~/Rqsts/EditDeposit",
            URLEditEntity = "~/Admin/EditEntity",
            URLEditExpense = "~/Rqsts/EditExpense",
            URLEditGLCode = "~/Admin/EditGLCode",
            URLEditPerson = "~/Admin/EditPerson",
            URLEditProject = "~/Admin/EditProject",
            URLEditProjectClass = "~/Proj/EditProjectClass",
            URLEditRegistration = "~/Account/EditRegistration",
            URLEditVendor = "~/Admin/EditVendor",
            URLErrorDatabase = "~/ErrorLog/DatabaseError.aspx",
            URLErrorFatal = "~/ErrorLog/FatalError.aspx",
            URLLogin = "~/Account/Login",
            URLLoginDispatch = "~/Account/LoginDispatch",
            URLProjectDashboard = "~/Proj/ProjectDashboard",
            URLReviewApproval = "~/Rqsts/ReviewApproval",
            URLReviewDeposit = "~/Rqsts/ReviewDeposit",
            URLReviewExpense = "~/Rqsts/ReviewExpense",
            URLSelectEntity = "~/Select/SelectEntity",
            URLSelectGLCode = "~/Select/SelectGLCode",
            URLSelectPerson = "~/Select/SelectPerson",
            URLSelectProject = "~/Select/SelectProject",
            URLSelectProjectClass = "~/Select/SelectProjectClass",
            URLSelectUser = "~/Select/SelectUser",
            URLSelectVendor = "~/Select/SelectVendor",
            URLStaffDashboard = "~/Staff/StaffDashboard";

        // Names of radio buttons in the Source of Funds panel used by EditDeposit and EditExpense

        public const string
            RDOFundsRestricted = "Restricted",
            RDOFundsUnrestricted = "Unrestricted",
            RDONotApplicable = "NA",
            RDOAnonymous = "Anonymous",
            RDOEntity = "Entity",
            RDOIndividual = "Individual";

        // Names of radio buttons in the PO VendorMode and DeliveryMode panels

        public const string
            CYes = "Yes",
            CNo = "No",
            CNone = "None",
            CSVFileName = "Uploaded CSV File.CSV", CSVExt = ".CSV", CSVType = "APPLICATION/VND.MS-EXCEL",
            POVendorModeYes = "Yes",
            POVendorModeNo = "No",
            PODeliveryModePickup = "Pickup",
            PODeliveryModeDeliverCW = "DeliverCW",
            PODeliveryModeDeliverAddress = "DeliverAddress",
            DeliveryModePickup = "Pickup",
            DeliveryModeMailPayee = "MailPayee",
            DeliveryModeMailAddress = "MailAddress";

    }
}