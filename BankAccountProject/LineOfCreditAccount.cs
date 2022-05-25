using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankAccountProject
{
    //LineOfCreditAccount Class is a derived class from BankAccount Class.
    //So BankAccount Class is the base class.
    public class LineOfCreditAccount : BankAccount
    {
        //Constructor of this Class. It calls for the constructor of base. Except creditLimit = -minimalBalance, which makes sense logically.
        public LineOfCreditAccount(string name, decimal initialBalance, decimal creditLimit) : base(name, initialBalance, -creditLimit)
        {
        }

        //This method overides the same method in the base class.
        public override void PerformMonthEndTransactions()
        {
            if (Balance < 0)
            {
                // Negate the balance to get a positive interest charge:
                decimal interest = -Balance * 0.07m;
                MakeWithdrawal(interest, DateTime.Now, "Charge monthly interest");
            }
        }

        //The ?: operator means the following
        //is this condition true ? if yes do this: if no do this
        protected override Transaction? CheckWithdrawalLimit(bool isOverdrawn) => //=> means return
            isOverdrawn
            ? new Transaction(-20, DateTime.Now, "Apply overdraft fee")
            : default; //Null is the default of Transaction object.
    }
}
