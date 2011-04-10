using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace LoanTracker.Models
{
	public class Loan
	{

		public Loan(Person lender, Person borrower, decimal principle, decimal interest)
			: this(lender, borrower, principle, interest, DateTime.Now)
		{
		}

		public Loan(Person lender, Person borrower, decimal principle, decimal interestRate, DateTime openingDate)
		{
			_Lender = lender;
			_Borrower = borrower;
			_InitialPrinciple = principle;
			_APR = interestRate;
			_OpeningDate = openingDate;

			_Payments = new SortedList<DateTime, Payment>();
		}

		private readonly Person _Lender;
		public Person Lender
		{
			get
			{
				return _Lender;
			}
		}

		private readonly Person _Borrower;
		public Person Borrower
		{
			get
			{
				return _Borrower;
			}
		}

		private readonly decimal _InitialPrinciple;
		public decimal InitialPrinciple
		{
			get
			{
				return _InitialPrinciple;
			}
		}

		private readonly DateTime _OpeningDate;
		public DateTime OpeningDate
		{
			get
			{
				return _OpeningDate;
			}
		}

		private readonly decimal _APR;
		public decimal APR
		{
			get
			{
				return _APR;
			}
		}

		private readonly SortedList<DateTime, Payment> _Payments;
		public IEnumerable<Payment> Payments
		{
			get
			{
				return _Payments.Values;
			}
		}

		public decimal OutstandingBalance(DateTime balanceDate)
		{
			Contract.Requires(balanceDate >= this.OpeningDate);
			Contract.Ensures(Contract.Result<decimal>() >= 0m);

			decimal outstandingBalance = this.InitialPrinciple;
			var calculationDate = this.OpeningDate;
			int paymentIndex = 0;
			Payment nextPaymentToApply = this.Payments.FirstOrDefault();

			while (calculationDate < balanceDate)
			{
				calculationDate = calculationDate.AddMonths(1);

				//Apply payments
				while (nextPaymentToApply != null
					&& nextPaymentToApply.Date <= calculationDate)
				{
					outstandingBalance -= nextPaymentToApply.Amount;
					paymentIndex += 1;
					nextPaymentToApply = this.Payments.Skip(paymentIndex).FirstOrDefault();
				}

				if (calculationDate < balanceDate)
				{
					//Apply interest
					outstandingBalance += outstandingBalance * (this.APR / 1200m);
				}
			}

			return outstandingBalance;
		}

		public bool MakePayment(decimal amount, DateTime paymentDate)
		{
			Contract.Requires(amount > 0);
			Contract.Requires(paymentDate >= this.OpeningDate);

			if (!this.Borrower.Withdraw(amount))
			{
				return false;
			}

			var currentBalance = OutstandingBalance(paymentDate);
			decimal appliedAmount = amount;
			if (currentBalance < amount)
			{
				this.Borrower.Deposit(amount - currentBalance);
				appliedAmount = currentBalance;
			}

			var payment = new Payment(appliedAmount, paymentDate);
			_Payments.Add(payment.Date, payment);
			this.Lender.Deposit(appliedAmount);

			return true;
		}

	}
}
