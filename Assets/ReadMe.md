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
        - call GetCharacterOnBoard and change the status of isAlive and ownership
      - void DefineId()
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
    - int id GetCharacterOnBoard(int x, y)

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
