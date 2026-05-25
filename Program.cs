using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

public class Tetris
{
  List<List<bool[,]>> blocks;
  int tH = 10, tW = 10, offsetLeft = 2, offsetTop = 4;
  bool[] tMap;
  int rotationId = 0, activeBlockId = 0;
  int blockH, blockW, blockLeft = 4;
  public int blockTop = 0;
  public bool exit = false;
  bool canDraw = true;
  Random rng;
  bool debug = false; // this is the flag to show debug

  enum CollisionType {Wall, Cell, Floor};
  CollisionType collisionType;
  string[,] collisionMatrix = new string[0, 0];

  void NewBlock()
  {
    activeBlockId = rng.Next(blocks.Count); // 0..blocks.Count-1
    rotationId = 0;
    blockTop = 0;
    blockLeft = (tW / 2) - 1; // or center based on block width
  }

  // Next block will convert the 2d block into 1d for the map and store it in an 
  // array which will help to figure out collisions and should simplify drawing
  void NextBlock()
  {
    blockH = blocks[activeBlockId][rotationId].GetLength(0);
    blockW = blocks[activeBlockId][rotationId].GetLength(1);
    for (int y = 0; y < blockH; y++)
    {
      for (int x = 0; x < blockW; x++)
      {
        if (blocks[activeBlockId][rotationId][y, x])
          MapPutAtXY(x + blockLeft, y + blockTop);
      }
    }
    NewBlock();
    return;
  }
  void DrawBlock(string c = "x")
  {
    blockH = blocks[activeBlockId][rotationId].GetLength(0);
    blockW = blocks[activeBlockId][rotationId].GetLength(1);
    for (int y = 0; y < blockH; y++)
    {
      for (int x = 0; x < blockW; x++)
      {
        if (blocks[activeBlockId][rotationId][y, x])
          putText(x + blockLeft + offsetLeft, y + blockTop + offsetTop, c);
      }
    }
  }


  bool Collision()
  {
    var shape = blocks[activeBlockId][rotationId];
    blockH = shape.GetLength(0);
    blockW = shape.GetLength(1);
    collisionMatrix = new string[blockH, blockW];
    bool hasCollision = false;
    bool collisionTypeSet = false;

    for (int y = 0; y < blockH; y++)
    {
      for (int x = 0; x < blockW; x++)
      {
        if (!shape[y, x])
        {
          collisionMatrix[y, x] = ".";
          continue;
        }
        collisionMatrix[y, x] = $"[{x + blockLeft} : {y + blockTop}] ";

        int mapX = blockLeft + x;
        int mapY = blockTop + y;

        if (mapX <= 0 || mapX >= tW)
        {
          collisionMatrix[y, x] = "X";
          hasCollision = true;
          if (!collisionTypeSet)
          {
            collisionType = CollisionType.Wall;
            collisionTypeSet = true;
          }
          continue;
        }

        if (mapY >= tH) 
        {
          collisionMatrix[y, x] = "X";
          hasCollision = true;
          if (!collisionTypeSet)
          {
            collisionType = CollisionType.Floor;
            collisionTypeSet = true;
          }
          continue;
        }

        if (mapY < 0)
        {
          continue;
        }


        int i = tW * mapY + mapX;
        if (tMap[i])
        {
          collisionMatrix[y, x] = "X";
          hasCollision = true;
          if (!collisionTypeSet)
          {
            collisionType = CollisionType.Cell;
            collisionTypeSet = true;
          }
        }
      }
    }
    return hasCollision;
  }



  void MapPutAtXY(int x, int y, bool val = true)
  {
    int i = tW * y + x;
    if (i < 0 || i >= tMap.Length)
    {
      // do nothing
      return;
    }
    if (tMap[i] != val)
    {
      tMap[i] = val;
    }
  }


  (int x, int y) MapIToXY(int i)
      => (i % tW, i / tW);


  public void DrawDebug()
  {
    putText(17, 0, $"blockTop: {blockTop} blockLeft: {blockLeft} rotId: {rotationId} canDraw {canDraw}");
    putText(17, 1, $"tW: {tW} tH: {tH} offsetTop: {offsetTop} offsetLeft: {offsetLeft}");
    putText(17, 2, $"ColType: {collisionType} Collision: {Collision()}");
    putText(17, 4, "Matrix: .=empty o=test X=hit");
    
    for (int y = 0; y < blockH; y++)
    {
      string row = "";
      for (int x = 0; x < blockW; x++)
      {
        row += collisionMatrix[y, x];
      }
      putText(17, 5 + y, row);
    }
     for (int i = 0; i < tW; i++)
    {
      putText(offsetLeft + i + 1, offsetTop + tH, i + "");
    }
    for (int i = 0; i < tH; i++)
    {
      putText(offsetLeft + tW + 2, offsetTop + i, $"{i}");
    }
  
  }

  public void Draw()
  {
    if (!canDraw) return;
    Console.Clear(); // this could be more specific to reduce blinking
    // frame
    for (int i = 0; i < tH; i++)
    {
      putText(offsetLeft , offsetTop + i, "║");
      putText(offsetLeft + tW + 1, offsetTop + i, $"║");
    }
        
    DrawBlock();
    // map
    for (int i = 0; i < tMap.Length; i++)
    {
      if (tMap[i])
      {
        var (x, y) = MapIToXY(i);
        putText(x + offsetLeft, y + offsetTop, "o");
      }
    }
    if (debug) DrawDebug();
// score
  
    canDraw = false;
  }

  void putText(int x, int y, string txt)
  {
    if (x < 0 || y < 0) return;
    if (x >= Console.BufferWidth || y >= Console.BufferHeight) return;
    Console.SetCursorPosition(x, y);
    Console.Write(txt);
  }

  public void MoveBlock()
  {
    blockTop++;
    canDraw = true;
  }
  public void HandleInput()
  {
    if (Console.KeyAvailable)
    {
      ConsoleKeyInfo key = Console.ReadKey(true);

      if (key.Key == ConsoleKey.A)
      {
        blockLeft = blockLeft - 1;
        if (Collision() && collisionType == CollisionType.Wall) blockLeft += 1;
      }
      if (key.Key == ConsoleKey.D)
      {
        blockLeft = blockLeft + 1;
        if (Collision() && collisionType == CollisionType.Wall) blockLeft -= 1;
      }
      if (key.Key == ConsoleKey.S || key.Key == ConsoleKey.Spacebar)
      {
        int prevRotId = rotationId;
        rotationId = (rotationId + 1) % blocks[activeBlockId].Count;
        if (
          Collision() && collisionType == CollisionType.Wall ||
          Collision() && collisionType == CollisionType.Cell
        )
        {
          rotationId = prevRotId;
        }
      }
      canDraw = true;
    }
  }

  bool HasReachedBottom()
  {
    return (Collision()
       && (collisionType == CollisionType.Cell
             || collisionType == CollisionType.Floor)
     );
  }

  bool WillReachBottom()
  {
    blockTop += 1;
    bool check = (Collision()
       && (collisionType == CollisionType.Cell
             || collisionType == CollisionType.Floor)
     );
     blockTop -= 1;
     return check;
  }

  void CheckProgress()
  {
    if (HasReachedBottom())
    {
      // blockTop -= 1;
      NextBlock();
    }
  }

  public Tetris()
  {
    rng = new Random();
    tMap = new bool[tH * tW];
    blocks = TetrisBlocks.Create();
  }

  public void Run()
  {
    while (!exit)
    {
      HandleInput();
      // CheckProgress();
      if (WillReachBottom()) NextBlock();
      else MoveBlock(); // if this happens before check, check can reverse it
      Draw();
      Thread.Sleep(200);
    }
  }

}

class TetrisRenderer
{
  Tetris tetris;
  public TetrisRenderer(Tetris t)
  {
    tetris = t;
  }

  public void Draw()
  {

  }
}

class Program
{
  static void Main(string[] args)
  {
    Tetris t = new Tetris();
    t.Run();
  }
}
