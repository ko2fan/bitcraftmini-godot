using Godot;
using System;

using SpacetimeDB;
using SpacetimeDB.Types;
using System.Net.WebSockets;
using System.Collections.Generic;

public partial class SpacetimeDBManager : Node
{
	private Identity local_identity;

	const string HOST = "localhost:3000";
	const string DBNAME = "bitcraftmini-godot";
	const bool SSL_ENABLED = false;

	Panel UIUsernameChooser;

	public override void _Ready()
	{
		UIUsernameChooser = GetNode<Panel>("/root/Game/UI/UIUsernameChooser");

		AuthToken.Init(".spacetime_bitcraftmini-godot");

		SpacetimeDBClient.CreateInstance(new ConsoleLogger());

		SpacetimeDBClient.instance.onConnect += () =>
        {
            GD.Print("Connected.");

            SpacetimeDBClient.instance.Subscribe(new List<string>()
            {
                "SELECT * FROM Config",
                "SELECT * FROM SpawnableEntityComponent",
                "SELECT * FROM PlayerComponent",
                "SELECT * FROM MobileEntityComponent",
            });
        };

        // called when we have an error connecting to SpacetimeDB
        SpacetimeDBClient.instance.onConnectError += (error, message) =>
        {
            GD.PrintErr($"Connection error: " + (error.HasValue ? error.Value.ToString() : "Null") + " - " + message);
        };

        // called when we are disconnected from SpacetimeDB
        SpacetimeDBClient.instance.onDisconnect += (closeStatus, error) =>
        {
            GD.Print("Disconnected.");
        };


        // called when we receive the client identity from SpacetimeDB
        SpacetimeDBClient.instance.onIdentityReceived += (token, identity) => {
            AuthToken.SaveToken(token);
            local_identity = identity;
        };


        // called after our local cache is populated from a Subscribe call
        SpacetimeDBClient.instance.onSubscriptionApplied += OnSubscriptionApplied;

        PlayerComponent.OnInsert += PlayerComponent_OnInsert;

        //Reducer.OnChatMessageEvent += OnChatMessageEvent;

		SpacetimeDBClient.instance.Connect(AuthToken.Token, HOST, DBNAME, SSL_ENABLED);
	}

	public override void _Process(double delta)
	{
		SpacetimeDBClient.instance.Update();
	}
	
	void OnSubscriptionApplied()
	{
		var player = PlayerComponent.FilterByOwnerId(local_identity);
		if (player == null)
		{
			// Show username selection
			UIUsernameChooser.Show();
		}

		// Show the Message of the Day in our Config table of the Client Cache
		//UIChatController.instance.OnChatMessageReceived("Message of the Day: " + Config.FilterByVersion(0).MessageOfTheDay);

		// Now that we've done this work we can unregister this callback
		SpacetimeDBClient.instance.onSubscriptionApplied -= OnSubscriptionApplied;
	}

	private void PlayerComponent_OnInsert(PlayerComponent obj, ReducerEvent callInfo)
    {
        // if the identity of the PlayerComponent matches our user identity then this is the local player
        if (obj.OwnerId == local_identity)
        {
            // Get the MobileEntityComponent for this object and update the position to match the server
            MobileEntityComponent mobPos = MobileEntityComponent.FilterByEntityId(obj.EntityId);
            // Vector3 playerPos = new Vector3(mobPos.Location.X, 0.0f, mobPos.Location.Z);
            // LocalPlayer.instance.transform.position = new Vector3(playerPos.x, MathUtil.GetTerrainHeight(playerPos), playerPos.z);
            // LocalPlayer.instance.EntityId = obj.EntityId;

            // PlayerInventoryController.Local.Spawn();
			var playerScene = GD.Load<PackedScene>("res://Scenes/player.tscn");
			Player playerNode = playerScene.Instantiate() as Player;
			playerNode.Position = new Vector3(mobPos.Location.X, 1.0f, mobPos.Location.Z);
			AddChild(playerNode);

            // // Now that we have our initial position we can start the game
            // StartGame();
        }
        // otherwise this is a remote player
        else
        {
            // spawn the player object and attach the RemotePlayer component
            // var remotePlayer = Instantiate(PlayerPrefab);
            // remotePlayer.AddComponent<RemotePlayer>().EntityId = obj.EntityId;
        }
    }
}
