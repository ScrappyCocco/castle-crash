using Sandbox;
using Sandbox.Diagnostics;

public sealed class CastleDestructiblePieceComponent : CastleDestructibleComponent, Component.ICollisionListener
{
	[Property] public bool bCauseChainBreakToUpperPieces { get; set; } = true;
	bool bBroken = false;
	CastleCustomPropComponent CustomPropComponent = null;

	protected override void OnAwake()
	{
		CustomPropComponent = GameObject.GetComponent<CastleCustomPropComponent>();
		Assert.NotNull( CustomPropComponent );
		// Validation check
		Assert.True( CustomPropComponent.IsStatic );
		if ( !CustomPropComponent.bLocalForceDefaultPropKill && string.IsNullOrWhiteSpace( CustomPropComponent.BrokenPiecesMaterialGroup ) )
		{
			Log.Warning( $"[CastlePieceValidation]{GameObject} doesn't have a BrokenPiecesMaterialGroup, you should use bLocalForceDefaultPropKill" );
		}
		else if ( !CustomPropComponent.bLocalForceDefaultPropKill && CustomPropComponent.BrokenPiecesMaterialGroup.Equals( "default" ) )
		{
			Log.Warning( $"[CastlePieceValidation]{GameObject} is using default as BrokenPiecesMaterialGroup, you should use bLocalForceDefaultPropKill" );
		}
	}

	[Rpc.Broadcast]
	public override void Break()
	{
		if ( !bBroken )
		{
			bBroken = true;

			if ( bCauseChainBreakToUpperPieces )
			{
				TryChainCollapse();
			}

			// Stats
			StatsManagerComponent.Instance.CastlePieceBroken();

			if ( CustomPropComponent is not null && CastleCrash.GameManager.Instance.bUseCustomPropKillLogic )
			{
				// Call our custom kill Function
				CustomPropComponent.Kill();
			}
			else
			{
				// Call base Prop Kill Function
				CustomPropComponent.bLocalForceDefaultPropKill = true;
				CustomPropComponent.Kill();
			}
		}
	}

	void TryChainCollapse()
	{
		// Check attached props around
		{
			// Could also use new BBox( x, y )
			Vector3 BoxRayExtent = new Vector3( 150, 150, 100 );
			// Should be shot from the Centre of the Model for better precision
			// Can be calculated with something like (PropComponent.Model.Bounds.Center * GameObject.WorldScale) + RayFrom;
			// But with models having strange/different pivots it was not accurate and it was a little on the side
			Vector3 RayFrom = GameObject.WorldPosition;
			Vector3 RayTo = RayFrom + (GameObject.WorldTransform.Up * 100);
			IEnumerable<SceneTraceResult> HitResults = Scene.Trace.Box( BoxRayExtent, RayFrom, RayTo )
			.WithAnyTags( "destructible_prop" )
			.IgnoreGameObject( GameObject )
			.RunAll();

			// Prop Raycast Rendering
			if ( Game.IsEditor && CastleCrash.GameManager.Instance.bDrawChainCollisionRaycast )
			{
				Color PropDebugRaycast = Color.Green;
				DebugOverlay.Box( RayFrom, BoxRayExtent, PropDebugRaycast, 10, default, true );
				DebugOverlay.Line( RayFrom, RayTo, PropDebugRaycast, 10, default, true );
			}

			foreach ( SceneTraceResult Res in HitResults )
			{
				if ( Res.Hit )
				{
					Log.Info( $"[ChainCollapse]{GameObject} hitted prop {Res.GameObject} and may cause chain destruction" );
					var CastlePiece = Res.GameObject.GetComponent<CastleDestructibleComponent>();
					if ( CastlePiece is not null && CastlePiece != this ) // Should not happen
					{
						CastlePiece.Break();
					}
				}
			}
		}
		// Fire a Ray UP
		{
			Vector3 BoxRayExtent = new Vector3( 3, 3, 5 ); // Could also use new BBox( x, y )
			Vector3 RayFrom = GameObject.WorldPosition + (GameObject.WorldTransform.Up * 2);
			Vector3 RayTo = RayFrom + (GameObject.WorldTransform.Up * 500);
			IEnumerable<SceneTraceResult> HitResults = Scene.Trace.Box( BoxRayExtent, RayFrom, RayTo )
			.WithAnyTags( "destructible", "destructible_prop" )
			.IgnoreGameObject( GameObject )
			.RunAll();

			// Chain Destruction Raycast Rendering
			if ( Game.IsEditor && CastleCrash.GameManager.Instance.bDrawChainCollisionRaycast )
			{
				Color PropDebugRaycast = Color.Yellow;
				DebugOverlay.Box( RayFrom, BoxRayExtent, PropDebugRaycast, 10, default, true );
				DebugOverlay.Line( RayFrom, RayTo, PropDebugRaycast, 10, default, true );
			}

			foreach ( SceneTraceResult Res in HitResults )
			{
				if ( Res.Hit )
				{
					Log.Info( $"[ChainCollapse]{GameObject} hitted {Res.GameObject} and may cause chain destruction" );
					if ( Res.GameObject.WorldPosition.z > GameObject.WorldPosition.z )
					{
						var CastlePiece = Res.GameObject.GetComponent<CastleDestructibleComponent>();
						if ( CastlePiece is not null && CastlePiece != this ) // Should not happen
						{
							CastlePiece.Break();
						}
					}
				}
			}
		}
	}
}