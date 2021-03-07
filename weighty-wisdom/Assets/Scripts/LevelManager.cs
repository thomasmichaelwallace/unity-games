public static class LevelManager
{
    // each level is defined as:
    //  first line: starting position (row, column) from bottom left and;
    //  second line+: an ascii art layout, one character per tile:
    //   - [ ]: empty space
    //   - [1-9]: tile that breaks after no. steps
    //   - [0]: invincible tile
    //   - [*]: invincible tile + book
    //   - [#]: invincible tile + shelf
    // final line is trimmed, to give layout segment consistent indent.
    public static readonly string[] Levels =
    {
        // level 1
        @"0,0
        #
        0
        3
0111*222*
",
        // level 2
        @"0,0
  *2
# 12
0232
  1
011
",
        // level 3
        @"0,0
 #   #
 0   0
 *222*
   1
0111 
",
        // level 4
        @"1,0
     879*
#    7
05587879*
0  7
   87779*
",
        // level 5
        @"0,2
   #
   1
#  *22
2  2 2
2203*2#
",
        // level 6
        @"1,1
#
000
  2313*
  1214
  2122
 03212 #
     000
",
        // level 7
        @"6,3
   0222
   *  2
#  2  3333
4  2  3  3
444422522*
 3    3
 344443
",
        // level 8
        @"1,0
 #
 51115*
 33432
 23152
 21453
051125
     *
",
        // level 9
        @"4,2
  #
 000
*741
 347*
*742
 337*
",
        // level 10
        @"0,0
00332121200
0 1 2 3 2 0
0 3121222 0 #
* 3 2*2 3 000
0 2131232 0
0 2 2 2 2 0
00213331300
"
    };

    public static int LevelNo = 1; // DEBUG: Levels.Length;
}