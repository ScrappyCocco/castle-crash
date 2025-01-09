using Sandbox;
using Sandbox.Diagnostics;

public sealed class FractureTest : Component
{
	[Property] public bool bManualBreak { get; set; } = true;
	[Property] public float TimerBreak { get; set; } = 5;
	// Use default Prop destruction
	[Property] public bool bUseStandardDestruction { get; set; } = true;
	bool bBroken = false;
	float Timer = 0;

	protected override void OnUpdate()
	{
		if (bManualBreak && Input.Pressed("Flashlight"))
		{
			ExecuteDestruction();
		}
		else if (!bManualBreak)
		{
			Timer += Time.Delta;
			if (Timer >= TimerBreak)
			{
				ExecuteDestruction();
			}
		}
	}

	public void ExecuteDestruction()
	{
		CastleCustomPropComponent PropComponent = GameObject.GetComponent<CastleCustomPropComponent>();
		if (PropComponent is not null && !bUseStandardDestruction)
		{
			ExecuteCustomPropDestruction();
		}
		else
		{
			ExecuteBasePropDestruction();
		}
	}

	void ExecuteBasePropDestruction()
	{
		if (!bBroken)
		{
			bBroken = true;

			CastleCustomPropComponent PropComponent = GameObject.GetComponent<CastleCustomPropComponent>();
			if (PropComponent is not null)
			{
				PropComponent.bLocalForceDefaultPropKill = true;
				PropComponent.Kill();
			}
		}
	}

	void ExecuteCustomPropDestruction()
	{
		if (!bBroken)
		{
			bBroken = true;

			CastleCustomPropComponent PropComponent = GameObject.GetComponent<CastleCustomPropComponent>();
			if (PropComponent is not null)
			{
				PropComponent.bLocalForceDefaultPropKill = false;
				PropComponent.Kill();
			}
		}
	}
}
