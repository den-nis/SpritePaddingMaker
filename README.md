# SpritePaddingMaker

Simple command line tool to create padding around tiles in a spritesheet. 

![example](https://i.imgur.com/hIubSy7.png)

For some reason graphics libraries like to read pixels from neighbouring tiles in spritesheets when working with floating point numbers, you can either try to fix that or add padding around each tile.

# Usage
```
SpritePaddingMaker {image} {tileWidth}x{tileHeight} {paddingWidth}x{paddingHeight}
```
```
SpritePaddingMaker Example.png 32x32 1x1
```
