using Godot;
using System;

public partial class BaseUnit : Node2D
{
	// Called when the node enters the scene tree for the first time.
	[Export] public int MaxMovement { get; set; } = 3;
	public Vector2I CurrentCell { get; private set; }
	private TileMapLayer _map;
	public override void _Ready()
	{
	
	}

	public void Init(Vector2I cell, TileMapLayer map)
	{
		// _map = GetTree().GetFirstNodeInGroup("HexMap") as TileMapLayer;
		_map = map;
		CurrentCell = cell;
		Position = _map.MapToLocal(cell);
	}

	public void MoveTo(Vector2I targetCell)
	{
		CurrentCell = targetCell;
		Position = _map.MapToLocal(targetCell);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
