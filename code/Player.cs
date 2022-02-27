using Instagib;
using Sandbox;
using Template.UI;

namespace Template
{
	public partial class Player : Sandbox.Player
	{
		public Player()
		{
			Inventory = new BaseInventory( this );
		}
		
		public override void Respawn()
		{
			SetModel( "models/citizen/citizen.vmdl" );

			Controller = new WalkController();
			Animator = new StandardPlayerAnimator();
			Camera = new ThirdPersonCamera();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			Inventory.Add( new TestWeapon(), true );

			base.Respawn();
		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );
			SimulateActiveChild( cl, ActiveChild );
			TickPlayerUse();

			if ( Input.Pressed( InputButton.View ) )
			{
				if ( Camera is ThirdPersonCamera )
					Camera = new FirstPersonCamera();
				else
					Camera = new ThirdPersonCamera();
			}

			if ( Input.Pressed( InputButton.Menu ) && IsClient )
			{
				string[] icons = new[] { "priority_high", "drive_eta", "live_tv", "sync" };
				// Vitals.Instance.RemoveItem( "test" );
				MercuryHud.Instance.AddItem( MercuryHud.HudAlignment.Left, "test", Rand.FromArray( icons ), () => "Hello!" );
			}
		}

		public override void OnKilled()
		{
			base.OnKilled();

			EnableDrawing = false;
			EnableAllCollisions = false;
		}
	}
}
