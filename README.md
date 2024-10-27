# OHK

Download here: https://github.com/Morgyn/OHK/releases/latest

This application is designed to allow momentary hotkeys in OBS.

It uses the inbuilt WebSocket Server that is in OBS28+

At the moment this is limited to one hotkey that shows a source when pressed, and hides it again when released.

This is primarily designed as a map hider for Rust, but hopefully the full range of features will be added soon.

## How to configure

Watch this video or, read the steps below:

https://github.com/user-attachments/assets/405a1469-2b8a-4a14-8258-e29344fc66bf

 1. Create a new image source above your game, This will need to cover the entire game.
 2. In OBS, Enable the WebSocket Server
	 1. Select Tools > WebSocket Server Settings
	 2. Check the Enable WebSocket server setting
	 3. Click Apply
	 4. Accept the windows firewall request
 3. Copy the settings for the WebSocket Server into OHK
	 1. Start OHK
	 2. In OHK, open the OHK configuration
     1. Click the ☰
     2. Click configure
	 4. In OBS, copy the connection settings into ohk
		 1. Select Tools > WebSocket Server Settings
		 2. Select Show Connection Info
		 3. Copy IP, Port and Password in turn, and Paste into OHK
	 5. Test the connection, Then save.
 4. Configure the HotKey in OHK
	 1. Click Connect
	 2. Click the ☰, then click configure
	 3. Select the Key, Scene and Source
	 4. Click Save
