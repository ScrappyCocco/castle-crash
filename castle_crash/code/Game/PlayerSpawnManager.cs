using Sandbox;
using CastleCrash;
using Sandbox.Diagnostics;

/*
This works similar to NetworkHelper
But it picks the SpawnPoints based on the player index in Connection List instead of Randomly picking it
*/
public sealed class PlayerSpawnManagerComponent : Component, Component.INetworkListener
{
	[Property, Group( "Prefabs" )] public GameObject PlayerCellsPrefab { get; set; }
	[Property] public GameObject[] SpawnPoints { get; set; }

	// This is called on the host!
	public void OnConnected( Connection channel )
	{
		Log.Info( $"Player '{channel.DisplayName}' connected" );
	}

	// This is called on the host!
	public void OnDisconnected( Connection channel )
	{
		Log.Info( $"Player '{channel.DisplayName}' disconnected" );
	}

	// This is called on the host!
	public void OnActive( Connection channel )
	{
		Log.Info( $"Player '{channel.DisplayName}' has joined the game" );

		SetupConnectionPermissions( channel );

		var ConnectionList = Connection.All.ToList();
		int SpawnIndexToUse = ConnectionList.IndexOf( channel );

		Assert.True( SpawnIndexToUse <= SpawnPoints.Length );

		Log.Info( $"Player '{channel.DisplayName}' is spawning at index {SpawnIndexToUse}-{SpawnPoints[SpawnIndexToUse]}" );

		var NewPlayerCells = PlayerCellsPrefab.Clone( SpawnPoints[SpawnIndexToUse].WorldTransform, name: $"Player - {channel.DisplayName}" );
		NewPlayerCells.BreakFromPrefab();
		NewPlayerCells.NetworkSpawn( channel );
	}

	void SetupConnectionPermissions( Connection channel )
	{
		channel.CanSpawnObjects = true;
		channel.CanRefreshObjects = true;
	}
}