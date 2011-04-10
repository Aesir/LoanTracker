using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace LoanTracker.Models
{
	public class Person
	{
		[ContractInvariantMethod]
		private void ObjectInvariant()
		{
			Contract.Invariant(this.Balance >= 0);
			Contract.Invariant(!String.IsNullOrWhiteSpace(this.Name));
			Contract.Invariant(this.Lent != null);
			Contract.Invariant(this.Borrowed != null);
		}

		public Person(string name, decimal openingBalance)
		{
			Contract.Requires(!String.IsNullOrWhiteSpace(Name));
			Contract.Requires(openingBalance >= 0);

			_Name = name;
			this.Balance = openingBalance;

			_Lent = new List<Loan>();
			_Borrowed = new List<Loan>();
		}

		private readonly string _Name;
		public string Name
		{
			get
			{
				return _Name;
			}
		}

		public decimal Balance
		{
			get;
			private set;
		}

		private readonly IList<Loan> _Lent;
		public IEnumerable<Loan> Lent
		{
			get
			{
				return _Lent;
			}
		}

		private readonly IList<Loan> _Borrowed;
		public IEnumerable<Loan> Borrowed
		{
			get
			{
				return _Borrowed;
			}
		}



		public void Deposit(decimal amount)
		{
			Contract.Requires(amount > 0);

			this.Balance += amount;
		}

		public bool Withdraw(decimal amount)
		{
			Contract.Requires(amount > 0);

			if (this.Balance < amount)
			{
				return false;
			}

			this.Balance -= amount;
			return true;
		}

		public bool Borrow(Person lender, decimal amount)
		{
			Contract.Requires(lender != null);
			Contract.Requires(amount > 0);

			if (!lender.Withdraw(amount))
			{
				return false;
			}
			else
			{
				this.Deposit(amount);
			}

			var loan = new Loan(lender, this, amount, 6.0m);
			this._Borrowed.Add(loan);
			lender._Lent.Add(loan);
			return true;
		}


		private static IEnumerable<Person> _Everyone;
		public static IEnumerable<Person> GetEveryone()
		{
			if (_Everyone == null)
			{
				var data = new List<Person>()
				{
					new Person("Jake", 10000m),
					new Person("Beth", 10000m),
					new Person("Ethan", 10000m),
					new Person("Jessica", 10000m),
					new Person("Mike", 10000m),
					new Person("Margret", 10000m),
					new Person("Alex", 10000m),
					new Person("Kim", 10000m),
					new Person("William", 10000m),
					new Person("Laura", 10000m),
					new Person("Josh", 10000m),
					new Person("Amanda", 10000m),
					new Person("Dan", 10000m),
					new Person("Emily", 10000m),
					new Person("Jayden", 10000m),
					new Person("Ayla", 10000m),
					new Person("Noah", 10000m),
					new Person("Mary", 10000m),
					new Person("Anthony", 10000m),
					new Person("Christine", 10000m)
				};

				var rand = new Random();
				for (int i = 0; i < 20; i++)
				{
					int borrowerIndex = rand.Next(data.Count());
					int lenderIndex;
					do
					{
						lenderIndex = rand.Next(data.Count());
					} while (borrowerIndex == lenderIndex);

					data[borrowerIndex].Borrow(data[lenderIndex], 2000m);
				}

				_Everyone = data;
			}

			return _Everyone;
		}

	}
}
