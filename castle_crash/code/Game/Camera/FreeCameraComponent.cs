using Sandbox;

public sealed class FreeCameraComponent : Component
{
	[Property, Group( "Camera" )] public float FlySpeed { get; set; } = 10;
	float SpeedMultiplier = 150;
	bool bEnabled = false;
	Vector3 SavedPlayerPosition { get; set; }
	Rotation SavedPlayerRotation { get; set; }

	public void SavePlayerPosition( Vector3 InPosition, Rotation InRotation )
	{
		SavedPlayerPosition = InPosition;
		SavedPlayerRotation = InRotation;
	}

	protected override void OnUpdate()
	{
		if ( CastleCrash.GameManager.Instance.GameState == GameState.Playing )
		{
			if ( Input.Pressed( "Flashlight" ) )
			{
				bEnabled = !bEnabled;
			}
			if ( Input.Pressed( "Reload" ) )
			{
				bEnabled = false;
				Scene.Camera.WorldPosition = SavedPlayerPosition;
				Scene.Camera.WorldRotation = SavedPlayerRotation;
			}
			if ( bEnabled )
			{
				if ( Input.Down( "Forward" ) )
				{
					WorldPosition += Vector3.Forward * FlySpeed * SpeedMultiplier * Time.Delta;
				}
				if ( Input.Down( "Backward" ) )
				{
					WorldPosition += -Vector3.Forward * FlySpeed * SpeedMultiplier * Time.Delta;
				}
				if ( Input.Down( "Left" ) )
				{
					WorldPosition += -Vector3.Right * FlySpeed * SpeedMultiplier * Time.Delta;
				}
				if ( Input.Down( "Right" ) )
				{
					WorldPosition += Vector3.Right * FlySpeed * SpeedMultiplier * Time.Delta;
				}
				if ( Input.Down( "Jump" ) )
				{
					WorldPosition += Vector3.Up * FlySpeed * SpeedMultiplier * Time.Delta;
				}
				if ( Input.Down( "Duck" ) )
				{
					WorldPosition += -Vector3.Up * FlySpeed * SpeedMultiplier * Time.Delta;
				}
			}
		}
	}
}