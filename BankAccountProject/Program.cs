// see https://aka.ms/new-console-template for more information
//comment shortcut Ctrl+K+C
//uncomment shortcut Ctrl+K+U


using System.Text;

namespace BankAccountProject
{
    //If no public keyword, then it's private by default.
    class Program 
    { 
        //If no public keyword, then it's private by default.
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //Create a bird object using Bird Class
            var bird = new Bird("kiki", "seeds", 24, 70);
            Console.WriteLine($"Bird name is {bird.name}. It is {bird.age} years old, it weighs {bird.weight}kg, and it loves {bird.feed}.");

            //Create a account object using BankAccount Class
            var account = new BankAccount("Hellen", 1000);
            Console.WriteLine($"Account {account.Number} was created for {account.Owner} with {account.Balance} initial balance.");

            //Say Hellen make a withdraw and a deposite
            account.MakeWithdrawal(700, DateTime.Now, "Rent payment");
            Console.WriteLine($"account balance after withdrawal: {account.Balance}");
            account.MakeDeposit(100, DateTime.Now, "Friend paid me back");
            Console.WriteLine($"account balance after deposite: {account.Balance}");

            /*
            // Test that the initial balances must be positive.
            BankAccount invalidAccount;
            try
            {
                invalidAccount = new BankAccount("invalid", -55);
            }
            //Catch is great, because it lets us continue with the program even if there is an exception.
            //It just ignores the line that causes exception as if it doesn't exit.
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine("Exception caught creating account with negative balance");
                Console.WriteLine(e.ToString());
                return;
            }

            // Test for a negative balance.
            try
            {
                account.MakeWithdrawal(750, DateTime.Now, "Attempt to overdraw");
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("Exception caught trying to overdraw");
                Console.WriteLine(e.ToString());
            }
            */

            Console.WriteLine(account.GetAccountHistory());

            var giftCard = new GiftCardAccount("gift card", 100, 50);
            giftCard.MakeWithdrawal(20, DateTime.Now, "get expensive coffee");
            giftCard.MakeWithdrawal(50, DateTime.Now, "buy groceries");
            giftCard.PerformMonthEndTransactions();
            // can make additional deposits:
            giftCard.MakeDeposit(27.50m, DateTime.Now, "add some additional spending money");
            Console.WriteLine(giftCard.GetAccountHistory());

            var savings = new InterestEarningAccount("savings account", 10000);
            savings.MakeDeposit(750, DateTime.Now, "save some money");
            savings.MakeDeposit(1250, DateTime.Now, "Add more savings");
            savings.MakeWithdrawal(250, DateTime.Now, "Needed to pay monthly bills");
            savings.PerformMonthEndTransactions();
            Console.WriteLine(savings.GetAccountHistory());

            var lineOfCredit = new LineOfCreditAccount("line of credit", 0, 2000);
            // How much is too much to borrow?
            lineOfCredit.MakeWithdrawal(1000m, DateTime.Now, "Take out monthly advance");
            lineOfCredit.MakeDeposit(50m, DateTime.Now, "Pay back small amount");
            lineOfCredit.MakeWithdrawal(5000m, DateTime.Now, "Emergency funds for repairs");
            lineOfCredit.MakeDeposit(150m, DateTime.Now, "Partial restoration on repairs");
            lineOfCredit.PerformMonthEndTransactions();
            Console.WriteLine(lineOfCredit.GetAccountHistory());
        }
    }


    public class Bird
    {   
        //properties of Bird
        public string name;
        public string feed;
        public int age;
        public decimal weight;

        //Bird constructor
        public Bird(string name, string feed, int age, decimal weight)
        {
            this.name = name;
            this.feed = feed;
            this.age = age;
            this.weight = weight;
        }
    }


    public class BankAccount
    {
        //Static means it is shared by all of the BankAccount objects.
        //It is also private, so nothing outside of BankAccount class can access it.
        private static int accountNumberSeed = 1234567890;
        public string Number { get; }
        public string Owner { get; set; }
        private readonly decimal _minimumBalance;
        public decimal Balance
        {
            get
            {
                decimal balance = 0;
                foreach (var item in allTransactions)
                {
                    balance += item.Amount;
                }

                return balance;
            }
        }

        //Public constructor. But the derived classes of this class does not use this constructor by default. We have to call it.
        public BankAccount(string name, decimal initialBalance, decimal minimumlBalance)
        {
            this.Owner = name;
            this.Number = accountNumberSeed.ToString();
            accountNumberSeed++;
            this._minimumBalance = minimumlBalance;

            //Ensuring the current instance of an account is not a LineOfCreditAccount. As that would have initialBalance = 0.
            //So people can only make deposite to a non-LineOfCreditAccount
            if (initialBalance > 0)
                //When someone opens a bank account, they should make a deposite, and the amount they deposite is initial balance.
                MakeDeposit(initialBalance, DateTime.Now, "Initial balance");
        }

        //This is also a constructor for BankAccount Class.
        //It calls for the first BankAccount constructor, which takes in 3 parameters. By using ":this" structure, it allows opening a bank account with only 2 parameters.
        //Note when opening a bank account with only 2 parameters, it requires minimalBalance=0, as it is not a LineOfCreditAccount.
        public BankAccount(string name, decimal initialBalance) : this(name, initialBalance, 0) { }

        private List<Transaction> allTransactions = new List<Transaction>();

        public void MakeDeposit(decimal amount, DateTime date, string note)
        {
            if (amount <= 0)
            {
                //Just like Python, when an exception occurs, it stops the program.
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount of deposit must be positive");
            }
            var deposit = new Transaction(amount, date, note);
            allTransactions.Add(deposit);
        }

        //protected: only things in this class and its derived classes can access this method.
        //virtual: this method can be overridden by derived classes.
        //Transaction?: this method can either return a Transaction object or Null. "?" after a variable makes it "Nullable".
        protected virtual Transaction? CheckWithdrawalLimit(bool isOverdrawn)
        {
            if (isOverdrawn)
            {
                //If we enter here, the function returns the exception, and stops here.
                throw new InvalidOperationException("Not sufficient funds for this withdrawal");
            }
            else
            {
                //default of Transaction object is Null. 
                return default;
            }
        }

        public void MakeWithdrawal(decimal amount, DateTime date, string note)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount of withdrawal must be positive");
            }
            //with ?, it declares that the overdraftTransaction variable can either be a Transaction type or Null type.
            Transaction? overdraftTransaction = CheckWithdrawalLimit(this.Balance - amount < this._minimumBalance);
            //short hand for `Transaction withdrawal = new Transaction(-amount, date, note);`. This simplification can be used when type is apparant.
            Transaction? withdrawal = new(-amount, date, note);
            allTransactions.Add(withdrawal);
            if (overdraftTransaction != null) //if isOverdrawn = True
                allTransactions.Add(overdraftTransaction);
        }




        public string GetAccountHistory()
        {
            var report = new StringBuilder();

            decimal balance = 0;
            //Header of report.
            report.AppendLine("Date\t\tAmount\tBalance\tNote");
            foreach (var item in allTransactions)
            {
                //Rows of report
                balance += item.Amount;
                report.AppendLine($"{item.Date.ToShortDateString()}\t{item.Amount}\t{balance}\t{item.Notes}");
            }

            //Almost every object in C# has a ToString method.
            return report.ToString();
        }

        //This is a method.
        //Virtual means this class's derived classes may overide this method.
        //Since each derived classes needs to use this method in a different way, we leave it blank here.
        public virtual void PerformMonthEndTransactions() { }

    }


    public class Transaction
    {
        public decimal Amount { get; }
        public DateTime Date { get; }
        public string Notes { get; }

        public Transaction(decimal amount, DateTime date, string note)
        {
            this.Amount = amount;
            this.Date = date;
            this.Notes = note;
        }
    }
}
