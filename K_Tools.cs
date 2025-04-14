using System;
using System.IO;
using System.Text.Json;

namespace Kociemba;

public class Tools
{
    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Check if the cube string s represents a solvable cube.
    // 0: Cube is solvable
    // -1: There is not exactly one facelet of each colour
    // -2: Not all 12 edges exist exactly once
    // -3: Flip error: One edge has to be flipped
    // -4: Not all corners exist exactly once
    // -5: Twist error: One corner has to be twisted
    // -6: Parity error: Two corners or two edges have to be exchanged
    // 
    /// <summary>
    /// Check if the cube definition string s represents a solvable cube.
    /// </summary>
    /// <param name="s"> is the cube definition string , see <seealso cref="Facelet"/> </param>
    /// <returns> 0: Cube is solvable<br>
    ///         -1: There is not exactly one facelet of each colour<br>
    ///         -2: Not all 12 edges exist exactly once<br>
    ///         -3: Flip error: One edge has to be flipped<br>
    ///         -4: Not all 8 corners exist exactly once<br>
    ///         -5: Twist error: One corner has to be twisted<br>
    ///         -6: Parity error: Two corners or two edges have to be exchanged </returns>
    public static int verify(string s, out CubieCube cc)
    {
        cc = null;
        int[] count = new int[6];
        try
        {
            for (int i = 0; i < 54; i++)
            {
                count[(int)Enum.Parse<CubeColor>(s[i].ToString())]++;
            }
        }
        catch (Exception)
        {
            return -1;
        }

        for (int i = 0; i < 6; i++)
        {
            if (count[i] != 9)
            {
                return -1;
            }
        }

        FaceCube fc = new FaceCube(s);
        cc = fc.toCubieCube();

        return cc.verify();
    }

    /// <summary>
    /// Generates a random cube. </summary>
    /// <returns> A random cube in the string representation. Each cube of the cube space has the same probability. </returns>
    public static string randomCube()
    {
        CubieCube cc = new CubieCube();
        Random gen = new Random();
        cc.setFlip((short)gen.Next(CoordCube.N_FLIP));
        cc.setTwist((short)gen.Next(CoordCube.N_TWIST));
        do
        {
            cc.setURFtoDLB(gen.Next(CoordCube.N_URFtoDLB));
            cc.setURtoBR(gen.Next(CoordCube.N_URtoBR));
        } while ((cc.edgeParity() ^ cc.cornerParity()) != 0);
        FaceCube fc = cc.toFaceCube();
        return fc.to_fc_String();
    }

    private const string SerializationFolder = @"Assets\Kociemba\Tables\";

    public static void SerializeTable(string filename, short[,] array)
    {
        Serialize(filename, Table.FromArray(array));
    }

    public static short[,] DeserializeTable(string filename)
    {
        return Deserialize<Table>(filename).ToArray2d();
    }

    public static void SerializeSbyteArray(string filename, sbyte[] array)
    {
        Serialize(filename, array);
    }

    public static sbyte[] DeserializeSbyteArray(string filename)
    {
        return Deserialize<sbyte[]>(filename);
    }

    private static void Serialize<T>(string filename, T value)
    {
        Directory.CreateDirectory(SerializationFolder);
        File.WriteAllBytes(SerializationFolder + filename + ".json", JsonSerializer.SerializeToUtf8Bytes(value));
    }
    private static T Deserialize<T>(string filename)
    {
        Directory.CreateDirectory(SerializationFolder);
        return JsonSerializer.Deserialize<T>(File.ReadAllBytes(SerializationFolder + filename + ".json"));
    }

    // helper class to json serialize 2d array
    private record Table(int X, int Y, short[] Array1d)
    {
        public static Table FromArray(short[,] array2d)
        {
            int x = array2d.GetLength(0);
            int y = array2d.GetLength(1);
            Table table = new(x, y, new short[x * y]);
            Buffer.BlockCopy(array2d, 0, table.Array1d, 0, 2 * table.Array1d.Length);
            return table;
        }

        public short[,] ToArray2d()
        {
            short[,] array2d = new short[X, Y];
            Buffer.BlockCopy(Array1d, 0, array2d, 0, 2 * Array1d.Length);
            return array2d;
        }
    }
}