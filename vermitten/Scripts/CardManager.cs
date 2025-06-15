#nullable enable
using System;
using System.IO;
using Godot;

namespace Vermitten.Scripts;

public partial class CardManager : Node2D
{
	private const string LeftClickPress = "LeftClick";
	private const int CardCollisionMask = 5; // unused for now
	private Card? _cardDragged;
	private Card? _cardHovering;
	private Vector2 _screenSize;
	
	[Export]
	private Vector2 _normalCardSize = new Vector2(0.25f, 0.25f);
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		_screenSize = GetViewport().GetVisibleRect().Size;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)	
	{
		if (_cardDragged is not null) {
			_cardDragged.Position = new Vector2(
				Math.Clamp(GetGlobalMousePosition().X, 0, _screenSize.X),
				Math.Clamp(GetGlobalMousePosition().Y, 0, _screenSize.Y));
		}
	}

	public override void _Input(InputEvent @event) {
		if (@event.IsActionPressed(LeftClickPress)) {
			//GD.Print("click");
			Card? card = MouseHovering();
			//find which card is being hovered over if any
			
			if (card is not null) {
				GD.Print($"card clicked - {card.Name}");
				_cardDragged = card;
			}
		}
		if (@event.IsActionReleased(LeftClickPress)) {
			//GD.Print("click release");
			_cardDragged = null;
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

	public void ConnectCard(Card card) {
		Area2D cardArea = card.CardArea;

		card.Scale = _normalCardSize;
		
		cardArea.MouseEntered += () => Hover(card);
		cardArea.MouseExited += () => UnHover(card);
	}

	public void Hover(Card card) {
		if (_cardHovering is null) {
			_cardHovering = card;
			HighlightCard(card, true); // Actually make raycast instead of action based!!
			GD.Print("hover");
		}
	}

	public void UnHover(Card card) {
		if (_cardHovering == card) {
			_cardHovering = null;
			HighlightCard(card, false);
			GD.Print("unhover");
		}
	}

	public void HighlightCard(Card card, bool hovered) {
		if (hovered) {
			card.Scale = _normalCardSize*1.05f;
			card.ZIndex = 2;
		}
		else {
			card.Scale = _normalCardSize;
			card.ZIndex = 1;
		}
	}
}