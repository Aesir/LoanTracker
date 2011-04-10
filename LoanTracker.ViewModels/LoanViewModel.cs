using System;
using System.Linq;
using System.Windows.Input;
using LoanTracker.Models;
using ReactiveUI;

namespace LoanTracker.ViewModels
{
	public class LoanViewModel
		: PropertyChangedBase
	{
		private readonly Loan myLoan;
		private readonly IMessageBus myMessenger;

		public LoanViewModel(IMessageBus messenger, Loan loan)
		{
			myLoan = loan;
			_OweTo = myLoan.Lender.Name;
			_APR = myLoan.APR;
			_OutstandingBalance = myLoan.OutstandingBalance(DateTime.Now);

			_MakePaymentCommand = new DelegateCommand(MakePaymentExecute, MakePaymentCanExecute);

			myMessenger = messenger;
			myMessenger
				.Listen<Loan>("PaymentMade")
				.ObserveOnDispatcher()
				.Subscribe(
					l =>
					{
						if (l == myLoan)
						{
							myMessenger.SendMessage<Person>(loan.Lender, "BalanceChanged");
							myMessenger.SendMessage<Person>(loan.Borrower, "BalanceChanged");
							this.PaymentMaker = null;
							this.OutstandingBalance = myLoan.OutstandingBalance(DateTime.Now);
						}
					});
		}

		private readonly decimal _APR;
		public decimal APR
		{
			get
			{
				return _APR;
			}
		}

		private decimal _OutstandingBalance;
		public decimal OutstandingBalance
		{
			get
			{
				return _OutstandingBalance;
			}
			private set
			{
				if (!ReferenceEquals(_OutstandingBalance, value))
				{
					_OutstandingBalance = value;
					NotifyPropertyChanged("OutstandingBalance");
				}
			}
		}


		private readonly string _OweTo;
		public string OweTo
		{
			get
			{
				return _OweTo;
			}
		}

		private LoanPaymentViewModel _PaymentMaker;
		public LoanPaymentViewModel PaymentMaker
		{
			get
			{
				return _PaymentMaker;
			}
			private set
			{
				if (!ReferenceEquals(_PaymentMaker, value))
				{
					_PaymentMaker = value;
					NotifyPropertyChanged("PaymentMaker");
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
			return this.PaymentMaker == null;
		}

		private void MakePaymentExecute(object cmdParam)
		{
			this.PaymentMaker = new LoanPaymentViewModel(myMessenger, myLoan);
		}

	}
}
