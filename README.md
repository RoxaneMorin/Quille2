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
- An incomplete genetics system, partially ported from an older project.
- Miscellaneous fancies such as procedural grid generation and some post processing shaders.

My next steps will likely be:
- Implementing a few more character data structures (long term needs, miscellaneous personality quirks).
- Sketching out more of the personality creation UI for ease of testing.
- Further modularizing character needs? (At the moment, personality elements' impact on a need are set in the need’s scriptableObject instance. This could be decoupled.)
- Sketching out a basic data model for interactions, and logic for characters’ to choose the best fit for satisfying a specific need. Stress testing once that is done.
- More testing and tinkering with my grids. Possibly the creation of wall and room tools using their framework. 


"Quille" is French for a bowling pin. The project has this name because my little base character is similar in shape.



---




The project uses the following third party assets:
- [Amplify Shader Editor](https://amplify.pt/unity/amplify-shader-editor) (excluded from the repo)
- [Newtonsoft Json.NET Converters for Unity](https://github.com/applejag/Newtonsoft.Json-for-Unity.Converters)
- [Serialized Dictionaries by AYellowpaper](https://github.com/ayellowpaper/SerializedDictionary)
- Sprites from various creators, mostly downloaded from [flaticon](https://www.flaticon.com/) and [freekpik](https://www.freepik.com/):

  -> [Flat Icons](https://www.flaticon.com/authors/flat-icons)

  -> [Freepik](https://www.flaticon.com/authors/freepik)

  -> [Parzival' 1997](https://www.flaticon.com/authors/parzival-1997)

  -> [KosonIcon](https://www.flaticon.com/authors/kosonicon)
  
  -> [SmashIcons](https://www.flaticon.com/authors/smashicons)

  -> [Juicy-Fish](https://www.flaticon.com/authors/juicy-fish)

  -> [SmashingStocks](https://www.flaticon.com/authors/smashingstocks)

  -> [Jagat Icon](https://www.flaticon.com/authors/jagat-icon)

  -> [Moudesain](https://www.flaticon.com/authors/moudesain)

  -> [Nuricon](https://www.flaticon.com/authors/nuricon)

  -> [UniconLabs](https://www.flaticon.com/authors/uniconlabs)

  -> [kmg design](https://www.flaticon.com/authors/kmg-design)

  -> [VectorsLab](https://www.flaticon.com/authors/vectorslab)

  -> [MythArt](https://www.freepik.com/author/user24791284/icons)

  -> [IconPlace](https://www.freepik.com/author/iconplace/icons)
  
  -> [DonieIndo](https://www.shutterstock.com/g/donieindo)

  -> [Cutey and Kdawg on CleanPNG](https://www.cleanpng.com/)
