using Sandbox;
using CastleCrash;
using Sandbox.Diagnostics;
using System;

/*
This works similar to NetworkHelper
But it picks the SpawnPoints based on the player index in Connection List instead of Randomly picking it
*/
public sealed class PlayerSpawnManagerComponent : Component, Component.INetworkListener
{
	[Property, Group( "Prefabs" )] public GameObject PlayerCellsPrefab { get; set; }
	[Property] public GameObject[] SpawnPoints { get; set; }

	Connection[] ConnectionToSpawnIndexArray;

	protected override void OnAwake()
	{
		ConnectionToSpawnIndexArray = new Connection[SpawnPoints.Length];
	}

	// This is called on the host!
	public void OnConnected( Connection channel )
	{
		Log.Info( $"[PlayerManager]Player '{channel.DisplayName}' connected" );
	}

	// This is called on the host!
	public void OnDisconnected( Connection channel )
	{
		Log.Info( $"[PlayerManager]Player '{channel.DisplayName}' disconnected" );

		int FoundIndex = Array.IndexOf( ConnectionToSpawnIndexArray, channel );
		if ( FoundIndex >= 0 && FoundIndex < ConnectionToSpawnIndexArray.Length )
		{
			ConnectionToSpawnIndexArray[FoundIndex] = null;
			Log.Info( $"[PlayerManager]Player '{channel.DisplayName}' Spawn Index {FoundIndex} removed" );
		}
		else
		{
			Log.Error( $"[PlayerManager]Unable to remove Player '{channel.DisplayName}' entry from the array, Index:{FoundIndex}" );
		}
	}

	// This is called on the host!
	public void OnActive( Connection channel )
	{
		Log.Info( $"[PlayerManager]Player '{channel.DisplayName}' has joined the game" );

		SetupConnectionPermissions( channel );

		// Find the first free slot (null)
		int SpawnIndexToUse = Array.IndexOf( ConnectionToSpawnIndexArray, null );
		Assert.True( SpawnIndexToUse != -1 );
		Assert.True( SpawnIndexToUse <= SpawnPoints.Length );
		ConnectionToSpawnIndexArray[SpawnIndexToUse] = channel;

		Log.Info( $"[PlayerManager]Player '{channel.DisplayName}' is spawning at index {SpawnIndexToUse}-{SpawnPoints[SpawnIndexToUse]}" );

		var NewPlayerCells = PlayerCellsPrefab.Clone( SpawnPoints[SpawnIndexToUse].WorldTransform, name: $"Player - {channel.DisplayName}" );
		NewPlayerCells.BreakFromPrefab();
		NewPlayerCells.NetworkSpawn( channel );
	}

	void SetupConnectionPermissions( Connection channel )
	{
		channel.CanSpawnObjects = true;
		channel.CanRefreshObjects = true; // Could not be needed
	}
}