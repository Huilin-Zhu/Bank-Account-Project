using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankAccountProject
{
    //InterestEarningAccount Class is a derived class from BankAccount Class.
    //So BankAccount Class is the base class.
    public class InterestEarningAccount : BankAccount
    {
        //Constructor for this class.
        //It is calling the constructor from the bass class, ie BankAccount Class. Where the parameters should be kept the same.
        public InterestEarningAccount(string name, decimal initialBalance) : base(name, initialBalance)
        {
        }

        //This method overides the same method in the base class
        public override void PerformMonthEndTransactions()
        {
            if (Balance > 500m)
            {
                decimal interest = Balance * 0.05m;
                MakeDeposit(interest, DateTime.Now, "apply monthly interest");
            }
        }
    }
}
