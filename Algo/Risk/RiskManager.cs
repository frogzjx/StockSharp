#region S# License
/******************************************************************************************
NOTICE!!!  This program and source code is owned and licensed by
StockSharp, LLC, www.stocksharp.com
Viewing or use of this code requires your acceptance of the license
agreement found at https://github.com/StockSharp/StockSharp/blob/master/LICENSE
Removal of this comment is a violation of the license agreement.

Project: StockSharp.Algo.Risk.Algo
File: RiskManager.cs
Created: 2015, 11, 11, 2:32 PM

Copyright 2010 by StockSharp, LLC
*******************************************************************************************/
#endregion S# License
namespace StockSharp.Algo.Risk
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Ecng.Collections;
	using Ecng.Serialization;

	using MoreLinq;

	using StockSharp.Messages;

	/// <summary>
	/// The risks control manager.
	/// </summary>
	public class RiskManager : IRiskManager
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RiskManager"/>.
		/// </summary>
		public RiskManager()
		{
		}

		private readonly CachedSynchronizedSet<IRiskRule> _rules = new CachedSynchronizedSet<IRiskRule>();

		/// <summary>
		/// Rule list.
		/// </summary>
		public SynchronizedSet<IRiskRule> Rules => _rules;

		/// <summary>
		/// To reset the state.
		/// </summary>
		public virtual void Reset()
		{
			_rules.Cache.ForEach(r => r.Reset());
		}

		/// <summary>
		/// To process the trade message.
		/// </summary>
		/// <param name="message">The trade message.</param>
		/// <returns>List of rules, activated by the message.</returns>
		public IEnumerable<IRiskRule> ProcessRules(Message message)
		{
			if (message.Type == MessageTypes.Reset)
			{
				Reset();
				return Enumerable.Empty<IRiskRule>();
			}

			return _rules.Cache.Where(r => r.ProcessMessage(message)).ToArray();
		}

		/// <summary>
		/// Load settings.
		/// </summary>
		/// <param name="storage">Storage.</param>
		public void Load(SettingsStorage storage)
		{
			Rules.AddRange(storage.GetValue<SettingsStorage[]>(nameof(Rules)).Select(s => s.LoadEntire<IRiskRule>()));
		}

		/// <summary>
		/// Save settings.
		/// </summary>
		/// <param name="storage">Storage.</param>
		public void Save(SettingsStorage storage)
		{
			storage.SetValue(nameof(Rules), Rules.Select(r => r.SaveEntire(false)).ToArray());
		}

		RiskActions IRiskRule.Action
		{
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
		}

		string IRiskRule.Title
		{
			get { throw new NotSupportedException(); }
		}

		bool IRiskRule.ProcessMessage(Message message)
		{
			throw new NotSupportedException();
		}
	}
}