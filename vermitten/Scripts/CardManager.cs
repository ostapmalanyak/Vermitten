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
			Node3D? card = RaycastForCards();
			if (card is not null) {
				GD.Print($"card clicked - {card.Name}");
			}
		}
		if (@event.IsActionReleased(LeftClickPress)) {
			GD.Print("no click");
		}
	}

	private Node3D? RaycastForCards() {
		PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;

		var rayParameters = new PhysicsRayQueryParameters3D();
		Camera3D globalCamera = GetViewport().GetCamera3D();
		
		rayParameters.From = globalCamera.ProjectRayOrigin(GetViewport().GetMousePosition());
		rayParameters.To = rayParameters.From + globalCamera.ProjectRayNormal(GetViewport().GetMousePosition()) * 1000f;
		
		rayParameters.CollideWithAreas = true;
		rayParameters.CollisionMask = CardCollisionMask; // COLLISION MASK RESERVED FOR CARDS CHECK BEFORE USING!!!!!!!
		
		var result = spaceState.IntersectRay(rayParameters);
		
		if (result.Count != 0) {
			Node3D collider = (Node3D) result["collider"];
			Node3D? cardFound = collider.GetParent<Node3D>();
			if (cardFound is null) {
				throw new Exception(
					$"Card area mask layer ({CardCollisionMask}) used without a card parent at {this}");
			}

			return cardFound;
		}

		return null;
	}
}