# Template

This is a simple s&box template that contains stuff I often re-use between projects.

## Features

Here's what it contains:

### HUD

The HUD system within this template is incredibly powerful. 
You can customize its look & feel, and add things pretty easily.

Here's what a basic HUD setup would look like:

```csharp
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

  //
  // Ammo
  //
  mercury.AddItem( MercuryHud.HudAlignment.Right,
    "ammo",
    "",
    () => $"{Local.Pawn.Inventory.Active.Clip}" );

  mercury.AddItem( MercuryHud.HudAlignment.Right,
    "reserve",
    "/",
    () => $"{Local.Pawn.Inventory.Active.Reserve}" );
}
```

This example would set up a HUD with the health in the bottom left and ammo in the bottom right. These values will update appropriately.

![HUD preview](https://cdn.discordapp.com/attachments/839155256964284459/854665686436806666/unknown.png)

In order to customize the default HUD layout, you can change five lines [in MercuryHud.cs](https://github.com/xezno/sbox-template/blob/master/code/UI/MercuryHud.cs#L40-L44). These properties are pretty self-explanatory.

```csharp
		private static bool showIcons = true;
		private static string textColor = "white";
		private static string textFont = "Roboto";
		private static bool hudVisible = true;
		private static Vector2 padding = new( 25, 25 );
```

Note that users can use the following commands to override these at any time:
- `hud_icons`
- `hud_font`
- `hud_color`
- `hud_padding`
- `hud_visible`

Additionally, various [presets are available](https://github.com/xezno/sbox-template/blob/master/code/UI/MercuryHud.cs#L30-L36). Feel free to change these to fit your gamemode's needs. Users can switch through these using the `hud_preset` command.

### Textures & Materials

Various prototype materials are available by default:

![Prototype textures](https://media.discordapp.net/attachments/839155256964284459/854666383873933362/unknown.png)

These all use one tint mask, so adding a new texture is as simple as copying an existing one and changing its tint color. Additionally, changing the contents of all materials is as simple as editing the tint mask.


### Swimming

Movement has been slightly modified to remove various unused pieces of code, plus add some enhancements to swimming: a speed increase, being able to properly rise to the surface while in water, and the ability for the player to jump slightly higher when near the edge of the water, enabling them to "climb" onto ledges and out of water.

Also, if the player & camera are both submerged in water, the screen is tinted blue. This is currently done through a simple UI panel covering the entire screen however in future (once the refract shader is working) should be done through a screen-covering triangle or quad.
