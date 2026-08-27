namespace Opgave03;

public class BankAccount
{
    public string AccountNumber { get; init; }

    public string Owner
    {
        get { return Owner; }
        set
        {
            if (value == "" || value == null)
            {
                throw new ArgumentException("Owner cannot be empty");
            }
        }
    }

    public decimal Balance { get; private set; }

    public bool IsOverdrawn => Balance < 0;

    public string FormattedBalance => Balance.ToString("DKK");

    public void Deposit(decimal amount)
    {
        if (amount > 0)
            Balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (amount < 0)
        {
            Balance -= amount;
        }
    }
}