using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class MainTileMapLayer : TileMapLayer
{
	[Signal] public delegate void CellClickedEventHandler(Vector2I cell);
	[Export] private TileMapLayer _hoverLayer;
	[Export] private TileMapLayer _selectionLayer;
	[Export] private PackedScene _unitScene;
	[Export] private int _unitCount { get; set; } = 2;
	
	private Node2D _unitsContainer;
	private Vector2I? _hoveredCell = null;
	private Vector2I? _selectedCell = null;
	private bool _isDeploymentPhase = true;
	// private List<BaseUnit> _units = new List<BaseUnit>();
	private Dictionary<Vector2I, BaseUnit> _unitsByCell = new Dictionary<Vector2I, BaseUnit>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (_hoverLayer == null)
			GD.PrintErr("HexMap: _hoverLayer is not assigned in the editor!");

		_unitsContainer = GetTree().Root.GetNode<Node2D>("Main/UnitsContainer");
		
		if (_isDeploymentPhase)
		{
			DeploymentPhase(_unitCount);
		}
		else
		{
			SpawnUnit(new Vector2I(0,1));
		}
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 mousePos = GetGlobalMousePosition();
		Vector2I cell = LocalToMap(ToLocal(mousePos));

		if (cell == _hoveredCell) return;

		// Remove old hover tile
		if (_hoveredCell.HasValue)
		{
			_hoverLayer.EraseCell(_hoveredCell.Value);
		}

		// Update hover tile
		if (GetCellSourceId(cell) != -1)
		{
			_hoveredCell = cell;
			// GD.Print("Cell: " + _hoveredCell + " | SourceId: " + GetCellSourceId(cell) + " | CellAtlas: " + GetCellAtlasCoords(cell));
			// _hoverLayer.SetCell(cell, GetCellSourceId(cell), GetCellAtlasCoords(cell));
			_hoverLayer.SetCell(cell, 1, new Vector2I(0,0));
		}
		else
		{
			_hoveredCell = null;
		}

	}

	//
	// Game Phase Logic
	//
	public async void DeploymentPhase(int unitCount)
	{
		for (var i = 0; i < unitCount; i++)
		{
			GD.Print("Select a tile to deploy unit " + i + ".");
			Vector2I cell = await AwaitCellClick();
			SpawnUnit(cell);
		}
		_isDeploymentPhase = false;

		foreach (var unit in _unitsByCell)
		{
			GD.Print("Unit: " + unit.Value + " | Pos: " + unit.Key);
		}
	}

	//
	// Highlight selected cell logic
	//
	private void SelectCell(Vector2I cell)
	{
		_selectedCell = cell;
		// Clear previous selection
    _selectionLayer.Clear();
    
    // Paint new selection
    _selectionLayer.SetCell(cell, 1, new Vector2I(0, 0));
	}
	private void SelectClear()
	{
		_selectedCell = null;
		_selectionLayer.Clear();
	}

	//
	// Unit Spawn
	//
	private void SpawnUnit(Vector2I cell)
	{
		GD.Print("Spawning unit at " + cell);
		var unit = _unitScene.Instantiate<BaseUnit>();
    _unitsContainer.AddChild(unit);
		_unitsByCell.Add(cell, unit);
    unit.Init(cell, this);
	}

	//
	// User Interaction logic
	//
  public override void _Input(InputEvent @event)
  {
		// Left Mouse Click
    if (@event is InputEventMouseButton mouseEvent
			&& mouseEvent.ButtonIndex == MouseButton.Left
			&& mouseEvent.Pressed)
		{
			Vector2I cell = LocalToMap(ToLocal(mouseEvent.Position));

			if (GetCellSourceId(cell) != -1)
			{
				// GD.Print("Selected cell: " + cell);
				SelectCell(cell);
				EmitSignal(SignalName.CellClicked, cell);
			}
		}

		// Right Mouse Click
		if (@event is InputEventMouseButton rightClickEvent
			&& rightClickEvent.ButtonIndex == MouseButton.Right
			&& rightClickEvent.Pressed)
		{
			SelectClear();
		}
  }
	
	public async Task<Vector2I> AwaitCellClick()
	{
		var result = await ToSignal(this, SignalName.CellClicked);
		return (Vector2I)result[0];
	}

	// Leaving here for now
	// maybe recycle these for battle scaring
	/*
	private void HighlightCell(Vector2I cell)
	{
		int sourceId = GetCellSourceId(cell);
		Vector2I atlasCoords = GetCellAtlasCoords(cell);
		SetCell(cell, sourceId, atlasCoords, HighlightAltId);
	}

	private void RestoreCell(Vector2I cell)
	{
		int sourceId = GetCellSourceId(cell);
		if (sourceId == -1) return; // No cell exists


		Vector2I atlasCoords = GetCellAtlasCoords(cell);
		SetCell(cell, sourceId, atlasCoords, NormalAltId);
	}
	*/
}
