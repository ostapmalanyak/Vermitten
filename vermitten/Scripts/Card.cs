using System;
using Godot;

namespace Vermitten.Scripts;

public partial class Card : Node3D
{
	protected const float ShakeRotation = 15f;
	protected const float ShakeSpeed = 10f;
	
	public bool MouseHovering = false;
	protected float Rotated = 0;
	protected float Mult = 1;
	
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (MouseHovering) {
			if (Rotated < ShakeRotation) {
				Rotate(new Vector3(0, 1, 0),  ToRadians(1)*Mult);
				Rotated += 1;
			}
			else {
				Rotated *= Mult;
				Mult *= -1;
			}
		}
	}

	public void StartShake() {
		MouseHovering = true;
	}

	public void StopShaking() {
		MouseHovering = false;
	}

	private float ToRadians(float degrees) {
		return (float) (degrees / 180 * Math.PI);
	}
}