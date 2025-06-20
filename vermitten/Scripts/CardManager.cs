#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using Godot;

namespace Vermitten.Scripts;

public partial class CardManager : Node2D
{
	private const string LeftClickPress = "LeftClick";
	private Card? _cardHovering;
	private Vector2 _screenSize;
	private readonly List<Card> _hoverQueue = new List<Card>();
	private Hand _hand = null!;
	private UnitManager _unitManager = null!;

	private const float DecreaseSizeStart = 500;
	private const float DecreaseSizeStop = 390;
	
	[Export]
	public Vector2 NormalCardSize { get; private set; } = new Vector2(0.25f, 0.25f);
	
	public Card? CardDragged { get; private set; }
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		_screenSize = GetViewport().GetVisibleRect().Size;
		_hand = GetNode<Hand>(new NodePath("../Hand")) ??
		                      throw new FileNotFoundException("Invalid hand path");
		_unitManager = GetNode<UnitManager>(new NodePath("../../UnitManager")) ??
		        throw new FileNotFoundException("Invalid unit manager path");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)	
	{
		if (CardDragged is not null) {
			CardDragged.Position = new Vector2(
				Math.Clamp(GetGlobalMousePosition().X, 0, _screenSize.X),
				Math.Clamp(GetGlobalMousePosition().Y, 0, _screenSize.Y));

			CardDragged.Scale = NormalCardSize *
			                     (float)Math.Pow((Math.Clamp(GetGlobalMousePosition().Y,
				                                      DecreaseSizeStop,
				                                      DecreaseSizeStart) / DecreaseSizeStart), 8);
		}
	}

	public override void _Input(InputEvent @event) {
		if (@event.IsActionPressed(LeftClickPress)) {
			//GD.Print("click");
			Card? card = MouseHovering();
			//find which card is being hovered over if any
			
			if (card is not null) {
				GD.Print($"card clicked - {card.Name} with hand priority {card.HandPriority}");
				StartDrag(card);
			}
		}
		if (@event.IsActionReleased(LeftClickPress)) {
			//GD.Print("click release");
			EndDrag();
		}
	}

	private void StartDrag(Card card) {
		CardDragged = card;
		_hand.RemoveCardFromHand(card);
		HighlightCard(card, false, false);
	}
	
	private void EndDrag() {
		if (CardDragged is null) {
			return;
		}

		Platform? letGoOn = _unitManager.CardLetGo();
		GD.Print(letGoOn);
		
		if (letGoOn is null) {
			if (!_hand.HandCards.Contains(CardDragged)) {
				_hand.AddCardToHand(CardDragged);
			}
		
			Hand.AnimateCardToPos(CardDragged, CardDragged.HandPos, GetTree());
			HighlightCard(CardDragged, true);
			CardDragged = null;
		}
		else {
			_unitManager.CreateUnit(CardDragged, letGoOn);
			
			//CardDragged.QueueFree(); TODO Implement Free later
			CardDragged.Visible = false;
			CardDragged = null;
		}
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

		card.Scale = NormalCardSize;
		
		cardArea.MouseEntered += () => Hover(card);
		cardArea.MouseExited += () => UnHover(card);
	}

	private void Hover(Card card) {
		_hoverQueue.Add(card); // add it to the hover queue
		
		if(CardDragged!=_hoverQueue[0]) {
			HighlightCard(_hoverQueue[0], true); // highlight the top card
		}
	}

	private void UnHover(Card card) {
		HighlightCard(card, false); // unhighlight the card from whose area mouse exited
		_hoverQueue.Remove(card); // remove from queue
		
		if(_hoverQueue.Count!=0 && CardDragged!=_hoverQueue[0]) {
			HighlightCard(_hoverQueue[0], true); // hover the new top card
		}
	}

	private void HighlightCard(Card card, bool hovered, bool zChange = true) {
		if (hovered) {
			card.Scale = NormalCardSize*1.05f;
			if(zChange) card.ZIndex = 2;
		}
		else {
			card.Scale = NormalCardSize;
			if(zChange) card.ZIndex = 1;
		}
	}
}