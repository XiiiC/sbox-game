using Sandbox.UI;

namespace Instagib.UI
{
	public class Crosshair : Panel
	{
		public Crosshair()
		{
			SetClass( "crosshair", true );
			StyleSheet.Load( "/Code/UI/Hud.scss" );
		}
	}
}
