<div align="center">
    <br/>
        <img src="https://fizzyapple12.com/files/github/BeatSaberPresence1.png" alt="BeatSaberPresence1">
        <br/>
        <img src="https://fizzyapple12.com/files/github/BeatSaberPresence2.png" alt="BeatSaberPresence2">
    <br/>
    <br/>
    <h2>Beat Saber Presence</h2>
    <br/>
    <br/>
    <br/>
</div>

## What is this?

Beat Saber Presence is a simple mod for Beat Saber which gives you Rich Presence functionality in Discord.

<br/>

## Great! How do I get it?

You can either download a released version from the releases tab or build it yourself.

Here are the steps to build the Project:

1. Link all of the DLLs from the Beat Saber directory

2. Install the ``DiscordRichPresence`` NuGet Package

3. Download ``NativeNamedPipe.dll`` from https://github.com/Lachee/unity-named-pipes/tree/master/Unity%20Package/Assets/NamedPipeClient/Plugins/x86_64 in order to allow Rich Presence to connect properly

4. Build the project

5. Copy ``NativeNamedPipe.dll`` and ``DiscordRPC.dll`` in to the ``Libs`` folder

6. Copy ``BeatSaberPresence.dll`` to the ``Plugins`` folder

7. That's it! You're done!

<br/>

## Code Sources

https://github.com/Lachee/discord-rpc-csharp (Rich Presence Library)

https://github.com/Lachee/unity-named-pipes (Native Named Pipes for Unity so the Rich Presence Library doesn't heccin die)