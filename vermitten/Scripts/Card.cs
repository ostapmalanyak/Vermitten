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
	private float _selectingFrame = 0;
	private float _rotated = 0;
	private float _mult = -1;
	private Vector3 _prevPos;
	private MeshInstance3D? _cardMesh;
	
	public override void _Ready() {
		_cardMesh = GetChild<MeshInstance3D>(0);
		if (_cardMesh is null) throw new Exception($"No card mesh at {this}");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_cardMesh is null) throw new Exception($"No card mesh at {this}");
		
		if (_selectingState == 1) {
			if (_selectingFrame<SelectSeconds) {
				_selectingFrame+=(float)delta;
				
				_cardMesh.Translate(Vector3.Up*(SelectMove/SelectSeconds)*(float)delta);
				_cardMesh.Scale = new Vector3(
					1f + SelectScale/SelectSeconds*_selectingFrame,
					1f + SelectScale/SelectSeconds*_selectingFrame,
					1f + SelectScale/SelectSeconds*_selectingFrame);
			}
			else {
				_cardMesh.Position = new Vector3(_cardMesh.Position.X, _cardMesh.Position.Y, SelectMove*-1);
				_cardMesh.Scale = new Vector3(1f + SelectScale, 1f + SelectScale, 1f + SelectScale);
				
				_selectingState = 0;
			}
		}
		else if (_selectingState == 2) {
			if (_selectingFrame>0) {
				_selectingFrame-=(float)delta;
				
				_cardMesh.Translate(Vector3.Down*(SelectMove/SelectSeconds)*(float)delta);
				_cardMesh.Scale = new Vector3(
					1f + (SelectScale/SelectSeconds)*_selectingFrame,
					1f + (SelectScale/SelectSeconds)*_selectingFrame,
					1f + (SelectScale/SelectSeconds)*_selectingFrame);
			}
			else {
				_cardMesh.Position = _prevPos;
				_cardMesh.Scale = new Vector3(1f, 1f, 1f);
				
				_selectingState = 0;
			}
		}
	}

	private void StartShake() {
		if (_cardMesh is null) throw new Exception($"No card mesh at {this}");
		MouseHovering = true;
		_prevPos = _cardMesh.Position;
		_selectingState = 1;
	}

	private void StopShaking() {
		if (_cardMesh is null) throw new Exception($"No card mesh at {this}");
		MouseHovering = false;
		_selectingState = 2;
	}
}