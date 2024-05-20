using UnityEngine;

public class Vector3i {

    public int x, y, z;

    public Vector3i() {}
    public Vector3i(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3i(Vector3 vec)
    {
        this.x = (int)vec.x;
        this.y = (int)vec.y;
        this.z = (int)vec.z;
    }

    public bool Equals(Vector3i other)
    {
        return (x == other.x) && (y == other.y) && (z == other.z);
    }

    public override string ToString()
    {
        return "( " + x + " / " + y + " / " + z + " )";
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }


    // Operator Overloading

    public override bool Equals(System.Object obj)
    {
        // If parameter is null return false.
        if (obj == null)
        {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        Vector3i other = obj as Vector3i;
        if ((System.Object)other == null)
        {
            return false;
        }

        // Return true if the fields match:
        return (x == other.x) && (y == other.y) && (z == other.z);
    }

    public override int GetHashCode()
    {
        return new { x, y, z }.GetHashCode();
    }
}
