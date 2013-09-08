using System;
using System.ComponentModel;

namespace PlaymoreClient.Assets
{
	[TypeConverter(typeof(DescriptionEnumTypeConverter<GameTypes>))]
	public enum GameTypes
	{
		UNKNOWN,
		[Description("Ranked")]
		RANKED_GAME,
		[Description("Match")]
		MATCHED_GAME
	}
}