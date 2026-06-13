using System;
using System.Collections.Generic;
using System.Threading;

public partial class BlockStack
{
  List<List<bool[,]>> blocks;
  BlockStackRenderer renderer;
  int tH = 10, tW = 10, offsetLeft = 2, offsetTop = 4;
  bool[] tMap;
  int rotationId = 0, activeBlockId = 0;
  int blockH, blockW, blockLeft = 4;
  public int blockTop = 0;
  public bool exit = false;
  bool canDraw = true;
  Random rng;
  bool debug = false; // this is the flag to show debug
  const int fallDelayMs = 400;

  enum CollisionType { Wall, Cell, Floor };
  CollisionType collisionType;
  string[,] collisionMatrix = new string[0, 0];

  void NewBlock()
  {
    activeBlockId = rng.Next(blocks.Count); // 0..blocks.Count-1
    rotationId = 0;
    blockTop = 0;
    blockLeft = (tW / 2) - 1; // or center based on block width
  }

  void ClearInputBuffer()
  {
    while (Console.KeyAvailable)
    {
      Console.ReadKey(true);
    }
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
    ClearInputBuffer();
    return;
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

        if (mapX < 0 || mapX >= tW)
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

  public void MoveBlock()
  {
    blockTop++;
    canDraw = true;
  }

  public void HandleInput()
  {
    bool changed = false;
    while (Console.KeyAvailable)
    {
      ConsoleKeyInfo key = Console.ReadKey(true);

      if (key.Key == ConsoleKey.A)
      {
        blockLeft = blockLeft - 1;
        if (Collision() && collisionType == CollisionType.Wall) blockLeft += 1;
        changed = true;
      }
      else if (key.Key == ConsoleKey.D)
      {
        blockLeft = blockLeft + 1;
        if (Collision() && collisionType == CollisionType.Wall) blockLeft -= 1;
        changed = true;
      }
      else if (key.Key == ConsoleKey.S || key.Key == ConsoleKey.Spacebar)
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
        changed = true;
      }
    }

    if (changed) canDraw = true;
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

  public BlockStack()
  {
    rng = new Random();
    tMap = new bool[tH * tW];
    blocks = BlockStackBlocks.Create();
    renderer = new BlockStackRenderer(this);
    Console.CursorVisible = false;
  }

  void CheckLines()
  {
    for (int y = tH - 1; y >= 0; y--)
    {
      bool full = true;
      for (int x = 0; x < tW; x++)
      {
        if (!tMap[y * tW + x])
        {
          full = false;
          break;
        }
      }

      if (!full) continue;

      for (int row = y; row > 0; row--)
      {
        for (int x = 0; x < tW; x++)
        {
          tMap[row * tW + x] = tMap[(row - 1) * tW + x];
        }
      }

      for (int x = 0; x < tW; x++)
      {
        tMap[x] = false;
      }

      y++;
      canDraw = true;
    }
  }

  public void Run()
  {
    DateTime lastFall = DateTime.UtcNow;
    while (!exit)
    {
      HandleInput();
      if ((DateTime.UtcNow - lastFall).TotalMilliseconds >= fallDelayMs)
      {
        lastFall = DateTime.UtcNow;
        if (WillReachBottom()) {
          NextBlock();
          CheckLines();
        }
        else MoveBlock(); // if this happens before check, check can reverse it
      }
      renderer.Draw();
      Thread.Sleep(16);
    }
  }
}

class Program
{
  static void Main(string[] args)
  {
    BlockStack game = new BlockStack();
    game.Run();
  }
}
