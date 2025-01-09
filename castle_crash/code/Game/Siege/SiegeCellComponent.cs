using Sandbox;
using Sandbox.Diagnostics;

public sealed class SiegeCellComponent : Component
{
	[Property] ModelRenderer Renderer { get; set; }

	[Property] public GameObject Siege_Catapult_Spawn_Loc { get; set; }
	[Property] public GameObject Siege_Trebuchet_Spawn_Loc { get; set; }
	public GameObject SpawnedSiege { get; private set; } = null;
	public SiegeMachineComponent SpawnedSiegeComponent { get; private set; } = null;

	public bool IsHovering { get; private set; } = false;
	public bool IsSelected { get; set; } = false;

	[Sync] public bool IsOccupied { get; set; } = false;

	Color BaseColor => Color.Gray;

	protected override void OnStart()
	{
		UpdateHighlight();
	}

	public void MouseEnter()
	{
		IsHovering = true;

		UpdateHighlight();
	}

	public void MouseExit()
	{
		IsHovering = false;

		UpdateHighlight();
	}

	public void Select()
	{
		IsSelected = true;

		UpdateHighlight();
	}

	public void Deselect()
	{
		IsSelected = false;

		UpdateHighlight();
	}

	void UpdateHighlight()
	{
		if ( IsSelected )
		{
			var color = Color.Yellow;
			Renderer.Tint = Color.Lerp( BaseColor, color, 0.8f );
		}
		else if ( IsHovering )
		{
			//Renderer.Tint = IsOccupied ? Color.Lerp( BaseColor, Color.Red, 0.95f ) : Color.Lerp( BaseColor, Color.Green, 0.95f );
			Renderer.Tint = Color.Lerp( BaseColor, Color.Green, 0.95f );
		}
		else
		{
			Renderer.Tint = BaseColor;
		}
	}

	public void UpdateSpawnedSiege( GameObject NewSiege )
	{
		if ( NewSiege is not null )
		{
			SiegeMachineComponent FoundSiegeComponent = NewSiege.GetComponent<SiegeMachineComponent>();
			Assert.NotNull( FoundSiegeComponent );

			if ( FoundSiegeComponent is not null )
			{
				SpawnedSiegeComponent = FoundSiegeComponent;
				SpawnedSiege = NewSiege;
			}
		}
		else
		{
			SpawnedSiege = null;
			SpawnedSiegeComponent = null;
		}
	}

	public void ChangeSiegeRotation( Angles Rotation )
	{
		if ( SpawnedSiege is not null )
		{
			SpawnedSiege.WorldRotation *= Rotation * Time.Delta;
		}
	}

	public void ChangeSiegeFirePower( int PowerChange )
	{
		if ( SpawnedSiegeComponent is not null )
		{
			SpawnedSiegeComponent.UpdateFirePower( PowerChange );
		}
	}
}