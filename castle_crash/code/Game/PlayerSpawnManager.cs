using Sandbox;
using CastleCrash;
using Sandbox.Diagnostics;

public sealed class PlayerSpawnManagerComponent : Component, Component.INetworkListener
{
	[Property, Group( "Prefabs" )] public GameObject PlayerCellsPrefab { get; set; }
	[Property] public GameObject[] SpawnPoints { get; set; }

	public void OnActive( Connection channel )
	{
		SetupConnectionPermissions( channel );

		// Is this stable?
		// The better alternative would be to have a stable player-id as 0-1-2-...
		// Otherwise we could do Client -Rpc-> Ask Host Spawn -Rpc-> Spawn there
		var ConnectionList = Connection.All.ToList();
		int SpawnIndexToUse = ConnectionList.IndexOf( channel );

		Assert.True( SpawnIndexToUse <= SpawnPoints.Length );

		var NewPlayerCells = PlayerCellsPrefab.Clone( SpawnPoints[SpawnIndexToUse].WorldTransform, name: $"Player - {channel.DisplayName}" );
		NewPlayerCells.BreakFromPrefab();
		//NewPlayerCells.WorldRotation = SpawnPoints[SpawnIndexToUse].WorldRotation;
		NewPlayerCells.NetworkSpawn( channel );
	}

	void SetupConnectionPermissions( Connection channel )
	{
		channel.CanSpawnObjects = true;
		channel.CanRefreshObjects = true;
	}
}