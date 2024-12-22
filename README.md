The goal of this project is to create a small dungeon-crawler type of game. Every level of the game will be procedurally generated and the randomness will be based on a seed (integer value) so that the levels can always be reproduced.

![](https://github.com/CristianDejica00/Room-Layout-Generator/blob/main/GitPres/Pres_01.gif)

We start with a grid of "cells". To generate the room shapes and the layout, we select cells randomly and merge them with some of their neighbors. 

![](https://github.com/CristianDejica00/Room-Layout-Generator/blob/main/GitPres/Pres_02.gif)

After having a complete collection of rooms, we choose a starting room, marking it as "connected". Afterwards, we select rooms that are neigbors to already connected rooms and mark a "door" between a cell from the selected room and a closeby cell from the connected room, thus connecting them with a door. The newly connected room will be marked as "connected". We do this until all rooms are marked as "connected".

![](https://github.com/CristianDejica00/Room-Layout-Generator/blob/main/GitPres/Pres_03.gif)

Now, with all rooms interconnected, we can start thinking about room contents. It is worth mentioning that the array of cells I used at the beginning has a certain size (called mapSize), and I used another variable (called roomSize) to upscale the array. So if the mapSize is 10, and the roomSize is 9, the final layout will consist of 90x90 units.

To determine the furniture placement, I spawned rectangles of width and height between 1 and 3 near walls. I did this up until the occupiable space is at 40% capacity. Afterwards, I cleared the doors by erasing rectangles that stand in the way of doorways and I filled in the spaces that would be unreachable (for example corners that have furniture around them).

![](https://github.com/CristianDejica00/Room-Layout-Generator/blob/main/GitPres/Pres_04.gif)

Afterwards, for the furniture found in the middle of rooms, I drew two types of areas: dividing walls and rectangles. To draw the dividing walls, I went from cell to cell and, after calculating the spawn chance, connected one cell to the other with a line that connects their centers. The rectangles were spawned in the remaining cells, unnocupied by said lines, after calculating the spawn chance.

![](https://github.com/CristianDejica00/Room-Layout-Generator/blob/main/GitPres/Pres_05.gif)
