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
			if ( Input.Pressed( "FreeCameraEnable" ) )
			{
				bEnabled = !bEnabled;
			}
			if ( Input.Pressed( "FreeCameraReset" ) )
			{
				bEnabled = false;
				Scene.Camera.WorldPosition = SavedPlayerPosition;
				Scene.Camera.WorldRotation = SavedPlayerRotation;
			}
			if ( bEnabled )
			{
				if ( Input.Down( "FreeCameraForward" ) )
				{
					WorldPosition += Vector3.Forward * FlySpeed * SpeedMultiplier * Time.Delta;
				}
				if ( Input.Down( "FreeCameraBackward" ) )
				{
					WorldPosition += -Vector3.Forward * FlySpeed * SpeedMultiplier * Time.Delta;
				}
				if ( Input.Down( "FreeCameraLeft" ) )
				{
					WorldPosition += -Vector3.Right * FlySpeed * SpeedMultiplier * Time.Delta;
				}
				if ( Input.Down( "FreeCameraRight" ) )
				{
					WorldPosition += Vector3.Right * FlySpeed * SpeedMultiplier * Time.Delta;
				}
				if ( Input.Down( "FreeCameraUP" ) )
				{
					WorldPosition += Vector3.Up * FlySpeed * SpeedMultiplier * Time.Delta;
				}
				if ( Input.Down( "FreeCameraDown" ) )
				{
					WorldPosition += -Vector3.Up * FlySpeed * SpeedMultiplier * Time.Delta;
				}
			}
		}
	}
}