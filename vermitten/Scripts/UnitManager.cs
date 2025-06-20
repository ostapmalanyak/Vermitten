#nullable enable
using System;
using System.IO;
using Godot;

namespace Vermitten.Scripts;

public partial class UnitManager : Node3D
{
	private const int PlatformCollisionMask = 5;
	private CardManager _cardManager = null!;
	private Platform? _platformHovered;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_cardManager = GetNode<CardManager>(new NodePath("../CardInterface/CardManager")) ??
		        throw new FileNotFoundException("Invalid card manager path");
	}
	
	public void ConnectPlatform(Platform platform) {
		Area3D platformArea = platform.PlatformArea;
		platform.PhantomSprite.Visible = false;
		
		platformArea.MouseEntered += () => Hover(platform);
		platformArea.MouseExited += () => UnHover(platform);
	}
	
	public void Hover(Platform platform) {
		if (_cardManager.CardDragged is null) return;
		
		//TODO Add changing sprite image to specific card variable
		_platformHovered = platform;
		_cardManager.CardDragged.CardSprite.Visible = false;
		platform.PhantomSprite.Visible = true;
	}

	public void UnHover(Platform platform) {
		if (_cardManager.CardDragged is null) return;
		
		platform.PhantomSprite.Visible = false;
		_platformHovered = null;
		_cardManager.CardDragged.CardSprite.Visible = true;
	}

	public Platform? CardLetGo() {
		Platform? letGoOn =  _platformHovered;
		_platformHovered = null;
		return letGoOn;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void CreateUnit(Card cardDragged, Platform createOn) {
		PackedScene unitPrefab = GD.Load<PackedScene>(Card.UnitPrefabPath) ??
		             throw new FileNotFoundException("Invalid unit prefab path");
		Unit unit = unitPrefab.Instantiate<Unit>() ?? // TODO Causes leaks for some reason
		            throw new InvalidCastException("Loaded prefab was not a unit");
		
		unit.Name = "Unit"; // temporary !!!!!!!!!!!!!!!
		GD.Print(unit.Name);
		PositionUnit(unit, createOn);
		GD.Print(unit.Position);
	}

	public void PositionUnit(Unit unit, Platform platform) {
		unit.Visible = true;
		unit.Position = new Vector3(platform.Position.X, platform.Position.Y + 6.5f, platform.Position.Z);
	}
}