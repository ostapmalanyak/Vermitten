#nullable enable
using System;
using Godot;

namespace Vermitten.Scripts;

public partial class CardManager : Node3D
{
	private const string LeftClickPress = "LeftClick";
	private const int CardCollisionMask = 5;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)	
	{
	}

	public override void _Input(InputEvent @event) {
		if (@event.IsActionPressed(LeftClickPress)) {
			GD.Print("click");
			Node3D? card = MouseHovering();
			if (card is not null) {
				GD.Print($"card clicked - {card.Name}");
			}
		}
		if (@event.IsActionReleased(LeftClickPress)) {
			GD.Print("no click");
		}
	}

	private Card? MouseHovering() {
		var array = GetChildren();
		foreach (var node in array) {
			if (node is not Card card) {
				throw new Exception($"Card Manager contains non-cards({node.Name} - {node})");
			}

			if (card.MouseHovering) {
				return card;
			}
		}

		return null;
	}
}