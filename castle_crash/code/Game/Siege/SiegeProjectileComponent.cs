using Sandbox;
using Sandbox.Diagnostics;

public sealed class SiegeProjectileComponent : Component, Component.ICollisionListener
{
	[Property] public int MassOverride { get; set; } = 0;
	[Property] public int MinHitSpeedToBreak { get; set; } = 50;
	[Property] public int LifetimeAfterSleep { get; set; }
	[Property] public int LifetimeTimer { get; set; } = 10;
	[Property] public bool bBreakOnImpact { get; set; } = true;
	float Timer = 0;

	Rigidbody RigidbodyComponent = null;
	CastleCustomPropComponent PropComponent = null;

	protected override void OnAwake()
	{
		// Rigidbody
		RigidbodyComponent = GameObject.GetComponent<Rigidbody>();
		Assert.NotNull( RigidbodyComponent );
		RigidbodyComponent.MassOverride = MassOverride;

		// Prop
		PropComponent = GameObject.GetComponent<CastleCustomPropComponent>();
		Assert.NotNull( PropComponent );
	}

	public void OnCollisionStart( Collision collision )
	{
		// this is controlled by someone else
		if ( IsProxy ) return;

		if ( collision.Other.GameObject != null && !collision.Other.GameObject.Tags.Has( "debris" ) )
		{
			Log.Info( $"[Projectile]{GameObject} hitted {collision.Other.GameObject} with a speed of {RigidbodyComponent.Velocity.Length}" );
			if ( collision.Other.GameObject.Tags.Has( "destructible" ) )
			{
				if ( RigidbodyComponent.Velocity.Length >= MinHitSpeedToBreak )
				{
					CastleDestructibleComponent CastleDestructiblePiece = collision.Other.GameObject.GetComponent<CastleDestructibleComponent>();
					if ( CastleDestructiblePiece is not null )
					{
						CastleDestructiblePiece.Break();
					}
				}
			}
			else if ( collision.Other.GameObject.Tags.Has( "destructible_prop" ) )
			{
				// Prop doesn't require minimum speed
				CastleDestructibleComponent CastleDestructiblePiece = collision.Other.GameObject.GetComponent<CastleDestructibleComponent>();
				if ( CastleDestructiblePiece is not null )
				{
					CastleDestructiblePiece.Break();
				}
			}

			if ( bBreakOnImpact && !collision.Other.GameObject.Tags.Has( "destructible_prop" ) )
			{
				// On the first collision on non-debris and not on a prop kill the projectile
				PropComponent.Kill();
			}
		}
	}
	public void OnCollisionStop( CollisionStop collision )
	{
		// Nothing for now
	}

	protected override void OnUpdate()
	{
		// this is controlled by someone else
		if ( IsProxy ) return;

		if ( RigidbodyComponent is not null )
		{
			// This is too slow and too many rigidbodies stay alive
			/*if ( RigidbodyComponent.Sleeping || RigidbodyComponent.AngularVelocity.IsNearZeroLength )
			{
				Timer += Time.Delta;
				if ( Timer >= LifetimeAfterSleep )
				{
					GameObject.Destroy();
				}
			}
			else
			{
				Timer = 0;
			}*/
			Timer += Time.Delta;
			if ( Timer >= LifetimeTimer )
			{
				GameObject.Destroy();
			}
		}
	}
}