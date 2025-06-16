using System;
using System.Collections.Generic;
using System.IO;
using Godot;

namespace Vermitten.Scripts;

public partial class Hand : Node2D
{
	
	[Export]
	private int _handCount = 5;
	
	private const float HandYPos = 300;
	private const int CardWidth = 150;
	
	public List<Card> HandCards { get; } = new List<Card>();
	
	private const string CardPrefabPath = "res://Prefabs/card.tscn";
	
	private float _centerScreen;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		_centerScreen = GetViewport().GetVisibleRect().Size.X / 2;
		
		
		PackedScene newCard = GD.Load<PackedScene>(CardPrefabPath) ??
		                      throw new FileNotFoundException("Invalid card prefab path");
		CardManager manager = GetNode<CardManager>(new NodePath("../CardManager")) ??
		                      throw new FileNotFoundException("Invalid card manager path");

		for (int i = 0; i < _handCount; i++) {
			Card card = newCard.Instantiate<Card>() ??
			            throw new InvalidCastException("Loaded prefab was not a card");
			card.Name = "Card";
			card.HandPriority = i;
			
			manager.AddChild(card);
			AddCardToHand(card);
		}
		
	}

	private void NormalizePriorities() {
		int i = 0;
		foreach (var card in HandCards) {
			card.HandPriority = i++;
		}
	}

	public void AddCardToHand(Card card) {
		HandCards.Add(card);
		HandCards.Sort((card1, card2) => card1.HandPriority>=card2.HandPriority ? 1 : 0);
		UpdateHandPosition();
	}

	public void RemoveCardFromHand(Card card) {
		HandCards.Remove(card);
		UpdateHandPosition();
	}

	private void UpdateHandPosition() {
		NormalizePriorities();
		for (int i = 0; i < HandCards.Count; i++) {
			var newPos = new Vector2(CalculatePos(i), HandYPos);
			HandCards[i].HandPos = newPos;
			AnimateCardToPos(HandCards[i], newPos, GetTree());
		}
	} 

	public static void AnimateCardToPos(Card card, Vector2 newPos, SceneTree tree) {
		var tween = tree.CreateTween();
		tween.TweenProperty(card, "position", newPos, 0.1);	
	}

	private float CalculatePos(int index) {
		float xOffset = (HandCards.Count - 1) * CardWidth;
		var xPosition = _centerScreen + index * CardWidth - xOffset/2f;

		return xPosition;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}