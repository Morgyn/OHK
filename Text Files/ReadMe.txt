https://github.com/Palakis/obs-websocket-dotnet to communicate with OBS
https://github.com/gmamaladze/globalmousekeyhook to detect the key input

Required to install: https://github.com/Palakis/obs-websocket (Set Password and Enable authentication) https://hoppel.co/QJh1.png

Config

#  "IP Adress. Default: 127.0.0.1": "127.0.0.1"  #
- Use your local ip address (127.0.0.1) or if OBS is running on another IP you need to use the IP OBS is running on

#  "Port. Default: 4444": "4444"  #
- Use the port you set in your OBS websocket settings https://hoppel.co/kB5o.png

#  "Password from your OBS websocket plugin": "test"  #
- Enter the password you set in your OBS websocket settings https://hoppel.co/h82q.png

#  "Key to pause the application": 117  #
- Set a key you wanna use to pause the program (117 = F6). There is a KeyCodes.txt that has all numbers for each key

#  "Delay after releasing key": 300  #
- After releasing the key it will delay making the item invisible by xx ms (default 300m)

#  "KeysSetup":   #
- Add Keys Linked to a scene item, it will always access the items in the current scene so make sure you are in the right scene
- Pressing the key will make the item VISIBLE
- Releasing the Key will make the item INVISIBLE

Example:

#  "G": "Image"  #
- Pressing G makes the item 'Image' visible in your scene
- Releasing G makes the item 'Image' invisible in your scene


