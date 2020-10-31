using System;

namespace Com.TheFallenGames.OSA.CustomAdapters.TableView
{
	public interface IColumnState
	{
		IColumnInfo Info { get; }
		TableValueSortType CurrentSortingType { get; set; }
		bool CurrentlyReadOnly { get; set; }
	}
}