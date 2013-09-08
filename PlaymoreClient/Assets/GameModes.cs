using System;
using System.ComponentModel;

namespace PlaymoreClient.Assets
{
	[TypeConverter(typeof(DescriptionEnumTypeConverter<GameTypes>))]
	public enum GameModes
	{
		UNKNOWN,
		[Description("Dominion")]
		ODIN,
		[Description("Classic")]
		CLASSIC
	}
}