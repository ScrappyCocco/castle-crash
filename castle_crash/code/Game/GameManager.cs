using System;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.Diagnostics;
using Sandbox.Network;

namespace CastleCrash;

public sealed class GameManager : Component, Component.INetworkListener
{
    public static GameManager Instance { get; private set; }

    [RequireComponent] SiegeInputComponent _siegeInput { get; set; }
    [RequireComponent] PlayerSpawnManagerComponent _playerSpawn { get; set; }
    [RequireComponent] ResultManagerComponent _ResultManager { get; set; }
    [RequireComponent] SoundsManagerComponent _SoundsManager { get; set; }

    [Property, Group( "Prefabs" )] public GameObject CatapultPrefab { get; set; }
    [Property, Group( "Prefabs" )] public GameObject TrebuchetPrefab { get; set; }

    [Sync( SyncFlags.FromHost )] public GameState GameState { get; set; }
    public PlayerState PlayerState { get; set; }
    // Waiting
    [Property] public float OtherPlayersWaitTimer = 10;
    [Sync( SyncFlags.FromHost )] public float WaitingTime { get; set; } = 0;
    // Debug
    [Property] public bool bDrawChainCollisionRaycast { get; set; } = true;
    [Property] public bool bUseCustomPropKillLogic { get; set; } = true;
    [Property] public float CustomPropGibMassOverride { get; set; } = 10;

    protected override void OnAwake()
    {
        Instance = this;
    }

    protected override async Task OnLoad()
    {
        if ( Scene.IsEditor ) return;
        if ( Networking.IsActive ) return;

        LoadingScreen.Title = "Creating Lobby";
        await Task.DelayRealtimeSeconds( 0.1f );

        LobbyConfig config = new LobbyConfig();
        config.MaxPlayers = 4;
        config.Privacy = LobbyPrivacy.Public;
        Networking.CreateLobby( config );
    }

    protected override void OnUpdate()
    {
        switch ( GameState )
        {
            case GameState.Waiting: UpdateWaiting(); break;
            case GameState.Playing: break; // Check done in the Result Manager
            case GameState.Results: break; // Nothing else from here
        }

        SiegeInputComponent.Instance.Enabled = GameState == GameState.Playing;
    }

    public void UpdateWaiting()
    {
        // Nothing to do here
        if ( Networking.IsHost )
        {
            WaitingTime += Time.Delta;
            if ( WaitingTime >= OtherPlayersWaitTimer )
            {
                StartGame();
            }
        }
    }

    public void StartGame()
    {
        if ( Networking.IsHost )
        {
            Assert.True( GameState == GameState.Waiting );
            GameState = GameState.Playing;
        }
    }

    public void FinishGame()
    {
        Assert.True( GameState == GameState.Playing );
        GameState = GameState.Results;
    }

    public void CreateCatapult( SiegeCellComponent CellComponent )
    {
        SpawnSiege( CellComponent, CellComponent.Siege_Catapult_Spawn_Loc, CatapultPrefab );

        StatsManagerComponent.Instance.CatapultBuilt();
    }

    public void CreateTrebuchet( SiegeCellComponent CellComponent )
    {
        SpawnSiege( CellComponent, CellComponent.Siege_Trebuchet_Spawn_Loc, TrebuchetPrefab );

        StatsManagerComponent.Instance.TrebuchetBuilt();
    }

    public void SpawnSiege( SiegeCellComponent CellComponent, GameObject SpawnPositionObject, GameObject Prefab )
    {
        var NewSpawnedSiege = Prefab.Clone( SpawnPositionObject.WorldPosition );
        NewSpawnedSiege.BreakFromPrefab();
        NewSpawnedSiege.WorldRotation = SpawnPositionObject.WorldRotation;
        NewSpawnedSiege.SetParent( CellComponent.GameObject );
        NewSpawnedSiege.NetworkSpawn( CellComponent.Network.Owner );

        CellComponent.IsOccupied = true;
        CellComponent.UpdateSpawnedSiege( NewSpawnedSiege );

        SoundsManagerComponent.Instance.SiegeBuild( NewSpawnedSiege.WorldPosition );

        PlayerState = PlayerState.Editing;
    }

    public void RemoveSiege( SiegeCellComponent CellComponent )
    {
        if ( CellComponent.SpawnedSiege is not null )
        {
            SoundsManagerComponent.Instance.SiegeDestroy( CellComponent.SpawnedSiege.WorldPosition );

            CellComponent.SpawnedSiege.Destroy();
            CellComponent.UpdateSpawnedSiege( null );
            CellComponent.IsOccupied = false;
        }

        PlayerState = PlayerState.Playing;
    }
}