using Godot;
using System;

using SpacetimeDB.Types;

public partial class UI : Control
{
	Panel UIUsernameChooser;

	public override void _Ready()
	{
		UIUsernameChooser = GetNode<Panel>("/root/Game/UI/UIUsernameChooser");
		UIUsernameChooser.Hide();
	}

	public void _on_username_field_text_submitted(string value)
	{
		GD.Print($"Selected username: {value}");
		Reducer.CreatePlayer(value);
		UIUsernameChooser.Hide();
	}
}
