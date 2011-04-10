using System;
using System.Diagnostics.Contracts;

namespace LoanTracker.Models
{
	public class Payment
	{

		[ContractInvariantMethod]
		private void ObjectInvariant()
		{
			Contract.Invariant(this.Amount > 0);
		}

		public Payment(decimal amount, DateTime paymentDate)
		{
			Contract.Requires(amount > 0);
			Contract.Requires(paymentDate != null);

			_Amount = amount;
			_Date = paymentDate;
		}

		private readonly decimal _Amount;
		public decimal Amount
		{
			get
			{
				return _Amount;
			}
		}

		private readonly DateTime _Date;
		public DateTime Date
		{
			get
			{
				return _Date;
			}
		}

	}
}
