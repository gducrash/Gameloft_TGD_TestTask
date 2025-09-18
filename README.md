# Technical Game Design Test Task for Gameloft
By Alex H.

<img width="2371" height="1183" alt="image" src="https://github.com/user-attachments/assets/fed4bbab-d852-4b88-af92-16a0c07816ed" />


## Setup guide
- First, ensure you have git and git-lfs, as this repository contains large files (scene lighting data) over 100MB.
- Clone the repository and pull its contents.
- Open the project in Unity 6.2. It should take a while, as the library and object files get generated.
- In the editor, navigate to `Assets/Scenes` and open the `01_OutdoorsScene` scene.
- In `Project Settings > Quality > Levels` you can choose between Fantastic, Balanced and Performance. All 3 quality levels have been tested and optimized for different hardware categories. If you're on a high-end PC, use Fantastic; if you're on a mid-range device (like a laptop); choose Balanced and for mobile devices choose Performance.
- Press play to test it in the editor, or build the game to your platform of choice.

## Task overview
I have built 2 small demo scenes — one outdoors and one indoors — using the following assets from the Unity Asset Store:
- [Unity Learn | 3D Game Kit](https://assetstore.unity.com/packages/templates/tutorials/unity-learn-3d-game-kit-115747)
- [Animals FREE by ithappy](https://assetstore.unity.com/packages/3d/characters/animals/animals-free-animated-low-poly-3d-models-260727)
- [Campfires & Torches by Piloto Studio](https://assetstore.unity.com/packages/3d/environments/campfires-torches-models-and-fx-242552)
- [Piloto Studio Shaders](https://assetstore.unity.com/packages/vfx/shaders/piloto-studio-shaders-258376) (dependency library)

<img width="2206" height="1232" alt="image" src="https://github.com/user-attachments/assets/7387f1a3-a0b2-496d-954f-858dd47186b0" />

Each scene is optimized to run fast while looking good using Light Probes, Baked GI, Occlusion Culling and other common techniques. The scenes feature a controllable player character, a few NPC characters (such as animals and enemy monsters), interactable elements, moving platforms and a final boss character.

This task was made in 1 week. I started it on September 12th and finished on September 19th.

## Scene Structure
Each scene follows the following hierarchy structure:
- **Gameplay** elements contain a death volume below the floor.
- **GameUtilities** — include a camera rig, transition, checkpoint and audio systems, etc.
- **UI** — all of the user interface.
- **Characters** — playable character and NPCs in the scene.
- **LevelAssets**
  - **Skybox** — contain skybox geometry, rendered separately by the skybox camera.
  - **World** — all of the location's objects, decorations and interactable elements. Inside it, there are further categories to organize all of the objects.
  - **Navmesh** — navmesh guides and surfaces. Used by NPC characters.
- **Lighting**
  - **Lights** — contains all static lights. Please do not, that some lights, specifically torch lights and dynamic lights in the characters are located inside those respective objects.
  - **Probes** — light and reflection probes.
- **SpawnedPrefabs** — objects spawned at runtime.

## Project Structure
And here's the overall hierarchy of the project's assets:
- `/AssetPacks` — contains all external asset packs mentioned above.
- `/Materials`
- `/Models`
  - `/Location` — FBX models specific to the scene locations. Made in Blender.
- `/Prefabs`
  - `/Entities` — NPC entities, such as NavMesh agents.
- `/Scripts`
- `/Settings` — GI settings used for Light Baking.
- `/Shaders` — custom shaders.
- `/Textures` — art and textures not present in the asset packs.

o/
