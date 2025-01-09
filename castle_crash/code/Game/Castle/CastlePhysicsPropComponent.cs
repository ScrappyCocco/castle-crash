using Sandbox;
using Sandbox.Diagnostics;

public sealed class CastlePhysicsPropComponent : CastleDestructibleComponent
{
	[Property] public int LifetimeBeforeDestroy { get; set; } = 10;
	[Property] public Collider ColliderComponent { get; set; }
	[Property] public Rigidbody RigidbodyComponent { get; set; }
	float Timer = 0;
	bool bBroken = false;

	[Rpc.Broadcast]
	public override void Break()
	{
		if ( !bBroken )
		{
			bBroken = true;

			ColliderComponent.Enabled = true;
			ColliderComponent.Static = false;

			RigidbodyComponent.Enabled = true;
		}

	}

	protected override void OnUpdate()
	{
		if ( bBroken )
		{
			Timer += Time.Delta;
			if ( Timer >= LifetimeBeforeDestroy )
			{
				GameObject.Destroy();
			}
		}
	}
}