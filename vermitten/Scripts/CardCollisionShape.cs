#nullable enable
using System;
using Godot;

namespace Vermitten.Scripts;

public partial class CardCollisionShape : CollisionShape3D
{
	// Called when the node enters the scene tree for the first time.
	private const string MeshPath = "./CardMesh";
	public override void _Ready() {
		Node3D parent = GetParent<Node3D>();
		if (parent is null) {
			throw new Exception($"No parent found in {this}");
		}
		
		parent = parent.GetParent<Node3D>();
		if (parent is null) {
			throw new Exception($"No parent found in {this}");
		}

		MeshInstance3D CardMesh = (MeshInstance3D) parent.FindChild(MeshPath);
		CardMesh.CreateConvexCollision();
		CollisionShape3D = FindChild()
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}