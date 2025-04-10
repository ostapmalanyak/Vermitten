#nullable enable
using System;
using Godot;

namespace Vermitten.Scripts;

public partial class CardManager : Node3D
{
	private const string LeftClickPress = "LeftClick";
	private const int CardCollisionMask = 5; // unused for now
	
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
			//find which card is being hovered over if any
			
			if (card is not null) {
				GD.Print($"card clicked - {card.Name}");
			}
		}
		if (@event.IsActionReleased(LeftClickPress)) {
			GD.Print("no click");
		}
	}

	private Card? MouseHovering() {
		var children = GetChildren();
		foreach (var child in children) {
			// go through every child in the card manager to check which one the mouse is hovering over
			
			if (child is not Card card) {
				// CardManager should only contain cards as children
				throw new Exception($"Card Manager contains non-cards({child.Name} - {child})");
			}

			if (card.MouseHovering) {
				return card;
			}
		}

		return null;
		// if no card is hovered over return null;
	}
}