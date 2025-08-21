namespace SenegaleseAssociation.Areas.Admin.Models
{
    public class FinanceOverviewViewModel
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal MonthlyIncome { get; set; }
        public decimal MonthlyExpenses { get; set; }
        public int PendingDonations { get; set; }
        
        public decimal NetIncome => TotalIncome - TotalExpenses;
        public decimal MonthlyNet => MonthlyIncome - MonthlyExpenses;
        
        public List<FinanceTransactionViewModel> RecentTransactions { get; set; } = new();
    }

    public class FinanceTransactionViewModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Type { get; set; } = string.Empty; // Income/Expense
        public string Category { get; set; } = string.Empty;
    }

    public class DonationViewModel
    {
        public int Id { get; set; }
        public string DonorName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
    }

    public class ExpenseViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; } = string.Empty;
        public string ApprovedBy { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class FinanceReportsViewModel
    {
        public List<decimal> MonthlyIncomeData { get; set; } = new();
        public List<decimal> MonthlyExpenseData { get; set; } = new();
        public List<string> MonthLabels { get; set; } = new();
        public Dictionary<string, decimal> CategoryBreakdown { get; set; } = new();
    }
}