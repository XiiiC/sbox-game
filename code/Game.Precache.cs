using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace Template
{
	public partial class Game
	{
		private class PrecacheFile
		{
			public List<string> precache;
		}

		private void LoadPrecacheFile( string precacheFilePath )
		{
			Log.Info( $"Loading precache file {precacheFilePath}" );
			var precacheFile = FileSystem.Mounted.ReadJson<PrecacheFile>( precacheFilePath );
			
			foreach ( var precacheEntry in precacheFile.precache )
			{
				Log.Trace( $"Adding precache entry {precacheEntry}" );
				Precache.Add( precacheEntry );
			}
			
			Log.Info( "Loaded precache file." );
		}
	}
}
