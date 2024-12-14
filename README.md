# Quille2 - a Life Simulation Prototype

![CC_Drives](https://github.com/user-attachments/assets/6608ea77-cbf2-442f-8209-b33041aa28d9)

The early prototype in progress of a life simulator developed with Unity.
This is a personal project I work on intermittently.

My primary focus is the characters. My goal is for them to have diverse and interesting personalities, impactful to their autonomous behaviours and internal logic. Their actions, interactions and moods should produce entertaining situations and stories without the player's input.

In terms of programming, I aim for modularity and “soft-coding”. I focus on systems, logic and templates rather than the final game “assets”. I make extensive use of Unity’s ScriptableObjects. I want a game designer to be able to come in and create character traits, interactions and the like without having to touch C# code.

So far, I've mostly worked on:
- The implementation of initial need and personality systems, including their interplay, serialization, and some example assets and in-game UI.
- The implementation of a system of “checks and modulators”, small objects meant to fetch specific bits of character information, and alter or evaluate it. They’re meant to be the building blocks of the character AI.
- The creation of custom editors and property drawers for most of the above.
- A simple character creation UI, containing names and personality elements. The ability to save and load external character files, written in JSON.
- A first sketch of the characters’ need management loop.
- An incomplete genetics system, partially ported from an older project.
- Miscellaneous fancies such as procedural grid generation and some post processing shaders.

My next steps will likely be:
- Implementing a few more character data structures (long term needs, miscellaneous personality quirks).
- Expanding the character creation UI for the above. Adding the ability to sort items by categories, such as general interest "domains".
- Sketching out UI for needs.
- Further modularizing character needs? (At the moment, personality elements' impact on a need are set in the need’s scriptableObject instance. This could be decoupled.)
- Sketching out a basic data model for interactions, and logic for characters’ to choose the best fit for satisfying a specific need. Stress testing once that is done.
- Investigating whether Unity's Job system could be useful / optimize the checks and modulators' action.
- More testing and tinkering with my grids. Possibly the creation of wall and room tools using their framework. 


"Quille" is French for a bowling pin. The project has this name because my little base character is similar in shape.



---




The project uses the following third party assets:
- [Amplify Shader Editor](https://amplify.pt/unity/amplify-shader-editor) (excluded from the repo)
- [Newtonsoft Json.NET Converters for Unity](https://github.com/applejag/Newtonsoft.Json-for-Unity.Converters)
- [Serialized Dictionaries by AYellowpaper](https://github.com/ayellowpaper/SerializedDictionary)
- Sprites from various creators, [see the full list here](https://github.com/RoxaneMorin/Quille2/wiki/Icon-Attributions).
