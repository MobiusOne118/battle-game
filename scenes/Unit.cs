using Godot;
using System;

public partial class Unit : Node2D
{
	private Selectable _selectable;
	private ColorRect _selectionUI;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_selectable = GetNode<Selectable>("Selectable");
		_selectionUI = GetNode<ColorRect>("SelectionUI");
		_selectable.Selected += OnSelected;
		_selectable.Deselected += OnDeselected;
	}

	private void OnSelected()
	{
		// GD.Print("Unit Selected");
		OnUpdateUI(true);
	}

	private void OnDeselected()
	{
		// GD.Print("Unit Deselected");
		OnUpdateUI(false);
	}

	private void OnUpdateUI(bool isSelected)
	{
		if (isSelected)
		{
			_selectionUI.Visible = true;
		} else
		{
			_selectionUI.Visible = false;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
