using Godot;
using System;

public partial class MainTileMapLayer : TileMapLayer
{
	[Export] private TileMapLayer _hoverLayer;
	private Vector2I? _hoveredCell = null;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (_hoverLayer == null)
        GD.PrintErr("HexMap: _hoverLayer is not assigned in the editor!");
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
			GD.Print("Cell: " + _hoveredCell + " | SourceId: " + GetCellSourceId(cell) + " | CellAtlas: " + GetCellAtlasCoords(cell));
			// _hoverLayer.SetCell(cell, GetCellSourceId(cell), GetCellAtlasCoords(cell));
			_hoverLayer.SetCell(cell, 1, new Vector2I(0,0));
			GD.Print("HoverLayer Count: " + _hoverLayer.GetUsedCells().Count);
		}
		else
		{
			_hoveredCell = null;
		}

	}


	// Leaving here for now, maybe recycle these for battle scaring
	/*private void HighlightCell(Vector2I cell)
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
	}*/

  public override void _UnhandledInput(InputEvent @event)
  {
    if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			// Fetch our mouse position and hex tile
			// var local_pos = _tileMapLayer.GetLocalMousePosition();
			var global_pos = GetGlobalMousePosition();
			GD.Print("Mouse Pos: " + global_pos);
		}
  }

}
