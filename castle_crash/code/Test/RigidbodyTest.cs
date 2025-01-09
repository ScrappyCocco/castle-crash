using Sandbox.ActionGraphs;

public sealed class RigidbodyTest : Component, Component.ICollisionListener
{
	public void OnCollisionStart( Collision collision )
	{
		Log.Info( $"{collision.Self} Started Touching {collision.Other}" );
		FractureTest Component = collision.Other.GameObject.GetComponent<FractureTest>();
		if ( Component != null )
		{
			Component.ExecuteDestruction();
		}
	}
	public void OnCollisionStop( CollisionStop collision )
	{
		Log.Info( $"{collision.Self} Stopped Touching {collision.Other}" );
	}
}