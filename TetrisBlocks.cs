using System.Collections.Generic;

public static class TetrisBlocks
{
  public static List<List<bool[,]>> Create()
  {
    return new()
    {
      // I
      new()
      {
        new bool[,]
        {
          { false, false, false, false },
          { true , true , true , true  },
          { false, false, false, false },
          { false, false, false, false },
        },

        new bool[,]
        {
          { false, true, false, false },
          { false, true, false, false },
          { false, true, false, false },
          { false, true, false, false },
        }
      },

      // O
      new()
      {
        new bool[,]
        {
          { true, true },
          { true, true },
        }
      },

      // T
      new()
      {
        new bool[,]
        {
          { false, true , false },
          { true , true , true  },
          { false, false, false },
        },

        new bool[,]
        {
          { false, true , false },
          { false, true , true  },
          { false, true , false },
        },

        new bool[,]
        {
          { false, false, false },
          { true , true , true  },
          { false, true , false },
        },

        new bool[,]
        {
          { false, true , false },
          { true , true , false },
          { false, true , false },
        }
      },

      // S
      new()
      {
        new bool[,]
        {
          { false, true , true  },
          { true , true , false },
          { false, false, false },
        },

        new bool[,]
        {
          { false, true , false },
          { false, true , true  },
          { false, false, true  },
        }
      },

      // Z
      new()
      {
        new bool[,]
        {
          { true , true , false },
          { false, true , true  },
          { false, false, false },
        },

        new bool[,]
        {
          { false, false, true  },
          { false, true , true  },
          { false, true , false },
        }
      },

      // J
      new()
      {
        new bool[,]
        {
          { true , false, false },
          { true , true , true  },
          { false, false, false },
        },

        new bool[,]
        {
          { false, true , true  },
          { false, true , false },
          { false, true , false },
        },

        new bool[,]
        {
          { false, false, false },
          { true , true , true  },
          { false, false, true  },
        },

        new bool[,]
        {
          { false, true , false },
          { false, true , false },
          { true , true , false },
        }
      },

      // L
      new()
      {
        new bool[,]
        {
          { false, false, true  },
          { true , true , true  },
          { false, false, false },
        },

        new bool[,]
        {
          { false, true , false },
          { false, true , false },
          { false, true , true  },
        },

        new bool[,]
        {
          { false, false, false },
          { true , true , true  },
          { true , false, false },
        },

        new bool[,]
        {
          { true , true , false },
          { false, true , false },
          { false, true , false },
        }
      }
    };
  }
}
