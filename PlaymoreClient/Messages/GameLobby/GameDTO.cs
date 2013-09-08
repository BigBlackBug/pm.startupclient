using PlaymoreClient.Flash;
using PlaymoreClient.Messages;
using FluorineFx;
using NotMissing;
using System;
using System.Runtime.CompilerServices;
using PlaymoreClient.Messages.GameLobby.Participants;

namespace PlaymoreClient.Messages.GameLobby
{
	[Message(".GameDTO")]
	public class GameDTO : MessageObject, ICloneable
	{
		[InternalName("bannedChampions")]
		public BannedChampionList BannedChampions
		{
			get;
			set;
		}

		[InternalName("gameMode")]
		public string GameMode
		{
			get;
			set;
		}

		[InternalName("gameState")]
		public string GameState
		{
			get;
			set;
		}

		[InternalName("gameType")]
		public string GameType
		{
			get;
			set;
		}

		[InternalName("id")]
		public long Id
		{
			get;
			set;
		}

		[InternalName("mapId")]
		public int MapId
		{
			get;
			set;
		}

		[InternalName("maxNumPlayers")]
		public int MaxPlayers
		{
			get;
			set;
		}

		[InternalName("name")]
		public string Name
		{
			get;
			set;
		}

		[InternalName("pickTurn")]
		public int PickTurn
		{
			get;
			set;
		}

		[InternalName("playerChampionSelections")]
		public PlayerChampionSelectionsList PlayerChampionSelections
		{
			get;
			set;
		}

		[InternalName("queueTypeName")]
		public string QueueTypeName
		{
			get;
			set;
		}

		[InternalName("teamOne")]
		public TeamParticipants TeamOne
		{
			get;
			set;
		}

		[InternalName("teamTwo")]
		public TeamParticipants TeamTwo
		{
			get;
			set;
		}

		public GameDTO() : base(null)
		{
		}

		public GameDTO(ASObject obj) : base(obj)
		{
            base.ObjectType = "GameDTO";
			BaseObject.SetFields<GameDTO>(this, obj);
		}

		public object Clone()
		{
			GameDTO gameDTO = new GameDTO();
			gameDTO.MaxPlayers = this.MaxPlayers;
			gameDTO.Name = this.Name;
			gameDTO.MapId = this.MapId;
			gameDTO.Id = this.Id;
			gameDTO.GameMode = this.GameMode;
			gameDTO.GameState = this.GameState;
			gameDTO.GameType = this.GameType;
			gameDTO.TeamOne = new TeamParticipants(this.TeamOne.Clone<Participant>());
			gameDTO.TeamTwo = new TeamParticipants(this.TeamTwo.Clone<Participant>());
			gameDTO.TimeStamp = base.TimeStamp;
			return gameDTO;
		}
        public override string ToString()
        {
            return "game_state " + GameState + "game_mode " + GameMode + "queuetype" + QueueTypeName + "game_type " + GameType + " team_one " + TeamOne.ToString() + "team_two " + TeamTwo.ToString() + " selections " + PlayerChampionSelections.ToString();
        }
	}
}