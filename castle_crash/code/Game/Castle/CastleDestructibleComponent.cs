using Sandbox;

public abstract class CastleDestructibleComponent : Component
{
	[Rpc.Broadcast]
	public abstract void Break();
}