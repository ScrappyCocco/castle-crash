using Sandbox;
public sealed class PlayerObjectInfoComponent : Component
{
	[Property, Group( "Prefabs" )] public GameObject PlayerCameraPosition { get; set; }

	protected override void OnStart()
	{
		// this is controlled by someone else
		if ( IsProxy ) return;

		Scene.Camera.WorldPosition = PlayerCameraPosition.WorldPosition;
		Scene.Camera.WorldRotation = PlayerCameraPosition.WorldRotation;

		// Save position
		FreeCameraComponent CameraComp = Scene.Camera.GameObject.GetComponent<FreeCameraComponent>();
		CameraComp.SavePlayerPosition(PlayerCameraPosition.WorldPosition, PlayerCameraPosition.WorldRotation);
	}
}