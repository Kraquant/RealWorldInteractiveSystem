using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;


[Serializable]
public class HexCoordinates : IEquatable<HexCoordinates>
{
    public static readonly HexCoordinates[] direction_vectors = new HexCoordinates[6] {
        new HexCoordinates(0, +1, -1), //East vector
        new HexCoordinates(-1, +1, 0), //North East Vector
        new HexCoordinates(-1, 0, +1), //North West Vector
        new HexCoordinates(0, -1, +1), //West Vector
        new HexCoordinates(+1, -1, 0), //South West Vector
        new HexCoordinates(+1, 0, -1), //South East Vector
    };



    public static Vector3 R_Direction = new Vector3(Mathf.Cos(5.0f / 3.0f * Mathf.PI), Mathf.Sin(5.0f / 3.0f * Mathf.PI), 0.0f);
    public static readonly Vector3 Q_Direction = new Vector3(1.0f, 0.0f, 0.0f);

    [SerializeField] int _r;
    [SerializeField] int _q;
    [SerializeField] int _s;

    public int R
    {
        get => _r;
        set
        {
            _r = value;
            var test = Vector3.zero;
        }
    }
    public int Q
    {
        get => _q;
        set
        {

        }
    }
    public int S
    {
        get => _s;
        set
        {

        }
    }

    #region Constructors

    public HexCoordinates()
    {
        _r = 0;
        _q = 0;
        _s = 0;
    }
    public HexCoordinates(int r, int q, int s)
    {
        if (r + q + s != 0) throw new System.Exception("Invalid coordinates : Sum must be 0");
        _r = r;
        _q = q;
        _s = s;
    }
    public HexCoordinates(int r, int q)
    {
        _r = r;
        _q = q;
        _s = -q - r;
    }
    #endregion

    public Vector3 GetVector3Position()
    {
        return
            R_Direction * R +
            Q_Direction * Q;
    }

    #region Static Methods
    public static HexCoordinates[] GetHexCircle(int radius)
    {
        HashSet<Tuple<int, int, int>> set = new HashSet<Tuple<int, int, int>>();
        for (int j = 0; j <= radius; j++)
        {
            Tuple<int, int, int> T1 = Tuple.Create(-radius, j, radius - j);
            Tuple<int, int, int> T2 = Tuple.Create(j, -radius, radius - j);
            Tuple<int, int, int> T3 = Tuple.Create(j, radius - j, -radius);
            Tuple<int, int, int> T4 = Tuple.Create(radius, -j, -radius + j);
            Tuple<int, int, int> T5 = Tuple.Create(-j, radius, -radius + j);
            Tuple<int, int, int> T6 = Tuple.Create(-j, -radius + j, radius);

            set.Add(T1);
            set.Add(T2);
            set.Add(T3);
            set.Add(T4);
            set.Add(T5);
            set.Add(T6);
        }

        Tuple<int, int, int>[] setArray = set.ToArray();
        HexCoordinates[] res = new HexCoordinates[setArray.Length];
        for (int i = 0; i < set.Count; i++) res[i] = new HexCoordinates(setArray[i].Item1, setArray[i].Item2, setArray[i].Item3);
        return res;
    }
    public static HexCoordinates VectorToPointyHex(Vector3 point, float cellSize) // The z coordinate is not used
    {
        float q = (Mathf.Sqrt(3) / 3 * point.x - 1.0f/ 3.0f * point.y) / cellSize;
        float r = (2.0f/ 3.0f * point.y) / cellSize;

        Vector3 coords = new Vector3(q, r, -q - r);

        return CubeRound(coords);
    }


    public static HexCoordinates CubeRound(Vector3 coord)
    {
        float q = Mathf.Round(coord.x);
        float r = Mathf.Round(coord.y);
        float s = Mathf.Round(coord.z);

        float qDiff = Mathf.Abs(q - coord.x);
        float rDiff = Mathf.Abs(r - coord.y);
        float sDiff = Mathf.Abs(s - coord.z);

        if (qDiff > rDiff || qDiff > sDiff) q = -r - s;
        else if (rDiff > sDiff) r = -q - s;
        else s = -q - r;

        return new HexCoordinates((int)r, (int)q);

    }


    #endregion

    #region Operator Overload
    public static HexCoordinates operator +(HexCoordinates A, HexCoordinates B)
    {
        return new HexCoordinates(A.R + B.R, A.Q + B.Q, A.S + B.S);
    }
    public static HexCoordinates operator -(HexCoordinates A, HexCoordinates B)
    {
        return new HexCoordinates(A.R - B.R, A.Q - B.Q, A.S - B.S);
    }
    public static HexCoordinates operator *(int k, HexCoordinates A)
    {
        return new HexCoordinates(k * A.R, k * A.Q, k * A.S);
    }
    public static HexCoordinates operator *(HexCoordinates A, int k)
    {
        return k * A;
    }


    public static bool operator ==(HexCoordinates A, HexCoordinates B)
    {
        return A.R == B.R && A.Q == B.Q;
    }

    public static bool operator !=(HexCoordinates A, HexCoordinates B)
    {
        return A.R != B.R || A.Q != B.Q;
    }

    public bool Equals(HexCoordinates other)
    {
        return this == other;
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return _r.GetHashCode() ^ (_q.GetHashCode() << 2);
    }
    #endregion
}
