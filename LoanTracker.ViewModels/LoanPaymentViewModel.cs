using System;
using System.Windows.Input;
using LoanTracker.Models;
using ReactiveUI;


namespace LoanTracker.ViewModels
{
	public class LoanPaymentViewModel
		: PropertyChangedBase
	{
		private readonly Loan myLoan;
		private IMessageBus myMessenger;

		public LoanPaymentViewModel(IMessageBus messenger, Loan loan)
		{
			myLoan = loan;
			myMessenger = messenger;

			_MakePaymentCommand = new DelegateCommand(MakePaymentExecute, MakePaymentCanExecute);
		}

		private decimal _PaymentAmount;
		public decimal PaymentAmount
		{
			get
			{
				return _PaymentAmount;
			}
			set
			{
				if (!ReferenceEquals(_PaymentAmount, value))
				{
					_PaymentAmount = value;
					NotifyPropertyChanged("PaymentAmount");
				}
			}
		}

		private readonly ICommand _MakePaymentCommand;
		public ICommand MakePaymentCommand
		{
			get
			{
				return _MakePaymentCommand;
			}
		}

		private bool MakePaymentCanExecute(object cmdParam)
		{
			return this.PaymentAmount <= myLoan.Borrower.Balance;
		}

		private void MakePaymentExecute(object cmdParam)
		{
			myLoan.MakePayment(this.PaymentAmount, DateTime.Now);
			myMessenger.SendMessage<Loan>(myLoan, "PaymentMade");
		}

	}
}
