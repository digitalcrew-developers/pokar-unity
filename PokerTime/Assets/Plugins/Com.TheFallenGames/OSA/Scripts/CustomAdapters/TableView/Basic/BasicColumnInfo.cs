using System;

namespace Com.TheFallenGames.OSA.CustomAdapters.TableView.Basic
{
	public class BasicColumnInfo : IColumnInfo
	{
		public string Name
		{
			get { return _Name; }
			set
			{
				if (_Name == value)
					return;

				_Name = value;
				ReconstructDisplayName();
			}
		}
		public string DisplayName { get; set; }
		public TableValueType ValueType { get; private set; }
		public Type EnumValueType { get; private set; }

		string _Name;


		public BasicColumnInfo(string name, TableValueType valueType, Type enumValueType = null)
		{
			ValueType = valueType;
			EnumValueType = enumValueType;

			// Setting it last, so the display name will be reconstructed using the other properties
			Name = name;
		}

		void ReconstructDisplayName()
		{
			DisplayName = ConstructColumnDisplayName(Name, ValueType, EnumValueType);
		}

		public static string ConstructColumnDisplayName(string name, TableValueType valueType, Type enumValueType = null)
		{
			string innerStr;
			if (valueType == TableValueType.ENUMERATION)
				innerStr = "ENUM <i>" + (enumValueType == null ? "<Unknown>" : enumValueType.Name) + "</i>";
			else
				innerStr = valueType.ToString();

			return name + "\n<color=#00000070><size=12>" + innerStr + "</size></color>";
		}
	}
}