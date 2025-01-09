using Sandbox;

// Fancy Stats we could display in the future
public sealed class StatsManagerComponent : Component
{
	public static StatsManagerComponent Instance { get; private set; }

	protected override void OnAwake()
	{
		Instance = this;
	}

	public void CastlePieceBroken()
	{
		Sandbox.Services.Stats.Increment( "castle_piece_broken", 1 );
	}

	public void CatapultBuilt()
	{
		Sandbox.Services.Stats.Increment( "siege_catapult_built", 1 );
	}

	public void TrebuchetBuilt()
	{
		Sandbox.Services.Stats.Increment( "siege_trebuchet_built", 1 );
	}

	public void ProjectileShot()
	{
		Sandbox.Services.Stats.Increment( "siege_projectile_shot", 1 );
	}
}