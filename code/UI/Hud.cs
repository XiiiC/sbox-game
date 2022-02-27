using Instagib.UI;
using Sandbox;
using Sandbox.UI;
using Template.UI;

namespace Template
{
	public partial class Hud : Sandbox.HudEntity<RootPanel>
	{
		public Hud()
		{
			if ( !IsClient )
				return;

			RootPanel.AddChild<Crosshair>();
			RootPanel.AddChild<WaterOverlay>();
			RootPanel.AddChild<Scoreboard<ScoreboardEntry>>();
			
			SetupMercury();
				
			RootPanel.SetTemplate( "/Code/UI/Hud.html" );
		}

		private void SetupMercury()
		{
			var mercury = RootPanel.AddChild<MercuryHud>();

			//
			// Health
			//
			mercury.AddItem( MercuryHud.HudAlignment.Left,
				"health",
				"add_box",
				() => $"{Local.Pawn.Health.CeilToInt()}" );
				
			mercury.AddItem( MercuryHud.HudAlignment.Left,
				"suit",
				"security",
				() => $"{0f.CeilToInt()}" );
				
			mercury.AddItem( MercuryHud.HudAlignment.Left,
				"speed",
				"directions_run",
				() => $"{Local.Pawn.Velocity.Cross( Vector3.Up ).Length.FloorToInt()}u/s" );
				
			//
			// Ammo
			//
			mercury.AddItem( MercuryHud.HudAlignment.Right,
				"ammo",
				"",
				() => $"30" );
				
			mercury.AddItem( MercuryHud.HudAlignment.Right,
				"reserve",
				"/",
				() => $"90" );
		}
	}
}
