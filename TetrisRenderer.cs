using System;

public partial class Tetris
{
  class TetrisRenderer
  {
    readonly Tetris tetris;

    public TetrisRenderer(Tetris t)
    {
      tetris = t;
    }

    void PutText(int x, int y, string txt)
    {
      if (x < 0 || y < 0) return;
      if (x >= Console.BufferWidth || y >= Console.BufferHeight) return;
      Console.SetCursorPosition(x, y);
      Console.Write(txt);
    }

    (int x, int y) MapIToXY(int i)
      => (i % tetris.tW, i / tetris.tW);

    void DrawBlock(string c = "x")
    {
      tetris.blockH = tetris.blocks[tetris.activeBlockId][tetris.rotationId].GetLength(0);
      tetris.blockW = tetris.blocks[tetris.activeBlockId][tetris.rotationId].GetLength(1);
      for (int y = 0; y < tetris.blockH; y++)
      {
        for (int x = 0; x < tetris.blockW; x++)
        {
          if (tetris.blocks[tetris.activeBlockId][tetris.rotationId][y, x])
            PutText(x + tetris.blockLeft + tetris.offsetLeft, y + tetris.blockTop + tetris.offsetTop, c);
        }
      }
    }

    void DrawDebug()
    {
      PutText(17, 0, $"blockTop: {tetris.blockTop} blockLeft: {tetris.blockLeft} rotId: {tetris.rotationId} canDraw {tetris.canDraw}");
      PutText(17, 1, $"tW: {tetris.tW} tH: {tetris.tH} offsetTop: {tetris.offsetTop} offsetLeft: {tetris.offsetLeft}");
      PutText(17, 2, $"ColType: {tetris.collisionType} Collision: {tetris.Collision()}");
      PutText(17, 4, "Matrix: .=empty o=test X=hit");

      for (int y = 0; y < tetris.blockH; y++)
      {
        string row = "";
        for (int x = 0; x < tetris.blockW; x++)
        {
          row += tetris.collisionMatrix[y, x];
        }
        PutText(17, 5 + y, row);
      }
      for (int i = 0; i < tetris.tW; i++)
      {
        PutText(tetris.offsetLeft + i + 1, tetris.offsetTop + tetris.tH, i + "");
      }
      for (int i = 0; i < tetris.tH; i++)
      {
        PutText(tetris.offsetLeft + tetris.tW + 2, tetris.offsetTop + i, $"{i}");
      }
    }

    public void Draw()
    {
      if (!tetris.canDraw) return;
      Console.Clear();

      for (int i = 0; i < tetris.tH; i++)
      {
        PutText(tetris.offsetLeft, tetris.offsetTop + i, "|");
        PutText(tetris.offsetLeft + tetris.tW + 1, tetris.offsetTop + i, "|");
      }

      DrawBlock();

      for (int i = 0; i < tetris.tMap.Length; i++)
      {
        if (tetris.tMap[i])
        {
          var (x, y) = MapIToXY(i);
          PutText(x + tetris.offsetLeft, y + tetris.offsetTop, "o");
        }
      }

      if (tetris.debug) DrawDebug();
      tetris.canDraw = false;
    }
  }
}
