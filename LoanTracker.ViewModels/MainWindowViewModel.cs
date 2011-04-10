using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LoanTracker.Models;
using ReactiveUI;

namespace LoanTracker.ViewModels
{
	public class MainWindowViewModel
	{
		private readonly IMessageBus myMessenger;

		public MainWindowViewModel()
		{
			myMessenger = new MessageBus();
			_People = Person.GetEveryone().Select(p => new PersonViewModel(myMessenger, p));
		}

		private readonly IEnumerable<PersonViewModel> _People;
		public IEnumerable<PersonViewModel> People
		{
			get
			{
				return _People;
			}
		}
		
	}
}
