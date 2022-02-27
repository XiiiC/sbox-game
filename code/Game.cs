using Sandbox;

namespace Template
{
	[Library( "template", Title = "Template")]
	public partial class Game : Sandbox.Game
	{
		private static Hud hud;
		public Game()
		{
			if ( IsClient )
			{
				// LoadPrecacheFile( "/data/precache.json" );
			}
			
			if ( IsServer )
			{
				hud = new Hud();
			}
		}

		public override void ClientJoined( Client client )
		{
			base.ClientJoined( client );

			var player = new Player();
			client.Pawn = player;

			player.Respawn();
		}
	}
}
