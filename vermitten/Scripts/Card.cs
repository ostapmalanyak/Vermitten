#nullable enable
using System;
using Godot;

namespace Vermitten.Scripts;

public partial class Card : Node3D
{
	private const float SelectScale = 0f;
	private const float SelectMove = 5f;
	private const float SelectSeconds = 0.05f;
	
	public bool MouseHovering = false;
	private int _selectingState = 0; // 0 - neutral, 1 - moving up, 2 - moving back
	private float _selectingTimeSpent = 0;
	private float _rotated = 0;
	private float _mult = -1;
	private Vector3 _prevPos;
	private MeshInstance3D? _cardMesh;
	
	public override void _Ready() {
		_cardMesh = GetChild<MeshInstance3D>(0); 
		// Card mesh is the 1st child in the card scene
		
		if (_cardMesh is null) throw new Exception($"No card mesh at {this}");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_cardMesh is null) throw new Exception($"No card mesh at {this}");
		
		if (_selectingState == 1) {
			if (_selectingTimeSpent<SelectSeconds) { // while we haven't moved forward enough
				_selectingTimeSpent+=(float)delta; 
				// add the time since last frame to total time spent
				
				_cardMesh.Translate(Vector3.Up*(SelectMove/SelectSeconds)*(float)delta); 
				// SelectMove/SelectSeconds is how much we need to move per second
				// so we multiply it by the time spent since last frame to move it accordingly
				
				_cardMesh.Scale = new Vector3(
					1f + SelectScale/SelectSeconds*_selectingTimeSpent, 
					1f + SelectScale/SelectSeconds*_selectingTimeSpent,
					1f + SelectScale/SelectSeconds*_selectingTimeSpent);
				// Scare the card accordingly to the time spent
				
			}
			else {
				_cardMesh.Position = new Vector3(_cardMesh.Position.X, _cardMesh.Position.Y, SelectMove*-1);
				// set the position that it's expected to be to remove division inaccuracies
				// (float cannot represent all decimal numbers perfectly)
				
				_cardMesh.Scale = new Vector3(1f + SelectScale, 1f + SelectScale, 1f + SelectScale);
				//same with scaling
				
				_selectingState = 0;
				// put it into neutral
			}
		}
		else if (_selectingState == 2) { // this has the same logic just reversed
			if (_selectingTimeSpent>0) {
				_selectingTimeSpent-=(float)delta;
				
				_cardMesh.Translate(Vector3.Down*(SelectMove/SelectSeconds)*(float)delta);
				_cardMesh.Scale = new Vector3(
					1f + (SelectScale/SelectSeconds)*_selectingTimeSpent,
					1f + (SelectScale/SelectSeconds)*_selectingTimeSpent,
					1f + (SelectScale/SelectSeconds)*_selectingTimeSpent);
			}
			else {
				_cardMesh.Position = _prevPos;
				_cardMesh.Scale = new Vector3(1f, 1f, 1f);
				
				_selectingState = 0;
			}
		}
	}

	private void StartShake() { // this is connected to the mouse_entered action in the CardCollider node of the card scene
		if (_cardMesh is null) throw new Exception($"No card mesh at {this}");
		MouseHovering = true;
		_prevPos = _cardMesh.Position;
		_selectingState = 1; // move card mesh forward
	}

	private void StopShaking() { // this is connected to the mouse_exited action in the CardCollider node of the card scene
		if (_cardMesh is null) throw new Exception($"No card mesh at {this}");
		MouseHovering = false;
		_selectingState = 2; // move card mash back
	}
}