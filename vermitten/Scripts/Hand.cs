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
	private const int CardWidth = 200;
	private readonly List<Card> _hand = new List<Card>();
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
			manager.AddChild(card);
			AddCardToHand(card);
		}
		
	}

	private void AddCardToHand(Card card) {
		_hand.Add(card);
		UpdateHandPosition();
	}

	private void UpdateHandPosition() {
		for (int i = 0; i < _hand.Count; i++) {
			var newPos = new Vector2(CalculatePos(i), HandYPos);
			_hand[i].HandPos = newPos;
			AnimateCardToPos(_hand[i], newPos, GetTree());
		}
	}

	public static void AnimateCardToPos(Card card, Vector2 newPos, SceneTree tree) {
		var tween = tree.CreateTween();
		tween.TweenProperty(card, "position", newPos, 0.1);	
	}

	private float CalculatePos(int index) {
		float xOffset = (_hand.Count - 1) * CardWidth;
		var xPosition = _centerScreen + index * CardWidth - xOffset/2f;

		return xPosition;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}