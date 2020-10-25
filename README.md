<div align="center">
    <br/>
        <img src="https://fizzyapple12.com/files/github/BeatSaberPresence1.png" alt="BeatSaberPresence1">
        <br/>
        <img src="https://fizzyapple12.com/files/github/BeatSaberPresence2.png" alt="BeatSaberPresence2">
    <br/>
    <br/>
    <h1>Beat Saber Presence</h1>
    <br/>
    <br/>
    <br/>
</div>

## What is this?

Beat Saber Presence is a fully customizable mod for Beat Saber which gives you Rich Presence functionality in Beat Saber.

<br/>

## How can I customize it?

Inside of the game, on the mods panel, there is a tab for BeatSaberPresence. Inside that menu, you can:

* Enable or disable the rich presence
* Enable or disable the large Beat Saber cover art
* Enable or disable the small icon denoting what you are doing
* Enable or disable the timestamp which counts up
* Make the timestamp count down in game

Inside of the BeatSaberPresence.json (Beat Saber/UserData/BeatSaberPresence.json), you can customize:

* All previously mentioned properties
* The Rich Presence inside of the Menu:
  * The top and bottom lines underneath "Beat Saber" (``MenuTopLine`` and ``MenuBottomLine``)
  * The text shown when you hover over the Beat Saber cover art (``MenuLargeImageLine``)
  * The text shown when you hover over the small icon (``MenuSmallImageLine``)
* The Rich Presence inside of the Game:
  * The top and bottom lines underneath "Beat Saber" (``GameTopLine`` and ``GameBottomLine``)
  * The text shown when you hover over the Beat Saber cover art (``GameLargeImageLine``)
  * The text shown when you hover over the small icon (``GameSmallImageLine``)

When customizing the text, you can insert snippets which replace themselves with game data.

Snippet | Description | Replacement Example | Works In
--- | --- | --- | ---
``{DiscordName}`` | Replaced with your Discord username (no discriminator) | ``FizzyApple12 (She/Her)`` | Menu or Game
``{DiscordDiscriminator}`` | Replaced with your Discord discriminator (no username) | ``3494`` | Menu or Game
``{SongName}`` | Replaced with the name of the song you are playing | ``Killbot`` | Game
``{SongSubName}`` | Replaced with the subname of the song you are playing | ``Something or other`` | Game //
``{SongAuthorName}`` | Replaced with the author of the song you are playing | ``Devin Martin`` | Game
``{SongDuration}`` | Replaced with the length of the song you are playing in minutes and seconds | ``03:32`` | Game
``{SongDurationSeconds}`` | Replaced with the length of the song you are playing in seconds | ``212`` | Game
``{LevelAuthorName}`` | Replaced with the name of the mapper of the song you are playing | ``JonathanRune`` | Game
``{SongBPM}`` | Replaced with the BPM of the song you are playing | ``120`` | Game
``{LevelID}`` | Replaced with the level ID of the song you are playing | ``custom_level_3DDE3B1...`` | Game
``{EnvironmentName}`` | Replaced with the name of the environment you are currently playing in | ``Nice`` | Game
``{Submission}`` | Replaced with ``Enabled`` or ``Disabled`` depending on the status of score submission | ``Enabled`` | Game
``{NoFail}`` | Replaced with ``On`` or ``Off`` depending on the status of the "No Fail" modifier | ``Off`` | Game
``{NoBombs}`` | Replaced with ``On`` or ``Off`` depending on the status of the "No Bombs" modifier | ``Off`` | Game
``{NoObsticles}`` | Replaced with ``On`` or ``Off`` depending on the status of the "No Obsticles" modifier | ``Off`` | Game
``{NoArrows}`` | Replaced with ``On`` or ``Off`` depending on the status of the "No Arrows" modifier | ``Off`` | Game
``{SlowerSong}`` | Replaced with ``On`` or ``Off`` depending on the status of the "Slower Song" modifier | ``Off`` | Game
``{InstaFail}`` | Replaced with ``On`` or ``Off`` depending on the status of the "Insta Fail" modifier | ``Off`` | Game
``{BatteryEnergy}`` | Replaced with ``On`` or ``Off`` depending on the status of the "Battery Energy" modifier | ``Off`` | Game
``{GhostNotes}`` | Replaced with ``On`` or ``Off`` depending on the status of the "Ghost Notes" modifier | ``Off`` | Game
``{DisappearingArrows}`` | Replaced with ``On`` or ``Off`` depending on the status of the "Disappearing Arrows" modifier | ``Off`` | Game
``{FasterSong}`` | Replaced with ``On`` or ``Off`` depending on the status of the "Faster Song" modifier | ``Off`` | Game

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