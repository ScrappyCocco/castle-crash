using Sandbox;
using Sandbox.Diagnostics;

public sealed class SiegeMachineComponent : Component
{
	[Property, Group( "Prefabs" )] public GameObject ProjectilePrefab { get; set; }

	[Property] public GameObject Fire_Spawn_Loc { get; set; }

	[Property] public int FirePower { get; set; }
	[Property] public int MinFirePower { get; set; }
	[Property] public int MaxFirePower { get; set; }
	[Property] public int FireSpeed { get; set; }

	float Timer = 0;

	protected override void OnUpdate()
	{
		// this is controlled by someone else
		if ( IsProxy ) return;

		Timer += Time.Delta;
		if ( Timer >= FireSpeed )
		{
			Timer = 0;

			Fire();
		}
	}

	public void UpdateFirePower( int PowerChange )
	{
		FirePower = MathX.Clamp( FirePower + PowerChange, MinFirePower, MaxFirePower ).CeilToInt();
	}

	void Fire()
	{
		var NewProjectile = ProjectilePrefab.Clone( Fire_Spawn_Loc.WorldPosition );
		NewProjectile.BreakFromPrefab();
		NewProjectile.WorldRotation = Fire_Spawn_Loc.WorldRotation;
		// Set the cell as parent
		// Careful! Not the siege or it would rotate with it left/right
		NewProjectile.SetParent( GameObject.Parent );
		NewProjectile.NetworkSpawn( Network.Owner );

		Rigidbody RigidbodyComp = NewProjectile.GetComponent<Rigidbody>();
		SiegeProjectileComponent SiegeProjComp = NewProjectile.GetComponent<SiegeProjectileComponent>();
		Assert.NotNull( RigidbodyComp );
		Assert.NotNull( SiegeProjComp );

		// Fire
		RigidbodyComp.ApplyForce( Fire_Spawn_Loc.WorldTransform.Forward * FirePower * RigidbodyComp.Mass * 10000 );

		// Sound
		SoundsManagerComponent.Instance.SiegeFire( Fire_Spawn_Loc.WorldPosition );

		// Stats
		StatsManagerComponent.Instance.ProjectileShot();
	}
}