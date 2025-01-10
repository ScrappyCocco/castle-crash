using Sandbox;

public abstract class CastleDestructibleComponent : Component
{
	/*
	There might be a problem with this being an RPC
	It's an RPC so every connected client break the object and do the physics logic
	If it's a Prop every client create its own Gibs
	The problem with this is that theoretically if the Host execute Kill() faster than a client
	It could Destroy the GameObject BEFORE a client receive and execute the RPC on it
	Maybe the Castle Pieces should not be Networked but just Snapshotted on Connection(?)
	*/
	[Rpc.Broadcast]
	public abstract void Break();
}