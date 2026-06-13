using System;

public partial class BlockStack
{
  class BlockStackRenderer
  {
    readonly BlockStack game;

    public BlockStackRenderer(BlockStack gameInstance)
    {
      game = gameInstance;
    }

    void PutText(int x, int y, string txt)
    {
      if (x < 0 || y < 0) return;
      if (x >= Console.BufferWidth || y >= Console.BufferHeight) return;
      Console.SetCursorPosition(x, y);
      Console.Write(txt);
    }

    (int x, int y) MapIToXY(int i)
      => (i % game.tW, i / game.tW);

    void DrawBlock(string c = "x")
    {
      game.blockH = game.blocks[game.activeBlockId][game.rotationId].GetLength(0);
      game.blockW = game.blocks[game.activeBlockId][game.rotationId].GetLength(1);
      for (int y = 0; y < game.blockH; y++)
      {
        for (int x = 0; x < game.blockW; x++)
        {
          if (game.blocks[game.activeBlockId][game.rotationId][y, x])
            PutText(x + game.blockLeft + game.offsetLeft + 1, y + game.blockTop + game.offsetTop, c);
        }
      }
    }

    void DrawDebug()
    {
      PutText(17, 0, $"blockTop: {game.blockTop} blockLeft: {game.blockLeft} rotId: {game.rotationId} canDraw {game.canDraw}");
      PutText(17, 1, $"tW: {game.tW} tH: {game.tH} offsetTop: {game.offsetTop} offsetLeft: {game.offsetLeft}");
      PutText(17, 2, $"ColType: {game.collisionType} Collision: {game.Collision()}");
      PutText(17, 4, "Matrix: .=empty o=test X=hit");

      for (int y = 0; y < game.blockH; y++)
      {
        string row = "";
        for (int x = 0; x < game.blockW; x++)
        {
          row += game.collisionMatrix[y, x];
        }
        PutText(17, 5 + y, row);
      }
      for (int i = 0; i < game.tW; i++)
      {
        PutText(game.offsetLeft + i + 1, game.offsetTop + game.tH, i + "");
      }
      for (int i = 0; i < game.tH; i++)
      {
        PutText(game.offsetLeft + game.tW + 2, game.offsetTop + i, $"{i}");
      }
    }

    public void Draw()
    {
      if (!game.canDraw) return;
      Console.Clear();

      for (int i = 0; i < game.tH; i++)
      {
        PutText(game.offsetLeft, game.offsetTop + i, "|");
        PutText(game.offsetLeft + game.tW + 1, game.offsetTop + i, "|");
      }

      DrawBlock();

      for (int i = 0; i < game.tMap.Length; i++)
      {
        if (game.tMap[i])
        {
          var (x, y) = MapIToXY(i);
          PutText(x + game.offsetLeft + 1, y + game.offsetTop, "o");
        }
      }

      if (game.debug) DrawDebug();
      game.canDraw = false;
    }
  }
}
