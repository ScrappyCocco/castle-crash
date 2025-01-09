using CastleCrash;
using Sandbox;

public sealed class ResultManagerComponent : Component
{
	[Property] public float UpdateFrequency { get; set; } = 5;
	float Timer = 0;

	protected override void OnUpdate()
	{
		if ( Networking.IsHost && CastleCrash.GameManager.Instance.GameState == GameState.Playing )
		{
			Timer += Time.Delta;
			if ( Timer >= UpdateFrequency )
			{
				Timer = 0;

				var Result = Scene.GetAllComponents<CastleDestructibleComponent>().ToList();
				if ( Result.Count == 0 )
				{
					CastleCrash.GameManager.Instance.FinishGame();
				}
			}
		}
	}
}