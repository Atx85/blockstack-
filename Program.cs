using System;
using System.Collections.Generic;
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

  enum CollisionType {Wall, Cell, Floor};
  CollisionType collisionType;

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
        if (blocks[activeBlockId][rotationId][x, y])
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
        if (blocks[activeBlockId][rotationId][x, y])
          putText(x + blockLeft + offsetLeft, y + blockTop + offsetTop, c);
      }
    }
  }


  bool Collision()
  {
    var shape = blocks[activeBlockId][rotationId];
    blockH = shape.GetLength(0);
    blockW = shape.GetLength(1);

    for (int y = 0; y < blockH; y++)
    {
      for (int x = 0; x < blockW; x++)
      {
        if (!shape[y, x])
        {
          continue;
        }

        int mapX = blockLeft + x;
        int mapY = blockTop + y;

        if (mapX < 0 || mapX > tW)
        {
          collisionType = CollisionType.Wall;
          return true;
        }

        // if (mapX < 0 || mapX > tW || mapY < 0 || mapY >= tH)
        if (mapY >= tH)
        {
          collisionType = CollisionType.Floor;
          return true;
        }


        int i = tW * mapY + mapX;
        if (tMap[i])
        {
          collisionType = CollisionType.Cell;
          return true;
        }
      }
    }
    return false;
  }



  void MapPutAtXY(int x, int y, bool val = true)
  {
    int i = tW * y + x;
    if (i < 0 || i > tMap.Length)
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



  public void Draw()
  {
    if (!canDraw) return;
    Console.Clear(); // this could be more specific to reduce blinking
    // frame
    for (int i = 0; i < tH; i++)
    {
      putText(offsetLeft, offsetTop + i, "|");
      putText(offsetLeft + tW, offsetTop + i, "|");
    }
    DrawBlock();
    if (Collision() 
          && (collisionType == CollisionType.Cell
                || collisionType == CollisionType.Floor)
        ) NextBlock();
    // map
    for (int i = 0; i < tMap.Length; i++)
    {
      if (tMap[i])
      {
        var (x, y) = MapIToXY(i);
        putText(x + offsetLeft, y + offsetTop, "o");
      }
    }

// score
    putText(15, 0, $"blockTop: {blockTop} blockLeft: {blockLeft} rotId: {rotationId}");
    putText(15, 1, $"tW: {tW} tH: {tH} offsetTop: {offsetTop} offsetLeft: {offsetLeft}");
    putText(15, 2, $"ColType: {collisionType} Collision: {Collision()}");
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
    if (tH - blockH > blockTop)
    {
      blockTop++;
    }
    canDraw = true;
  }
  public void HandleInput()
  {
    if (Console.KeyAvailable)
    {
      ConsoleKeyInfo key = Console.ReadKey(true);

      if (key.Key == ConsoleKey.A)
      {
        blockLeft = Math.Max(0, blockLeft - 1);
      }
      if (key.Key == ConsoleKey.D)
      {
        blockLeft = blockLeft + 1;
        if (Collision() && collisionType == CollisionType.Wall) blockLeft -= 1;
      }
      if (key.Key == ConsoleKey.S || key.Key == ConsoleKey.Spacebar)
      {
        rotationId = (rotationId + 1) % blocks[activeBlockId].Count;
      }
      canDraw = true;
    }
  }

  public Tetris()
  {
    rng = new Random();
    tMap = new bool[tH * tW];
    blocks = TetrisBlocks.Create();
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
    while (!t.exit)
    {
      t.HandleInput();
      t.Draw();
      Thread.Sleep(200);
      t.MoveBlock();
    }
  }
}
