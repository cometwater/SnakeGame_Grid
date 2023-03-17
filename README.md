# SnakeGame_Grid

1. This solution uses 2D grids to represent the whole gameboard and the each grid can represent a snake piece, an apple, a boarder piece or an empty spot, so there is no actual "snake body".
2. On each update, the grid where the snake head is going to move to is "added" to the snake as its new head, and the tail gets "removed". 
2. The snake detects what it runs into by checking the value of the grid it's going to to move to.

## How the snake moves

Every snake consists of a list of grids with value of "SnakePiece". On every update, the snake head will first check what value the grid in front of it has. If it is an available grid, it adds this grid as the new head and removes its tail. If it is an apple, it will adds this grid as the new head without removing the tail. If it is an unavailable(boarder) grid or a snake piece, it dies. It doesn't need to track the direction for each snake piece, because the head piece is always added towards the current direction.

## What each script does

**GameManager** controls the game. 
> It takes the size (i.e. the number of rows and columns) of the game board and generate a boader for the game board. 

> It has a list to track the value of all the grids of the game board.

> It generates all the snakes at the center of the board and initializes them. 

> It has a list to keep track of all the grids available on the board where the apple can be spawned. 

> It spawns an apple on the board.

> It is also in charge of displaying some text and contains functions to replay or quit the game. 

> You can also change the number of snakes, initial length of the snakes, the speed of the snakes and the colors of the snakes.

**Snake** controls a snake and its snake pieces. 
> On every update, the snake "add" a piece to the head and "remove" the tail by updating the value of the corresponding grids, if the grid ahead of the snake is available. The snake gets destroyed if the spots ahead is unavailable or a snake piece. If the spot ahead is an apple, the snake will only add a piece at the head and will not remove the tail piece. 

> When the corresponding direction keys are pressed, the snake will update its moving direction based on the input. 

> When the snake dies, it will display text.

**MapGrid** is responsible for updating the color of the grid it attaches to to display the status of the grid.
