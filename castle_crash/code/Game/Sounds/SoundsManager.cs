using Sandbox;

/*
Small Sound Manager to centralize the Sound.Play calls
*/
public sealed class SoundsManagerComponent : Component
{
	public static SoundsManagerComponent Instance { get; private set; }

	protected override void OnAwake()
	{
		Instance = this;
	}

	// 2D Sounds

	public void UISelect()
	{
		Sound.Play( "ui-select" );
	}

	// 3D RPC Sounds for everyone

	[Rpc.Broadcast]
	public void SiegeBuild( Vector3 Position )
	{
		Sound.Play( "siege-build", Position );
	}

	[Rpc.Broadcast]
	public void SiegeDestroy( Vector3 Position )
	{
		Sound.Play( "siege-destroy", Position );
	}

	[Rpc.Broadcast]
	public void SiegeFire( Vector3 Position )
	{
		Sound.Play( "siege-fire", Position );
	}

	[Rpc.Broadcast]
	public void ProjectileImpact( Vector3 Position )
	{
		Sound.Play( "projectile-impact", Position );
	}

}