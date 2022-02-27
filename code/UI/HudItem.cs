using System;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Template.UI
{
	public class HudItem : Panel
	{
		public string ItemName { get; set; }
		
		private Func<string> Method { get; set; }
		private Label ValueLabel { get; set; }
			
		/// <summary>
		/// A new HUD item.
		/// </summary>
		/// <param name="name">The identifier for this item. Also shows up when icons are disabled.</param>
		/// <param name="icon">The https://fonts.google.com/icons icon for this item.</param>
		/// <param name="method">The method returning the value for this item.</param>
		public HudItem( string name, string icon, Func<string> method )
		{
			ItemName = name;
			Method = method;

			/*
				HTML reference:
			    <div class="health">
			        <text class="text-label">
			            HEALTH
			        </text>
			        <icon>add_box</icon>
			        <text @text="PlayerHealth">
			            100
			        </text>
			    </div>
		    */
			
			SetClass( name, true );

			var textLabel = Add.Label( name.ToUpper(), "text-label" );

			if ( !string.IsNullOrEmpty( icon ) )
				Add.Icon( icon );
			
			var valueLabel = Add.Label( "bogos binted :smike:" );

			StyleSheet.Load( "/Code/UI/Vitals.scss" );
			ValueLabel = valueLabel;
		}

		public override void Tick()
		{
			base.Tick();
			
			ValueLabel.Text = Method.Invoke();
		}
	}
}
