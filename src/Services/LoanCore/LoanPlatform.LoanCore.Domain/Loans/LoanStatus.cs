namespace LoanPlatform.LoanCore.Domain.Loans
{
    public enum LoanStatus
    {
        Pending = 0, //заявка/займ ожидает решения
        Approved = 1, //одобрен
        Active = 2, //займ выдан и обслуживается
        Closed = 3, //полностью погашен
        Rejected = 4 //отказ
    }
}