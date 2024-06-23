## Specification

### Normal Shogi

#### Players can move one character of their choice each turn

- The position from which they can move depends on the role of the character.
  - hohei (roleId 00)
  - kyousha (roleId 01)
  - keuma (roleId 02)
  - gin (roleId 03)
  - kin (roleId 04)
  - kaku (roleId 05)
  - hisha (roleId 06)
  - oh (roleId 07)
  - nari-kin (roleId 14)
  - nari-kaku (roleId 15)
  - nari-hisha (roleId 16)
- Character can move by their range of move point
  - if player face to the other character or deadend, the following will happen depend on the following situation
    - deadend: character stop at the deadend
    - enemy character: character stop at the enemy character point
    - player character: player stom at the point before the faced character
- Each players turn has time limit
- Player who lost their oh is lose, the other player win
- if character stop at the enemy character position, enemy character dissapear from the board and the ownership take over to the playe who command character stop at the enemy position.

### Required Component

- GameManager.cs

  - For controling general stuff
    - Timer: for decision-making time limits on each player's turn.
  - Property:

    - bool isMasterTurn
    - bool isTurnPlayerMoved
    - bool isMasterWin
    - bool isMasterLose

  - Method:
    - void CountDownTimer()
      - countdown timer while isTurnPlayerMoved != true
    - void ChangeTurnPlayer()
      - Check oh is alive
        - if no, GameFinish
      - This method will work when isTurnPlayerMoved =true
    - void ChangeCameraForTurnPlayer()
      - Enable turn player's camera and disable other player's camera

- RangeOfMovementOnBoard.cs
  - For define the range of move each character
    - Method:
      - (int x, int y)[] IndicateMovablePosition(int[,] currentPos, Role role, bool isMasterPlayer)
        - Input: Arbitrary coordinates
        - Output: MovablePos of the role
        - This method does not return the coordinate if the MovablePosition is on the out of board
        - This method does not return a position beyond the obstructing character if a character is on their way.
          - return the before position if the obstructing character is the same owner as the character which is moving
          - return the same position if the obstructing character is the different owner as the character which is moving
      - void DisplayMovablePos(int x, int y)
        - Change Parameter of BoardInfo's peoperty by input
- CharacterModel.cs
  - This is the class for role and management info of master player tapped, client player tapped and ownership
    - Property:
      - int id: roleNo + 2DigitFirstCoordinate(+ coordinate: 0, - coordinate: 1 | ex (1,3) => 0103, ex (-2,3) => 1203) + firstOwner(0: master / 1: client)
      - bool isMasterPlayerTapped
      - bool isClientPlayerTapped
      - bool hasMasterOwnership
      - bool isAlive
      - role
    - Method:
      - (int x, int y)[] RangeOfMovementAsRole(string role)
        - Output is gonna be the movablePoint from origin(0,0)
      - void Move(int x, int y)
        - Change the coorinate of this character
        - Chnage the isTurnPlayerMoved true
      - void Attack()
        <!-- - call GetCharacterOnBoard -->
        - and change the status of isAlive and ownership
        <!-- - void DefineId() -->
      - int[,] GetCurrentPos()
- BoardManager.cs

  - For define board information
    - the coordinate of board from (-4,-4) to (4,4)
  - Detect touch of the board
  - Property:

    - bool isMasterTapped
    - bool isClientTapped
    - int[,] masterTappedPos
    - int[,] clientTappedPos

  - Method:
    - void DetectToch(bool isMaster, bool isClient, (int x, int y)coordinate)
      - Change the status of each BoardInfo as isMasterTapped/isClientTapped to false except input coordinate
      <!-- - int id GetCharacterOnBoard(int x, y) -->

- BoardInfo.cs
  - Manage each board info
  - Property:
    - isMasterTapped
    - isClientTapped
    - isMovablePos
    - isCurrentPos
  - Method:
    - void ColorPos(bool isMasterTapped, bool isClientTapped, bool isMovablePos, bool isCurrentPos)
      - call Role.Move if the turnPlayer(Master/Client) tapped is true && isMovablePos is true
      - change color as current pos if the isCurrentPos true && (isMasterTapped== true || isClientTapped ==true)
      - change color as isMovablePos if the isMovablePos true
    - void NoticeTapped(bool isMaster, int[,] pos)
    - ChangeGridInfo
      - OnTriggerEnter
        - if the GridSquare is hit Character, insert/update BoardInfo

---

### Implemention

#### Start()

- BoardManager.cs
  - Add all grid into gridsPosDictionary
- gridsPosDictionary magage grid data: key x,y, value: GameObject

- BoardInfo.cs
  - RegisterIntoBoardDataBank()
    - Each BoardInfo is registered into boardDataBannk: key x,y value: BoardStatus, isMovablePos?
    - The flow of this method is :
      - 1. All board are registerd as its position(x,y) with BoardStatus Empty and isMovable null
      - 2. Using Physics.OverlapBox allow to judge which type of character is contacted the board
      - 3. Depend on the type of character in 2. , baordDatabank will be updated like:
        - if Master Character is contacted: Board Status and isMovablePos are gonna be MasterCharacterExist and false(!isMasterTurn), respectively.
        - if Client Character is contacted: Board Status and isMovablePos are gonna be ClientCharacterExist and true(isMasterTurn), respectively.
          - CAUTION: the reason board status's isMovablePos is gonna be true when there was Client Character is that the first turn is for Master player. So when it comes to Client Turn, the method "RegisterIntoBoardDataBank" should be called as for register as movablePos is false when it comes to there was client character on the board.

#### The flow after the game start

##### When Charater are touched

- GameManager.cs

  - Each player turn has time limit
  - This file manage the following:
    - time for each turn
    - which player's turn

- CharacterModel.cs

  - currentPos is defined in OnTriggerStay()
    - OnTriggerStay()
      - if the character is touching any Grid:
        - this.currentPos is defined as its Grid.transform.position
        - this.boardInfo is defined as the Grid's BoardInfo Component
  - When it comes to each player try for their action, the player needs to touch any character on the board.
    - OnPointerClick()
      - The method has the following role:
        - register:
          - boardManager.TouchedChara as for GameObject
          - boardManager.CurrentPos = currentPos
          - boardManager.CharacterModelForTransfer as for CharacterModel
        - Color
          - if the boardManager.CurrentPos equals to this.currentPos, its clored as the current pos color red

- BoardManager.cs
  - When this CurrentPos is updated:
    - update lastPos by the past this.currentPos
    - this.currentPos is updated by the currentPos data from CharacterModel
  - Update All boardDataBank data
    - get each grid info from gridsPosDictionary
      - Call "RegisterIntoBoardDataBank()"
        - this time, the movablePos is registered as diffrent turn player(!gameManager.IsMasterTurn) as default ?? what the hell it might mitake because I should register turn player(gameManager.IsMasterTurn)
  - When this CharacterModelForTransfer is updated:
    - Update All movablePos false by calling "ResetAllGridAsMovableIsFalse" in "CalculateMovablePos"
      - in this case, CalculateMovablePos is called by one of the argument "isCurrent" false
        - Due to this method is called as "isCurrent" false, the movablePos are calculated by lastPos which means the last touched characterPos
    - characterModelForTransfer is updated by latest data
    - CalculateMovablePos is called by one of the argument "isCurrent" true
      - Hence, all movablePos are gonna be updated based on the latest CurrentPos
    - As last, target grid are colored by "CallChangeColorMovablePos()"

##### When mobanlePos are touched

- BoardInfo.cs

  - OnPointerClick()
    - if this.isMovablePos is true
      - Update the boardDataBank:
        - the BoardStatus are changed as the turnPlayer Chara exist
        - the movablePos is false
    - boardManager.ExpectedMovingPos are inserted by its position

- BoardManager.cs
  - this.expectedMovingPos is updated by the new value from BoardInfo.cs
  - check what charanter position is same as expectedPos and AttackingTarget is updated by expectedPos's characterModel
  - this.characterModelForTransfer Move
    - for move the character and delete the attackedCharacter
  - ResetAllGridAsMovableIsFalse()
  - CallChangeColorMovablePos
  - Update all boardDataBank based on gridsPosDictionary
