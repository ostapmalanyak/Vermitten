using System.IO;
using Godot;

namespace Vermitten.Scripts;

public partial class Platform : Node3D
{
	private UnitManager _unitManager;

	public Area3D PlatformArea { get; private set; }
	
	public Sprite3D PhantomSprite { get; private set; }
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PlatformArea = GetNode<Area3D>("PlatformArea") ?? throw new FileNotFoundException($"No platform area in card {this}");
		PhantomSprite = GetNode<Sprite3D>("PhantomUnit") ?? throw new FileNotFoundException($"No phantom sprite in card {this}");

		_unitManager = GetTree().Root.GetChild(0).GetNode<UnitManager>("UnitManager") ??
		               throw new FileNotFoundException($"No unit manager found in root dir");
		_unitManager.ConnectPlatform(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Hover() {
		return;
	}

	public void UnHover() {
		return;
	}
}