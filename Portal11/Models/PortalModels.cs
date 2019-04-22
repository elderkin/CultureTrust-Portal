using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace Portal11.Models
{

    // One row of the GridView named AssignProjectView, used by AssignUserToProject

    public class rowAssignUserToProject
    {
        public string RowID { get; set; }
        public ProjectRole ProjectRole { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CurrentPD { get; set; }
        public string UserRole { get; set; }
        public const int DescriptionLength = 40;
    }

    // One row of the GridView named gvEDHistory, used by EditDeposit, EditRequest, ReviewDeposit, and ReviewRequest

    public class rowEDHistory
    {
        public DateTime Date { get; set; }
        public string FormerStatus { get; set; }
        public string EstablishedBy { get; set; }
        public string UpdatedStatus { get; set; }
        public string ReasonForChange { get; set; }
        public string ReturnNote { get; set; }
        public string HyperLinkTarget { get; set; }
    }

    // One row of the GridView named ExpenseSplit, used by EditExpense, and DepositSplit, used by EditDeposit

    public class rowGLCodeSplit
    {
        public string Amount { get; set; }
        public string HourlyRate { get; set; }
        public decimal HoursPaid { get; set; }
        public string Note { get; set; }
        public string SelectedGLCodeID { get; set; }
        public string SelectedPersonID { get; set; }
        public string SelectedProjectClassID { get; set; }
        public int TotalRows { get; set; }
    }

    // One row of the GridView named ImportCSV, used by ImportProjectBalance

    public class rowImportCSV
    {
        public string ProjectCode { get; set; }
        public string BalanceDate { get; set; }
        public string CurrentFunds { get; set; }
        public string Status { get; set; }
        public bool Error { get; set; }
    }

    // One row of the GridView named gvEntitiess, used by ListEntities

    public class rowListEntities
    {
        public string ProjectDepositCodes { get; set; }
        public string ProjectExpenseCodes { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string URL { get; set; }
        public string Phone { get; set; }
        public string Inactive { get; set; }
        public const int InactiveColumn = 7;
        public string EntityID { get; set; }
    }

    // One row of the GridView named gvPersons, used by ListPersons

    public class rowListPersons
    {
        public string ProjectContractorCodes { get; set; }
        public string ProjectDonorCodes { get; set; }
        public string ProjectEmployeeCodes { get; set; }
        public string ProjectResponsiblePersonCodes { get; set; }
        public string ProjectRecipientCodes { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Inactive { get; set; }
        public const int InactiveColumn = 9;
        public string PersonID { get; set; }
    }

    // One row of the GridView named gvListProjectClasses, used by ListProjectMetadata

    public class rowListProjectClass
    {
        public int ProjectClassID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTime { get; set; }
        public bool CreatedFromMaster { get; set; }
        public bool Default { get; set; }
    }

    // One row of the GridView named gvListProjectEntitys, used by ListProjectMetadata

    public class rowListProjectEntity
    {
        public int ProjectEntityID { get; set; }
        public int EntityID { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime CreatedTime { get; set; }
        public string EntityRole { get; set; }
    }

    // One row of the GridView named gvListProjectMembers, used by EditProjectUser

    public class rowListProjectMember
    {
        public string UserRole { get; set; }
        public string ProjectName { get; set; }
    }

    // One row of the GridView named gvListProjectPersons, used by ListProjectMetadata

    public class rowListProjectPerson
    {
        public int ProjectPersonID { get; set; }
        public int PersonID { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime CreatedTime { get; set; }
        public string PersonRole { get; set; }
    }

    // One row of the GridView named gvListProjectStaff, used by EditProject

    public class rowListProjectStaff
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    // One row of a list of persons with IDs. Very simple

    public class rowPerson
    {
        public int PersonID { get; set; }
        public string Name { get; set; }
    }

    // One row of the GridView named gvListProjectUsers, used by ListProjectMetadata

    public class rowListProjectUser
    {
        public int ProjectUserID { get; set; }
        public string UserID { get; set; }
        public string Name { get; set; }
        public string ProjectRole { get; set; }
    }

    // One row of the GridView named PortalUsers, used by ListPortalUsers

    public class rowPortalUsers
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

    // One row of the GridView named gvProjects, used by ListProjects

    public class rowProjects
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
        public string ProjectID { get; set; }
        public int YearExpenses { get; set; }
        public int YearDeposits { get; set; }
        public int YearDocuments { get; set; }
        public decimal TotalAmounts { get; set; }
    }

    // One row of the GridView named gvAllAppView, used by ProjectDashboard. Now obsolete. But also ListMetadata

    public class rowProjectAppView
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

    // One row of the GridView named gvAllDepView, used by ProjectDashboard

    public class rowProjectDepView
    {
        public string RowID { get; set; }
        public DateTime Time { get; set; }
        public const int TimeRow = 1;
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
        public UserRole ReviseUserRole { get; set; }        // User Role of user who revised the Exp
    }

    // One row of the GridView named gvAllDocView, used by ProjectDashboard

    public class rowProjectDocView
    {
        public string RowID { get; set; }
        public DateTime Time { get; set; }
        public const int TimeRow = 1;
        public string DocTypeDesc { get; set; }
        public string Description { get; set; }
        public DocState CurrentState { get; set; }
        public const int CurrentStateCell = 4;
        public string CurrentStateDesc { get; set; }
        public const int CurrentStateDescRow = 5;
        public string ReturnNote { get; set; }
        public bool Archived { get; set; }
        public bool Rush { get; set; }
        public UserRole ReviseUserRole { get; set; }        // User Role of user who revised the Exp
    }

    // One row of the GridView named gvAllExpView, used by ProjectDashboard

    public class rowProjectExpView
    {
        public string RowID { get; set; }
        public DateTime Time { get; set; }
        public const int TimeRow = 1;
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
        public UserRole ReviseUserRole { get; set; }        // User Role of user who revised the Exp
    }

    // One row of the GridViews named gvAllProject, used by SelectProject

    public class rowSelectProjectAllView
    {
        public string ProjectID { get; set; }
        public string Inactive { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
        public const int InactiveColumn = 5;
    }

    public class rowSelectProjectUserView
    {
        public string ProjectID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProjectRole { get; set; }
        public int Count { get; set; }
    }

    // One row of the GridView named gvStaffApp, used by StaffDashboard

    //public class rowStaffApp
    //{
    //    public string RowID { get; set; }
    //    public DateTime CurrentTime { get; set; }
    //    public string ProjectID { get; set; }
    //    public string ProjectName { get; set; }
    //    public string AppTypeDesc { get; set; }
    //    public string AppReviewType { get; set; }
    //    public AppState CurrentState { get; set; }
    //    public string CurrentStateDesc { get; set; }
    //    public string Owner { get; set; }
    //    public const int OwnerColumn = 5;
    //    public string Description { get; set; }
    //    public string ReturnNote { get; set; }
    //    public bool Archived { get; set; }
    //}

    public class rowStaffDep
    {
        public string RequestID { get; set; }
        public DateTime Time { get; set; }
        public string ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string DepTypeDesc { get; set; }
        public decimal Amount { get; set; }
        public DepState CurrentState { get; set; }
        public string CurrentStateDesc { get; set; }
        public string Owner { get; set; }
        public const int OwnerColumn = 5;
        public string Description { get; set; }
        public string ReturnNote { get; set; }
        public bool Archived { get; set; }
        public UserRole ReviseUserRole { get; set; }        // User Role of user who revised the request
    }

    // One row of GridView named gvStaffDoc, used by StaffDashboard

    public class rowStaffDoc
    {
        public string RequestID { get; set; }
        public DateTime Time { get; set; }
        public string ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string DocTypeDesc { get; set; }
        public DocState CurrentState { get; set; }
        public string CurrentStateDesc { get; set; }
        public string Owner { get; set; }
        public const int OwnerColumn = 4;
        public string Description { get; set; }
        public string ReturnNote { get; set; }
        public bool Archived { get; set; }
        public bool Rush { get; set; }
        public UserRole ReviseUserRole { get; set; }        // User Role of user who revised the request
    }

    // One row of the GridView named gvStaffExp, used by StaffDashboard

    public class rowStaffExp
    {
        public string RequestID { get; set; }
        public DateTime Time { get; set; }
        public string ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string ExpTypeDesc { get; set; }
        public decimal Amount { get; set; }
        public ExpState CurrentState { get; set; }
        public string CurrentStateDesc { get; set; }
        public const int StateColumn = 4;
        public string Owner { get; set; }
        public const int OwnerColumn = 5;
        public string Target { get; set; }
//        public string Summary { get; set; }
        public string Description { get; set; }
        public string ReturnNote { get; set; }
        public bool Archived { get; set; }
        public bool Rush { get; set; }
        public UserRole ReviseUserRole { get; set; }        // User Role of user who revised the Exp
    }

    // One row of the GridView named UserProjectView, used by AssignUserToProject

    //public class rowUserProjectView
    //{
    //    public int ProjectID { get; set; }
    //    public ProjectRole ProjectRole { get; set; }
    //    public string ProjectName { get; set; }
    //    public string ProjectDescription { get; set; }
    //}

    // The Approval Request - made by a CW Staff on a Project.

    public class App
    {
        public int AppID { get; set; }
        public bool Inactive { get; set; }
        public bool Archived { get; set; }
        public AppType AppType { get; set; }                // Will grow to include many types
        public const int AppTypeICOnly = 0, AppTypeExpress = 1, AppTypeFull = 2; // Order in which they appear in radio button list
                                                            // Note this is different than the AppReviewType enumeration
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

        [DataType(DataType.MultilineText)]
        public string StaffNote { get; set; }

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
        [Description("Awaiting Community Director")]
        AwaitingCommunityDirector,
        [Description("Awaiting Finance Director")]
        AwaitingFinanceDirector,
        [Description("Awaiting President")]
        AwaitingPresident,
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
        [Description("IC Only - Review only by Internal Coordinator")]
        ICOnly,
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

        [DataType(DataType.MultilineText)]
        public string StaffNote { get; set; }

        // The CURRENT state of the Deposit - where it is in the flow right now and who put it in this state
        [Required]
        public DepState CurrentState { get; set; }
        public DateTime CurrentTime { get; set; }
        // The User who took the most recent action on the Deposit
        public string CurrentUserID { get; set; }
        public virtual ApplicationUser CurrentUser { get; set; }
        public string SubmitUserID { get; set; }            // The User who originally submitted the Deposit. They get notified if it's returned
        public virtual ApplicationUser SubmitUser { get; set; }
        public UserRole ReviseUserRole { get; set; }        // User Role of user who revised the Exp. This helps us turn Dashboard rows Teal
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
        [Description("Awaiting Community Director")]
        AwaitingCommunityDirector,
        [Description("Awaiting Finance Director")]
        AwaitingFinanceDirector,
        [Description("Obsolete")]
        ApprovedReadyToDeposit,
        [Description("Approved/Deposit Complete")]
        DepositComplete,
        [Description("Returned (IC)")]
        Returned,
        [Description("Error During Processing")]
        Error,
        [Description("Reserved For Future Use")]
        ReservedForFutureUse,
        [Description("Revising (PD)")]
        RevisingByProjectDirector,
        [Description("Revising (FD)")]
        RevisingByFinanceDirector,
        [Description("Revised by FD")]
        RevisedByFinanceDirector
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

    // The Document Request - made by a CW Staff on a Project.

    public class Doc
    {
        public int DocID { get; set; }
        public bool Inactive { get; set; }
        public bool Archived { get; set; }
        public DocType DocType { get; set; }                // Will grow to include many types
        [Required]
        public int? ProjectID { get; set; }
        public virtual Project Project { get; set; }

        [Required]
        public DateTime CreatedTime { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }                   // Free-form comments on the Request

        [DataType(DataType.MultilineText)]
        public string ReturnNote { get; set; }

        [DataType(DataType.MultilineText)]
        public string StaffNote { get; set; }

        // The CURRENT state of the Request - where it is in the flow right now and who put it in this state
        [Required]
        public DocState CurrentState { get; set; }
        public DateTime CurrentTime { get; set; }
        // The User who took the most recent action on the Document
        public string CurrentUserID { get; set; }
        public virtual ApplicationUser CurrentUser { get; set; }
        // The User who originally submitted the Deposit. They get notified if it's returned
        public string SubmitUserID { get; set; }
        public virtual ApplicationUser SubmitUser { get; set; }
        public ProjectRole SubmitProjectRole { get; set; }      // Project Role of user who submitted the Doc. Revisions are returned to them.
        public UserRole ReviseUserRole { get; set; }            // User Role of user who revised the Exp. This helps us turn Dashboard rows Teal

        // Links to other records

        public int? EntityID { get; set; }
        public virtual Entity Entity { get; set; }
        public int? GLCodeID { get; set; }
        public virtual GLCode GLCode { get; set; }
        public int? PersonID { get; set; }
        public virtual Person Person { get; set; }
        public int? ProjectClassID { get; set; }
        public virtual ProjectClass ProjectClass { get; set; }

        // Other fields common to all Document types

        public bool Rush { get; set; }                          // Whether the Request has "Rush" status

        // Specifics for Document Type - Contract

        public DateTime ContractBeginningDate { get; set; }         // Duration of contract
        public DateTime ContractEndingDate { get; set; }
        public YesNo ContractCOIRequired { get; set; }
        [DataType(DataType.MultilineText)]
//        public string ContractDetailsOnDeliverables { get; set; }
        public YesNo ContractDocumentsSigned { get; set; }
        [DataType(DataType.MultilineText)]
        public string ContractDocumentsSignedReason { get; set; }
//        public string ContractDuration { get; set; }
        public DocContractOtherParty ContractOtherParty { get; set; }
//        public int? ContractEntityID { get; set; }
        public DocContractFunds ContractFunds { get; set; }
        public decimal ContractFundsAmount { get; set; }
        public YesNo ContractFundsMultiple { get; set; }
        public int ContractFundsNumber { get; set; }
//       public int? ContractPersonID { get; set; }
//        public bool ContractSchedule { get; set; }
        [DataType(DataType.MultilineText)]
        public string ContractScheduleDetails { get; set; }
        public bool ContractVerifyProjectName { get; set; }
        public bool ContractVerifyCTPresident { get; set; }
        [DataType(DataType.MultilineText)]
        public string ContractVerifyReason { get; set; }

        // Specifics for Document Type - Campaign

        public bool CampaignVerifyMention { get; set; }
        [DataType(DataType.MultilineText)]
        public string CampaignVerifyReason { get; set; }

        // Specifics for Document Type - Certificate

        public string CertificateEventDate { get; set; }
        public string CertificateEventTime { get; set; }
        public string CertificateName { get; set; }
        [DataType(DataType.MultilineText)]
        public string CertificateAddress { get; set; }

        // Specifics for Document Type - Financial

        public DateTime FinancialBeginningDate { get; set; }
        public DateTime FinancialEndingDate { get; set; }
        // Use common GLCode and ProjectClass

        // Specifics for Document Type - Grant

        public bool GrantVerifyNarrative { get; set; }
        public bool GrantVerifyBudget { get; set; }
        [DataType(DataType.MultilineText)]
        public string GrantVerifyReason { get; set; }

    }
    public enum DocContractFunds
    {
        ContractReceivingFunds = 1,
        ContractPayingFunds,
        None
    }

// The states that the Document has gone through. Who did what when why to the Request

public class DocHistory
    {
        public int DocHistoryID { get; set; }
        public int DocID { get; set; }
        public DocState PriorDocState { get; set; }
        public DocState NewDocState { get; set; }
        public DateTime HistoryTime { get; set; }
        public string HistoryUserID { get; set; }
        public virtual ApplicationUser HistoryUser { get; set; }
        public string HistoryNote { get; set; }
        public string ReturnNote { get; set; }
        public int? SupportingDocID { get; set; }
        public string MIME { get; set; }
        public string FileName { get; set; }
    }

    public enum DocContractOtherParty
    {
        Entity = 1,
        Person,
        None
    }

    // The processing states that a Approval can go through

    public enum DocState
    {
        [Description("Unsubmitted (IC)")]
        UnsubmittedByInternalCoordinator = 1,
        [Description("Unsubmitted (PS)")]
        UnsubmittedByProjectStaff,
        [Description("Unsubmitted (PD)")]
        UnsubmittedByProjectDirector,
        [Description("Awaiting Project Director")]
        AwaitingProjectDirector,
        [Description("Awaiting Internal Coordinator")]
        AwaitingInternalCoordinator,
        [Description("Awaiting Finance Director")]
        AwaitingFinanceDirector,
        [Description("Awaiting Community Director")]
        AwaitingCommunityDirector,
        [Description("Awaiting President")]
        AwaitingPresident,
        [Description("Executed")]
        Executed,
        [Description("Returned (PD)")]
        ReturnedToProjectDirector,
        [Description("Returned (PS)")]
        ReturnedToProjectStaff,
        [Description("Returned (IC)")]
        ReturnedToInternalCoordinator,
        [Description("Revising (PD)")]
        RevisingByProjectDirector,
        [Description("Revising (IC)")]
        RevisingByInternalCoordinator,
        [Description("Revising (FD)")]
        RevisingByFinanceDirector,
        [Description("Revising (CD)")]
        RevisingByCommunityDirector,
        [Description("Revising (PR)")]
        RevisingByPresident,
        [Description("Revised By IC")]
        RevisedByInternalCoordinator,
        [Description("Revised By FD")]
        RevisedByFinanceDirector,
        [Description("Revised By CD")]
        RevisedByCommunityDirector,
        [Description("Revised By PR")]
        RevisedByPresident,
        [Description("Error During Processing")]
        Error,
        [Description("Reserved For Future Use")]
        ReservedForFutureUse,
    }

    // Each type of Document fills different fields in the Doc object. This mapping is complicated and expressed in code.

    public enum DocType
    {
        [Description("Contract")]
        Contract = 1,
        [Description("Grant Proposal")]
        Grant,
        [Description("Certificate of Insurance")]
        Certificate,
        [Description("Financial Report")]
        Financial,
        [Description("Marketing/Fundraising Campaign")]
        Campaign,
        [Description("Reserved for Future Use")]
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

        public decimal Amount { get; set; }                 // Total funds in this Request. For Payroll, this is Gross Pay

        public DateTime BeginningDate { get; set; }         // Pay Period for Payroll
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

        public POVendorMode POVendorMode { get; set; }     // PO fulfillment Instructions

        [Required]
        public int? ProjectID { get; set; }                 // The Project that owns this Request
        public virtual Project Project { get; set; }

        [DataType(DataType.MultilineText)]
        public string ReturnNote { get; set; }              // When approval is denied, the reason goes here

        public bool Rush { get; set; }                      // Whether the Request has "Rush" status

        public SourceOfExpFunds SourceOfFunds { get; set; } // Where the Request gets its Funds OBSOLETE
        public int? ProjectClassID { get; set; }
        public virtual ProjectClass ProjectClass { get; set; }

        [DataType(DataType.MultilineText)]
        public string StaffNote { get; set; }

//        [StringLength(30)]
//        public string Summary { get; set; }                 // A free text name for the Expense

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
        public ProjectRole SubmitProjectRole { get; set; }  // Project Role of user who submitted the Exp. Revisions are returned to them. 
        public UserRole ReviseUserRole { get; set; }        // User Role of user who revised the Exp. This helps us turn Dashboard rows Teal
    }

    // The states that the Expense has gone through. Who did what when why to the Request

    public class ExpHistory
    {
        public int ExpHistoryID { get; set; }
        public int ExpID { get; set; }
        public virtual Exp Exp { get; set; }
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
        [Description("Awaiting Community Director")]
        AwaitingCommunityDirector,
        [Description("Awaiting Finance Director")]
        AwaitingFinanceDirector,
        [Description("Awaiting President")]
        AwaitingPresident,
        [Description("Approved/Ready to Pay")]
        Approved,
        [Description("Payment Sent")]
        PaymentSent,                                    // Obsolete
        [Description("Paid and Sent")]
        Paid,
        [Description("Returned (PD)")]
        ReturnedToProjectDirector,
        [Description("Error During Processing")]
        Error,
        [Description("Reserved For Future Use")]
        ReservedForFutureUse,
        [Description("Awaiting Internal Coordinator")]
        AwaitingInternalCoordinator,
        [Description("Returned (PS)")]
        ReturnedToProjectStaff,
        [Description("Returned (IC)")]
        ReturnedToInternalCoordinator,
        [Description("Revising (PD)")]
        RevisingByProjectDirector,
        [Description("Revising (IC)")]
        RevisingByInternalCoordinator,
        [Description("Revising (FD)")]
        RevisingByFinanceDirector,
        [Description("Revising (CD)")]
        RevisingByCommunityDirector,
        [Description("Revising (PR)")]
        RevisingByPresident,
        [Description("Revised By IC")]
        RevisedByInternalCoordinator,
        [Description("Revised By FD")]
        RevisedByFinanceDirector,
        [Description("Revised By CD")]
        RevisedByCommunityDirector,
        [Description("Revised By PR")]
        RevisedByPresident,
        [Description("Revising (IC) Late")]
        RevisingByFinanceDirectorLate
    }

    // Each type of Expense fills different fields in the Expense object. This mapping is complicated and expressed in code.
    public enum ExpType
    {
        [Description("Contractor Invoice")]
        ContractorInvoice = 1,
        [Description("PEX Card")]
        PEXCard,
        [Description("Payroll")]
        Payroll,
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
        public const string LocalFranchiseEntity = "CultureTrust Greater Philadelphia";
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

    // The split GL Code rows for a given Request. This has evolved into several flavors of splits. For example, employees can split in Payroll.
    // So the name of the class is stale, but we'll stick with it.

    public class GLCodeSplit
    {
        public int GLCodeSplitID { get; set; }
        public RequestType RqstType { get; set; }
        public int RqstID { get; set; }
        public int? GLCodeID { get; set; }
        public virtual GLCode GLCode { get; set; }
        public int? ProjectClassID { get; set; }
        public virtual ProjectClass ProjectClass { get; set; }
        public decimal Amount { get; set; }
        public string Note { get; set; }

        // Specifics for payroll splits. I decided to fold these into the GLSplit logic rather than creating a new split just for it

        public int? PersonID { get; set; }
        public virtual Person Person { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal HoursPaid { get; set; }
//        public decimal GrossPay { get; set; }                         // Use Amount instead
    }

    // A Grant from a Grant Maker to a Project. Obsolete.

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

    // Block of parameters derived by parsing a command-line

    public class ParseParam
    {
        public YesNo COI { get; set; }
        public DocState DocState { get; set; }
        public DocType DocType { get; set; }
        public DocContractFunds DocContractFunds { get; set; }
        public string Recipients { get; set; }
        public RequestType RequestType { get; set; }
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

    public enum PODeliveryMode
    {
        [Description("Have the vendor hold this item. I will pick it up.")]
        Pickup,
        [Description("Have item delivered to CultureWorks.")]
        DeliverCW,
        [Description("Have item delivered to below address.")]
        DeliverAddress
    }

    public enum POVendorMode
    {
        [Description("Please use SKU/Model# and purchase from this vendor")]
        No,
        [Description("Any version of this item will do")]
        Yes,
        [Description("None")]
        None
    }

    // The Parameters that control the entire portal

    public class PortalParameter    {
        public int PortalParameterID { get; set; }
        [Required, StringLength(20), Index("Franchise", IsUnique = true)]
        public string FranchiseKey { get; set; }
        public bool EmailFDRefer { get; set; }
        public string EmailFDReferSubject { get; set; }
        [DataType(DataType.MultilineText)]
        public string EmailFDReferBody { get; set; }
        public bool EmailICRefer { get; set; }
        public string EmailICReferSubject { get; set; }
        [DataType(DataType.MultilineText)]
        public string EmailICReferBody { get; set; }
        public bool EmailPDRefer { get; set; }
        public string EmailPDReferSubject { get; set; }
        [DataType(DataType.MultilineText)]
        public string EmailPDReferBody { get; set; }
        public bool EmailCDRefer { get; set; }
        public string EmailCDReferSubject { get; set; }
        [DataType(DataType.MultilineText)]
        public string EmailCDReferBody { get; set; }
        public bool EmailPRRefer { get; set; }
        public string EmailPRReferSubject { get; set; }
        [DataType(DataType.MultilineText)]
        public string EmailPRReferBody { get; set; }

        public string EmailPDBroadcastRushSubject { get; set; }
        [DataType(DataType.MultilineText)]
        public string EmailPDBroadcastRushBody { get; set; }
        public string EmailStaffBroadcastRushSubject { get; set; }
        [DataType(DataType.MultilineText)]
        public string EmailStaffBroadcastRushBody { get; set; }
        public string EmailFDRushSubject { get; set; }
        [DataType(DataType.MultilineText)]
        public string EmailFDRushBody { get; set; }
        public bool EmailICRush { get; set; }
        public string EmailICRushSubject { get; set; }
        [DataType(DataType.MultilineText)]
        public string EmailICRushBody { get; set; }
        public string EmailPDRushSubject { get; set; }
        [DataType(DataType.MultilineText)]
        public string EmailPDRushBody { get; set; }
        public string EmailCDRushSubject { get; set; }
        [DataType(DataType.MultilineText)]
        public string EmailCDRushBody { get; set; }
        public string EmailPRRushSubject { get; set; }
        [DataType(DataType.MultilineText)]
        public string EmailPRRushBody { get; set; }

        public bool EmailDebug { get; set; }
        public string EmailDebugAddress { get; set; }

        public int SupportingDocCurrentIndex { get; set; }
        public string SupportingDocCurrentSubdirectory { get; set; }

        public bool DisableLogins { get; set; }                         // Only for non-Admin accounts

        [DataType(DataType.MultilineText)]
        public string EmailControlString { get; set; }                  // Parameterized string to control some emails

        public string EmailForgotPasswordSubject { get; set; }
        [DataType(DataType.MultilineText)]
        public string EmailForgotPasswordBody { get; set; }
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
        public virtual Project Project { get; set; }
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
        public virtual Project Project { get; set; }
        public int PersonID { get; set; }
        public virtual Person Person { get; set; }
        public PersonRole PersonRole { get; set; }
    }

    // Roles that Portal Users can have on Projects. Staff members have no "permanent" roles on projects.
    // But for the purpose of revising a request, they can have a "temporary" role, which lasts just for
    // the duration of the editing session

    public enum ProjectRole
    {
        [Description("Internal Coordinator")]
        InternalCoordinator = 1,
        [Description("Project Director")]
        ProjectDirector,
        [Description("Project Staff")]
        ProjectStaff,
        [Description("No Role")]
        NoRole,
        [Description("Revising Community Director")] 
        RevisingCommunityDirector,
        [Description("Revising Finance Director")]
        RevisingFinanceDirector,
        [Description("Revising President")]
        RevisingPresident
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
        public int PriorState { get; set; }                             // Could be DocState, DepState, ExpState
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
        [Description("Document Request")]
        Document,
        [Description("Reserved for Future Use")]
        ReservedForFutureUse
    }

    // The actions a reviewer can take while reviewing a request

    public enum ReviewAction
    {
        [Description("Approve Request")]
        Approve = 1,
        [Description("Return Request")]
        Return,
        [Description("Revise Request")]
        Revise,
        [Description("Submit Request")]
        Submit
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
        [Description("Unrestricted (No Project Class)")]
        Unrestricted,
        [Description("Restricted (One Project Class)")]
        Restricted
    }

    // Files attached to a Expense or Deposit. This is just a description of the file. The actual file is stored in the
    // /Supporting directory.

    public class SupportingDoc
    {
        public int SupportingDocID { get; set; }
        public string FileName { get; set; }
        public int? Index { get; set; }                         // Currently unused
        public string Subdirectory { get; set; }                // Currently unused
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
        [Description("Community Director")]
        CommunityDirector,
        [Description("President")]
        President,
        [Description("Undefined")]
        Undefined,
        [Description("None")]
        None = 0
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

    // The vendors that Projects want to pay. Obsolete - replaced by Entity.

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
            QSCode = "Code",
            QSCommand = "Command",
            QSCommandAssign = "Assign",
            QSCommandAssignEntitys = "AssignEntitys",
            QSCommandAssignGrant = "AssignGrant",
            QSCommandAssignPersons = "AssignPersons",
            QSCommandChangePassword = "ChangePassword",
            QSCommandCopy = "Copy",
            QSCommandEdit = "Edit",
            QSCommandFirst = "First",
            QSCommandList = "List",
            QSCommandMenu = "Menu",
            QSCommandNew = "New",
            QSCommandReview = "Review",
            QSCommandRevise = "Revise",
            QSCommandView = "View",
            QSCommandUserLogin = "UserLogin",
            QSDanger = "Danger",
            QSDirectory = "Directory",
            QSEmail = "Email",
            QSEntityID = "EntityID",
            QSEntityRole = "EntityRole",
            QSEntityName = "EntityName",
            QSErrorText = "ErrorText",
            QSFullName = "FullName",
            QSGLCodeID = "GLCodeID",
            QSGrantID = "GrantID",
            QSMethod = "Method",
            QSMIME = "MIME",
            QSNull = "Null",
            QSPageName = "PageName",
            QSPersonID = "PersonID",
            QSPersonRole = "PersonRole",
            QSProjectID = "ProjectID",
            QSProjectClassID = "ProjectClassID",
            QSProjectRole = "ProjectRole",
            QSProjectName = "ProjectName",
            QSRememberEmail = "RememberEmail",
            QSRequestID = "RequestID",
            QSReturn = "Return",
            QSReturn2="Return2",
            QSServerFile = "ServerFile",
            QSSeverity = "Severity",
            QSStatus = "Status",
            QSSuccess = "Success",
            QSVendorID = "VendorID",
            QSVendorName = "VendorName",
            QSUserFile = "UserFile",
            QSUserID = "UserID",
            QSUserRole = "UserRole",
            QSUTCOffset = "UTCOffset";

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
            CUserGridViewRows = "GridViewRows",
            CUserUTCOffset = "CUserUTCOffset";                          // Current time zone: Minutes later than UTC

        // AvatarImageCookie stores a sequence number for the logged in user. When the avatar changes, the sequence number changes, forcing browser to refresh image.

        public const string
            CAvatarImage = "AvatarImageCookie",
            CAvatarImageTag = "AvatarImageTag";
        public const int
            CAvatarNextTagRangeStart = 2, CAvatarNextTagRangeEnd = 1000000;

        // Default email subjects and bodies

        public const string
            CEmailDefaultApprovalApprovedSubject = "Approval Request is ready for your action",
            CEmailDefaultApprovalApprovedBody = "An Approval Request has advanced in the review process. It is ready for your action.",
            CEmailDefaultApprovalReturnedSubject = "Approval Request returned to you",
            CEmailDefaultApprovalReturnedBody = "An Approval Request has been returned to you during the review process. It is ready for your action.";

        public const string
            CEmailDefaultDepositApprovedSubject = "Deposit Request is ready for your action",
            CEmailDefaultDepositApprovedBody = "A Deposit Request has advanced in the review process. It is ready for your action.",
            CEmailDefaultDepositReturnedSubject = "Deposit Request returned to you",
            CEmailDefaultDepositReturnedBody = "A Deposit Request has been returned to you during the review process. It is ready for your action.";

        public const string
            CEmailDefaultDocumentApprovedSubject = "Document Request is ready for your action",
            CEmailDefaultDocumentApprovedBody = "An Document Request has advanced in the review process. It is ready for your action.",
            CEmailDefaultDocumentBroadcastSubject = "A rush Document Request has been submitted for project {0}",
            CEmailDefaultDocumentBroadcastBody = "A rush Document Request has been submitted. Expect an email asking for your review.",
            CEmailDefaultDocumentExecutedSubject = "Document Request executed",
            CEmailDefaultDocumentExecutedBody = "An Document Request has been executed at the conclusion of the review process. It is ready for your action.",
            CEmailDefaultDocumentReturnedSubject = "Document Request returned to you",
            CEmailDefaultDocumentReturnedBody = "An Document Request has been returned to you during the review process. It is ready for your action.";

        public const string
            CEmailDefaultExpenseApprovedSubject = "Expense Request is ready for your action",
            CEmailDefaultExpenseApprovedBody = "An Expense Request has advanced in the review process. It is ready for your action.",
            CEmailDefaultExpenseBroadcastSubject = "A rush Expense Request has been submitted for project {0}",
            CEmailDefaultExpenseBroadcastBody = "A rush Expense Request has been submitted. Expect an email asking for your review.",
            CEmailDefaultExpenseReturnedSubject = "Expense Request returned to you",
            CEmailDefaultExpenseReturnedBody = "An Expense Request has been returned to you during the review process. It is ready for your action.";

        // FlowControlCookie stores information to let us "return" to an earlier page with context.

        public const string
            CFlowControl = "FlowControlCookie",
            CReturnURL = "ReturnURL";

        // ProjectInfoCookie stores information about the project that the user has selected to work on.

        public const string
            CProjectInfo = "ProjectInfoCookie",
            CProjectID = "ProjectID",
            CProjectName = "ProjectName",
            CProjectCode = "ProjectCode",
            CProjectRole = "ProjectRole",
            CProjectRoleDescription = "ProjectRoleDescription";

        // This cookie holds the settings of checkboxes on the Project Dashboard page so we can leave the page and return with the same settings

        public const string
            CProjectCheckboxes = "ProjectCheckboxesCookie",

            CProjectCkSearchVisible = "ProjectSearchVisible",
            CProjectCkRUnsubmitted = "CkRUnsubmitted",
            CProjectCkRAwaitingCWStaff = "CkRAwaitingCWStaff",
            CProjectCkRApproved = "CkRApproved",
            CProjectCkRReturned = "CkRReturned",
            CProjectCkRActive = "CkRActive",
            CProjectCkRArchived = "CkRArchived",
            CProjectFromDate = "CProjectFromDate",
            CProjectToDate = "CProjectToDate",

            CProjectDdlEntityID = "CProjectDdlEntityID",
            CProjectDdlGLCodeID = "CProjectDdlGLCodeID",
            CProjectDdlPersonID = "CProjectDdlPersonID",
            CProjectDdlProjectClassID = "CProjectDdlProjectClassID",

            CProjectAppVisible = "ProjectAppVisible",
            
            CProjectDepVisible = "ProjectDepVisible",
            CProjectCkDAwaitingProjectStaff = "CkDAwaitingProjectStaff",
            CProjectCkDAwaitingCWStaff = "CkDAwaitingCWStaff",
            CProjectCkDDepositComplete = "CkDDepositComplete",
            CProjectCkDReturned = "CkDReturned",

            CProjectDocVisible = "ProjectDocVisible",
            CProjectCkOAwaitingProjectStaff = "CkOAwaitingProjectStaff",
            CProjectCkOAwaitingCWStaff = "CkOAwaitingCWStaff",
            CProjectCkODepositComplete = "CkODepositComplete",
            CProjectCkOReturned = "CkOReturned",

            CProjectExpVisible = "ProjectExpVisible",
            CProjectCkEAwaitingProjectStaff = "CkEAwaitingProjectStaff",
            CProjectCkEAwaitingCWStaff = "CkEAwaitingCWStaff",
            CProjectCkEPaid = "CkEPaid",
            CProjectCkEReturned = "CkEReturned";

        // This cookie holds the settings of checkboxes on the Staff Dashbord page so we can leave the page and return with the same settings

        public const string
            CStaffCheckboxes = "StaffCheckboxesCookie",

            CStaffCkSearchVisible = "StaffSearchVisible",

            CStaffCkRCompleted = "CkRCompleted",
            CStaffCkRFinanceDirector = "CkRFinanceDirector",
            CStaffCkRInternalCoordinator = "CkRInternalCoordinator",
            CStaffCkRProjectMember = "CkRProjectMember",
            CStaffCkRReturned = "CkRReturned",
            CStaffCkRCommunityDirector = "CkRCommunityDirector",
            CStaffCkRPresident = "CkRPresident",
            CStaffCkRUnsubmitted = "CkRUnsubmitted",

            CStaffFromDate = "CStaffFromDate",
            CStaffToDate = "CStaffToDate",

            CStaffDdlEntityID = "CStaffDdlEntityID",
            CStaffDdlGLCodeID = "CStaffDdlGLCodeID",
            CStaffDdlPersonID = "CStaffDdlPersonID",
            CStaffDdlProjectID = "CStaffDdlProjectID",

            CStaffCkRActive = "CStaffCkRActive",
            CStaffCkRArchived = "CStaffCkRArchived",

            //CStaffApprovalsVisible = "StaffApprovalsVisible",
            //CStaffCkAExpress = "CkAExpress",
            //CStaffCkAFull = "CkAFull",

            CStaffDepositsVisible = "StaffDepositsVisible",
            CStaffCkDCheck = "CkDCheck",
            CStaffCkDEFT = "CkDEFT",
            CStaffCkDCash = "CkDCash",
            CStaffCkDInKind = "CkDInKind",
            CStaffCkDPledge = "CkDPledge",
            CStaffCkDFinanceDirector = "CkDFinanceDirector",
            CStaffCkDProjectMember = "CkDProjectMember",
            CStaffCkDCommunityDirector = "CkDCommunityDirector",

            CStaffDocumentsVisible = "StaffDocumentsVisible",
            CStaffCkCContract = "CkCContract",
            CStaffCkCGrant = "CkCGrant",
            CStaffCkCCertificate = "CkCertificate",
            CStaffCkCFinancial = "CkCFinancial",
            CStaffCkCCampaign = "CkCCampaign",

            CStaffExpVisible = "StaffExpVisible",
            CStaffCkEContractorInvoice = "CkEContractorInvoice",
            CStaffCkEPEXCard = "CkEPEXCard",
            CStaffCkEPayroll = "CkEPayroll",
            CStaffCkEPurchaseOrder = "CkEPurchaseOrder",
            CStaffCkEReimbursement = "CkEReimbursement",
            CStaffCkEVendorInvoice = "CkEVendorInvoice",
            CStaffCkEFinanceDirector = "CkEFinanceDirector",
            CStaffCkEProjectMember = "CkEProjectMember",
            CStaffCkECommunityDirector = "CkECommunityDirector",
            CStaffCkEPresident = "CkEPresident";

        // This cookie holds the settings of page index values within the gridviews on the Staff Dashbord page

        public const string
            CStaffPageIndexes = "StaffPageIndexes",
            CStaffPIApp = "StaffPIApp",
            CStaffPIDep = "StaffPIDep",
            CStaffPIDoc = "StaffPIDoc",
            CStaffPIExp = "StaffPIExp";

        // Random constants

        public const string
            AssemblyName = "Portal11",
            DDLAllGLCodes = "-- All GL Codes --",
            DDLAllProjectClasses = "-- All Project Classes --",
            DdlID = "ID",
            DdlName = "Name",
            DdlNeededSignal = "-1",
            EventArgument = "__EVENTARGUMENT",
            EventSupporting = "Supporting",
            ForeColor = "#18bc9c",
            ImageDir = "~/Images/", ImageTempName = "Temp_", ImageFullName = "Full_", ImageAvatarName = "Avatar_", ImageExt = ".jpg", ImageType = "image/jpeg",
            ImageIntermediateName = "Intermediate_", ImageIntermediateExt = ".temp",
            AvatarDir = "~/Images/Avatars/", DefaultAvatarDir = ImageDir, DefaultAvatar = "Avatar_Default.jpg", FullAvatar = "Full_Default.jpg", EmptyAvatar = "Avatar_Empty.jpg",
            ReadmeDir = "~/App_Readme/",
            ReturnNotePresent = "You have pressed the Approve button with text in the Return Note field. Please press the 'Clear' button next to the Return Note field to proceed.",
            ReturnNoteMissing = "Please supply a Return Note before pressing the Return button.",
            SupportingDir = "~/Supporting/",
            SupportingTempFlag = "T",
            WhatsNewName = "WhatsNew.txt";
        public const int
            AvatarHeightWidth = 50,
            gvAllPersonEmailColumn = 2,
            gvAllPersonW9Column = 3,
            SingleClickTimeout = 400,
            MaxProjectNameLength1 = 50, MaxProjectNameLength2 = 50,
            MaxSupportingFileSize = 6000000,
            gvProjectPersonW9Column = 3;

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
            URLEditDocument = "~/Rqsts/EditDocument",
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
            URLFlowControl = "~/Logic/FlowControl.aspx",
            URLForgot = "~/Account/Forgot.aspx",
            URLForgot1 = "/Account/Forgot",
            URLListProjectMetadata = "~/Lists/ListProjectMetadata.aspx",
            URLLogin = "~/Account/Login",
            URLLoginDispatch = "~/Account/LoginDispatch",
            URLProjectDashboard = "~/Proj/ProjectDashboard",
            URLResetPassword = "/Account/ResetPassword",
            URLResetPasswordConfirmation = "~/Account/ResetPasswordConfirmation",
            URLReviewApproval = "~/Rqsts/ReviewApproval",
            URLReviewDeposit = "~/Rqsts/ReviewDeposit",
            URLReviewDocument = "~/Rqsts/ReviewDocument",
            URLReviewExpense = "~/Rqsts/ReviewExpense",
            URLSelectEntity = "~/Select/SelectEntity",
            URLSelectGLCode = "~/Select/SelectGLCode",
            URLSelectPerson = "~/Select/SelectPerson",
            URLSelectProject = "~/Select/SelectProject",
            URLSelectProjectClass = "~/Select/SelectProjectClass",
            URLSelectUser = "~/Select/SelectUser",
            URLSelectVendor = "~/Select/SelectVendor",
            URLStaffDashboard = "~/Staff/StaffDashboard",
            URLViewDoc = "~/Proj/ViewDoc";

        // Names of check boxes in the Contract panel used by EditDocument

        public const string
            RDOContractEntity = "Entity",
            RDOContractPerson = "Person",
            RDOContractReceivingFunds = "ContractReceivingFunds",
            RDOContractPayingFunds = "ContractPayingFunds",
            CBLCampaignVerifyMention = "CampaignVerifyMention",
            CBLContractVerifyProjectName = "ContractVerifyProjectName",
            CBLContractVerifyCTPresident = "ContractVerifyCTPresident",
            CBLContractVerifySchedule = "ContractVerifySchedule",
            CBLGrantVerifyNarrative = "GrantVerifyNarrative",
            CBLGrantVerifyBudget = "GrantVerifyBudget";

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
            NewsFileDir = "~//Images//", NewsFileName="News.pdf", NewsFileMIME = "application/pdf", NewsOutputFile = "CultureWorks News.pdf",
            ZipFileDir = "TempZipFiles\\", ZipExt = ".ZIP", ZipMIME = "APPLICATION/X-ZIPCOMPRESSED", ZipOutputFile = "Supporting Documents.ZIP",
            EmailAddress = "portal@cultureworksfoundry.org", EmailPassword = "GUsr9o9y3Aj4&", EmailServer = "mail.cultureworksfoundry.org",
            POVendorModeYes = "Yes",
            POVendorModeNo = "No",
            DeliveryInstructionsRush = "Rush",
            DocumentVerify1Starter = "Project name represented as 'Your Project of ",
            SplitGLDeenergized = "Split", SplitGLEnergized = "Cancel";

        // Alternate Expense status displays for special cases

        public const string 
            ExpTypePaidPickup = "Check Cut",
            ExpTypePaidPEX = "Card Generated/Funds Transferred";

        // Error message common to Edit Deposit, Edit Document, and Edit Expense. User has pressed <Back> and then <Submit>. That's a no-no.

        public const string
            RequestReSubmitError = "You have sinned. The request has NOT been submitted. Please use the dashboard to edit or revise this request.";

        public const int
            EmailPort = 587;

        public const decimal
            BadDecimalValue = -123456;


        // Highlight colors in Dashboards

        public static Color DashboardRevising
        {
            get
            {
                return Color.Teal;
            }
        }

        public static Color DashboardRush
        {
            get
            {
                return Color.Red;
            }
        }

        public static Color DashboardSecondFD
        {
            get
            {
                return Color.Purple;
            }
        }
    }
}