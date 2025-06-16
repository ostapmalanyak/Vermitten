#nullable enable
using System;
using System.Collections.Generic;
using Godot;

namespace Vermitten.Scripts;

public partial class CardManager : Node2D
{
	private const string LeftClickPress = "LeftClick";
	//private const int CardCollisionMask = 5; //unused for now
	private Card? _cardDragged;
	private Card? _cardHovering;
	private Vector2 _screenSize;
	private readonly List<Card> _hoverQueue = new List<Card>();
	
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
				StartDrag(card);
			}
		}
		if (@event.IsActionReleased(LeftClickPress)) {
			//GD.Print("click release");
			EndDrag();
		}
	}

	private void StartDrag(Card card) {
		_cardDragged = card;
		HighlightCard(card, false, false);
	}
	
	private void EndDrag() {
		if (_cardDragged is null) {
			return;
		}
		Hand.AnimateCardToPos(_cardDragged, _cardDragged.HandPos, GetTree());
		HighlightCard(_cardDragged, true);
		_cardDragged = null;
	}

	private Card? MouseHovering() {
		var children = GetChildren();
		Card? maxCard = null;
		int maxZ = -1;
		
		foreach (var child in children) {
			// go through every child in the card manager to check which one the mouse is hovering over has the highest z index
			
			if (child is not Card card) {
				// CardManager should only contain cards as children
				throw new Exception($"Card Manager contains non-cards({child.Name} - {child})");
			}

			if (card.MouseHovering) {
				if (maxZ < card.ZIndex) {
					maxZ = card.ZIndex;
					maxCard = card;
				}
			}
		}

		return maxCard;
	}

	public void ConnectCard(Card card) {
		Area2D cardArea = card.CardArea;

		card.Scale = _normalCardSize;
		
		cardArea.MouseEntered += () => Hover(card);
		cardArea.MouseExited += () => UnHover(card);
	}

	private void Hover(Card card) {
		_hoverQueue.Add(card); // add it to the hover queue
		
		if(_cardDragged!=_hoverQueue[0]) {
			HighlightCard(_hoverQueue[0], true); // highlight the top card
		}
		
		GD.Print("hover");
	}

	private void UnHover(Card card) {
		HighlightCard(card, false); // unhighlight the card from whose area mouse exited
		_hoverQueue.Remove(card); // remove from queue
		
		if(_hoverQueue.Count!=0 && _cardDragged!=_hoverQueue[0]) {
			HighlightCard(_hoverQueue[0], true); // hover the new top card
		}
		
		GD.Print("unhover");
	}

	private void HighlightCard(Card card, bool hovered, bool zChange = true) {
		if (hovered) {
			card.Scale = _normalCardSize*1.05f;
			if(zChange) card.ZIndex = 2;
		}
		else {
			card.Scale = _normalCardSize;
			if(zChange) card.ZIndex = 1;
		}
	}
}