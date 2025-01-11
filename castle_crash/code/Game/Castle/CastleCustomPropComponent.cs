using Sandbox;

/*
This is a custom override of the Prop component to manually manage the created Gibs
Because the problem is that unless you use the External Break Piece that require a lot of time importing and setting up the pieces
You can't change the created Gibs material, so with the Castle models the inner part of the material or some pieces had the material all broken
You can try by changing GameManager.bUseCustomPropKillLogic
This is a custom logic to manually change the created Gib material with a custom one, in this case a base simple color
It's also useful to manually change some Gib data for all objects
If we get the base Prop component and call Kill() we still execute the base logic as it's not virtual
*/
public sealed class CastleCustomPropComponent : Prop
{
	[Property, Model.MaterialGroup, Group( "CustomPropData" )] public string BrokenPiecesMaterialGroup { get; set; }
	[Property, Group( "CustomPropData" )] public bool bLocalForceDefaultPropKill { get; set; } = false;
	[Property, Group( "CustomPropData" )] public string SurfaceNameForSoundToPlay { get; set; } = "default";

	public new void Kill()
	{
		if ( bLocalForceDefaultPropKill )
		{
			Log.Info( $"[CustomProp]{GameObject} executing base.Kill()" );
			base.Kill();
		}
		else
		{
			Log.Info( $"[CustomProp]{GameObject} executing custom Kill()" );
			OnBreak();
			GameObject.Destroy();
		}
	}

	void OnBreak()
	{
		OnPropBreak?.Invoke();

		PlayBreakSound();

		CreateGibs();
	}

	public new List<Gib> CreateGibs()
	{
		var CreatedGibs = base.CreateGibs();

		// Edit the Material
		foreach ( Gib CreatedGib in CreatedGibs )
		{
			// Change the Material
			CreatedGib.MaterialGroup = BrokenPiecesMaterialGroup;
			// Change the Mass
			if ( CastleCrash.GameManager.Instance is not null && CastleCrash.GameManager.Instance.CustomPropGibMassOverride > 0 )
			{
				var GibRigidbody = CreatedGib.Components.Get<Rigidbody>();
				if ( GibRigidbody is not null )
				{
					GibRigidbody.MassOverride = CastleCrash.GameManager.Instance.CustomPropGibMassOverride;
				}
				else
				{
					Log.Warning( $"[Gib]'{CreatedGib}' of index '{CreatedGibs.IndexOf( CreatedGib )}' has an invalid Rigidbody? How?" );
				}
			}
		}

		return CreatedGibs;
	}

	private void PlayBreakSound()
	{
		// Simple implementation
		var FoundSurface = Surface.FindByName( SurfaceNameForSoundToPlay );
		if ( !FoundSurface.IsValid() )
		{
			return;
		}

		var sound = FoundSurface.Breakables.BreakSound;
		if ( string.IsNullOrWhiteSpace( sound ) )
		{
			return;
		}

		Sound.Play( sound, WorldPosition );
	}
}