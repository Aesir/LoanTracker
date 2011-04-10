using System;
using System.Diagnostics.Contracts;
using System.Windows.Input;

namespace LoanTracker.ViewModels
{
	/// <summary>
	/// Creates a simple ICommand from delegate methods
	/// </summary>
	internal class DelegateCommand
		: ICommand
	{
		private Action<object> m_execute;
		private Func<object, bool> m_canExecute;

		public DelegateCommand(Action<object> execute)
			: this(execute, o => true)
		{
			Contract.Requires(execute != null);
		}

		public DelegateCommand(Action<object> execute, Func<object, bool> canExecute)
		{
			Contract.Requires(execute != null);
			Contract.Requires(canExecute != null);

			m_execute = execute;
			m_canExecute = canExecute;
		}

		/// <summary>
		/// Defines the method that determines whether the command can execute in its current state.
		/// </summary>
		/// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
		/// <returns>
		/// true if this command can be executed; otherwise, false.
		/// </returns>
		[Pure]
		public bool CanExecute(object parameter)
		{
			return m_canExecute(parameter);
		}

		/// <summary>
		/// Occurs when changes occur that affect whether or not the command should execute.
		/// </summary>
		public event EventHandler CanExecuteChanged
		{
			add
			{
				CommandManager.RequerySuggested += value;
			}
			remove
			{
				CommandManager.RequerySuggested -= value;
			}
		}

		/// <summary>
		/// Defines the method to be called when the command is invoked.
		/// </summary>
		/// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
		[Pure]
		public void Execute(object parameter)
		{
			Contract.Assume(this.CanExecute(parameter));
			m_execute(parameter);
		}

		[ContractInvariantMethod()]
		private void ClassInvariants()
		{
			Contract.Invariant(m_execute != null);
			Contract.Invariant(m_canExecute != null);
		}
	}

	/// <summary>
	/// Creates a simple ICommand from delegate methods that have a typed command parameter
	/// </summary>
	internal class GenericDelegateCommand<TParam>
		: ICommand
	{
		private Action<TParam> m_execute;
		private Func<TParam, bool> m_canExecute;

		public GenericDelegateCommand(Action<TParam> execute)
			: this(execute, o => true)
		{
			Contract.Requires(execute != null);
		}

		public GenericDelegateCommand(Action<TParam> execute, Func<TParam, bool> canExecute)
		{
			Contract.Requires(execute != null);
			Contract.Requires(canExecute != null);

			m_execute = execute;
			m_canExecute = canExecute;
		}

		/// <summary>
		/// Defines the method that determines whether the command can execute in its current state.
		/// </summary>
		/// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
		/// <returns>
		/// true if this command can be executed; otherwise, false.
		/// </returns>
		[Pure]
		public bool CanExecute(object parameter)
		{
			if (parameter == null)
			{
				return m_canExecute(default(TParam));
			}

			if (!typeof(TParam).IsInstanceOfType(parameter))
			{
				return false;
			}
			return m_canExecute((TParam)parameter);
		}

		/// <summary>
		/// Occurs when changes occur that affect whether or not the command should execute.
		/// </summary>
		public event EventHandler CanExecuteChanged
		{
			add
			{
				CommandManager.RequerySuggested += value;
			}
			remove
			{
				CommandManager.RequerySuggested -= value;
			}
		}


		/// <summary>
		/// Defines the method to be called when the command is invoked.
		/// </summary>
		/// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
		[Pure]
		public void Execute(object parameter)
		{
			Contract.Assume(this.CanExecute(parameter));

			if (parameter != null)
				m_execute((TParam)parameter);
			else
				m_execute(default(TParam));
		}

		[ContractInvariantMethod()]
		private void ClassInvariants()
		{
			Contract.Invariant(m_execute != null);
			Contract.Invariant(m_canExecute != null);
		}
	}
}
