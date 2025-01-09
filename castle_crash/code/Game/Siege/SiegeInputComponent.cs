using Sandbox;

namespace CastleCrash;

public sealed class SiegeInputComponent : Component
{

	public static SiegeInputComponent Instance { get; private set; }

	[Property] public Angles Rotation_Left_Speed { get; set; }
	[Property] public Angles Rotation_Right_Speed { get; set; }

	public SiegeCellComponent HighlightedCell { get; set; } = null;
	public SiegeCellComponent SelectedCell { get; set; } = null;

	bool bIsSiegeTurning = false;
	Angles SiegeTurningAngle;

	protected override void OnAwake()
	{
		Instance = this;
		Enabled = false;
	}

	protected override void OnDisabled()
	{
		HighlightedCell?.Deselect();
		SelectedCell?.Deselect();
	}

	protected override void OnUpdate()
	{
		var tr = Scene.Trace.Ray( Scene.Camera.ScreenPixelToRay( Mouse.Position ), 8000f )
			.WithAnyTags( "siege_cell" )
			.Run();

		if ( tr.Hit && tr.GameObject.Network.IsOwner )
		{
			var SiegeCell = tr.GameObject.GetComponent<SiegeCellComponent>();
			if ( SiegeCell != HighlightedCell )
			{
				HighlightedCell?.MouseExit();
				HighlightedCell = SiegeCell;
				HighlightedCell?.MouseEnter();
			}
		}
		else
		{
			HighlightedCell?.MouseExit();
			HighlightedCell = null;
		}

		// Select
		if ( Input.Pressed( "Select" ) && HighlightedCell is not null && HighlightedCell != SelectedCell )
		{
			SelectedCell?.Deselect();
			SelectedCell = null;

			SelectedCell = HighlightedCell;
			SelectedCell?.Select();

			SoundsManagerComponent.Instance.UISelect();

			if ( SelectedCell.IsOccupied )
			{
				GameManager.Instance.PlayerState = PlayerState.Editing;
			}
			else
			{
				GameManager.Instance.PlayerState = PlayerState.Placing;
			}
		}

		// DeSelect
		if ( SelectedCell is not null && Input.Pressed( "DeSelect" ) )
		{
			RequestCancelAction();
		}

		// Rotation
		if ( bIsSiegeTurning )
		{
			SelectedCell.ChangeSiegeRotation( SiegeTurningAngle );
		}
	}

	public void RequestCatapultSpawn()
	{
		if ( SelectedCell is not null )
		{
			GameManager.Instance.CreateCatapult( SelectedCell );
		}
	}

	public void RequestTrebuchetSpawn()
	{
		GameManager.Instance.CreateTrebuchet( SelectedCell );
	}

	public void RequestSiegeRemove()
	{
		if ( SelectedCell is not null )
		{
			GameManager.Instance.RemoveSiege( SelectedCell );

			SelectedCell?.Deselect();
			SelectedCell = null;
		}
	}

	public void RequestSiegeTurnLeft()
	{
		RequestSiegeTurn( true );
	}

	public void RequestSiegeTurnRight()
	{
		RequestSiegeTurn( false );
	}

	public void StopSiegeTurn()
	{
		bIsSiegeTurning = false;
	}

	void RequestSiegeTurn( bool bIsLeft )
	{
		if ( SelectedCell is not null )
		{
			bIsSiegeTurning = true;
			if ( bIsLeft )
			{
				SiegeTurningAngle = Rotation_Left_Speed;
			}
			else
			{
				SiegeTurningAngle = Rotation_Right_Speed;
			}
		}
	}

	public string RequestCurrentSiegeCurrentPower()
	{
		if ( SelectedCell is not null && SelectedCell.SpawnedSiege is not null )
		{
			return SelectedCell.SpawnedSiegeComponent.FirePower.ToString();
		}
		return "0";
	}

	public string RequestCurrentSiegeFireSpeed()
	{
		if ( SelectedCell is not null && SelectedCell.SpawnedSiege is not null )
		{
			return SelectedCell.SpawnedSiegeComponent.FireSpeed.ToString();
		}
		return "0";
	}

	public void RequestSiegePowerUp()
	{
		RequestSiegePowerChange( 1 );
	}

	public void RequestSiegePowerDown()
	{
		RequestSiegePowerChange( -1 );
	}

	void RequestSiegePowerChange( int Change )
	{
		if ( SelectedCell is not null && SelectedCell.SpawnedSiege is not null )
		{
			SelectedCell.ChangeSiegeFirePower( Change );
		}
	}

	public void RequestCancelAction()
	{
		SelectedCell?.Deselect();
		SelectedCell = null;

		GameManager.Instance.PlayerState = PlayerState.Playing;
	}
}