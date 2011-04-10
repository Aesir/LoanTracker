using System;
using System.Collections.Generic;
using System.Linq;
using LoanTracker.Models;
using ReactiveUI;

namespace LoanTracker.ViewModels
{
	public class PersonViewModel
		: PropertyChangedBase
	{
		private readonly Person myPerson;
		private readonly IMessageBus myMessenger;

		public PersonViewModel(IMessageBus messenger, Person person)
		{
			myPerson = person;
			this.LoansInRepayment =
				myPerson
				.Borrowed
				.Where(l => l.OutstandingBalance(DateTime.Now) > 0)
				.Select(l => new LoanViewModel(myMessenger, l));


			myMessenger = messenger;
			myMessenger
				.Listen<Person>("BalanceChanged")
				.ObserveOnDispatcher()
				.Subscribe(
					p =>
					{
						if (p == myPerson)
							NotifyPropertyChanged("Balance");
					});
		}

		public string Name
		{
			get
			{
				return myPerson.Name;
			}
		}

		public decimal Balance
		{
			get
			{
				return myPerson.Balance;
			}
		}

		private IEnumerable<LoanViewModel> _LoansInRepayment;
		public IEnumerable<LoanViewModel> LoansInRepayment
		{
			get
			{
				return _LoansInRepayment;
			}
			private set
			{
				if (!ReferenceEquals(_LoansInRepayment, value))
				{
					_LoansInRepayment = value;
					NotifyPropertyChanged("LoansInRepayment");
				}
			}
		}
	}
}
