#nullable enable
using System;
using System.IO;
using Godot;

namespace Vermitten.Scripts;

// Remember this if needed sometime
/*
[Signal]
public delegate void SignalEventHandler();
https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_signals.html
*/

public partial class Card : Node2D
{
	public bool MouseHovering { get; private set; } // Only card should change when it's getting hovered over
	
	public Area2D CardArea { get; private set; } = null!; // cannot be null
	
	public int HandPriority { get; set; }
	
	public Vector2 HandPos { get; set; }
	
	private CardManager? _cardManager;
	
	public override void _Ready() {
		CardArea = GetNode<Area2D>("CardArea") ?? throw new FileNotFoundException("No card area in card") 
			?? throw new FileNotFoundException($"No card area in card {this}");
		
		_cardManager = GetParent<CardManager>()
		                           ?? throw new InvalidCastException("Parent is not a card manager");
		_cardManager.ConnectCard(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public void MouseEntered() {
		MouseHovering = true;
		//GD.Print("Hovering");
	}

	public void MouseExited() {
		MouseHovering = false;
		//GD.Print("Stopped hovering");
	}
}