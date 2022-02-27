using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Sandbox.UI;

namespace Template.UI
{
	/// <summary>
	/// Mercury HUD panel base.
	/// </summary>
	public class MercuryHud : Panel
	{
		/// <summary>
		/// Where do you want the UI element to go?
		/// </summary>
		public enum HudAlignment
		{
			Left,
			Right
			
			// TODO: Center
		}
		
		public static MercuryHud Instance { get; set; }
		
		#region Presets

		// Could also load these from JSON
		public static Dictionary<string, string[]> Presets { get; } = new()
		{
			{ "hl2", new[] { "hud_color #FEDD2E", "hud_font Roboto", "hud_icons 0" } },
			{ "s&box", new[] { "hud_color #FFA200", "hud_font Roboto", "hud_icons 1" } },
			{ "default", new[] { "hud_color #FFFFFF", "hud_font Roboto", "hud_icons 1" } },
			{ "troll", new[] { "hud_color #FF00FF", "hud_font \"Comic Sans MS\"", "hud_icons 1" } }
		};
		
		#endregion
		
		private static bool showIcons = true;
		private static string textColor = "white";
		private static string textFont = "Roboto";
		private static bool hudVisible = true;
		private static Vector2 padding = new( 25, 25 );

		#region Commands

		[ClientCmd( "hud_preset" )]
		public static void CmdSetPreset( string presetName )
		{
			if ( Presets.TryGetValue( presetName, out var presetCmds ) )
			{
				Log.Info( $"Applying HUD preset {presetName}" );
				foreach ( var presetCmd in presetCmds )
				{
					if ( presetCmd.StartsWith( "hud_" ) ) // Prevents it from running stuff like "unbindall" or "crash"
						ConsoleSystem.Run( presetCmd );
				}
			}
			else
			{
				Log.Error( $"No such HUD preset {presetName}" );
			}
		}
		
		[ClientVar( "hud_icons" )] 
		public static bool ShowIcons 
		{ 
			get => showIcons;
			set
			{
				if ( Instance == null )
					return;
				
				showIcons = value;

				if ( showIcons )
				{
					Instance.RemoveClass( "icons-disabled" );
				}
				else
				{
					Instance.AddClass( "icons-disabled" );
				}
				
				Log.Info( $"HUD icons are now {(showIcons ? "visible" : "hidden")}" );
				
				Cookie.Set( "ShowIcons", showIcons );
			}
		}
		
		[ClientVar( "hud_font" )]
		public static string TextFont
		{
			get => textFont;
			set
			{
				if ( Instance == null )
					return;
				
				textFont = value;

				Instance.Style.FontFamily = textFont;
				Instance.Style.Dirty();
				
				Log.Info( $"Set HUD font to {textFont}" );
				
				Cookie.Set( "TextFont", textFont );
			}
		}

		[ClientVar( "hud_color" )]
		public static string TextColor
		{
			get => textColor;
			set
			{
				if ( Instance == null )
					return;
				
				textColor = value;

				Instance.Style.FontColor = Color.Parse( textColor );
				Instance.Style.Dirty();
				
				Log.Info( $"Set HUD text color to {textColor}" );
				
				Cookie.Set( "TextColor", textColor );
			}
		}


		[ClientVar( "hud_visible" )]
		public static bool HudVisible
		{
			get => hudVisible;
			set
			{
				if ( Instance == null )
					return;
				
				hudVisible = value;
				if ( hudVisible )
				{
					Instance.RemoveClass( "hidden" );
				}
				else
				{
					Instance.AddClass( "hidden" );
				}
				
				Log.Info( $"The HUD is now {(hudVisible ? "visible" : "hidden")}" );
			}
		}
		
		[ClientVar( "hud_padding" )]
		public static Vector2 Padding
		{
			get => padding;
			set
			{
				if ( Instance == null )
					return;

				padding = value;

				Instance.leftPanel.Style.Left = padding.x;
				Instance.leftPanel.Style.Bottom = padding.y;
				
				Instance.rightPanel.Style.Right = padding.x;
				Instance.rightPanel.Style.Bottom = padding.y;
				
				Instance.leftPanel.Style.Dirty();
				Instance.rightPanel.Style.Dirty();
			
				Log.Info( $"Set HUD padding to {padding}" );
			}
		}

		[ClientCmd( "hud_toggle" )]
		public static void CmdToggle()
		{
			HudVisible = !HudVisible;
			
			Log.Info( "Toggled HUD visibility" );
		}
		
		[ClientCmd( "hud_remove" )]
		public static void CmdRemoveElement( string name )
		{
			Instance.RemoveItem( name );
		}
		
		#endregion

		private List<HudItem> hudItems = new();
		private Panel leftPanel;
		private Panel rightPanel;
		
		/// <summary>
		/// This will set up loading, singletons, sub-panels, and stylesheets for you! <3
		/// </summary>
		public MercuryHud()
		{
			Instance = this;
			StyleSheet.Load( "/Code/UI/MercuryHud.scss" );
			
			leftPanel = Add.Panel( "left-panel" );
			rightPanel = Add.Panel( "right-panel" );
			
			TextColor = Cookie.Get( "TextColor", textColor );
			TextFont = Cookie.Get( "TextFont", textFont );
			ShowIcons = Cookie.Get( "ShowIcons", showIcons );
			Padding = Cookie.Get( "Padding", padding );

			SetClass( "mercury-panel", true );
		}
		
		/// <summary>
		/// Add an item to the HUD.
		/// </summary>
		/// <param name="name">The identifier for this item. Also shows up when icons are disabled.</param>
		/// <param name="icon">The https://fonts.google.com/icons icon for this item.</param>
		/// <param name="method">The method returning the value for this item.</param>
		public void AddItem( HudAlignment alignment, string name, string icon, Func<string> method )
		{
			if ( hudItems.Any( item => item.ItemName == name ) )
			{
				// Element already exists, let's remove it to prevent collisions
				Log.Warning( $"Tried to add {name} when it already existed on this HUD - deleting old"  );
				RemoveItem( name );
			}
			
			var hudItem = new HudItem( name, icon, method );
			hudItem.Parent = alignment switch
			{
				HudAlignment.Left => leftPanel,
				HudAlignment.Right => rightPanel,
				_ => this
			};

			hudItems.Add( hudItem );
		}

		/// <summary>
		/// Remove an item from the HUD by ID.
		/// </summary>
		public void RemoveItem( string name )
		{
			try
			{
				var foundItem = hudItems.First( item => item.ItemName == name );
				foundItem.Delete();
				hudItems.Remove( foundItem );
			}
			catch ( InvalidOperationException _ )
			{
				// Element didn't exist - let's throw a warning
				Log.Warning( $"Tried to delete {name} when it didn't exist" );
			}
		}
	}
}
