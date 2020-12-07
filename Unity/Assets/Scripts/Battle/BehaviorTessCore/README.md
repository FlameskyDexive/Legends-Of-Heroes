![planilo](https://user-images.githubusercontent.com/1077394/91163953-be4d8d80-e6a4-11ea-9f86-127a6374235d.png)

[![Discord](http://img.shields.io/badge/discord-join-lightblue?logo=discord&style=flat)](https://discord.gg/QrMfMsy)

A set of tools for designing AI in a visual node editor on unity. Use for:
* Behavior Trees.
* Finite State Machines.
* Implement your own AI graphs.

## Road to v0.2.0
I'm in the process of updating this repository, if you are interested in seeing the changes follow the `development` branch. Also feel free to jump on discord with any suggestions you might have about how to improve these tools.

### What is changing?
* **Separation of concerns.** Using xNode as a serialization and behaviour builder tool only. Making no assumptions of how runtime execution should be.
* Interface blackboards for behaviours instead of dictionaries.
* Behaviour tree overhaul with new composite nodes Active Selector and Active Sequencer.

### Other possible changes
* Jobified versions of the AI behaviour. 
* Utility based Behaviour Tree.
* Some samples.

## Key features
* A visual editor for Behavior Trees, Finite State Machines and the basis to create other types of AI tools.
* AI graphs implemented as reusable scriptable objects, same instance can be run by multiple GameObjects.
* Share data between the Scene an your Behavior Tree using Blackboard variables.
* Ready implemented examples to use as guidance.
* See highlighted nodes in runtime to understand how your AI graphs are running.
* Modularize your AI graphs and execute them as part of nodes of other AI graphs.
* See more in [documentation](https://github.com/jlreymendez/planilo/wiki)

## Installing with Unity Package Manager
*(Requires Unity version 2018.3.0b7  or above)*

To install this project as a [Git dependency](https://docs.unity3d.com/Manual/upm-git.html) using the Unity Package Manager,
add the following line to your project's `manifest.json`:

```
"com.github.jlreymendez.planilo": "https://github.com/jlreymendez/planilo.git"
```
NOTE: This will also install the dependencies xNode and Unity-SerializableDictionary under the Planilo path.

You will need to have Git installed and available in your system's PATH.

## Acknowledgements:
* [xNode by Siccity](https://github.com/Siccity/xNode)
* [Unity-SerializableDictionary by azixMcAze](https://github.com/azixMcAze/Unity-SerializableDictionary)
* [Game-icons.net and Delapouite](https://game-icons.net/1x1/delapouite/choice.html)
