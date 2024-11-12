# Quille2
 
The early prototype in progress of a life simulator developed with Unity.
This is a personal project I work on intermittently.

My primary focus is the characters. My goal is for them to have diverse and interesting personalities, impactful to their autonomous behaviours and internal logic. Their actions, interactions and moods should produce entertaining situations and stories without the player's input.

In terms of programming, I aim for modularity and “soft-coding”. I focus on systems, logic and templates rather than the final game “assets”. I make extensive use of Unity’s ScriptableObjects. I want a game designer to be able to come in and create character traits, interactions and the like without having to touch C# code.

So far, I've mostly worked on:
- The implementation of initial need and personality systems, including their interplay, serialization, and some example assets and in-game UI.
- The implementation of a system of “checks and modulators”, small objects meant to fetch specific bits of character information, and alter or evaluate it. They’re meant to be the building blocks of the character AI.
- The creation of custom editors and property drawers for most of the above.
- A first sketch of the characters’ need management loop.
- Miscellaneous fancies such as procedural grid generation and some post processing shaders.

My next steps will likely be:
- Further testing of everything’s Json serialization.
- Further modularizing character needs. (At the moment, a personality element’s impact on a need is set in the need’s scriptableObject instance.)
- Sketching out a basic data model for interactions, and logic for characters’ to choose the best fit for satisfying a specific need. Stress testing once that is done.
- More testing and tinkering with my grids. Possibly the creation of wall and room tools using their framework. 


"Quille" is French for a bowling pin. The project has this name because my little base character is similar in shape.


The project uses the following third party assets:
- Amplify Shader Editor
- Newtonsoft Json.NET Framework
- Newtonsoft Json.NET Converters for Unity
- Serialized Dictionaries by AYellowpaper
- Sprites from various FlatIcon.com creators
