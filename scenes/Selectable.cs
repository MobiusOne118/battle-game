using Godot;
using System;

public partial class Selectable : Area2D
{
	// Declare with 'EventHandler', use prefix name for function
	// Selected -> SelectedEventHandler
	[Signal]
	public delegate void SelectedEventHandler();
	
	[Signal]
	public delegate void DeselectedEventHandler();

	private bool _isHovered = false;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

  public override void _UnhandledInput(InputEvent @event)
  {
    if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			if (mouseEvent.ButtonIndex == MouseButton.Left && _isHovered)
			{
				if (Input.IsActionPressed("select_sub"))
				{
					EmitSignal(SignalName.Deselected);
				} 
				else
				{
					EmitSignal(SignalName.Selected);
				}

			} 
			else if (mouseEvent.ButtonIndex == MouseButton.Left && !Input.IsActionPressed("select_add"))
			{
				EmitSignal(SignalName.Deselected);
			}
		}
  }

  private void OnMouseEntered()
	{
		_isHovered = true;
	}
	private void OnMouseExited()
	{
		_isHovered = false;
	}
}
