# Swinging Delivery

Wing it or swing it being the coolest Delivery man with a grappling hook


Prototype phase (Test of idea)

Use your mouse to play.

(Not an entry to the [mix-and-game-jam](https://itch.io/jam/mix-and-game-jam))

It's inspired by spiderman. I wanted to demake it in 2d, as a practice.  
As a twist, i try to make the whole game playable with just 1 finger (1 mouse in the web version)

## Basic Controls

Please try to only use basic controls to move around before reading Advanced Controls

- Left-Mouse-Click-hold = Swing
- Left-Mouse-Click, release quickly = Dash

## Advanced Controls
- When standing, quick tap below player to Hop forward in the direction of click
- Release Swing at the very end to get a height boost
- Input window of dash is indicated by the white line connecting the tip of the grapple

## Setup

1. Clone `_ContentfulKey.example.cs` as `_ContentfulKey.cs`
2. In `_ContentfulKey.cs`, rename class `_ContentfulKeyExample` to `_ContentfulKey`

## TODO
- (DONE) Wall run, platform run
- One-way platforms
- Bigger world
- Delivery-based mission objectives

# Credits
- Sebastian Lague  (https://www.youtube.com/watch?v=MbWK8bCAU2w&list=PLFt_AvWsXl0f0hqURlhyIoAabKPgRsqjz) for its excellent unity tutorial. I have followed it several times, so this time i directly copied from its github repo (https://github.com/SebLague/2DPlatformer-Tutorial) and made heavy original changes to the code
- GMTK (https://www.youtube.com/watch?v=DKSpE2PGJjI) for footages and excellent commentary
- Kenney (Kenney.nl) for graphics
- Various math and physics tutorials, although i finally gave up real physics and went for semi-lerping instead
- Unity. A very nice engine for OOP-compositions. The editor crashed in the middle of development though
